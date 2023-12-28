////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Core.UserControlManagement
{
    /// <summary>
    /// Used by scanners to manage the usercontrols they host.
    /// Has functions to handle top level iterations between
    /// usercontrols, stack usercontrols in a container etc.
    /// </summary>
    public class UserControlManager
    {
        private int _iterationCount = 0;
        private int _iterations = 1;
        private volatile bool _playerTransitioned = false;

        private IScannerPanel _scannerPanel;
        private volatile bool _stopTopLevelAnimation = false;

        private TextController _textController;
        private List<IUserControl> _userControls = new List<IUserControl>();

        private Dictionary<Guid, IUserControl> _userControlCache = new Dictionary<Guid, IUserControl>();

        public UserControlManager(IScannerPanel scannerPanel, TextController textController)
        {
            _scannerPanel = scannerPanel;
            GridScanIterations = CoreGlobals.AppPreferences.GridScanIterations;
            _textController = textController;
        }

        public int GridScanIterations { get; set; }

        public static void FindAllUserControls(Control control, List<IUserControl> list)
        {
            if (control.Controls.Count == 0)
            {
                return;
            }

            try
            {
                foreach (var ctl in control.Controls)
                {
                    if (ctl is IUserControl)
                    {
                        list.Add(ctl as IUserControl);
                    }
                }
            }
            catch
            {
            }

            // now recursively add children
            foreach (Control ctl in control.Controls)
            {
                UserControlManager.FindAllUserControls(ctl, list);
            }
        }

        public static List<Widget> findAllWidgets(List<IUserControl> list)
        {
            var Widgets = new List<Widget>();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].UserControlCommon.RootWidget.Finder.FindAllChildren(typeof(WinControlWidget), Widgets);
                    list[i].UserControlCommon.RootWidget.Finder.FindAllChildren(typeof(WordListWidget), Widgets);
                    list[i].UserControlCommon.RootWidget.Finder.FindAllChildren(typeof(LetterListWidget), Widgets);
                    list[i].UserControlCommon.RootWidget.Finder.FindAllChildren(typeof(SentenceListWidget), Widgets);
                }
            }
            catch (Exception es)
            {
                Log.Debug("Error geting widgets: " + es.ToString());
            }
            return Widgets;
        }

        public bool AddUserControlByGuid(Control parent, Guid guid)
        {
            var mapEntry = UserControlConfigMap.GetUserControlConfigMapEntry(guid);

            if (mapEntry == null)
            {
                return false;
            }

            return addUserControlByName(parent, mapEntry.Name, null);
        }

        public bool AddUserControlByKeyOrName(Control parent, String userControlKeyName, String userControlName, object tag = null)
        {
            StopTopLevelAnimation();

            if (parent.Controls.Count > 0)
            {
                removeUserControl(parent, parent.Controls[0] as IUserControl);
            }

            _playerTransitioned = false;

            var retVal = String.IsNullOrEmpty(userControlKeyName) ? false : addUserControlByKey(parent, userControlKeyName, tag);

            if (!retVal)
            {
                retVal = String.IsNullOrEmpty(userControlName) ? false : addUserControlByName(parent, userControlName, tag);
            }

            return retVal;
        }

        public bool Initialize()
        {
            Context.AppActuatorManager.EvtSwitchActivated += appActuatorManager_EvtSwitchActivated;

            return true;
        }

        public void OnClosing()
        {
            Context.AppActuatorManager.EvtSwitchActivated -= appActuatorManager_EvtSwitchActivated;

            foreach (var control in _userControls)
            {
                closeUserControl(control);
            }

            _userControls.Clear();
        }

        public void OnPause()
        {
            Log.Debug("CALIBTEST UserControlManager.OnPause()");
            _playerTransitioned = false;

            foreach (var userControl in _userControls)
            {
                Log.Debug("CALIBTEST calling onPause for " + userControl.Descriptor.Name);
                userControl.OnPause();
            }
        }

        public void OnResume()
        {
            _stopTopLevelAnimation = true;

            Log.Debug("CALIBTEST UserControlManager.OnResume()");

            foreach (var userControl in _userControls)
            {
                Log.Debug("CALIBTEST. Calling onResume for uc" + userControl.Descriptor.Name);
                userControl.OnResume();
            }

            Log.Debug("CALIBTEST Calling StartTopLevelAnimation");
            StartTopLevelAnimation();
        }

        public bool PopUserControl(Control parent)
        {
            Guid guid = Guid.Empty;

            StopTopLevelAnimation();

            if (parent.Controls.Count > 0)
            {
                var userControl = parent.Controls[0] as IUserControl;

                var list = parent.Tag as List<Guid>;

                if (list != null && list.Count > 0)
                {
                    guid = list[list.Count - 1];

                    if (guid == Guid.Empty)
                    {
                        return false;
                    }

                    list.RemoveAt(list.Count - 1);

                    Log.Debug("MLEAK: Removed last entry. list.Count is " + list.Count);

                    removeUserControl(parent, userControl);
                }
                else
                {
                    Log.Debug("MLEAK: list.Count is already zero");

                }
            }

            return (guid != Guid.Empty) ? AddUserControlByGuid(parent, guid) : false;
        }

        public bool PushUserControlByKeyOrName(Control parent, String userControlKeyName, String userControlName, bool replaceCurrent = false)
        {
            Guid guid = Guid.Empty;

            if (parent.Controls.Count > 0)
            {
                var userControl = parent.Controls[0] as IUserControl;

                if (userControl != null)
                {
                    guid = userControl.Descriptor.Id;
                }

                List<Guid> list;
                if (parent.Tag == null)
                {
                    list = new List<Guid>();
                    parent.Tag = list;
                }
                else
                {
                    list = parent.Tag as List<Guid>;
                }

                if (!replaceCurrent)
                {
                    list.Add(guid);
                }

                Log.Debug("MLEAK: Added guid " + guid + ", List cocunt is " + list.Count);

                removeUserControl(parent, userControl);
            }

            return AddUserControlByKeyOrName(parent, userControlKeyName, userControlName, (guid != Guid.Empty) ? guid.ToString() : null);
        }

        public void StartTopLevelAnimation()
        {
            _iterationCount = 0;
            _stopTopLevelAnimation = false;

            _playerTransitioned = false;

            if (_userControls.Count > 0)
            {
                Log.Debug("CALIBTEST StartTopLevelAnimation. Starting animation for " + _userControls[0].Descriptor.Name);
                _userControls[0].UserControlCommon.AnimationManager.Start();
            }
        }

        public void StopTopLevelAnimation()
        {
            _stopTopLevelAnimation = true;
            foreach (var userControl in _userControls)
            {
                userControl.UserControlCommon.AnimationManager.Interrupt();
            }
        }

        private bool addUserControlByKey(Control parent, String userControlKeyName, object tag = null)
        {
            bool retVal = true;
            String userControlName = String.Empty;

            if (String.IsNullOrEmpty(userControlKeyName))
            {
                return false;
            }

            try
            {
                var panelConfigMapEntry = PanelConfigMap.GetPanelConfigMapEntry(_scannerPanel.PanelClass);
                if (panelConfigMapEntry == null)
                {
                    return false;
                }

                userControlName = panelConfigMapEntry.GetUserControlName(userControlKeyName);

                if (String.IsNullOrEmpty(userControlName))
                {
                    return false;
                }

                retVal = createAndInitializeUserControl(parent, userControlName, tag);
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to load userControl " + userControlName + ", exception: " + ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        private bool addUserControlByName(Control parent, String userControlName, object tag = null)
        {
            bool retVal = true;

            try
            {
                retVal = createAndInitializeUserControl(parent, userControlName, tag);
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to load userControl " + userControlName + ", exception: " + ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        private void animationManager_EvtPlayerAnimationTransition(object sender, string animationName, bool isTopLevel)
        {
            Log.Debug("AP1: transition " + animationName + ", isTopLevel: " + isTopLevel);

            if (!isTopLevel)
            {
                Log.Debug("AP1: SETTING _PlayterTransitioned to TRUE");
                _playerTransitioned = true;
                _iterationCount = 0;
            }
            else
            {
                Log.Debug("AP1: SETTING _PlayterTransitioned to FALSE");
                _playerTransitioned = false;
            }
        }

        private void appActuatorManager_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
        {
            Log.Debug("Switch activated");
            foreach (var userControl in _userControls)
            {
                var playerState = userControl.UserControlCommon.AnimationManager.GetPlayerState();

                Log.Debug("userControl: " + userControl.Descriptor.Name + ", state: " + playerState);

                if (playerState != PlayerState.Timeout && playerState != PlayerState.Interrupted)
                {
                    return;
                }
            }

            StartTopLevelAnimation();
        }

        private void closeUserControl(IUserControl userControl)
        {
            if (userControl != null)
            {
                userControl.EvtPlayerStateChanged -= userControl_EvtPlayerStateChanged;
                userControl.UserControlCommon.AnimationManager.EvtPlayerAnimationTransition -= animationManager_EvtPlayerAnimationTransition;

                userControl.UserControlCommon.Close();
                //userControl.UserControlCommon.Dispose();
            }
        }

        private bool createAndInitializeUserControl(Control parent, String userControlName, object tag = null)
        {
            var mapEntry = UserControlConfigMap.GetUserControlConfigMapEntry(userControlName);

            if (mapEntry == null)
            {
                return false;
            }

            var guid = mapEntry.UserControlId;

            IUserControl iUserControl;
            UserControl userControl;

            if (!_userControlCache.TryGetValue(guid, out iUserControl))
            {
                userControl = (UserControl)Activator.CreateInstance(mapEntry.UserControlType);
                iUserControl = (userControl as IUserControl);

                _userControlCache.Add(iUserControl.Descriptor.Id, iUserControl);
                Log.Debug("Adding UserControl to cache: " + iUserControl.Descriptor.Name);
            }
            else
            {
                userControl = (iUserControl as UserControl);
                Log.Debug("Got UserControl from cache: " + iUserControl.Descriptor.Name);
            }

            if (tag != null)
            {
                userControl.Tag = tag;
            }

            userControl.Dock = DockStyle.Fill;

            parent.Controls.Add(userControl);

            iUserControl.Initialize(mapEntry, _textController, _scannerPanel);

            iUserControl.OnLoad();
            iUserControl.EvtPlayerStateChanged += userControl_EvtPlayerStateChanged;
            iUserControl.UserControlCommon.AnimationManager.EvtPlayerAnimationTransition += animationManager_EvtPlayerAnimationTransition;

            _userControls.Add(iUserControl);

            iUserControl.UserControlCommon.RootWidget.HighlightOff();

            _iterations = GridScanIterations * _userControls.Count;

            return true;
        }

        private IUserControl getNextUserControl(IUserControl userControl)
        {
            int ii;
            Log.Debug("AP1 Find next user control. Count: " + _userControls.Count);
            for (ii = 0; ii < _userControls.Count; ii++)
            {
                if (_userControls[ii] == userControl)
                {
                    Log.Debug("AP1 Found! ii = " + ii);
                    break;
                }
            }

            if (ii < _userControls.Count)
            {
                ii++;
                if (ii >= _userControls.Count)
                {
                    ii = 0;
                }

                Log.Debug("AP1 Returning next user control " + _userControls[ii].Descriptor.Name);
                return _userControls[ii];
            }

            return null;
        }

        private void removeUserControl(Control parent, IUserControl userControl)
        {
            if (userControl != null)
            {
                closeUserControl(userControl);

                _userControls.Remove(userControl);
            }

            parent.Controls.Clear();
        }

        private void userControl_EvtPlayerStateChanged(IUserControl userControl, PlayerStateChangedEventArgs e)
        {
            Log.Debug("AP1 playerStateChanged for " + userControl.Descriptor.Name + ", " + "newState: " + e.NewState);

            if (_playerTransitioned)
            {
                Log.Debug("AP1: _playterTransitioned is TRUE.  Returning");
                return;
            }

            if (_stopTopLevelAnimation)
            {
                Log.Debug("AP1: _stopTopLevelanimation is TRUE.  Returning");
                return;
            }

            if (e.NewState == PlayerState.Timeout)
            {
                Log.Debug("AP1 timeout for " + userControl.Descriptor.Name);
                var next = getNextUserControl(userControl);
                if (next != null)
                {
                    _iterationCount++;

                    if (_iterationCount < _iterations)
                    {
                        Log.Debug("AP1 Calling start on " + next.Descriptor.Name);
                        next.UserControlCommon.AnimationManager.Start();
                    }
                }
            }
        }
    }
}