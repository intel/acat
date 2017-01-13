////////////////////////////////////////////////////////////////////////////
// <copyright file="Preferences.cs" company="Intel Corporation">
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
        public String AppId;

        [NonSerialized, XmlIgnore]
        public String AppName = "ACAT";

        [NonSerialized, XmlIgnore]
        public bool EnableGlass = false;

        [NonSerialized, XmlIgnore]
        public float GlassOpacity = 0.8f;

        [NonSerialized, XmlIgnore]
        public bool GlassFadeIn = false;

        // Scanner settings
        [IntDescriptor("Scan time for ACAT Menus and Dialogs (in msecs)", 100, 3000)]
        public int MenuDialogScanTime = 1000;

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

        [BoolDescriptor("If the scanner is repositioned, save its position")]
        public bool AutoSaveScannerLastPosition = false;

        [BoolDescriptor("If the scanner is resized, save its size")]
        public bool AutoSaveScannerScaleFactor = true;

        [BoolDescriptor("Include disabled buttons in the scanning cycle", true)]
        public bool ScanDisabledElements = true;

        public Windows.WindowPosition ScannerPosition = Windows.WindowPosition.MiddleRight;

        public String PreferredPanelConfigNames = "AlphabetQwerty";

        [IntDescriptor("Minimum hold time for the actuator switch to trigger (in msecs)", 0, 2000)]
        public int MinActuationHoldTime = 50;

        public bool HideWindowsTaskBar = false;

        [BoolDescriptor("Auto-hide scanner if the acutator is not triggered for a specified period (see HideOnIdleTimeout) (in msecs)", true)]
        public bool HideScannerOnIdle = false;

        [IntDescriptor("Hide the scanner if no actuator switch trigger is detected for this length of time ", 3000, 60000)]
        public int HideOnIdleTimeout = 5000;

        [BoolDescriptor("Expand an abbreviation only if a space, comma or a period is inserted after the abbreviation")]
        public bool ExpandAbbreviationsOnSeparator = false;

        [BoolDescriptor("Log debug trace messages to a file.  DebugMessagesEnable must also be set to true.  Use with caution.  This will slow down the app and also consume disk space.  Use only for troubleshooting")]
        public bool DebugLogMessagesToFile = false;

        [BoolDescriptor("Enable debug trace messages. Use the DebugView utility to view the messages  (Use with caution.  This will slow down the app)")]
        public bool DebugMessagesEnable = false;
        public bool DebugAssertOnError = false;

        [BoolDescriptor("Enable audit logging of important events.  Use with caution.  This will slow down the app.  Use only for troubleshooting)")]
        public bool AuditLogEnable = false;

        public String AuditLogFilter = "*";

        [BoolDescriptor("Retain the text in the Talk window when its closed and restore it when the Talk window is displayed the next time")]
        public bool RetainTalkWindowContentsOnHide = true;

        [FloatDescriptor("Size of the font in the Talk window.  Set to 0 for default font size", 0, 72)]
        public float TalkWindowFontSize = 0.0f;

        [BoolDescriptor("Snap Talk window to vertically stretch from the top of the display to the bottom")]
        public bool SnapTalkWindow = false;

        [BoolDescriptor("Enable Text-to-speech")]
        public bool EnableTextToSpeech = true;

        public String Extensions = "Default";

        public String Language = String.Empty;

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