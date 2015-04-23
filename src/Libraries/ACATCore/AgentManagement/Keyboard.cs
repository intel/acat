////////////////////////////////////////////////////////////////////////////
// <copyright file="Keyboard.cs" company="Intel Corporation">
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;

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
    /// Sends keystokes to the active window.
    /// </summary>
    public class Keyboard : IKeyboard
    {
        /// <summary>
        /// Windows keyboard layout
        /// </summary>
        private readonly IntPtr _keyboardLayout = User32Interop.GetKeyboardLayout(0);

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        private enum Flags
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Simulates a keydown for a modifier key such as
        /// CTRL, SHIFT etc
        /// </summary>
        /// <param name="key">modifier key</param>
        public void ExtendedKeyDown(Keys key)
        {
            Log.Debug(key.ToString());
            User32Interop.keybd_event((byte)key, 0xAA, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Simulates a keyup for a modifier key such as
        /// CTRL, SHIFT etc
        /// </summary>
        /// <param name="key">modifier key</param>
        public void ExtendedKeyUp(Keys key)
        {
            Log.Debug(key.ToString());
            User32Interop.keybd_event((byte)key, 0xAA, (uint)Flags.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        /// <summary>
        /// Simulates a key down of the specified key
        /// </summary>
        /// <param name="key">the key</param>
        public void KeyDown(Keys key)
        {
            ExtendedKeyDown(key);
        }

        /// <summary>
        /// Simulates a key up of the specified key
        /// </summary>
        /// <param name="key">the key</param>
        public void KeyUp(Keys key)
        {
            ExtendedKeyUp(key);
        }

        /// <summary>
        /// Sends the specified character
        /// </summary>
        /// <param name="ch">character to send</param>
        public void Send(char ch)
        {
            AgentManager.Instance.TextControlAgent.SetFocus();

            bool shift = shiftNeeded(ch);

            if (shift)
            {
                ExtendedKeyDown(Keys.LShiftKey);
            }

            keyPress(ch);

            if (shift)
            {
                ExtendedKeyUp(Keys.LShiftKey);
            }
        }

        /// <summary>
        /// Sends the specified key 'count' times
        /// </summary>
        /// <param name="extendedKey">key to send</param>
        /// <param name="count">how many times?</param>
        public void Send(Keys extendedKey, int count)
        {
            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                ExtendedKeyUp(Keys.LShiftKey);

                for (int ii = 0; ii < count; ii++)
                {
                    Send(extendedKey);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                AgentManager.Instance.TextChangedNotifications.Release();
            }
        }

        /// <summary>
        /// Sends the specified modifiers (SHIFT, CTRL etc) and
        /// the character
        /// </summary>
        /// <param name="extendedKeys">modifier keys</param>
        /// <param name="ch">char to send</param>
        public void Send(IEnumerable<Keys> extendedKeys, char ch)
        {
            AgentManager.Instance.TextControlAgent.SetFocus();

            var enumerable = extendedKeys as IList<Keys> ?? extendedKeys.ToList();
            foreach (Keys key in enumerable)
            {
                ExtendedKeyDown(key);
            }

            if (!trySendExtendedASCII(ch))
            {
                sendChar(ch);
            }

            foreach (Keys key in enumerable)
            {
                ExtendedKeyUp(key);
            }
        }

        /// <summary>
        /// Sends the specified modifiers (SHIFT, CTRL etc) and
        /// the key
        /// </summary>
        /// <param name="extendedKeys">modifiers</param>
        /// <param name="keyToSend">key to send</param>
        public void Send(IEnumerable<Keys> extendedKeys, Keys keyToSend)
        {
            AgentManager.Instance.TextControlAgent.SetFocus();

            var enumerable = extendedKeys as IList<Keys> ?? extendedKeys.ToList();
            foreach (Keys key in enumerable)
            {
                ExtendedKeyDown(key);
            }

            ExtendedKeyDown(keyToSend);
            ExtendedKeyUp(keyToSend);

            foreach (Keys key in enumerable)
            {
                ExtendedKeyUp(key);
            }
        }

        /// <summary>
        /// Sends the specified modifier (SHIFT, CTRL etc) and
        /// the specified character
        /// </summary>
        /// <param name="extendedKey">Modifier key</param>
        /// <param name="ch">character to send</param>
        public void Send(Keys extendedKey, char ch)
        {
            AgentManager.Instance.TextControlAgent.SetFocus();

            ExtendedKeyDown(extendedKey);

            if (!trySendExtendedASCII(ch))
            {
                sendChar(ch);
            }

            ExtendedKeyUp(extendedKey);
        }

        /// <summary>
        /// Sens the specified array of keys
        /// </summary>
        /// <param name="keysToSend">array of keys</param>
        public void Send(params Keys[] keysToSend)
        {
            for (int ii = 0; ii < keysToSend.Length - 1; ii++)
            {
                ExtendedKeyDown(keysToSend[ii]);
            }

            ExtendedKeyDown(keysToSend[keysToSend.Length - 1]);
            ExtendedKeyUp(keysToSend[keysToSend.Length - 1]);

            for (int ii = 0; ii < keysToSend.Length - 1; ii++)
            {
                ExtendedKeyUp(keysToSend[ii]);
            }
        }

        /// <summary>
        /// Send the key to the keyboard buffer
        /// </summary>
        /// <param name="keyToSend">key</param>
        public void Send(Keys keyToSend)
        {
            AgentManager.Instance.TextControlAgent.SetFocus();

            ExtendedKeyDown(keyToSend);
            ExtendedKeyUp(keyToSend);
        }

        /// <summary>
        /// Sends the string
        /// </summary>
        /// <param name="str">string to send</param>
        public void Send(String str)
        {
            AgentManager.Instance.TextControlAgent.SetFocus();

            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                foreach (char ch in str)
                {
                    Send(ch);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                AgentManager.Instance.TextChangedNotifications.Release();
            }
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Sends a key down and key up event for the specified
        /// character
        /// </summary>
        /// <param name="ch">character to send</param>
        private void keyPress(char ch)
        {
            if (!trySendExtendedASCII(ch))
            {
                sendChar(ch);
            }

            SendKeys.Flush();
            Thread.Sleep(10);
        }

        /// <summary>
        /// Sends a keydown event for the character
        /// </summary>
        /// <param name="ch">the character</param>
        private void sendChar(char ch)
        {
            if (ch == ' ')
            {
                SendKeys.SendWait(" ");
            }
            else if (ch == 0x0A)
            {
                SendKeys.SendWait("{ENTER}");
            }
            else if (ch == 0x08)
            {
                SendKeys.SendWait("{BACKSPACE}");
            }
            else
            {
                SendKeys.SendWait("{" + ch + "}");
            }
        }

        /// <summary>
        /// Indicates whether a shift needs to be pressed for the
        /// specified character.
        /// </summary>
        /// <param name="c">the character</param>
        /// <returns>true if needed</returns>
        private bool shiftNeeded(char c)
        {
            short virtualKey = User32Interop.VkKeyScanEx(c, _keyboardLayout);
            Log.Debug("virtualKey for [" + c + "] is " + virtualKey);
            if ((virtualKey & 0x100) == 0x100)
            {
                Log.Debug("Shift needs to be pressed for [" + c + "]");
                return true;
            }

            Log.Debug("Shift does not need to be pressed for [" + c + "]");
            return false;
        }

        /// <summary>
        /// Converts the character into a virtual key code and if it
        /// converts successfully, sends it to the keyboard buffer.
        /// Otherwise returns false
        /// </summary>
        /// <param name="ch">char to send</param>
        /// <returns>true if sent successfully</returns>
        private bool trySendExtendedASCII(char ch)
        {
            bool retVal = true;
            int scanCode = User32Interop.VkKeyScan(ch);

            if (scanCode < 0)
            {
                String str = Encoding.Default.GetString(new[] { (byte)ch });
                SendKeys.SendWait(str);
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }
    }
}