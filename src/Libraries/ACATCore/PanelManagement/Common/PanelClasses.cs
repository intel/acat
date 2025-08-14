////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.PanelManagement.Common
{
    /// <summary>
    /// Categories of panels in ACAT
    /// </summary>
    public enum PanelCategory
    {
        Unknown = 0,

        /// <summary>
        /// Like the alphabet scanner, punctuations scanner
        /// </summary>
        Scanner = 1,

        /// <summary>
        /// Dialogs like the Settings dialogs
        /// </summary>
        Dialog = 2,

        /// <summary>
        /// Menus
        /// </summary>
        Menu = 3,
    }

    /// <summary>
    /// Represents types of common scanner types
    /// </summary>
    public class PanelClasses
    {
        /// <summary>
        /// The main Alphabet scanner
        /// </summary>
        public const string Alphabet = "Alphabet";

        /// <summary>
        /// The alphabet scanner without word prediction
        /// </summary>
        public const string AlphabetMinimal = "AlphabetMinimal";

        /// <summary>
        /// The Cursor navigation scanner
        /// </summary>
        public const string Cursor = "Cursor";

        /// <summary>
        /// Contexutal menus to interact with application dialogs. E.g. the
        /// Find dialog box when the user presses Ctrl-F in Notepad
        /// </summary>
        public const string DialogContextMenu = "DialogContextMenu";

        /// <summary>
        /// Scanner for function keys F1 through F12
        /// </summary>
        public const string FunctionKey = "FunctionKey";

        /// <summary>
        /// Contexutal menu to interact with application menus (E.g user right
        /// clicks in an application window and Windows displays a
        /// menu.
        /// </summary>
        public const string MenuContextMenu = "MenuContextMenu";

        /// <summary>
        /// The Mouse navigation scanner
        /// </summary>
        public const string Mouse = "Mouse";

        /// <summary>
        /// For uninitialized panels
        /// </summary>
        public const string None = "None";

        /// <summary>
        /// The Numbers scanner (equivalent to the numeric keypad)
        /// </summary>
        public const string Number = "Number";

        /// <summary>
        /// Scanner to enter Punctuations.
        /// </summary>
        public const string Punctuation = "Punctuation";
    }
}