////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkAppAgent.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
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

namespace ACAT.Applications.ACATTalk
{
    /// <summary>
    /// Application agent for the ACAT application.
    /// </summary>
    [DescriptorAttribute("AAF2B6C4-2F31-403D-BF45-7C35FA8B4FFC",
                            "TalkApp Agent",
                            "Application Agent for the ACAT Talk Application")]
    internal class TalkAppAgent : AgentBase
    {
        /// <summary>
        /// The text control agent responsbile for handling
        /// editing and caret movement functions
        /// </summary>
        private TextControlAgentBase _textInterface;

        private IntPtr _windowHandle = IntPtr.Zero;

        private Control textBoxControl;

        /// <summary>
        /// Gets the list of process supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo("blahblah") }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            arg.Handled = true;
            switch (arg.Widget.SubClass)
            {
                case "ClearTalkWindowText":
                    arg.Enabled = textBoxControl != null && (textBoxControl.Text.Length != 0);
                    break;

                default:
                    arg.Handled = false;
                    break;
            }
        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        /// <summary>
        /// Request for a contextual menu came in. If talk window is visible
        /// display the contextual menu for it.
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
        }

        /// <summary>
        /// Invoked when the foreground window focus changes. Create the Text
        /// control agent for the texbox in the talkwindow application window
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            _windowHandle = monitorInfo.FgHwnd;

            Log.Debug("window handle: " + _windowHandle);

            if (monitorInfo.IsNewFocusedElement || monitorInfo.IsNewWindow)
            {
                var automationElement = getTalkTextWinAutomationElement();
                if (automationElement != null)
                {
                    Log.Debug("found automationelement for the text box");

                    disposeTextInterface();
                    createTalkWindowTextInterface(monitorInfo.FgHwnd, automationElement);
                }
                else
                {
                    Log.Debug("DID NOT find automationelement for the text box");
                    textBoxControl = null;
                }
            }

            handled = true;
        }

        /// <summary>
        /// Focus shifted to an app not supported by this agent.
        /// Release resources
        /// </summary>
        public override void OnFocusLost()
        {
            disposeTextInterface();
        }

        /// <summary>
        /// Invoked on disposal. Cleanup
        /// </summary>
        protected override void OnDispose()
        {
            disposeTextInterface();
        }

        /// <summary>
        /// Event handler for when text or caret position in the text
        /// window changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _textInterface_EvtTextChanged(object sender, TextChangedEventArgs e)
        {
            triggerTextChanged(e.TextInterface);
        }

        /// <summary>
        /// Creates the text interface for the Talk window
        /// </summary>
        /// <param name="handle">Handle to the talk window</param>
        /// <param name="automationElement">talk window automation element</param>
        private void createTalkWindowTextInterface(IntPtr handle, AutomationElement automationElement)
        {
            disposeTextInterface();

            bool handled = false;
            _textInterface = new EditTextControlAgent(handle, automationElement, ref handled);
            _textInterface.EvtTextChanged += _textInterface_EvtTextChanged;
            setTextInterface(_textInterface);
            triggerTextChanged(_textInterface);
        }

        /// <summary>
        /// Disposes text interface
        /// </summary>
        private void disposeTextInterface()
        {
            if (_textInterface != null)
            {
                Log.Debug("Disposing old text interface");
                _textInterface.EvtTextChanged -= _textInterface_EvtTextChanged;
                _textInterface.Dispose();
                _textInterface = null;
                setTextInterface();
            }
        }

        /// <summary>
        /// Gets the automation element of the talk textbox in the main window
        /// </summary>
        /// <returns>the automation element, null if not found</returns>
        private AutomationElement getTalkTextWinAutomationElement()
        {
            if (_windowHandle == IntPtr.Zero)
            {
                return null;
            }

            var form = Form.FromHandle(_windowHandle);
            if (form != null)
            {
                var controls = GetAll(form, typeof(TextBox));
                if (controls != null)
                {
                    foreach (Control control in controls)
                    {
                        if (control.Name == "TextBoxTalkWindow")
                        {
                            textBoxControl = control;
                            return AutomationElement.FromHandle(control.Handle);
                        }
                    }
                }
            }

            return null;
        }
    }
}