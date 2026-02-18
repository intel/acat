////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.Extensions;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Extension.UI
{
    /// <summary>
    /// Contains useful utility cover functions such as
    /// Yes/No confirmation dialogs, Timed dialogs, Toast
    /// Dialog, launching functional agents etc.
    /// </summary>
    public class DialogUtils
    {
        private static ILogger<DialogUtils> _logger;

        static DialogUtils()
        {
            _logger = LoggingConfiguration.CreateLogger<DialogUtils>();
        }

        /// <summary>
        /// Displays a yes no confirmation dialog box.
        /// </summary>
        /// <param name="caption">Prompt string</param>
        /// <param name="title">title of the dialog</param>
        /// <returns>true if yes</returns>
        public static bool Confirm(string caption, string title)
        {
            return Confirm(null, caption, title);
        }

        /// <summary>
        /// Displays a yes no confirmation dialog box
        /// </summary>
        /// <param name="caption">Prompt string</param>
        /// <returns>true if yes</returns>
        public static bool Confirm(string caption)
        {
            return Confirm(null, caption, null);
        }

        /// <summary>
        /// Displays a yes no confirmation dialog box
        /// </summary>
        /// <param name="parent">parent scanner</param>
        /// <param name="caption">prompt string</param>
        /// <param name="title">title of the dialog</param>
        /// <returns>true if yes</returns>
        public static bool Confirm(IPanel parent, string caption, string title = null)
        {
            bool retVal = false;

            Form form = initYesNoDialog(title, caption);
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(parent, form as IPanel);
                if (form is IExtension)
                {
                    ExtensionInvoker invoker = (form as IExtension).GetInvoker();
                    bool? yesNo = invoker != null && invoker.GetBoolValue("Choice").Value;
                    retVal = yesNo != null && yesNo.Value;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Displays yes/no confirmation as a contextual menu scanner
        /// </summary>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        public static bool ConfirmScanner(string caption)
        {
            return ConfirmScanner(null, caption);
        }

        /// <summary>
        /// Displays yes/no confirmation as a contextual menu scanner
        /// </summary>
        /// <param name="parent">parent scanner</param>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        public static bool ConfirmScanner(IPanel parent, string caption)
        {
            return showYesNoScanner(parent, "YesNoScanner", caption);
        }

        /// <summary>
        /// Displays yes/no confirmation as a narrow contextual menu scanner
        /// </summary>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        public static bool ConfirmScannerNarrow(string caption)
        {
            return ConfirmScannerNarrow(null, caption);
        }

        /// <summary>
        /// Displays yes/no confirmation as a narrow contextual menu scanner
        /// </summary>
        /// <param name="parent">parent scanner</param>
        /// <param name="caption">prompt string</param>
        /// <param name="title">scanner title</param>
        /// <returns>true if yes</returns>
        public static bool ConfirmScannerNarrow(IPanel parent, string caption, string title = null)
        {
            return showYesNoScanner(parent, "YesNoScannerNarrow", caption);
        }

        /// <summary>
        /// Runs the volume settings agent to control the volume of
        /// text to speech
        /// </summary>
        public static async void LaunchVolumeSettingsAgent()
        {
            IApplicationAgent volumeSettingsAgent = Context.AppAgentMgr.GetAgentByCategory("VolumeSettingsAgent");
            if (volumeSettingsAgent == null)
            {
                return;
            }

            await Context.AppAgentMgr.ActivateAgent(volumeSettingsAgent as IFunctionalAgent);
        }

        /// <summary>
        /// Displays the About box
        /// </summary>
        /// <param name="parentForm">parent form</param>
        /// <param name="appName">Name of the assembly</param>
        /// <param name="versionInfo">version information</param>
        /// <param name="companyInfo">company information</param>
        /// <param name="copyrightInfo">copyright info</param>
        /// <param name="attributions">3rd party attributions</param>
        public static void ShowAboutBox(Form parentForm, string appName,
                                        string versionInfo, string companyInfo, string copyrightInfo,
                                        IEnumerable<string> attributions)
        {
            if (parentForm is IPanel)
            {
                (parentForm as IPanel).OnPause();
            }
            parentForm.Invoke(new MethodInvoker(delegate
            {
                var dlg = new AboutBoxForm("About ACAT")
                {
                    AppName = appName,
                    VersionInfo = versionInfo,
                    UrlInfo = companyInfo,
                    CopyrightInfo = copyrightInfo,
                    Attributions = attributions
                };
                dlg.ShowDialog(parentForm);

                dlg.Dispose();
            }));

            if (parentForm is IPanel)
            {
                (parentForm as IPanel).OnResume();
            }
        }

        /// <summary>
        /// Activates the functional agent responsible for launching
        /// applications
        /// </summary>
        public static async void ShowAppLauncher()
        {
            try
            {
                if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
                {
                    return;
                }

                IApplicationAgent launchAppAgent = Context.AppAgentMgr.GetAgentByCategory("LaunchAppAgent");
                if (launchAppAgent == null)
                {
                    return;
                }

                await Context.AppAgentMgr.ActivateAgent(launchAppAgent as IFunctionalAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error launching app");
            }
        }

        /// <summary>
        /// Activates the File Browser functional agent.
        /// </summary>
        public static async void ShowFileBrowser(string action)
        {
            try
            {
                if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
                {
                    return;
                }

                IApplicationAgent fileBrowserAgent = Context.AppAgentMgr.GetAgentByCategory("FileBrowserAgent");
                if (fileBrowserAgent == null)
                {
                    return;
                }

                fileBrowserAgent.GetInvoker().SetValue("AutoLaunchFile", true);
                fileBrowserAgent.GetInvoker().SetValue("Action", action);

                await Context.AppAgentMgr.ActivateAgent(fileBrowserAgent as IFunctionalAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Activates the Switch Windows functional agent to enable the
        /// user to switch windows/apps.  If taskname
        /// is not null, it only shows windows belonging to the task (eg
        /// notepad, word)
        /// </summary>
        /// <param name="taskName">filter by this process name</param>
        public static async void ShowTaskSwitcher(string taskName = "")
        {
            try
            {
                if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
                {
                    return;
                }

                IApplicationAgent switchWindowsAgent = Context.AppAgentMgr.GetAgentByCategory("SwitchWindowsAgent");
                if (switchWindowsAgent == null)
                {
                    return;
                }

                IExtension extension = switchWindowsAgent;
                extension.GetInvoker().SetValue("FilterByProcessName", taskName);
                await Context.AppAgentMgr.ActivateAgent(switchWindowsAgent as IFunctionalAgent);
                _logger.LogDebug("Returned from activate agent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Displays the task switcher form which is the Alt-Tab
        /// equivalent to switch between application windows. If taskname
        /// is not null, it only shows windows belonging to the task (eg
        /// notepad, word)
        /// </summary>
        /// <param name="taskName">filter by this process name</param>
        public static void ShowTaskSwitcherAltTab(string taskName = "")
        {
            try
            {
                Form taskSwitcherForm = Context.AppPanelManager.CreatePanel("TaskSwitcherForm");
                if (taskSwitcherForm != null)
                {
                    if (!string.IsNullOrEmpty(taskName) && taskSwitcherForm is IExtension)
                    {
                        IExtension extension = taskSwitcherForm as IExtension;
                        extension.GetInvoker().SetValue("FilterProcessName", taskName);
                    }

                    Context.AppPanelManager.ShowDialog(taskSwitcherForm as IPanel);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating task switcher dialog. Exception: " + e);
            }
        }

        /// <summary>
        /// Displays a toast message centered in the parent form.
        /// </summary>
        /// <param name="message">message to display</param>
        /// <param name="timeout">how long to display</param>
        public static void Toast(string message, int timeout = 2000)
        {
            var toast = new ToastForm(message, timeout);
            var panel = Context.AppPanelManager.GetCurrentPanel() as Form;

            toast.StartPosition = FormStartPosition.CenterParent;
            toast.ShowDialog(panel);
            toast.Dispose();
        }

        /// <summary>
        /// Creates the Yes/No dialog form
        /// </summary>
        /// <param name="title">title of the dialog</param>
        /// <param name="caption">caption to display</param>
        /// <returns>The dialog form object</returns>
        private static Form initYesNoDialog(string title, string caption)
        {
            const string panelClass = "YesNoDialog";

            _logger.LogDebug("Creating panel " + panelClass);
            Form form = Context.AppPanelManager.CreatePanel(panelClass);
            if (form == null)
            {
                return null;
            }

            if (form is IExtension)
            {
                ExtensionInvoker invoker = (form as IExtension).GetInvoker();
                invoker.SetValue("TitleBar", string.IsNullOrEmpty(title) ? Common.AppPreferences.AppName : title);
                invoker.SetValue("Caption", caption);
            }

            return form;
        }

        /// <summary>
        /// Creates the Yes/No scanner form
        /// </summary>
        /// <param name="title">title of the dialog</param>
        /// <param name="caption">caption to display</param>
        /// <returns>The dialog form object</returns>
        private static Form initYesNoScanner(string panelClass, string title, string caption)
        {
            _logger.LogDebug("Creating panel " + panelClass);
            Form form = Context.AppPanelManager.CreatePanel(panelClass, title);
            if (form == null)
            {
                _logger.LogDebug("Unable to create panel " + panelClass);
                return null;
            }

            if (form is IExtension)
            {
                ExtensionInvoker invoker = (form as IExtension).GetInvoker();
                invoker.SetValue("Caption", caption);
            }

            return form;
        }

        /// <summary>
        /// Creates the yes/no scanner form and shows it as a dialog.
        /// Returns the result of the user choice
        /// </summary>
        /// <param name="parent">parent scanner</param>
        /// <param name="panelClass">The yes/no scanner class</param>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        private static bool showYesNoScanner(IPanel parent, string panelClass, string caption)
        {
            bool retVal = false;

            Form form = initYesNoScanner(panelClass, Common.AppPreferences.AppName, caption);
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(parent, form as IPanel);
                if (form is IExtension)
                {
                    ExtensionInvoker invoker = (form as IExtension).GetInvoker();
                    bool? yesNo = invoker != null && invoker.GetBoolValue("Choice").Value;
                    retVal = yesNo != null && yesNo.Value;
                }
            }

            return retVal;
        }
    }
}