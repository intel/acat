////////////////////////////////////////////////////////////////////////////
// <copyright file="AboutBoxForm.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// Displays the about box with information about the application,
    /// version, copyright and 3rd Party attributions.
    /// </summary>
    [DescriptorAttribute("F9A367F9-F9C4-4CF6-BDE7-6995675E1BE4",
                        "AboutBoxForm",
                        "About box")]
    public partial class AboutBoxForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// Provdes access to methods/properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The name of the application
        /// </summary>
        private String _appName;

        /// <summary>
        /// 3rd party attributions
        /// </summary>
        private List<String> _attributions;

        /// <summary>
        /// Company information
        /// </summary>
        private String _companyInfo;

        /// <summary>
        /// Copyright info
        /// </summary>
        private String _copyRightInfo;

        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Full path to the application logo image
        /// </summary>
        private String _logo;

        /// <summary>
        /// Version number information
        /// </summary>
        private String _versionInfo;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AboutBoxForm(string titleText)
        {
            InitializeComponent();

            _attributions = new List<string>();

            _invoker = new ExtensionInvoker(this);

            Load += Form_Load;
            FormClosing += Form_Closing;
        }

        /// <summary>
        /// Gets or sets the title of the form
        /// </summary>
        public String AppName
        {
            get
            {
                return _appName;
            }

            set
            {
                _appName = value;
                Windows.SetText(labelAppTitle, value);
            }
        }

        /// <summary>
        /// Gets or sets 3rd party attributions
        /// </summary>
        public IEnumerable<String> Attributions
        {
            get
            {
                return _attributions;
            }

            set
            {
                _attributions = value.ToList();

                var sb = new StringBuilder();

                sb.AppendLine("Click on the \"Licenses\" button for detailed licensing information");
                sb.AppendLine();

                foreach (var str in _attributions)
                {
                    sb.AppendLine(str);
                    sb.AppendLine();
                }

                Windows.SetText(textBoxOtherInfo, sb.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the company information
        /// </summary>
        public String CompanyInfo
        {
            get
            {
                return _companyInfo;
            }
            set
            {
                _companyInfo = value;
                Windows.SetText(labelCompanyInfo, value);
            }
        }

        /// <summary>
        /// Gets or sets copyright information
        /// </summary>
        public String CopyrightInfo
        {
            get
            {
                return _copyRightInfo;
            }

            set
            {
                _copyRightInfo = value;
                Windows.SetText(labelCopyrightInfo, value);
            }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets the logo string
        /// </summary>
        public String Logo
        {
            get
            {
                return _logo;
            }

            set
            {
                try
                {
                    _logo = value;
                    var image = Image.FromFile(FileUtils.GetImagePath(_logo));
                    var bitmap = ImageUtils.ImageOpacity(image, 1.0F, new Rectangle(0, 0, pictureBoxLogo.Width, pictureBoxLogo.Height));
                    pictureBoxLogo.Image = bitmap;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _dialogCommon; } }

        /// <summary>
        /// Gets synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Gets or sets the prompt to be displayed
        /// </summary>
        public String VersionInfo
        {
            get
            {
                return _versionInfo;
            }

            set
            {
                _versionInfo = value;
                Windows.SetText(labelVersionInfo, value);
            }
        }

        /// <summary>
        /// Sets the form style
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return DialogCommon.SetFormStyles(Windows.SetFormStyles(base.CreateParams)); }
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns>The invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Intitializes the class
        /// </summary>
        /// <param name="startupArg">startup param</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _dialogCommon = new DialogCommon(this);
            return _dialogCommon.Initialize(startupArg);
        }

        /// <summary>
        /// Triggered when a widget is actuated.
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            var value = widget.Value;
            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            Log.Debug("**Actuate** " + widget.Name + " Value: " + value);

            switch (value)
            {
                case "ok":
                    Windows.CloseForm(this);
                    break;
            }
        }

        /// <summary>
        /// Pause the scanner
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resume paused scanner
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
        }

        /// <summary>
        /// Handles the command. There is only the OK
        /// button that we have to handle
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <param name="handled">was it handled?</param>
        public void OnRunCommand(string command, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                case "@ok":
                    Windows.CloseAsync(this);
                    break;

                default:
                    handled = false;
                    break;
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="e">closing argument</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Event handler for displaying license file
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonLicenses_Click(object sender, EventArgs e)
        {
            Hide();

            var showLicenseForm = new ShowLicenseForm();
            showLicenseForm.ShowDialog();

            Show();

            Activate();
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form loader handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form_Load(object sender, EventArgs e)
        {
            updateUserProfileInfo();
            _dialogCommon.OnLoad();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Updates user/profile info on the form
        /// </summary>
        private void updateUserProfileInfo()
        {
            labelUserProfileInfo.Text = "User: " + UserManager.CurrentUser + ", Profile: " + ProfileManager.CurrentProfile;
        }
    }
}