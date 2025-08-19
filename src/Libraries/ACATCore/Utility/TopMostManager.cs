using ACAT.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Core.Utility
{
    public static class TopMostManager
    {
        private static bool _enabled =
    #if !DEBUG
            false;
    #else
            true;
    #endif

        private static readonly List<Form> _forms = new List<Form>();
        private static readonly object _lock = new object();

        public static bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                ApplyToAllForms();
            }
        }

        public static void Register(Form form)
        {
            if (form == null) return;

            lock (_lock)
            {
                if (_forms.Contains(form)) return;

                _forms.Add(form);

                ApplyTopMostIfNeeded(form);

                form.Disposed += (s, e) => Unregister(form);

                // Event-driven focus restoration
                form.Activated += (s, e) =>
                {
                    if (_enabled)
                        ApplyTopMostIfNeeded(form);
                };
                form.Deactivate += (s, e) =>
                {
                    if (_enabled)
                        SoftRestoreFocusIfNeeded(form);
                };
            }
        }

        public static void Unregister(Form form)
        {
            if (form == null) return;

            lock (_lock)
            {
                _forms.Remove(form);
            }
        }

        private static void ApplyTopMostIfNeeded(Form form)
        {
            if (!_enabled || form == null || form.IsDisposed) return;

            if (form.InvokeRequired)
            {
                form.BeginInvoke(new Action(() => ApplyTopMostIfNeeded(form)));
                return;
            }

            if (!form.TopMost)
            {
                form.TopMost = false;
                form.TopMost = true;
            }
        }

        private static void SoftRestoreFocusIfNeeded(Form form)
        {
            if (form == null || form.IsDisposed) return;

            if (form.InvokeRequired)
            {
                form.BeginInvoke(new Action(() =>  SoftRestoreFocusIfNeeded(form)));
            }

            if (!form.Visible || form.WindowState == FormWindowState.Minimized)
                return;

            if (Form.ActiveForm != form)
            {
                // Only restore focus if form is fully on-screen
                Rectangle formBounds = form.Bounds;
                Rectangle screenBounds = Screen.FromControl(form).Bounds;

                if (screenBounds.Contains(formBounds))
                {
                    Windows.SetForegroundWindow(form.Handle);
                    form.Activate();
                }
            }
        }

        private static void ApplyToAllForms()
        {
            lock (_lock)
            {
                foreach (var form in _forms.ToList())
                {
                    if (form.IsDisposed)
                    {
                        _forms.Remove(form);
                        continue;
                    }

                    ApplyTopMostIfNeeded(form);
                    SoftRestoreFocusIfNeeded(form);
                }
            }
        }
    }

}
