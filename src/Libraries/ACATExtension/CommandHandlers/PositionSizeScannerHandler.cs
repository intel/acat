////////////////////////////////////////////////////////////////////////////
// <copyright file="PositionSizeScannerHandler.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Repositions the scanner to one of the pre-defined positions on
    /// the display.  Also enables resizing the scanner - making it
    /// bigger or smaller.
    /// </summary>
    public class PositionSizeScannerHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public PositionSizeScannerHandler(String cmd)
            : base(cmd)
        {
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="handled">set to true if the command was handled</param>
        /// <returns>true on success</returns>
        public override bool Execute(ref bool handled)
        {
            handled = true;

            switch (Command)
            {
                case "CmdAutoPositionScanner":  // autoposition
                    var scannerCommon = Dispatcher.Scanner.ScannerCommon;
                    scannerCommon.AnimationManager.Interrupt();
                    scannerCommon.PositionSizeController.EvtAutoRepositionScannerStop += PositionSizeController_EvtAutoRepositionScannerStop;
                    scannerCommon.PositionSizeController.AutoRepositionScannerStart();
                    break;

                case "CmdPositionScannerTopRight":
                    Windows.SetWindowPositionAndNotify(Dispatcher.Scanner.Form, Windows.WindowPosition.TopRight);
                    break;

                case "CmdPositionScannerTopLeft":
                    Windows.SetWindowPositionAndNotify(Dispatcher.Scanner.Form, Windows.WindowPosition.TopLeft);
                    break;

                case "CmdPositionScannerBottomRight":
                    Windows.SetWindowPositionAndNotify(Dispatcher.Scanner.Form, Windows.WindowPosition.BottomRight);
                    break;

                case "CmdPositionScannerBottomLeft":
                    Windows.SetWindowPositionAndNotify(Dispatcher.Scanner.Form, Windows.WindowPosition.BottomLeft);
                    break;

                case "CmdPositionScannerMiddleRight":
                    Windows.SetWindowPositionAndNotify(Dispatcher.Scanner.Form, Windows.WindowPosition.MiddleRight);
                    break;

                case "CmdPositionScannerMiddleLeft":
                    Windows.SetWindowPositionAndNotify(Dispatcher.Scanner.Form, Windows.WindowPosition.MiddleLeft);
                    break;

                case "CmdScannerZoomIn":
                    if (Dispatcher.Scanner is IScannerPreview)
                    {
                        var preview = Dispatcher.Scanner as IScannerPreview;
                        preview.ScaleUp();

                        if (Common.AppPreferences.AutoSaveScannerScaleFactor)
                        {
                            preview.SaveScaleSetting();
                        }
                    }
                    break;

                case "CmdScannerZoomOut":
                    if (Dispatcher.Scanner is IScannerPreview)
                    {
                        var preview = Dispatcher.Scanner as IScannerPreview;
                        preview.ScaleDown();

                        if (Common.AppPreferences.AutoSaveScannerScaleFactor)
                        {
                            preview.SaveScaleSetting();
                        }
                    }
                    break;

                case "CmdScannerZoomDefault":
                    if (Dispatcher.Scanner is IScannerPreview)
                    {
                        var preview = Dispatcher.Scanner as IScannerPreview;
                        preview.ScaleDefault();

                        if (Common.AppPreferences.AutoSaveScannerScaleFactor)
                        {
                            preview.SaveScaleSetting();
                        }
                    }
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Event handler when autorepositon scanner stops moving the scanner.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event handler</param>
        private void PositionSizeController_EvtAutoRepositionScannerStop(object sender, EventArgs e)
        {
            Dispatcher.Scanner.ScannerCommon.PositionSizeController.EvtAutoRepositionScannerStop -= PositionSizeController_EvtAutoRepositionScannerStop;

            if (CoreGlobals.AppPreferences.AutoSaveScannerLastPosition)
            {
                Dispatcher.Scanner.ScannerCommon.PositionSizeController.SaveSettings(ACATPreferences.Load());
            }
        }
    }
}