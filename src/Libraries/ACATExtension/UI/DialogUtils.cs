////////////////////////////////////////////////////////////////////////////
// <copyright file="DialogUtils.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Contains useful utility cover functions such as
    /// Yes/No confirmation dialogs, Timed dialogs, Toast
    /// Dialog, launching functional agents etc.
    /// </summary>
    public class DialogUtils
    {
        /// <summary>
        /// Displays a yes no confirmation dialog box.
        /// </summary>
        /// <param name="caption">Prompt string</param>
        /// <param name="title">title of the dialog</param>
        /// <returns>true if yes</returns>
        public static bool Confirm(String caption, String title)
        {
            return Confirm(null, caption, title);
        }

        /// <summary>
        /// Displays a yes no confirmation dialog box
        /// </summary>
        /// <param name="caption">Prompt string</param>
        /// <returns>true if yes</returns>
        public static bool Confirm(String caption)
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
        public static bool Confirm(IPanel parent, String caption, String title = null)
        {
            bool retVal = false;

            Form form = initYesNoDialog(title, caption);
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(parent, form as IPanel);
                if (form is IExtension)
                {
                    var invoker = (form as IExtension).GetInvoker();
                    bool? yesNo = invoker != null ? invoker.GetBoolValue("Choice").Value : false;
                    retVal = (yesNo != null) ? yesNo.Value : false;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Displays yes/no confirmation as a contextual menu scanner
        /// </summary>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        public static bool ConfirmScanner(String caption)
        {
            return ConfirmScanner(null, caption);
        }

        /// <summary>
        /// Displays yes/no confirmation as a contextual menu scanner
        /// </summary>
        /// <param name="parent">parent scanner</param>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        public static bool ConfirmScanner(IPanel parent, String caption)
        {
            return showYesNoScanner(parent, "YesNoScanner", caption);
        }

        /// <summary>
        /// Displays yes/no confirmation as a narrow contextual menu scanner
        /// </summary>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        public static bool ConfirmScannerNarrow(String caption)
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
        public static bool ConfirmScannerNarrow(IPanel parent, String caption, String title = null)
        {
            return showYesNoScanner(parent, "YesNoScannerNarrow", caption);
        }

        /// <summary>
        /// Runs the volume settings agent to control the volume of
        /// text to speech
        /// </summary>
        public static async void LaunchVolumeSettingsAgent()
        {
            var volumeSettingsAgent = Context.AppAgentMgr.GetAgentByCategory("VolumeSettingsAgent");
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
        /// <param name="logo">filename of the logo to display</param>
        /// <param name="appName">Name of the assembly</param>
        /// <param name="versionInfo">version information</param>
        /// <param name="companyInfo">company information</param>
        /// <param name="copyrightInfo">copyright info</param>
        /// <param name="attributions">3rd party attributions</param>
        public static void ShowAboutBox(Form parentForm, String logo, String appName,
                                        String versionInfo, String companyInfo, String copyrightInfo,
                                        IEnumerable<String> attributions)
        {
            parentForm.Invoke(new MethodInvoker(delegate
            {
                Form dlg = Context.AppPanelManager.CreatePanel("AboutBoxForm");
                if (dlg is IExtension)
                {
                    ExtensionInvoker invoker = (dlg as IExtension).GetInvoker();
                    invoker.SetValue("Logo", logo);
                    invoker.SetValue("AppName", appName);
                    invoker.SetValue("VersionInfo", versionInfo);
                    invoker.SetValue("CompanyInfo", companyInfo);
                    invoker.SetValue("CopyrightInfo", copyrightInfo);
                    invoker.SetValue("Attributions", attributions);
                    invoker.SetValue("ShowButton", true);
                    Context.AppPanelManager.ShowDialog(dlg as IPanel);
                }
            }));
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
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Activates the File Browser functional agent.
        /// </summary>
        public static async void ShowFileBrowser(String action)
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
                Log.Debug(ex.ToString());
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
                Log.Debug("Returned from activate agent");
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Displays the task switcher form which is the Alt-Tab
        /// equivalent to switch between application windows. If taskname
        /// is not null, it only shows windows belonging to the task (eg
        /// notepad, word)
        /// </summary>
        /// <param name="taskName">filter by this process name</param>
        public static void ShowTaskSwitcherAltTab(String taskName = "")
        {
            try
            {
                //Context.AppTalkWindowManager.CloseTalkWindow();
                Form taskSwitcherForm = Context.AppPanelManager.CreatePanel("TaskSwitcherForm");
                if (taskSwitcherForm != null)
                {
                    if (!String.IsNullOrEmpty(taskName) && taskSwitcherForm is IExtension)
                    {
                        IExtension extension = taskSwitcherForm as IExtension;
                        extension.GetInvoker().SetValue("FilterProcessName", taskName);
                    }

                    Context.AppPanelManager.ShowDialog(taskSwitcherForm as IPanel);
                }
            }
            catch (Exception e)
            {
                Log.Debug("Error creating task switcher dialog. Exception: " + e);
            }
        }

        /// <summary>
        /// Shows the TimedDialog window
        /// </summary>
        /// <param name="parentForm">parent form</param>
        /// <param name="message">message to display</param>
        public static void ShowTimedDialog(Form parentForm, String message)
        {
            ShowTimedDialog(parentForm, "Message", message);
        }

        /// <summary>
        /// Shows the TimedDialog window
        /// </summary>
        /// <param name="parentForm">parent form</param>
        /// <param name="title">title of the dialog</param>
        /// <param name="message">message to display</param>
        public static void ShowTimedDialog(Form parentForm, String title, String message)
        {
            if (parentForm != null)
            {
                parentForm.Invoke(new MethodInvoker(delegate
                {
                    showTimedDialog(title, message);
                }));
            }
            else
            {
                showTimedDialog(title, message);
            }
        }

        /// <summary>
        /// Displays a toast message centered in the parent form.
        /// </summary>
        /// <param name="message">message to display</param>
        /// <param name="timeout">how long to display</param>
        public static void Toast(String message, int timeout = 2000)
        {
            var toast = new ToastForm(message, timeout);
            var panel = Context.AppPanelManager.GetCurrentPanel() as Form;

            toast.StartPosition = FormStartPosition.CenterParent;
            toast.ShowDialog(panel);
        }

        /// <summary>
        /// Creates the Yes/No dialog form
        /// </summary>
        /// <param name="title">title of the dialog</param>
        /// <param name="caption">caption to display</param>
        /// <returns>The dialog form object</returns>
        private static Form initYesNoDialog(String title, String caption)
        {
            const String panelClass = "YesNoDialog";

            Log.Debug("Creating panel " + panelClass);
            Form form = Context.AppPanelManager.CreatePanel(panelClass);
            if (form == null)
            {
                return null;
            }

            if (form is IExtension)
            {
                var invoker = (form as IExtension).GetInvoker();
                invoker.SetValue("TitleBar", String.IsNullOrEmpty(title) ? Common.AppPreferences.AppName : title);
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
        private static Form initYesNoScanner(String panelClass, String title, String caption)
        {
            Log.Debug("Creating panel " + panelClass);
            Form form = Context.AppPanelManager.CreatePanel(panelClass, title);
            if (form == null)
            {
                Log.Debug("Unable to create panel " + panelClass);
                return null;
            }

            if (form is IExtension)
            {
                var invoker = (form as IExtension).GetInvoker();
                invoker.SetValue("Caption", caption);
            }

            return form;
        }

        /// <summary>
        /// Shows the TimedDialog window
        /// </summary>
        /// <param name="title">title of the dialog</param>
        /// <param name="message">message to display</param>
        private static void showTimedDialog(String title, String message)
        {
            Form dlg = Context.AppPanelManager.CreatePanel("TimedDialogForm");
            if (dlg is IExtension)
            {
                ExtensionInvoker invoker = (dlg as IExtension).GetInvoker();
                invoker.SetValue("MessageText", message);
                invoker.SetValue("TitleText", title);
                invoker.SetValue("ShowButton", true);
                Context.AppPanelManager.ShowDialog(dlg as IPanel);
            }
        }

        /// <summary>
        /// Creates the yes/no scanner form and shows it as a dialog.
        /// Returns the result of the user choice
        /// </summary>
        /// <param name="parent">parent scanner</param>
        /// <param name="panelClass">The yes/no scanner class</param>
        /// <param name="caption">prompt string</param>
        /// <returns>true if yes</returns>
        private static bool showYesNoScanner(IPanel parent, String panelClass, String caption)
        {
            bool retVal = false;

            Form form = initYesNoScanner(panelClass, Common.AppPreferences.AppName, caption);
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(parent, form as IPanel);
                if (form is IExtension)
                {
                    var invoker = (form as IExtension).GetInvoker();
                    bool? yesNo = invoker != null ? invoker.GetBoolValue("Choice").Value : false;
                    retVal = (yesNo != null) ? yesNo.Value : false;
                }
            }

            return retVal;
        }
    }
}