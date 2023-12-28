////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AbbreviationReplaceEventArgs.cs
//
// Represents argument used by the event that is raised to indicate that
// an abbreviation needs to be handled.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.AbbreviationsManagement
{
    /// <summary>
    /// Represents argument used by the event that is raised to indicate that
    /// an abbreviation needs to be handled.
    /// </summary>
    public class AbbreviationReplaceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="startPos">Offset in the text stream</param>
        /// <param name="wordLength">Length of the word in the stream that needs replacing</param>
        /// <param name="replaceString">What to replace the word with</param>
        public AbbreviationReplaceEventArgs(int startPos, int wordLength, String replaceString)
        {
            StartPos = startPos;
            WordLength = wordLength;
            ReplaceString = replaceString;
        }

        /// <summary>
        /// The replacement string
        /// </summary>
        public String ReplaceString { get; private set; }

        /// <summary>
        /// Gets the starting pos in the text stream where the abbr was detected
        /// </summary>
        public int StartPos { get; private set; }

        /// <summary>
        /// Gets the length of the word to replace
        /// </summary>
        public int WordLength { get; private set; }
    }
}