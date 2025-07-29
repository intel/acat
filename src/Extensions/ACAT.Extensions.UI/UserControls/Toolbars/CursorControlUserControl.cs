using ACAT.Core.Utility;
using System;
using System.Collections.Generic;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("72061314-CE5E-4DB8-92C8-C0F81E5CB3EE",
        Name = "CursorControlUserControl",
        Description = "User control for the ACAT Dashboard")]
    public class CursorControlUserControl : LargeToolbarUserControl
    {
        public CursorControlUserControl() : base("CursorControlUserControl") { }

        protected override void InitializeButtonsList()
        {
            Buttons = new List<ButtonSpec>
            {
                new() { Name = "MoveAndClick", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("reply-all"), Visible = true },
                new() { Name = "Move", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("reply"), Visible = true },
                new() { Name = "LeftClick", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mouse2"), Visible = true },
                new() { Name = "RightClick", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("menu-app-fill"), Visible = true },
                new() { Name = "ClickHold", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mouse2-fill"), Visible = true },
                new() { Name = "ScrollUp", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrow-up-square"), Visible = true },
                new() { Name = "ScrollDown", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrow-down-square"), Visible = true },
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // create the cursor controller

        }
    }
}
