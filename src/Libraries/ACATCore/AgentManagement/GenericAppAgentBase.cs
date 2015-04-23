////////////////////////////////////////////////////////////////////////////
// <copyright file="GenericAppAgentBase.cs" company="Intel Corporation">
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
using System.Windows.Automation;
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

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Base class for application agents.  Does most of the heavy lifting in
    /// the text agent aspect of application management.  Determines the type
    /// of the text control that is currently active and creates the appropriate
    /// text control agent to interact with the text window.
    /// If the application agent want to create its own text control agent,
    /// derive directly from AgentBase instead.
    /// </summary>
    public abstract class GenericAppAgentBase : AgentBase
    {
        /// <summary>
        /// Gets or sets the text control agent object
        /// </summary>
        protected TextControlAgentBase appTextInterface { get; set; }

        /// <summary>
        /// Implement this to display a contexutal menu for
        /// the currently active process
        /// </summary>
        /// <param name="monitorInfo">Info  about the active process/window</param>
        public abstract override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Invoked when active window/control focus changes in the active
        /// application. Creates a text control agent for the target
        /// control that's currently in focus.
        /// Set handled to true if this function handled the focus changed
        /// notification. Otherwise set it to false.
        ///
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="handled">was this handled?</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            disposeAndCreateTextInterface(monitorInfo);
            triggerTextChanged(appTextInterface);
            handled = true;
        }

        /// <summary>
        /// Invoked when this agent is going to be deactivated because
        /// there was a context switch to a different application.
        /// </summary>
        public override void OnFocusLost()
        {
            disposeTextInterface();
        }

        /// <summary>
        /// Creates an edit control text agent to manipulate text
        /// in the target text control that's currently active.
        /// the 'handled' param is set to true if it was
        /// handled successfully.
        /// </summary>
        /// <param name="handleMain">handle to the active window</param>
        /// <param name="focusedElement">the active text control</param>
        /// <param name="handled">true if handled</param>
        /// <returns>edit text control agent object</returns>
        protected virtual TextControlAgentBase createEditControlTextInterface(
                        IntPtr handleMain,
                        AutomationElement focusedElement,
                        ref bool handled)
        {
            Log.Debug();
            return new EditTextControlAgent(handleMain, focusedElement, ref handled);
        }

        /// <summary>
        /// Creates a key logger text interface which uses a shadow
        /// text box to manipulate text in the active window. Key
        /// logger is used for text controls which are NOT edit controls.
        /// We can't track the cursor or scrape text from the control
        /// </summary>
        /// <param name="handleMain">handle to the active window</param>
        /// <param name="editTextElement">the active text control</param>
        /// <returns>Key logger text agent</returns>
        protected virtual TextControlAgentBase createKeyLoggerTextInterface(
                        IntPtr handleMain,
                        AutomationElement editTextElement)
        {
            return new KeyLogTextControlAgent();
        }

        /// <summary>
        /// Disposer
        /// </summary>
        protected override void OnDispose()
        {
            disposeTextInterface();
        }

        /// <summary>
        /// Event handler that handles text changed event.  This handler
        /// is called whenver there are editing changes in the target
        /// text control or if the cursor moves in the target text window.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _textInterface_EvtTextChanged(object sender, TextChangedEventArgs e)
        {
            Log.Debug();
            if (e.TextInterface != null)
            {
                Log.Debug("Calling triggertextchanged");
                triggerTextChanged(e.TextInterface);
            }
        }

        /// <summary>
        /// Creates an edit control text agent to manipulate text
        /// in the target text control that's currently active.
        /// </summary>
        /// <param name="handleMain">handle to the active window</param>
        /// <param name="focusedElement">the active text control</param>
        /// <returns>edit text control agent object</returns>
        private TextControlAgentBase createEditControlTextInterface(
                        IntPtr handleMain,
                        AutomationElement focusedElement)
        {
            bool handled = false;
            Log.Debug("base.createEditControlTextInterface()");
            var textInterface = createEditControlTextInterface(handleMain, focusedElement, ref handled);
            if (handled)
            {
                return textInterface;
            }

            return null;
        }

        /// <summary>
        /// Disposes currently active text control agent and creates
        /// a new one
        /// </summary>
        /// <param name="monitorInfo">active window info</param>
        private void disposeAndCreateTextInterface(WindowActivityMonitorInfo monitorInfo)
        {
            disposeTextInterface();
            Log.Debug("Calling createEditControlTextInterface");
            var textInterface = createEditControlTextInterface(monitorInfo.FgHwnd, monitorInfo.FocusedElement) ??
                                createKeyLoggerTextInterface(monitorInfo.FgHwnd, monitorInfo.FocusedElement);

            appTextInterface = textInterface;
            appTextInterface.EvtTextChanged += _textInterface_EvtTextChanged;
            setTextInterface(appTextInterface);
        }

        /// <summary>
        /// Dispose currently active text control agent
        /// </summary>
        private void disposeTextInterface()
        {
            if (appTextInterface != null)
            {
                appTextInterface.EvtTextChanged -= _textInterface_EvtTextChanged;
                appTextInterface.Dispose();
                appTextInterface = null;
                setTextInterface();
            }
        }
    }
}