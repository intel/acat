using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ACATCore.ScreenManagement;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Permissions;
using ACATCore.Utility;
using ACATCore.AgentManagement;
using System.Reflection;
using System.Windows.Automation;

namespace ACATCore.Extensions.Base.UI.ContextMenus
{
    [DescriptorAttribute("A1B4D6F5-7F48-4A06-A36C-5239CA94C5BA", "TaskManagerContextMenu", "Task Manager Context Menu")]
    public partial class TaskManagerContextMenu: ACATCore.ScreenManagement.AutoScanContextMenu
    {
        public TaskManagerContextMenu(String panelClass, String panelTitle)
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

                case "@Space":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Space);
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

                case "@Escape":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Escape);
                    break;

                case "@Right":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Right);
                    break;

                case "@Select":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Enter);
                    break;

                default:
                    //base.OnRunCommand(command, ref handled);
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        protected override void onAutoScanTick()
        {
            Context.AppAgentMgr.Keyboard.Send(Keys.Tab);
        }
    }
}
