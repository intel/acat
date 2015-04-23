////////////////////////////////////////////////////////////////////////////
// <copyright file="VolumeSettingsScanner.cs" company="Intel Corporation">
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
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;
using ACAT.Lib.Extension.CommandHandlers;

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

namespace ACAT.Extensions.Hawking.FunctionalAgents.VolumeSettings
{
    /// <summary>
    /// Form that enables the user to set the volume
    /// of text to speech.  Volume is normalized from 0 to 9 as
    /// different TTS engines have different ranges.  The value
    /// is then scaled by the TTS engine before setting it.
    /// </summary>
    [DescriptorAttribute("48694D06-DF49-4175-ADAC-E4533EB29E17", "VolumeSettingsScanner", "Volume Settings Scanner")]
    public partial class VolumeSettingsScanner : Form, IScannerPanel, ISupportsStatusBar
    {
        /// <summary>
        /// Command dispatcher object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly SyncLock _syncObj;

        /// <summary>
        /// Initial value of the volume before setting
        /// was changed
        /// </summary>
        private int _initialSetting;

        /// <summary>
        /// Have any of the settings changed?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// The ScannerCommon object
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// Widget that holds the title of the scanner
        /// </summary>
        private Widget _titleWidget;

        /// <summary>
        /// The volume settings selected by the user
        /// </summary>
        private int _volumeSelected = -1;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public VolumeSettingsScanner()
        {
            InitializeComponent();

            _syncObj = new SyncLock();

            Load += VolumeSettingsScanner_Load;
            FormClosing += VolumeSettingsScanner_FormClosing;

            PanelClass = "VolumeSettingsScanner";

            _dispatcher = new Dispatcher(this);
        }

        /// <summary>
        /// Gets the command dispatcher object used to
        /// run commands
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets this form
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class of this form
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets the ScannerCommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// We don't have a status bar
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncObj; }
        }

        /// <summary>
        /// Gets the text controlleer object
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Sets form styles
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                return Windows.SetFormStyles(base.CreateParams);
            }
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        /// <returns>true on success</returns>
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            switch (arg.Widget.SubClass)
            {
                default:
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Initializes the form
        /// </summary>
        /// <param name="startupArg">Startup arg</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _scannerCommon = new ScannerCommon(this);

            if (!_scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animation and hides the form
        /// </summary>
        public void OnPause()
        {
            Log.Debug();

            _scannerCommon.GetAnimationManager().Pause();

            _scannerCommon.HideScanner();

            _scannerCommon.OnPause();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="eventArg"></param>
        /// <returns></returns>
        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        /// <summary>
        /// Resumes animation and shows the form
        /// </summary>
        public void OnResume()
        {
            Log.Debug();

            _scannerCommon.GetAnimationManager().Resume();

            _scannerCommon.ShowScanner();

            _scannerCommon.OnResume();
        }

        /// <summary>
        /// User actuated a widget. Act on it i.e, set
        /// the volume based on the level selected
        /// </summary>
        /// <param name="widget">widget activated</param>
        /// <param name="handled">was this handled</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            var value = widget.Value;
            handled = true;
            if (Char.IsDigit(value[0]))
            {
                int volume = Convert.ToInt32(value);
                _isDirty = true;
                _volumeSelected = volume;
                setVolumeAndUpdateTitle(_volumeSelected);
            }
            else
            {
                handled = false;
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="widget"></param>
        public void SetTargetControl(Form parent, Widget widget)
        {
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _scannerCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Set the title of the form with the
        /// volume level
        /// </summary>
        /// <param name="setting">volume setting</param>
        private void setTitle(int setting = -1)
        {
            if (_titleWidget == null)
            {
                return;
            }

            if (Context.AppTTSManager.ActiveEngine.IsMuted())
            {
                setting = 0;
            }

            _titleWidget.SetText("Volume (" + ((setting < 0) ?
                                    Context.AppTTSManager.GetNormalizedVolume().Value :
                                    setting) + ")");
        }

        /// <summary>
        /// Sets the volume in the TTS engine and updates
        /// the level setting in the title bar
        /// </summary>
        /// <param name="volume">volume level</param>
        private void setVolumeAndUpdateTitle(int volume)
        {
            Context.AppTTSManager.SetNormalizedVolume(volume);
            setTitle(volume);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void VolumeSettingsScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Form is loaded. Initialize it.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void VolumeSettingsScanner_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();

            _titleWidget = _scannerCommon.GetRootWidget().Finder.FindChild("Title");

            setTitle();

            ITTSValue<int> volume = Context.AppTTSManager.ActiveEngine.GetVolume();
            _initialSetting = volume.Value;
            _volumeSelected = Context.AppTTSManager.GetNormalizedVolume().Value;

            _scannerCommon.GetAnimationManager().Start(_scannerCommon.GetRootWidget());
        }

        /// <summary>
        /// Handles the "close" command to exit. Confirms with
        /// the user whether to set the volume, does what the
        /// user orders and then exits.
        /// </summary>
        private class CloseHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a ne instance of the class
            /// </summary>
            /// <param name="cmd">the command to execute</param>
            public CloseHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the specified command
            /// </summary>
            /// <param name="handled">true if it was handled</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                var form = Dispatcher.Scanner.Form as VolumeSettingsScanner;

                if (form._isDirty)
                {
                    String prompt;

                    if (form._volumeSelected == 0)
                    {
                        prompt = "Mute Speaker?";
                    }
                    else
                    {
                        prompt = "Set volume to " + form._volumeSelected + "?";
                    }

                    if (DialogUtils.ConfirmScanner(prompt))
                    {
                        Context.AppTTSManager.SetNormalizedVolume(form._volumeSelected);
                        Context.AppTTSManager.ActiveEngine.Save();
                    }
                    else
                    {
                        Context.AppTTSManager.ActiveEngine.SetVolume(form._initialSetting);
                    }
                }

                Windows.CloseForm(form);
                return true;
            }
        }

        /// <summary>
        /// Command handler to test the current volume level.
        /// Speaks a line of text.
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            /// <param name="cmd">The command to execute</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">true if it was handled</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                switch (Command)
                {
                    case "VolumeTest":
                        Context.AppTTSManager.ActiveEngine.Speak("Test");
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }
        }

        /// <summary>
        /// Command dispatcher to run commands
        /// </summary>
        private class Dispatcher : DefaultCommandDispatcher
        {
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
                Commands.Add(new CommandHandler("VolumeTest"));
                Commands.Add(new CloseHandler("CmdGoBack"));
            }
        }
    }
}