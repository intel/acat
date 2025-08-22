using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Extension.UI;
using System;
using System.Windows.Forms;

namespace ACAT.Extension.CommandHandlers
{
    /// <summary>
    /// Activates the Launch app functional agent
    /// </summary>
    public class LaunchAppHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public LaunchAppHandler(String cmd)
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

            Dispatcher.Scanner.Form.Invoke(new MethodInvoker(DialogUtils.ShowAppLauncher));

            return true;
        }
    }
}