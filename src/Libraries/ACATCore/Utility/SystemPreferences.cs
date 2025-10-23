////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using System;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ACATResources;

namespace ACAT.Core.Utility
{
    /// <summary>*
    /// Contains system-wide preference settings.  Settings are serialized
    /// into a file for saving and loaded from the file.
    /// Be careful about renaming variables in this as the variable names
    /// are used as the setting names in the saved XML file.
    /// </summary>
    [Serializable]
    public partial class SystemPreferences : PreferencesBase
    {
        [NonSerialized, XmlIgnore]
        public String AppName = "ACAT";

        // Scanner settings
        [Display(Name = nameof(StringResources.ScannerFirstRepeatTime),
           Description = nameof(StringResources.Firstrepeattimeforstickybuttons), 
           ResourceType = typeof(StringResources))]
        [Range(200, 3000)]
        [UIHint("Slider")]
        [DefaultValue(1000)]
        [ObservableProperty]
        private int firstRepeatTime = 1000;

        [Display(Name = nameof(StringResources.Scantimeinmsecs),
            ResourceType = typeof(StringResources))]
        [Range(100, 3000)]
        [UIHint("Slider")]
        [DefaultValue(1000)]
        [ObservableProperty]
        private int scanTime = 1000;

        [Display(Name = nameof(StringResources.ScannerFirstPauseTime),
            Description = nameof(StringResources.Extratimetopauseonthefirstrowcolumnbuttoninmsecs),
            ResourceType = typeof(StringResources))]
        [Range(0, 3000)]
        [UIHint("Slider")]
        [DefaultValue(250)]
        [ObservableProperty]
        private int firstPauseTime = 250;

        [Display(Name = nameof(StringResources.Playabeeponaselection),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool selectClick = false;

        public float ScannerScaleFactor = 10.0f;

        public String FontName = "Arial";
        public int FontSize = 18;

        public String Theme = "Default";

        [Display(Name = nameof(StringResources.Includedisabledbuttonsinthescanningcycle),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool scanDisabledElements = true;

        public Windows.WindowPosition ScannerPosition = Windows.WindowPosition.CenterScreen;

        public String PreferredPanelConfigNames = "AlphabetQwerty";

        [Display(Name = nameof(StringResources.Minimumholdtimefortheactuatorswitchtotrigger),
        ResourceType = typeof(StringResources))]
        [Range(0, 2000)]
        [UIHint("Slider")]
        [DefaultValue(0)]
        [ObservableProperty]
        private int minActuationHoldTime = 0;

        [Display(Name = nameof(StringResources.Expandanabbreviationonlyifaspacecommaoraperiodisinsertedaftertheabbreviation),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool expandAbbreviationsOnSeparator = false;

        [Display(Name = nameof(StringResources.Logapplicationmessagestoafilesewithcaution),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool enableLogs = true;

        public bool DebugLogMessagesToFile = true;
        public bool DebugMessagesEnable = false;

        public bool DebugAssertOnError = false;

        [Display(Name = nameof(StringResources.Enableauditloggingofimportantevents),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool auditLogEnable = false;

        public String AuditLogFilter = "*";

        [Display(Name = nameof(StringResources.EnableTexttospeech),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool enableTextToSpeech = true;

       [Display(Name = nameof(StringResources.EnablePerformanceMonitortomonitorandlogMemoryandCPUutilizationstatistics),
            ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool perMonEnable = false;

       [Display(Name = nameof(StringResources.MonitorandlogCPUutilization),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool perMonCPUEnable = false;

       [Display(Name = nameof(StringResources.Monitorandlogmemoryutilization),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool perMonMemoryEnable = false;

       [Display(Name = nameof(StringResources.HowoftentomonitorandlogPerformanceMonitorstatistics),
       ResourceType = typeof(StringResources))]
        [Range(5, 3600)]
        [UIHint("Slider")]
        [DefaultValue(15)]
        [ObservableProperty]
        private int perfMonLogInterval = 15;

        [Display(Name = nameof(StringResources.Includeemptygridelementsinthegridlevelscanningsequence),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool topLevelScanIncludeEmptyGrids = true;

       [Display(Name = nameof(StringResources.Displaytheactuatorswitchdialogonstartup),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool showSwitchTryoutOnStartup = true;

       [Display(Name = nameof(StringResources.DisplaytheACATTalkapplicationinterfacedescriptiononstartup),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool showTalkInterfaceDescOnStartup = true;

       [Display(Name = nameof(StringResources.Numberoftimesthegridtoplevelisscanned),
       ResourceType = typeof(StringResources))]
        [Range(1, 10)]
        [UIHint("Slider")]
        [DefaultValue(4)]
        [ObservableProperty]
        private int gridScanIterations = 4;

       [Display(Name = nameof(StringResources.Numberoftimestherowsinagridarescanned),
       ResourceType = typeof(StringResources))]
        [Range(1, 10)]
        [UIHint("Slider")]
        [DefaultValue(1)]
        [ObservableProperty]
        private int rowScanIterations = 1;

       [Display(Name = nameof(StringResources.Numberoftimesthebuttonsinarowarescanned),
       ResourceType = typeof(StringResources))]
        [Range(1, 10)]
        [UIHint("Slider")]
        [DefaultValue(1)]
        [ObservableProperty]
        private int columnScanIterations = 1;

       [Display(Name = nameof(StringResources.Preventthesystemfromgoingtosleepandthedisplayfromturningoff),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool disableSystemSleepMode = false;

       [Display(Name = nameof(StringResources.Checkdisplayscalefactorissetto100or125anddisplaywarningifsot),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool showDisplayScaleMessageOnStartup = true;

       [Display(Name = nameof(StringResources.Displaycalibrationwindowhelp),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool showCalibrationHelp = true;

        public bool OnboardingComplete = false;

        public bool FirstTimeUser = true;

        public String Extensions = "";

        [Display(Name = nameof(StringResources.Language),
            ResourceType = typeof(StringResources))]
        [UIHint("LanguageSelection")]
        [DefaultValue("en")]
        [ObservableProperty]
        private string language = "en";

        [XmlElement(IsNullable = true)]
        public String DefaultScanTimingsConfigurePanelName = String.Empty;

        [XmlElement(IsNullable = true)]
        public String DefaultTryoutPanelName = String.Empty;

        //[BoolDescriptor("Auto-hide scanner if the acutator is not triggered for a specified period (see HideOnIdleTimeout) (in msecs)", true)]
        public bool HideScannerOnIdle = false;

        //[IntDescriptor("Hide the scanner if no actuator switch trigger is detected for this length of time ", 3000, 60000)]
        public int HideOnIdleTimeout = 5000;

       [Display(Name = nameof(StringResources.EnableinAppsounds),
       ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool enableSounds = true;

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
        public virtual int resolveVariableInt(String variableName, int defaultValue)
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

        public override bool Save()
        {
            throw new NotImplementedException();
        }

        public override bool ResetToDefault()
        {
            throw new NotImplementedException();
        }
    }
}