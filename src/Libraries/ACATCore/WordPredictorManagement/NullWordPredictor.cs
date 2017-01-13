////////////////////////////////////////////////////////////////////////////
// <copyright file="NullWordPredictor.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ACAT.Lib.Core.WordPredictionManagement
{
    /// <summary>
    /// The null word predictor basically does nothing.  It is used
    /// where no word predictor is currently valid.
    /// </summary>
    [DescriptorAttribute("3EF5A318-6357-467D-BF45-9C925CF72FF4",
                            "Null Word Predictor",
                            "Disable word prediction")]
    public class NullWordPredictor : IWordPredictor, ISupportsPreferences, IExtension
    {
        /// <summary>
        /// Used to invoke methods/properties in the agent
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Handle for the document context
        /// </summary>
        private int _contextHandle = 1;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        public NullWordPredictor()
        {
            _invoker = new ExtensionInvoker(this);
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
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
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public bool SupportsPreferencesDialog
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
        /// Returns the default preferences object for the word predictor
        /// </summary>
        /// <returns>default preferences object</returns>
        public IPreferences GetDefaultPreferences()
        {
            return null;
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns></returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Returns the preferences object for the word predictor
        /// </summary>
        /// <returns>preferences object</returns>
        public IPreferences GetPreferences()
        {
            return null;
        }

        /// <summary>
        /// Initialize the word predictor
        /// </summary>
        /// <param name="ci">not used</param>
        /// <returns>true on success, false on failure</returns>
        public bool Init(CultureInfo ci)
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
        /// Shows the preferences dialog
        /// </summary>
        /// <returns>true on success</returns>
        public bool ShowPreferencesDialog()
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