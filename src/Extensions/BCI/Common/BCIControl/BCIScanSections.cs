////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIMode.cs
//
// Parameters send from ACAT to indicate the current BCI Mode.
// Supported modes: Signal Monitor, Calibration, Tryout, Typing and Unknown
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public enum BCIScanSections
    {
        Box,
        Word,
        Sentence,
        KeyboardL,
        KeyboardR,
        None
    }
}