////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConvAssistUtils.cs
//
// Contains static functions for filtering characters, matching prefixes etc.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    public class ConvAssistUtils
    {
        private static byte[] _byteBuffer = new byte[10240];

        public static string DefaultEncodingToUTF8(string input)
        {
            int length = Encoding.Default.GetBytes(input, 0, input.Length, _byteBuffer, 0);

            return Encoding.UTF8.GetString(_byteBuffer, 0, length);
        }

        public static String FilterChars(String chars, String input)
        {
            var removedChars = input.Select(ch => chars.Contains(ch) ? (char?)null : ch);

            return string.Concat(removedChars.ToArray());
        }

        /// <summary>
        /// Checks to see how much of the prefix matches with the
        /// specified word. The preference setting
        /// WordPredictionFilterMatchPrefixLengthAdjust controls
        /// how many chars to match.  If a match if found, returns true
        /// </summary>
        /// <param name="prefix">partially entered word</param>
        /// <param name="word">word to match with</param>
        /// <returns>true if a match was found</returns>
        public static bool MatchPrefix(String prefix, String word)
        {
            if (!Common.AppPreferences.WordPredictionFilterMatchPrefix)
            {
                return true;
            }

            prefix = prefix.Trim();
            if (String.IsNullOrEmpty(prefix))
            {
                return true;
            }

            int numCharsToMatch = prefix.Length - Common.AppPreferences.WordPredictionFilterMatchPrefixLengthAdjust;
            if (numCharsToMatch <= 0)
                numCharsToMatch = prefix.Length;

            if (numCharsToMatch > 0)
            {
                if (word.Length > numCharsToMatch)
                    word = word.Substring(0, numCharsToMatch);
                if (prefix.Length > numCharsToMatch)
                    prefix = prefix.Substring(0, numCharsToMatch);
            }

            return (word.Length > prefix.Length) ?
                word.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) :
                prefix.StartsWith(word, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string RemoveApostrophes(string inputStr, bool sentence = false)
        {
            string outputStr = string.Empty;
            try
            {
                outputStr = inputStr.Trim(new char[] { (char)39 });
            }
            catch (Exception)
            {
                if (!sentence)
                    outputStr = RemoveSpecialCharacters(inputStr, false);
                //else
                //outputStr = RemoveSpecialCharactersSenetnces(inputStr, false);
            }
            return outputStr;
        }

        /// <summary>
        /// Removes special characters from the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str, bool includeApostrophes = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' 
                        || c == '_' || (includeApostrophes && c == '\''))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpecialCharactersSentences(string str, bool includeApostrophes = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
                    c == '.' || c == '_' || c == ' ' || c == ',' || (includeApostrophes && c == '\''))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static List<KeyValuePair<string, double>> ToList(List<string> predictions)
        {
            List<KeyValuePair<string, double>> newList = new List<KeyValuePair<string, double>>();
            try
            {
                //Create Dictionary of each to set the number value as a Double
                IDictionary<string, Double> sentences = new Dictionary<string, Double>();
                foreach (string predict in predictions)
                {
                    int lastCommaIndex = predict.LastIndexOf(',');
                    string substringBeforeLastComma = predict.Substring(0, lastCommaIndex);
                    string substringAfterLastComma = predict.Substring(lastCommaIndex + 1);
                    string[] values = { substringBeforeLastComma, substringAfterLastComma };
                    sentences.Add(values[0], Double.Parse(values[1]));
                }
                newList = sentences.ToList();
            }
            catch (Exception es)
            {
                Log.Debug("ConvAssist Predict " + es);
            }
            return newList;
        }

        public static string UTF8EncodingToDefault(string input)
        {
            int length = Encoding.UTF8.GetBytes(input, 0, input.Length, _byteBuffer, 0);

            return Encoding.Default.GetString(_byteBuffer, 0, length);
        }

        public static async Task<T> WithTimeout<T>(Task<T> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                return await task; // Task completed within timeout
            }
            else
            {
                throw new TimeoutException("The task did not complete within the given time.");
            }
        }

        public static String RemoveEnclosingCharacters(String str, char ch)
        {
            if (str.StartsWith(ch.ToString()) && str.EndsWith(ch.ToString()))
            {
                return str.Substring(1, str.Length - 2);
            }

            return str;
        }

        public static String RemoveEnclosingQuotes(String str)
        {
            if (str.StartsWith("'") || str.StartsWith("\""))
            {
                str = str.Substring(1);
            }

            if (str.EndsWith("'") || str.EndsWith("\""))
            {
                str = str.Substring(0, str.Length - 1);
            }   
            
            return str;
        }

        public static String RemoveEnclosingCharacters(String str, char ch)
        {
            if (str.StartsWith(ch.ToString()) && str.EndsWith(ch.ToString()))
            {
                return str.Substring(1, str.Length - 2);
            }

            return str;
        }

        public static String RemoveEnclosingQuotes(String str)
        {
            if (str.StartsWith("'") || str.StartsWith("\""))
            {
                str = str.Substring(1);
            }

            if (str.EndsWith("'") || str.EndsWith("\""))
            {
                str = str.Substring(0, str.Length - 1);
            }   
            
            return str;
        }
    }
}