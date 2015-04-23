////////////////////////////////////////////////////////////////////////////
// <copyright file="WidgetUtils.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.WidgetManagement;

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

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// Contains useful utility functions to get/set states of
    /// toggle widgets, slider widgets etc
    /// </summary>
    public class WidgetUtils
    {
        public const decimal SliderUnitsHundredths = 0.01M;

        /// <summary>
        /// Slider track bar tick units.
        /// </summary>
        public const decimal SliderUnitsOnes = 1;

        public const decimal SliderUnitsTenths = 0.1M;
        public const decimal SliderUnitsThousandths = 0.001M;

        /// <summary>
        /// Returns the toggle state of the CheckBoxWidget
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="name">name of the widget</param>
        /// <returns>the toggle state</returns>
        public static bool GetCheckBoxWidgetState(Widget rootWidget, String name)
        {
            var toggleButton = (CheckBoxWidget)rootWidget.Finder.FindChild(name);

            if (toggleButton != null)
            {
                return toggleButton.GetToggleState();
            }

            return false;
        }

        /// <summary>
        /// Returns the slider position to the indicated value. The 'units' parameter
        /// is used to normalize the value for the number of ticks in the track bar
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="name">name of the slider widget</param>
        /// <param name="units">conversion units</param>
        public static int GetSliderState(Widget rootWidget, String name, decimal units)
        {
            var sliderWidget = (SliderWidget)rootWidget.Finder.FindChild(name);

            if (sliderWidget != null)
            {
                decimal unconvertedValue = sliderWidget.GetSliderValue();
                int retVal = Convert.ToInt32(unconvertedValue / units);
                return retVal;
            }

            return 0;
        }

        /// <summary>
        /// Sets the toggle state of the CheckBoxWidget widget indicated by
        /// the 'name'.
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="name">name of the imagebutton widget</param>
        /// <param name="state">toggle state </param>
        public static void SetCheckBoxWidgetState(Widget rootWidget, String name, Boolean state)
        {
            var toggleButton = (CheckBoxWidget)rootWidget.Finder.FindChild(name);

            if (toggleButton != null)
            {
                toggleButton.SetToggleState(state);
            }
        }

        /// <summary>
        /// Sets the slider position to the indicated value. The 'units' parameter
        /// is used to normalize the value for the number of ticks in the track bar
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="name">name of the slider widget</param>
        /// <param name="sliderPosition">slider value</param>
        /// <param name="units">conversion units</param>
        public static void SetSliderState(Widget rootWidget, String name, int sliderPosition, decimal units)
        {
            var sliderWidget = (SliderWidget)rootWidget.Finder.FindChild(name);

            if (sliderWidget != null)
            {
                sliderWidget.SetSliderValue(sliderPosition, 1 / units);
            }
        }
    }
}