using System;
using System.Windows.Forms;

namespace RunAsAdminTest
{
    internal static class Program
    {
        public static String LaunchAppName { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                LaunchAppName = args[0].Trim();
            }

            Application.Run(new Form1());
        }
    }
}