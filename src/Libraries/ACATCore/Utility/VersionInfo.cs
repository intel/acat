////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// VersionInfo.cs
//
// Records the current version of ACAT in the Public Documents folder.
// This information can be used to upgrade to new versions
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ACAT.Lib.Core.Utility
{
    [Serializable]
    public class VersionInfo : PreferencesBase
    {
        [NonSerialized, XmlIgnore]
        private const String VersionInfoFileName = "VersionInfo.xml";

        [NonSerialized, XmlIgnore]
        private const String VersionInfoFolderName = "ACAT";

        [NonSerialized, XmlIgnore]
        private bool forUser;

        public VersionInfo()
        {
            MajorVersion = 2;
            MinorVersion = 0;
            forUser = false;
        }

        public int MajorVersion { get; set; }

        public int MinorVersion { get; set; }

        public static Tuple<int, int> GetCurrentVersion()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            return new Tuple<int, int>(entryAssembly.GetName().Version.Major, entryAssembly.GetName().Version.Minor);
        }

        public static Tuple<int, int> GetPreviousInstalledVersion()
        {
            var versionInfo = VersionInfo.load();

            return new Tuple<int, int>(versionInfo.MajorVersion, versionInfo.MinorVersion);
        }

        public static Tuple<int, int> GetPreviousInstalledVersionForUser()
        {
            var versionInfo = VersionInfo.load(true);

            return new Tuple<int, int>(versionInfo.MajorVersion, versionInfo.MinorVersion);
        }

        public static bool SaveCurrentVersion()
        {
            var tuple = GetCurrentVersion();

            var versionInfo = new VersionInfo
            {
                MajorVersion = tuple.Item1,
                MinorVersion = tuple.Item2
            };

            return versionInfo.Save();
        }

        public static bool SaveCurrentVersionForUser()
        {
            var tuple = GetCurrentVersion();

            var versionInfo = new VersionInfo
            {
                MajorVersion = tuple.Item1,
                MinorVersion = tuple.Item2,
                forUser = true
            };

            return versionInfo.Save();
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            var fileName = (forUser) ? getVersionInfoFileNameForUser() : getVersionInfoFileName();
            return Save<VersionInfo>(this, fileName);
        }

        private static string getVersionInfoFileName()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) +
                                                "\\" + VersionInfoFolderName +
                                                "\\" + VersionInfoFileName;
        }

        private static String getVersionInfoFileNameForUser()
        {
            return UserManager.GetUserDir(UserManager.CurrentUser) + "\\" + VersionInfoFileName;
        }

        private static VersionInfo load(bool forUser = false)
        {
            var fileName = getVersionInfoFileName();
            var dir = new FileInfo(fileName).Directory.FullName;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var versionInfo = PreferencesBase.Load<VersionInfo>(fileName);

            versionInfo.forUser = forUser;

            return versionInfo;
        }
    }
}