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

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Core.AbbreviationsManagement
{
    public class AbbreviationsManager : IAbbreviationsManager, IDisposable
    {
        private readonly ILogger<AbbreviationsManager> _logger;

        /// <summary>
        /// Static singleton instance - lazy initialized to get logger from DI container
        /// </summary>
        private static readonly Lazy<AbbreviationsManager> _instance = new Lazy<AbbreviationsManager>(() =>
        {
            // Get logger from DI container if available, otherwise create standalone logger
            ILogger<AbbreviationsManager> logger = Context.ServiceProvider?.GetService(typeof(ILogger<AbbreviationsManager>)) as ILogger<AbbreviationsManager>
                ?? LogManager.GetLogger<AbbreviationsManager>();
            return new AbbreviationsManager(logger);
        });

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        private AbbreviationsManager(ILogger<AbbreviationsManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Context.EvtCultureChanged += Context_EvtCultureChanged;
        }

        /// <summary>
        /// Gets the singleton instance of Agent manager
        /// </summary>
        public static AbbreviationsManager Instance
        {
            get { return _instance.Value; }
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
            Abbreviations?.Dispose();

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