////////////////////////////////////////////////////////////////////////////
// <copyright file="ITalkWindow.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement;

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

namespace ACAT.Lib.Core.TalkWindowManagement
{
    /// <summary>
    /// Interface for the talk window.  The Talk Window allows
    /// the user to communicate by typing text and converting the
    /// text to speech
    /// </summary>
    public interface ITalkWindow : IPanel
    {
        /// <summary>
        /// The talk window form must raise this event if
        /// the user wants to close the talk window.  The
        /// Talk window manager will close the window
        /// </summary>
        event EventHandler EvtRequestCloseTalkWindow;

        /// <summary>
        /// Raised when the font changes
        /// </summary>
        event EventHandler EvtTalkWindowFontChanged;

        /// <summary>
        /// Gets the size of the font currently in use
        /// </summary>
        float FontSize { get; set; }

        /// <summary>
        /// Gets the talk window Form
        /// </summary>
        Form TalkWindowForm { get; }

        /// <summary>
        /// Gets the text in the talk window
        /// </summary>
        String TalkWindowText { get; set; }

        /// <summary>
        /// Gets the text box that contains the text
        /// used for communication. Every talk window MUST have
        /// a text box which is used for commmunication.
        /// </summary>
        Control TalkWindowTextBox { get; }

        /// <summary>
        /// Centers the talk window wrt to the screen
        /// </summary>
        void Center();

        /// <summary>
        /// Clears the text in the talk window
        /// </summary>
        void Clear();

        /// <summary>
        /// Copies the text in the talk window to the clipboard
        /// </summary>
        void Copy();

        /// <summary>
        /// Cuts the text in the talk window to the clipboard
        /// </summary>
        void Cut();

        /// <summary>
        /// Invoked when the position of the talk window changes
        /// </summary>
        void OnPositionChanged();

        /// <summary>
        /// Pastes text from the clipboard into the talk window
        /// </summary>
        void Paste();

        /// <summary>
        /// Selects all the text in the talk window
        /// </summary>
        void SelectAll();

        /// <summary>
        /// Restores default font size
        /// </summary>
        void ZoomDefault();

        /// <summary>
        /// Increases the font size in the talk window by a step
        /// </summary>
        void ZoomIn();

        /// <summary>
        /// Decreases the font size in the talk window by a step
        /// </summary>
        void ZoomOut();
    }
}