////////////////////////////////////////////////////////////////////////////
// <copyright file="Preferences.cs" company="Intel Corporation">
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
using System.Reflection;
using System.Text;

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
[module: SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1401:FieldsMustBePrivate",
        Scope = "namespace",
        Justification = "This class is serialized")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Contains system-wide preference settings.  Settings are serialized
    /// into a file for saving and loaded from the file.
    /// Be careful about renaming variables in this as the variable names
    /// are used as the setting names in the saved XML file.
    /// </summary>
    [Serializable]
    public abstract class Preferences : PreferencesBase
    {
        [NonSerialized]
        public static Assembly ApplicationAssembly;

        // Scanner settings
        public int TabScanTime = 1000;

        public int FirstRepeatTime = 1000;
        public int SteppingTime = 1000;
        public int HesitateTime = 250;
        public bool ScanClick = false;
        public bool SelectClick = true;
        public float ScannerScaleFactor = 10.0f;
        public String FontName = "Arial";
        public int FontSize = 18;
        public String Skin = "BlackYellow";
        public bool AutoSaveScannerLastPosition = false;
        public bool ScanDisabledElements = true;
        public Windows.WindowPosition ScannerPosition = Windows.WindowPosition.TopRight;
        public bool ScannerShowBorder = true;
        public bool ScannerShowTitleBar = true;
        public String PreferredPanelConfigNames = "MenusWithText";
        //public bool ScannerRoundedCorners = false;

        //Actuator settings
        public int AcceptTime = 50;

        // General settings
        public bool EnableGlass = true;

        public float GlassOpacity = 0.8f;
        public bool GlassFadeIn = false;
        public int SpacesAfterPunctuation = 1;
        public bool HideWindowsTaskBar = false;
        public bool HideScannerOnIdle = false;
        public int HideOnIdleTimeout = 5000;
        public bool ExpandAbbreviationsOnSeparator = false;

        // Debug settings
        public bool DebugLogMessagesToFile = false;

        public bool DebugMessagesEnable = false;
        public bool DebugAssertOnError = false;

        // Audit Log settings
        public bool AuditLogEnable = false;

        public String AuditLogFilter = "*";

        // Talk window settings
        public bool RetainTalkWindowContentsOnHide = true;

        public float TalkWindowFontSize = 0.0f;

        // TTS Settings
        public bool EnableTextToSpeech = true;

        public String Extensions = "Default";

        public String PreferredTTSEngines = "SAPI TTS Engine";

        public String PreferredWordPredictor = "Presage Word Predictor";

        public String PreferredSpellChecker = "ACAT SpellChecker";

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
        /// the preference setting of the value.  For instance, @SteppingTime would
        /// mean the Stepping time preference setting.
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

            switch (variableName)
            {
                case "@AcceptTime":
                    retVal = AcceptTime;
                    break;

                case "@FontSize":
                    retVal = FontSize;
                    break;
            }

            return retVal;
        }
    }
}