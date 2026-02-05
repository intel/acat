////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace ACATCore.Tests.Logging
{
    /// <summary>
    /// Shared utilities for managing test workspace and cleanup
    /// </summary>
    public static class TestWorkspace
    {
        private static readonly object lockObj = new object();
        private static readonly List<string> activeFolders = new List<string>();
        private static int folderCounter = 0;

        public static string CreateIsolatedFolder()
        {
            lock (lockObj)
            {
                int id = Interlocked.Increment(ref folderCounter);
                string uniqueName = $"test_workspace_{id}_{DateTime.UtcNow.Ticks}";
                string fullPath = Path.Combine(Path.GetTempPath(), uniqueName);
                
                Directory.CreateDirectory(fullPath);
                activeFolders.Add(fullPath);
                
                return fullPath;
            }
        }

        public static void CleanupAll()
        {
            lock (lockObj)
            {
                foreach (string folder in activeFolders)
                {
                    RemoveFolderSafely(folder);
                }
                activeFolders.Clear();
            }
        }

        private static void RemoveFolderSafely(string path)
        {
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    return;
                }
                catch
                {
                    Thread.Sleep(150);
                }
            }
        }

        public static string ReadFileWithRetry(string filePath)
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(
                        File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    if (attempt == 4) throw;
                    Thread.Sleep(100);
                }
            }
            return string.Empty;
        }

        public static bool WaitForFile(string path, int maxWaitMs = 3000)
        {
            int elapsed = 0;
            while (elapsed < maxWaitMs)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) { }
                        return true;
                    }
                    catch { }
                }
                Thread.Sleep(100);
                elapsed += 100;
            }
            return false;
        }
    }
}
