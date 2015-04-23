////////////////////////////////////////////////////////////////////////////
// <copyright file="IKeyboard.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Interface to send keystrokes to the active application
    /// </summary>
    public interface IKeyboard : IDisposable
    {
        /// <summary>
        /// Simulates a keydown for a modifier key such as
        /// CTRL, SHIFT etc
        /// </summary>
        /// <param name="key">modifier key</param>
        void ExtendedKeyDown(Keys key);

        /// <summary>
        /// Simulates a keyup for a modifier key such as
        /// CTRL, SHIFT etc
        /// </summary>
        /// <param name="key">modifier key</param>
        void ExtendedKeyUp(Keys key);

        /// <summary>
        /// Simulates a keydown for the speified key
        /// </summary>
        /// <param name="key">key to send</param>
        void KeyDown(Keys key);

        /// <summary>
        /// Simulates a keyup for the specified key
        /// </summary>
        /// <param name="key">key to send</param>
        void KeyUp(Keys key);

        /// <summary>
        /// Sends the specified character
        /// </summary>
        /// <param name="ch">character to send</param>
        void Send(char ch);

        /// <summary>
        /// Sends the specified key 'count' times
        /// </summary>
        /// <param name="extendedKey">key to send</param>
        /// <param name="count">how many times?</param>
        void Send(Keys extendedKey, int count);

        /// <summary>
        /// Sends the specified modifiers (SHIFT, CTRL etc) and
        /// the character
        /// </summary>
        /// <param name="extendedKeys">modifier keys</param>
        /// <param name="ch">char to send</param>
        void Send(IEnumerable<Keys> extendedKeys, char ch);

        /// <summary>
        /// Sends the specified modifiers (SHIFT, CTRL etc) and
        /// the key
        /// </summary>
        /// <param name="extendedKeys">modifiers</param>
        /// <param name="keyToSend">key to send</param>
        void Send(IEnumerable<Keys> extendedKeys, Keys keyToSend);

        /// <summary>
        /// Sends the specified modifier (SHIFT, CTRL etc) and
        /// the specified character
        /// </summary>
        /// <param name="extendedKey">Modifier key</param>
        /// <param name="ch">character to send</param>
        void Send(Keys extendedKey, char ch);

        /// <summary>
        /// Sens the specified array of keys
        /// </summary>
        /// <param name="keysToSend">array of keys</param>
        void Send(params Keys[] keysToSend);

        /// <summary>
        /// Sens the specified key
        /// </summary>
        /// <param name="keyToSend">key to send</param>
        void Send(Keys keyToSend);

        /// <summary>
        /// Sends the string
        /// </summary>
        /// <param name="str">string to send</param>
        void Send(String str);
    }
}