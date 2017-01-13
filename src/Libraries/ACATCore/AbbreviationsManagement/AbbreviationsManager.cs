////////////////////////////////////////////////////////////////////////////
// <copyright file="AbbreviationsManager.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Core.AbbreviationsManagement
{
    /// <summary>
    /// Manages list of abbreviations. Loads abbreviations from
    /// a file.  Abbreviations add/delete/update can be done
    /// through the Abbreviations class
    /// </summary>
    public class AbbreviationsManager : IDisposable
    {
        /// <summary>
        /// Static singleton instance
        /// </summary>
        private static readonly AbbreviationsManager _instance = new AbbreviationsManager();

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        private AbbreviationsManager()
        {
            Context.EvtCultureChanged += Context_EvtCultureChanged;
        }

        /// <summary>
        /// Gets the singleton instance of Agent manager
        /// </summary>
        public static AbbreviationsManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets or sets the list of abbreviations
        /// </summary>
        public Abbreviations Abbreviations { get; private set; }

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
        /// Loads abbreviations from the abbreviations file
        /// </summary>
        /// <returns></returns>
        public bool Init(String abbreviationsFile = null)
        {
            if (Abbreviations != null)
            {
                Abbreviations.Dispose();
            }

            Abbreviations = new Abbreviations();

            return Abbreviations.Load(abbreviationsFile);
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
                    Context.EvtCultureChanged -= Context_EvtCultureChanged;

                    if (Abbreviations != null)
                    {
                        Abbreviations.Dispose();
                        Abbreviations = null;
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Event handler for when culture changes
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="arg">event args</param>
        private void Context_EvtCultureChanged(object sender, CultureChangedEventArg arg)
        {
            Abbreviations.Dispose();

            Init();
        }
    }
}