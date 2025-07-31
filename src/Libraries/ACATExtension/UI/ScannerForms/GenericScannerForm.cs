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
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.ThemeManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictionManagement;
using ACATResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using ACAT.Extension.CommandHandlers;
using ACAT.Extension;

namespace ACAT.Scanners
{
    [ClassDescriptor("D9A5B53F-7119-445B-BDEA-F76EC53077F1",
                        "TalkApplicationScanner",
                        "Talk application main window")]
    public abstract class GenericScannerForm : Form, IScannerPanel
    {
        protected readonly ScannerCommon _scannerCommon;
        protected bool _dimScanner;
        protected String _panelClass;
        protected ScannerHelper _scannerHelper;
        protected WindowActiveWatchdog _windowActiveWatchdog;
        public GenericScannerForm()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            SubscribeToEvents();

            _dimScanner = true;
        }

        public abstract DefaultCommandDispatcher _dispatcher { get; }
        public abstract RunCommandDispatcher CommandDispatcher { get; }
        public ClassDescriptorAttribute Descriptor => ClassDescriptorAttribute.GetDescriptor(GetType());

        public Form Form => this;

        public String PanelClass => _panelClass;

        public IPanelCommon PanelCommon => _scannerCommon;

        public ScannerCommon ScannerCommon => _scannerCommon;

        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }
        public abstract ITextController TextController { get; }

        protected override CreateParams CreateParams
        {
            get
            {
                base.CreateParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_COMPOSITED;
                return base.CreateParams;
            }
        }
        public abstract bool CheckCommandEnabled(CommandEnabledArg arg);

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public abstract bool HandleInitialize(StartupArg startupArg);

        public bool Initialize(StartupArg startupArg)
        {
            _panelClass = startupArg.PanelClass;

            _scannerHelper = new ScannerHelper(this, startupArg);

            bool retVal = _scannerCommon.Initialize(startupArg);

            retVal = HandleInitialize(startupArg);

            ControlBox = true;

            List<IUserControl> list = new();

            UserControlManager.FindAllUserControls(this, list);

            return retVal;
        }
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        public virtual void OnPause()
        {
            Log.Debug("CALIBTEST TalkScanner OnPause. Pausing watchdog");
            _windowActiveWatchdog?.Pause();

            Log.Debug("CALIBTEST calling usercontrolmanager.pause");
            _scannerCommon.UserControlManager.OnPause();

            Log.Debug("CALIBTEST calling scannercommon2.pause");
            _scannerCommon.OnPause(_dimScanner ?
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

            Log.Debug("CALIBTEST TalkScanner OnResume. Resuming watchdog");
            _windowActiveWatchdog?.Resume();

            _dimScanner = true;

            Log.Debug("CALIBTEST TalkScanner OnResume. calling user control manager.OnREsume");
            _scannerCommon.UserControlManager.OnResume();

            Log.Debug("CALIBTEST TalkScanner OnResume. calling scannercommon2 resume");
            _scannerCommon.OnResume();

            //_scannerCommon.ResizeToFitDesktop(this);
        }

        public virtual void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
        }

        public virtual void SetTargetControl(Form parent, Widget widget)
        {
        }

        protected virtual void HandlePause()
        {
            Log.Warn($"No pause handler defined for {GetType().Name}. Defaulting to do nothing.");
        }
        protected virtual void HandleResume()
        {
            Log.Warn($"No resume handler defined for {GetType().Name}. Defaulting to do nothing.");
        }
        protected virtual void InitializeComponent()
        {
            Log.Warn($"No InitializeComponent() defined for {GetType().Name}. Defaulting to do nothing.");
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            _scannerCommon.OnClientSizeChanged();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        protected virtual void ScannerFormClosing(object sender, FormClosingEventArgs e)
        {
            removeWatchdogs();

            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        protected abstract void ScannerFormLoaded(object sender, EventArgs e);

        protected abstract void ScannerShown(object sender, EventArgs e);

        protected virtual void SetColorScheme()
        {
            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.TalkWindowSchemeName);

            updateControlsFromTheme(colorScheme);
        }

        protected virtual void SubscribeToEvents()
        {
            //Default to subscribe to nothing.
            Log.Warn($"No event handlers defined for {GetType().Name}");
        }

        protected virtual void updateControlsFromTheme(ColorScheme colorScheme)
        {
            Log.Warn($"Not updating theme for {GetType().Name}");
        }

        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            // By Default, do nothing and just all base WndProc.
            // If you want to handle any messages, override this method and
            // call base.WndProc only if you do not handle the message.
            base.WndProc(ref m);
        }

        private void removeWatchdogs()
        {
            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
                _windowActiveWatchdog = null;
            }
        }
    }
}