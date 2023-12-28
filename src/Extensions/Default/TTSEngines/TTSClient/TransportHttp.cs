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

using ACAT.Lib.Core.Utility;
using System;
using System.Net.Http;
using System.Text;

namespace ACAT.Extensions.Default.TTSEngines.TTSClient
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
            StringContent stringContent;

            switch (format)
            {
                case TTSFormat.Json:
                    stringContent = new StringContent(data, Encoding.UTF8, "application/json");
                    break;

                case TTSFormat.SSML:
                    stringContent = new StringContent(data, Encoding.UTF8, "application/xml");
                    break;

                case TTSFormat.Text:
                default:

                    stringContent = new StringContent(data, Encoding.UTF8, "text/plain");
                    break;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(Url, stringContent);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("*** Could not send TTS request over http to " + Url + ". Exception: " + ex);
            }
        }
    }
}