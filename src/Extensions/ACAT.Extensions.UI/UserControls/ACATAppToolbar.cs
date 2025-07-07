using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACATApp.UI
{
    public class ACATAppToolbar : UserControl
    {
        public RunCommandDispatcher CommandDispatcher => throw new NotImplementedException();

        public Form Form => throw new NotImplementedException();

        public string PanelClass => throw new NotImplementedException();

        public ScannerCommon ScannerCommon => throw new NotImplementedException();

        public ITextController TextController => throw new NotImplementedException();

        public ClassDescriptorAttribute Descriptor => throw new NotImplementedException();

        public IPanelCommon PanelCommon => throw new NotImplementedException();

        public SyncLock SyncObj => throw new NotImplementedException();

        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(StartupArg initArg)
        {
            throw new NotImplementedException();
        }

        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            throw new NotImplementedException();
        }

        public void OnPause()
        {
            throw new NotImplementedException();
        }

        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            throw new NotImplementedException();
        }

        public void OnResume()
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

        private TableLayoutPanel tableLayout;
        private Font defaultFont;
        private Font acatFont1Font;


        public ACATAppToolbar(Font defaultfont, Font acatfont1font) : base()
        {
            defaultFont = defaultfont;
            acatFont1Font = acatfont1font;

            InitializeComponents();
        }

        private void InitializeComponents()
        { 
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;

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

            var ToolbarButtons = new List<Control>
                {
                    new ScannerButtonControl { Name = "Settings", Text = "i", Font = acatFont1Font, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink},
                    new ScannerButtonControl { Name = "Help", Text = "F", Font = acatFont1Font,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink },
                    new ScannerButtonControl { Name = "About", Text = "!", Font = defaultFont,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink },
                    new ScannerButtonControl { Name = "Home", Text = "_", Font = defaultFont,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink },
                    new ScannerButtonControl { Name = "Minimize", Text = "_", Font = defaultFont,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink },
                    new ScannerButtonControl { Name = "CloseButton", Text = "X", Font = defaultFont, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink }
                };

            tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(2)
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                WrapContents = false
            };


            // Add the buttons to the tableLayout
            foreach (var button in ToolbarButtons)
            {
                //button.Font = defaultFont;
                button.BackColor = Color.Transparent;
                button.ForeColor = Color.White;

                buttonPanel.Controls.Add(button);
            }

            tableLayout.Controls.Add(appName, 0, 0);
            tableLayout.Controls.Add(buttonPanel, 1, 0);

            tableLayout.MouseDown += Toolbar_MouseDown;

            this.Controls.Add(tableLayout);
        }

        private void Toolbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ACAT.Win32.NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, Win32Constants.WM_NCLBUTTONDOWN, Win32Constants.HTCAPTION, 0);
            }
        }

        public void OnButtonActuated(Widget widget)
        {
            throw new NotImplementedException();
        }

        public void OnRunCommand(string command, ref bool handled)
        {
            throw new NotImplementedException();
        }
    }
}
