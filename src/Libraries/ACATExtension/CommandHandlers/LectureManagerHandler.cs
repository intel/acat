////////////////////////////////////////////////////////////////////////////
// <copyright file="LectureManagerHandler.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;
using ACAT.ACATResources;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Launches the Lecture manager functional agent
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

            if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
            {
                return false;
            }

            Form form = Dispatcher.Scanner.Form;

            showLectureManager(form);

            return true;
        }

        /// <summary>
        /// Launches the lecture manager.  First launches the
        /// file browser to get lecture file and then launches lecture
        /// manager with the file.  Only text files and word doc files
        /// are supported.
        /// </summary>
        /// <param name="form">scanner form</param>
        private async void launchLectureManager(Form form)
        {
            // First launch the file browser to get
            // the file name from the user
            IApplicationAgent fileBrowserAgent = Context.AppAgentMgr.GetAgentByCategory("FileBrowserAgent");
            if (fileBrowserAgent == null)
            {
                return;
            }

            fileBrowserAgent.GetInvoker().SetValue("AutoLaunchFile", false);
            fileBrowserAgent.GetInvoker().SetValue("Action", "Open");
            fileBrowserAgent.GetInvoker().SetValue("IncludeFileExtensions", new[] { "txt", "doc", "docx" });

            await Context.AppAgentMgr.ActivateAgent(fileBrowserAgent as IFunctionalAgent);

            String selectedFile = fileBrowserAgent.GetInvoker().GetStringValue("SelectedFile");

            if (!String.IsNullOrEmpty(selectedFile))
            {
                // now launch lecture manager for the selected file
                IApplicationAgent agent = Context.AppAgentMgr.GetAgentByCategory("LectureManagerAgent");
                if (agent != null)
                {
                    Windows.CloseForm(form);
                    IExtension extension = agent;
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
            IntPtr handle = User32Interop.FindWindow(null, R.GetString("LectureManager"));
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