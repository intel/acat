////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Argument for the event raised when the TTS engine reaches
    /// a bookmark.  Bookmarks can be inserted into the text stream
    /// and during the conversion process, the bookmarks are returned
    /// through this event, telling how far the speech engine has progressed.
    /// </summary>
    public class TTSBookmarkReachedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="bookmark">bookmark</param>
        public TTSBookmarkReachedEventArgs(int bookmark)
        {
            Bookmark = bookmark;
        }

        /// <summary>
        /// Gets the bookmark value
        /// </summary>
        public int Bookmark { get; private set; }
    }
}