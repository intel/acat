////////////////////////////////////////////////////////////////////////////
// <copyright file="DashboardForm.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Utility;
using ACATDashboard;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Applications.ACATDashboard
{
    /// <summary>
    /// The main Form for ACAT Dashboard.  Displays icons of
    /// the apps, enables user to click on an icon to launch the app
    /// and also create shortcuts on the desktop.
    /// </summary>
    public partial class DashboardForm: Form
    {
        /// <summary>
        /// Icon _iconSize
        /// </summary>
        private const int _iconSize = 64;

        /// <summary>
        /// Maximum number of icons to display in each row
        /// </summary>
        private const int _maxIconsPerRow = 4;

        /// <summary>
        /// Windows message
        /// </summary>
        private const int LVM_FIRST = 0x1000;

        /// <summary>
        /// Windows message to control listview look-and-feel
        /// </summary>
        private const int LVM_SETICONSPACING = LVM_FIRST + 53;

        /// <summary>
        /// Windows message to control the context menu
        /// </summary>
        private const int MF_SEPARATOR = 0x800;

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Windows message to control the context menu
        /// </summary>
        private const int MF_STRING = 0x0;

        /// <summary>
        /// Message to close the app
        /// </summary>
        private const int WM_ACAT_CLOSE = (WM_USER + 1);

        /// <summary>
        /// Windows message
        /// </summary>
        private const int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// Windows message
        /// </summary>
        private const int WM_USER = 0x400;

        /// <summary>
        /// Spacing between the icons
        /// </summary>
        private readonly int _iconSpacing = 80;

        /// <summary>
        /// Sys tray icon for this app
        /// </summary>
        private readonly NotifyIcon _trayIcon;

        /// <summary>
        /// Systray contextual menu
        /// </summary>
        private readonly ContextMenu _trayMenu;

        /// <summary>
        /// Was the "exit" button invoked?
        /// </summary>
        private bool _exitButtonInvoked = false;

        /// <summary>
        /// Timer interval to check if any of the ACAT apps
        /// are running
        /// </summary>
        private int _interval = 2000;

        /// <summary>
        /// Open menu item
        /// </summary>
        private MenuItem _menuItemOpen;

        /// <summary>
        /// Timer to check if any of the ACAT apps are running
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Id of the menu item for exit
        /// </summary>
        private int SYSMENU_EXITID = 0x1;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public DashboardForm()
        {
            InitializeComponent();

            ShowInTaskbar = false;
            _trayMenu = createContextMenu();
            _trayIcon = createTrayIcon();

            subscribeToEvents();
        }

        /// <summary>
        /// Create the contexual menu for the sys tray
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            var hSysMenu = User32Interop.GetSystemMenu(this.Handle, false);

            User32Interop.AppendMenu(hSysMenu, MF_SEPARATOR, 0, string.Empty);
            User32Interop.AppendMenu(hSysMenu, MF_STRING, SYSMENU_EXITID, "E&xit…");
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m">window message</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == (WM_ACAT_CLOSE) ||
                ((m.Msg == WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_EXITID)))
            {
                _exitButtonInvoked = true;
                Close();
            }
        }

        /// <summary>
        /// Client size changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (_firstClientChangedCall)
            {
                _designTimeAspectRatio = (float)ClientSize.Height / ClientSize.Width;

                Log.Debug("HIHIHI ClientSize: " + ClientSize.ToString());
                _firstClientChangedCall = false;
            }
        }

        /// <summary>
        /// Calculates the number of icons per row and
        /// the number of rows and sets the size of the form
        /// appropriately
        /// </summary>
        private void arrangeIconsAndSetWindowSize()
        {
            int iconsPerRow = Math.Min(imageList1.Images.Count, _maxIconsPerRow);
            int numRows;
            if (iconsPerRow < _maxIconsPerRow)
            {
                numRows = 1;
            }
            else
            {
                if ((imageList1.Images.Count % _maxIconsPerRow) > 0)
                {
                    numRows = (imageList1.Images.Count / iconsPerRow) + 1;
                }
                else
                {
                    numRows = imageList1.Images.Count / iconsPerRow;
                }
            }

            Width = (_iconSize + _iconSpacing) * iconsPerRow + 50;
            Height = ((_iconSize + _iconSpacing) * numRows) + statusStrip1.Height + label1.Height + checkBoxHideWhenMinimized.Height + 100;
            setListViewSpacing(listView1, _iconSize + _iconSpacing, _iconSize + _iconSpacing);
        }

        /// <summary>
        /// Displays the yes/no dialog with the prompt
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>yes or no</returns>
        private DialogResult bigMessageBox(String prompt)
        {
            var ynForm = new YesNoForm(prompt);
            return ynForm.ShowDialog();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            OnExit(this, new EventArgs());
        }

        /// <summary>
        /// User clicked on the "Hide when minimized" chckbox
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void checkBoxHideWhenMinimized_Click(object sender, EventArgs e)
        {
            Program.Settings.HideWhenMinimzied = checkBoxHideWhenMinimized.Checked;
            Program.Settings.Save();
        }

        /// <summary>
        /// Checks if any of the apps launched from the dashboard
        /// are running
        /// </summary>
        /// <returns>true if they are</returns>
        private bool checkIfAppsAreRunning()
        {
            foreach (ListViewItem item in listView1.Items)
            {
                var tag = item.Tag as object[];
                var file = tag[0] as FileInfo;

                var name = Path.GetFileNameWithoutExtension(file.Name);
                if (FileUtils.IsRunning(name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the contextual menu for when the app is minimzied
        /// to the systray
        /// </summary>
        /// <returns>Context menu object</returns>
        private ContextMenu createContextMenu()
        {
            var retVal = new ContextMenu();
            _menuItemOpen = retVal.MenuItems.Add("&Open", OnOpen);

            retVal.MenuItems.Add("&Exit", OnExit);

            return retVal;
        }

        /// <summary>
        /// Creates a shortcut of the select icon (app) on the
        /// desktop
        /// </summary>
        /// <param name="appInfo">App info of the selecteda app</param>
        /// <param name="fileInfo">File info of the app</param>
        private void createShortcut(AppInfo appInfo, FileInfo fileInfo)
        {
            try
            {
                var wsh = new WshShellClass();

                var shortCutName = (String.IsNullOrEmpty(appInfo.ShortcutName)) ? appInfo.Name : appInfo.ShortcutName;

                var targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" +
                                 Path.GetFileNameWithoutExtension(shortCutName) + ".lnk";

                var shortcut = wsh.CreateShortcut(targetPath) as IWshShortcut;
                if (shortcut != null)
                {
                    shortcut.Arguments = appInfo.CommandLine;
                    shortcut.TargetPath = fileInfo.FullName;
                    shortcut.WorkingDirectory = !String.IsNullOrEmpty(appInfo.WorkingDirectory) ? appInfo.WorkingDirectory : Path.GetDirectoryName(shortcut.TargetPath);
                    shortcut.WindowStyle = 1;
                    shortcut.Description = appInfo.Description;
                    shortcut.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create shortcut. Exception: " + ex, Name);
            }
        }

        /// <summary>
        /// Creates the systray icon for the dashboard
        /// </summary>
        /// <returns>The icon</returns>
        private NotifyIcon createTrayIcon()
        {
            var trayIcon = new NotifyIcon
            {
                Text = this.Text,
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                ContextMenu = _trayMenu,
                Visible = false,
                BalloonTipText = this.Text + " minimized to tray",
                BalloonTipIcon = ToolTipIcon.Info
            };

            trayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            return trayIcon;
        }

        /// <summary>
        /// Form closing event handler
        /// </summary>
        /// <param name="sender">event handler</param>
        /// <param name="e">event args</param>
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (!_exitButtonInvoked &&
                e.CloseReason == CloseReason.UserClosing && checkBoxHideWhenMinimized.Checked)
            {
                hideThis();

                e.Cancel = true;
                return;
            }

            _exitButtonInvoked = false;

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }

            if (_trayMenu != null)
            {
                _trayMenu.Dispose();
            }
        }

        /// <summary>
        /// Form load event handler. Populate the list view
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            Log.Debug("HIHIHI AspectRatio: " + currentAspectRatio + ", design: " + _designTimeAspectRatio);

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {

                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));

                Log.Debug("HIHIHI New clientsize: " + ClientSize.ToString());
            }

            Log.Debug("HIHIHI currentsize: " + Size.ToString());

            CenterToScreen();

            TopMost = false;
            TopMost = true;

            var appList = Program.Settings.Applications;
            checkBoxHideWhenMinimized.Checked = Program.Settings.HideWhenMinimzied;

            Debug.WriteLine("applist.Count: " + appList.Length);

            var files = new List<FileInfo>();
            var appsIncluded = new List<AppInfo>();

            foreach (var appInfo in appList)
            {
                try
                {
                    Debug.WriteLine("appInfo.Path: " + appInfo.Path);

                    var fullPath = Path.GetFullPath(appInfo.Path);

                    Debug.WriteLine("fullPath: " + fullPath);

                    var bitmap = getIconAsBitmap(fullPath);

                    if (bitmap != null)
                    {
                        var b = ResizeImage(bitmap, _iconSize, _iconSize);
                        imageList1.Images.Add(b);

                        files.Add(new FileInfo(fullPath));
                        appsIncluded.Add(appInfo);
                    }
                    else
                    {
                        Debug.WriteLine("Icon is null");
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            }

            listView1.View = View.LargeIcon;
            imageList1.ImageSize = new Size(_iconSize, _iconSize);
            listView1.LargeImageList = imageList1;

            for (var imageIndex = 0; imageIndex < imageList1.Images.Count; imageIndex++)
            {
                var item = new ListViewItem
                {
                    ImageIndex = imageIndex,
                    Text = !String.IsNullOrEmpty(appsIncluded[imageIndex].Name)
                            ? appsIncluded[imageIndex].Name
                            : Path.GetFileNameWithoutExtension((files[imageIndex].Name)),
                    //Font = new Font("Arial", 14, FontStyle.Regular),
                    Tag = new object[] { files[imageIndex], appsIncluded[imageIndex] }
                };

                item.ToolTipText = item.Text;
                listView1.Items.Add(item);
            }

            listView1.ShowItemToolTips = true;
            listView1.MultiSelect = false;

            arrangeIconsAndSetWindowSize();

            listView1.Activation = ItemActivation.OneClick;

            CenterToScreen();
        }

        /// <summary>
        /// Extracts the icon associated with the specified exe
        /// </summary>
        /// <param name="exeFullPath">fullpath to the ext</param>
        /// <returns>icon, null if none found</returns>
        private Bitmap getIconAsBitmap(String exeFullPath)
        {
            var extractor = new IconExtractor(exeFullPath);
            var icon = extractor.GetIcon(0);

            if (icon != null)
            {
                var splitIcons = IconUtil.Split(icon);

                if (splitIcons.Length > 0)
                {
                    return ImageUtils.ConvertIconToBitmap(splitIcons[0]);
                }
            }

            return null;
        }

        /// <summary>
        /// Hides this form and shows the systray icon
        /// </summary>
        private void hideThis()
        {
            Visible = false;
            _trayIcon.Visible = true;
        }

        /// <summary>
        /// User clicked on a list view item. Launches the app selected
        /// by the user and minimizes the dashboard to the systray (if
        /// configured to do so) otherwise quits
        /// </summary>
        private void launchApp()
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                return;
            }

            var selectedItem = listView1.SelectedItems[0];
            var tag = selectedItem.Tag as object[];
            var file = tag[0] as FileInfo;
            var appInfo = tag[1] as AppInfo;

            selectedItem.Selected = false;

            hideThis();

            var processStartInfo = new ProcessStartInfo
            {
                FileName = file.FullName,
                Arguments = appInfo.CommandLine
            };

            if (!String.IsNullOrEmpty(appInfo.WorkingDirectory))
            {
                processStartInfo.WorkingDirectory = appInfo.WorkingDirectory;
            }

            try
            {
                Process.Start(processStartInfo);

                if (!checkBoxHideWhenMinimized.Checked)
                {
                    Close();
                }
                else if (_timer == null)
                {
                    _timer = new Timer { Enabled = false, Interval = _interval };
                    _timer.Tick += timer_Tick;
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error launching " + appInfo.Name + ", FullPath: " + appInfo.Path + ", Exception: " + ex);
            }
        }

        /// <summary>
        /// User right clicked on the systray icon.  Display the contextual
        /// menu
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenu();
                var menuItem = new MenuItem("Create desktop shortcut");
                menuItem.Click += menuItem_OnClick;
                contextMenu.MenuItems.Add(menuItem);

                var lvi = listView1.GetItemAt(e.Location.X, e.Location.Y);

                if (lvi != null)
                {
                    menuItem.Tag = lvi;
                    contextMenu.Show(listView1, new Point(e.X, e.Y));
                }

                if (listView1.SelectedItems.Count > 0)
                {
                    listView1.SelectedItems[0].Selected = false;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                launchApp();
            }
        }

        /// <summary>
        /// Mouse moved away from the listview
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listView1_MouseLeave(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = String.Empty;
            listView1.SelectedIndices.Clear();
        }

        /// <summary>
        /// Mouse moved in the list view
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            var listViewItem = listView1.GetItemAt(e.Location.X, e.Location.Y);

            if (listViewItem == null)
            {
                toolStripStatusLabel1.Text = String.Empty;
            }
            else
            {
                listViewItem.Selected = true;
                var tag = listViewItem.Tag as object[];
                var appInfo = tag[1] as AppInfo;

                toolStripStatusLabel1.Text = appInfo.Description + "  (Right click for options)";
            }
        }

        /// <summary>
        /// User selected a menu item from the context menu
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void menuItem_OnClick(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                return;
            }

            var selectedItem = menuItem.Tag as ListViewItem;

            if (selectedItem == null)
            {
                return;
            }

            var tag = selectedItem.Tag as object[];
            var file = tag[0] as FileInfo;
            var appInfo = tag[1] as AppInfo;

            if ((bigMessageBox("Create desktop shortcut for " + appInfo.Name + "?") == DialogResult.Yes))
            {
                createShortcut(appInfo, file);
            }
        }

        /// <summary>
        /// Exit the app
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void OnExit(object sender, EventArgs e)
        {
            _exitButtonInvoked = true;
            Close();
        }

        /// <summary>
        /// Show the dashboard window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void OnOpen(object sender, EventArgs e)
        {
            if (checkIfAppsAreRunning())
            {
                SystemSounds.Beep.Play();
                return;
            }

            showThis();
        }

        /// <summary>
        /// Sets the _iconSpacing in the list view between icons
        /// </summary>
        /// <param name="listView">list view</param>
        /// <param name="x">horizontal _iconSpacing</param>
        /// <param name="y">vertical _iconSpacing</param>
        private void setListViewSpacing(ListView listView, int x, int y)
        {
            User32Interop.SendMessage(listView.Handle, LVM_SETICONSPACING, 0, x * 65536 + y);
        }

        /// <summary>
        /// Shows this form and hides the systray icon
        /// </summary>
        private void showThis()
        {
            Visible = true;
            _trayIcon.Visible = false;
        }

        /// <summary>
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            Load += Form1_Load;
            FormClosing += Form1_Closing;
            listView1.MouseMove += listView1_MouseMove;
            listView1.MouseClick += listView1_MouseClick;
            listView1.MouseLeave += listView1_MouseLeave;
            checkBoxHideWhenMinimized.Click += checkBoxHideWhenMinimized_Click;
        }

        /// <summary>
        /// Timer function.  Checks if any of the apps in the
        /// dashboard are running. If none are running, displays
        /// the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (checkIfAppsAreRunning())
            {
                _menuItemOpen.Enabled = false;
                return;
            }

            _menuItemOpen.Enabled = true;

            _timer.Stop();
            _timer.Dispose();
            _timer = null;

            showThis();
        }

        /// <summary>
        /// Mouse double click handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnOpen(this, new EventArgs());
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}