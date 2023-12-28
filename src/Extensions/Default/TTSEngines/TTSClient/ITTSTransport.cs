////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ITTsTransport.cs
//
// Interface class for the tranport protocol
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.Default.TTSEngines.TTSClient
{
    public enum TTSFormat
    {
        None,
        Text,
        Json,
        SSML
    }

    public interface ITTSTransport
    {
        bool Send(String data, TTSFormat format);
    }
}