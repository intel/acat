////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Checks if the specified font or the Montserrat font has been installed
    /// </summary>
    public static class FontCheck
    {
        public static bool IsFontInstalled(string fontName)
        {
            using (var testFont = new Font(fontName, 8))
            {
                return 0 == string.Compare(
                  fontName,
                  testFont.Name,
                  StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static bool IsMontserratFontInstalled()
        {
            return IsFontInstalled("Montserrat") &&
                    IsFontInstalled("Montserrat Black") &&
                   IsFontInstalled("Montserrat ExtraBold") &&
                    IsFontInstalled("Montserrat ExtraLight") &&
                    IsFontInstalled("Montserrat Light") &&
                    IsFontInstalled("Montserrat ExtraLight") &&
                    IsFontInstalled("Montserrat Medium") &&
                    IsFontInstalled("Montserrat Semibold") &&
                    IsFontInstalled("Montserrat Thin");
        }
    }
}