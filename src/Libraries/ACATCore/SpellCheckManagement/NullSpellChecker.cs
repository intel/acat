////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.Utility;
using System;
using System.Globalization;

namespace ACAT.Lib.Core.SpellCheckManagement
{
    /// <summary>
    /// The null spellchecker basically does nothing.  It is used
    /// where no SpellChecker is currently active/valid.
    /// </summary>
    [DescriptorAttribute("CCC45241-9BA0-4BD9-AB37-DC2C960772F4",
                        "Null Spell Checker",
                        "No spell checking functionality.")]
    public class NullSpellChecker : ISpellChecker, IExtension
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
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
        /// Returns the invoker object
        /// </summary>
        /// <returns>Invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return null;
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>true</returns>
        public bool Init(CultureInfo ci)
        {
            return true;
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
        /// Saves the settings (no-op)
        /// </summary>
        /// <param name="configFileDirectory">file</param>
        /// <returns></returns>
        public bool SaveSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Uninitializes
        /// </summary>
        public void Uninit()
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