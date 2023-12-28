////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Interface to send keystrokes to the active application by
    /// inserting them into the Windows keyboard buffer.
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