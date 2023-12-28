////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
// 
// Handles the HTML files to open local files in a web browser
//
////////////////////////////////////////////////////////////////////////////


using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ACAT.Lib.Core.Utility
{
    public class HtmlUtils
    {
        /// <summary>
        /// Paths of the browsers exe 
        /// </summary>
        private static string _chromeBrowserPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

        private static string _edgeBrowserPath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

        //private static string _firefoxBrowserPath = @"C:\Program Files\Mozilla Firefox\firefox.exe"; // Uncomment if would be used

        private enum HtmlDataArrayIndex
        {
            FILETYPE = 0,
            HTMLSCRIPTINLINE,
            HTMLSCRIPT,
            FILENAME,
            PARAMETER,
        };
        /// <summary>
        /// Loads and launch a html document
        /// Modify sections of the document and create a temporary html file to be open by a browser and display a video or a PDF
        /// </summary>
        /// <param name="path">Base path of the files</param>
        /// <param name="htmlData">Data to load specific file and section</param>
        public static void LoadHtml(string path, string[] htmlData)
        {
            try
            {
                
                string htmlsPath = SmartPath.DocsPath;
                string htmlContent = string.Empty;


                if (htmlData.Length != 5)
                {
                    return;
                }
                
                htmlData[(int)HtmlDataArrayIndex.FILENAME] = ReplaceDash(htmlData[(int)HtmlDataArrayIndex.FILENAME]);

                switch (htmlData[(int)HtmlDataArrayIndex.FILETYPE])
                {
                    case "PDF":
                        if (htmlData[(int)HtmlDataArrayIndex.HTMLSCRIPTINLINE] == "false")
                        {
                            string htmlBaseScriptPath = Path.Combine(SmartPath.DocsPath, htmlData[(int)HtmlDataArrayIndex.HTMLSCRIPT]);
                            if (File.Exists(htmlBaseScriptPath))
                            {
                                htmlContent = File.ReadAllText(htmlBaseScriptPath);
                            }
                        }
                        else
                        {
                            htmlContent = htmlData[(int)HtmlDataArrayIndex.HTMLSCRIPT];
                        }
                        string pdfPath = SmartPath.DocsPath;
                        if (File.Exists(Path.Combine(pdfPath, htmlData[(int)HtmlDataArrayIndex.FILENAME])))
                        {
                            htmlData[(int)HtmlDataArrayIndex.PARAMETER] = ReplaceDash(htmlData[(int)HtmlDataArrayIndex.PARAMETER]);
                                
                            htmlContent = htmlContent
                                .Replace(CoreGlobals.MacroAssetsImagesDir, FileUtils.GetImagesDir())
                                .Replace("FILE_PATH", "file:///" + (EncodeString(pdfPath) + "/" + htmlData[(int)HtmlDataArrayIndex.FILENAME]) +
                                            "#" + htmlData[(int)HtmlDataArrayIndex.PARAMETER]);
                        }
                        
                        break;

                    case "Video":
                        var videoFileName = htmlData[(int)HtmlDataArrayIndex.FILENAME];
                        var fullPath = Path.Combine(FileUtils.GetVideosDir(), videoFileName);
                        if (File.Exists(fullPath))
                        {
                            Process.Start(fullPath);
                        }
                        return;
                    /*
                    string videoPath = Path.Combine(path, "Assets", "Videos");
                    if (File.Exists(Path.Combine(videoPath, htmlData[(int)FileValues.FILENAME])))
                    {
                        if (!Regex.IsMatch(htmlData[(int)FileValues.PARAMETER], @"^\d+$"))
                        {
                            htmlData[(int)FileValues.PARAMETER] = "0";
                        }
                        htmlContent = File.ReadAllText(htmlBaseScriptPath);
                        htmlContent = htmlContent
                            .Replace("$$$", "file:///" + EncodeString(htmlsPath))
                            .Replace("FILE_PATH", "file:///" + (EncodeString(videoPath) + "/" + htmlData[(int)FileValues.FILENAME]))
                            .Replace("TIME_STAMP", htmlData[(int)FileValues.PARAMETER]);
                    }
                    break;
                    */

                    default:
                        break;
                }

                string tempFilePathLogs = Path.Combine(FileUtils.GetLogsDir(), "TempFile.html");
                try
                {
                    using (StreamWriter writer = new StreamWriter(tempFilePathLogs))
                    {
                        writer.Write(htmlContent);
                    }
                }
                catch (Exception exp)
                {
                    Console.Write(exp.Message);
                }
                // Best results are in Chrome or Edge Browser
                //First option to open files is edge since is common that users have this installed by default
                if (File.Exists(_edgeBrowserPath))
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = _edgeBrowserPath,
                        Arguments = $"\"{tempFilePathLogs}\"",
                    };
                    processStartInfo.WorkingDirectory = Path.GetDirectoryName(tempFilePathLogs);
                    Process.Start(processStartInfo);

                    // Code to open Html local files using Firefox
                    /*var processStartInfo = new ProcessStartInfo
                    {
                        FileName = "firefox.exe",
                        Arguments = $"/c start \"{tempFilePathLogs}\"",
                    };
                    processStartInfo.WorkingDirectory = Path.GetDirectoryName(tempFilePathLogs);
                    Process.Start(processStartInfo);*/
                }
                else if (File.Exists(_chromeBrowserPath))
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = _chromeBrowserPath,
                        Arguments = $"/c \"{tempFilePathLogs}\"",
                    };
                    processStartInfo.WorkingDirectory = Path.GetDirectoryName(tempFilePathLogs);
                    Process.Start(processStartInfo);


                }
                else
                {
                    // Launch using default web browser
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = tempFilePathLogs,
                    };
                    processStartInfo.WorkingDirectory = Path.GetDirectoryName(tempFilePathLogs);
                    Process.Start(processStartInfo);
                }

             }
            catch (Exception ex)
            {
                Log.Debug("Error loading HTML script: " + ex.Message);
            }

        }
        /// <summary>
        /// Custom encoder for html URL path for local files 
        /// The encoder is for special characters so the browser can read local files when opening a html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncodeString(string text)
        {
            text = text.Replace("\\", "/");
            StringBuilder encodedStringBuilder = new StringBuilder();
            foreach (char c in text)
            {
                //Exlclude cases. so the path is readeable by the browser for Videos or objects sources
                if (c == '/' || c == ':')
                    encodedStringBuilder.Append(c);
                else
                {
                    string encodedCharacter = Uri.EscapeDataString(c.ToString());
                    encodedStringBuilder.Append(encodedCharacter);
                }
            }
            return encodedStringBuilder.ToString();
        }

        /// <summary>
        /// Replaces the Dash character to space so the file does not take it as a reserved word from HTML
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ReplaceDash(string text)
        {
            return text.Replace("-", " ");
        }
    }
}
