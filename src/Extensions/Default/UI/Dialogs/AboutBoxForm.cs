////////////////////////////////////////////////////////////////////////////
// <copyright file="AboutBoxForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// Displays the about box with infor about the application,
    /// copyright and 3rd Party attributions.
    /// </summary>
    [DescriptorAttribute("F9A367F9-F9C4-4CF6-BDE7-6995675E1BE4", "AboutBoxForm", "About box")]
    public partial class AboutBoxForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// The dialogCommon object
        /// </summary>
        private readonly DialogCommon _dialogCommon;

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
        /// Copyright info
        /// </summary>
        private String _copyRightInfo;

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

            _dialogCommon = new DialogCommon(this);
            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

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
                foreach (var str in _attributions)
                {
                    sb.AppendLine(str);
                    sb.AppendLine();
                }

                Windows.SetText(textBoxOtherInfo, sb.ToString());
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
                catch (Exception)
                {
                }
            }
        }

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
        /// Handle the command. There is only the OK
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
            _dialogCommon.OnLoad();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }
    }
}