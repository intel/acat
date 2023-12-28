////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AbbreviationsManager.cs
//
// Manages list of abbreviations. Loads abbreviations from
// a file.  Abbreviations add/delete/update can be done
// through the Abbreviations class
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Core.AbbreviationsManagement
{
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