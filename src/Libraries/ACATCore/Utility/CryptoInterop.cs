////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace ACAT.Lib.Core.Utility
{
    public static class CryptoInterop
    {
        // source type
        public const int CERT_QUERY_OBJECT_FILE = 1;

        // object type
        private const int CERT_QUERY_CONTENT_CERT = 1;

        private const int CERT_QUERY_CONTENT_CTL = 2;
        private const int CERT_QUERY_CONTENT_CRL = 3;
        private const int CERT_QUERY_CONTENT_SERIALIZED_STORE = 4;
        private const int CERT_QUERY_CONTENT_SERIALIZED_CERT = 5;
        private const int CERT_QUERY_CONTENT_SERIALIZED_CTL = 6;
        private const int CERT_QUERY_CONTENT_SERIALIZED_CRL = 7;
        private const int CERT_QUERY_CONTENT_PKCS7_SIGNED = 8;
        private const int CERT_QUERY_CONTENT_PKCS7_UNSIGNED = 9;
        private const int CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED = 10;
        private const int CERT_QUERY_CONTENT_PKCS10 = 11;
        private const int CERT_QUERY_CONTENT_PFX = 12;
        private const int CERT_QUERY_CONTENT_CERT_PAIR = 13;

        private const int CERT_QUERY_CONTENT_FLAG_CERT = (1 << CERT_QUERY_CONTENT_CERT);
        private const int CERT_QUERY_CONTENT_FLAG_CTL = (1 << CERT_QUERY_CONTENT_CTL);
        private const int CERT_QUERY_CONTENT_FLAG_CRL = (1 << CERT_QUERY_CONTENT_CRL);
        private const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_STORE = (1 << CERT_QUERY_CONTENT_SERIALIZED_STORE);
        private const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_CERT = (1 << CERT_QUERY_CONTENT_SERIALIZED_CERT);
        private const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_CTL = (1 << CERT_QUERY_CONTENT_SERIALIZED_CTL);
        private const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_CRL = (1 << CERT_QUERY_CONTENT_SERIALIZED_CRL);
        private const int CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED = (1 << CERT_QUERY_CONTENT_PKCS7_SIGNED);
        private const int CERT_QUERY_CONTENT_FLAG_PKCS7_UNSIGNED = (1 << CERT_QUERY_CONTENT_PKCS7_UNSIGNED);
        private const int CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED = (1 << CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED);
        private const int CERT_QUERY_CONTENT_FLAG_PKCS10 = (1 << CERT_QUERY_CONTENT_PKCS10);
        private const int CERT_QUERY_CONTENT_FLAG_PFX = (1 << CERT_QUERY_CONTENT_PFX);
        private const int CERT_QUERY_CONTENT_FLAG_CERT_PAIR = (1 << CERT_QUERY_CONTENT_CERT_PAIR);

        public const int CERT_QUERY_CONTENT_FLAG_ALL =
            CERT_QUERY_CONTENT_FLAG_CERT |
            CERT_QUERY_CONTENT_FLAG_CTL |
            CERT_QUERY_CONTENT_FLAG_CRL |
            CERT_QUERY_CONTENT_FLAG_SERIALIZED_STORE |
            CERT_QUERY_CONTENT_FLAG_SERIALIZED_CERT |
            CERT_QUERY_CONTENT_FLAG_SERIALIZED_CTL |
            CERT_QUERY_CONTENT_FLAG_SERIALIZED_CRL |
            CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED |
            CERT_QUERY_CONTENT_FLAG_PKCS7_UNSIGNED |
            CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED |
            CERT_QUERY_CONTENT_FLAG_PKCS10 |
            CERT_QUERY_CONTENT_FLAG_PFX |
            CERT_QUERY_CONTENT_FLAG_CERT_PAIR;

        // format type
        private const int CERT_QUERY_FORMAT_BINARY = 1;

        private const int CERT_QUERY_FORMAT_BASE64_ENCODED = 2;
        private const int CERT_QUERY_FORMAT_ASN_ASCII_HEX_ENCODED = 3;
        private const int CERT_QUERY_FORMAT_FLAG_BINARY = 1 << CERT_QUERY_FORMAT_BINARY;
        private const int CERT_QUERY_FORMAT_FLAG_BASE64_ENCODED = 1 << CERT_QUERY_FORMAT_BASE64_ENCODED;
        private const int CERT_QUERY_FORMAT_FLAG_ASN_ASCII_HEX_ENCODED = 1 << CERT_QUERY_FORMAT_ASN_ASCII_HEX_ENCODED;

        public const int CERT_QUERY_FORMAT_FLAG_ALL =
            CERT_QUERY_FORMAT_FLAG_BINARY |
            CERT_QUERY_FORMAT_FLAG_BASE64_ENCODED |
            CERT_QUERY_FORMAT_FLAG_ASN_ASCII_HEX_ENCODED;

        public const int CMSG_ENCODED_MESSAGE = 29;

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CryptQueryObject(
            int dwObjectType,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pvObject,
            int dwExpectedContentTypeFlags,
            int dwExpectedFormatTypeFlags,
            int dwFlags,
            ref int pdwMsgAndCertEncodingType,
            ref int pdwContentType,
            ref int pdwFormatType,
            ref IntPtr phCertStore,
            ref IntPtr phMsg,
            ref IntPtr ppvContext
        );

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CryptMsgGetParam(
            IntPtr hCryptMsg,
            int dwParamType,
            int dwIndex,
            byte[] pvData,
            ref int pcbData
        );

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CryptMsgClose(
            IntPtr hCryptMsg
        );

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CertCloseStore(
            IntPtr hCertStore,
            int dwFlags
        );
    }
}
