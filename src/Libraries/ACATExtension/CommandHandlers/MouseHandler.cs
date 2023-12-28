////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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