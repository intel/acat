using ACAT.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ACAT.Extensions.UI.UserControls.Toolbars
{
    [ClassDescriptor("3933BEAF-FAB3-4C6B-AB16-D0B07B0F2C6D",
        Name = "ToolsMenuUserControl",
        Description = "User control for the ACAT Dashboard")]
    [DesignerCategory("code")]

    public class ToolsMenuUserControl : LargeToolbarUserControl
    {
        public ToolsMenuUserControl() : base("ToolsMenuUserControl") { }

        public override void OnButtonClicked(object s, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void InitializeButtonsList()
        {
            Buttons = new List<ButtonSpec>
            {
                new() { Name = "ACATTalkSentence", LabelText = "Talk", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat-fill"), Visible = true },
                new() { Name = "ACATTalkPhrase", LabelText = "Phrase", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat-left-quote"), Visible = true },
                new() { Name = "ACATTalkShorthand", LabelText = "Shorthand", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("lightning"), Visible = true },
                new() { Name = "SwitchWindow", LabelText = "Switch", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("window-stack"), Visible = true },
                new() { Name = "LaunchApp", LabelText = "Launch", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("grid-fill"), Visible = true },
                new() { Name = "Location", LabelText = "Location", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrows-move"), Visible = true },
            };
        }
    }
}
