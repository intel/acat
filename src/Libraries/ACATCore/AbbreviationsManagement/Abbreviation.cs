////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Abbreviation.cs
//
// Represents an abbreviation.  The abbreviation has a mnemonic,
// the expansion and the mode of expansion - "Write" or "Speak".
// In the first case, the abbreviation is expanded to its full form
// in the textual form.  In the second case, the expansion is converted
// to speech.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Text.RegularExpressions;

namespace ACAT.Lib.Core.AbbreviationsManagement
{
    public class Abbreviation : IDisposable
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mnemonic">abbreviation</param>
        /// <param name="expansion">abbreviation expansion</param>
        /// <param name="mode">mode of expansion - speech or text</param>
        public Abbreviation(String mnemonic, String expansion, String mode)
        {
            Mode = Convert(mode);
            init(mnemonic, expansion, Mode);
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="mnemonic">abbreviation</param>
        /// <param name="expansion">abbreviation expansion</param>
        /// <param name="mode">mode of expansion - speech or text</param>
        public Abbreviation(String mnemonic, String expansion, AbbreviationMode mode)
        {
            init(mnemonic, expansion, mode);
        }

        /// <summary>
        /// Mode of abbreviation expansion - expand as text, or render as
        /// text to speech
        /// </summary>
        public enum AbbreviationMode
        {
            None,
            Write,
            Speak,
        }

        /// <summary>
        /// Gets or sets the expansion
        /// </summary>
        public String Expansion { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>
        public String Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation mode
        /// </summary>
        public AbbreviationMode Mode { get; set; }

        /// <summary>
        /// Converts the string representation of the mode to the enum
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static AbbreviationMode Convert(String mode)
        {
            var retVal = AbbreviationMode.None;
            try
            {
                retVal = (AbbreviationMode)Enum.Parse(typeof(AbbreviationMode), mode);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return retVal;
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
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Initializes the properties of the abbreviation object
        /// </summary>
        /// <param name="mnemonic">abbreviation</param>
        /// <param name="expansion">abbreviation expansion</param>
        /// <param name="mode">mode of expansion - speech or text</param>
        private void init(String mnemonic, String expansion, AbbreviationMode mode)
        {
            Mnemonic = mnemonic.ToUpper();
            Expansion = Regex.Replace(expansion, "\r\n", "\n");
            Mode = mode;
        }
    }
}