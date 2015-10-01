////////////////////////////////////////////////////////////////////////////
// <copyright file="TextDocumentReader.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ACAT.Lib.Core.Utility;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Extensions.Default.FunctionalAgents.LectureManager
{
    /// <summary>
    /// Handles reading of a text file or a word document.  Returns
    /// the text read
    /// </summary>
    internal class TextDocumentReader
    {
        /// <summary>
        /// Reads the input file and returns the text contained
        /// in it.
        /// Only supported file formats are .txt, .doc and .docx
        /// </summary>
        /// <param name="fileName">Input filename</param>
        /// <returns>Text from the file</returns>
        public String GetText(string fileName)
        {
            string strSpeechBuffer = String.Empty;

            string fileExtension = Path.GetExtension(fileName).ToLower();

            try
            {
                if (fileExtension.Equals(".txt") || String.IsNullOrEmpty(fileExtension))
                {
                    strSpeechBuffer = File.ReadAllText(fileName, Encoding.Default);
                }
                else if (fileExtension.Equals(".doc") || fileExtension.Equals(".docx"))
                {
                    strSpeechBuffer = getTextFromWordFile(fileName);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error reading from file " + fileName + ", ex: " + ex.ToString());
            }

            return strSpeechBuffer;
        }

        /// <summary>
        /// Read text from the speficied word document.  Uses
        /// office interop to do this
        /// </summary>
        /// <param name="wordFileName">name of the word document</param>
        /// <returns>text from the document, empty string if exception/error</returns>
        private string getTextFromWordFile(string wordFileName)
        {
            bool comCreated = false;
            String retVal = String.Empty;
            try
            {
                Application wordApp = null;
                try
                {
                    wordApp = (Application)Marshal.GetActiveObject("Word.Application");
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    wordApp = null;
                }

                if (wordApp == null)
                {
                    wordApp = new Application();
                }
                else
                {
                    comCreated = true;
                }

                Documents docs = wordApp.Documents;
                bool docAlreadyOpen = false;
                _Document doc = null;
                foreach (Document document in docs)
                {
                    String fullName = Path.Combine(document.Path, document.Name);
                    if (String.Compare(fullName, wordFileName, true) == 0)
                    {
                        docAlreadyOpen = true;
                        doc = document;
                        break;
                    }
                }

                if (doc == null)
                {
                    wordApp.WordBasic.DisableAutoMacros(1);

                    object isMissing = Missing.Value;
                    object path = wordFileName;
                    object readOnly = true;

                    doc = wordApp.Documents.Open(
                                ref path,
                                ref isMissing,
                                ref readOnly,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing,
                                ref isMissing);
                }

                Revisions revs = doc.Revisions;
                String text = String.Empty;
                if (revs.Count != 0)
                {
                    WdRevisionsView v = doc.ActiveWindow.View.RevisionsView;
                    doc.ActiveWindow.View.RevisionsView = WdRevisionsView.wdRevisionsViewFinal;
                    bool showRevisions = doc.ActiveWindow.View.ShowRevisionsAndComments;
                    doc.ActiveWindow.View.ShowRevisionsAndComments = false;
                    text = doc.Content.Text;
                    doc.ActiveWindow.View.RevisionsView = v;
                    doc.ActiveWindow.View.ShowRevisionsAndComments = showRevisions;
                }
                else
                {
                    text = doc.Content.Text;
                }

                retVal = Regex.Replace(text, "\r(?<!\n)", "\r\n");

                if (!docAlreadyOpen)
                {
                    doc.Close(false);
                }

                if (comCreated)
                {
                    Marshal.ReleaseComObject(wordApp);
                }
                else
                {
                    ((Microsoft.Office.Interop.Word._Application)wordApp).Quit();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return retVal;
        }
    }
}