//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using ACAT.Core.WidgetManagement.Layout;
using ACAT.Core.Widgets;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace ACAT.Core.UserControlManagement
{
    public class UserControlKeyboardCommon : UserControlCommon
    {
        private readonly ILogger<UserControlKeyboardCommon> _logger;

        public UserControlKeyboardCommon(IUserControl userControl, UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel iScannerPanel, ILogger<UserControlKeyboardCommon> logger = null) :
            base(userControl, mapEntry, iScannerPanel, logger ?? LoggingConfiguration.CreateLogger<UserControlCommon>())
        {
            _logger = logger;
            TextController = textController;
        }

        public TextController TextController { get; private set; }

        public void AutoCompleteWord(String word)
        {
            TextController.AutoCompleteWord(word);
        }


        // TODO FIX ME - Call base after checking for virtual key
        protected override void actuateButton(Widget widget)
        {
            ActuatedWidget = null;

            if (isPaused ||
                widget is WordListItemWidget ||
                String.IsNullOrEmpty(widget.Value) ||
                widget is not IButtonWidget)
            {
                return;
            }

            var button = (IButtonWidget)widget;

            ActuatedWidget = widget;

            if (widget.IsCommand)
            {
                runCommand(widget.Command);
            }
            else if (button.GetWidgetAttribute().IsVirtualKey)
            {
                actuateVirtualKey(button.GetWidgetAttribute(), widget.Value);
            }
            else if (!Context.AppAgentMgr.TextChangedNotifications.OnHold())
            {
                if (widget.Value.Length > 1)
                {
                    CoreGlobals.Stopwatch1.Reset();
                    CoreGlobals.Stopwatch1.Start();
                    Context.AppAgentMgr.TextChangedNotifications.Hold();
                    SendKeys.SendWait(widget.Value + " ");
                    Context.AppAgentMgr.TextChangedNotifications.Release();
                    CoreGlobals.Stopwatch1.Stop();
                    _logger?.LogDebug("TimeElapsed 1: {ElapsedMs}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
                }
                else
                {
                    CoreGlobals.Stopwatch1.Reset();
                    CoreGlobals.Stopwatch1.Start();

                    actuateKey(button.GetWidgetAttribute(), widget.Value[0]);

                    CoreGlobals.Stopwatch1.Stop();
                    _logger?.LogDebug("TimeElapsed 2 : {ElapsedMs}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
                }
            }

            ActuatedWidget = null;
        }

        private void actuateKey(WidgetAttribute widgetAttribute, char value)
        {
            _logger?.LogDebug("{Value}", value);
            if (!TextController.HandlePunctuation(widgetAttribute.Modifiers, value))
            {
                if ((KeyStateTracker.IsShiftOn() || KeyStateTracker.IsCapsLockOn()) &&
                    !String.IsNullOrEmpty(widgetAttribute.ShiftValue))
                {
                    TextController.HandleAlphaNumericChar(widgetAttribute.ShiftValue[0]);
                }
                else
                {
                    TextController.HandleAlphaNumericChar(widgetAttribute.Modifiers, value);
                }
            }
        }

        private void actuateVirtualKey(WidgetAttribute widgetAttribute, String value)
        {
            _logger?.LogDebug("VirtualKey: {Value}", value);

            TextController.HandleVirtualKey(widgetAttribute.Modifiers, value);
        }
    }
}