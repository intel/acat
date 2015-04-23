using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Aster.AgentManagement;
using Aster.Utility;
using Aster.ExtensionHelper;
using Aster.ScreenManagement;
using Aster.TalkWindowManagement;

namespace Aster.Extensions.Hawking.AppAgents.LectureManager
{
    public partial class LectureManagerOpenFileForm : Form, IPanel
    {
        const string exitItem = "-------------------EXIT-------------------";
        
        // backing field
        private WindowActiveWatchdog _windowActiveWatchdog;
        private String _folderPath;

        public string FileName {get; set;}

        public LectureManagerOpenFileForm()
        {
            InitializeComponent();
            _folderPath = Common.AppPreferences.FavoriteFolders;
            this.ShowInTaskbar = false;
            
            this.FormClosing += new FormClosingEventHandler(OpenFileForm_FormClosing);
        }

        void OpenFileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AgentManager.Instance.RemoveAgent(this.Handle);

            removeWatchdogs();

            Form form = new LectureManagerMainForm();
            if (form != null)
            {
                LectureManagerMainForm mainForm = form as LectureManagerMainForm;
                mainForm.LectureFile = FileName;
                Utility.Windows.ShowForm(form);
            }
        }

        private void loadFiles()
        {
            try
            {
                var filteredFiles = Directory
                                    .GetFiles(_folderPath, "*.*")
                                    .Where(file => file.ToLower().EndsWith("txt") ||
                                                    file.ToLower().EndsWith("doc") ||
                                                    file.ToLower().EndsWith("docx") ||
                                                    file.ToLower().EndsWith("")).ToList();

                lboxFiles.Items.Clear();

                lboxFiles.Items.Add(exitItem);

                foreach (string file in filteredFiles)
                {
                    lboxFiles.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {                
                Debug.WriteLine("exception caught! ex=" + ex.Message);
            }
        }

        public void OnPause()
        {

        }

        public void OnResume()
        {
        }

        private void lboxFiles_MouseDown(object sender, MouseEventArgs e)
        {
            FileName = lboxFiles.Text;

            if ((FileName == "") || (FileName == exitItem))
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                FileName = _folderPath + "\\" + lboxFiles.Text;
                Debug.WriteLine("filename=" + FileName);
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }


            //KILLROY
            this.Close();

        }

        private void formOpenFile_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(_folderPath))
            {
                DialogUtils.ShowTimedDialog(this, "Directory " + _folderPath + " does not exist");
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
                return;
            }
            else
            {
                loadFiles();
                this.MaximizeBox = false;
                Utility.Windows.SetWindowSizePercent(Context.AppWindowPosition, this.Handle, 75);
                this.Activate();
                this.Focus();
                lboxFiles.SelectedIndex = 0;
                lboxFiles.Focus();
                lboxFiles.KeyPress += new KeyPressEventHandler(lboxFiles_KeyPress);
                AgentManager.Instance.AddAgent(this.Handle, new LectureManagerAgent(this));

                TalkWindowManager.Instance.EvtTalkWindowPreVisibilityChanged += new TalkWindowManager.TalkWindowVisibilityChanged(Instance_EvtTalkWindowPreVisibilityChanged);
                enableWatchdogs();

            }
        }

        void Instance_EvtTalkWindowPreVisibilityChanged(object sender, TalkWindowVisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                removeWatchdogs();
            }
            else
            {
                enableWatchdogs();
            }
        }

        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }
        }

        private void removeWatchdogs()
        {
            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
                _windowActiveWatchdog = null;
            }
        }

        void lboxFiles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x0d)
            {
                lboxFiles_MouseDown(null, null);

            }
        }

    }
}
