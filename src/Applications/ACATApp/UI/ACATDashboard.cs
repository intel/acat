using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.Widgets;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACAT.Extensions.UI.Scanners.UserControls;
using ACAT.Scanners.UserControls;
using ACAT.Win32;
using ACATApp.Utilities;
using ACATResources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [Descriptor("7923DB0E-F4AF-4DDD-8FF4-5EDA5C034850",
        "ACATDashboard",
        "Main ACAT Dashboard Window")]
    public class ACATDashboard : Form, IScannerPanel
    {
        private Dispatcher _dispatcher;

        private TableLayoutPanel _panel;

        private UserControl _currentUIElement;

        private UserControl currentUIElement
        {
            get { return _currentUIElement; }
            set
            {
                if (_currentUIElement != value)
                {
                    _currentUIElement = value;
                    OnCurrentControlChanged(EventArgs.Empty);
                }
            }
        }

        #region ACAT Dashboard Custom
        public event EventHandler CurrentControlChanged;

        protected virtual void OnCurrentControlChanged(EventArgs e)
        {
            CurrentControlChanged?.Invoke(this, e);
        }

        private UserControl mainMenu;
        private KeyboardQwertyUserControl keyboardControl;
        private PointerScannerUserControl pointerScanner;
        #endregion

        public IDescriptor Descriptor => DescriptorAttribute.GetDescriptor(GetType());


        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        public RunCommandDispatcher CommandDispatcher => throw new NotImplementedException();

        public Form Form
        {
            get { return this; }
        }

        private readonly ScannerCommon _scannerCommon;

        public ITextController TextController => throw new NotImplementedException();

        private ScannerHelper _scannerHelper;

        public void OnPause()
        {
            _scannerCommon.UserControlManager.OnPause();
        }

        public void OnResume()
        {
            _scannerCommon.UserControlManager.OnResume();
        }

        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            throw new NotImplementedException();
        }

        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs widgetActuatedEvent, ref bool handled)
        {
            //throw new NotImplementedException();
        }

        public void SetTargetControl(Form parent, Widget widget)
        {
            //throw new NotImplementedException();
        }

        public ACATDashboard()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            CurrentControlChanged += ACATDashboard_CurrentControlChanged;

            _dispatcher = new Dispatcher(this);

        }

        private void ACATDashboard_CurrentControlChanged(object sender, EventArgs e)
        {
            var toolbar = _panel.GetControlFromPosition(0, 0) as TableLayoutPanel;

            if (currentUIElement == mainMenu)
            {
                toolbar.Controls.Find("Settings", true)[0].Visible = true;
                toolbar.Controls.Find("Help", true)[0].Visible = true;
                toolbar.Controls.Find("About", true)[0].Visible = true;
                toolbar.Controls.Find("Home", true)[0].Visible = false;
            }
            else
            {
                toolbar.Controls.Find("Settings", true)[0].Visible = false;
                toolbar.Controls.Find("Help", true)[0].Visible = false;
                toolbar.Controls.Find("About", true)[0].Visible = false;
                toolbar.Controls.Find("Home", true)[0].Visible = true;
            }

            toolbar.Controls.Find("Minimize", true)[0].Visible = true;
            toolbar.Controls.Find("CloseButton", true)[0].Visible = true;
        }

        public bool Initialize(StartupArg startupArg)
        {
            PanelClass = startupArg.PanelClass;

            _scannerHelper = new ScannerHelper(this, startupArg);

            _scannerCommon.Initialize(startupArg);

            //Text = title;

            var defaultFont = new Font("Montserrat", 14, FontStyle.Regular);
            var acatIconFont = new Font("ACAT ICON", 18, FontStyle.Regular);
            var acatFont1Font = new Font("ACAT Font 1", 18, FontStyle.Regular);


            Label appName = new Label
            {
                Name = "ACAT",
                Text = "ACAT Dashboard",
                Font = defaultFont,
                AutoSize = true,
                Padding = new Padding(10),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                ForeColor = Color.AntiqueWhite
            };

            var ToolbarButtons = new List<ScannerRoundedButtonControl>
                {
                    new ScannerRoundedButtonControl { Name = "Settings", Text = "i", Font = acatFont1Font },
                    new ScannerRoundedButtonControl { Name = "Help", Text = "F", Font = acatFont1Font },
                    new ScannerRoundedButtonControl { Name = "About", Text = "!", Font = defaultFont },
                    new ScannerRoundedButtonControl { Name = "Home", Text = "M", Font = acatIconFont },
                    new ScannerRoundedButtonControl { Name = "Minimize", Text = "_", Font = defaultFont },
                    new ScannerRoundedButtonControl { Name = "CloseButton", Text = "X", Font = defaultFont }
                };

            var toolbar = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(2)
            };
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            toolbar.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                WrapContents = false
            };


            // Add the buttons to the toolbar
            foreach (var button in ToolbarButtons)
            {
                FormatButton(button, new Size(36, 36));
                button.Visible = false; // Initially hide the buttons
                button.Click += (sender, e) =>
                {
                    var scannerButton = sender as ScannerRoundedButtonControl;
                    if (scannerButton != null)
                    {
                        switch (scannerButton.Name)
                        {
                            case "Settings":
                                ShowSettingsPanel();
                                break;
                            case "Help":
                                ShowHelpPanel();
                                break;
                            case "About":
                                ShowAboutPanel();
                                break;
                            case "Home":
                                showMainMenu();
                                break;
                            case "Minimize":
                                this.WindowState = FormWindowState.Minimized;
                                break;
                            case "CloseButton":
                                this.Close();
                                break;
                        }
                    }
                };
                buttonPanel.Controls.Add(button);
            }

            toolbar.Controls.Add(appName, 0, 0);
            toolbar.Controls.Add(buttonPanel, 1, 0);

            toolbar.MouseDown += Toolbar_MouseDown;

            _panel.Controls.Add(toolbar, 0, 0);

            mainMenu = BuildMainMenuPanel(acatIconFont, acatFont1Font);
            _panel.Controls.Add(mainMenu, 0, 1);

            currentUIElement = mainMenu;

            return true;
        }

        private static void FormatButton(ScannerRoundedButtonControl button, Size size)
        {
            Bitmap bmp = RenderButtonIcon(button, size);

            button.Image = bmp;
            button.Text = "";
            button.BackColor = Color.Transparent;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.Size = size;
            button.BorderColor = System.Drawing.Color.DimGray;
            button.BorderRadiusBottomLeft = 12;
            button.BorderRadiusBottomRight = 12;
            button.BorderRadiusTopLeft = 12;
            button.BorderRadiusTopRight = 12;
            button.BorderWidth = 3F;
        }

        private static Bitmap RenderButtonIcon(ScannerRoundedButtonControl button, Size size)
        {
            Bitmap bmp = new Bitmap(size.Height, size.Width);

            if (button.Name == "Windows")
            {
                bmp = ButtonIconGenerator.WindowsStartButton(size.Height);
                return bmp;
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // measure the symbol size
                SizeF symbolSize = g.MeasureString(button.Text, button.Font);

                // calculate offsets to center
                float x = (bmp.Width - symbolSize.Width) / 2;
                float y = (bmp.Height - symbolSize.Height) / 2;

                g.DrawString(button.Text, button.Font, Brushes.White, x, y);
            }

            return bmp;
        }

        private UserControl BuildMainMenuPanel(Font acatIconFont, Font acatFont1Font)
        {
            UserControl panel = new UserControl
            {
                Dock = DockStyle.Fill,
                //AutoSize = true,
                //AutoSizeMode = AutoSizeMode.GrowOnly,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 0),
                Size = new Size(550, 100)
            };

            var MainMenuButtons = new List<ScannerRoundedButtonControl>
                {
                    new ScannerRoundedButtonControl { Name = "ACatTalk", Text = "h", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "QuickTalk",Text = "i", Font =  new Font(acatIconFont.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "PointerControl", Text = "q", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "Keyboard", Text = "e", Font = new Font(acatFont1Font.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "Windows", Text = "" },
                    new ScannerRoundedButtonControl { Name = "Location", Text = "L", Font = new Font(acatIconFont.FontFamily, 44) },
                };

            var MainMenu = new TableLayoutPanel
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 0, 0, 0),
                GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
            };


            var MenuFont = new Font("Montserrat", 14, FontStyle.Regular);

            // Add the buttons to the toolbar
            foreach (var button in MainMenuButtons)
            {
                FormatButton(button, new Size(80, 80));

                button.Click += (sender, e) =>
                {
                    var scannerButton = sender as ScannerRoundedButtonControl;
                    if (scannerButton != null)
                    {
                        this.OnScannerButtonClicked(scannerButton.Name);
                    }
                };
                var col = MainMenu.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                MainMenu.Controls.Add(button, col, 0);
            }

            panel.Controls.Add(MainMenu);
            return panel;
        }

        private void OnScannerButtonClicked(string name)
        {
            // Handle the button click event here
            switch (name)
            {
                case "ACatTalk":
                    showAcatTalk();
                    break;
                case "QuickTalk":
                    // Open Quick Talk panel
                    break;
                case "PointerControl":
                    showPointerControl();
                    break;
                case "Keyboard":
                    showKeyboard();
                    break;
                case "System":
                    // Open System panel
                    break;
                case "Location":
                    // Open Location panel
                    break;
                default:
                    throw new NotImplementedException($"Button {name} not implemented.");
            }
        }

        private void showPointerControl()
        {
            if (pointerScanner == null)
            {
                pointerScanner = new PointerScannerUserControl();
                _scannerCommon.UserControlManager.AddUserControlByKeyOrName(pointerScanner, "usercontrol", "PointerScannerUserControl");
            }

            _panel.Controls.Remove(currentUIElement);
            _panel.Controls.Add(pointerScanner, 0, 1);

            currentUIElement = pointerScanner;

        }

        private void showAcatTalk()
        {
            this.Hide();
            var startupArg = new StartupArg("TalkApplicationScanner")
            {
                QuitAppOnFormClose = false
            };

            var form = PanelManager.Instance.CreatePanel("TalkApplicationScanner", startupArg);
            if (form != null)
            {
                // Add ad-hoc agent that will handle the form
                IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Talk Application Agent");
                if (agent == null)
                {
                    MessageBox.Show("Could not find application agent for this application.");
                    return;
                }

                Context.AppAgentMgr.AddAgent(form.Handle, agent);
                Context.AppPanelManager.ShowDialog(form as IPanel);
            }
            else
            {
                MessageBox.Show(String.Format(StringResources.InvalidFormName, "TalkApplicationScanner"));
                return;
            }

            this.Show();
        }

        public void ShowSettingsPanel()
        {
            var settingsPanel = new Core.PreferencesManagement.ACATConfigMainForm();
            settingsPanel.ShowDialog();
        }

        public void ShowHelpPanel()
        {
            ConfirmBoxOneOption.ShowDialog("Help", "ACAT Dashboard Help", "This is the help panel for the ACAT Dashboard. It provides information on how to use the dashboard and its features.", null, true);
        }

        public void ShowAboutPanel()
        {
            var aboutPanel = new AboutBoxForm("ACAT Dashboard");
            aboutPanel.ShowDialog();
        }

        private void showKeyboard()
        {
            if (keyboardControl == null)
            {
                keyboardControl = new KeyboardQwertyUserControl();
                _scannerCommon.UserControlManager.AddUserControlByKeyOrName(keyboardControl, "keyboard", "KeyboardQwertyUserControl");
            }


            _panel.Controls.Remove(mainMenu);
            _panel.Controls.Add(keyboardControl, 0, 1);

            currentUIElement = keyboardControl;
        }

        private void showMainMenu()
        {
            _panel.Controls.Remove(currentUIElement);

            if (mainMenu == null)
            {
                mainMenu = BuildMainMenuPanel(new Font("ACAT ICON", 44), new Font("ACAT Font 1", 44));
            }

            _panel.Controls.Add(mainMenu, 0, 1);
            currentUIElement = mainMenu;
        }

        private void Toolbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ACAT.Win32.NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, Win32Constants.WM_NCLBUTTONDOWN, Win32Constants.HTCAPTION, 0);
            }
        }

        private void ACATDashboardPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.AutoSize = true;

            _panel = new TableLayoutPanel
            {
                BackColor = Color.Transparent,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
            };

            _panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            this.Controls.Add(_panel);
        }

        private void ACATDashboardPanel_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the panel class for the scanner
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon
        { get { return _scannerCommon; } }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }
        public class Dispatcher : DefaultCommandDispatcher
        {
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
            }
            public new void DispatchCommand(String command, ref bool handled)
            {
                // Handle commands specific to the dashboard panel here
                base.DispatchCommand(command, ref handled);
            }
        }
    }
}
