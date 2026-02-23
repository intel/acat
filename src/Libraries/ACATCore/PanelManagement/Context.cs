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
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.SpellCheckManagement;
using ACAT.Core.ThemeManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictorManagement;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ACAT.Core.PanelManagement
{
    /// <summary>
    /// Encapsulates system wide global shared objects. Creates
    /// instances of all the managers in ACAT.  Since all managers
    /// are singletons, access to them can be made through this class.
    /// Handles initialization of all the different managers in ACAT
    /// such as the TTSManager, WordPrediction manager etc.
    /// There is a sequence to the initialization:
    ///     PreInit()
    ///     Init()
    ///     PostInit()
    ///
    /// </summary>
    public class Context : IContext
    {
        private static readonly Lazy<AbbreviationsManager> _abbreviationsManager = new Lazy<AbbreviationsManager>(() => AbbreviationsManager.Instance);
        private static readonly Lazy<ActuatorManager> _actuatorManager = new Lazy<ActuatorManager>(() => ActuatorManager.Instance);
        private static readonly Lazy<AgentManager> _agentManager = new Lazy<AgentManager>(() => AgentManager.Instance);
        private static readonly Lazy<AutomationEventManager> _automationEventManager = new Lazy<AutomationEventManager>(() => AutomationEventManager.Instance);
        private static readonly Lazy<CommandManager> _commandManager = new Lazy<CommandManager>(() => CommandManager.Instance);
        private static readonly List<String> _extensionDirs = new();
        private static readonly Lazy<PanelManager> _panelManager = new Lazy<PanelManager>(() => PanelManager.Instance);
        private static readonly Lazy<SpellCheckManager> _spellCheckManager = new Lazy<SpellCheckManager>(() => SpellCheckManager.Instance);
        private static readonly Lazy<ThemeManager> _themeManager = new Lazy<ThemeManager>(() => ThemeManager.Instance);
        private static readonly Lazy<TTSManager> _ttsManager = new Lazy<TTSManager>(() => TTSManager.Instance);
        private static readonly Lazy<WordPredictionManager> _wordPredictionManager = new Lazy<WordPredictionManager>(() => WordPredictionManager.Instance);

        /// <summary>
        /// Error message if there was an error during initialization
        /// </summary>
        private static String _completionStatus = String.Empty;

        /// <summary>
        /// Was there a warning (non-fatal) during initialization
        /// </summary>
        private static bool _initWarning;

        /// <summary>
        /// Did a fatal error occur during initialization?
        /// </summary>
        private static bool _isFatal;

        /// <summary>
        /// Which modules should be initialized
        /// </summary>
        private static StartupFlags _startupFlags = StartupFlags.All;

        /// <summary>
        /// Initializes the singleton instance of the class
        /// </summary>
        static Context()
        {
            AppQuit = false;
            AppWindowPosition = Windows.WindowPosition.CenterScreen;

            // Manager singleton objects will be initialized lazily on first access
            // This allows Context.ServiceProvider to be set before managers are created
        }

        /// <summary>
        /// Initializes a new instance of Context for dependency injection.
        /// Configures the static <see cref="ServiceProvider"/> from the injected
        /// <paramref name="serviceProvider"/> so that all static accessor properties
        /// resolve managers through the DI container.
        /// </summary>
        /// <param name="serviceProvider">The application DI service provider.</param>
        public Context(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            ServiceProvider = serviceProvider;
        }

        // ---------------------------------------------------------------
        // IContext instance implementation – each property / method
        // delegates to the corresponding static member so that consumers
        // using the injected IContext get the same behaviour as those
        // using the static Context.AppXXX API.
        // ---------------------------------------------------------------

        AbbreviationsManager IContext.AppAbbreviationsManager => AppAbbreviationsManager;
        ActuatorManager IContext.AppActuatorManager => AppActuatorManager;
        AgentManager IContext.AppAgentMgr => AppAgentMgr;
        AutomationEventManager IContext.AppAutomationEventManger => AppAutomationEventManger;
        CommandManager IContext.AppCommandManager => AppCommandManager;
        PanelManager IContext.AppPanelManager => AppPanelManager;

        bool IContext.AppQuit
        {
            get => AppQuit;
            set => AppQuit = value;
        }

        SpellCheckManager IContext.AppSpellCheckManager => AppSpellCheckManager;
        ThemeManager IContext.AppThemeManager => AppThemeManager;
        TTSManager IContext.AppTTSManager => AppTTSManager;

        Windows.WindowPosition IContext.AppWindowPosition
        {
            get => AppWindowPosition;
            set => AppWindowPosition = value;
        }

        WordPredictionManager IContext.AppWordPredictionManager => AppWordPredictionManager;
        IEnumerable<String> IContext.ExtensionDirs => ExtensionDirs;

        string IContext.KeyboardLayout
        {
            get => KeyboardLayout;
            set => KeyboardLayout = value;
        }

        bool IContext.RestartKeyboardLayout
        {
            get => RestartKeyboardLayout;
            set => RestartKeyboardLayout = value;
        }

        bool IContext.ShowTalkWindowOnStartup
        {
            get => ShowTalkWindowOnStartup;
            set => ShowTalkWindowOnStartup = value;
        }

        TInterface IContext.GetManager<TInterface>() => GetManager<TInterface>();
        string IContext.GetInitCompletionStatus() => GetInitCompletionStatus();
        bool IContext.IsInitFatal() => IsInitFatal();

        /// <summary>
        /// Raised when the culture changes
        /// </summary>
        public static event CultureChanged EvtCultureChanged;

        /// <summary>
        /// Which modules to activate?
        /// </summary>
        [Flags]
        public enum StartupFlags
        {
            Minimal = 0,
            WordPrediction = 1,
            WindowsActivityMonitor = 2,
            TextToSpeech = 4,
            SpellChecker = 16,
            Abbreviations = 32,
            AgentManager = 64,
            PerfMon = 128,
            NoUI = 256,
            NoActuator = 512,
            MouseScanner = 1024,
            All = 0xffff
        }

        /// <summary>
        /// Gets the single Abbreviations Manager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static AbbreviationsManager AppAbbreviationsManager
        {
            get { return ResolveManager<AbbreviationsManager>(() => _abbreviationsManager.Value); }
        }

        /// <summary>
        /// Gets the ACAT ActuatorManager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static ActuatorManager AppActuatorManager
        {
            get { return ResolveManager<ActuatorManager>(() => _actuatorManager.Value); }
        }

        /// <summary>
        /// Gets the ACAT Application Agent Manager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static AgentManager AppAgentMgr
        {
            get { return ResolveManager<AgentManager>(() => _agentManager.Value); }
        }

        /// <summary>
        /// Gets the ACAT Automation Event Manager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static AutomationEventManager AppAutomationEventManger
        {
            get { return ResolveManager<AutomationEventManager>(() => _automationEventManager.Value); }
        }

        /// <summary>
        /// Gets the ACAT Command Manager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static CommandManager AppCommandManager
        {
            get { return ResolveManager<CommandManager>(() => _commandManager.Value); }
        }

        /// <summary>
        /// Gets the ACAT PanelManager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static PanelManager AppPanelManager
        {
            get { return ResolveManager<PanelManager>(() => _panelManager.Value); }
        }

        /// <summary>
        /// Gets or sets whether the application should quit
        /// </summary>
        public static bool AppQuit { get; set; }

        /// <summary>
        /// Gets the ACAT Spellcheck Manager.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static SpellCheckManager AppSpellCheckManager
        {
            get { return ResolveManager<SpellCheckManager>(() => _spellCheckManager.Value); }
        }

        /// <summary>
        /// Gets the ACAT Theme Manager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static ThemeManager AppThemeManager
        {
            get { return ResolveManager<ThemeManager>(() => _themeManager.Value); }
        }

        /// <summary>
        /// Gets the ACAT Text to speech Manager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static TTSManager AppTTSManager
        {
            get { return ResolveManager<TTSManager>(() => _ttsManager.Value); }
        }

        /// <summary>
        /// Gets or sets the current scanner position
        /// </summary>
        public static Windows.WindowPosition AppWindowPosition { get; set; }

        /// <summary>
        /// Gets the ACAT WordPredictionManager object.
        /// Resolves from the DI container when <see cref="ServiceProvider"/> is configured,
        /// otherwise falls back to the static singleton instance.
        /// </summary>
        public static WordPredictionManager AppWordPredictionManager
        {
            get { return ResolveManager<WordPredictionManager>(() => _wordPredictionManager.Value); }
        }

        /// <summary>
        /// Gets the list of extension directories
        /// </summary>
        public static IEnumerable<String> ExtensionDirs
        {
            get
            {
                if (!_extensionDirs.Any())
                {
                    getExtensionDirs();
                }

                return _extensionDirs;
            }
        }

        /// <summary>
        /// Gets or sets the command name for the keyboardLayout
        /// </summary>
        public static string KeyboardLayout { get; set; }

        /// <summary>
        /// Gets or sets if the Keyboard layout has been requested to change
        /// </summary>
        public static bool RestartKeyboardLayout { get; set; }

        /// <summary>
        /// Gets or sets whether the talk windows should be displayed
        /// when the application is launched
        /// </summary>
        public static bool ShowTalkWindowOnStartup { get; set; }

        /// <summary>
        /// Gets or sets the service provider for dependency injection in extension instantiation
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets a manager from the service provider if available, otherwise falls back to singleton.
        /// Captures the provider reference once to avoid race conditions with concurrent updates.
        /// This method provides a transition path to full dependency injection.
        /// </summary>
        /// <typeparam name="T">The type of manager to resolve</typeparam>
        /// <param name="fallback">Fallback function to get the singleton instance</param>
        /// <returns>The manager instance from DI or singleton</returns>
        private static T ResolveManager<T>(Func<T> fallback) where T : class
        {
            var provider = ServiceProvider;
            if (provider != null)
            {
                var service = provider.GetService(typeof(T)) as T;
                if (service != null)
                {
                    return service;
                }
            }
            return fallback();
        }

        /// <summary>
        /// Resolves a manager by its interface type from the service provider.
        /// This is the preferred way to access managers when using dependency injection.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to resolve</typeparam>
        /// <returns>The manager instance, or null if not registered</returns>
        public static TInterface GetManager<TInterface>() where TInterface : class
        {
            var provider = ServiceProvider;
            if (provider == null)
            {
                throw new InvalidOperationException(
                    "ServiceProvider is not configured. Call Context.ServiceProvider = serviceProvider before accessing managers via DI.");
            }

            return provider.GetService(typeof(TInterface)) as TInterface;
        }

        ///// <summary>
        ///// Changes the culture to the specified culture
        ///// </summary>
        ///// <param name="culture">culture to change to</param>
        //public static void ChangeCulture(CultureInfo cultureInfo)
        //{
        //    var culture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

        //    CultureInfo.DefaultThreadCurrentCulture = culture;
        //    CultureInfo.DefaultThreadCurrentUICulture = culture;

        //    ResourceUtils.InstallLanguageForUser();

        //    if (EvtCultureChanged != null)
        //    {
        //        WindowActivityMonitor.Pause();
        //        EvtCultureChanged(cultureInfo, new CultureChangedEventArg(cultureInfo));
        //        WindowActivityMonitor.Resume();
        //    }
        //}

        /// <summary>
        /// Disposes allocated resources
        /// </summary>
        public static void Dispose()
        {
            PerfMon.StopLogging();

            if (!isEnabled(StartupFlags.NoActuator))
            {
                AppActuatorManager?.Dispose();
            }

            if (!isEnabled(StartupFlags.NoUI))
            {
                AppPanelManager?.Dispose();
            }

            if (isEnabled(StartupFlags.WordPrediction))
            {
                AppWordPredictionManager?.Dispose();
            }

            if (isEnabled(StartupFlags.TextToSpeech))
            {
                AppTTSManager?.Dispose();
            }

            if (isEnabled(StartupFlags.SpellChecker))
            {
                AppSpellCheckManager?.Dispose();
            }

            if (isEnabled(StartupFlags.Abbreviations))
            {
                AppAbbreviationsManager?.Dispose();
            }

            if (isEnabled(StartupFlags.AgentManager))
            {
                AppAgentMgr?.Dispose();
            }

            AppAutomationEventManger?.Dispose();

            WindowActivityMonitor.Dispose();
        }

        /// <summary>
        /// Returns the string that contains the completion status
        /// (the error message for example if init failed)
        /// </summary>
        /// <returns>Status string</returns>
        public static String GetInitCompletionStatus()
        {
            return _completionStatus;
        }

        /// <summary>
        /// Performs initialization.  Depending on the startup
        /// flags, only loads those modules that have been specified.
        /// </summary>
        /// <param name="startup">Which modules to init?</param>
        /// <returns></returns>
        public static bool Init(StartupFlags startup = StartupFlags.All)
        {
            getExtensionDirs();

            _startupFlags = startup;

            AppWindowPosition = CoreGlobals.AppPreferences.ScannerPosition;

            bool retVal = AppCommandManager.Init();

            if (retVal)
            {
                retVal = createAgentManager();
            }

            if (retVal && !isEnabled(StartupFlags.NoUI))
            {
                retVal = createThemeManager();

                if (retVal)
                {
                    retVal = createPanelManager();
                }

                if (retVal)
                {
                    retVal = initWidgetManager();
                }
            }

            if (retVal)
            {
                retVal = createActuatorManager();
            }

            if (retVal)
            {
                retVal = createWordPredictionManager();
            }

            if (retVal)
            {
                retVal = createTTSManager();
            }

            if (retVal)
            {
                retVal = createSpellCheckManager();
            }

            if (retVal)
            {
                retVal = createAbbreviationsManager();
            }

            if (retVal)
            {
                startPerformanceMonitor();
            }

            if (_initWarning)
            {
                retVal = false;
            }

            LogManager.GetLogger<Context>().LogDebug("Returning {RetVal} from context init", retVal);
            return retVal;
        }

        /// <summary>
        /// Returns whether initialization error was fatal
        /// </summary>
        /// <returns></returns>
        public static bool IsInitFatal()
        {
            return _isFatal;
        }

        /// <summary>
        /// Call this AFTER the Init function
        /// </summary>
        public static bool PostInit()
        {
            if (isEnabled(StartupFlags.AgentManager))
            {
                if (!AppAgentMgr.PostInit())
                {
                    return false;
                }
            }

            if (AppActuatorManager != null)
            {
                if (!AppActuatorManager.PostInit())
                {
                    return false;
                }
            }

            if (AppWordPredictionManager != null)
            {
                if (!AppWordPredictionManager.PostInit())
                {
                    return false;
                }
            }

            if (isEnabled(StartupFlags.WindowsActivityMonitor))
            {
                WindowActivityMonitor.Start();
            }

            return true;
        }

        /// <summary>
        /// Call this BEFORE calling the Init() function.
        /// </summary>
        /// <returns>true on success</returns>
        public static bool PreInit()
        {
            return true;
        }

        /// <summary>
        /// Creates the singleton instance of the Abbreviations manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool createAbbreviationsManager()
        {
            bool retVal = true;

            if (isEnabled(StartupFlags.Abbreviations))
            {
                retVal = AppAbbreviationsManager.Init();

                if (!retVal)
                {
                    setCompletionStatus("Abbreviations load error.  Abbreviations will be disabled", false);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates the singleton instance of the Actuator manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool createActuatorManager()
        {
            bool retVal = true;

            if (!isEnabled(StartupFlags.NoActuator))
            {
                retVal = AppActuatorManager.LoadExtensions(ExtensionDirs);

                if (retVal)
                {
                    retVal = AppActuatorManager.Init(ExtensionDirs);
                }

                if (!retVal)
                {
                    setCompletionStatus("Error initializing actuator manager");
                }
            }

            return retVal;
        }

        /// <summary>
        /// Initializes the singleton instance of the Agent manager
        /// </summary>
        /// <returns></returns>
        private static bool createAgentManager()
        {
            bool retVal = true;

            if (isEnabled(StartupFlags.AgentManager))
            {
                retVal = AppAgentMgr.LoadExtensions(ExtensionDirs);
                if (retVal)
                {
                    retVal = AppAgentMgr.Init(ExtensionDirs);
                }

                if (!retVal)
                {
                    setCompletionStatus("Error initializing the Agent manager");
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates the singleton instance of the Screen manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool createPanelManager()
        {
            bool retVal = AppPanelManager.Init(ExtensionDirs);
            if (!retVal)
            {
                setCompletionStatus("Error initializing Panel Manager");
            }

            return retVal;
        }

        /// <summary>
        /// Creates the singleton instance of the SpellCheck manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool createSpellCheckManager()
        {
            bool retVal = true;

            if (isEnabled(StartupFlags.SpellChecker))
            {
                retVal = AppSpellCheckManager.LoadExtensions(ExtensionDirs);
                if (retVal)
                {
                    retVal = AppSpellCheckManager.Init(ExtensionDirs);
                }

                if (retVal)
                {
                    retVal = AppSpellCheckManager.SetActiveSpellChecker();
                    if (!retVal)
                    {
                        setCompletionStatus("Error setting active spell checker");
                    }
                }
                else
                {
                    setCompletionStatus("Error initializing SpellCheck Manager");
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates the singleton instance of the Theme manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool createThemeManager()
        {
            bool retVal = AppThemeManager.Init();
            if (retVal)
            {
                AppThemeManager.SetActiveTheme(CoreGlobals.AppPreferences.Theme);
            }
            else
            {
                setCompletionStatus("Error initializing Theme Manager");
            }

            return retVal;
        }

        /// <summary>
        /// Creates the singleton instance of the Text-to-Speech manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool createTTSManager()
        {
            bool retVal = true;

            if (isEnabled(StartupFlags.TextToSpeech) && CoreGlobals.AppPreferences.EnableTextToSpeech)
            {
                retVal = AppTTSManager.LoadExtensions(ExtensionDirs);
                if (retVal)
                {
                    retVal = AppTTSManager.Init(ExtensionDirs);
                }

                if (retVal)
                {
                    retVal = AppTTSManager.SetActiveEngine();
                }

                if (!retVal)
                {
                    setCompletionStatus("Error initializing text to speech.  TTS will be disabled", false);
                    retVal = true;
                    _initWarning = true;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates the singleton instance of the Word Prediction manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool createWordPredictionManager()
        {
            bool retVal = true;

            if (isEnabled(StartupFlags.WordPrediction))
            {
                retVal = AppWordPredictionManager.LoadExtensions(ExtensionDirs);

                if (retVal)
                {
                    retVal = AppWordPredictionManager.Init(ExtensionDirs);
                }

                if (retVal)
                {
                    retVal = AppWordPredictionManager.SetActiveWordPredictor(CultureInfo.CurrentUICulture);
                    if (!retVal)
                    {
                        setCompletionStatus("Error setting active word prediction engine");
                    }
                }
                else
                {
                    setCompletionStatus("Error initializing Word Predition engine");
                }
            }

            return retVal;
        }

        /// <summary>
        /// Reads the config setting for extension directories,
        /// resolves them into full directory paths, checks if they
        /// are valid and stores the resultant set of valid directories
        /// in _extensionDirs
        /// </summary>
        private static void getExtensionDirs()
        {
            _extensionDirs.Clear();
            // TODO: Add support for multiple extension directories
            //var dirs = CoreGlobals.AppPreferences.Extensions.Split(',');

            _extensionDirs.Add(FileUtils.GetExtensionDir());
        }

        /// <summary>
        /// Initialize the widget manager
        /// </summary>
        /// <returns>true on success</returns>
        private static bool initWidgetManager()
        {
            bool retVal = WidgetManager.Init(ExtensionDirs);
            if (!retVal)
            {
                setCompletionStatus("Error initializing Widget Manager");
            }

            return retVal;
        }

        /// <summary>
        /// Checks if the specified flag is in startup
        /// </summary>
        /// <param name="flag">Flag to check for</param>
        /// <returns>true if it is</returns>
        private static bool isEnabled(StartupFlags flag)
        {
            return (_startupFlags & flag) == flag;
        }

        /// <summary>
        /// Sets the completion status string. This could be an
        /// error message for instance
        /// </summary>
        /// <param name="status">status string</param>
        /// <param name="fatal">is the error fatal?</param>
        private static void setCompletionStatus(String status, bool fatal = true)
        {
            _completionStatus = status;
            _isFatal = fatal;
        }

        private static void startPerformanceMonitor()
        {
            if (!isEnabled(StartupFlags.PerfMon))
            {
                return;
            }

            PerfMon.Enable = CoreGlobals.AppPreferences.PerMonEnable;
            PerfMon.EnablePerfMonCpu = CoreGlobals.AppPreferences.PerMonCPUEnable;
            PerfMon.EnablePerfMonMemory = CoreGlobals.AppPreferences.PerMonMemoryEnable;
            if (CoreGlobals.AppPreferences.PerfMonLogInterval >= 0)
            {
                PerfMon.StartLogging(CoreGlobals.AppPreferences.PerfMonLogInterval * 1000);
            }
        }
    }
}