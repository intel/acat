////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using Microsoft.Extensions.Logging;

namespace ACAT.Core.Utility
{
    public class VerifyDigitalSignature
    {
        private static ILogger<VerifyDigitalSignature> _logger => LogManager.GetLogger<VerifyDigitalSignature>();
#if ENABLE_DIGITAL_VERIFICATION
        private static string[] _dllFiles =
        {
            "acatresources.resources.dll",
            "scanners.dll",
            "usercontrols.dll",
            "acat_gestures_dll.dll",
            "acat_gestures_dll_d.dll",
            "libinfra.dll",
            "libinfra_d.dll",
            "libivcp.dll",
            "libivcp_d.dll",
            "libpipeline.dll",
            "libpipeline_d.dll",
            "CameraActuator.dll",
            "acatagent.dll",
            "talkapplicationscanneragent.dll",
            "menus.dll",
            "SpellCheck.dll",
            "SAPIEngine.dll",
            "TTSClient.dll",
            "ConvAssist.dll",
            "BCIControl.dll",
            "BCIActuator.dll",
            "EEGDataAcquisition.dll",
            "EEGProcessing.dll",
            "EEGSettings.dll",
            "openBCISensorUI.dll",
            "gTecSensorUI.dll",
            "animationsharp.dll",
            "BCIInterfaceUtilities.dll"
        };
#endif

        /// <summary>
        /// If the DLL needs to validate if it has an active certificate
        /// </summary>
        /// <param name="fileName">dll path</param>
        /// <returns>True to verify</returns>
        public static bool ValidateCertificate(string fileName)
        {
#if ENABLE_DIGITAL_VERIFICATION
            foreach(string str in _dllFiles)
            {
                if (fileName.ToLower().Contains(str.ToLower()) && str.Length > 0)
                    return true;
            }
            return false;
#else
            return false;
#endif
        }

        public static void Verify(String fileName)
        {
            IntPtr certStore = IntPtr.Zero;
            IntPtr msgHandle = IntPtr.Zero;
            IntPtr context = IntPtr.Zero;
            int msgAndCertEncodingType = 0;
            int contentType = 0;
            int formatType = 0;
            const int ErrCertExpired = -2146762495;

            _logger?.LogDebug("Verify digital signature for {FileName}", fileName);

            if (!CryptoInterop.CryptQueryObject(
                CryptoInterop.CERT_QUERY_OBJECT_FILE,
                fileName,
                CryptoInterop.CERT_QUERY_CONTENT_FLAG_ALL,
                CryptoInterop.CERT_QUERY_FORMAT_FLAG_ALL,
                0,
                ref msgAndCertEncodingType,
                ref contentType,
                ref formatType,
                ref certStore,
                ref msgHandle,
                ref context
            ))
            {
                _logger?.LogDebug("Crypto query failed: {Message}", (new Win32Exception(Marshal.GetLastWin32Error())).Message);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            int data = 0;
            if (!CryptoInterop.CryptMsgGetParam(msgHandle, CryptoInterop.CMSG_ENCODED_MESSAGE, 0, null, ref data))
            {
                _logger?.LogDebug("CryptMsgGetParam failed: {Message}", (new Win32Exception(Marshal.GetLastWin32Error())).Message);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            byte[] pvData = new byte[data];
            CryptoInterop.CryptMsgGetParam(msgHandle, CryptoInterop.CMSG_ENCODED_MESSAGE, 0, pvData, ref data);
            var signedCms = new SignedCms();
            signedCms.Decode(pvData);
            try
            {
                signedCms.CheckSignature(false);
                _logger?.LogDebug("Signature check passed");
            }
            catch (Exception e)
            {
                if (e.HResult != ErrCertExpired)
                {
                    _logger?.LogError(e, "Signature check failed");
                    throw (e);
                }
            }
            finally
            {
                CryptoInterop.CryptMsgClose(msgHandle);
                CryptoInterop.CertCloseStore(certStore, 0);
            }
        }
    }
}