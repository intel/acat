////////////////////////////////////////////////////////////////////////////
// <copyright file="NullSpellChecker.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Lib.Core.SpellCheckManagement
{
    /// <summary>
    /// The null word predictor basically does nothing.  It is used
    /// where no word predictor is currently valid.
    /// </summary>
    ///
    [DescriptorAttribute("CCC45241-9BA0-4BD9-AB37-DC2C960772F4", "Null Spell Checker", "No spell checking functionality.")]
    public class NullSpellChecker : ISpellChecker
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the settings dialog
        /// </summary>
        public ISpellCheckerSettingsDialog SettingsDialog
        {
            get { return null; }
        }

        /// <summary>
        /// Gets whether this supports a settings dialog
        /// </summary>
        public bool SupportsSettingsDialog
        {
            get { return false; }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <returns>true onf success</returns>
        public bool Init()
        {
            return true;
        }

        /// <summary>
        /// Load factory default settings.
        /// </summary>
        /// <returns>true on success</returns>
        public bool LoadDefaultSettings()
        {
            return true;
        }

        /// <summary>
        /// Loads settings from the specified directory
        /// </summary>
        /// <param name="configFileDirectory">Location of the config file</param>
        /// <returns>true on success</returns>
        public bool LoadSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Looks up the specified word and returns the
        /// correct spelling
        /// </summary>
        /// <param name="word">word to lookup</param>
        /// <returns>spelling</returns>
        public String Lookup(String word)
        {
            return String.Empty;
        }

        /// <summary>
        ///  Save settings into the specified directory
        /// </summary>
        /// <param name="configFileDirectory">Location of the config file</param>
        /// <returns>true on success</returns>

        public bool SaveSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }
    }
}