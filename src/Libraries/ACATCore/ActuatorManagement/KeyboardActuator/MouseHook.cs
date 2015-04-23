////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseHook.cs" company="Intel Corporation">
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
using System.Diagnostics;
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

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// Manages windows low level mouse hooks to capture global
    /// mouse events
    /// </summary>
    internal class MouseHook
    {
        /// <summary>
        /// windows hook handle
        /// </summary>
        private IntPtr _hookHandle = IntPtr.Zero;

        /// <summary>
        /// hook function that will be called
        /// </summary>
        private User32Interop.HookProc _hookProc;

        /// <summary>
        /// Raised on a mouse double click
        /// </summary>
        public event MouseEventHandler EvtMouseDblClick;

        /// <summary>
        /// Raised on a mouse down
        /// </summary>
        public event MouseEventHandler EvtMouseDown;

        /// <summary>
        /// Raised when the mouse moves
        /// </summary>
        public event MouseEventHandler EvtMouseMove;

        /// <summary>
        /// Raised on a mouse up
        /// </summary>
        public event MouseEventHandler EvtMouseUp;

        /// <summary>
        /// Raised on wheel movement
        /// </summary>
        public event MouseEventHandler EvtMouseWheel;

        /// <summary>
        /// Windows constants
        /// </summary>
        private enum HookType
        {
            WH_MOUSE = 7,
            WH_MOUSE_LL = 14
        }

        /// <summary>
        /// Windows constants
        /// </summary>
        private enum MouseWParam
        {
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,

            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E,

            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9
        }

        /// <summary>
        /// The low level hook function that windows invokes on any mouse
        /// event
        /// </summary>
        /// <param name="nCode">code</param>
        /// <param name="wParam">wParam</param>
        /// <param name="lParam">lParam</param>
        /// <returns>-1 if handled</returns>
        public int HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool handled = false;
            if (nCode >= 0)
            {
                var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                var wmMouse = (MouseWParam)wParam;

                notifyEvent(wmMouse, hookStruct, ref handled);
            }

            return handled ? -1 : User32Interop.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        /// <summary>
        /// Removes a previously installed mouse hook
        /// </summary>
        /// <returns></returns>
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
        /// Sets the global mouse hook.
        /// </summary>
        /// <returns>true on success</returns>
        public bool SetHook()
        {
            _hookProc = HookProc;
            _hookHandle = User32Interop.SetWindowsHookEx(
                                        (int)HookType.WH_MOUSE_LL,
                                        _hookProc,
                                        Process.GetCurrentProcess().MainModule.BaseAddress,
                                        0);
            return _hookHandle != IntPtr.Zero;
        }

        /// <summary>
        /// Notifies subscribers of the mouse event
        /// </summary>
        /// <param name="wParam">wParam</param>
        /// <param name="mouseHookStruct">Contains event info</param>
        /// <param name="handled">set to true if handled</param>
        private void notifyEvent(MouseWParam wParam, MSLLHOOKSTRUCT mouseHookStruct, ref bool handled)
        {
            var button = MouseButtons.None;
            MouseEventHandler mouseEvent = null;
            int delta = 0;

            switch (wParam)
            {
                case MouseWParam.WM_NCLBUTTONDOWN:
                case MouseWParam.WM_LBUTTONDOWN:
                    button = MouseButtons.Left;
                    mouseEvent = EvtMouseDown;
                    break;

                case MouseWParam.WM_RBUTTONDOWN:
                case MouseWParam.WM_NCRBUTTONDOWN:
                    button = MouseButtons.Right;
                    mouseEvent = EvtMouseDown;
                    break;

                case MouseWParam.WM_MBUTTONDOWN:
                case MouseWParam.WM_NCMBUTTONDOWN:
                    button = MouseButtons.Middle;
                    mouseEvent = EvtMouseDown;
                    break;

                case MouseWParam.WM_NCLBUTTONUP:
                case MouseWParam.WM_LBUTTONUP:
                    button = MouseButtons.Left;
                    mouseEvent = EvtMouseUp;
                    break;

                case MouseWParam.WM_RBUTTONUP:
                case MouseWParam.WM_NCRBUTTONUP:
                    button = MouseButtons.Right;
                    mouseEvent = EvtMouseUp;
                    break;

                case MouseWParam.WM_MBUTTONUP:
                case MouseWParam.WM_NCMBUTTONUP:
                    button = MouseButtons.Middle;
                    mouseEvent = EvtMouseUp;
                    break;

                case MouseWParam.WM_MOUSEMOVE:
                case MouseWParam.WM_NCMOUSEMOVE:
                    mouseEvent = EvtMouseMove;
                    break;

                case MouseWParam.WM_NCLBUTTONDBLCLK:
                case MouseWParam.WM_LBUTTONDBLCLK:
                    button = MouseButtons.Left;
                    mouseEvent = EvtMouseDblClick;
                    break;

                case MouseWParam.WM_NCRBUTTONDBLCLK:
                case MouseWParam.WM_RBUTTONDBLCLK:
                    button = MouseButtons.Right;
                    mouseEvent = EvtMouseDblClick;
                    break;

                case MouseWParam.WM_NCMBUTTONDBLCLK:
                case MouseWParam.WM_MBUTTONDBLCLK:
                    button = MouseButtons.Middle;
                    mouseEvent = EvtMouseDblClick;
                    break;

                case MouseWParam.WM_MOUSEWHEEL:
                case MouseWParam.WM_MOUSEHWHEEL:
                    delta = mouseHookStruct.mouseData >> 16;
                    mouseEvent = EvtMouseWheel;
                    break;
            }

            if (mouseEvent != null)
            {
                var args = new MouseEventArgs(button, 1, mouseHookStruct.pt.x, mouseHookStruct.pt.y, delta);
                mouseEvent(this, args);
            }
        }

        /// <summary>
        /// Windows mouse hook struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// Windows POINT class
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            public int x;
            public int y;
        }
    }
}