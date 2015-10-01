using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RunAsAdminTest
{
    public partial class Form1 : Form
    {
        private const String caption = "ACAT Setup";
        private const String presageExeName = "presage-0.9.1-32bit-setup.exe";
        private readonly String exeName;

        public Form1()
        {
            InitializeComponent();
            exeName = String.IsNullOrEmpty(Program.LaunchAppName) ? presageExeName : Program.LaunchAppName;
            Load += OnLoad;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    bool retVal = installPresage();
                    if (retVal)
                    {
                        break;
                    }

                    var result = MessageBox.Show(
                        "You did not install Presage. " +
                        "Word Prediction will not work in ACAT. Press Retry to install Presage\nPress Cancel to quit.",
                        caption,
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Retry)
                    {
                        var fileName = Process.GetCurrentProcess().MainModule.FileName;
                        var path = Path.GetDirectoryName(fileName);
                        var presageFile = path + "\\" + exeName;
                        MessageBox.Show("Please install Presage manually by running " + presageFile, caption);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error installing Presage. " + ex, caption, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    break;
                }
            }

            Close();
        }

        private bool installPresage()
        {
            const int USER_CANCELLED = 1223;

            var info = new ProcessStartInfo(@exeName)
            {
                UseShellExecute = true,
                Verb = "runas",
                Arguments = "/S /NoNpp"
            };

            try
            {
                Process.Start(info);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == USER_CANCELLED)
                {
                    return false;
                }
                throw;
            }

            return true;
        }

        private void OnLoad(object sender, EventArgs eventArgs)
        {
            TopMost = false;
            TopMost = true;
            ShowInTaskbar = false;
            Text = caption;
            CenterToScreen();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}