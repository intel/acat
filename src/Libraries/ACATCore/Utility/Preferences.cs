////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using System;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ACAT.Core.Utility
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
        [Descriptor("First repeat time for sticky buttons (in msecs)")]
        [Range(200, 3000)]
        [UIHint("Slider")]
        [DefaultValue(1000)]
        public int FirstRepeatTime { get; set; }  =  1000;

        [Descriptor("Scan time (in msecs)")]
        [Range(100, 3000)]
        [UIHint("Slider")]
        [DefaultValue(1000)]
        public int ScanTime { get; set; }  =  1000;

        [Descriptor("Extra time to pause on the first row/column/button (in msecs)")]
        [Range(0, 3000)]
        [UIHint("Slider")]
        [DefaultValue(250)]
        public int FirstPauseTime { get; set; }  =  250;

        [Descriptor("Play a beep on a selection")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool SelectClick { get; set; }  =  false;

        public float ScannerScaleFactor = 10.0f;

        public String FontName = "Arial";
        public int FontSize = 18;

        public String Theme = "Default";

        [Descriptor("Include disabled buttons in the scanning cycle")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool ScanDisabledElements { get; set; }  =  true;

        public Windows.WindowPosition ScannerPosition = Windows.WindowPosition.MiddleRight;

        public String PreferredPanelConfigNames = "AlphabetQwerty";

        [Descriptor("Minimum hold time for the actuator switch to trigger (in msecs)")]
        [Range(0, 2000)]
        [UIHint("Slider")]
        [DefaultValue(0)]
        public int MinActuationHoldTime { get; set; }  =  0;

        [Descriptor("Expand an abbreviation only if a space, comma or a period is inserted after the abbreviation")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool ExpandAbbreviationsOnSeparator { get; set; }  =  false;

        [Descriptor("Log application messages to a file. Use with caution. This will slow down the app and also consume disk space.  Use only for troubleshooting")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool EnableLogs { get; set; }  =  false;

        public bool DebugLogMessagesToFile = false;
        public bool DebugMessagesEnable = false;

        public bool DebugAssertOnError = false;

        [Descriptor("Enable audit logging of important events.  Use with caution.  This will slow down the app.  Use only for troubleshooting)")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool AuditLogEnable { get; set; }  =  false;

        public String AuditLogFilter = "*";

        [Descriptor("Enable Text-to-speech")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool EnableTextToSpeech { get; set; }  =  true;
        
        [Descriptor("Enable Performance Monitor to monitor and log Memory and CPU utilization statistics")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool PerMonEnable { get; set; }  =  false;

        [Descriptor("Monitor and log CPU utilization")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool PerMonCPUEnable { get; set; }  =  false;

        [Descriptor("Monitor and log memory utilization")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool PerMonMemoryEnable { get; set; }  =  false;

        [IntDescriptor("How often to monitor and log Performance Monitor statistics (in seconds) ")]
        [Range(5, 3600)]
        [UIHint("Slider")]
        [DefaultValue(15)]
        public int PerfMonLogInterval { get; set; }  =  15;

        [Descriptor("Include empty grid elements in the grid level scanning sequence")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool TopLevelScanIncludeEmptyGrids { get; set; }  =  true;

        [Descriptor("Display the actuator switch dialog on startup")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool ShowSwitchTryoutOnStartup { get; set; }  =  true;

        [Descriptor("Display the ACAT Talk application interface description on startup")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool ShowTalkInterfaceDescOnStartup { get; set; }  =  true;

        [Descriptor("Number of times the grid (top level) is scanned")]
        [Range(1, 10)]
        [UIHint("Slider")]
        [DefaultValue(4)]
        public int GridScanIterations { get; set; }  =  4;

        [Descriptor("Number of times the rows in a grid are scanned")]
        [Range(1, 10)]
        [UIHint("Slider")]
        [DefaultValue(1)]
        public int RowScanIterations { get; set; }  =  1;

        [Descriptor("Number of times the buttons in a row are scanned")]
        [Range(1, 10)]
        [UIHint("Slider")]
        [DefaultValue(1)]
        public int ColumnScanIterations { get; set; }  =  1;

        [Descriptor("Prevent the system from going to sleep and the display from turning off")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool DisableSystemSleepMode { get; set; }  =  false;

        [Descriptor("Check display scale factor is set to 100% or 125% and display warning if it is not")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool ShowDisplayScaleMessageOnStartup { get; set; }  =  true;

        [Descriptor("Display calibration window help")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool ShowCalibrationHelp { get; set; }  =  true;

        public bool OnboardingComplete = false;

        public bool FirstTimeUser = true;

        public String Extensions = "Default,BCI";

        [XmlElement(IsNullable = true)]
        public String Language = String.Empty;

        [XmlElement(IsNullable = true)]
        public String DefaultScanTimingsConfigurePanelName = String.Empty;

        [XmlElement(IsNullable = true)]
        public String DefaultTryoutPanelName = String.Empty;

        //[BoolDescriptor("Auto-hide scanner if the acutator is not triggered for a specified period (see HideOnIdleTimeout) (in msecs)", true)]
        public bool HideScannerOnIdle = false;

        //[IntDescriptor("Hide the scanner if no actuator switch trigger is detected for this length of time ", 3000, 60000)]
        public int HideOnIdleTimeout = 5000;


        [Descriptor("Enable in App sounds")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool EnableSounds { get; set; }  =  true;

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
            sb.Append(XmlUtils.XmlSerializeToString(this));
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
            if (String.IsNullOrEmpty(value))
            {
                return defaultIfNull;
            }

            if (value[0] != '@')
            {
                int retVal;
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