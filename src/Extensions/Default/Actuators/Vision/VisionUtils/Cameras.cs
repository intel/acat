////////////////////////////////////////////////////////////////////////////
// <copyright file="Cameras.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ACAT.Extensions.Default.Actuators.VisionActuator.VisionUtils
{
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
        Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pType,
            [Out] out IEnumMoniker ppEnumMoniker,
            [In] int dwFlags);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("3127CA40-446E-11CE-8135-00AA004BB851"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IErrorLog
    {
        [PreserveSig]
        int AddError(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
         Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyBag
    {
        [PreserveSig]
        int Read(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [Out, MarshalAs(UnmanagedType.Struct)] out object pVar,
            [In] IErrorLog pErrorLog
            );

        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In, MarshalAs(UnmanagedType.Struct)] ref object pVar
            );
    }

    /// <summary>
    /// Returns a list of installed cameras on the host machine
    /// </summary>
    public class Cameras
    {
        public static readonly Guid VideoInputDevice = new Guid(0x860BB310,
                                                                0x5D01, 0x11d0,
                                                                0xBD, 0x3B,
                                                                0x00, 0xA0,
                                                                0xC9, 0x11,
                                                                0xCE, 0x86);

        private const int S_OK = 0;

        /// <summary>
        /// Returns a list of names of the cameras currently installed on the
        /// host computer
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<String> GetCameraNames()
        {
            var monikerList = getDevicesOfCategory(VideoInputDevice);

            var namesList = new List<string>();

            foreach (var moniker in monikerList)
            {
                var name = getPropBagValue(moniker, "FriendlyName");

                if (!String.IsNullOrEmpty(name))
                {
                    namesList.Add(name);
                }
            }

            foreach (var moniker in monikerList)
            {
                Marshal.ReleaseComObject(moniker);
            }

            return namesList;
        }

        /// <summary>
        /// Returns a list of monikers of the devices of
        /// the type 'category'
        /// </summary>
        /// <param name="category">device category</param>
        private static List<IMoniker> getDevicesOfCategory(Guid category)
        {
            var monikerList = new List<IMoniker>();
            IEnumMoniker enumMoniker;

            var enumDev = (ICreateDevEnum)new CreateDevEnum();

            var hResult = enumDev.CreateClassEnumerator(category, out enumMoniker, 0);
            if (hResult != S_OK || enumMoniker == null)
            {
                return monikerList;
            }

            try
            {
                try
                {
                    var moniker = new IMoniker[1];
                    while ((enumMoniker.Next(1, moniker, IntPtr.Zero) == 0))
                    {
                        try
                        {
                            monikerList.Add(moniker[0]);
                        }
                        catch
                        {
                            Marshal.ReleaseComObject(moniker[0]);
                        }
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(enumMoniker);
                }
            }
            catch
            {
                foreach (var m in monikerList)
                {
                    Marshal.ReleaseComObject(m);
                }
            }

            return monikerList;
        }

        /// <summary>
        /// Retrieves the property of the specified property name from
        /// the specified moniker
        /// </summary>
        /// <param name="moniker">moniker to retrieve from</param>
        /// <param name="propertyName">name of the property to retrieve</param>
        /// <returns>the property, null if invalid property</returns>
        private static string getPropBagValue(IMoniker moniker, string propertyName)
        {
            string retVal = null;
            object bagObject = null;

            try
            {
                var guid = typeof(IPropertyBag).GUID;
                moniker.BindToStorage(null, null, ref guid, out bagObject);

                var bag = (IPropertyBag)bagObject;

                object val;
                var hResult = bag.Read(propertyName, out val, null);

                if (hResult != S_OK)
                {
                    return retVal;
                }

                retVal = val as string;
            }
            catch
            {
                retVal = null;
            }
            finally
            {
                if (bagObject != null)
                {
                    Marshal.ReleaseComObject(bagObject);
                }
            }

            return retVal;
        }
    }

    [ComImport, Guid("62BE5D10-60EB-11d0-BD3B-00A0C911CE86")]
    public class CreateDevEnum
    {
    }
}