////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Text;
using ACAT.Lib.Core.PreferencesManagement;
using System.Xml.Serialization;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>*
    /// Contains system-wide preference settings.  Settings are serialized
    /// into a file for saving and loaded from the file.
    /// Be careful about renaming variables in this as the variable names
    /// are used as the setting names in the saved XML file.
    /// </summary>
    [Serializable]
    public abstract class Preferences : PreferencesBase
    {
        [NonSerialized, XmlIgnore]
        public static Assembly ApplicationAssembly;

        [NonSerialized, XmlIgnore]
        public String AppName = "ACAT";

        // Scanner settings
        [IntDescriptor("First repeat time for sticky buttons (in msecs)", 200, 3000)]
        public int FirstRepeatTime = 1000;

        [IntDescriptor("Scan time (in msecs)", 100, 3000)]
        public int ScanTime = 1000;

        [IntDescriptor("Extra time to pause on the first row/column/button (in msecs)", 0, 3000)]
        public int FirstPauseTime = 250;

        [BoolDescriptor("Play a beep on a selection")]
        public bool SelectClick = false;

        public float ScannerScaleFactor = 10.0f;

        public String FontName = "Arial";
        public int FontSize = 18;

        public String Theme = "Default";

        [BoolDescriptor("Include disabled buttons in the scanning cycle", true)]
        public bool ScanDisabledElements = true;

        public Windows.WindowPosition ScannerPosition = Windows.WindowPosition.MiddleRight;

        public String PreferredPanelConfigNames = "AlphabetQwerty";

        [IntDescriptor("Minimum hold time for the actuator switch to trigger (in msecs)", 0, 2000)]
        public int MinActuationHoldTime = 0;

        [BoolDescriptor("Expand an abbreviation only if a space, comma or a period is inserted after the abbreviation")]
        public bool ExpandAbbreviationsOnSeparator = false;

        [BoolDescriptor("Log application messages to a file. Use with caution. This will slow down the app and also consume disk space.  Use only for troubleshooting")]
        public bool EnableLogs = false;

        public bool DebugLogMessagesToFile = false;
        public bool DebugMessagesEnable = false;

        public bool DebugAssertOnError = false;

        [BoolDescriptor("Enable audit logging of important events.  Use with caution.  This will slow down the app.  Use only for troubleshooting)")]
        public bool AuditLogEnable = false;

        public String AuditLogFilter = "*";

        [BoolDescriptor("Enable Text-to-speech")]
        public bool EnableTextToSpeech = true;
        
        [BoolDescriptor("Enable Performance Monitor to monitor and log Memory and CPU utilization statistics")]
        public bool PerMonEnable = false;

        [BoolDescriptor("Monitor and log CPU utilization")]
        public bool PerMonCPUEnable = false;

        [BoolDescriptor("Monitor and log memory utilization")]
        public bool PerMonMemoryEnable = false;

        [IntDescriptor("How often to monitor and log Performance Monitor statistics (in seconds) ", 5, 3600, 15)]
        public int PerfMonLogInterval = 60;

        [BoolDescriptor("Include empty grid elements in the grid level scanning sequence")]
        public bool TopLevelScanIncludeEmptyGrids = true;

        [BoolDescriptor("Display the actuator switch dialog on startup")]
        public bool ShowSwitchTryoutOnStartup = true;

        [BoolDescriptor("Display the ACAT Talk application interface description on startup")]
        public bool ShowTalkInterfaceDescOnStartup = true;

        [IntDescriptor("Number of times the grid (top level) is scanned", 1, 10)]
        public int GridScanIterations = 4;

        [IntDescriptor("Number of times the rows in a grid are scanned", 1, 10)]
        public int RowScanIterations = 1;

        [IntDescriptor("Number of times the buttons in a row are scanned", 1, 10)]
        public int ColumnScanIterations = 1;

        [BoolDescriptor("Prevent the system from going to sleep and the display from turning off")]
        public bool DisableSystemSleepMode = false;

        [BoolDescriptor("Check display scale factor is set to 100% or 125% and display warning if it is not")]
        public bool ShowDisplayScaleMessageOnStartup = true;

        [BoolDescriptor("Display calibration window help")]
        public bool ShowCalibrationHelp = true;

        public bool OnboardingComplete = false;

        public bool FirstTimeUser = true;

        public String Extensions = "Default,BCI";

        public String Language = String.Empty;

        public String DefaultScanTimingsConfigurePanelName = String.Empty;

        public String DefaultTryoutPanelName = String.Empty;

        //[BoolDescriptor("Auto-hide scanner if the acutator is not triggered for a specified period (see HideOnIdleTimeout) (in msecs)", true)]
        public bool HideScannerOnIdle = false;

        //[IntDescriptor("Hide the scanner if no actuator switch trigger is detected for this length of time ", 3000, 60000)]
        public int HideOnIdleTimeout = 5000;


        [BoolDescriptor("Enable in App sounds")]
        public bool EnableSounds = true;

        // unused variables for ACAT Talk. These will be used in ACAT App
        //[BoolDescriptor("Manual scan mode. User controls the direction of the highlight", false)]
        public bool EnableManualScan = false;

        //[IntDescriptor("Scan time for ACAT Menus and Dialogs (in msecs)", 100, 3000)]
        public int MenuDialogScanTime = 1000;

        //[IntDescriptor("Pre-actuate pause time (in msecs) (only for manual scan)", 2000, 2000)]
        public int ManualScanPreActuatePauseTime = 2000;

        //[IntDescriptor("Actuate pause time (in msecs) (only for manual scan)", 2000, 2000)]
        public int ManualScanActuatePauseTime = 2000;

        //[BoolDescriptor("Enables delayed acutate when scanning stops in the manual scan mode", true)]
        public bool ManualScanDelayedActuateEnable = true;

        //[BoolDescriptor("Highlights the home button after actuation in the manual scan mode", true)]
        public bool ManualScanHighlightDefaultHomePostActuate = true;

        //[BoolDescriptor("Retain the text in the Talk window when its closed and restore it when the Talk window is displayed the next time")]
        public bool RetainTalkWindowContentsOnHide = true;

        //[BoolDescriptor("If the scanner is repositioned, save its position")]
        public bool AutoSaveScannerLastPosition = false;

        //[BoolDescriptor("If the scanner is resized, save its size")]
        public bool AutoSaveScannerScaleFactor = true;

        //[BoolDescriptor("Start scanning automatically", true)]
        public bool EnableAutoStartScan = true;

        /// <summary>
        /// Returns a string representation of the settings
        /// </summary>
        public override String toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Preferences: ");
            sb.Append(XmlUtils.XmlSerializeToString<Preferences>(this));
            return sb.ToString();
        }

        /// <summary>
        /// Resolves a string representation of a value into an integer. The
        /// String can start with the '@' symbol in which case, it refers to
        /// the preference setting of the value.  For instance, @ScanTime would
        /// mean the scan time preference setting.
        /// </summary>
        /// <param name="value">String representation</param>
        /// <param name="defaultIfNull">Value to return if the string is null or empty</param>
        /// <param name="defaultValue">Value to return if not found in the preferences</param>
        /// <returns></returns>
        public int ResolveVariableInt(String value, int defaultIfNull, int defaultValue)
        {
            int retVal = defaultValue;

            if (String.IsNullOrEmpty(value))
            {
                return defaultIfNull;
            }

            if (value[0] != '@')
            {
                try
                {
                    retVal = Convert.ToInt32(value);
                }
                catch
                {
                    retVal = defaultValue;
                }

                return retVal;
            }

            return resolveVariableInt(value, defaultValue);
        }

        /// <summary>
        /// Resolves a name into a value by looking up preferences
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected virtual int resolveVariableInt(String variableName, int defaultValue)
        {
            int retVal = defaultValue;

            switch (variableName.ToLower())
            {
                case "@accepttime":
                    retVal = MinActuationHoldTime;
                    break;

                case "@minactuationholdtime":
                    retVal = MinActuationHoldTime;
                    break;

                case "@fontsize":
                    retVal = FontSize;
                    break;
            }

            return retVal;
        }
    }
}