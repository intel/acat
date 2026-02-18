// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// TalkApplicationScanner.cs
//
// The main form for the ACAT Talk application. This is a container for
// user controls for word prediction, sentence prediction, talk text box
// and the keyboard.
// It also handles commands associated with keys such as Undo, Backspace,
// text navigation etc.

using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.ThemeManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension.CommandHandlers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extension.UI.ScannerForms
{
    public partial class GenericScannerForm : Form, IScannerPanel
    {
        private readonly ILogger<GenericScannerForm> _logger;
        private readonly object _lock = new object();
        private ScannerCommon _scannerCommon;

        public ScannerCommon ScannerCommon
        {
            get 
            { if (_scannerCommon == null)
                {
                    lock (_lock)
                    {
                        if(_scannerCommon == null)
                        {
                            _scannerCommon = new ScannerCommon(this);
                        }
                    }
                }
            return _scannerCommon;
            }
        }
     
        
        protected bool _dimScanner;
        protected string _panelClass;
        protected ScannerHelper _scannerHelper;
        private Label label1;
        public GenericScannerForm(ILogger<GenericScannerForm> logger = null)
        {
            _logger = logger ?? LoggingConfiguration.CreateLogger<GenericScannerForm>();
            InitializeComponent();

            SubscribeToEvents();

            _dimScanner = true;
        }

        public virtual DefaultCommandDispatcher _dispatcher { get; }
        public virtual RunCommandDispatcher CommandDispatcher { get; }
        public ClassDescriptorAttribute Descriptor => ClassDescriptorAttribute.GetDescriptor(GetType());

        public Guid Id => Descriptor.Id;

        public Form Form => this;

        public string PanelClass => _panelClass;

        public IPanelCommon PanelCommon => ScannerCommon;

        public SyncLock SyncObj
        {
            get { return ScannerCommon.SyncObj; }
        }
        public virtual ITextController TextController { get; }

        protected override CreateParams CreateParams
        {
            get
            {
                base.CreateParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_COMPOSITED;
                return base.CreateParams;
            }
        }
        public virtual bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            return false;
        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            IEnumerable<Control> controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public virtual bool HandleInitialize(StartupArg startupArg)
        {
            return false;
        }

        public bool Initialize(StartupArg startupArg)
        {
            _panelClass = startupArg.PanelClass;

            _scannerHelper = new ScannerHelper(this, startupArg);

            bool retVal = ScannerCommon.Initialize(startupArg);

            retVal = HandleInitialize(startupArg);

            ControlBox = true;

            List<IUserControl> list = new();

            UserControlManager.FindAllUserControls(this, list);

            return retVal;
        }
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            ScannerCommon.OnFocusChanged(monitorInfo);
        }

        public virtual void OnPause()
        {

            _logger.LogDebug("CALIBTEST calling usercontrolmanager.pause");
            ScannerCommon.UserControlManager.OnPause();

            _logger.LogDebug("CALIBTEST calling scannercommon2.pause");
            ScannerCommon.OnPause(_dimScanner ?
                                ScannerCommon.PauseDisplayMode.FadeScanner :
                                ScannerCommon.PauseDisplayMode.None);

            HandlePause();
        }

        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        public virtual void OnResume()
        {
            HandleResume();

            _logger.LogDebug("CALIBTEST TalkScanner OnResume. Resuming watchdog");

            _dimScanner = true;

            _logger.LogDebug("CALIBTEST TalkScanner OnResume. calling user control manager.OnREsume");
            ScannerCommon.UserControlManager.OnResume();

            _logger.LogDebug("CALIBTEST TalkScanner OnResume. calling scannercommon2 resume");
            ScannerCommon.OnResume();

            ScannerCommon.ResizeToFitDesktop(this);
        }

        public virtual void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
        }

        public virtual void SetTargetControl(Form parent, Widget widget)
        {
        }

        protected virtual void HandlePause()
        {
            _logger.LogWarning($"No pause handler defined for {GetType().Name}. Defaulting to do nothing.");
        }
        protected virtual void HandleResume()
        {
            _logger.LogWarning($"No resume handler defined for {GetType().Name}. Defaulting to do nothing.");
        }
        protected virtual void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "All Your Bases Are Mine";
            // 
            // GenericScannerForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label1);
            this.Name = "GenericScannerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            ScannerCommon.OnClientSizeChanged();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ScannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        protected virtual void ScannerFormClosing(object sender, FormClosingEventArgs e)    
        {
            ScannerCommon.OnClosing();
            ScannerCommon.Dispose();
        }

        protected virtual void ScannerFormLoaded(object sender, EventArgs e)
        {

        }

        protected virtual void ScannerShown(object sender, EventArgs e)
        {

        }

        protected virtual void SetColorScheme()
        {
            ColorScheme colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.TalkWindowSchemeName);

            updateControlsFromTheme(colorScheme);
        }

        protected virtual void SubscribeToEvents()
        {
            //Default to subscribe to nothing.
            _logger.LogWarning($"No event handlers defined for {GetType().Name}");
        }

        protected virtual void updateControlsFromTheme(ColorScheme colorScheme)
        {
            _logger.LogWarning($"Not updating theme for {GetType().Name}");
        }

        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            // By default, do nothing and just call base WndProc.
            // If you want to handle any messages, override this method and
            // call base.WndProc only if you do not handle the message.
            base.WndProc(ref m);
        }
    }
}