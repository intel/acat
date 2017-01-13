////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseHandler.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Executes mouse commands such as right click, left click etc
    /// </summary>
    public class MouseHandler : RunCommandHandler
    {
        public MouseHandler(String cmd)
            : base(cmd)
        {
        }

        public override bool Execute(ref bool handled)
        {
            handled = true;

            switch (Command)
            {
                case "CmdRightClick":
                    Context.AppAgentMgr.Keyboard.Send(Keys.LShiftKey, Keys.F10);
                    break;

                case "CmdLeftClick":
                    MouseUtils.SimulateLeftMouseClick();
                    break;

                case "CmdLeftDoubleClick":
                    MouseUtils.SimulateLeftMouseDoubleClick();
                    break;

                case "CmdLeftClickAndHold":
                    MouseUtils.SimulateLeftMouseDrag();
                    break;

                case "CmdRightDoubleClick":
                    MouseUtils.SimulateRightMouseDoubleClick();
                    break;

                case "CmdRightClickAndHold":
                    MouseUtils.SimulateRightMouseDrag();
                    break;

                case "CmdMoveCursorNW":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.Northwest);
                    break;

                case "CmdMoveCursorN":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.North);
                    break;

                case "CmdMoveCursorNE":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.Northeast);
                    break;

                case "CmdMoveCursorW":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.West);
                    break;

                case "CmdMoveCursorE":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.East);
                    break;

                case "CmdMoveCursorSW":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.Southwest);
                    break;

                case "CmdMoveCursorS":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.South);
                    break;

                case "CmdMoveCursorSE":
                    MouseUtils.SimulateMoveMouse(MouseUtils.MouseDirections.Southeast);
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }
    }
}