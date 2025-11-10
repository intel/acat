using ACATConfigNext.Forms;
using System;
using System.Windows.Forms;

namespace ACATConfigNext
{
    internal static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new SettingsForm());
        }
    }
}