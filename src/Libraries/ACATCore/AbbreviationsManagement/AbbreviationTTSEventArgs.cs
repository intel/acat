////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AbbreviationTTSEventArgs.cs
//
// Represents the event args for a text to speech request for abbreviation
// expansion
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.AbbreviationsManagement
{
    public class AbbreviationTTSEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="text">Text to convert to speech</param>
        public AbbreviationTTSEventArgs(String text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets the string to be converted to speech
        /// </summary>
        public String Text { get; private set; }
    }
}