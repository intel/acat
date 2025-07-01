using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACATApp.UI
{
    public class ACATAppMainMenu : UserControl
    {
        public IDescriptor Descriptor => throw new NotImplementedException();

        public IPanelCommon PanelCommon => throw new NotImplementedException();

        public SyncLock SyncObj => throw new NotImplementedException();

        public ACATAppMainMenu(Font acatIconFont, Font acatFont1Font) : base()
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            var MainMenuButtons = new List<ScannerButtonControl>
                {
                    new ScannerButtonControl { Name = "ACatTalk", Text = "h", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerButtonControl { Name = "QuickTalk",Text = "i", Font =  new Font(acatIconFont.FontFamily, 44) },
                    new ScannerButtonControl { Name = "PointerControl", Text = "q", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerButtonControl { Name = "Keyboard", Text = "e", Font = new Font(acatFont1Font.FontFamily, 44) },
                    new ScannerButtonControl { Name = "System", Text = "M", Font = new Font(acatIconFont.FontFamily, 44) },
                    new ScannerButtonControl { Name = "Location", Text = "L", Font = new Font(acatIconFont.FontFamily, 44) },
                };

            var MainMenu = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Padding = new Padding(0, 0, 0, 0)
            };

            var MenuFont = new Font("ACAT ICON", 24, FontStyle.Regular);

            // Add the buttons to the toolbar
            foreach (var button in MainMenuButtons)
            {
                button.Dock = DockStyle.Fill;
                button.AutoSize = true;
                button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                button.BackColor = Color.Transparent;
                button.ForeColor = Color.White;
                MainMenu.Controls.Add(button);
            }

            this.Size = MainMenu.Size;
            this.Controls.Add(MainMenu);
        }

        public bool Initialize(StartupArg initArg)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnButtonActuated(Widget widget)
        {
            //throw new NotImplementedException();
        }

        public void OnPause()
        {
            throw new NotImplementedException();
        }

        public void OnResume()
        {
            throw new NotImplementedException();
        }

        public void OnRunCommand(string command, ref bool handled)
        {
            throw new NotImplementedException();
        }
    }
}
