////////////////////////////////////////////////////////////////////////////
// <copyright file="KeyboardHook.cs" company="Intel Corporation">
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
using System.Runtime.InteropServices;
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
#endregion

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// Manages windows low level keyboard hooks to capture global 
    /// keyboard events
    /// </summary>
    internal class KeyboardHook
    {
        /// <summary>
        /// Windows constant
        /// </summary>
        private const byte VK_CAPITAL = 0x14;

        /// <summary>
        /// Windows constant
        /// </summary>
        private const byte VK_SHIFT = 0x10;
        /// <summary>
        /// Hook proc handle
        /// </summary>
        private IntPtr _hookHandle = IntPtr.Zero;

        /// <summary>
        /// Low level hook proc
        /// </summary>
        private User32Interop.HookProc _hookProc = null;

        /// <summary>
        /// Raised on a keydown
        /// </summary>
        public event KeyEventHandler EvtKeyDown;

        /// <summary>
        /// Raised on a key press
        /// </summary>
        public event KeyPressEventHandler EvtKeyPress;

        /// <summary>
        /// Raised on a key up
        /// </summary>
        public event KeyEventHandler EvtKeyUp;
        internal enum KeyboardWParam
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105
        }

        private enum HookType
        {
            WH_KEYBOARD = 2,
            WH_KEYBOARD_LL = 13,
        }

        /// <summary>
        /// Low level hook func that is invoked by windows whenever there is a 
        /// global keyboard event
        /// </summary>
        /// <param name="nCode">the code</param>
        /// <param name="wParam">wParam</param>
        /// <param name="lParam">lParam</param>
        /// <returns>-1 if handled, otherwise if not handled</returns>
        public int HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool handled = false;
            if (nCode >= 0)
            {
                var keyboardLLHookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                var wmKeyboard = (KeyboardWParam)wParam;

                notifyEvent(wmKeyboard, keyboardLLHookStruct, ref handled);
            }

            return handled ? -1 : User32Interop.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        /// <summary>
        /// Removes the keyboard hook 
        /// </summary>
        /// <returns>true on success</returns>
        public bool RemoveHook()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                if (!User32Interop.UnhookWindowsHookEx(_hookHandle))
                {
                    return false;
                }

                _hookHandle = IntPtr.Zero;
            }

            return true;
        }

        /// <summary>
        /// Set keyboard hook to start capturing keyboard events globally
        /// </summary>
        /// <returns>true on success</returns>
        public bool SetHook()
        {
            _hookProc = HookProc;

            _hookHandle = User32Interop.SetWindowsHookEx(
                                    (int)HookType.WH_KEYBOARD_LL,
                                    _hookProc,
                                    System.Diagnostics.Process.GetCurrentProcess().MainModule.BaseAddress,
                                    0);
            return _hookHandle != IntPtr.Zero;
        }
        /// <summary>
        /// Notifies event subscribers of a keyboard event
        /// </summary>
        /// <param name="wParam">key info</param>
        /// <param name="hookStruct">keyboard info</param>
        /// <param name="handled">was the event handled?  set to true if so</param>
        private void notifyEvent(KeyboardWParam wParam, KBDLLHOOKSTRUCT hookStruct, ref bool handled)
        {
            var args = new KeyEventArgs((Keys)hookStruct.vkCode) {Handled = false};
            switch (wParam)
            {
                case KeyboardWParam.WM_SYSKEYDOWN:
                    if (EvtKeyDown != null)
                    {
                        EvtKeyDown(this, args);
                    }

                    break;

                case KeyboardWParam.WM_KEYDOWN:
                    if (EvtKeyDown != null)
                    {
                        EvtKeyDown(this, args);
                    }

                    if (EvtKeyPress != null)
                    {
                        bool isShiftDown = (User32Interop.GetKeyState(VK_SHIFT) & 0x80) == 0x80;
                        bool isCapsLockDown = User32Interop.GetKeyState(VK_CAPITAL) != 0;

                        var keyState = new byte[256];
                        var inBuffer = new byte[2];

                        User32Interop.GetKeyboardState(keyState);

                        if (User32Interop.ToAscii(hookStruct.vkCode, hookStruct.scanCode, keyState, inBuffer, hookStruct.flags) == 1)
                        {
                            var key = (char)inBuffer[0];
                            if ((isCapsLockDown ^ isShiftDown) && Char.IsLetter(key))
                            {
                                key = Char.ToUpper(key);
                            }

                            var e = new KeyPressEventArgs(key);
                            EvtKeyPress(this, e);
                        }
                    }

                    break;

                case KeyboardWParam.WM_SYSKEYUP:
                case KeyboardWParam.WM_KEYUP:
                    if (EvtKeyUp != null)
                    {
                        EvtKeyUp(this, args);
                    }

                    break;
            }

            handled = args.Handled;
        }

        /// <summary>
        /// The structure contains information about a low-level keyboard input event. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct KBDLLHOOKSTRUCT
        {
            public int vkCode;      // Specifies a virtual-key code
            public int scanCode;    // Specifies a hardware scan code for the key
            public int flags;       // keyboard flags
            public int time;        // Specifies the time stamp for this message
            public int dwExtraInfo; // not used
        }
    }
}