////////////////////////////////////////////////////////////////////////////
// <copyright file="Glass.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents a transluscent glass that covers the
    /// entire screen.  Can be used as a backdrop to a
    /// foreground window so the ACAT scanners and the
    /// talk window stand out.
    /// </summary>
    public partial class Glass : Form
    {
        /// <summary>
        /// Thread used to fade-in the glass when displayed
        /// </summary>
        private static Thread _fadeInThread;

        /// <summary>
        /// There can be only one glass instance active
        /// </summary>
        private static Glass _glass;

        /// <summary>
        /// Quit the threads?
        /// </summary>
        private static volatile bool _quit;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Glass()
        {
            InitializeComponent();

            HideTaskBar = false;

            // hide the glass, make it completely transparent
            Opacity = 0.0;
            Windows.SetVisible(this, false);
            ShowInTaskbar = false;
            FormClosing += Glass_FormClosing;
            Load += Glass_Load;
            CoreGlobals.AppPreferences.EvtPreferencesChanged += AppPreferences_EvtPreferencesChanged;
        }

        /// <summary>
        /// Raised when the glass is made visbile/invisible
        /// </summary>
        public static event EventHandler EvtShowGlass;

        /// <summary>
        /// Gets or sets whether glass should be used or not.
        /// </summary>
        public static bool Enable { get; set; }

        /// <summary>
        /// Gets or sets whether the windows task bar also be hidden?
        /// </summary>
        public static bool HideTaskBar { get; set; }

        /// <summary>
        /// Prevent from showing in alt-tab
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        /// <summary>
        /// Tell windows not to set focus to this form when
        /// user clicks on it
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a glass object if one is not already created.
        /// Doesn't show it though
        /// </summary>
        /// <returns>true</returns>
        public static bool CreateGlass()
        {
            if (!Enable)
            {
                return false;
            }

            if (_glass == null)
            {
                _glass = new Glass { TopMost = true, ShowInTaskbar = false };
            }

            return true;
        }

        /// <summary>
        /// Hides the glass.
        /// </summary>
        public static void HideGlass()
        {
            try
            {
                Windows.ShowTaskbar();

                if (_glass == null)
                {
                    return;
                }

                waitForThread();

                _quit = true;

                Windows.CloseForm(_glass);
                _glass = null;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Indicates whether the glass is currently visible or not
        /// </summary>
        /// <returns>true if visible</returns>
        public static bool IsVisible()
        {
            bool retVal = false;

            if (_glass != null)
            {
                retVal = Windows.GetVisible(_glass);
            }

            return retVal;
        }

        /// <summary>
        /// Shows the glass if it is not already visible. Glass should
        /// be enabled for it to show
        /// </summary>
        public static void ShowGlass()
        {
            Log.Debug();
            if (!Enable)
            {
                return;
            }

            _quit = false;

            float opacity = CoreGlobals.AppPreferences.GlassOpacity;

            if (opacity < 0.0f)
            {
                opacity = 0.0f;
            }
            else if (opacity > 1.0f)
            {
                opacity = 1.0f;
            }

            if (opacity == 0.0f)
            {
                return;
            }

            try
            {
                if (_glass == null)
                {
                    CreateGlass();
                }
                else
                {
                    if (Windows.GetVisible(_glass))
                    {
                        return;
                    }
                }

                if (HideTaskBar)
                {
                    Windows.HideTaskbar();
                }

                _glass.TopMost = true;

                Windows.SetText(_glass, String.Empty);
                Windows.SetOpacity(_glass, CoreGlobals.AppPreferences.GlassOpacity);
                Windows.SetWindowPosition(_glass, new IntPtr(-2), Windows.WindowPosition.TopLeft);
                Windows.SetTopMost(_glass);

                if (EvtShowGlass != null)
                {
                    EvtShowGlass(_glass, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            if (CoreGlobals.AppPreferences.GlassFadeIn)
            {
                fadeIn();
            }
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEACTIVATE = 0x0021;
            const int MA_NOACTIVATE = 0x0003;

            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATE;
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Gradually fade in the glass when displaying.  It
        /// is done asynchronously
        /// </summary>
        private static void fadeIn()
        {
            if (HideTaskBar)
            {
                Windows.HideTaskbar();
            }

            Windows.SetOpacity(_glass, 0.0);
            Windows.SetVisible(_glass, true);

            _fadeInThread = new Thread(fadeInProc);
            _fadeInThread.Start();
        }

        /// <summary>
        /// Thread proc that fades in the glass
        /// </summary>
        private static void fadeInProc()
        {
            try
            {
                while (true)
                {
                    if (_quit || _glass == null)
                    {
                        break;
                    }

                    double opacity = Windows.GetOpacity(_glass);
                    opacity += 0.05;
                    Windows.SetOpacity(_glass, opacity);
                    if (opacity >= CoreGlobals.AppPreferences.GlassOpacity)
                    {
                        return;
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Waits for the fade in thread to complete
        /// </summary>
        private static void waitForThread()
        {
            try
            {
                if (_fadeInThread != null)
                {
                    _fadeInThread.Join(100);
                    _fadeInThread = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Preferences have changed. Respond appropriately
        /// </summary>
        private void AppPreferences_EvtPreferencesChanged()
        {
            HideTaskBar = CoreGlobals.AppPreferences.HideWindowsTaskBar;
            Enable = CoreGlobals.AppPreferences.EnableGlass;
        }

        /// <summary>
        /// Close event handler
        /// </summary>
        private void Glass_FormClosing(object sender, FormClosingEventArgs e)
        {
            CoreGlobals.AppPreferences.EvtPreferencesChanged -= AppPreferences_EvtPreferencesChanged;

            Windows.ShowTaskbar();
        }

        /// <summary>
        /// Form load handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void Glass_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
        }
    }
}