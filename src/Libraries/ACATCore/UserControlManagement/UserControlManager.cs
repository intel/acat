////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.PanelManagement.PanelConfig;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.Widgets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Core.UserControlManagement
{
    /// <summary>
    /// Used by scanners to manage the usercontrols they host.
    /// Has functions to handle top level iterations between
    /// usercontrols, stack usercontrols in a container etc.
    /// </summary>
    public class UserControlManager
    {
        private readonly ILogger<UserControlManager> _logger;
        private int _iterationCount = 0;
        private int _iterations = 1;
        private volatile bool _playerTransitioned = false;

        private readonly IScannerPanel _scannerPanel;
        private volatile bool _stopTopLevelAnimation = false;

        private readonly TextController _textController;
        private readonly List<IUserControl> _userControls = new();

        private readonly Dictionary<Guid, IUserControl> _userControlCache = new();

        public UserControlManager(IScannerPanel scannerPanel, TextController textController, ILogger<UserControlManager> logger)
        {            _logger = logger;            _scannerPanel = scannerPanel;
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
                _logger.LogError(es, "Error geting widgets: {Exception}", es.ToString());
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
            //StopTopLevelAnimation();

            if (parent.Controls.Count > 0)
            {
                removeUserControl(parent, parent.Controls[0] as IUserControl);
            }

            _playerTransitioned = false;

            var retVal = !String.IsNullOrEmpty(userControlKeyName) && addUserControlByKey(parent, userControlKeyName, tag);

            if (!retVal)
            {
                retVal = !String.IsNullOrEmpty(userControlName) && addUserControlByName(parent, userControlName, tag);
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
            _logger.LogDebug("CALIBTEST UserControlManager.OnPause()");
            _playerTransitioned = false;

            foreach (var userControl in _userControls)
            {
                _logger.LogTrace("CALIBTEST calling onPause for {UserControlName}", userControl.Descriptor.Name);
                userControl.OnPause();
            }
        }

        public void OnResume()
        {
            _stopTopLevelAnimation = true;

            _logger.LogDebug("CALIBTEST UserControlManager.OnResume()");

            foreach (var userControl in _userControls)
            {
                _logger.LogTrace("CALIBTEST. Calling onResume for uc{UserControlName}", userControl.Descriptor.Name);
                userControl.OnResume();
            }

            _logger.LogDebug("CALIBTEST Calling StartTopLevelAnimation");
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

                    removeUserControl(parent, userControl);
                }
                else
                {
                    _logger.LogWarning("MLEAK: list.Count is already zero");
                }
            }

            return (guid != Guid.Empty) && AddUserControlByGuid(parent, guid);
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
                _logger.LogDebug("CALIBTEST StartTopLevelAnimation. Starting animation for {UserControlName}", _userControls[0].Descriptor.Name);
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
            String userControlName = String.Empty;

            if (String.IsNullOrEmpty(userControlKeyName))
            {
                return false;
            }

            bool retVal;
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
                _logger.LogError(ex, "Unable to load userControl {UserControlName}", userControlName);
                retVal = false;
            }

            return retVal;
        }

        private bool addUserControlByName(Control parent, String userControlName, object tag = null)
        {
            bool retVal;
            try
            {
                retVal = createAndInitializeUserControl(parent, userControlName, tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to load userControl {UserControlName}", userControlName);
                retVal = false;
            }

            return retVal;
        }

        private void animationManager_EvtPlayerAnimationTransition(object sender, string animationName, bool isTopLevel)
        {
            _logger.LogDebug("AP1: transition {AnimationName}, isTopLevel: {IsTopLevel}", animationName, isTopLevel);

            if (!isTopLevel)
            {
                _logger.LogTrace("AP1: SETTING _PlayerTransitioned to TRUE");
                _playerTransitioned = true;
                _iterationCount = 0;
            }
            else
            {
                _logger.LogTrace("AP1: SETTING _PlayerTransitioned to FALSE");
                _playerTransitioned = false;
            }
        }

        private void appActuatorManager_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
        {
            _logger.LogDebug("Switch activated");
            foreach (var userControl in _userControls)
            {
                var playerState = userControl.UserControlCommon.AnimationManager.GetPlayerState();

                _logger.LogTrace("userControl: {UserControlName}, state: {PlayerState}", userControl.Descriptor.Name, playerState);

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

            UserControl userControl;

            if (!_userControlCache.TryGetValue(guid, out IUserControl iUserControl))
            {
                userControl = (UserControl)Activator.CreateInstance(mapEntry.UserControlType);
                iUserControl = (userControl as IUserControl);

                _userControlCache.Add(iUserControl.Descriptor.Id, iUserControl);
                _logger.LogTrace("Adding UserControl to cache: {UserControlName}", iUserControl.Descriptor.Name);
            }
            else
            {
                userControl = (iUserControl as UserControl);
                _logger.LogTrace("Got UserControl from cache: {UserControlName}", iUserControl.Descriptor.Name);
            }

            if (tag != null)
            {
                userControl.Tag = tag;
            }

            //// Only change the DockStyle if it's not already set.
            //if (userControl.Dock == DockStyle.None)
            //{
            //    userControl.Dock = DockStyle.Top;
            //}
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
            _logger.LogTrace("AP1 Find next user control. Count: {Count}", _userControls.Count);
            for (ii = 0; ii < _userControls.Count; ii++)
            {
                if (_userControls[ii] == userControl)
                {
                    _logger.LogTrace("AP1 Found! ii = {Index}", ii);
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

                _logger.LogTrace("AP1 Returning next user control {UserControlName}", _userControls[ii].Descriptor.Name);
                return _userControls[ii];
            }

            return null;
        }

        public IUserControl getUserControlByGuid(String name)
        {
            return _userControls.Find(uc => uc.Descriptor.Name == name);
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
            _logger.LogTrace("AP1 playerStateChanged for {UserControlName}, newState: {NewState}", userControl.Descriptor.Name, e.NewState);

            if (_playerTransitioned)
            {
                _logger.LogTrace("AP1: _playterTransitioned is TRUE.  Returning");
                return;
            }

            if (_stopTopLevelAnimation)
            {
                _logger.LogTrace("AP1: _stopTopLevelanimation is TRUE.  Returning");
                return;
            }

            if (e.NewState == PlayerState.Timeout)
            {
                _logger.LogTrace("PlayerState timeout for {UserControlName}", userControl.Descriptor.Name);
                var next = getNextUserControl(userControl);
                if (next != null)
                {
                    _iterationCount++;

                    if (_iterationCount < _iterations)
                    {
                        _logger.LogTrace("AP1 Calling start on {UserControlName}", next.Descriptor.Name);
                        next.UserControlCommon.AnimationManager.Start();
                    }
                }
            }
        }
    }
}