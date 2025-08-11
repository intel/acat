using ACAT.Core.AgentManagement;
using ACAT.Core.Extensions;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension;
using ACATResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("BE615A9B-3B93-4EF5-9C67-A06C4D34070D",
        Name = "LaunchAppUserControl",
        Description = "User control for the ACAT Dashboard")]
    [DesignerCategory("code")]
    public class LaunchAppUserControl : LargeToolbarUserControl
    {
        private class LaunchButtonSpec : ButtonSpec
        {
            public AppInfo AppInfo { get; set; }
        }

        /// <summary>
        /// Used to invoke methods/properties of this class
        /// </summary>
        //public  ExtensionInvoker GetInvoker => new ExtensionInvoker(this);

        //private LaunchAppAgent launchAppAgent;

        /// <summary>
        /// List of apps that match the filter specified by the user
        /// </summary>
        private List<AppInfo> _appsList;


        public LaunchAppUserControl() : base("LaunchAppUserControl")
        {
        }

        /// <summary>
        /// For the event raised to launch an app
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="appInfo">info about the app</param>
        public delegate void LaunchAppDelegate(object sender, AppInfo appInfo);

        /// <summary>
        /// Raised when the user wants to launch the app
        /// </summary>
        public event LaunchAppDelegate EvtLaunchApp;

        public override void OnButtonClicked(object s, EventArgs e)
        {
            string buttonName = e.ToString();
            Log.Info($"Button clicked: {buttonName}");

            var dict = Buttons.ToDictionary(x => x.Name);

            LaunchButtonSpec button = dict[buttonName] as LaunchButtonSpec;

            if (button != null)
            {
                handleAppSelect(button.AppInfo);
            }


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

        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            var widget = e.SourceWidget as Widget;
            handleWidgetSelection(widget, ref handled);
            highlightOff();
        }

        private void handleWidgetSelection(Widget widget, ref bool handled)
        {
            if (widget.UserData is AppInfo)
            {
                handleAppSelect((AppInfo)widget.UserData);
                handled = true;
            }
            else
            {
                handled = true;
                switch (widget.Value)
                {
                    //case "@AppListSort":
                    //    switchSortOrder();
                    //    break;

                    //case "@CmdNextPage":
                    //    gotoNextPage();
                    //    break;

                    //case "@CmdPrevPage":
                    //    gotoPreviousPage();
                    //    break;

                    //case "@AppListSearch":
                    //    if (EvtShowScanner != null)
                    //    {
                    //        EvtShowScanner.BeginInvoke(null, null, null, null);
                    //    }
                    //    break;

                    //case "@AppListClearFilter":
                    //    ClearFilter();
                    //    break;

                    default:
                        handled = false;
                        break;
                }
            }
        }


        /// <summary>
        /// Confirms if the user wants to launch the selected
        /// app. If so, triggers an event to indicate that the user
        /// wants to launch the app.
        /// </summary>
        /// <param name="appInfo"></param>
        private void handleAppSelect(AppInfo appInfo)
        {
            if (EvtLaunchApp != null)
            {
                EvtLaunchApp.BeginInvoke(this, appInfo, null, null);
            }
        }

        /// <summary>
        /// Turn highlight off for all widgets
        /// </summary>
        private void highlightOff()
        {
            _userControlCommon.RootWidget.HighlightOff();
        }


        protected override void InitializeButtonsList()
        {
            Buttons = new List<ButtonSpec>
            {
                new LaunchButtonSpec()
                {
                    Name = "Chrome",
                    Icon = BootstrapFontUtility.GetBootstrapFontCharacter("browser-chrome"),
                    Visible = true,
                    AppInfo = new AppInfo()
                    {
                        Name = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
                    }
                },
                new LaunchButtonSpec()
                {
                    Name = "Edge",
                    Icon = BootstrapFontUtility.GetBootstrapFontCharacter("browser-edge"),
                    Visible = true,
                    AppInfo = new AppInfo()
                    {
                        Name = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                        Arguments = "--profile-directory=Default"
                    }
                },
                new() { Name = "Outlook", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mailbox"), Visible = true },
            };
        }
    }
}
