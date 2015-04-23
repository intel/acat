////////////////////////////////////////////////////////////////////////////
// <copyright file="EditTextControlAgent.cs" company="Intel Corporation">
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
using System.Windows.Forms;
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

namespace ACAT.Lib.Core.AgentManagement.TextInterface
{
    /// <summary>
    /// Text control for a Windows control that supports
    /// editing.  In Windows UI Automation terms, this is
    /// any control that supports TextPattern.  Example is a
    /// TextBox.  This allows us to track the cursor position,
    /// get text from the control etc.
    ///
    /// </summary>
    public class EditTextControlAgent : TextControlAgentBase
    {
        /// <summary>
        /// Handle of the active target window (eg the Notepad window)
        /// </summary>
        private readonly IntPtr _handle = IntPtr.Zero;

        /// <summary>
        /// Automation element representing the text element in the
        /// window
        /// </summary>
        private readonly AutomationElement _textElement;

        /// <summary>
        /// Handle to the target text control
        /// </summary>
        private IntPtr _handleTextWindow = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the class..
        /// </summary>
        /// <param name="handle">Handle to the target active window</param>
        /// <param name="editControlElement">The text control that is in focus</param>
        /// <param name="handled">set to true if this object knows how to
        ///                       handle the text control</param>
        public EditTextControlAgent(IntPtr handle, AutomationElement editControlElement, ref bool handled)
        {
            handled = trackTextChanges(handle, editControlElement);

            if (handled)
            {
                _handle = handle;
                _textElement = editControlElement;
            }
        }

        /// <summary>
        /// Clears text in the target text control
        /// </summary>
        public override void ClearText()
        {
            ClearText(_handleTextWindow);
        }

        /// <summary>
        /// Should abbreviations be expanded?
        /// </summary>
        /// <returns>true on success</returns>
        public override bool ExpandAbbreviations()
        {
            return true;
        }

        /// <summary>
        /// Gets the caret position in the target text control
        /// </summary>
        /// <returns>caret position, -1 on error</returns>
        public override int GetCaretPos()
        {
            return GetCaretPos(_handleTextWindow);
        }

        /// <summary>
        /// Returns highlighted text (if any)
        /// </summary>
        /// <returns></returns>
        public override String GetSelectedText()
        {
            return GetSelectedText(_handleTextWindow);
        }

        /// <summary>
        /// Gets the string of text from the target app's window
        /// </summary>
        /// <returns>text</returns>
        public override String GetText()
        {
            return (_handleTextWindow != IntPtr.Zero) ?
                    Windows.GetText(_handleTextWindow) :
                    String.Empty;
        }

        /// <summary>
        /// Indicates if text is highlighted in the window
        /// </summary>
        /// <returns></returns>
        public override bool IsTextSelected()
        {
            int start = -1;
            int end = -1;

            User32Interop.SendMessageRefRef(_handleTextWindow, User32Interop.EM_GETSEL, ref start, ref end);
            return start >= 0 && end >= 0 && start != end;
        }

        /// <summary>
        /// Invoked on a key up
        /// </summary>
        /// <param name="keyEventArgs">event arg</param>
        public override void OnKeyUp(KeyEventArgs keyEventArgs)
        {
            if (AgentUtils.IsPrintable(keyEventArgs.KeyCode))
            {
                triggerTextChanged(this);
            }
        }

        /// <summary>
        /// Sets the caret position in the output window
        /// </summary>
        /// <param name="pos">caret position</param>
        /// <returns>true on success</returns>
        public override bool SetCaretPos(int pos)
        {
            return SetCaretPos(_handleTextWindow, pos);
        }

        /// <summary>
        /// Sets focus to the target text control
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SetFocus()
        {
            return SetFocus(_handleTextWindow);
        }

        /// <summary>
        /// Does the text control support spell check?
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SupportsSpellCheck()
        {
            return false;
        }

        /// <summary>
        /// Un-highlights text if it is highlighted
        /// </summary>
        public override void UnselectText()
        {
            UnselectText(_handleTextWindow);
        }

        /// <summary>
        /// Invoked to dispose off the object
        /// </summary>
        protected override void OnDispose()
        {
            if (_textElement != null)
            {
                AutomationEventManager.RemoveAutomationEventHandler(_handle,
                                                            TextPattern.TextSelectionChangedEvent,
                                                            _textElement);
            }
        }

        /// <summary>
        /// Callback function invoked by Windows automation if there are any
        /// changes in the text control
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event args</param>
        private void onTextChanged(object sender, AutomationEventArgs e)
        {
            triggerTextChanged(this);
        }

        /// <summary>
        /// Tracks any changes in the target text window. Changes could be due
        /// to editing or even just a cursor movement.  Uses Windows automation to
        /// track editing changes and cursor movements
        /// </summary>
        /// <param name="handleMainWindow">Active target window</param>
        /// <param name="textElement">The text control</param>
        /// <returns>true on success</returns>
        private bool trackTextChanges(IntPtr handleMainWindow, AutomationElement textElement)
        {
            bool retVal = true;

            Log.Debug();

            try
            {
                object objPattern;
                if (textElement.TryGetCurrentPattern(TextPattern.Pattern, out objPattern))
                {
                    int nativeHandle = textElement.Current.NativeWindowHandle;
                    if (nativeHandle != 0)
                    {
                        _handleTextWindow = new IntPtr(nativeHandle);
                    }

                    AutomationEventManager.RemoveAutomationEventHandler(handleMainWindow,
                                                    TextPattern.TextSelectionChangedEvent,
                                                    textElement);
                    Log.Debug("Adding onTextChanged event handler");
                    AutomationEventManager.AddAutomationEventHandler(handleMainWindow,
                                                    TextPattern.TextSelectionChangedEvent,
                                                    textElement,
                                                    onTextChanged);

                    if (nativeHandle == 0)
                    {
                        Log.Debug("handle is zero");
                        retVal = false;
                    }
                }
                else
                {
                    Log.Debug("Focused element does not support textpattern");
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                // exception can be thrown by AddAutomationEventHandler to the effect that
                // WindowClosed event can only be attached to top level windows.
                // For instance, the "Start" menu would throw this exception.
                Log.Debug(ex.ToString());
                retVal = false;
            }

            if (!retVal)
            {
                _handleTextWindow = IntPtr.Zero;
            }

            return retVal;
        }
    }
}