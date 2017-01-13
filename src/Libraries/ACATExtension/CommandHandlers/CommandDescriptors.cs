////////////////////////////////////////////////////////////////////////////
// <copyright file="CommandDescriptors.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.CommandManagement;

namespace ACATExtension.CommandHandlers
{
    public class CommandDescriptors
    {
        private static CmdDescriptorTable _cmdDescriptorTable;

        public static CmdDescriptorTable CmdDescriptorTable
        {
            get { return _cmdDescriptorTable; }
        }

        public static void Dispose()
        {
        }

        public static bool Init()
        {
            createCmdDescriptorTable();

            return true;
        }

        /// <summary>
        /// Creates the table of commands and their descriptions
        /// </summary>
        private static void createCmdDescriptorTable()
        {
            _cmdDescriptorTable = new CmdDescriptorTable();

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdUndoLastEditChange", "Undo the last editing change", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdRestartScanning", "Restart scanning sequence", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdContextMenu", "Display the Contextual menu for the active application", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPhraseSpeak", "Select a phrase from a list of preferred phrases and convert it to speech", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdAutocompleteWithFirstWord", "Auto-complete by selecting the first word in the word list", true));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMainMenu", "Display the Main menu"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdSettingsMenu", "Display the Settings menu"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdToolsMenu", "Display the Tools Menu", true));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdSwitchApps", "Switch between windows of all active applications", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdSwitchWindows", "Switch between windows for the foreground application", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdFileBrowserFileOpen", "Display the ACAT File Browser to open files", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdFileBrowserFileDelete", "Display the ACAT File Browser to delete files"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdCreateFile", "Create a new text file or Word document"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdLockScreen", "Lock the display, use a PIN to unlock"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdLaunchApp", "Lanuch an application from a list of preferred applications", true));
            
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowAbbreviationSettings", "Display dialog to add/delete/update abbreviations"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdLectureManager", "Start Lecture Manager to deliver speeches"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdWindowPosSizeMenu", "Close the active application window", true));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdTalkWindowToggle", "Show/Hide the Talk window", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdTalkWindowClear", "Clear the text in the Talk window", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdTalkWindowShow", "Show the Talk window", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdTalkWindowClose", "Close the Talk window", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdTalkApp", "Show the Talk application window", true));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdAutoPositionScanner", "Select the position of the scanner window"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPositionScannerTopRight", "Move scanner window to the top right corner of the display"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPositionScannerTopLeft", "Move scanner window to the top left corner of the display"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPositionScannerBottomRight", "Move scanner to the bottom right corner of the display"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPositionScannerBottomLeft", "Move scanner to the bottom left corner of the display"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdScannerZoomIn", "Make the scanner window larger"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdScannerZoomOut", "Make the scanner window smaller"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdScannerZoomDefault", "Restore default scanner size"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdCut", "Cut selection to clipboard (Ctrl-X)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdCopy", "Copy selection to clipboard (Ctrl-C)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPaste", "Paste selection from clipboard (Ctrl-V"));

            
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdUndo", "Undo (Ctrl-Z)"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPrevChar", "Move cursor left (Left arrow)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdNextChar", "Move cursor right (Right arrow)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPrevLine", "Move cursor up (Up arrow)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdNextLine", "Move cursor down (Down arrow)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPrevWord", "Move cursor to start of the previous word"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdNextWord", "Move cursor to start of the next word"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPrevPara", "Move cursor to start of the previous paragraph"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdNextPara", "Move cursor to start of the next paragraph"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPrevPage", "Page Up"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdNextPage", "Page Down"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdHome", "The Home key"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdEnd", "The End key"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdTopOfDoc", "Go to the top of the document"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdEndOfDoc", "Go to the end of the document"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPunctuationScanner", "Display the punctuations scanner"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdCursorScanner", "Display the cursor navigation scanner"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMouseScanner", "Display the mouse scanner"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdNumberScanner", "Display the number scanner"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdFunctionKeyScanner", "Display the function key scanner"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdZoomIn", "Zoom In"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdZoomOut", "Zoom Out"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdZoomFit", "Zoom Fit"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdCapsLock", "Toggle Caps Lock"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdNumLock", "Toggle Num Lock"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdScrollLock", "Toggle Scroll Lock"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdEnterKey", "The ENTER key"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdCommaKey", "The comma key"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdPeriodKey", "The period key"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdRightClick", "Mouse right click"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdLeftClick", "Mouse left click"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdLeftDoubleClick", "Mouse left double click"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdLeftClickAndHold", "Mouse left click and hold"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdRightDoubleClick", "Mouse double click"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdRightClickAndHold", "Mouse right click and hold"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorNW", "Move mouse cursor NW"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorN", "Move mouse cursor N"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorNE", "Move mouse cursor NE"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorW", "Move mouse cursor W"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorE", "Move mouse cursor E"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorSW", "Move mouse cursor SW"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorS", "Move mouse cursor S"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdMoveCursorSE", "Move mouse cursor SE"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowGeneralSettings", "Display General Settings dialog"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowScanSettings", "Display Scan Settings dialog"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowWordPredictionSettings", "Display Word Prediciton Settings dialog"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowMouseGridSettings", "Display Mouse Settings dialog"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowVoiceSettings", "Display Text-to-speech Settings dialog"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowScreenLockSettings", "Display Screen Lock Settings dialog"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdResizeRepositionScanner", "Display Scanner size/position Settings dialog"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdShowAboutBox", "Display About box"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdSelectModeToggle", "Toggle selection mode"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdFind", "Find (Ctrl F)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdSelectAll", "Select all (Ctrl A)"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdDeletePrevChar", "Delete the previous character (Backspace)", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdDeleteNextChar", "Delete the next character (Del)"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdDeletePrevWord", "Delete the previous word", true));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdSwitchLanguage", "Display dialog to switch language"));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdExitAppWithConfirm", "Exit application after user confirmation"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdExitApp", "Exit application without confirmation"));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdGoBack", "Close scanner and go back to the previous scanner", true));

            _cmdDescriptorTable.Add(new CmdDescriptor("CmdScannerZoomIn", "Make the scanner larger", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdScannerZoomOut", "Make the scanner smaller", true));
            _cmdDescriptorTable.Add(new CmdDescriptor("CmdScannerZoomDefault", "Restore default scanner size", true));

            CommandManager.Instance.AppCommandTable = _cmdDescriptorTable;
        }
    }
}