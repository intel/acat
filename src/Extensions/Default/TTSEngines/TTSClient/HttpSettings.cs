////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// HttpSettings.cs
//
// Settings for the HTTP text to speech client
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.Default.TTSEngines.TTSClient
{
    public class HttpSettings
    {
        public HttpSettings()
        {
            Url = "http://localhost";
            Port = 8004;
            Format = TTSFormat.Json;
        }

        public TTSFormat Format { get; set; }
        public int Port { get; set; }
        public String Url { get; set; }
    }
}