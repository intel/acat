using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension;
using ACAT.Extensions.UI.Scanners.UserControls;
using ACAT.Scanners.UserControls;
using ACAT.Win32;
using ACATResources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ACAT.Extensions.UI.Scanners
{
    [Descriptor("7923DB0E-F4AF-4DDD-8FF4-5EDA5C034850",
        "ACATDashboard",
        "Main ACAT Dashboard Window")]
    public class ACATDashboard : HorizontalStripScanner
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
                    //OnCurrentControlChanged(EventArgs.Empty);
                }
            }
        }

        private UserControl mainMenu;
        private KeyboardQwertyUserControl keyboardControl;
        private PointerScannerUserControl pointerScanner;


        public override bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            // Handle the button click event here
            switch (arg.Command)
            {
                case "CmdShowACatTalk":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
                case "CmdShowQuickTalk":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
                case "CmdShowPointerControl":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
                case "CmdShowKeyboard":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
                case "CmdShowSystem":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
                case "CmdShowLocation":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
                default:
                    arg.Enabled = false;
                    arg.Handled = false;
                    break;
            }

            return true;
        }

        public ACATDashboard() : base ("ACATDashboardPanel", "ACAT Dashboard")
        {
            InitializeComponent();
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

        public bool InitializeComponent()
        {
            var defaultFont = new Font("Montserrat", 14, FontStyle.Regular);
            var acatIconFont = new Font("ACAT ICON", 18, FontStyle.Regular);
            var acatFont1Font = new Font("ACAT Font 1", 18, FontStyle.Regular);

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

        public override bool Initialize(StartupArg startupArg)
        {
            base.Initialize(startupArg);
            ScannerCommon.UserControlManager.AddUserControlByKeyOrName(this, "ACATDashboard", "ACATDashboard");
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
                bmp = WindowsStartButton(size.Height);
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
                var col = MainMenu.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                MainMenu.Controls.Add(button, col, 0);
            }

            panel.Controls.Add(MainMenu);
            return panel;
        }

        private void showPointerControl()
        {
            if (pointerScanner == null)
            {
                pointerScanner = new PointerScannerUserControl();
                //_scannerCommon.UserControlManager.AddUserControlByKeyOrName(pointerScanner, "usercontrol", "PointerScannerUserControl");
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
                //_scannerCommon.UserControlManager.AddUserControlByKeyOrName(keyboardControl, "keyboard", "KeyboardQwertyUserControl");
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

        public static Bitmap WindowsStartButton(int size)
        {
            // Create a square bitmap
            Bitmap bmp = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Define a simple 4-pane white Windows flag
                using (Brush flagBrush = new SolidBrush(Color.White))
                {
                    int margin = size / 6;
                    int half = size / 2 - margin / 2;

                    // Top-left pane
                    g.FillRectangle(flagBrush, margin, margin, half - margin / 2, half - margin / 2);

                    // Top-right pane
                    g.FillRectangle(flagBrush, size / 2 + margin / 2, margin, half - margin / 2, half - margin / 2);

                    // Bottom-left pane
                    g.FillRectangle(flagBrush, margin, size / 2 + margin / 2, half - margin / 2, half - margin / 2);

                    // Bottom-right pane
                    g.FillRectangle(flagBrush, size / 2 + margin / 2, size / 2 + margin / 2, half - margin / 2, half - margin / 2);
                }
            }

            return bmp;
        }

        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">the command</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">was the command handled?</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as ACATDashboard;

                switch (Command)
                {
                    case "CmdShowACatTalk":
                        form.showAcatTalk();
                        handled = true;
                        break;
                    case "CmdShowQuickTalk":
                        form.showAcatTalk();
                        handled = true;
                        break;
                    case "CmdShowPointerControl":
                        form.showPointerControl();
                        handled = true;
                        break;
                    case "CmdShowKeyboard":
                        form.showKeyboard();
                        handled = true;
                        break;
                    case "CmdShowSystem":
                        //form.showAcatTalk();
                        handled = true;
                        break;
                    case "CmdShowLocation":
                        //form.showAcatTalk();
                        handled = true;
                        break;
                    default:
                        handled = false;
                        break;
                }

                return true;
            }
        }
    }
}
