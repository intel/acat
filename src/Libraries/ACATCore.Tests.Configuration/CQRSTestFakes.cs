////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CQRSTestFakes.cs
//
// Minimal fake (test-double) implementations of IActuatorManager and
// IAgentManager used exclusively by CQRSPatternTests.  Only the members
// exercised by the CQRS command/query handlers are implemented; all other
// interface members throw NotImplementedException.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.ActuatorManagement.Settings;
using ACAT.Core.AgentManagement;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACATCore.Tests.Configuration
{
    /// <summary>
    /// Minimal fake <see cref="IActuatorManager"/> that tracks Pause/Resume calls.
    /// </summary>
    internal sealed class FakeActuatorManager : IActuatorManager
    {
        public bool PauseCalled { get; private set; }
        public bool ResumeCalled { get; private set; }

        public void Pause() => PauseCalled = true;
        public void Resume() => ResumeCalled = true;

        // ---- unused members ----
        public IEnumerable<IActuator> ActuatorsList => throw new NotImplementedException();
        public event ActuatorManager.CalibrationStartNotify EvtCalibrationStartNotify { add { } remove { } }
        public event EventHandler EvtCalibrationEndNotify { add { } remove { } }
        public event ActuatorManager.ActuatorSwitchEvent EvtSwitchAccepted { add { } remove { } }
        public event ActuatorManager.ActuatorSwitchEvent EvtSwitchActivated { add { } remove { } }
        public event ActuatorManager.ActuatorSwitchEvent EvtSwitchDown { add { } remove { } }
        public event ActuatorManager.SwitchHook EvtSwitchHook { add { } remove { } }
        public event ActuatorManager.ActuatorSwitchEvent EvtSwitchRejected { add { } remove { } }
        public event ActuatorManager.ActuatorSwitchEvent EvtSwitchUp { add { } remove { } }
        public bool CheckScanTimingConfigureEnable() => throw new NotImplementedException();
        public void Dispose() { }
        public ActuatorConfig GetActuatorConfig() => throw new NotImplementedException();
        public IActuator GetActuator(Type actuatorType) => throw new NotImplementedException();
        public IActuator GetActuator(Guid id) => throw new NotImplementedException();
        public IActuator GetCalibrationSupportedActuator() => throw new NotImplementedException();
        public IActuator GetKeyboardActuator() => throw new NotImplementedException();
        public IActuator GetSwitchInterfaceActuator() => throw new NotImplementedException();
        public bool Init(IEnumerable<string> extensionDirs) => throw new NotImplementedException();
        public bool IsSwitchActive() => throw new NotImplementedException();
        public bool LoadExtensions(IEnumerable<string> extensionDirs, bool all = false) => throw new NotImplementedException();
        public void NotifyEndCalibration() => throw new NotImplementedException();
        public void NotifyStartCalibration(CalibrationNotifyEventArgs args) => throw new NotImplementedException();
        public void OnCalibrationCanceled(IActuator source) => throw new NotImplementedException();
        public void OnCalibrationPeriodExpired(IActuator source) => throw new NotImplementedException();
        public void OnEndCalibration(IActuator source, string errorMessage = "", bool enableConfigure = true) => throw new NotImplementedException();
        public void OnError(IActuator source, string message, bool enableConfigure = true) => throw new NotImplementedException();
        public void OnInitDone(IActuator source, bool success = true) => throw new NotImplementedException();
        public void OnPostInitDone(IActuator source, bool success = true) => throw new NotImplementedException();
        public bool PostInit() => throw new NotImplementedException();
        public bool RegisterSwitch(IActuator actuator, SwitchSetting switchSetting) => throw new NotImplementedException();
        public void RequestCalibration(IActuator source, RequestCalibrationReason reason) => throw new NotImplementedException();
        public void SavePreferences(object sender, IEnumerable<PreferencesCategory> preferencesCategories) => throw new NotImplementedException();
        public void ShowScanTimingsConfigureDialog() => throw new NotImplementedException();
        public void ShowTryoutDialog(bool startup = false) => throw new NotImplementedException();
        public void UpdateCalibrationStatus(IActuator source, string caption, string prompt, int timeout = 0, bool enableConfigure = true, string buttonText = "") => throw new NotImplementedException();
        public bool UnregisterSwitch(IActuator actuator, string switchName) => throw new NotImplementedException();
    }

    /// <summary>
    /// Minimal fake <see cref="IAgentManager"/> that supports
    /// <see cref="GetCurrentAgentName"/>.
    /// </summary>
    internal sealed class FakeAgentManager : IAgentManager
    {
        public string CurrentAgentName { get; set; }

        public string GetCurrentAgentName() => CurrentAgentName;

        // ---- unused members ----
        public IApplicationAgent ActiveAgent => throw new NotImplementedException();
        public EditingMode CurrentEditingMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IApplicationAgent DefaultAgentForContextSwitchDisable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool EnableAppAgentContextSwitch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool EnableContextualMenusForDialogs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool EnableContextualMenusForMenus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IApplicationAgent GenericAppAgent => throw new NotImplementedException();
        public IKeyboard Keyboard => throw new NotImplementedException();
        public IApplicationAgent NullAgent => throw new NotImplementedException();
        public TriggerLock TextChangedNotifications => throw new NotImplementedException();
        public event FocusChanged EvtFocusChanged { add { } remove { } }
        public event MouseEventHandler EvtNonScannerMouseDown { add { } remove { } }
        public event PanelRequest EvtPanelRequest { add { } remove { } }
        public event EventHandler EvtPreActivateAgent { add { } remove { } }
        public event ScannerHitTest EvtScannerHitTest { add { } remove { } }
        public event EventHandler EvtTextChanged { add { } remove { } }
        public AgentContext ActiveContext() => throw new NotImplementedException();
        public Task ActivateAgent(IApplicationAgent caller, IFunctionalAgent agent) => throw new NotImplementedException();
        public Task ActivateAgent(IFunctionalAgent agent) => throw new NotImplementedException();
        public void AddAgent(IntPtr handle, IApplicationAgent agent) => throw new NotImplementedException();
        public bool CanActivateFunctionalAgent() => throw new NotImplementedException();
        public void CheckCommandEnabled(CommandEnabledArg arg) => throw new NotImplementedException();
        public void Dispose() { }
        public IApplicationAgent GetAgentByCategory(string category) => throw new NotImplementedException();
        public IApplicationAgent GetAgentByName(string name) => throw new NotImplementedException();
        public IEnumerable<object> GetExtensions() => throw new NotImplementedException();
        public IFunctionalAgent GetFunctionalAgentByName(string name) => throw new NotImplementedException();
        public bool Init(IEnumerable<string> extensionDirs) => throw new NotImplementedException();
        public bool IsCurrentAgent(string agentName) => throw new NotImplementedException();
        public bool LoadExtensions(IEnumerable<string> extensionDirs) => throw new NotImplementedException();
        public void OnPanelClosed(string panelClass) => throw new NotImplementedException();
        public void PausePanelChangeRequests() => throw new NotImplementedException();
        public bool PostInit() => throw new NotImplementedException();
        public void RemoveAgent(string AgentName) => throw new NotImplementedException();
        public void RemoveAgent(IntPtr handle) => throw new NotImplementedException();
        public void ResumePanelChangeRequests(bool getActiveWindow = true) => throw new NotImplementedException();
        public void RunCommand(string command, ref bool handled) => throw new NotImplementedException();
        public void RunCommand(string command, object arg, ref bool handled) => throw new NotImplementedException();
        public void ShowContextMenu() => throw new NotImplementedException();
    }
}
