using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ACAT.Lib.Core.ScreenManagement;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Permissions;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.AgentManagement;
using System.Reflection;
using System.Windows.Automation;

namespace ACAT.Lib.Core.Extensions.Base.UI.ContextMenus
{
    [DescriptorAttribute("715C231D-0325-4CF2-BB13-3470DFC84FEE", "WindowsExplorerNavigateContextMenu ", "Windows Explorer Navigate Context Menu")]
    public partial class WindowsExplorerNavigateContextMenu : ACAT.Lib.Core.ScreenManagement.AutoScanContextMenu
    {
        public WindowsExplorerNavigateContextMenu(String panelClass, String panelTitle)
            : base(panelClass, panelTitle)
        {
        }

        public  void OnRunCommand(string command, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "@Tab":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Tab);
                    break;

                case "@TabScan":
                    startAutoScan();
                    break;

                case "@Down":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Down);
                    break;

                case "@Up":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Up);
                    break;

                case "@Left":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Left);
                    break;

                case "@Right":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Right);
                    break;

                case "@Select":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Enter);
                    break;

                case "@Escape":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Escape);
                    break;

                case "@CmdRightClick":
                    Context.AppAgentMgr.Keyboard.Send(Keys.LShiftKey, Keys.F10);
                    break;

                case "@Back":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Back);
                    break;

                case "@Text":
                    Invoke(new MethodInvoker(delegate()
                    {
                        StartupArg startupArg = new StartupArg(PanelClasses.Alphabet);
                        startupArg.DialogMode = true;
                        startupArg.FocusedElement = AutomationElement.FocusedElement;
                        IPanel panel = Context.AppScreenManager.CreatePanel(PanelClasses.Alphabet, String.Empty, startupArg) as IPanel;
                        Context.AppScreenManager.Show(this, panel);
                    }));
                    break;

                default:
                    //base.OnRunCommand(command, ref handled);
                    break;
            }
        }

        protected override void onAutoScanTick()
        {
            Context.AppAgentMgr.Keyboard.Send(Keys.Tab);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        public override bool OnQueryPanelChange(PanelRequestEventArgs arg)
        {
            Log.Debug("Query change for " + arg.PanelClass);
            if (arg.MonitorInfo.FgHwnd == startupArg.HWnd)// && panelClass == PanelClasses.WindowsExplorerContextMenu)
            {
                if (PanelConfigMap.AreEqual(arg.PanelClass, "MenuAutoScanContextMenu"))
                    return true;
                Log.Debug("Refuse Query change for " + arg.PanelClass);
                return false;
            }

            return true;
        }
    }
}
