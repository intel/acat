using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using System;
using System.Collections.Generic;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("BE615A9B-3B93-4EF5-9C67-A06C4D34070D",
        Name = "SupportedAppsUserControl",
        Description = "User control for the ACAT Dashboard")]
    public class LaunchAppUserControl : LargeToolbarUserControl
    {
        public IFunctionalAgent launchAgent => _launchAgent;

        protected IFunctionalAgent _launchAgent { get; set; }

        public LaunchAppUserControl() : base("LaunchAppUserControl")
        {
        }

        public override void OnButtonClicked(object s, EventArgs e)
        {
            string buttonName = e.ToString();
            Log.Info($"Button clicked: {buttonName}");

            var launchAgent = this._userControlCommon.AppAgentMgr.GetFunctionalAgentByName("LaunchAppAgent");

            launchAgent.Activate();

            //switch (buttonName)
            //{
            //    case "Chrome":
            //        Log.Info($"Button clicked: {buttonName}");
            //        break;
            //    case "Edge":
            //        break;
            //    case "Outlook":
            //        break;
            //    //case ButtonSpec button when button.Name == "RightClick":
            //    //    MouseUtils.ClickRightMouseButton(MouseMover.CursorX, MouseMover.CursorY);
            //    //    break;
            //    //case ButtonSpec button when button.Name == "ClickHold":
            //    //    MouseUtils.ClickHoldMouseButton(MouseMover.CursorX, MouseMover.CursorY);
            //    //    break;
            //    //case ButtonSpec button when button.Name == "ScrollUp":
            //    //    MouseUtils.ScrollUp(MouseMover.CursorX, MouseMover.CursorY);
            //    //    break;
            //    //case ButtonSpec button when button.Name == "ScrollDown":
            //    //    MouseUtils.ScrollDown(MouseMover.CursorX, MouseMover.CursorY);
            //    //    break;
            //}
        }

        protected override void InitializeButtonsList()
        {
            Buttons = new List<ButtonSpec>
            {
                new() { Name = "Chrome", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("browser-chrome"), Visible = true },
                new() { Name = "Edge", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("browser-edge"), Visible = true },
                new() { Name = "Outlook", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mailbox"), Visible = true },
            };
        }
    }
}
