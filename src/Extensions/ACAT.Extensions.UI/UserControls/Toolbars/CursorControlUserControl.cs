using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("682B5CE4-A17F-42E2-B620-279A3ACF64C3",
        Name = "CursorControlUserControl",
        Description = "User control for the ACAT Dashboard")]
    public class CursorControlUserControl : LargeToolbarUserControl
    {
        protected override void InitializeButtonsList()
        {
            Dictionary<string, string> CursorControlButtons = new Dictionary<string, string>
            {
                { "MoveAndClick", BootstrapFontUtility.GetBootstrapFontCharacter("reply-all") },
                { "Move", BootstrapFontUtility.GetBootstrapFontCharacter("reply") },
                { "LeftClick", BootstrapFontUtility.GetBootstrapFontCharacter("mouse2") },
                { "RightClick", BootstrapFontUtility.GetBootstrapFontCharacter("menu-app-fill") },
                { "ClickHold", BootstrapFontUtility.GetBootstrapFontCharacter("mouse2-fill") },
                { "ScrollUp", BootstrapFontUtility.GetBootstrapFontCharacter("arrow-up-square") },
                { "ScrollDown", BootstrapFontUtility.GetBootstrapFontCharacter("arrow-down-square") },
            };
        }
    }
}
