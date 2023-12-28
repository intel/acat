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

#define ONBOARDING
//#define ENABLE_DIGITAL_VERIFICATION

using ACAT.ACATResources;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using ACATExtension.CommandHandlers;
using System;
using System.Windows.Forms;
#if ENABLE_DIGITAL_VERIFICATION
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
#endif

namespace ACAT.Applications.ACATTalk
{
    /// <summary>
    /// ACAT Talk is an application customized for conversations.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {
            if (AppCommon.OtherInstancesRunning())
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!validateACATCoreLibraryCertificates())
            {
                MessageBox.Show("Please reinstall ACAT and retry", "ACAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!checkFontsInstalled())
            {
                return;
            }

            CoreGlobals.AppId = "ACATTalk";
            CoreGlobals.ACATUserGuideFileName = "ACAT User Guide.pdf";

            FileUtils.LogAssemblyInfo();

            AppCommon.LoadGlobalSettings();

            AppCommon.SetUserName();
            AppCommon.SetProfileName();

            if (!AppCommon.CreateUserAndProfile())
            {
                return;
            }

            if (!AppCommon.LoadUserPreferences())
            {
                return;
            }

            DualMonitor.CheckAndDisplayScaleFactorWarning();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                User32Interop.SetProcessDPIAware();
            }

            Common.AppPreferences.AppName = "ACAT Talk";

            if (!AppCommon.SetCulture())
            {
                return;
            }

            Log.SetupListeners();

            AuditLog.Audit(new AuditEvent("Application", "start"));

            CommandDescriptors.Init();

            Common.AppPreferences.PreferredPanelConfigNames = String.Empty;

            // to enable onboarding, uncomment the #define ONBOARDING at the top of the file
            // to enable BCI, comment out the #define ONBOARDING statement and uncomment #define BCI
#if ONBOARDING

            if (!doOnboarding())
            {
                return;
            }

#elif BCI
            Common.AppPreferences.PreferredPanelConfigNames = "TalkApplicationBCI5x5";
#else
            Common.AppPreferences.PreferredPanelConfigNames = "TalkApplicationAbc";
#endif

            Splash splash = new Splash(2000);
            splash.Show();

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

                ConfirmBoxSingleOption.ShowDialog(Context.GetInitCompletionStatus(), "OK");
                if (Context.IsInitFatal())
                {
                    return;
                }
            }

            Context.ShowTalkWindowOnStartup = false;
            Context.AppAgentMgr.EnableContextualMenusForDialogs = false;
            Context.AppAgentMgr.EnableContextualMenusForMenus = false;
            Context.AppAgentMgr.DefaultAgentForContextSwitchDisable = Context.AppAgentMgr.NullAgent;

            if (splash != null)
            {
                splash.Close();
            }

            if (!Context.PostInit())
            {
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
                    MessageBox.Show(String.Format(R.GetString("InvalidFormName"), "TalkApplicationScanner"), R.GetString("Error"));
                    return;
                }
                AppCommon.ExitMessageShow();

                AuditLog.Audit(new AuditEvent("Application", "stop"));

                Context.Dispose();

                Common.Uninit();

                ScannerFocus.Stop();

                AppCommon.ExitMessageClose();

                Log.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            AppCommon.OnExit();
        }

        private static bool checkFontsInstalled()
        {
            string fontPath = SmartPath.ApplicationPath + "\\Assets\\Fonts";

            if (!FontCheck.IsMontserratFontInstalled())
            {
                MessageBox.Show("Montserrat fonts are not installed on this system.\nPlease install them and restart ACAT.\nThe fonts can be found here: " + fontPath,
                                    "ACAT",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                return false;
            }

            String fontName = "ACAT Font 1";
            if (!FontCheck.IsFontInstalled(fontName))
            {
                MessageBox.Show("Font \"" + fontName + "\" is not installed on this system.\nPlease install it and restart ACAT.\nThe font can be found here: " + fontPath,
                                    "ACAT",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                return false;
            }

            return true;
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

        private static bool doOnboarding()
        {
            var onboardingSequence = new OnboardingSequence();
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(new Guid("6d8da00e-5035-4b7f-a646-ed9f840a13bf")));
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(new Guid("301dbc87-c98c-491a-a2ee-d17863eab831")));
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(new Guid("65b95de3-bf5a-4ae8-b44d-f5e7950ab8d6")));
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(new Guid("e03754b3-85af-4f43-855e-47e20f7400c2")));

            var onboardingForm = new OnboardingForm();

            onboardingForm.Sequence = onboardingSequence;

            Application.Run(onboardingForm);

            Context.AppActuatorManager.Dispose();

            if (onboardingForm.QuitOnboarding)
            {
                return false;
            }

            return true;
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

#if ENABLE_DIGITAL_VERIFICATION
        private static bool validateCertificate(String dllPath)
        {
            try
            {
                Verify(dllPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Digital signature verification failed for the following DLL.\n\n" + dllPath + "\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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