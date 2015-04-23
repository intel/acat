#region Author Information
// Author: Sai Prasad
// Group : XTL, IXR
#endregion

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using Aster.Utility;
#endregion

namespace Aster.WordPredictionManagement
{
    /// <summary>
    /// The null word predictor basically does nothing.  It is used
    /// where no word predictor is currently valid.
    /// </summary>
    [DescriptorAttribute("3EF5A318-6357-467D-BF45-9C925CF72FF4", "Null Word Predictor", "Word prediction Disabled.")]
    public class NullWordPredictor : IWordPredictor
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;
        private int _contextHandle = 1;

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

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // Take ourselves off the Finalization queue 
            // to prevent finalization code for this object
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
        /// Doesn't learn anything :-)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool Learn(String text)
        {
            return true;
        }

        /// <summary>
        /// Get or sets the word count
        /// </summary>
        public int PredictionWordCount { get; set; }

        /// <summary>
        /// Gets or sets whether to filter punctuations
        /// </summary>
        public bool FilterPunctuationsEnable { get; set; }

        /// <summary>
        /// Gets or sets the NGram for word prediction
        /// </summary>
        public int NGram { get; set; }

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
        /// Load factory default settings.
        /// </summary>
        /// <returns></returns>
        public bool LoadDefaultSettings()
        {
            return true;
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
        /// Call this to provide context from the document that is 
        /// currently active.
        /// </summary>
        /// <param name="text"></param>
        public int LoadContext(String text)
        {
            return _contextHandle++;
        }

        /// <summary>
        /// Call this oto unload context
        /// </summary>
        /// <param name="handle"></param>
        public void UnloadContext(int handle)
        {
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.
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
