////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardQwertyUserControl.cs
//
// User control for the QWERT layout of the alphabet keyboard
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Scanners.UserControls
{
    [DescriptorAttribute("C4668F6A-79D6-4D27-8C68-18172A49F333",
                    "KeyboardQwertyUserControl",
                    "User Control for Qwerty keyboard")]
    public partial class KeyboardQwertyUserControl : UserControl, IUserControl
    {
        private UserControlKeyboardCommon _keyboardCommon;

        public KeyboardQwertyUserControl()
        {
            InitializeComponent();
        }

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the snchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _keyboardCommon.SyncObj; }
        }

        public IUserControlCommon UserControlCommon
        {
            get
            {
                return _keyboardCommon;
            }
        }

        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            bool retVal = _keyboardCommon.Initialize();

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            return retVal;
        }

        public void OnLoad()
        {
            _keyboardCommon.OnLoad();
            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);

            if (CoreGlobals.LnRMode)
            {
                var widget = _keyboardCommon.RootWidget.Finder.FindChild("ButtonLnR");
                if (widget != null)
                {
                    widget.Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.LnRResponseButtonSchemeName);
                }
            }
        }

        public void OnPause()
        {
            _keyboardCommon.OnPause();
        }

        public void OnResume()
        {
            _keyboardCommon.OnResume();
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            handled = false;
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            if (EvtPlayerStateChanged != null)
            {
                EvtPlayerStateChanged(this, e);
            }
        }
    }
}