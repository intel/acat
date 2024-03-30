////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIInterfaceUtils.cs
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.PanelManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    public class BCIInterfaceUtils
    {
        private static int CaretPositionPhraseTextBoxUC;
        private static int CaretPositionTextBoxUC;
        public const int CLEAR = 2;
        public const int EXIT_APP = 1;
        public const int MODE_CANNED = 5;
        public const int MODE_SENTENCE = 4;
        public const int MODE_SHORTHAND = 6;
        public const int NA = 0;
        public const int SAVE_TO_CANNED = 3;
        public const string ADDEDGELAUC = "Added gel to the electrodes \n or \n Reposition or removed the cap \n or \n Want to improve your earlier calibration ";
        public const string BACKTOMAINMODES = "Back";
        public const string CALIBRATE = "Calibrate";
        public const string CALIBRATEAGAIN = "Calibrate again";
        public const string CALIBRATIONEXPIRED = "Calibration Expired";
        public const string CALIBRATIONFAILEDAUC = "Calibration failed, \n Score: ";
        public const string CALIBRATIONMODE = "Calibration modes";
        public const string CALIBRATIONNEEDED = "Calibration needed";
        public const string CALIBRATIONNOTFOUND = "Not all required calibrations found";
        public const string CALIBRATIONSTATUS = "Calibration Status";
        public const string CHECKSIGNALS = "Check signals";
        public const string CLEARPROMPT = "Clear the talk window?";
        public const string EXITAPPLICATION = "Exit Application";
        public const string EXITPROMPT = "Exit the application?";
        public const string IMPROVECALIBRATION = "Want to improve your earlier calibration";
        public const string INITIALIZING = "Initializing....";
        public const string MODEROMPT = "Change mode to ";
        public const string RECALIBRATEIF = "Recalibrate if you";
        public const string SAVEPROMPT = "Save to Canned phrases?";
        public const string SELECTCALIBRATIONMODE = "Select calibration mode";
        public const string STARTINGIN = "Starting in ";
        public const string STARTYPING = "Start typing";
        public const string SUREEXITAPP = "Are you sure to exit the App?";

        /// <summary>
        /// Get the caret position for the Phrase TextBox UserControl
        /// </summary>
        /// <returns></returns>
        public static int GetCaretPhrasePositionOfTextBoxUC()
        {
            return CaretPositionPhraseTextBoxUC;
        }

        /// <summary>
        /// Get the caret position for the main TextBox UserControl
        /// </summary>
        /// <returns></returns>
        public static int GetCaretPositionOfTextBoxUC()
        {
            return CaretPositionTextBoxUC;
        }

        /// <summary>
        /// Saves the caret position for the Phrase TextBox UserControl
        /// </summary>
        /// <param name="caretPosition"></param>
        public static void SaveCaretPhrasePositionOfTextBoxUC(int caretPosition)
        {
            CaretPositionPhraseTextBoxUC = caretPosition;
        }

        /// <summary>
        /// Saves the caret position for the main TextBox UserControl
        /// </summary>
        /// <param name="caretPosition"></param>
        public static void SaveCaretPositionOfTextBoxUC(int caretPosition)
        {
            CaretPositionTextBoxUC = caretPosition;
        }

        /// <summary>
        /// Show Eyes closed calibration form
        /// </summary>
        /// <param name="form"></param>
        public static void ShowCalibrationEyesForm(Form form)
        {
            CalibrationEyesForm calibrationEyesForm = new CalibrationEyesForm();
            calibrationEyesForm.ShowDialog(form);
            calibrationEyesForm.Dispose();
        }

        /// <summary>
        /// Show the help info for calibration screen
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool ShowCalibrationHelpWindow(Form form)
        {
            return ConfirmBoxCalibrationHelp.ShowDialogHelp(form, true);
        }

        /// <summary>
        /// Show the calibration settings form
        /// </summary>
        /// <param name="actuatorResponse"></param>
        /// <param name="enbleBeginBtn"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Tuple<BCIMenuOptions.Options, BCISimpleParameters> ShowCalibrationModesWindow(BCICalibrationStatus actuatorResponse, bool enbleBeginBtn, Form form)
        {
            return ConfirmBoxCalibrationModes.ShowDialog(actuatorResponse, enbleBeginBtn, form, true);
        }

        /// <summary>
        /// Shows the result from calibration form
        /// </summary>
        /// <param name="titleMessage"></param>
        /// <param name="message"></param>
        /// <param name="form"></param>
        public static void ShowCalibrationResultWindow(string titleMessage, string message, Form form)
        {
            ConfirmBoxSingleOption.ShowDialog(titleMessage, message, "Ok", form);
        }

        /// <summary>
        /// Shows a message window asking if want to exit the main App
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool ShowExitAppWindow(Form form)
        {
            return ConfirmBox.ShowDialog(SUREEXITAPP, null, false);
        }

        /// <summary>
        /// Show the main options Form
        /// </summary>
        /// <param name="form"></param>
        /// <param name="title"></param>
        /// <param name="Label"></param>
        /// <param name="enableOp3"></param>
        /// <returns></returns>
        public static BCIMenuOptions.MainMenuOptions ShowMainOptionsWindow(Form form, string title, string Label, bool enableOp3)
        {
            return ConfirmBoxThreeOption.ShowDialog(title, Label, EXITAPPLICATION, CALIBRATE, STARTYPING, enableOp3, form);
        }

        /// <summary>
        /// Shows the More test available for BCI form
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static BCIMenuOptions.Options ShowMoreTestForm(Form form)
        {
            return OtherTestForm.ShowFormDialog(form);
        }

        /// <summary>
        /// Shows message box when is needed a recalibration
        /// </summary>
        /// <param name="form"></param>
        /// <param name="auc"></param>
        /// <returns></returns>
        public static BCIMenuOptions.MainMenuOptions ShowRecalibrationWindow(Form form, float auc)
        {
            return ConfirmBoxThreeOption.ShowDialog(CALIBRATIONSTATUS, CALIBRATIONFAILEDAUC + (auc * 100), EXITAPPLICATION, BACKTOMAINMODES, CALIBRATEAGAIN, true, form);
        }

        /// <summary>
        /// Shows the Remap calibrations Form
        /// </summary>
        /// <param name="form"></param>
        public static bool ShowRemapCalibrationsForm(Form form)
        {
            return RemapCalibrationForm.ShowFormDialog(form);
        }

        /// <summary>
        /// Shows a timed message box
        /// </summary>
        /// <param name="form"></param>
        public static void ShowTimedMessageBox(Form form)
        {
            ConfirmBoxTimer confirmBoxTimer2 = new ConfirmBoxTimer
            {
                Prompt = STARTINGIN,
                SecondsCounter = 3
            };
            confirmBoxTimer2.ShowDialog(form);
            confirmBoxTimer2.Dispose();
        }

        /// <summary>
        /// Show the trigger test form
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Tuple<BCIMenuOptions.Options, BCISimpleParameters> ShowTriggerTestSettingsForm(Form form)
        {
            return ConfirmBoxTriggerBoxSettings.ShowDialogForm(form);
        }
    }
}