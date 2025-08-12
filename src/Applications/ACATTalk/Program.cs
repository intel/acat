////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Program.cs
//
// Main entry point into the program. Does onboarding, initializes all
// the extensions and displays the main UI
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Applications;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACATResources;
using System;
using System.Windows.Forms;

#if ENABLE_DIGITAL_VERIFICATION
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
#endif

namespace ACATTalk
{
    /// <summary>
    /// ACAT Talk is an application customized for conversations.
    /// </summary>
    internal static class Program
    {
        private static Splash splash = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (AppCommon.OtherInstancesRunning())
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!validateACATCoreLibraryCertificates() || !validateConvAssistCertificate() || !validateACATWatchCertificate())
            {
                MessageBox.Show("Please reinstall ACAT and retry", "ACAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //if (!AppCommon.CheckFontsInstalled())
            //{
            //    return;
            //}

            CoreGlobals.AppId = "ACATTalk";
            CoreGlobals.ACATUserGuideFileName = "ACAT User Guide.pdf";
            FatalErrorHandler.EvtFatalError += CoreGlobals_EvtFatalError;

            FileUtils.LogAssemblyInfo();

            AppCommon.LoadGlobalSettings();

            AppCommon.SetUserName();
            AppCommon.SetProfileName();

            bool freshInstallForUser = !UserManager.UserExists(UserManager.CurrentUser);

            if (!AppCommon.CreateUserAndProfile())
            {
                return;
            }

            if (!AppCommon.LoadUserPreferences())
            {
                return;
            }
            if (!AppCommon.SetCulture())
            {
                return;
            }

            User32Interop.SetProcessDPIAware();

            AppCommon.CheckDisplayScalingAndResolution();

            Common.AppPreferences.AppName = "ACAT Talk";

            Log.SetupListeners();

            Log.Debug("ACAT Talk Application Launch");

            AuditLog.Audit(new AuditEvent("Application", "start"));

            // DOn't need this anymore...  Why was it here in the first place?
            //AppCommon.addBCIActuatorSetting();
            //AppCommon.addPanelClassConfigMapForBCI();

            CommandDescriptors.Init();

            Common.AppPreferences.PreferredPanelConfigNames = string.Empty;

            //if (!AppCommon.DoOnboarding())
            //{
            //    return;
            //}

            splash = new Splash(2000);
            splash.Show(StringResources.StartingACAT);

            Context.PreInit();
            Common.PreInit();
            Context.AppAgentMgr.EnableAppAgentContextSwitch = false;

            if (!Context.Init(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
                                Context.StartupFlags.WordPrediction |
                                Context.StartupFlags.AgentManager |
                                Context.StartupFlags.SpellChecker |
                                Context.StartupFlags.WindowsActivityMonitor |
                                Context.StartupFlags.Abbreviations))
            {
                splash.Close();
                splash = null;

                ConfirmBoxOneOption.ShowDialog(Context.GetInitCompletionStatus(), "", StringResources.OK);
                if (Context.IsInitFatal())
                {
                    return;
                }
            }

            Context.ShowTalkWindowOnStartup = false;
            Context.AppAgentMgr.EnableContextualMenusForDialogs = false;
            Context.AppAgentMgr.EnableContextualMenusForMenus = false;
            Context.AppAgentMgr.DefaultAgentForContextSwitchDisable = Context.AppAgentMgr.NullAgent;

            splash?.Close();

            splash = null;

            if (!Context.PostInit())
            {
                Context.Dispose();
                return;
            }

            Common.Init();

            Context.AppWindowPosition = Windows.WindowPosition.CenterScreen;

            AuditLog.Audit(new AuditEvent("Application", "Initialiation complete"));

            try
            {
                Context.AppActuatorManager.ShowTryoutDialog(true);

                showTalkInterfaceDescription();

                var startupArg = new StartupArg("TalkApplicationScanner")
                {
                    QuitAppOnFormClose = false
                };

                var form = PanelManager.Instance.CreatePanel("TalkApplicationScanner", startupArg);
                if (form != null)
                {
                    // Add ad-hoc agent that will handle the form
                    IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Talk Application Agent");
                    if (agent == null)
                    {
                        MessageBox.Show("Could not find application agent for this application.");
                        return;
                    }

                    Context.AppAgentMgr.AddAgent(form.Handle, agent);
                    Context.AppPanelManager.ShowDialog(form as IPanel);
                }
                else
                {
                    MessageBox.Show(string.Format(StringResources.InvalidFormName, "TalkApplicationScanner"));
                    return;
                }

                splash = new Splash();
                splash.Show(StringResources.ExitingACAT);
                AuditLog.Audit(new AuditEvent("Application", "stop"));

                Context.Dispose();

                Common.Uninit();

                ScannerFocus.Stop();

                splash?.Close();
                splash = null;

                Log.Debug("ACATTalk Application shutdown");

                Log.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            AppCommon.OnExit();
        }

        /// <summary>
        /// A fatal error has occurred.  Try and gracefully exit ACAT
        /// </summary>
        /// <param name="reason"></param>
        private static void CoreGlobals_EvtFatalError(string reason)
        {
            splash?.Close();

            ScannerFocus.Stop();

            if (Context.AppPanelManager != null && Context.AppPanelManager.GetCurrentForm() != null &&
                Context.AppPanelManager.GetCurrentForm().PanelCommon != null && Context.AppPanelManager.GetCurrentForm().PanelCommon.RootWidget != null)
            {
                Context.AppPanelManager.GetCurrentForm().OnPause();
                var form = Context.AppPanelManager.GetCurrentForm().PanelCommon.RootWidget.UIControl as Form;
                ConfirmBoxLargeSingleOption.ShowDialog(reason, "OK", form);
            }
            else
            {
                ConfirmBoxLargeSingleOption.ShowDialog(reason, "OK");
            }

            Application.ExitThread();

            Environment.FailFast(reason);
        }

        private static void showTalkInterfaceDescription()
        {
            if (!Common.AppPreferences.ShowTalkInterfaceDescOnStartup)
            {
                return;
            }

            var form = PanelManager.Instance.CreatePanel("DefaultInterfaceScanner", "ACAT Talk Description");
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(form as IPanel);
            }
        }



        private static bool validateACATCoreLibraryCertificates()
        {
#if ENABLE_DIGITAL_VERIFICATION

            String [] listOfDlls = { "ACATCore.dll", "ACATExtension.dll", "ACATResources.dll", "AppCommon.dll"};
            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            foreach (var dll in listOfDlls)
            {
                var dllPath = Path.Combine(appPath, "SharedLibs", dll);
                if (!validateCertificate(dllPath))
                {
                    return false;
                }
            }

            return true;
#else
            return true;
#endif
        }

        private static bool validateConvAssistCertificate()
        {
#if ENABLE_DIGITAL_VERIFICATION

            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var convAssistPath = Path.Combine(appPath, "ConvAssistApp", "ConvAssist.exe");
            if (!validateCertificate(convAssistPath))
            {
                return false;
            }

            return true;
#else
            return true;
#endif
        }

        private static bool validateACATWatchCertificate()
        {
#if ENABLE_DIGITAL_VERIFICATION

            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var acatWatchPath = Path.Combine(appPath, "ACATWatch.exe");
            if (!validateCertificate(acatWatchPath))
            {
                return false;
            }

            return true;
#else
            return true;
#endif
        }

#if ENABLE_DIGITAL_VERIFICATION
        private static bool validateCertificate(String filePath)
        {
            try
            {
                Verify(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Digital signature verification failed for the following file.\n\n" + filePath + "\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            return true;
        }

        private static void Verify(String fileName)
        {
            IntPtr certStore = IntPtr.Zero;
            IntPtr msgHandle = IntPtr.Zero;
            IntPtr context = IntPtr.Zero;
            int msgAndCertEncodingType = 0;
            int contentType = 0;
            int formatType = 0;
            const int ErrCertExpired = -2146762495;

            if (!CryptoInterop.CryptQueryObject(
                CryptoInterop.CERT_QUERY_OBJECT_FILE,
                fileName,
                CryptoInterop.CERT_QUERY_CONTENT_FLAG_ALL,
                CryptoInterop.CERT_QUERY_FORMAT_FLAG_ALL,
                0,
                ref msgAndCertEncodingType,
                ref contentType,
                ref formatType,
                ref certStore,
                ref msgHandle,
                ref context
            ))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            int data = 0;
            if (!CryptoInterop.CryptMsgGetParam(msgHandle, CryptoInterop.CMSG_ENCODED_MESSAGE, 0, null, ref data))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            byte[] pvData = new byte[data];
            CryptoInterop.CryptMsgGetParam(msgHandle, CryptoInterop.CMSG_ENCODED_MESSAGE, 0, pvData, ref data);
            var signedCms = new SignedCms();
            signedCms.Decode(pvData);
            try
            {
                signedCms.CheckSignature(false);
            }
            catch (Exception e)
            {
                if (e.HResult != ErrCertExpired)
                {
                    throw (e);
                }
            }
            finally
            {
                CryptoInterop.CryptMsgClose(msgHandle);
                CryptoInterop.CertCloseStore(certStore, 0);
            }
        }

#endif
    }
}