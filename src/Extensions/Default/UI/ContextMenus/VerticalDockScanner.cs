using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aster.InputActuators;
using Aster.Animations;
using Aster.Interpreter;
using Aster.Widgets;
using Aster.Utility;
using System.Diagnostics;
using System.Collections;
using System.Windows.Automation;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Runtime.InteropServices;
using Aster.AppAgents;
using Aster.ScreenManagement;

namespace Aster.Screens
{
    [GuidAttribute("A19C899B-A744-4D7C-8002-621E539FF60F")]
    public partial class VerticalDockScanner : Form, IScreen, IScreenCommon, IScreenAutomation
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public void SetTargetControl(Form parent, Widget widget) { _targetWidget = widget; _parentForm = parent; }
        public bool QueryPanelChange(PanelRequestEventArg arg) { return true; }
        public bool RetainTalkWindowState { get { return _retainTalkWindowState; } }
        public String GetPanelType() { return _panelName; }
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo) { _screenCommon.OnFocusChanged(monitorInfo); }
        private Widget _rootWidget;
        ScreenCommon _screenCommon;
        bool _retainTalkWindowState = false;
        IntPtr _hWnd;
        Widget _targetWidget = null;
        Form _parentForm = null;
        String _panelName = PanelTypes.MenuContextMenu;
        int _menuWidth;
        AutomationElement _automationElementDockTo;

        AutoScanTimer _autoScanTimer;

        public VerticalDockScanner()//String panelName, IntPtr hWndMain, AutomationElement focusedElement)
        {
            InitializeComponent();
        
            this.Load += new EventHandler(MenuContextMenu_Load);
            this.FormClosing += new FormClosingEventHandler(MenuContextMenu_FormClosing);
        }

        public virtual bool Initialize(StartupArg startupArg)
        {
            _menuWidth = this.Width;
            _panelName = startupArg.PanelType;

            _screenCommon = new ScreenCommon(this);
            _screenCommon.AutoSetScreenLocation = false;

            if (!_screenCommon.Initialize(startupArg))
            {
                MessageBox.Show("Could not initialize form " + Name);
                return false;
            }

            _rootWidget = _screenCommon.GetRootWidget();
            _hWnd = startupArg.HWnd;
            if (startupArg.FocusedElement != null)
            {
                OnFocusChanged(startupArg.FocusedElement);
            }

            _screenCommon.GetAnimationManager().EvtPlayerStateChanged += MenuContextMenu_EvtPlayerStateChanged;

            _autoScanTimer = new AutoScanTimer(startupArg.HWnd, this);
            _autoScanTimer.EvtStart += new AutoScanTimer.StartScan(_autoScanTimer_EvtStart);
            _autoScanTimer.EvtStop += new AutoScanTimer.StopScan(_autoScanTimer_EvtStop);
            _autoScanTimer.EvtTick += new AutoScanTimer.Tick(_autoScanTimer_EvtTick);

            return true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _rootWidget.HighlightOff();
            _autoScanTimer.Start();
        }

        void MenuContextMenu_EvtPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            Log.Debug("Playerstate changed to: " + newState);
            if (newState == PlayerState.Timeout)
            {
                _autoScanTimer.Start();
            }
        }

        void _autoScanTimer_EvtTick()
        {
            Context.AppAgentMgr.Keyboard.Send(Keys.Down);
        }

        void _autoScanTimer_EvtStop()
        {
            Windows.SetOpacity(this, 1.0);
            //_screenCommon.GetAnimationManager().Resume();
            _screenCommon.GetAnimationManager().Start(_rootWidget);
        }

        void _autoScanTimer_EvtStart()
        {
            Windows.SetOpacity(this, 0.5);
            //_screenCommon.GetAnimationManager().Pause();
        }

        void MenuContextMenu_Load(object sender, EventArgs e)
        {
            //for some reason, the width was getting set to 132. DUnno why.
            // resetting it here to the design-time width
            this.Width = _menuWidth;

            setFormHeight();

            if (_parentForm != null)
            {
                dockScanner();
            }
            
            _screenCommon.OnLoad();
            

            //_screenCommon.GetAnimationManager().Start(_rootWidget);
        }

        void UIControl_Leave(object sender, EventArgs e)
        {
            this.Close();
        }

        void MenuContextMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_hWnd != IntPtr.Zero && _automationElementDockTo != null)
            {
                try
                {
                    AutomationEventManager.RemoveAutomationPropertyChangedEventHandler(_hWnd, AutomationElement.BoundingRectangleProperty, _automationElementDockTo, onWindowPositionChanged);
                }
                catch { }
            }

            _screenCommon.GetAnimationManager().EvtPlayerStateChanged -= MenuContextMenu_EvtPlayerStateChanged;

            _autoScanTimer.Dispose();

            _screenCommon.OnClosing();
        }

        public void OnFocusChanged(AutomationElement focusedElement)
        {
            Log.Debug("ControlType: " + focusedElement.Current.ControlType.ProgrammaticName + ", handle: " + focusedElement.Current.NativeWindowHandle);

            try
            {
                dockScanner();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        System.Windows.Rect getRect(AutomationElement focusedElement)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement parent = walker.GetParent(focusedElement);

            System.Windows.Rect rect = new System.Windows.Rect(0, 0, 0, 0);
            if (focusedElement.Current.ControlType.ProgrammaticName == "ControlType.MenuItem")
            {
                rect = parent.Current.BoundingRectangle;
                Log.Debug("MenuItem.  Parent menu rect: " + rect.ToString());
            }
            else if (focusedElement.Current.ControlType.ProgrammaticName == "ControlType.Menu")
            {
                rect = focusedElement.Current.BoundingRectangle;
                Log.Debug("This is the parent menu.rect: " + rect.ToString());
            }
            else if (focusedElement.Current.ControlType.ProgrammaticName == "ControlType.ListItem")
            {
                rect = parent.Current.BoundingRectangle;
                Log.Debug("ListItem.  Parent list rect: " + rect.ToString());
            }
            else if (focusedElement.Current.ControlType.ProgrammaticName == "ControlType.List")
            {
                rect = focusedElement.Current.BoundingRectangle;
                Log.Debug("This is the parent list.rect: " + rect.ToString());
            }
            else
            {
                Log.Debug("_hWnd is " + ((_hWnd == IntPtr.Zero) ? " null " : " not null"));
                //AutomationElement window = AutomationElement.FromHandle(_hWnd);
                AutomationElement window = focusedElement;//.FromHandle(_hWnd);
                rect = window.Current.BoundingRectangle;
                Log.Debug("BOUNDINGRECTANTGLE: " + rect.ToString());
            }
            return rect;

        }


        /// <summary>
        /// Pause the application
        /// </summary>
        void pauseAster()
        {
            Log.Debug("pauseAster");

            _screenCommon.GetAnimationManager().Pause();

            Windows.SetVisible(this, false);

            _screenCommon.Pause();
        }

        /// <summary>
        /// Resumes the application
        /// </summary>
        void resumeAster()
        {
            Log.Debug("resumeAster");

            _screenCommon.GetAnimationManager().Resume();

            Windows.SetVisible(this, true);

            _screenCommon.Resume();
        }

        public void OnPause()
        {
            pauseAster();
        }

        public void OnResume()
        {
            resumeAster();
        }

        public void OnRunCommand(string command, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "@goHome":
                    Context.AppAgentMgr.RunCommand("MenuContextClose", ref handled);
                    //Context.AppAgent.Send(Keys.Escape);
                    //Context.AppAgent.Send(Keys.Escape);
                    //Context.AppAgent.Send(Keys.Escape);
                    handled = false;
                    break;

                case "@TabScan":
                    _autoScanTimer.ToggleTimer();
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

                case "@PageUp":
                    Context.AppAgentMgr.Keyboard.Send(Keys.PageUp);
                    break;

                case "@PageDown":
                    Context.AppAgentMgr.Keyboard.Send(Keys.PageDown);
                    break;

                case "@Top":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Home);
                    break;

                case "@Bottom":
                    Context.AppAgentMgr.Keyboard.Send(Keys.End);
                    break;

                case "@Select":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Enter);
                    break;

                case "@SelectAndClose":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Enter);
                    Close();
                    break;

                case "@SelectWithMouse":
                    clickOnFocusedElement();
                    break;

                case "@Escape":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Escape);
                    break;

                case "@Text":
                    Invoke(new MethodInvoker(delegate()
                    {
                        IScreen panel = Context.AppScreenManager.CreatePanel(PanelTypes.Alphabet) as IScreen;
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
                return ScreenUtils.SetFormStyles(base.CreateParams);
            }
        }

        protected override void WndProc(ref Message m)
        {
            _screenCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        void dockScanner()
        {
            positionWindow();

            if (_hWnd != IntPtr.Zero)
            {
                _automationElementDockTo = AutomationElement.FromHandle(_hWnd);
                try
                {
                    AutomationEventManager.AddAutomationPropertyChangedEventHandler(_hWnd, AutomationElement.BoundingRectangleProperty, _automationElementDockTo, onWindowPositionChanged);
                }
                catch
                {
                    _automationElementDockTo = null;
                }
            }
        }


        void onWindowPositionChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            positionWindow();
        }

        void positionWindow()
        {
            if (_targetWidget != null && _targetWidget.UIControl != null)
            {
                Control control = _targetWidget.UIControl;
                Rectangle rect = control.ClientRectangle;
                Log.Debug("control.ClientRectangle: " + rect.ToString());
                int y = this.Location.Y;
                Point pt = control.Location;
                Point screen = _parentForm.PointToScreen(pt);

                Point location = new Point(screen.X - _menuWidth, screen.Y);
                //Point location = new Point(screen.X - this.Width, screen.Y);

                this.Left = location.X;
                this.Top = location.Y;

                Log.Debug("Repositioned scanner to: " + location.X + ", " + location.Y);
            }
            else
            {
                System.Windows.Rect rect = getRect(AutomationElement.FocusedElement);
                double left = rect.Left;
                double top = rect.Top;

                this.Left = Convert.ToInt32(left) - this.ClientSize.Width;
                if (this.Left < 0)
                {
                    this.Left = 0;
                }
                this.Top = Convert.ToInt32(top);
                if (this.Top < 0)
                {
                    this.Top = 0;
                }
                Log.Debug("Positioning scanner at " + this.Left + ", " + this.Top);
            }

        }

        void setFormHeight()
        {
            List<Widget> children = new List<Widget>();
            _rootWidget.GetAllChildren(typeof(LabelButtonKey), children);
            int count = children.Count();

            Control.ControlCollection controls = flowLayoutPanel1.Controls;
            int total = controls.Count;

            for (int ii = count; ii < total; ii++)
            {
                controls[ii].Visible = false;
            }
            flowLayoutPanel1.Height -= (total - count) * B1.Height + 5;
            this.Height -= (total - count) * B1.Height;
        }

        void clickOnFocusedElement()
        {
            System.Windows.Rect rect;
            AutomationElement element = AutomationElement.FocusedElement;
            if (element == null)
            {
                return;
            }

            rect = element.Current.BoundingRectangle;

            Log.Debug("rect: " + rect.Top + " " + rect.Left + " " + rect.Width + " " + rect.Height);

            int xpos = Convert.ToInt32(rect.X);
            int ypos = Convert.ToInt32(rect.Y);
            xpos += 4;
            ypos += 4;
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            Thread.Sleep(50);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    }
}
