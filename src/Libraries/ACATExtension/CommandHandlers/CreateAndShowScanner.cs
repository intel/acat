////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Creates a scanner based on the requested scanner, and displays it.
    /// Only handles the predefined scanners that ACAT supports.
    /// </summary>
    public class CreateAndShowScanner : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public CreateAndShowScanner(String cmd)
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
            bool retVal = true;

            handled = true;

            switch (Command)
            {
                case "CmdPunctuationScanner":
                    retVal = createAndShowScanner(PanelClasses.Punctuation);
                    break;

                case "CmdCursorScanner":
                    retVal = createAndShowScanner(PanelClasses.Cursor);
                    break;

                case "CmdMouseScanner":
                    retVal = createAndShowScanner(PanelClasses.Mouse);
                    break;

                case "CmdNumberScanner":
                    retVal = createAndShowScanner(PanelClasses.Number);
                    break;

                case "CmdFunctionKeyScanner":
                    retVal = createAndShowScanner(PanelClasses.FunctionKey);
                    break;

                default:
                    handled = false;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Creates and shows the specified scanner
        /// </summary>
        /// <param name="panelClass">type of the scanner</param>
        /// <returns>true on success</returns>
        private bool createAndShowScanner(String panelClass)
        {
            bool retVal = true;

            Form form = Dispatcher.Scanner.Form;

            if (form != null)
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    IPanel panel = Context.AppPanelManager.CreatePanel(panelClass) as IPanel;
                    if (panel != null)
                    {
                        Context.AppPanelManager.Show(form as IPanel, panel);
                    }
                    else
                    {
                        retVal = false;
                    }
                }));
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }
    }
}