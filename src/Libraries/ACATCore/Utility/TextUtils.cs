////////////////////////////////////////////////////////////////////////////
// <copyright file="TextUtils.cs" company="Intel Corporation">
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
using System.Text;
using System.Windows.Forms;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Useful text functions.  The SwiftKeys prediction engine needs the
    /// previous words in the current sentence PLUS the current word to
    /// make the current word and the next word prediction.  This class
    /// contains functions to get the current sentence, current word etc.
    /// </summary>
    public class TextUtils
    {
        private const String SentenceTerminators = ".?!";

        //private static String _terminators = "\".?!,:;‘’“”";
        private const String Terminators = ".?!,:;‘";

        /// <summary>
        /// Converts a byte array into a hex string
        /// </summary>
        /// <param name="byteArray">byte array to convert</param>
        /// <returns>a hex string</returns>
        public static string ByteArrayToHexString(byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder(byteArray.Length * 3);
            foreach (byte data in byteArray)
            {
                sb.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// Capitalize the first letter of the word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static String Capitalize(String word)
        {
            Log.Debug(word);
            if (String.IsNullOrEmpty(word))
            {
                return word;
            }

            // find the first non-whitespace character
            int index = 0;
            while (index < word.Length)
            {
                if (!Char.IsWhiteSpace(word[index]))
                {
                    break;
                }

                index++;
            }

            char c = word[index];
            Log.Debug("index: " + index + "c: " + c.ToString());

            if (!Char.IsLetter(c) || Char.IsUpper(c))
            {
                return null;
            }

            word = word.Remove(index, 1);
            c = Char.ToUpper(c);
            word = word.Insert(index, c.ToString());
            Log.Debug("returning " + word);
            return word;
        }

        /// <summary>
        /// Useful to replace a word in the input string with a new word.  Makes a
        /// determination whether the cursor is within a word or past the word.  If
        /// it is within the word, it means the word has to be replaced with the new word.
        /// If it is past the word (there is no current word), the new word has to be
        /// inserted at the cursor position
        /// ap
        /// </summary>
        /// <param name="inputString">Input text string</param>
        /// <param name="caretPos">Cursor position</param>
        /// <param name="insertOrReplaceOffset">Returns the offset at which to insert/replace</param>
        /// <param name="wordToReplace">Returns the word to replace (null if none)</param>
        /// <returns>true if the word has to be inserted</returns>
        public static bool CheckInsertOrReplaceWord(String inputString, int caretPos, out int insertOrReplaceOffset, out String wordToReplace)
        {
            try
            {
                wordToReplace = String.Empty;
                insertOrReplaceOffset = caretPos;

                if (String.IsNullOrEmpty(inputString))
                {
                    Log.Debug("NULL string. return true " + insertOrReplaceOffset);
                    return true;
                }

                caretPos--;
                if (caretPos < 0)
                {
                    caretPos = 0;
                }

                int len = inputString.Length;
                if (caretPos >= len)
                {
                    caretPos = len - 1;
                }

                int index = caretPos;
                insertOrReplaceOffset = caretPos;
                Log.Debug("index: " + index + ", inputString[index] is [" + inputString[index] + "]");
                // cursor is not within a word
                if (Char.IsWhiteSpace(inputString[index]))
                {
                    insertOrReplaceOffset = caretPos + 1;
                    Log.Debug("iswhiespace. return true " + insertOrReplaceOffset);
                    return true;
                }

                // cursor is at a sentence terminator.  needs
                // insertion
                if (TextUtils.IsSentenceTerminator(inputString[index]))
                {
                    insertOrReplaceOffset = caretPos + 1;
                    Log.Debug("is sentence terminator. return true " + insertOrReplaceOffset);
                    return true;
                }

                // cursor is at a word.  Needs to be replaced
                if (TextUtils.IsWordElement(inputString[index]))
                {
                    insertOrReplaceOffset = TextUtils.getWordToReplace(inputString, caretPos, out wordToReplace);
                    Log.Debug("iswordelement is true.  return false " + insertOrReplaceOffset);
                    return false;
                }
                else
                {
                    insertOrReplaceOffset = caretPos + 1;
                    Log.Debug("iswordelement is false.  return true " + insertOrReplaceOffset);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                insertOrReplaceOffset = 0;
                wordToReplace = String.Empty;
                return true;
            }
        }

        /// <summary>
        /// Given an input string with multiple paragraphs (crlf delimted), returns the
        /// paragraph which has the caret
        /// </summary>
        /// <param name="inputString">String with sentences</param>
        /// <param name="caretPos">Where is the caret positioned?</param>
        /// <param name="paragraphAtCaret">Returns the paragraph at the caret position</param>
        /// <returns>0-based index of the start of the paragraph</returns>
        public static int GetParagraphAtCaret(String inputString, int caretPos, out String paragraphAtCaret)
        {
            try
            {
                paragraphAtCaret = String.Empty;

                if (String.IsNullOrEmpty(inputString.Trim()))
                {
                    return -1;
                }

                // work backwards from the caret position til
                // we hit the previous new line or
                // the beginning of the string
                int len = inputString.Length;
                int index = (caretPos >= len) ? len - 1 : caretPos;
                int endPos = index;

                while (index >= 0)
                {
                    if (inputString[index] == '\n')
                    {
                        break;
                    }

                    index--;
                }

                int startPos = index + 1;
                int count = endPos - startPos + 1;
                paragraphAtCaret = (count > 0) ? inputString.Substring(startPos, count) : String.Empty;

                return startPos;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                paragraphAtCaret = String.Empty;
                return 0;
            }
        }

        public static int GetPrecedingCharacters(String input, int caretPos, int numberOfChars, out String word)
        {
            try
            {
                word = String.Empty;
                int index = caretPos;
                index -= numberOfChars;
                if (index < 0)
                {
                    return 0;
                }

                word = input.Substring(index, numberOfChars);

                return numberOfChars;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                word = String.Empty;
                return 0;
            }
        }

        /// <summary>
        /// Works backward from the cursor position looking for whitespaces
        /// and stops when it encounters the first non-whitespace character.
        /// Returns the offset of the first whitespace character behind the
        /// cursor and also the number of whitespace characters
        /// </summary>
        /// <param name="inputString">Input text string</param>
        /// <param name="caretPos">current cursor position</param>
        /// <param name="offset">offset of the first whitespace character behind the cursor</param>
        /// <param name="count">number of whitespace characters</param>
        /// <returns></returns>
        public static bool GetPrecedingWhiteSpaces(String inputString, int caretPos, out int offset, out int count)
        {
            count = 0;
            offset = caretPos;

            if (String.IsNullOrEmpty(inputString))
            {
                return true;
            }

            int len = inputString.Length;
            int index = caretPos;
            if (index >= len)
            {
                index = len - 1;
            }
            else
            {
                index--;
            }

            offset = caretPos;

            while (index >= 0 && inputString[index] != 0x0D && inputString[index] != 0x0A && Char.IsWhiteSpace(inputString[index]))
            {
                index--;
            }

            index++;
            offset = index;
            count = caretPos - offset;

            Log.Debug("offset: " + offset + ", caretPos: " + caretPos + ", count: " + count);

            return true;
        }

        /// <summary>
        /// Given a string with multipe sentences, and the position where the caret
        /// is at, returns the previous n words and the current word at the caret
        /// position
        /// </summary>
        /// <param name="inputString">The input string</param>
        /// <param name="caretPos">0-based index positon of the caret</param>
        /// <param name="prefix">Returns the previous n words</param>
        /// <param name="wordAtCaret">Current word at the caret position</param>
        /// <returns>The index offset where the prefix begins</returns>
        ///
        public static int GetPrefixAndWordAtCaret(String inputString, int caretPos, out String prefix, out String wordAtCaret)
        {
            try
            {
                prefix = String.Empty;
                wordAtCaret = String.Empty;

                Log.Debug();

                //Log.Debug("inputstring: [" + inputString + "]");

                // first get the current sentence
                String sentenceAtCaret;
                int startPos = GetSentenceAtCaret(inputString, caretPos, out sentenceAtCaret);
                if (startPos < 0)
                {
                    Log.Debug("returning " + startPos);
                    return startPos;
                }

                if (String.IsNullOrEmpty(sentenceAtCaret))
                {
                    Log.Debug("returning " + startPos);
                    return startPos;
                }

                //Log.Debug("sentence: [" + sentenceAtCaret + "]");

                String w;
                //Log.Debug("Calling getwordatcaret. InputString: [" + inputString + "]");
                int wordPos = GetWordAtCaret(inputString, caretPos, out w);
                Log.Debug("startPos: " + startPos + "wordPos: " + wordPos + " wordPos-startPos: " + (wordPos - startPos));
                Log.Debug("Getwordatcaret returned [" + w + "]");
                int count = wordPos - startPos;
                Log.Debug("count: " + count);
                if (count > 0)
                {
                    Log.Debug("Getting substring: startPos: " + startPos + ", count: " + count);
                }

                prefix = (count > 0) ? inputString.Substring(startPos, count) : String.Empty;

                Log.Debug("prefix: [" + prefix + "]");

                if (!String.IsNullOrEmpty(w))
                {
                    Log.Debug("caretPos: " + caretPos + " wordPos: " + wordPos);
                    count = caretPos - wordPos;

                    if (count > 0)
                    {
                        Log.Debug("calling w.Substring starting at 0, count = " + count);
                    }

                    if (count >= w.Length)
                    {
                        count = w.Length;
                    }

                    wordAtCaret = (count > 0) ? w.Substring(0, count) : String.Empty;
                }
                else
                {
                    wordAtCaret = String.Empty;
                }

                Log.Debug("returning " + startPos);
                return startPos;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                prefix = String.Empty;
                wordAtCaret = String.Empty;
                return 0;
            }
        }

        /// <summary>
        /// Starting from caretPos, works backward in the input string and gets
        /// the previous word
        /// </summary>
        /// <param name="input">input text string</param>
        /// <param name="caretPos">where to start from</param>
        /// <param name="word">returns the previous word</param>
        /// <returns>The starting position of the previous word</returns>
        public static int GetPreviousWord(String input, int caretPos, out String word)
        {
            try
            {
                int startPos = -1;
                word = String.Empty;
                int index = caretPos;

                if (index >= input.Length)
                {
                    index = input.Length - 1;
                }
                else
                {
                    index--;
                }

                // skip all punctuations and white spaces
                while (index >= 0 && input[index] != 0x0A && input[index] != 0x0D && IsPunctuationOrWhiteSpace(input[index]))
                {
                    index--;
                }

                if (index < 0)
                {
                    return -1;
                }

                // now skip all non-whitespace characters
                int endPos = index;
                index--;

                while (index >= 0 && input[index] != 0x0A && input[index] != 0x0D && !IsTerminatorOrWhiteSpace(input[index]))
                {
                    index--;
                }

                // extract the word
                startPos = index + 1;
                int count = endPos - startPos + 1;

                word = (count > 0) ? input.Substring(startPos, count) : String.Empty;

                Log.Debug("word: " + word);

                return startPos;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                word = String.Empty;
                return 0;
            }
        }

        /// <summary>
        /// Works backwards from the caret position and finds the index of the previous word,
        /// and also returns the size of the word
        /// </summary>
        /// <param name="inputString">input text string</param>
        /// <param name="caretPos">cursor position</param>
        /// <param name="offset">Offset of the previous word (0 if beginning reached)</param>
        /// <param name="count">size of the previous word</param>
        /// <returns></returns>
        public static bool GetPrevWordOffset(String inputString, int caretPos, out int offset, out int count)
        {
            try
            {
                count = 0;
                offset = caretPos;

                if (String.IsNullOrEmpty(inputString))
                {
                    return true;
                }

                caretPos--;
                int len = inputString.Length;
                if (caretPos >= len)
                {
                    caretPos = len - 1;
                }
                else if (caretPos < 0)
                {
                    return false;
                }

                int index = caretPos;
                offset = caretPos;
                int endPos = caretPos;

                while (index >= 0 && Char.IsWhiteSpace(inputString[index]))
                {
                    index--;
                }

                while (index >= 0 && IsTerminator(inputString[index]))
                {
                    index--;
                }

                if (index < 0)
                {
                    index = 0;
                    count = endPos + 1;
                }
                else
                {
                    while (index >= 0 && !Char.IsWhiteSpace(inputString[index]) && !TextUtils.IsSentenceTerminator(inputString[index]))
                    {
                        index--;
                    }

                    index++;
                }

                offset = index;
                count = endPos - offset + 1;

                return true;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                offset = 0;
                count = 0;
                return false;
            }
        }

        /// <summary>
        /// Given an input string with multiple sentences, returns the
        /// sentence which has the caret
        /// </summary>
        /// <param name="inputString">String with sentences</param>
        /// <param name="caretPos">Where is the caret positioned?</param>
        /// <param name="sentenceAtCaret">Returns the sentence at the caret position</param>
        /// <returns>0-based index of the start of the sentence</returns>
        ///
        public static int GetSentenceAtCaret(String inputString, int caretPos, out String sentenceAtCaret)
        {
            try
            {
                sentenceAtCaret = String.Empty;

                Log.Debug();
                if (String.IsNullOrEmpty(inputString.Trim()))
                {
                    Log.Debug("returning -1");
                    return -1;
                }

                // work backwards from the caret position til
                // we hit the previous sentence terminator or
                // the beginning of the string
                int len = inputString.Length;
                int index = (caretPos >= len) ? len - 1 : caretPos - 1;  // added -1 to caretpos

                if (index < 0)
                {
                    index = 0;
                }

                if (IsSentenceTerminator(inputString[index]) || IsLineOrParagraphTerminator(inputString[index]))
                {
                    return index;
                }

                int endPos = index;

                while (index >= 0)
                {
                    if (IsSentenceTerminator(inputString[index]) || IsLineOrParagraphTerminator(inputString[index]))
                    {
                        break;
                    }

                    index--;
                }

                int startPos = index + 1;

                int count = endPos - startPos + 1;
                sentenceAtCaret = (count > 0) ? inputString.Substring(startPos, count) : String.Empty;
                Log.Debug("returning " + startPos);
                return startPos;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                sentenceAtCaret = String.Empty;
                return 0;
            }
        }

        public static int GetWordAtCaret(String inputString, int caretPos, out String wordAtCaret)
        {
            try
            {
                int startPos = caretPos;

                //Log.Debug("inputString: [" + inputString + "], caretPos: " + caretPos);
                Log.Debug("caretPos: " + caretPos);

                wordAtCaret = String.Empty;
                if (String.IsNullOrEmpty(inputString))
                {
                    Log.Debug("inputstring is empty. returning");
                    return -1;
                }

                startPos = caretPos;

                if (startPos >= inputString.Length ||
                    inputString[startPos] == ' ' ||
                    inputString[startPos] == 0x0D ||
                    IsTerminator(inputString[startPos]))
                {
                    startPos--;
                }

                if (startPos >= inputString.Length)
                {
                    startPos = inputString.Length - 1;
                }

                while (startPos >= 0)
                {
                    if (!IsWordElement(inputString[startPos]))
                    {
                        break;
                    }

                    startPos--;
                }

                //startPos++;

                if (startPos < 0)
                {
                    startPos = 0;
                }

                //int endPos = caretPos;

                int endPos = caretPos - 1;
                if (endPos < 0)
                {
                    endPos = 0;
                }

                while (endPos < inputString.Length)
                {
                    if (!IsWordElement(inputString[endPos]))
                    {
                        break;
                    }

                    endPos++;
                }

                if (endPos > inputString.Length)
                {
                    endPos = inputString.Length;
                }

                while (startPos < inputString.Length && (inputString[startPos] == ' ' ||
                                    IsTerminator(inputString[startPos]) ||
                                    IsLineOrParagraphTerminator(inputString[startPos])))
                    startPos++;

                if (endPos >= startPos)
                {
                    Log.Debug("wordAtCaret: Getting substring from startPos: " + startPos + ", length: " + (endPos - startPos));
                    wordAtCaret = inputString.Substring(startPos, endPos - startPos).Trim();
                    Log.Debug("wordAtCaret: [" + wordAtCaret + "]");
                }
                else
                {
                    wordAtCaret = String.Empty;
                }

                Log.Debug("returning " + startPos + ", wordatCaret: " + wordAtCaret);
                return startPos;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                wordAtCaret = String.Empty;
                return 0;
            }
        }

        /// <summary>
        /// Converts hex string into a byte array
        /// </summary>
        /// <param name="hexString">string to convert</param>
        /// <returns>a byte array</returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            hexString = hexString.Replace(" ", String.Empty);

            byte[] byteArray = new byte[hexString.Length / 2];

            for (int ii = 0; ii < hexString.Length; ii += 2)
            {
                byteArray[ii / 2] = (byte)Convert.ToByte(hexString.Substring(ii, 2), 16);
            }

            return byteArray;
        }

        public static bool IsLineOrParagraphTerminator(char c)
        {
            return c == 0x0D || c == 0x0A;
        }

        /// <summary>
        /// Starting from startPos, works backward and checks if the first non-space
        /// character is a sentence terminator
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="startPos">Where to start from</param>
        /// <returns>true if so</returns>
        public static bool IsPrevSentenceTerminator(String text, int startPos)
        {
            int index = startPos;
            if (String.IsNullOrEmpty(text))
            {
                return false;
            }

            index--;
            while (index >= 0)
            {
                if (text[index] == 0x0D || text[index] == 0x0A || !Char.IsWhiteSpace(text[index]))
                {
                    break;
                }

                index--;
            }

            if (index < 0 || IsSentenceTerminator(text[index]) || IsLineOrParagraphTerminator(text[index]))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the char is a punctuation or whitespace
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>true if so</returns>
        public static bool IsPunctuationOrWhiteSpace(char ch)
        {
            return Char.IsPunctuation(ch) || Char.IsWhiteSpace(ch);
        }

        /// <summary>
        /// Checks if the character is a sentence terminator
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>true if so</returns>
        public static bool IsSentenceTerminator(char ch)
        {
            return SentenceTerminators.LastIndexOf(ch) >= 0;
        }

        /// <summary>
        /// Checks if the character is a terminator
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>true if so</returns>
        public static bool IsTerminator(char ch)
        {
            return Terminators.LastIndexOf(ch) >= 0;
        }

        /// <summary>
        /// Checks if the char is a terminator or white space`
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>true if so</returns>
        public static bool IsTerminatorOrWhiteSpace(char ch)
        {
            return Char.IsWhiteSpace(ch) || IsTerminator(ch);
        }

        /// <summary>
        /// Checks if the character is a part of a word
        /// </summary>
        /// <param name="c"></param>
        /// <returns>true if so</returns>
        public static bool IsWordElement(char c)
        {
            // 0x2019 is a closed single quote
            var retVal = Char.IsLetterOrDigit(c) || c == '\'' || c == '-' || c == 0x2019;
            return retVal;
        }

        /// <summary>
        /// Gets the word where the caret is positioned in a string of
        /// multiple sentences
        /// </summary>
        /// <param name="inputString">String with multiple sentences</param>
        /// <param name="caretPos">Where is the caret positioned at?</param>
        /// <param name="wordAtCaret">Returns word at caret (can be "")</param>
        /// <returns>0-based index where the word begins</returns>
        ///
        private static int getWordToReplace(String inputString, int caretPos, out String wordAtCaret)
        {
            int startPos = caretPos;

            while (startPos >= 0)
            {
                if (!IsWordElement(inputString[startPos]))
                {
                    break;
                }

                startPos--;
            }

            startPos++;

            if (startPos < 0)
            {
                startPos = 0;
            }

            int endPos = caretPos;
            while (endPos < inputString.Length)
            {
                if (!IsWordElement(inputString[endPos]))
                {
                    break;
                }

                endPos++;
            }

            if (endPos > inputString.Length)
            {
                endPos = inputString.Length;
            }

            Log.Debug("getWordAt: startpos, endpos = " + startPos.ToString() + ", " + endPos.ToString());
            int count = endPos - startPos;
            wordAtCaret = (count > 0) ? inputString.Substring(startPos, endPos - startPos) : String.Empty;

            return startPos;
        }
    }
}