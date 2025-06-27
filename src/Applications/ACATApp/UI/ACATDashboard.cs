using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.Widgets;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACAT.Extensions.UI.Scanners.UserControls;
using ACAT.Win32;
using ACATResources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [Descriptor("7923DB0E-F4AF-4DDD-8FF4-5EDA5C034850", "ACATDashboard", "Main ACAT Dashboard Window")]
    public class ACATDashboard : Form, IScannerPanel
    {
        private Dispatcher _dispatcher;
        private TableLayoutPanel _panel;

        public IDescriptor Descriptor => throw new NotImplementedException();

        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        public RunCommandDispatcher CommandDispatcher => throw new NotImplementedException();

        public Form Form
        {
            get { return this; }
        }

        public string _panelClass;

        private readonly ScannerCommon _scannerCommon;

        public ITextController TextController => throw new NotImplementedException();

        private ScannerHelper _scannerHelper;
        private FlowLayoutPanel mainMenu;
        private KeyboardQwertyUserControl keyboardControl;
        private MouseScanner mouseScanner;

        public void OnPause()
        {
            _scannerCommon.UserControlManager.OnPause();
        }

        public void OnResume()
        {
            _scannerCommon.UserControlManager.OnResume();
            //_scannerCommon.AnimationManager.OnResume(_scannerCommon.RootWidget);
            //showMainMenu();
        }

        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            throw new NotImplementedException();
        }

        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            throw new NotImplementedException();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            throw new NotImplementedException();
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs widgetActuatedEvent, ref bool handled)
        {
            throw new NotImplementedException();
        }

        public void SetTargetControl(Form parent, Widget widget)
        {
            throw new NotImplementedException();
        }

        public ACATDashboard()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponents();

            _dispatcher = new Dispatcher(this);

        }

        public bool Initialize(StartupArg startupArg)
        {
            _panelClass = startupArg.PanelClass;

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
                ForeColor = Color.White
            };

            var ToolbarButtons = new List<ScannerRoundedButtonControl>
                {
                    new ScannerRoundedButtonControl { Name = "Settings", Text = "i", Font = acatFont1Font },
                    new ScannerRoundedButtonControl { Name = "Help", Text = "F", Font = acatFont1Font },
                    new ScannerRoundedButtonControl { Name = "About", Text = "!", Font = defaultFont },
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

        private FlowLayoutPanel BuildMainMenuPanel(Font acatIconFont, Font acatFont1Font)
        {
            var MainMenuButtons = new List<ScannerRoundedButtonControl>
                {
                    new ScannerRoundedButtonControl { Name = "ACatTalk", Text = "h", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "QuickTalk",Text = "i", Font =  new Font(acatIconFont.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "PointerControl", Text = "q", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "Keyboard", Text = "e", Font = new Font(acatFont1Font.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "System", Text = "M", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerRoundedButtonControl { Name = "Location", Text = "L", Font = new Font(acatIconFont.FontFamily, 44) },
                };

            var MainMenu = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Padding = new Padding(0, 0, 0, 0)
            };


            var MenuFont = new Font("Montserrat", 14, FontStyle.Regular);

            // Add the buttons to the toolbar
            foreach (var button in MainMenuButtons)
            {
                FormatButton(button, new Size(80, 80));

                //button.Font = MenuFont;
                //button.TextAlign = ContentAlignment.MiddleCenter;
                //button.TextImageRelation = TextImageRelation.ImageAboveText;
                //button.Text = button.Name switch
                //{
                //    "ACatTalk" => "ACAT Talk",
                //    "QuickTalk" => "Quick Talk",
                //    "PointerControl" => "Pointer",
                //    "Keyboard" => "Keyboard",
                //    "System" => "System",
                //    "Location" => "Location",
                //    _ => button.Text
                //};
                //button.ForeColor = Color.White;

                button.Click += (sender, e) =>
                {
                    var scannerButton = sender as ScannerRoundedButtonControl;
                    if (scannerButton != null)
                    {
                        this.OnScannerButtonClicked(scannerButton.Name);
                    }
                };
                MainMenu.Controls.Add(button);
            }

            return MainMenu;
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
            this.Hide();
            mouseScanner = new MouseScanner("MouseScanner", "Pointer Control");
            mouseScanner.ShowDialog();
            this.Show();
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
        }

        private void showMainMenu()
        {
            if (keyboardControl != null)
            {
                _panel.Controls.Remove(keyboardControl);
                keyboardControl = null;
            }
            _panel.Controls.Remove(mainMenu);
            mainMenu = BuildMainMenuPanel(new Font("ACAT ICON", 44), new Font("ACAT Font 1", 44));
            _panel.Controls.Add(mainMenu, 0, 1);
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

        private void InitializeComponents()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.AutoSize = true;

            _panel = new TableLayoutPanel
            {
                BackColor = Color.FromArgb(35, 36, 51),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                Dock = DockStyle.Fill,
            };

            this.Controls.Add(_panel);
        }

        private void ACATDashboardPanel_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the panel class for the scanner
        /// </summary>
        public String PanelClass
        {
            get { return _panelClass; }
        }

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
