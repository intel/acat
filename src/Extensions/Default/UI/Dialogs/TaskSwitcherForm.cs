////////////////////////////////////////////////////////////////////////////
// <copyright file="TaskSwitecherForm.cs" company="Intel Corporation">
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

////#define FORCE_FIXED_TABLE_SIZE
////#define SHOW_TASK_TABLE_BORDER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;

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
    [DescriptorAttribute("9D73504D-518E-4B24-9B39-D6E2DC1BFD5B", "TaskSwitcherForm", "Task Switcher Dialog")]
    public partial class TaskSwitcherForm : Form, IDialogPanel, IExtension
    {
        private const int WS_SYSMENU = 0x80000;
        private const int TableLocationX = 75;
        private const int TableLocationY = 75;
        private const int TableWidth = 200;
        private const int TableHeight = 55;
        private const int NumAppsPerRow = 10;
        private const float RowHeight = 75F;
        private const int ColumnWidth = 75;
        private const int FormHeightBase = 85;
        private const int FormWidthBase = 85;

        private DialogCommon _dialogCommon;
        private List<PictureBoxWidget> _widgetList;
        private TableLayoutPanel dtlp;
        private TaskData _selectedTask;
        private Process _filterByProcess;
        private ExtensionInvoker _invoker;

        public TaskSwitcherForm()
        {
            Log.Debug();

            _invoker = new ExtensionInvoker(this);

            FilterProcessName = String.Empty;

            init();
        }

        public TaskSwitcherForm(Process process)
        {
            init();
            _filterByProcess = process;
        }

        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
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

        public String FilterProcessName { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                CreateParams createParams = base.CreateParams;
                createParams.Style &= ~WS_SYSMENU;
                createParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_NOACTIVATE;
                return createParams;
            }
        }

        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Triggered when a widget is triggered
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            Log.Debug("**Actuate** " + widget.UIControl.Name + " Value: " + widget.Value);

            String value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                Log.Debug("OnButtonActuated() -- received actuation from empty widget!");
                return;
            }

            Invoke(new MethodInvoker(delegate()
            {
                switch (value)
                {
                    case "valButtonCancel":
                        cancel();
                        break;

                    default:
                        // TODO add default case?
                        Log.Debug("OnButtonActuated() -- unhandled widget actuation!");
                        break;
                }
            }));
        }

        public void OnPause()
        {
            Log.Debug("Pausing...");
            _dialogCommon.OnPause();

            Windows.SetVisible(this, false);
        }

        public void OnResume()
        {
            Log.Debug("Resuming...");
            _dialogCommon.OnResume();

            Windows.SetVisible(this, true);
        }

        public void OnRunCommand(string command, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                default:
                    handled = false;
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Subscribes to all the events triggered by the
        /// widgets and the interpreter
        /// </summary>
        private void subscribeToEvents()
        {
            List<Widget> widgetList = new List<Widget>();
            _dialogCommon.GetRootWidget().Finder.FindAllButtons(widgetList);
            foreach (Widget widget in widgetList)
            {
                widget.EvtHighlightOn += new HighlightOnDelegate(widget_EvtHighlightOn);
            }
        }

        private void widget_EvtHighlightOn(Widget widget, out bool handled)
        {
            handled = false;
            Windows.SetText(Title, widget.SubClass);
        }

        private void cancel()
        {
            if (_selectedTask != null)
            {
                User32Interop.SetForegroundWindow(_selectedTask.Handle);
                User32Interop.ShowWindow(_selectedTask.Handle.ToInt32(), User32Interop.SW_SHOW);

                User32Interop.BringWindowToTop(_selectedTask.Handle);
            }

            Windows.CloseForm(this);
        }

        private void PopulateTLP(List<EnumWindows.WindowInfo> windowList)
        {
            Log.Debug("Starting to populate table layout panel...");

            int appCount = windowList.Count;
            if (appCount == 0)
            {
                return;
            }

            int columnWidth;

            _widgetList = new List<PictureBoxWidget>();

            // add rows and columns
            dtlp.ColumnCount = NumAppsPerRow;
            dtlp.RowCount = (int)Math.Ceiling((double)appCount / (double)NumAppsPerRow);

            columnWidth = ColumnWidth;

            dtlp.ColumnStyles.Clear();

            // add each column
            for (int y = 0; y < NumAppsPerRow; y++)
            {
                dtlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, columnWidth));
            }

            // start adding app icons
            // used to break out of loop once we have processed all the apps

            int i = 0;

            dtlp.RowStyles.Clear();

            for (int x = 0; x < dtlp.RowCount; x++)
            {
                dtlp.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));
            }

            for (int currentRow = 0, index = 0; currentRow < dtlp.RowCount; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < NumAppsPerRow; currentColumn++)
                {
                    PictureBox pbxAppIcon1 = new PictureBox();
                    pbxAppIcon1.Name = "ListItem" + index++;
                    pbxAppIcon1.SizeMode = PictureBoxSizeMode.CenterImage;

                    PictureBoxWidget pictureBoxWidget = _dialogCommon.GetWidgetManager().Layout.CreateWidget(typeof(PictureBoxWidget), pbxAppIcon1) as PictureBoxWidget;
                    pictureBoxWidget.EvtActuated += new WidgetEventDelegate(bitmapWidget_EvtActuated);
                    pictureBoxWidget.EvtHighlightOn += new HighlightOnDelegate(bitmapWidget_EvtHighlightOn);

                    Log.Debug("Bitmap widget is " + ((pictureBoxWidget == null) ? "null" : "not null"));

                    TaskData taskData = new TaskData();
                    taskData.Handle = windowList[i].Handle;
                    taskData.Title = windowList[i].Title;

                    pictureBoxWidget.UserData = taskData;

                    IntPtr handle = windowList[i].Handle;
                    Icon icon = EnumWindows.GetAppIcon(handle);

                    if (icon == null)
                    {
                        Log.Debug("No app icon found for hWnd=" + handle.ToString() + " title=" + windowList[i].Title);
                        Log.Debug("Loading default icon");
                        Image img = Image.FromFile(FileUtils.GetImagePath("taskSwitchdefaultIcon.bmp"));
                        if (img != null)
                        {
                            pbxAppIcon1.Image = img;
                        }
                    }
                    else
                    {
                        Log.Debug("Icon found for hWnd=" + handle.ToString() + " icon=" + icon.ToString() + " title=" + windowList[i].Title);
                        pbxAppIcon1.Image = icon.ToBitmap();
                    }

                    pbxAppIcon1.BorderStyle = BorderStyle.None;
                    pbxAppIcon1.Anchor = AnchorStyles.None;
                    pbxAppIcon1.Dock = DockStyle.Fill;
                    pbxAppIcon1.SizeMode = PictureBoxSizeMode.StretchImage;
                    dtlp.Controls.Add(pbxAppIcon1, currentColumn, currentRow);

                    _widgetList.Add(pictureBoxWidget);

                    // TODO: do something else with spare cells
                    if (i < (appCount - 1))
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

#if FORCE_FIXED_TABLE_SIZE
            this.Size = new System.Drawing.Size(1000, 1000);
#else
            int newFormWidth = FormWidthBase + (NumAppsPerRow * ColumnWidth);
            int newFormHeight = 20 + (FormHeightBase + Convert.ToInt32(Math.Ceiling(dtlp.RowCount * RowHeight)));
            Log.Debug("newFormWidth=" + newFormWidth + " newFormHeight=" + newFormHeight);

            _dialogCommon.Resize(new System.Drawing.Size(newFormWidth, newFormHeight));
            onResize();

            Log.Debug("form width=" + this.Size.Width.ToString() + " form height=" + this.Size.Height.ToString());
            Log.Debug("title width=" + Title.Size.Width);
#endif
        }

        private void onResize()
        {
            Invoke(new MethodInvoker(delegate()
                {
                    Title.Size = new System.Drawing.Size(Size.Width - 8, Title.Height);
                    Title.Left = (ClientSize.Width - Title.Width) / 2;
                }));
        }

        private void bitmapWidget_EvtActuated(object sender, WidgetEventArgs e)
        {
            Widget widget = e.SourceWidget;

            Log.Debug("widget=" + widget.ToString());

            if (widget is PictureBoxWidget)
            {
                PictureBoxWidget pictureBoxWidget = widget as PictureBoxWidget;
                TaskData taskData = pictureBoxWidget.UserData as TaskData;

                Log.Debug("Bringing selected window to foreground hWnd=" + taskData.Handle + " title=" + taskData.Title.ToString());

                setTitle(widget);

                _selectedTask = null;

                Windows.ActivateWindow(taskData.Handle);
                _selectedTask = taskData;
            }
            else
            {
                Log.Debug("actuated non-BitmapWidget");
            }
        }

        private void bitmapWidget_EvtHighlightOn(Widget widget, out bool handled)
        {
            Log.Debug("widget=" + widget.ToString());

            handled = false;

            setTitle(widget);
        }

        private void setTitle(Widget widget)
        {
            if (widget is PictureBoxWidget)
            {
                PictureBoxWidget pictureBoxWidget = widget as PictureBoxWidget;
                TaskData taskData = pictureBoxWidget.UserData as TaskData;

                Log.Debug("Setting title to " + taskData.Title.ToString());
                Windows.SetText(Title, taskData.Title.ToString());
            }
            else
            {
                Windows.SetText(Title, widget.GetText());
            }
        }

        private void CreateTaskTable()
        {
            dtlp = new TableLayoutPanel();
            dtlp.RowCount = 0; // init to be sure
            dtlp.RowStyles.Clear();
            dtlp.AutoScroll = false;
            dtlp.Location = new System.Drawing.Point(TableLocationX, TableLocationY);
            dtlp.Name = "TaskList"; // does this name need to match the xml config file?
            dtlp.Size = new System.Drawing.Size(TableWidth, TableHeight);
            Log.Debug(" dtlp.Size.Width=" + dtlp.Size.Width + " dtlp.Size.Height=" + dtlp.Size.Height);

            dtlp.BackColor = Color.Transparent;

#if SHOW_TASK_TABLE_BORDER
            dtlp.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
#endif

            dtlp.AutoSize = true;
            dtlp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            dtlp.Margin = new System.Windows.Forms.Padding(10, 10, 10, 10);
            dtlp.BackColor = Color.Transparent;

            this.Controls.Add(dtlp);
        }

        private void init()
        {
            Log.Debug();
            InitializeComponent();

            _filterByProcess = null;

            CreateTaskTable();

            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            this.Load += new EventHandler(TaskSwitcherForm_Load);
            this.FormClosing += new FormClosingEventHandler(TaskSwitcherForm_FormClosing);

            _dialogCommon.GetAnimationManager().EvtResolveWidgetChildren +=
                new AnimationManager.ResolveWidgetChildren(TaskSwitcherForm_EvtResolveWidgetChildren);

            Title.FlatAppearance.BorderSize = 0;

            Log.Debug("returning");
        }

        private void TaskSwitcherForm_EvtResolveWidgetChildren(object sender, ResolveWidgetChildrenEventArgs e)
        {
            e.ContainerWidget.ClearAllChildren();

            foreach (Widget widget in _widgetList)
            {
                e.ContainerWidget.AddChild(widget);
            }
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        private void TaskSwitcherForm_Load(object sender, EventArgs e)
        {
            Log.Debug();

            dtlp.Show();

            _dialogCommon.OnLoad();
            subscribeToEvents();

            var windowList = EnumWindows.Enumerate();

            if (!String.IsNullOrEmpty(FilterProcessName))
            {
                List<EnumWindows.WindowInfo> filteredList = new List<EnumWindows.WindowInfo>();

                for (int ii = 0; ii < windowList.Count; ii++)
                {
                    Process process = WindowActivityMonitor.GetProcessForWindow(windowList[ii].Handle);
                    if (String.Compare(process.ProcessName, FilterProcessName, true) == 0)
                    {
                        filteredList.Add(windowList[ii]);
                    }
                }

                windowList = filteredList;
            }
            else
            {
                Log.Debug("list count=" + windowList.Count);

                IntPtr desktopHWnd = User32Interop.GetDesktopWindow();

                Log.Debug("desktopHWnd=" + desktopHWnd.ToString());

                if (desktopHWnd != null)
                {
                    windowList.Insert(0, new EnumWindows.WindowInfo(desktopHWnd, "Show Desktop"));
                }
            }

            PopulateTLP(windowList);

            Windows.SetWindowPosition(this, Windows.WindowPosition.CenterScreen);

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
            Log.Debug("returning");
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        private void TaskSwitcherForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        private class TaskData
        {
            public IntPtr Handle { get; set; }

            public String Title { get; set; }
        }
    }
}