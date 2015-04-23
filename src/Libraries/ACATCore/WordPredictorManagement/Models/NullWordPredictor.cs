////////////////////////////////////////////////////////////////////////////
// <copyright file="IWordPredictorSettingsDialog.cs" company="Intel Corporation">
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
using System.Collections.Generic;
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

namespace ACAT.Lib.Core.WordPredictionManagement
{
    /// <summary>
    /// The null word predictor basically does nothing.  It is used
    /// where no word predictor is currently valid.
    /// </summary>
    [DescriptorAttribute("3EF5A318-6357-467D-BF45-9C925CF72FF4", "Null Word Predictor", "Word prediction Disabled.")]
    public class NullWordPredictor : IWordPredictor
    {
        private int _contextHandle = 1;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get
            {
                return DescriptorAttribute.GetDescriptor(GetType());
            }
        }

        /// <summary>
        /// Gets or sets whether to filter punctuations
        /// </summary>
        public bool FilterPunctuationsEnable { get; set; }

        /// <summary>
        /// Gets or sets the NGram for word prediction
        /// </summary>
        public int NGram { get; set; }

        /// <summary>
        /// Get or sets the word count
        /// </summary>
        public int PredictionWordCount { get; set; }

        /// <summary>
        /// Returns the settings dialog
        /// </summary>
        /// <returns></returns>
        public IWordPredictorSettingsDialog SettingsDialog
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Does this support learning.  Obviously not.
        /// </summary>
        public bool SupportsLearning
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Doesn't support a settings dialog
        /// </summary>
        public bool SupportsSettingsDialog
        {
            get
            {
                return false;
            }
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
        /// Perform initialization
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            return true;
        }

        /// <summary>
        /// Doesn't learn anything :-)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool Learn(String text)
        {
            return true;
        }

        /// <summary>
        /// Call this to provide context from the document that is
        /// currently active.
        /// </summary>
        /// <param name="text"></param>
        public int LoadContext(String text)
        {
            return _contextHandle++;
        }

        /// <summary>
        /// Load factory default settings.
        /// </summary>
        /// <returns></returns>
        public bool LoadDefaultSettings()
        {
            return true;
        }

        /// <summary>
        /// Loads settings from the specified directory
        /// </summary>
        /// <param name="configFileDirectory"></param>
        /// <returns></returns>
        public bool LoadSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Perform word prediction (does nothing here)
        /// </summary>
        /// <param name="prevNWords"></param>
        /// <param name="lastWord"></param>
        /// <returns></returns>
        public IEnumerable<String> Predict(String prevNWords, String lastWord)
        {
            return new List<String>();
        }

        /// <summary>
        ///  Save settings into the specified directory
        /// </summary>
        /// <param name="configFileDirectory"></param>
        /// <returns></returns>
        public bool SaveSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Call this oto unload context
        /// </summary>
        /// <param name="handle"></param>
        public void UnloadContext(int handle)
        {
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