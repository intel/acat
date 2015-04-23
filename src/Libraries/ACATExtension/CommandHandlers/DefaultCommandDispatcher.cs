////////////////////////////////////////////////////////////////////////////
// <copyright file="DefaultCommandDispatcher.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;

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
    /// This class takes care of most of the commands in ACAT.
    /// </summary>
    public class DefaultCommandDispatcher : RunCommandDispatcher
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="scanner"></param>
        public DefaultCommandDispatcher(IScannerPanel scanner)
            : base(scanner)
        {
            init();
        }

        /// <summary>
        /// Adds handlers for ACAT commands
        /// </summary>
        private void init()
        {
            Commands.Add(new ShowMainMenuHandler("CmdMainMenu"));
            Commands.Add(new ShowSettingsMenuHandler("CmdSettingsMenu"));
            Commands.Add(new SwitchWindowsHandler("CmdSwitchWindows"));
            Commands.Add(new ShowWindowsStartMenuHandler("WindowsStartMenu"));
            Commands.Add(new FileBrowserHandler("CmdFileBrowser"));
            Commands.Add(new SwitchAppsHandler("CmdSwitchApps"));
            Commands.Add(new ShowMuteScreenHandler("CmdMuteScreen"));
            Commands.Add(new ShowNewFileDialogHandler("CmdNewFileAgent"));
            Commands.Add(new LaunchAppHandler("CmdLaunchApp"));
            Commands.Add(new ShowAbbreviationsHandler("CmdShowAbbreviationSettings"));
            Commands.Add(new LectureManagerHandler("CmdLectureManager"));
            Commands.Add(new ContextMenuHandler("CmdContextMenu"));

            Commands.Add(new AppWindowManagementHandler("CmdCloseWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdMoveWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdSizeWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdMinimizeWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdMaxRestoreWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdMaximizeWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdRestoreWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdThreeFourthMaximizeWindow"));
            Commands.Add(new AppWindowManagementHandler("CmdMaximizeThreeFourthToggle"));

            Commands.Add(new TalkWindowHandler("CmdTalkWindowToggle"));
            Commands.Add(new TalkWindowHandler("CmdTalkWindowClear"));
            Commands.Add(new TalkWindowHandler("CmdTalkWindowClose"));

            Commands.Add(new FunctionKeyHandler("F1"));
            Commands.Add(new FunctionKeyHandler("F2"));
            Commands.Add(new FunctionKeyHandler("F3"));
            Commands.Add(new FunctionKeyHandler("F4"));
            Commands.Add(new FunctionKeyHandler("F5"));
            Commands.Add(new FunctionKeyHandler("F6"));
            Commands.Add(new FunctionKeyHandler("F7"));
            Commands.Add(new FunctionKeyHandler("F8"));
            Commands.Add(new FunctionKeyHandler("F9"));
            Commands.Add(new FunctionKeyHandler("F10"));
            Commands.Add(new FunctionKeyHandler("F11"));
            Commands.Add(new FunctionKeyHandler("F12"));

            Commands.Add(new ModifierKeyTriggerHandler("CmdShiftKey"));
            Commands.Add(new ModifierKeyTriggerHandler("CmdCtrlKey"));
            Commands.Add(new ModifierKeyTriggerHandler("CmdAltKey"));
            Commands.Add(new ModifierKeyTriggerHandler("CmdFunctionKey"));

            Commands.Add(new PositionScannerHandler("CmdAutoPositionScanner"));
            Commands.Add(new PositionScannerHandler("CmdPositionScannerTopRight"));
            Commands.Add(new PositionScannerHandler("CmdPositionScannerTopLeft"));
            Commands.Add(new PositionScannerHandler("CmdPositionScannerBottomRight"));
            Commands.Add(new PositionScannerHandler("CmdPositionScannerBottomLeft"));

            Commands.Add(new DocumentEditingHandler("CmdCut"));
            Commands.Add(new DocumentEditingHandler("CmdCopy"));
            Commands.Add(new DocumentEditingHandler("CmdPaste"));

            Commands.Add(new NavigationHandler("CmdPrevChar"));
            Commands.Add(new NavigationHandler("CmdNextChar"));
            Commands.Add(new NavigationHandler("CmdPrevLine"));
            Commands.Add(new NavigationHandler("CmdNextLine"));
            Commands.Add(new NavigationHandler("CmdPrevWord"));
            Commands.Add(new NavigationHandler("CmdNextWord"));
            Commands.Add(new NavigationHandler("CmdPrevPara"));
            Commands.Add(new NavigationHandler("CmdNextPara"));
            Commands.Add(new NavigationHandler("CmdPrevPage"));
            Commands.Add(new NavigationHandler("CmdNextPage"));
            Commands.Add(new NavigationHandler("CmdHome"));
            Commands.Add(new NavigationHandler("CmdEnd"));
            Commands.Add(new NavigationHandler("CmdTopOfDoc"));
            Commands.Add(new NavigationHandler("CmdEndOfDoc"));

            Commands.Add(new CreateAndShowScanner("CmdPunctuationScanner"));
            Commands.Add(new CreateAndShowScanner("CmdCursorScanner"));
            Commands.Add(new CreateAndShowScanner("CmdMouseScanner"));

            Commands.Add(new ZoomOperationsHandler("CmdZoomIn"));
            Commands.Add(new ZoomOperationsHandler("CmdZoomOut"));
            Commands.Add(new ZoomOperationsHandler("CmdZoomFit"));

            Commands.Add(new SendKeyHandler("CmdCapsLock"));
            Commands.Add(new SendKeyHandler("CmdNumLock"));
            Commands.Add(new SendKeyHandler("CmdScrollLock"));
            Commands.Add(new SendKeyHandler("CmdEnterKey"));

            Commands.Add(new MouseHandler("CmdRightClick"));
            Commands.Add(new MouseHandler("CmdLeftClick"));
            Commands.Add(new MouseHandler("CmdLeftDoubleClick"));
            Commands.Add(new MouseHandler("CmdLeftClickAndHold"));
            Commands.Add(new MouseHandler("CmdRightDoubleClick"));
            Commands.Add(new MouseHandler("CmdRightClickAndHold"));
            Commands.Add(new MouseHandler("CmdMoveCursorNW"));
            Commands.Add(new MouseHandler("CmdMoveCursorN"));
            Commands.Add(new MouseHandler("CmdMoveCursorNE"));
            Commands.Add(new MouseHandler("CmdMoveCursorW"));
            Commands.Add(new MouseHandler("CmdMoveCursorE"));
            Commands.Add(new MouseHandler("CmdMoveCursorSW"));
            Commands.Add(new MouseHandler("CmdMoveCursorS"));
            Commands.Add(new MouseHandler("CmdMoveCursorSE"));

            Commands.Add(new GoBackHandler("CmdGoBack"));

            Commands.Add(new ShowDialogsHandler("CmdShowGeneralSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowScanSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowWordPredictionSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowMouseRadarSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowMouseGridSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowVoiceSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowMuteScreenSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowDesignSettings"));
            Commands.Add(new ShowDialogsHandler("CmdShowAboutBox"));

            Commands.Add(new DocumentEditingHandler("CmdUndo"));
            Commands.Add(new DocumentEditingHandler("CmdRedo"));
            Commands.Add(new DocumentEditingHandler("CmdSelectModeToggle"));
            Commands.Add(new DocumentEditingHandler("CmdFind"));
            Commands.Add(new DocumentEditingHandler("CmdSelectAll"));
            Commands.Add(new DocumentEditingHandler("CmdDeletePrevChar"));
            Commands.Add(new DocumentEditingHandler("CmdDeleteNextChar"));
            Commands.Add(new DocumentEditingHandler("CmdDeletePrevWord"));

            Commands.Add(new ExitAppHandler("CmdExitAppWithConfirm"));
            Commands.Add(new ExitAppHandler("CmdExitApp"));
        }
    }
}