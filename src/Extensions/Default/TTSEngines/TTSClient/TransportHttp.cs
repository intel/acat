////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TransportHttp.cs
//
// Handles sending of a message over Http with the specified format
// of the message
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using System;
using System.Net.Http;
using System.Text;

namespace ACAT.Extensions.TTSEngines.TTSClient
{
    [Serializable]
    public class TransportHttp : ITTSTransport
    {
        public bool Send(String data, TTSFormat format)
        {
            if (format == TTSFormat.None)
            {
                format = TTSFormat.Text;
            }

            sendHttp(data, format);

            return true;
        }

        private async void sendHttp(String data, TTSFormat format)
        {
            string Url = "http://localhost:8004";
            StringContent stringContent = format switch
            {
                TTSFormat.Json => new StringContent(data, Encoding.UTF8, "application/json"),
                TTSFormat.SSML => new StringContent(data, Encoding.UTF8, "application/xml"),
                _ => new StringContent(data, Encoding.UTF8, "text/plain"),
            };
            try
            {
                using var client = new HttpClient();
                var response = await client.PostAsync(Url, stringContent);
            }
            catch (Exception ex)
            {
                Log.Exception("*** Could not send TTS request over http to " + Url + ". Exception: " + ex);
            }
        }
    }
}