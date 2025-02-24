////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Program.cs
//
// Entry point into the app that enables the user to configure ACAT
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Applications;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using ACATExtension.CommandHandlers;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Windows.Forms;

namespace ACATConfig
{
    /// <summary>
    /// Entry point into the app that enables the user to configure ACAT
    /// </summary>
    internal static class Program
    {
        private static Splash splash = null;

        /// <summary>
        /// Event handler to indicate language changed. Save
        /// the settings.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private static void form_EvtLanguageChanged(object sender, ACATConfigMainForm.PreferencesLanguageChanged arg)
        {
            Common.AppPreferences.Language = arg.CI.Name;
            ResourceUtils.SetCulture(Common.AppPreferences.Language);
            if (arg.IsDefault)
            {
                Common.AppPreferences.Save();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
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

            if (!AppCommon.CheckFontsInstalled())
            {
                return;
            }

            CoreGlobals.AppId = "ACATConfig";

            CoreGlobals.EvtFatalError += CoreGlobals_EvtFatalError;

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

            User32Interop.SetProcessDPIAware();

            AppCommon.CheckDisplayScalingAndResolution();

            Log.SetupListeners();

            CommandDescriptors.Init();

            if (!AppCommon.SetCulture())
            {
                return;
            }

            Common.PreInit();
            Common.Init();

            splash = new Splash(1000);
            splash.Show();

            splash.Close();

            var form = new ACATConfigMainForm();
            form.EvtLanguageChanged += form_EvtLanguageChanged;
            Application.Run(form);
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

        private static bool validateACATCoreLibraryCertificates()
        {
#if ENABLE_DIGITAL_VERIFICATION

            String[] listOfDlls = { "ACATCore.dll", "ACATExtension.dll", "ACATResources.dll", "AppCommon.dll" };
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