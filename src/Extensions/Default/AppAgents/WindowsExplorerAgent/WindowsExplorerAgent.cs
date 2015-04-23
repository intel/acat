#region Author Information
// Author: Sai Prasad
// Group : XTL, IXR
#endregion

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.ScreenManagement;
using ACAT.Lib.Core.AgentManagement;
#endregion

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.WindowsExplorer
{
    /// <summary>
    /// Interop class to control the output window to which the
    /// text from Aster is routed. 
    /// For now, we support one of two applications:
    /// Notepad or EZKeys
    /// </summary>
    /// 
    [DescriptorAttribute("5E616FC6-83BE-4644-BF7A-5534E45DC499", "Windows Explorer Agent", "Agent for Windows Explorer")]
    class WindowsExplorerAgent : GenericAppAgentBase
    {
        const string WindowsExplorerNavigateContextMenu = "WindowsExplorerNavigateContextMenu";
        const string WindowsExplorerContextMenu = "WindowsExplorerContextMenu";

        public WindowsExplorerAgent()
        {
            Name = "Windows Explorer Agent";
        }

        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get
            {
                return new AgentProcessInfo[] { new AgentProcessInfo("explorer") };
            }
        }

        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug("OnFocus: " + monitorInfo.ToString());

            base.OnFocusChanged(monitorInfo, ref handled);

            if (monitorInfo.IsNewWindow)
            {
                showPanelOnFocusChanged(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                handled = true;
            }

#if abc
            String panel = PanelClasses.None;
            handled = getPanel(monitorInfo, false, ref panel);

            if (handled)
            {
                showPanelOnFocusChanged(this, new PanelRequestEventArgs(panel, "Explorer", monitorInfo));
            }
#endif
        }

        public override void OnPanelClosed(String panelClass, WindowActivityMonitorInfo monitorInfo)
        {

#if AUTO_SWITCH_SCANNER
            if (PanelConfigMap.AreEqual(panelClass, WindowsExplorerContextMenu))
            {
                return;
            }
            AutomationElement windowElement = getWindowElement(monitorInfo.FgHwnd);
            if (isStartMenu(windowElement) && PanelConfigMap.AreEqual(panelClass, "DialogAutoScanContextMenu"))//PanelClasses.DialogDockContextMenu))
            {
                AgentManager.Instance.Keyboard.Send(Keys.Escape);
            }
            else if (isFileExplorer(windowElement))// && PanelConfigMap.AreEqual(panelClass, "WindowsExplorerDialogContextMenu"))
            {
                AgentManager.Instance.Keyboard.Send(Keys.F6);
            }
#endif
        }

        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
#if AUTO_SWITCH_SCANNER
            String panelClass = PanelClasses.None;
            if (!getPanel(monitorInfo, true, ref panelClass))
            {
                if (isDesktopWindow(monitorInfo.FocusedElement))
                {
                    showPanel(this, new PanelRequestEventArgs("WindowsDesktopContextMenu", "Desktop", monitorInfo));
                    return;
                }
            }
            showPanel(this, new PanelRequestEventArgs(panelClass, "Explorer", monitorInfo));
#endif
        }

        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                case "WindowsExplorerNavigate":
                    showWindowsExplorerScanner(WindowsExplorerNavigateContextMenu);
                    break;

                case "WindowsExplorerBack":
                    AgentManager.Instance.Keyboard.Send(Keys.Back);
                    break;

                case "WindowsExplorerForward":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Right);
                    break;

                case "WindowsExplorerUp":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Up);
                    break;

                case "WindowsExplorerAddressBar":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.D);
                    showWindowsExplorerScanner(WindowsExplorerNavigateContextMenu);
                    break;

                case "WindowsExplorerRecentLocations":
                    AgentManager.Instance.Keyboard.Send(Keys.F4);
                    showWindowsExplorerScanner(WindowsExplorerNavigateContextMenu);
                    break;

                case "WindowsExplorerNewWindow":
                    AgentManager.Instance.Keyboard.Send(Keys.LWin, Keys.E);
                    break;

                case "WindowsExplorerNewFolder":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.N);
                    break;

                case "WindowPosSizeContextMenu":
                    showWindowsExplorerScanner("WindowPosSizeContextMenu");
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }

        }
        

        bool isDesktopWindow(AutomationElement focusedElement)
        {
            Log.Debug();
            TreeWalker walker = TreeWalker.ControlViewWalker;

            AutomationElement parent = focusedElement;

            while (parent != null)
            {
                Log.Debug("class: " + parent.Current.ClassName + ", ctrltype: " + parent.Current.ControlType.ProgrammaticName + ", name: " + parent.Current.Name);
                if (parent == AutomationElement.RootElement || (String.Compare(parent.Current.ClassName, "SysListView32", true) == 0 && 
                    String.Compare(parent.Current.ControlType.ProgrammaticName, "ControlType.List", true) == 0 && 
                    String.Compare(parent.Current.AutomationId, "1", true) == 0 &&
                    String.Compare(parent.Current.Name, "FolderView", true) == 0) || 
                    (String.Compare(parent.Current.ClassName, "Shell_TrayWnd", true) == 0 && 
                    String.Compare(parent.Current.ControlType.ProgrammaticName, "ControlType.Pane", true) == 0))
                {
                    Log.Debug("Returning true");
                    return true;
                }
                parent = walker.GetParent(parent);
            }
            Log.Debug("Returning false");
            return false;
        }

        bool isStartMenu(AutomationElement windowElement)
        {
            return (windowElement != null) ? (String.Compare(windowElement.Current.ClassName, "DV2ControlHost") == 0 && String.Compare(windowElement.Current.Name, "Start menu", true) == 0) : false;
        }

        bool isFileExplorer(AutomationElement windowElement)
        {
            return (windowElement != null) ? (String.Compare(windowElement.Current.ClassName, "CabinetWClass") == 0 && String.Compare(windowElement.Current.ControlType.ProgrammaticName, "ControlType.Window") == 0) : false;
        }

        AutomationElement getWindowElement(IntPtr hWndMain)
        {
            AutomationElement retVal = null;
            try
            {
                retVal = AutomationElement.FromHandle(hWndMain);
            }
            catch 
            {
                retVal = null;
            }
            return retVal;
        }

        bool getPanel(WindowActivityMonitorInfo monitorInfo, bool getContextMenu, ref String panelClass)
        {
            Log.Debug();
            AutomationElement windowElement = getWindowElement(monitorInfo.FgHwnd);
            bool retVal = true;
            if (windowElement != null)
            {
                if (isStartMenu(windowElement))
                {
                    panelClass = "DialogAutoScanContextMenu";//PanelClasses.DialogDockContextMenu;
                }
                else if (isFileExplorer(windowElement))
                {
                    if (Windows.IsMinimized(monitorInfo.FgHwnd))
                    {
                        Log.Debug("Windows explorer is minimized. Trigger alphabet scanner");
                        panelClass = PanelClasses.Alphabet;
                    }
                    else if (isRenameFileOrFolder(monitorInfo))
                    {
                        panelClass = PanelClasses.Alphabet;
                    }
                    else
                    {
                        panelClass = WindowsExplorerContextMenu;
                    }
                }
                else if (!isDesktopWindow(windowElement))
                {
                    retVal = false;
                }
            }
            return retVal;
        }

        bool isRenameFileOrFolder(WindowActivityMonitorInfo monitorInfo)
        {
            AutomationElement element = monitorInfo.FocusedElement;
            if (element.Current.ClassName == "UIRenameTextElement")
            {
                return true;
            }
            return false;
        }

        bool isAddressBar(WindowActivityMonitorInfo monitorInfo)
        {
            AutomationElement element = monitorInfo.FocusedElement;

            return (element.Current.AutomationId == "41477");
        }

        void showWindowsExplorerScanner(String panel)
        {
            WindowActivityMonitorInfo monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
            PanelRequestEventArgs panelArg = new PanelRequestEventArgs(panel, monitorInfo);
            panelArg.Title = "Explorer";
            panelArg.UseCurrentScreenAsParent = true;
            showPanel(this, panelArg);
        }

#if !AUTO_SWITCH_SCANNER
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            arg.Handled = true;
            switch (arg.Widget.SubClass)
            {
                case "FileBrowserToggle":
                    arg.Enabled = true;
                    return;

                case "ContextualMenu":
                    arg.Enabled = false;
                    return;

                default:
                    arg.Handled = false;
                    break;
            }
        }
#endif

    }
}
