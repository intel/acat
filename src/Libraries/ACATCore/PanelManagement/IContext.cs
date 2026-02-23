////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AbbreviationsManagement;
using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.CommandManagement;
using ACAT.Core.SpellCheckManagement;
using ACAT.Core.ThemeManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using System;
using System.Collections.Generic;

namespace ACAT.Core.PanelManagement
{
    /// <summary>
    /// Provides access to ACAT system-wide managers and configuration.
    /// Implement this interface when consuming Context via dependency injection;
    /// it mirrors the static <see cref="Context"/> API so that callers can
    /// switch from <c>Context.AppXXX</c> to an injected <see cref="IContext"/>
    /// without changing the property names.
    /// </summary>
    public interface IContext
    {
        /// <summary>Gets the Abbreviations Manager.</summary>
        AbbreviationsManager AppAbbreviationsManager { get; }

        /// <summary>Gets the Actuator Manager.</summary>
        ActuatorManager AppActuatorManager { get; }

        /// <summary>Gets the Agent Manager.</summary>
        AgentManager AppAgentMgr { get; }

        /// <summary>Gets the Automation Event Manager.</summary>
        AutomationEventManager AppAutomationEventManger { get; }

        /// <summary>Gets the Command Manager.</summary>
        CommandManager AppCommandManager { get; }

        /// <summary>Gets the Panel Manager.</summary>
        PanelManager AppPanelManager { get; }

        /// <summary>Gets or sets whether the application should quit.</summary>
        bool AppQuit { get; set; }

        /// <summary>Gets the Spell Check Manager.</summary>
        SpellCheckManager AppSpellCheckManager { get; }

        /// <summary>Gets the Theme Manager.</summary>
        ThemeManager AppThemeManager { get; }

        /// <summary>Gets the Text-to-Speech Manager.</summary>
        TTSManager AppTTSManager { get; }

        /// <summary>Gets or sets the current scanner window position.</summary>
        Windows.WindowPosition AppWindowPosition { get; set; }

        /// <summary>Gets the Word Prediction Manager.</summary>
        WordPredictionManager AppWordPredictionManager { get; }

        /// <summary>Gets the list of extension directories.</summary>
        IEnumerable<String> ExtensionDirs { get; }

        /// <summary>Gets or sets the keyboard layout command name.</summary>
        string KeyboardLayout { get; set; }

        /// <summary>Gets or sets whether a keyboard-layout change was requested.</summary>
        bool RestartKeyboardLayout { get; set; }

        /// <summary>Gets or sets whether the talk window should be shown on startup.</summary>
        bool ShowTalkWindowOnStartup { get; set; }

        /// <summary>
        /// Resolves a manager by interface type from the service provider.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to resolve.</typeparam>
        /// <returns>The manager instance, or <c>null</c> if not registered.</returns>
        TInterface GetManager<TInterface>() where TInterface : class;

        /// <summary>Returns the initialization completion status string.</summary>
        string GetInitCompletionStatus();

        /// <summary>Returns whether the initialization error was fatal.</summary>
        bool IsInitFatal();
    }
}
