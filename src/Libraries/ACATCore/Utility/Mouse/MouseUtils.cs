////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseUtils.cs" company="Intel Corporation">
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
using System.Drawing;
using System.Runtime.InteropServices;
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
    /// Useful mouse utility functions. Simulates mouse events such
    /// as clicks, double clicks, right click as well as cursor
    /// movements
    /// </summary>
    public class MouseUtils
    {
        /// <summary>
        /// Which way to move the mouse cursor
        /// </summary>
        public enum MouseDirections
        {
            Northwest,
            North,
            Northeast,
            West,
            East,
            Southwest,
            South,
            Southeast
        }

        /// <summary>
        /// Simulates mouse left click at the indicated
        /// by doing a mouse down followed by a mouse up
        /// </summary>
        /// <param name="x">x position of cursor</param>
        /// <param name="y">y position of the cursor</param>
        public static void ClickLeftMouseButton(int x, int y)
        {
            float screenWidth = Screen.PrimaryScreen.Bounds.Width;
            float screenHeight = Screen.PrimaryScreen.Bounds.Height;

            var mouseInput = new User32Interop.INPUT { type = User32Interop.SendInputEventType.InputMouse };
            //mouseInput.mkhi.mi.dx = x;
            //mouseInput.mkhi.mi.dy = y;

            mouseInput.mkhi.mi.dx = (int)Math.Round(x * (65535 / screenWidth), 0);
            mouseInput.mkhi.mi.dy = (int)Math.Round(y * (65535 / screenHeight), 0);
            mouseInput.mkhi.mi.mouseData = 0;

            mouseInput.mkhi.mi.dwFlags = User32Interop.MouseEventFlags.MOUSEEVENTF_MOVE |
                                        User32Interop.MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
            User32Interop.SendInput(1, ref mouseInput, Marshal.SizeOf(new User32Interop.INPUT()));

            var inputDown = new User32Interop.INPUT();
            inputDown.mkhi.mi.dx = 0;
            inputDown.mkhi.mi.dy = 0;
            inputDown.mkhi.mi.mouseData = 0;
            inputDown.mkhi.mi.dwFlags = User32Interop.MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
            User32Interop.SendInput(1, ref inputDown, Marshal.SizeOf(new User32Interop.INPUT()));

            inputDown.mkhi.mi.dwFlags = User32Interop.MouseEventFlags.MOUSEEVENTF_LEFTUP;
            User32Interop.SendInput(1, ref inputDown, Marshal.SizeOf(new User32Interop.INPUT()));
        }

        /// <summary>
        /// Simulates a left click at the current
        /// mouse position
        /// </summary>
        public static void SimulateLeftMouseClick()
        {
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_LEFTDOWN |
                                        User32Interop.MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }

        /// Simulates a left button double click at the current
        /// cursor position
        public static void SimulateLeftMouseDoubleClick()
        {
            int mouseX = Cursor.Position.X;
            int mouseY = Cursor.Position.Y;
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_LEFTDOWN |
                                        User32Interop.MOUSEEVENTF_LEFTUP, mouseX, mouseY, 0, 0);

            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_LEFTDOWN |
                                        User32Interop.MOUSEEVENTF_LEFTUP, mouseX, mouseY, 0, 0);
        }

        /// <summary>
        /// Simulates a left mouse drag at the current
        /// cursor position by sending a left button down event
        /// </summary>
        public static void SimulateLeftMouseDrag()
        {
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_LEFTDOWN, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }

        /// <summary>
        /// Simulates a middel click at the current
        /// cursor position
        /// </summary>
        public static void SimulateMiddleMouseClick()
        {
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_MIDDLEDOWN |
                                        User32Interop.MOUSEEVENTF_MIDDLEUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }

        /// <summary>
        /// Simulates a middle button double click at the current
        /// cursor position
        /// </summary>
        public static void SimulateMiddleMouseDoubleClick()
        {
            int mouseX = Cursor.Position.X;
            int mouseY = Cursor.Position.Y;
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_MIDDLEDOWN |
                                        User32Interop.MOUSEEVENTF_MIDDLEUP, mouseX, mouseY, 0, 0);
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_MIDDLEDOWN |
                                        User32Interop.MOUSEEVENTF_MIDDLEUP, mouseX, mouseY, 0, 0);
        }

        /// <summary>
        /// Simulates a middle button drag at the current
        /// cursor position by sending a middle button down event
        /// </summary>
        public static void SimulateMiddleMouseDrag()
        {
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_MIDDLEDOWN, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }

        /// <summary>
        /// Mouse the cursor in the specified direction by the
        /// specified amount
        /// </summary>
        /// <param name="mouseDir">which way to move</param>
        /// <param name="pixels">by how much</param>
        public static void SimulateMoveMouse(MouseDirections mouseDir, int pixels = 1)
        {
            int mouseX = Cursor.Position.X;
            int mouseY = Cursor.Position.Y;
            int mouseChangeX = 0;
            int mouseChangeY = 0;

            switch (mouseDir)
            {
                case MouseDirections.East:
                    mouseChangeX += pixels;
                    mouseChangeY = 0;
                    break;

                case MouseDirections.North:
                    mouseChangeX = 0;
                    mouseChangeY -= pixels;
                    break;

                case MouseDirections.Northeast:
                    mouseChangeX += pixels;
                    mouseChangeY -= pixels;
                    break;

                case MouseDirections.Northwest:
                    mouseChangeX -= pixels;
                    mouseChangeY -= pixels;
                    break;

                case MouseDirections.South:
                    mouseChangeX = 0;
                    mouseChangeY += pixels;
                    break;

                case MouseDirections.Southeast:
                    mouseChangeX += pixels;
                    mouseChangeY += pixels;
                    break;

                case MouseDirections.Southwest:
                    mouseChangeX -= pixels;
                    mouseChangeY += pixels;
                    break;

                case MouseDirections.West:
                    mouseChangeX -= pixels;
                    mouseChangeY = 0;
                    break;
            }

            mouseX += mouseChangeX;
            mouseY += mouseChangeY;
            Cursor.Position = new Point(mouseX, mouseY);
        }

        /// <summary>
        /// Simulates a right click at the current
        /// cursor position
        /// </summary>
        public static void SimulateRightMouseClick()
        {
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_RIGHTDOWN |
                                        User32Interop.MOUSEEVENTF_RIGHTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }

        /// Simulates a right button double click at the current
        /// cursor position
        public static void SimulateRightMouseDoubleClick()
        {
            int mouseX = Cursor.Position.X;
            int mouseY = Cursor.Position.Y;
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_RIGHTDOWN |
                                        User32Interop.MOUSEEVENTF_RIGHTUP, mouseX, mouseY, 0, 0);
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_RIGHTDOWN |
                                        User32Interop.MOUSEEVENTF_RIGHTUP, mouseX, mouseY, 0, 0);
        }

        /// <summary>
        /// Simulates a right mouse drag at the current
        /// cursor position by sending a right button down event
        /// </summary>
        public static void SimulateRightMouseDrag()
        {
            User32Interop.mouse_event(User32Interop.MOUSEEVENTF_RIGHTDOWN, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }
    }
}