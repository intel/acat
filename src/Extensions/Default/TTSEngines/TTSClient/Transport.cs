////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Transport.cs
//
// Handles formatting of the message to send and sending of the message
// depending on the selected protocol
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.Default.TTSEngines.TTSClient
{
    public class Transport : ITTSTransport
    {
        /// <summary>
        /// Settings for this engine
        /// </summary>
        private static TTSClientSettings _settings;

        private TransportHttp _transportHttp;

        public Transport()
        {
            _settings = TTSClientSettings.Load();
            _transportHttp = new TransportHttp();
        }

        public TTSFormat Format
        {
            get
            {
                switch (Protocol)
                {
                    case TransportProtocol.Http:
                        return _settings.HttpSettings.Format;

                    default:
                        return TTSFormat.Text;
                }
            }
        }

        public TransportProtocol Protocol
        {
            get
            {
                return _settings.Protocol;
            }
        }

        public bool Send(String data, TTSFormat format = TTSFormat.None)
        {
            bool retVal = true;

            if (format == TTSFormat.None)
            {
                format = Format;
            }

            switch (Protocol)
            {
                case TransportProtocol.Http:
                    retVal = _transportHttp.Send(data, format);
                    break;

                default:
                    return false;
            }

            return retVal;
        }
    }
}