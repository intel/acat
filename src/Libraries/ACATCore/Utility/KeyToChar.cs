////////////////////////////////////////////////////////////////////////////
// <copyright file="KeyToChar.cs" company="Intel Corporation">
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

using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Maps a Key to its corresponding char value
    /// </summary>
    public class KeyToChar
    {
        /// <summary>
        /// Holds the mapping
        /// </summary>
        private static readonly Dictionary<Keys, char> keyToCharMapping = new Dictionary<Keys, char>();

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        static KeyToChar()
        {
            keyToCharMapping[Keys.A] = translateAlphabetic('a');
            keyToCharMapping[Keys.B] = translateAlphabetic('b');
            keyToCharMapping[Keys.C] = translateAlphabetic('c');
            keyToCharMapping[Keys.D] = translateAlphabetic('d');
            keyToCharMapping[Keys.E] = translateAlphabetic('e');
            keyToCharMapping[Keys.F] = translateAlphabetic('f');
            keyToCharMapping[Keys.G] = translateAlphabetic('g');
            keyToCharMapping[Keys.H] = translateAlphabetic('h');
            keyToCharMapping[Keys.I] = translateAlphabetic('i');
            keyToCharMapping[Keys.J] = translateAlphabetic('j');
            keyToCharMapping[Keys.K] = translateAlphabetic('k');
            keyToCharMapping[Keys.L] = translateAlphabetic('l');
            keyToCharMapping[Keys.M] = translateAlphabetic('m');
            keyToCharMapping[Keys.N] = translateAlphabetic('n');
            keyToCharMapping[Keys.O] = translateAlphabetic('o');
            keyToCharMapping[Keys.P] = translateAlphabetic('p');
            keyToCharMapping[Keys.Q] = translateAlphabetic('q');
            keyToCharMapping[Keys.R] = translateAlphabetic('r');
            keyToCharMapping[Keys.S] = translateAlphabetic('s');
            keyToCharMapping[Keys.T] = translateAlphabetic('t');
            keyToCharMapping[Keys.U] = translateAlphabetic('u');
            keyToCharMapping[Keys.V] = translateAlphabetic('v');
            keyToCharMapping[Keys.W] = translateAlphabetic('w');
            keyToCharMapping[Keys.X] = translateAlphabetic('x');
            keyToCharMapping[Keys.Y] = translateAlphabetic('y');
            keyToCharMapping[Keys.Z] = translateAlphabetic('z');

            keyToCharMapping[Keys.D0] = translateDn(')', '0');
            keyToCharMapping[Keys.D1] = translateDn('!', '1');
            keyToCharMapping[Keys.D2] = translateDn('@', '2');
            keyToCharMapping[Keys.D3] = translateDn('#', '3');
            keyToCharMapping[Keys.D4] = translateDn('$', '4');
            keyToCharMapping[Keys.D5] = translateDn('%', '5');
            keyToCharMapping[Keys.D6] = translateDn('^', '6');
            keyToCharMapping[Keys.D7] = translateDn('&', '7');
            keyToCharMapping[Keys.D8] = translateDn('*', '8');
            keyToCharMapping[Keys.D9] = translateDn('(', '9');

            keyToCharMapping[Keys.NumPad0] = translateNumpad('0');
            keyToCharMapping[Keys.NumPad1] = translateNumpad('1');
            keyToCharMapping[Keys.NumPad2] = translateNumpad('2');
            keyToCharMapping[Keys.NumPad3] = translateNumpad('3');
            keyToCharMapping[Keys.NumPad4] = translateNumpad('4');
            keyToCharMapping[Keys.NumPad5] = translateNumpad('5');
            keyToCharMapping[Keys.NumPad6] = translateNumpad('6');
            keyToCharMapping[Keys.NumPad7] = translateNumpad('7');
            keyToCharMapping[Keys.NumPad8] = translateNumpad('8');
            keyToCharMapping[Keys.NumPad9] = translateNumpad('9');
            keyToCharMapping[Keys.Decimal] = translateNumpad('.');

            keyToCharMapping[Keys.OemBackslash] = translateOem('|', '\\');
            keyToCharMapping[Keys.OemCloseBrackets] = translateOem('}', ']');
            keyToCharMapping[Keys.Oemcomma] = translateOem('<', ',');
            keyToCharMapping[Keys.OemMinus] = translateOem('_', '-');
            keyToCharMapping[Keys.OemOpenBrackets] = translateOem('{', '[');
            keyToCharMapping[Keys.OemPeriod] = translateOem('>', '.');
            keyToCharMapping[Keys.OemPipe] = translateOem('|', '\\');
            keyToCharMapping[Keys.Oemplus] = translateOem('+', '=');
            keyToCharMapping[Keys.OemQuestion] = translateOem('?', '/');
            keyToCharMapping[Keys.OemQuotes] = translateOem('"', '\'');
            keyToCharMapping[Keys.OemSemicolon] = translateOem(':', ';');
            keyToCharMapping[Keys.Oemtilde] = translateOem('~', '`');

            keyToCharMapping[Keys.Add] = translateOther('+');
            keyToCharMapping[Keys.Divide] = translateOther('/');
            keyToCharMapping[Keys.Multiply] = translateOther('*');
            keyToCharMapping[Keys.Subtract] = translateOther('-');
            keyToCharMapping[Keys.Space] = translateOther(' ');
            keyToCharMapping[Keys.Tab] = translateOther('\t');
        }

        /// <summary>
        /// Returns the char corresponding to the specified Key
        /// </summary>
        /// <param name="key">Key to lookup</param>
        /// <returns>the char</returns>
        public static char GetChar(Keys key)
        {
            return (keyToCharMapping.ContainsKey(key) ? keyToCharMapping[key] : (char)0);
        }

        /// <summary>
        /// Helper function
        /// </summary>
        /// <param name="ch">input character</param>
        /// <returns>output character</returns>
        private static char translateAlphabetic(char ch)
        {
            var capslock = KeyStateTracker.IsCapsLockOn();
            var shift = KeyStateTracker.IsShiftKeyDown() || KeyStateTracker.IsShiftOn();

            return (capslock ^ shift) ? char.ToUpper(ch) : ch;
        }

        /// <summary>
        /// Helper function
        /// </summary>
        /// <param name="ch">input character</param>
        /// <returns>output character</returns>
        private static char translateDn(char ch1, char ch2)
        {
            var shift = KeyStateTracker.IsShiftKeyDown() || KeyStateTracker.IsShiftOn();

            return shift ? ch1 : ch2;
        }

        /// <summary>
        /// Helper function
        /// </summary>
        /// <param name="ch">input character</param>
        /// <returns>output character</returns>
        private static char translateNumpad(char ch)
        {
            bool shift = KeyStateTracker.IsShiftKeyDown() || KeyStateTracker.IsShiftOn();
            bool numlock = KeyStateTracker.IsNumLockOn();

            return (numlock && !shift) ? ch : (char)0;
        }

        /// <summary>
        /// Helper function
        /// </summary>
        /// <param name="ch">input character</param>
        /// <returns>output character</returns>
        private static char translateOem(char ch1, char ch2)
        {
            var shift = KeyStateTracker.IsShiftKeyDown() || KeyStateTracker.IsShiftOn();

            return shift ? ch1 : ch2;
        }

        /// <summary>
        /// Helper function
        /// </summary>
        /// <param name="ch">input character</param>
        /// <returns>output character</returns>
        private static char translateOther(char ch)
        {
            return ch;
        }
    }
}