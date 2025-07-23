////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MenuUserControlBCI.cs
//
// User control for the menu to exit the app among other options.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.UI.UserControls
{
    [ClassDescriptorAttribute("4F0E7278-1495-4AF6-B609-E91A2421FCB0",
        "KeyboardControl",
        "User Control keyboard Modes BCI")]
    public partial class MenuUserControlBCI : UserControl, IUserControl
    {
        private static String _formConfigFilePath = "";
        private static UserControlConfigMapEntry _mapEntry;
        private UserControlKeyboardCommon _keyboardCommon;
        private IScannerPanel _scanner;

        public MenuUserControlBCI()
        {
            InitializeComponent();
        }

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public ClassDescriptorAttribute Descriptor
        {
            get { return ClassDescriptorAttribute.GetDescriptor(GetType()); }
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

        public static string getpathConfigFile()
        {
            try
            {
                if (_mapEntry != null)
                    _formConfigFilePath = _mapEntry.ConfigFileName;
            }
            catch (Exception)
            {
            }
            return _formConfigFilePath;
        }

        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _mapEntry = mapEntry;

            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            _scanner = scanner;

            bool retVal = _keyboardCommon.Initialize();

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;
            //_keyboardCommon.RootWidget.Finder.FindAllChildren(typeof(WinControlWidget), _listButtonsWidgets);//TO BE UPDATED WHEN KEYBOARD CHANGE
            return retVal;
        }

        public void OnLoad()
        {
            _keyboardCommon.OnLoad();

            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public void OnPause()
        {
        }

        public void OnResume()
        {
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            //_wordPredictionCommon.OnWidgetActuated(e, ref handled);
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
        }
    }
}