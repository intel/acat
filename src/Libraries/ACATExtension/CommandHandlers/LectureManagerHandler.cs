////////////////////////////////////////////////////////////////////////////
// <copyright file="LectureManagerHandler.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Launches the Lecture manager
    /// </summary>
    public class LectureManagerHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public LectureManagerHandler(String cmd)
            : base(cmd)
        {
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="handled">set to true if the command was handled</param>
        /// <returns>true on success</returns>
        public override bool Execute(ref bool handled)
        {
            handled = true;

            Form form = Dispatcher.Scanner.Form;

            showLectureManager(form);

            return true;
        }

        /// <summary>
        /// Which file extensions to exclude from the file browser
        /// </summary>
        /// <returns>list of extensions to exclude</returns>
        private String[] getExcludeExtensions()
        {
            return (!String.IsNullOrEmpty(Common.AppPreferences.FileBrowserExcludeFileExtensions)) ?
                            Common.AppPreferences.FileBrowserExcludeFileExtensions.Split(';') : new String[] { };
        }

        /// <summary>
        /// Launches the lecture manager.  First launches the
        /// file browser to get lecture file and then launches lecture
        /// manager with the file.
        /// </summary>
        /// <param name="form">scanner form</param>
        private async void launchLectureManager(Form form)
        {
            // First launch the file browser to get
            // the file name from the user
            IApplicationAgent fileBrowserAgent = Context.AppAgentMgr.GetAgentByName("FileBrowser Agent");
            if (fileBrowserAgent == null)
            {
                return;
            }

            fileBrowserAgent.GetInvoker().SetValue("AutoLaunchFile", false);
            fileBrowserAgent.GetInvoker().SetValue("SelectActionOpen", true);
            fileBrowserAgent.GetInvoker().SetValue("Folders", Common.AppPreferences.GetFavoriteFolders());//.AppPreferences.FavoriteFolders.Split(';'));
            fileBrowserAgent.GetInvoker().SetValue("IncludeFileExtensions", new[] { "*.", "txt", "doc", "docx" });
            fileBrowserAgent.GetInvoker().SetValue("ExcludeFileExtensions", getExcludeExtensions());

            await Context.AppAgentMgr.ActivateAgent(fileBrowserAgent as IFunctionalAgent);

            String selectedFile = fileBrowserAgent.GetInvoker().GetStringValue("SelectedFile");

            if (!String.IsNullOrEmpty(selectedFile))
            {
                // now launch lecture manager for the selected file
                IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Lecture Manager Agent");
                if (agent != null)
                {
                    Windows.CloseForm(form);
                    IExtension extension = agent as IExtension;
                    extension.GetInvoker().SetValue("LoadFromFile", true);
                    extension.GetInvoker().SetValue("LectureFile", selectedFile);
                    Log.Debug("Invoking LectureManager agent");
                    await Context.AppAgentMgr.ActivateAgent(agent as IFunctionalAgent);
                    Log.Debug("Returned from LectureManager agent");
                }
            }
        }

        /// <summary>
        /// Closes the talk window. Checks if lecture manager
        /// is active, if so, shows it.  Else, launches a
        /// new instance
        /// </summary>
        /// <param name="form">scanner form</param>
        private void showLectureManager(Form form)
        {
            Context.AppTalkWindowManager.CloseTalkWindow();
            IntPtr handle = User32Interop.FindWindow(null, "Lecture Manager");
            if (handle != IntPtr.Zero)
            {
                User32Interop.SetForegroundWindow(handle);
            }
            else
            {
                launchLectureManager(form);
            }
        }
    }
}