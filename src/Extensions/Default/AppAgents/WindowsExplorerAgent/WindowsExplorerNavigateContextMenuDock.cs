using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Utility;
using System.Diagnostics;
using System.Collections;
using System.Windows.Automation;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.ScreenManagement;
using ACAT.Lib.Core.ScreenManagement.CommandDispatcher;

namespace ACAT.Lib.Core.Extensions.Base.UI.ContextMenus
{
    [DescriptorAttribute("2008558E-D12C-4A6D-ACF2-8AF522B4DEA0", "WindowsExplorerNavigateContextMenuDock" , "Windows Explorer Navigate Docked Context Menu")]
    public partial class WindowsExplorerNavigateContextMenuDock : Form, IScannerPanel, IScreenAutomation
    {
        public void SetTargetControl(Form parent, Widget widget) { }
        public String PanelClass { get; private set; }
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo) { _scannerCommon.OnFocusChanged(monitorInfo); }
        public void OnFocusChanged(AutomationElement focusedElement) { }
        public Form Form { get { return this; } }
        public SyncLock SyncObj { get { return _scannerCommon.SyncObj; } }
        public ITextController TextController { get { return _scannerCommon.TextController; } }
        public ScannerCommon ScannerCommon { get { return _scannerCommon; } }
        public bool CheckWidgetEnabled(CheckEnabledArgs arg) { return true; }

        private Widget _rootWidget;
        ScannerCommon _scannerCommon;
        AutoScanTimer _autoScanTimer;
        DockScanner _dockScanner;
        StartupArg _startupArg;
        ControlHighlight _controlHighlight;
        private RunCommandDispatcher _dispatcher;

        public WindowsExplorerNavigateContextMenuDock(String panelClass, IntPtr hWndMain)
        {
            InitializeComponent();

            PanelClass = "WindowsExplorerDialogContextMenu";
            this.Load += new EventHandler(WindowsExplorerDialogContextMenu_Load);
            this.FormClosing += new FormClosingEventHandler(WindowsExplorerDialogContextMenu_FormClosing);
            _dispatcher = new RunCommandDispatcher(this);
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get
            {
                return DescriptorAttribute.GetDescriptor(GetType());
            }
        }

        public bool Initialize(StartupArg startupArg)
        {
            PanelClass = startupArg.PanelClass;
            _startupArg = startupArg;
            _scannerCommon = new ScannerCommon(this);
            _scannerCommon.PositionSizeController.AutoPosition = false;
            if (!_scannerCommon.Initialize(startupArg))
            {
                MessageBox.Show("Could not initialize form " + Name);
                return false;
        
            }

            _rootWidget = _scannerCommon.GetRootWidget();

            _dockScanner = new DockScanner(startupArg.HWnd, this);

            _autoScanTimer = new AutoScanTimer(startupArg.HWnd, this);
            _autoScanTimer.EvtStart += new EventHandler(_autoScanTimer_EvtStart);
            _autoScanTimer.EvtStop += new EventHandler(_autoScanTimer_EvtStop);
            _autoScanTimer.EvtTick += new EventHandler(_autoScanTimer_EvtTick);

         
            return true;
        }
        
        private void onWindowClose(object sender, AutomationEventArgs e)
        {
            Log.Debug("External window closed");
            Windows.CloseForm(this);
        }


        void _autoScanTimer_EvtTick(object sender, EventArgs e)
        {
            Context.AppAgentMgr.Keyboard.Send(Keys.Tab);
        }

        void _autoScanTimer_EvtStop(object sender, EventArgs e)
        {
            createControlHighlight();
            Windows.SetOpacity(this, 1.0);
            _scannerCommon.GetAnimationManager().Resume();
        }

        void _autoScanTimer_EvtStart(object sender, EventArgs e)
        {
            disposeControlHighlight();
            Windows.SetOpacity(this, 0.5);
            _scannerCommon.GetAnimationManager().Pause();
        }

        void WindowsExplorerDialogContextMenu_Load(object sender, EventArgs e)
        {
            AutomationElement windowElement = AutomationElement.FromHandle(_startupArg.HWnd);
            Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, windowElement, TreeScope.Element, onWindowClose);


            _dockScanner.Dock();

            _scannerCommon.OnLoad();

            createControlHighlight();

            _scannerCommon.GetAnimationManager().Start(_rootWidget);
        }

        void WindowsExplorerDialogContextMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Debug();

            try
            {
                disposeControlHighlight();

                _autoScanTimer.Dispose();

                _dockScanner.Dispose();

                _scannerCommon.OnClosing();
            }
            catch (Exception exp) { Log.Debug(exp.ToString()); }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        public bool OnQueryPanelChange(PanelRequestEventArgs arg) 
        { 
            Log.Debug("Query change for " + arg.PanelClass);
            if (arg.MonitorInfo.FgHwnd == _startupArg.HWnd)// && panelClass == PanelClasses.WindowsExplorerContextMenu)
            {
                if (PanelConfigMap.AreEqual(arg.PanelClass, PanelClasses.MenuDockContextMenu))
                    return true;
                Log.Debug("Refuse Query change for " + arg.PanelClass);
                return false;
            }

            return true; 
        }

        /// <summary>
        /// Pause the application
        /// </summary>
        void pauseAster()
        {
            Log.Debug("pauseAster");

            _scannerCommon.GetAnimationManager().Pause();

            _scannerCommon.HideScanner();

            _scannerCommon.OnPause();
        }

        /// <summary>
        /// Resumes the application
        /// </summary>
        void resumeAster()
        {
            Log.Debug("resumeAster");

            _scannerCommon.GetAnimationManager().Resume();

            _scannerCommon.ShowScanner();

            _scannerCommon.OnResume();
        }

        public void OnPause()
        {
            pauseAster();
        }

        public void OnResume()
        {
            resumeAster();
        }

        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            handled = false;
        }

        public RunCommandDispatcher CommandDispatcher
        {
            get
            {
                return _dispatcher;
            }
        }

        public void OnRunCommand(string command, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "@TabScan":
                    disposeControlHighlight();
                    _autoScanTimer.ToggleTimer();
                    break;

                case "@Tab":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Tab);
                    break;

                case "@Down":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Down);
                    break;

                case "@Up":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Up);
                    break;

                case "@Left":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Left);
                    break;

                case "@Right":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Right);
                    break;

                case "@Select":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Enter);
                    break;

                case "@Escape":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Escape);
                    break;

                case "@CmdRightClick":
                    Context.AppAgentMgr.Keyboard.Send(Keys.LShiftKey, Keys.F10);
                    break;

                case "@Text":
                    Invoke(new MethodInvoker(delegate()
                    {
                        disposeControlHighlight();
                        IPanel panel = Context.AppScreenManager.CreatePanel(PanelClasses.Alphabet) as IPanel;
                        Context.AppScreenManager.Show(this, panel);
                    }));
                    break;

                default:
                    handled = false;
                    break;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                return Windows.SetFormStyles(base.CreateParams);
            }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _scannerCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        void createControlHighlight()
        {
            disposeControlHighlight();
            _controlHighlight = new ControlHighlight(_scannerCommon.StartupArg.HWnd, this);
        }

        void disposeControlHighlight()
        {
            if (_controlHighlight != null)
            {
                _controlHighlight.Dispose();
            }
        }
    }
}
