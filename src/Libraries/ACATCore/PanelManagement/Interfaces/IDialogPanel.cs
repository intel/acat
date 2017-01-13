////////////////////////////////////////////////////////////////////////////
// <copyright file="IDialogPanel.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.WidgetManagement;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// All ACAT Dialogs must implement this interface
    /// </summary>
    public interface IDialogPanel : IPanel
    {
        /// <summary>
        /// When a button on the dialog is actuated.  Any widget
        /// that derives from ButtonWidgetBase is considered a button.
        /// </summary>
        /// <param name="widget"></param>
        void OnButtonActuated(Widget widget);

        /// <summary>
        /// A command handler.  If the dialog doesn't recongnize
        /// the command, it sets 'handled' to false.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handled"></param>
        void OnRunCommand(string command, ref bool handled);
    }
}