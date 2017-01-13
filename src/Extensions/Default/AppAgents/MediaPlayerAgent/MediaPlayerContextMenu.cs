////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaPlayerContextMenu.cs" company="Intel Corporation">
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

using System;
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using ACAT.ACATResources;

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

namespace ACAT.Extensions.Default.AppAgents.MediaPlayer
{
    /// <summary>
    /// The contextual menu for the Windows Media Player Agent
    /// </summary>
    [DescriptorAttribute("1FEA0089-747A-4037-AA79-675D067FEA2C",
                            "MediaPlayerContextMenu",
                            "Media Player Contextual AppMenu")]
    public partial class MediaPlayerContextMenu : MenuPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Name of the scanner</param>
        /// <param name="panelTitle">Title of the menu</param>
        public MediaPlayerContextMenu(String panelClass, String panelTitle)
            : base(panelClass, R.GetString("MediaPlayer"))
        {
            WindowActivityMonitor.EvtWindowMonitorHeartbeat += WindowActivityMonitor_EvtWindowMonitorHeartbeat;
            Closing += MediaPlayerContextMenu_Closing;
        }

        /// <summary>
        /// Event handler for whan the menu form closes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void MediaPlayerContextMenu_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowActivityMonitor.EvtWindowMonitorHeartbeat -= WindowActivityMonitor_EvtWindowMonitorHeartbeat;
        }

        /// <summary>
        /// Event handler for the heartbeat event.  Check if the windows
        /// medial player is maximized, if so, hide the scanner
        /// </summary>
        /// <param name="monitorInfo">fg window info</param>
        private void WindowActivityMonitor_EvtWindowMonitorHeartbeat(WindowActivityMonitorInfo monitorInfo)
        {
            if ((monitorInfo.Title == R.GetString2("WindowsMediaPlayer") && Windows.IsMaximized(monitorInfo.FgHwnd)))
            {
                if (ScannerCommon != null)
                {
                    ScannerCommon.HideScannerOnIdle = MediaPlayerAgent.Settings.HideContextualMenuOnPlayerWindowMaximized;
                }
            }
            else
            {
                ScannerCommon.HideScannerOnIdle = CoreGlobals.AppPreferences.HideScannerOnIdle;
            }
        }
    }
}