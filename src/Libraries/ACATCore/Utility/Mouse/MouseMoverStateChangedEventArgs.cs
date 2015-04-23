////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseMoverStateChangedEventArgs.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Delegate for the event that is raised when
    /// the mouse mover state chagnes
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">eent args</param>
    public delegate void MouseMoverStateChanged(object sender, MouseMoverStateChangedEventArgs e);

    /// <summary>
    /// State of the grid / mouse movers
    /// </summary>
    public enum MouseMoverStates
    {
        Idle,
        Radar,
        PreRadarMouseMove,
        RadarMouseMove,
        Grid,
        PreGridMouseMove,
        GridMouseMove
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public class MouseMoverStateChangedEventArgs : EventArgs
    {
        public MouseMoverStateChangedEventArgs(MouseMoverStates state)
        {
            State = state;
        }

        /// <summary>
        /// Gets or sets mouse mover state
        /// </summary>
        public MouseMoverStates State { get; private set; }
    }
}