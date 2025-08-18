using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ACAT.Core.Utility
{
    public static class HotkeyManager
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 0x1000; // arbitrary ID
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;

        public static void Initialize(Form form)
        {
            form.HandleDestroyed += (s, e) => UnregisterHotKey(form.Handle, HOTKEY_ID);
            RegisterHotKey(form.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (uint)Keys.T);

            form.KeyPreview = true;
            form.KeyDown += (s, e) =>
            {
                if (e.Control && e.Shift && e.KeyCode == Keys.T)
                {
                    ToggleAlwaysOnTop();
                    e.Handled = true;
                }
            };

            NativeMessageFilter.AddHandler(form, HOTKEY_ID, ToggleAlwaysOnTop);
        }

        private static void ToggleAlwaysOnTop()
        {
            TopMostManager.Enabled = !TopMostManager.Enabled;
            string status = TopMostManager.Enabled ? "enabled" : "disabled";

            using (NotifyIcon notifyIcon = new NotifyIcon())
            {
                notifyIcon.Visible = true;
                notifyIcon.Icon = SystemIcons.Application;
                notifyIcon.BalloonTipTitle = "AlwaysOnTopManager";
                notifyIcon.BalloonTipText = $"AlwaysOnTopManager {status}";
                notifyIcon.ShowBalloonTip(1000); // Show for 1 second
            }
        }

        private static class NativeMessageFilter
        {
            private const int WM_HOTKEY = 0x0312;

            public static void AddHandler(Form form, int hotkeyId, Action callback)
            {
                var messageHandler = new MessageWindow(hotkeyId, callback);
                Application.AddMessageFilter(messageHandler);
            }

            private class MessageWindow : IMessageFilter
            {
                private readonly int _hotkeyId;
                private readonly Action _callback;

                public MessageWindow(int hotkeyId, Action callback)
                {
                    _hotkeyId = hotkeyId;
                    _callback = callback;
                }

                public bool PreFilterMessage(ref Message m)
                {
                    if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == _hotkeyId)
                    {
                        _callback();
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}
