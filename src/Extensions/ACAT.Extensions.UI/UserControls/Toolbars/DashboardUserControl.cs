using ACAT.Core.Utility;
using System.Collections.Generic;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("3933BEAF-FAB3-4C6B-AB16-D0B07B0F2C6D",
        Name = "DashboardUserControl",
        Description = "User control for the ACAT Dashboard")]
    public class DashboardUserControl : LargeToolbarUserControl
    {
        public DashboardUserControl() : base("DashboardUserControl") { }

        protected override void InitializeButtonsList()
        {
            Buttons = new List<ButtonSpec>
            {
                new() { Name = "ACATTalk", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat-fill"), Visible = true },
                new() { Name = "QuickTalk", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat"), Visible = true },
                new() { Name = "PointerControl", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mouse2"), Visible = true },
                new() { Name = "Keyboard", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("keyboard"), Visible = true },
                new() { Name = "Windows", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("windows"), Visible = true },
                new() { Name = "Location", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrows-move"), Visible = true },
                //new() { Name = "MainMenu", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrows-move"), Visible = true },
            };
        }
    }
}
