////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using System;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// This class takes care of most of the commands in ACAT.  Any
    /// custom commands should be handled separately.
    /// </summary>
    public class DefaultCommandDispatcher : RunCommandDispatcher
    {
        protected RunCommands DefaultRunCommands;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="scanner"></param>
        public DefaultCommandDispatcher(IScannerPanel scanner)
            : base(scanner)
        {
            init();
        }

        public bool DefaultDispatch(String command, ref bool handled)
        {
            IRunCommandHandler runCommand = DefaultRunCommands.Get(command);

            bool retVal = runCommand != null && runCommand.Execute(ref handled);

            return retVal;
        }

        private void addHandler(RunCommandHandler runCommandHandler)
        {
            Commands.Add(runCommandHandler);
            DefaultRunCommands.Add(runCommandHandler);
        }

        /// <summary>
        /// Adds handlers for ACAT commands
        /// </summary>
        private void init()
        {
            DefaultRunCommands = new RunCommands(this);

            addHandler(new ShowMenusHandler("CmdMainMenu"));
            addHandler(new ShowMenusHandler("CmdSettingsMenu"));
            addHandler(new ShowMenusHandler("CmdToolsMenu"));

            addHandler(new ShowScreenLockHandler("CmdLockScreen"));
            addHandler(new ContextMenuHandler("CmdContextMenu"));

            addHandler(new AppWindowManagementHandler("CmdCloseWindow"));
            addHandler(new AppWindowManagementHandler("CmdMoveWindow"));
            addHandler(new AppWindowManagementHandler("CmdSizeWindow"));
            addHandler(new AppWindowManagementHandler("CmdMinimizeWindow"));
            addHandler(new AppWindowManagementHandler("CmdMaxRestoreWindow"));
            addHandler(new AppWindowManagementHandler("CmdMaximizeWindow"));
            addHandler(new AppWindowManagementHandler("CmdRestoreWindow"));
            addHandler(new AppWindowManagementHandler("CmdSnapWindow"));
            addHandler(new AppWindowManagementHandler("CmdSnapWindowToggle"));
            addHandler(new AppWindowManagementHandler("CmdDualMonitorMenu"));
            addHandler(new AppWindowManagementHandler("CmdMoveToOtherMonitor"));
            addHandler(new AppWindowManagementHandler("CmdMaxInOtherMonitor"));

            addHandler(new TalkWindowHandler("CmdTalkWindowShow"));
            addHandler(new TalkWindowHandler("CmdTalkWindowToggle"));
            addHandler(new TalkWindowHandler("CmdTalkWindowClear"));
            addHandler(new TalkWindowHandler("CmdTalkWindowClose"));
            addHandler(new TalkWindowHandler("CmdTalkApp"));

            addHandler(new FunctionKeyHandler("F1"));
            addHandler(new FunctionKeyHandler("F2"));
            addHandler(new FunctionKeyHandler("F3"));
            addHandler(new FunctionKeyHandler("F4"));
            addHandler(new FunctionKeyHandler("F5"));
            addHandler(new FunctionKeyHandler("F6"));
            addHandler(new FunctionKeyHandler("F7"));
            addHandler(new FunctionKeyHandler("F8"));
            addHandler(new FunctionKeyHandler("F9"));
            addHandler(new FunctionKeyHandler("F10"));
            addHandler(new FunctionKeyHandler("F11"));
            addHandler(new FunctionKeyHandler("F12"));

            addHandler(new ModifierKeyTriggerHandler("CmdShiftKey"));
            addHandler(new ModifierKeyTriggerHandler("CmdCtrlKey"));
            addHandler(new ModifierKeyTriggerHandler("CmdAltKey"));
            addHandler(new ModifierKeyTriggerHandler("CmdFunctionKey"));

            addHandler(new DocumentEditingHandler("CmdCut"));
            addHandler(new DocumentEditingHandler("CmdCopy"));
            addHandler(new DocumentEditingHandler("CmdPaste"));
            addHandler(new DocumentEditingHandler("CmdUndoLastEditChange"));
            addHandler(new DocumentEditingHandler("CmdDelPrevWord"));
            addHandler(new DocumentEditingHandler("CmdDelNextWord"));
            addHandler(new DocumentEditingHandler("CmdDelPrevSentence"));
            addHandler(new DocumentEditingHandler("CmdDelNextSentence"));
            addHandler(new DocumentEditingHandler("CmdBackSpace"));

            addHandler(new NavigationHandler("CmdPrevChar"));
            addHandler(new NavigationHandler("CmdNextChar"));
            addHandler(new NavigationHandler("CmdPrevLine"));
            addHandler(new NavigationHandler("CmdNextLine"));
            addHandler(new NavigationHandler("CmdPrevWord"));
            addHandler(new NavigationHandler("CmdNextWord"));
            addHandler(new NavigationHandler("CmdPrevPara"));
            addHandler(new NavigationHandler("CmdNextPara"));
            addHandler(new NavigationHandler("CmdPrevSentence"));
            addHandler(new NavigationHandler("CmdNextSentence"));
            addHandler(new NavigationHandler("CmdPrevPage"));
            addHandler(new NavigationHandler("CmdNextPage"));
            addHandler(new NavigationHandler("CmdHome"));
            addHandler(new NavigationHandler("CmdEnd"));
            addHandler(new NavigationHandler("CmdTopOfDoc"));
            addHandler(new NavigationHandler("CmdEndOfDoc"));

            addHandler(new CreateAndShowScanner("CmdPunctuationScanner"));
            addHandler(new CreateAndShowScanner("CmdCursorScanner"));
            addHandler(new CreateAndShowScanner("CmdMouseScanner"));
            addHandler(new CreateAndShowScanner("CmdNumberScanner"));
            addHandler(new CreateAndShowScanner("CmdFunctionKeyScanner"));

            addHandler(new SendKeyHandler("CmdCapsLock"));
            addHandler(new SendKeyHandler("CmdNumLock"));
            addHandler(new SendKeyHandler("CmdScrollLock"));
            addHandler(new SendKeyHandler("CmdEnterKey"));
            addHandler(new SendKeyHandler("CmdCommaKey"));
            addHandler(new SendKeyHandler("CmdPeriodKey"));

            addHandler(new MouseHandler("CmdRightClick"));
            addHandler(new MouseHandler("CmdLeftClick"));
            addHandler(new MouseHandler("CmdLeftDoubleClick"));
            addHandler(new MouseHandler("CmdLeftClickAndHold"));
            addHandler(new MouseHandler("CmdRightDoubleClick"));
            addHandler(new MouseHandler("CmdRightClickAndHold"));
            addHandler(new MouseHandler("CmdMoveCursorNW"));
            addHandler(new MouseHandler("CmdMoveCursorN"));
            addHandler(new MouseHandler("CmdMoveCursorNE"));
            addHandler(new MouseHandler("CmdMoveCursorW"));
            addHandler(new MouseHandler("CmdMoveCursorE"));
            addHandler(new MouseHandler("CmdMoveCursorSW"));
            addHandler(new MouseHandler("CmdMoveCursorS"));
            addHandler(new MouseHandler("CmdMoveCursorSE"));

            addHandler(new ShowDialogsHandler("CmdShowGeneralSettings"));
            addHandler(new ShowDialogsHandler("CmdShowScanSettings"));
            addHandler(new ShowDialogsHandler("CmdShowWordPredictionSettings"));
            addHandler(new ShowDialogsHandler("CmdShowMouseGridSettings"));
            addHandler(new ShowDialogsHandler("CmdShowVoiceSettings"));
            addHandler(new ShowDialogsHandler("CmdShowScreenLockSettings"));
            addHandler(new ShowDialogsHandler("CmdResizeRepositionScanner"));
            addHandler(new ShowDialogsHandler("CmdShowAboutBox"));

            addHandler(new DocumentEditingHandler("CmdUndo"));
            addHandler(new DocumentEditingHandler("CmdRedo"));
            addHandler(new DocumentEditingHandler("CmdSelectModeToggle"));
            addHandler(new DocumentEditingHandler("CmdFind"));
            addHandler(new DocumentEditingHandler("CmdSelectAll"));
            addHandler(new DocumentEditingHandler("CmdDeletePrevChar"));
            addHandler(new DocumentEditingHandler("CmdDeleteNextChar"));
            addHandler(new DocumentEditingHandler("CmdSmartDeletePrevWord"));

            addHandler(new GoBackHandler("CmdGoBack"));

            addHandler(new MiscCommandHandler("CmdSwitchLanguage"));
            addHandler(new MiscCommandHandler("CmdRestartScanning"));
            addHandler(new MiscCommandHandler("CmdExitAppWithConfirm"));
            addHandler(new MiscCommandHandler("CmdExitApp"));
        }
    }
}