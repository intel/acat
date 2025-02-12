﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// For the event raised when an agent exits
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void AgentClose(object sender, AgentCloseEventArgs e);

    /// <summary>
    /// For the event raised when focus changes on the windows desktop
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void FocusChanged(object sender, FocusChangedEventArgs e);

    /// <summary>
    /// For the event raised to request to display a scanner
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void PanelRequest(object sender, PanelRequestEventArgs e);

    /// <summary>
    /// For the event raised when the mouse was clicked anywhere on the scanner
    /// </summary>
    /// <param name="x">Mouse x position</param>
    /// <param name="y">Mouse Y position</param>
    /// <returns>return true if handled</returns>
    public delegate bool ScannerHitTest(int x, int y);

    public delegate void EditingModeSet(EditingMode oldMode, EditingMode newMode);


    /// <summary>
    /// Completion code of the Agent before it exits
    /// </summary>
    public enum CompletionCode
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Agent triggered a context switch to a window
        /// </summary>
        ContextSwitch,
    }

    /// <summary>
    /// Current editing mode
    /// </summary>
    public enum EditingMode
    {
        /// <summary>
        /// None
        /// </summary>
        Unknown,

        /// <summary>
        /// Text is being entered
        /// </summary>
        TextEntry,

        /// <summary>
        /// No text entry, only document navigation (arrow keys,
        /// pageup/page down etc
        /// </summary>
        Edit
    }

    /// <summary>
    /// The Agent Manager manages all the application and functional
    /// agent in ACAT.  It is responsible for loading the agents on
    /// startup.  Also responds to context changes on the desktop and
    /// responds by loading the appropriate agents.
    /// Notifies subscribers if the text or caret position in the the
    /// active text window.
    /// </summary>
    public class AgentManager : IDisposable
    {
        /// <summary>
        /// Root directory from which application agents will be loaded
        /// </summary>
        public static string AppAgentsRootDir = "AppAgents";

        /// <summary>
        /// Root directory from which functional agents will be loaded
        /// </summary>
        public static String FunctionalAgentsRootDir = "FunctionalAgents";

        public event EditingModeSet EvtEditingModeSet;

        /// <summary>
        /// Agent to handle Dialogs
        /// </summary>
        private const string DialogControlAgentName = "**dialogagent**";

        /// <summary>
        /// Hard-coded name of the Application agent for apps that are not
        /// natively supported by ACAT, in that, they don't have a custom
        /// Application agent
        /// </summary>
        private const string GenericAppAgentName = "**genericappagent**";

        /// <summary>
        /// Agent to handle menus
        /// </summary>
        private const string MenuControlAgentName = "**menuagent**";

        /// <summary>
        /// Null application agent, in-lieu of returning a null pointer
        /// </summary>
        private const string NullAgentName = "**nullagent**";

        /// <summary>
        /// Singleton instance of the Agent manager
        /// </summary>
        private static readonly AgentManager _instance = new AgentManager();

        /// <summary>
        /// Name of the executing assembly
        /// </summary>
        private readonly String _currentProcessName;

        /// <summary>
        /// Used to gate panel change notification calls.
        /// </summary>
        private readonly TriggerLock _panelChangeNotifications;

        /// <summary>
        /// Used for synchronization when activating an agent
        /// </summary>
        private readonly object _syncActivateAgent = new object();

        /// <summary>
        /// Used for synchronization when notifying that text changed
        /// in the target window
        /// </summary>
        private readonly object _syncTextChangeNotifyObj = new object();

        /// <summary>
        /// Used to gate for text change notification calls
        /// </summary>
        private readonly TriggerLock _textChangedNotifications;

        /// <summary>
        /// A cache of all agent objects loaded on startup
        /// </summary>
        private AgentsCache _agentsCache;

        /// <summary>
        /// Currently active agent
        /// </summary>
        private IApplicationAgent _currentAgent;

        /// <summary>
        /// Current editing mode - text entry or cursor movement?
        /// </summary>
        private EditingMode _currentEditingMode = EditingMode.Unknown;

        /// <summary>
        /// Dialog agent object
        /// </summary>
        private IApplicationAgent _dialogAgent;

        /// <summary>
        /// Has this object been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Application agent used for natively unsupported apps
        /// </summary>
        private IApplicationAgent _genericAppAgent;

        /// <summary>
        /// Has a request come in for displaying the contextual menu
        /// </summary>
        private volatile bool _getContextMenu;

        /// <summary>
        /// To prevent reentrancy
        /// </summary>
        private long _inActivateAppAgent = 0;

        /// <summary>
        /// The keyboard actuator object
        /// </summary>
        private KeyboardActuator _keyboardActuator;

        /// <summary>
        /// AppMenu agent object
        /// </summary>
        private IApplicationAgent _menuControlAgent;

        /// <summary>
        /// Used to gate text change notification calls
        /// </summary>
        private volatile bool _notifyTextChangedLock;

        /// <summary>
        /// Null agent object
        /// </summary>
        private IApplicationAgent _nullAgent;

        /// <summary>
        /// The Text Control agent that is currently active
        /// </summary>
        private ITextControlAgent _textControlAgent;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        private AgentManager()
        {
            _currentProcessName = Process.GetCurrentProcess().ProcessName.ToLower();

            _textChangedNotifications = new TriggerLock();
            TextChangedNotifications.EvtUnlocked += TextChangedNotifications_EvtUnlocked;

            _panelChangeNotifications = new TriggerLock();

            EnableAppAgentContextSwitch = true;
        }

        /// <summary>
        /// Raised to indicate that focus changed on the desktop
        /// </summary>
        public event FocusChanged EvtFocusChanged;

        /// <summary>
        /// Triggered when the mouse is clicked on a non-Scanner area
        /// of the screen
        /// </summary>
        public event MouseEventHandler EvtNonScannerMouseDown;

        /// <summary>
        /// Raised to request for a scanner to be displayed
        /// </summary>
        public event PanelRequest EvtPanelRequest;

        /// <summary>
        /// Triggered before functional agent is activated
        /// </summary>
        ///
        public event EventHandler EvtPreActivateAgent;

        /// <summary>
        /// Rasied when the user clicks anywhere on the scanner
        /// </summary>
        public event ScannerHitTest EvtScannerHitTest;

        /// <summary>
        /// Raised when text or caret position changes in the
        /// active text window.  The source of this event is one
        /// of the application agents
        /// </summary>
        public event EventHandler EvtTextChanged;

        /// <summary>
        /// Gets the singleton instance of Agent manager
        /// </summary>
        public static AgentManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the currently active agent
        /// </summary>
        public IApplicationAgent ActiveAgent
        {
            get { return _currentAgent; }
        }

        /// <summary>
        /// Gets the current editing mode
        /// </summary>
        public EditingMode CurrentEditingMode
        {
            get 
            { 
                return _currentEditingMode; 
            }
            set 
            {
                var prevMode = _currentEditingMode;
                _currentEditingMode = value; 

                EvtEditingModeSet?.BeginInvoke(prevMode, value, null, null);
            }
        }

        /// <summary>
        /// If context switches are disabled, which agent to use
        /// as the default agent
        /// </summary>
        public IApplicationAgent DefaultAgentForContextSwitchDisable { get; set; }

        /// <summary>
        /// Get/sets whether to enable context switch for application
        /// agents.  If disabled, it will only use the generic app
        /// agent.
        /// </summary>
        public bool EnableAppAgentContextSwitch { get; set; }

        /// <summary>
        /// Gets or sets the flag that controls whether the
        /// menu agent should be automatically displayed when
        /// a menu is activated on the desktop
        /// </summary>
        public bool EnableContextualMenusForDialogs { get; set; }

        /// <summary>
        /// Gets or sets the flag that controls whether the
        /// dialog agent should be automatically displayed when
        /// a dialog is activated on the desktop
        /// </summary>
        public bool EnableContextualMenusForMenus { get; set; }

        /// <summary>
        /// Gets the generic app agent
        /// </summary>
        public IApplicationAgent GenericAppAgent
        {
            get { return _genericAppAgent; }
        }

        /// <summary>
        /// Gets the Keyboard object that can be used to
        /// send keys to the keyboard buffer
        /// </summary>
        public IKeyboard Keyboard
        {
            get { return _textControlAgent.Keyboard; }
        }

        /// <summary>
        /// Gets the null app agent
        /// </summary>
        public IApplicationAgent NullAgent
        {
            get
            {
                return _nullAgent;
            }
        }

        /// <summary>
        /// Gets the semaphore used to gate text change notifications
        /// </summary>
        public TriggerLock TextChangedNotifications
        {
            get { return _textChangedNotifications; }
        }

        /// <summary>
        /// Gets or sets the active text control agent
        /// </summary>
        internal ITextControlAgent TextControlAgent
        {
            get { return _textControlAgent; }

            set { _textControlAgent = value; }
        }

        /// <summary>
        /// Activates a functional agent. The caller should to an 'await'
        /// on this to wait for it to complete.  This is because some
        /// functional agents return data and the caller has to wait
        /// for the functional agent to exit so it can get the data. Eg
        /// FileBrowser agent that returns a filename
        /// </summary>
        /// <param name="caller">The calling agent (can be null)</param>
        /// <param name="agent">Functional agent to activate</param>
        /// <returns>the task to wait on</returns>
        public async Task ActivateAgent(IApplicationAgent caller, IFunctionalAgent agent)
        {
            if (EvtPreActivateAgent != null)
            {
                EvtPreActivateAgent(this, new EventArgs());
            }

            lock (_syncActivateAgent)
            {
                if (_currentAgent != null && _currentAgent != agent)
                {
                    if (caller == null && !_currentAgent.QueryAgentSwitch(agent))
                    {
                        return;
                    }

                    _currentAgent.OnFocusLost();
                }

                if (caller != null)
                {
                    agent.Parent = caller;
                    caller.OnPause();
                }

                _textControlAgent = agent.TextControlAgent;

                setAgent(agent);
            }

            Log.Debug("Calling activateAgent: " + agent.Name);
            await activateAgent(agent);

            CompletionCode exitCode = agent.ExitCode;

            Log.Debug("Returned from activateAgent: " + agent.Name);
            setAgent(null);

            if (agent.ExitCommand != null)
            {
                if (agent.ExitCommand.ContextSwitch)
                {
                    Context.AppPanelManager.CloseCurrentPanel();
                }

                RunCommandDispatcher.Dispatch(agent.ExitCommand.Command);
            }
            else if (exitCode == CompletionCode.ContextSwitch)
            {
                Context.AppPanelManager.ClearStack();
                //EnumWindows.RestoreFocusToTopWindowOnDesktop();
                WindowActivityMonitor.GetActiveWindow();
            }
        }

        /// <summary>
        /// Activates the specified functional agent.
        /// The caller should to an 'await'
        /// on this to wait for it to complete.  This is because some
        /// functional agents return data and the caller has to wait
        /// for the functional agent to exit so it can get the data. Eg
        /// FileBrowser agent that returns a filename
        /// </summary>
        /// <param name="agent">agent to activate</param>
        /// <returns>Task to wait on</returns>
        public async Task ActivateAgent(IFunctionalAgent agent)
        {
            if (_currentAgent is IFunctionalAgent)
            {
                Log.Debug("Cannot activate functional agent " + agent.Name + ", functional agent " + _currentAgent.Name + " is currently active");
                return;
            }

            await ActivateAgent(null, agent);
        }

        /// <summary>
        /// Returns the agent context.  Remember as the
        /// focus changes on the desktop, agents can be switched.
        /// The AgentContext can be used to check if the context
        /// change during a call.
        /// </summary>
        /// <returns>ActiveContext object</returns>
        public AgentContext ActiveContext()
        {
            return new AgentContext(_textControlAgent);
        }

        /// <summary>
        /// Agent manager loads all agents on startup.  However,
        /// agents can also be added at runtime and this function
        /// allows for that.
        /// Adds an ad-hoc agent for the specified window. Whenever
        /// this window becomes active, the agent is activated.
        /// </summary>
        /// <param name="handle">window handle</param>
        /// <param name="agent">agent to add</param>
        public void AddAgent(IntPtr handle, IApplicationAgent agent)
        {
            Log.Debug("hwnd: " + handle + ", " + agent.Name);

            agent.EvtPanelRequest += agent_EvtPanelRequest;
            _agentsCache.AddAgent(handle, agent);

            var fgWindow = User32Interop.GetForegroundWindow();
            if (fgWindow != IntPtr.Zero)
            {
                if (fgWindow == handle)
                {
                    WindowActivityMonitor.GetActiveWindowAsync();
                }
            }
        }

        /// <summary>
        /// Returns true of a functional agent can be activated now
        /// </summary>
        /// <returns></returns>
        public bool CanActivateFunctionalAgent()
        {
            return !(_currentAgent is IFunctionalAgent);
        }

        /// <summary>
        /// Checks if a widget should be enabled or disabled. This
        /// depends on the context.  This function is invoked for
        /// each widget on the scanner, periodically.
        /// The call is forwarded to the active agent which actually handles
        /// it as it is the one that is intimately aware of the context
        /// </summary>
        /// <param name="arg">widget info</param>
        public void CheckCommandEnabled(CommandEnabledArg arg)
        {
            if (_currentAgent != null)
            {
                if (_currentAgent is IFunctionalAgent &&
                        ((FunctionalAgentBase)_currentAgent).IsClosing)
                {
                    Log.Debug("Functional agent is closing. returning");
                    return;
                }

                _currentAgent.CheckCommandEnabled(arg);
                if (!arg.Handled && _currentAgent.TextControlAgent != null)
                {
                    _currentAgent.TextControlAgent.CheckCommandEnabled(arg);
                }
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the agent that has the specified category
        /// </summary>
        /// <param name="category">category of the agent</param>
        /// <returns>agent, null not found</returns>
        public IApplicationAgent GetAgentByCategory(String category)
        {
            return _agentsCache.GetAgentByCategory(category);
        }

        /// <summary>
        /// Returns the agent that has the specified name
        /// </summary>
        /// <param name="name">name of the agent</param>
        /// <returns>agent, null if not found</returns>
        public IApplicationAgent GetAgentByName(String name)
        {
            return _agentsCache.GetAgentByName(name);
        }

        /// <summary>
        /// Returns the name of the currently active agent
        /// </summary>
        /// <returns>Name of the agent</returns>
        public String GetCurrentAgentName()
        {
            return (_currentAgent != null) ? _currentAgent.Name : String.Empty;
        }

        /// <summary>
        /// Returns a list of Agent objects.
        /// LoadExtensions must be called before this
        /// </summary>
        /// <returns>list</returns>
        public IEnumerable<object> GetExtensions()
        {
            if (_agentsCache == null)
            {
                return new List<object>();
            }

            return _agentsCache.GetExtensions();
        }

        /// <summary>
        /// Initializes the Agent manager.
        /// </summary>
        /// <param name="extensionDirs">directories to walk</param>
        /// <returns>true on success</returns>
        [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool Init(IEnumerable<String> extensionDirs)
        {
            if (_agentsCache == null)
            {
                return false;
            }

            _genericAppAgent = _agentsCache.GetAgent(GenericAppAgentName);
            if (_genericAppAgent == null)
            {
                _agentsCache.AddAgentByType(typeof(UnsupportedAppAgent));
                _genericAppAgent = _agentsCache.GetAgent(GenericAppAgentName);
            }

            _agentsCache.AddAgentByType(typeof(NullAgent));
            _nullAgent = _agentsCache.GetAgent(NullAgentName);

            _dialogAgent = _agentsCache.GetAgent(DialogControlAgentName);
            _menuControlAgent = _agentsCache.GetAgent(MenuControlAgentName);

            _textControlAgent = _genericAppAgent.TextControlAgent;

            WindowActivityMonitor.EvtFocusChanged += WindowActivityMonitor_EvtFocusChanged;

            return true;
        }

        /// <summary>
        /// Returns true if the current agent is the one specified
        /// by the agent name
        /// </summary>
        /// <param name="agentName">agent name to check for</param>
        /// <returns>true if it is</returns>
        public bool IsCurrentAgent(String agentName)
        {
            return (_currentAgent != null) && (String.Compare(_currentAgent.Name, agentName, true) == 0);
        }

        /// <summary>
        /// Recursively descends into each of the directories
        /// and loads the Agents (both application agents and
        /// functional agents) in there
        /// </summary>
        /// <param name="extensionDirs">directories to look into</param>
        /// <returns>true on success</returns>
        public bool LoadExtensions(IEnumerable<String> extensionDirs)
        {
            if (_agentsCache == null)
            {
                _agentsCache = new AgentsCache();
                _agentsCache.EvtAgentAdded += _agentsCache_EvtAgentAdded;
                return _agentsCache.Init(extensionDirs);
            }

            return true;
        }

        /// <summary>
        /// Invoked when a scanner closes. Informs the current agent that
        /// the scanner has closed
        /// </summary>
        /// <param name="panelClass">the name of the scanner that was closed</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnPanelClosed(String panelClass)
        {
            Log.Debug();
            Log.Debug("panelClass : " + panelClass);
            Log.Debug(" currentAgent: " + _currentAgent);
            if (_currentAgent != null)
            {
                var activityInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                _currentAgent.OnPanelClosed(panelClass, activityInfo);
            }
        }

        /// <summary>
        /// Pauses notifications for requests to display scanners.
        /// This essentially locks the curretly active scanner in place
        /// </summary>
        public void PausePanelChangeRequests()
        {
            _panelChangeNotifications.Hold();
        }

        /// <summary>
        /// During initialization of the application, call this
        /// after calling Init()
        /// </summary>
        /// <returns>true always</returns>
        public bool PostInit()
        {
            getKeyboardActuator();

            return true;
        }

        /// <summary>
        /// Removes a previously added ad-hoc agent
        /// </summary>
        /// <param name="handle">window handle for the agent</param>
        public void RemoveAgent(IntPtr handle)
        {
            IApplicationAgent agent = _agentsCache.GetAgent(handle);

            if (agent != null)
            {
                agent.EvtPanelRequest -= agent_EvtPanelRequest;
                _agentsCache.RemoveAgent(handle);
                if (agent == _currentAgent)
                {
                    _currentAgent = null;
                }
            }
        }

        /// <summary>
        /// Resumes notifications for requests to display scanners.
        /// The parameter controls whether this should detect which
        /// window is currently active.
        /// </summary>
        /// <param name="getActiveWindow"></param>
        public void ResumePanelChangeRequests(bool getActiveWindow = true)
        {
            _panelChangeNotifications.Release();
            if (getActiveWindow)
            {
                WindowActivityMonitor.GetActiveWindowAsync();
            }
        }

        /// <summary>
        /// Runs the specified command.  Passes it along to
        /// the active agent for processing
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <param name="handled">was it handled?</param>
        public void RunCommand(String command, ref bool handled)
        {
            if (_currentAgent != null)
            {
                Log.Debug("Calling runcommand agent : " + _currentAgent.Name + ", command: " + command);

                var cmd = (command[0] == '@') ? command.Substring(1) : command;
                _currentAgent.OnRunCommand(cmd, null, ref handled);
            }
        }

        /// <summary>
        /// Runs the command with the specified argument by passing it
        /// to the current agent to handle the command
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="arg">argument for the command</param>
        /// <param name="handled">was it handled?</param>
        public void RunCommand(String command, object arg, ref bool handled)
        {
            if (_currentAgent != null)
            {
                Log.Debug("Calling runcommand agent : " + _currentAgent.Name + ", command: " + command);
                String cmd = (command[0] == '@') ? command.Substring(1) : command;
                _currentAgent.OnRunCommand(cmd, arg, ref handled);
            }
        }

        /// <summary>
        /// Requests to display the contextual menu
        /// </summary>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public void ShowContextMenu()
        {
            // this will force an activateagent call which
            // in turn will get the contextual menu from the
            // active app agent

            _getContextMenu = true;
            WindowActivityMonitor.GetActiveWindow();
        }

        /// <summary>
        /// Displays the preferences dialog for the Agent manager.
        /// Displays a list of agents it has discovered
        /// </summary>
        public void ShowPreferencesAppAgents()
        {
            var agents = GetExtensions();

            var list = new List<object>();
            foreach (var agent in agents)
            {
                if (!(agent is IFunctionalAgent))
                {
                    var prefs = agent as ISupportsPreferences;

                    if (prefs.GetPreferences() != null || prefs.SupportsPreferencesDialog)
                    {
                        list.Add(agent);
                    }
                }
            }

            var categories = list.Select(agent => new PreferencesCategory(agent)).ToList();

            var form = new PreferencesCategorySelectForm
            {
                DisallowEnable = true,
                ShowEnable = false,
                PreferencesCategories = categories,
                Title = "Applications"
            };

            form.ShowDialog();

            form.Dispose();
        }

        /// <summary>
        /// Displays the preferences dialog for the Agent manager.
        /// Displays a list of functional agents it has discovered,
        /// only if they have custom dialog or settings to be changed
        /// </summary>
        public void ShowPreferencesFunctionalAgents()
        {
            var agents = GetExtensions();

            var list = new List<object>();
            foreach (var agent in agents)
            {
                if (agent is IFunctionalAgent)
                {
                    var prefs = agent as ISupportsPreferences;

                    if (prefs.GetPreferences() != null || prefs.SupportsPreferencesDialog)
                    {
                        list.Add(agent);
                    }
                }
            }

            var categories = list.Select(agent => new PreferencesCategory(agent)).ToList();

            var form = new PreferencesCategorySelectForm
            {
                DisallowEnable = true,
                ShowEnable = false,
                PreferencesCategories = categories,
                Title = "Tools"
            };

            form.ShowDialog();

            form.Dispose();
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        /// <summary>
        /// Event handler for when an agent is added to the cache.
        /// Subscribe to agent events
        /// </summary>
        /// <param name="agent">Agent that was added</param>
        private void _agentsCache_EvtAgentAdded(IApplicationAgent agent)
        {
            agent.EvtPanelRequest += agent_EvtPanelRequest;
            agent.EvtTextChanged += ApplicationAgent_EvtTextChanged;
            agent.EvtAgentClose += agent_EvtAgentClose;
        }

        /// <summary>
        /// Handler for mouse down. Raise an event to indicate this so
        /// the scanner can check if the mouse was clicked on it.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="mouseEventArgs">event args</param>
        private void _keyboardActuator_EvtMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            int hitCount = 0;

            if (EvtScannerHitTest != null)
            {
                var delegates = EvtScannerHitTest.GetInvocationList();

                hitCount += delegates.Cast<ScannerHitTest>().Count(hitTest => hitTest.Invoke(mouseEventArgs.X, mouseEventArgs.Y));
            }

            if (hitCount == 0)
            {
                EvtNonScannerMouseDown(this, mouseEventArgs);
            }
        }

        /// <summary>
        /// Activates the functional agent.
        /// The caller should to an 'await'
        /// on this to wait for it to complete.  This is because some
        /// functional agents return data and the caller has to wait
        /// for the functional agent to exit so it can get the data. Eg
        /// FileBrowser agent that returns a filename
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        private async Task activateAgent(IFunctionalAgent agent)
        {
            if (!(agent is FunctionalAgentBase))
            {
                return;
            }

            var form = new Form { WindowState = FormWindowState.Minimized, Visible = false, ShowInTaskbar = false };
            form.Load += form_Load;
            form.Show();

            var funcAgent = (FunctionalAgentBase)agent;
            Task task = Task.Factory.StartNew(() =>
            {
                Log.Debug("Calling funcAgent.activate: " + agent.Name);
                //funcAgent.IsClosing = false;
                funcAgent.ExitCommand = null;

                form.Invoke(new MethodInvoker(delegate
                {
                    Context.AppPanelManager.NewStack();
                    Log.Debug("Before activate " + funcAgent.Name);
                    funcAgent.Activate();
                    Log.Debug("Returned from activate " + funcAgent.Name + ", calling closestack");
                    Context.AppPanelManager.CloseStack();
                }));

                // This event is triggered by the functional agent when it exits
                Log.Debug("Waiting on CloseEvent...: " + agent.Name);
                funcAgent.CloseEvent.WaitOne();
                Log.Debug("Returned from CloseEvent: " + agent.Name);

                funcAgent.PostClose();
            });

            Log.Debug("Before await task");
            await task;
            Log.Debug("After await task");

            Windows.CloseForm(form);
        }

        /// <summary>
        /// Activates an application agent depending on the context.
        /// The monitorInfo parameter has all the information to make
        /// this decision. Depending on the active foreground process,
        /// the appropriate agent is activated.  if there is no dedicated
        /// agent for the process, the generic agent is used.
        /// If a dialog the foreground window, the dialog agent is activated.
        /// If a menu has been activated, the menu agent is activated
        /// </summary>
        /// <param name="monitorInfo">Foreground window/process info</param>
        private void activateAppAgent(WindowActivityMonitorInfo monitorInfo)
        {
            if (inActivateAppAgent)
            {
                Log.Debug("Already inside. returning");
                return;
            }

            Log.Debug("Before syncsetagent");
            lock (_syncActivateAgent)
            {
                inActivateAppAgent = true;
                Log.Debug("After syncsetagent");
                try
                {
                    bool handled = false;

                    Log.Debug(monitorInfo.ToString());

                    // did a request for displaying the contextual
                    // menu come in?  If so handle it.

                    bool getContextMenu = _getContextMenu;

                    Log.Debug("getContextMenu: " + getContextMenu);

                    _getContextMenu = false;

                    String processName = monitorInfo.FgProcess.ProcessName;

                    // first check if there is an ad-hoc agent, if so,
                    // activate it
                    Log.Debug("Looking for adhoc agent for " + monitorInfo.FgHwnd);
                    IApplicationAgent agent = _agentsCache.GetAgent(monitorInfo.FgHwnd);
                    if (agent == null)
                    {
                        // check if a dialog or menu is active
                        Log.Debug("Adhoc agent not present for " + monitorInfo.FgHwnd);

                        IntPtr parent = User32Interop.GetParent(monitorInfo.FgHwnd);
                        if (parent != IntPtr.Zero)
                        {
                            if (EnableContextualMenusForDialogs &&
                                (String.Compare(processName, _currentProcessName, true) != 0) &&
                                isDialog(monitorInfo))
                            {
                                Log.Debug("Fg window is a dialog.  Setting agent to dialog agent");

                                agent = _dialogAgent;
                            }
                            else if (EnableContextualMenusForMenus && isMenu(monitorInfo))
                            {
                                Log.Debug("Fg window is a menu.  Setting agent to menu agent");
                                agent = _menuControlAgent;
                            }
                        }

                        if (agent == null)
                        {
                            if (Windows.IsMinimized(monitorInfo.FgHwnd))
                            {
                                Log.Debug("Window is minimized. Use generic agent");
                                agent = _genericAppAgent;
                            }
                            else
                            {
                                // check if there is a dedicated agent for this process
                                Log.Debug("Getting agent for " + processName);
                                agent = _agentsCache.GetAgent(monitorInfo.FgProcess);
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("Adhoc agent IS present for " + monitorInfo.FgHwnd);
                    }

                    Log.Debug("Current agent: " + ((_currentAgent != null) ?
                                                    _currentAgent.Name : "null") +
                                                    ", agent:  " + ((agent != null) ? agent.Name : "null"));

                    // if there is an agent switch, query the current agent
                    // if it is OK to switch. Some agents may not allow
                    // the switch
                    if (_currentAgent != null && _currentAgent != agent)
                    {
                        bool allowSwitch = _currentAgent.QueryAgentSwitch(agent);
                        Log.Debug("CurrentAgent is " + _currentAgent.Name + ", queryAgentSwitch: " + allowSwitch);
                        if (!allowSwitch)
                        {
                            _currentAgent.OnFocusChanged(monitorInfo, ref handled);
                            _textControlAgent = _currentAgent.TextControlAgent;
                            return;
                        }

                        _currentAgent.OnFocusLost();
                    }

                    // if there was a request to display
                    // contextual menu, do so
                    if (getContextMenu)
                    {
                        if (agent == null)
                        {
                            agent = _genericAppAgent;
                        }

                        Log.Debug("agent : " + agent.Name);
                        agent.OnContextMenuRequest(monitorInfo);
                        return;
                    }

                    // Inform the new agent about the current
                    // focused element so it can display the scanner that
                    // is appropriated for the context.
                    if (agent != null)
                    {
                        Log.Debug("Trying agent " + agent.Name);
                        agent.OnFocusChanged(monitorInfo, ref handled);
                        Log.Debug("Returned from agent.OnFOcus");
                    }

                    // If we have reached here, it means there was no
                    // agent. Just use the default generic agent. See
                    // if it will handle it
                    if (!handled)
                    {
                        Log.Debug("Did not find agent for " + processName + ". trying generic app agent");
                        Log.Debug("_genericAppAgent is " + ((_genericAppAgent != null) ? "not null" : "null"));
                        agent = _genericAppAgent;
                        try
                        {
                            if (agent != null)
                            {
                                agent.OnFocusChanged(monitorInfo, ref handled);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                        }
                    }

                    // even the generic agent refused.  Use the null agent
                    // as the last resort
                    Log.Debug("handled " + handled);
                    if (!handled)
                    {
                        Log.Debug("generic app agent refused. Using null agent");
                        agent = _nullAgent;
                        agent.OnFocusChanged(monitorInfo, ref handled);
                    }

                    updateCurrentAgentAndNotify(agent);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
                finally
                {
                    inActivateAppAgent = false;
                }
            }

            Log.Debug("Return");
        }

        /// <summary>
        /// Handler for when an agents exits.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void agent_EvtAgentClose(object sender, AgentCloseEventArgs e)
        {
            if (sender is IApplicationAgent)
            {
                var agent = (IApplicationAgent)sender;
                if (agent.Parent == null)
                {
                    return;
                }

                IApplicationAgent parent = agent.Parent;
                setAgent(parent);
                parent.OnResume();
                agent.Parent = null;
            }
        }

        /// <summary>
        /// Event handler for request to display a scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        private void agent_EvtPanelRequest(object sender, PanelRequestEventArgs arg)
        {
            Log.Debug("Panel request for " + arg.PanelClass);
            triggerPanelRequest(sender, arg);
        }

        /// <summary>
        /// Event handler for when text or caret position changed
        /// in the text window.  Notify subscribers.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ApplicationAgent_EvtTextChanged(object sender, TextChangedEventArgs e)
        {
            notifyTextChanged(e.TextInterface);
        }

        /// <summary>
        /// Don't context switch to the app agent that
        /// handles the foreground process other than the agent for the current
        /// assembly.  Either use the agent  responsible for the executing assembly or use
        /// the null agent.
        /// </summary>
        /// <param name="monitorInfo">fg process info</param>
        private void disallowContextSwitch(WindowActivityMonitorInfo monitorInfo)
        {
            bool handled = false;

            // check if there is an adhoc agent for the current window
            // if not, check if there is an agent for the current process
            var agent = _agentsCache.GetAgent(monitorInfo.FgHwnd);
            if (agent == null)
            {
                if (String.Compare(monitorInfo.FgProcess.ProcessName, _currentProcessName, true) == 0)
                {
                    agent = _agentsCache.GetAgent(_currentProcessName) ?? DefaultAgentForContextSwitchDisable;
                }
            }

            if (agent == null)
            {
                agent = DefaultAgentForContextSwitchDisable;
            }

            if (agent == null)
            {
                agent = _genericAppAgent;
            }

            Log.Debug("agent : " + agent.Name);

            if (_getContextMenu)
            {
                _getContextMenu = false;

                agent.OnContextMenuRequest(monitorInfo);
                return;
            }

            if (agent != _currentAgent && _currentAgent != null)
            {
                _currentAgent.OnFocusLost();
            }

            _currentAgent = agent;
            agent.OnFocusChanged(monitorInfo, ref handled);
        }

        /// <summary>
        /// Disposes and releases resources
        /// </summary>
        /// <param name="disposing">disposed before?</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    if (_agentsCache != null)
                    {
                        _agentsCache.Dispose();
                    }

                    if (_keyboardActuator != null)
                    {
                        _keyboardActuator.EvtKeyDown -= keyboardActuator_EvtKeyDown;
                        _keyboardActuator.EvtKeyUp -= keyboardActuator_EvtKeyUp;
                        _keyboardActuator = null;
                    }
                }
                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Invisible form loader for activating functional agent.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void form_Load(object sender, EventArgs e)
        {
            if (sender is Form)
            {
                ((Form)sender).Hide();
            }
        }

        /// <summary>
        /// Gets the keyboard actuator object. Also subscribes to events
        /// from the actuator
        /// </summary>
        private void getKeyboardActuator()
        {
            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyDown += keyboardActuator_EvtKeyDown;
                _keyboardActuator.EvtKeyUp += keyboardActuator_EvtKeyUp;
                _keyboardActuator.EvtMouseDown += _keyboardActuator_EvtMouseDown;
            }
        }

        /// <summary>
        /// Checks if the focused window is actually a dialog
        /// </summary>
        /// <param name="monitorInfo">focused element information</param>
        /// <returns>true if it is</returns>
        private bool isDialog(WindowActivityMonitorInfo monitorInfo)
        {
            if (monitorInfo.FgHwnd == IntPtr.Zero)
            {
                return false;
            }

            bool retVal = false;

            AutomationElement window = AutomationElement.FromHandle(monitorInfo.FgHwnd);
            object objPattern;
            Log.Debug("controltype: " + window.Current.ControlType.ProgrammaticName);

            if (Equals(window.Current.ControlType, ControlType.Menu))
            {
                Log.Debug("**** controltype: IT IS MENU");
                retVal = true;
            }
            else if (window.TryGetCurrentPattern(WindowPattern.Pattern, out objPattern))
            {
                var windowPattern = objPattern as WindowPattern;
                retVal = (!windowPattern.Current.CanMinimize && !windowPattern.Current.CanMaximize) || windowPattern.Current.IsModal;
                Log.Debug("CanMinimize: " + windowPattern.Current.CanMinimize + ", canMaximize: " + windowPattern.Current.CanMaximize + ", isModal: " + windowPattern.Current.IsModal);
                Log.Debug("retVal: " + retVal);

                retVal = retVal && !Windows.IsMinimized(monitorInfo.FgHwnd);
            }

            Log.Debug("returning " + retVal);

            return retVal;
        }

        /// <summary>
        /// Checks if the focused window is actually a menu
        /// </summary>
        /// <param name="monitorInfo">focused element information</param>
        /// <returns>true if it is</returns>
        private bool isMenu(WindowActivityMonitorInfo monitorInfo)
        {
            return (monitorInfo.FocusedElement.Current.ControlType.ProgrammaticName == "ControlType.AppMenu" ||
                    monitorInfo.FocusedElement.Current.ControlType.ProgrammaticName == "ControlType.MenuItem");
        }

        /// <summary>
        /// A keydown event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="keyEventArgs">event args</param>
        private void keyboardActuator_EvtKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (TextController.IsKeyDown(Keys.LMenu) ||
                TextController.IsKeyDown(Keys.RMenu) ||
                TextController.IsKeyDown(Keys.LControlKey) ||
                TextController.IsKeyDown(Keys.RControlKey))
            {
                CurrentEditingMode = EditingMode.Edit;
            }
            else if (TextUtils.IsPrintable(keyEventArgs.KeyCode))
            {
                CurrentEditingMode = EditingMode.TextEntry;
            }
            else
            {
                CurrentEditingMode = EditingMode.Edit;
            }

            Log.Debug("_currentEditingMode: " + _currentEditingMode);

            if (_currentAgent != null && _currentAgent.TextControlAgent != null)
            {
                _currentAgent.TextControlAgent.OnKeyDown(keyEventArgs);
            }
        }

        /// <summary>
        /// Handler for key up event.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="keyEventArgs">event args</param>
        private void keyboardActuator_EvtKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (_currentAgent != null && _currentAgent.TextControlAgent != null)
            {
                _currentAgent.TextControlAgent.OnKeyUp(keyEventArgs);
            }
        }

        /// <summary>
        /// Notifies that something changed in the active text window -
        /// either the text changed or the caret moved. This is a tricky
        /// thing to handle The event can be raised while this function
        /// is still executing due to a previous event.  We must guard
        /// against. Subsequent calls must be blocked until the function
        /// has exited.
        /// </summary>
        /// <param name="textInterface">The source of the event</param>
        /// <returns>true if successful</returns>
        private bool notifyTextChanged(ITextControlAgent textInterface)
        {
            uint threadId = GetCurrentThreadId();
            Log.Debug("Entered notifyTextChanged: " + threadId);

            // Use the semaphore to see if this is blocked
            if (TextChangedNotifications.OnHold())
            {
                Log.Debug("NotificationsOnHold.  Exit ThreadID: " + threadId);
                return false;
            }

            // try to acquire a lock
            if (!tryLock(_syncTextChangeNotifyObj))
            {
                Log.Debug("_lock is true. returning on thread : " + threadId);
                return false;
            }

            try
            {
                // this is required as _syncTextChangeNotifyObj lock will
                // only work if different threads are trying to enter.
                // The following statement is required to block against
                // the same thread trying to re-enter the function
                if (_notifyTextChangedLock)
                {
                    Log.Debug("REENTRANCY DETECTED. RETURNING");
                    return true;
                }

                if (textInterface != _textControlAgent)
                {
                    Log.Debug("event source is not the current agent. returning...");
                    return false;
                }

                _notifyTextChangedLock = true;

                try
                {
                    if (EvtTextChanged != null)
                    {
                        Log.Debug("Calling EvtTextChanged");
                        EvtTextChanged(this, EventArgs.Empty);
                        Log.Debug("Returned from EvtTextChanged");
                    }
                    else
                    {
                        Log.Debug("EvtTextChanged is null");
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }

                _notifyTextChangedLock = false;

                Log.Debug("Exit End of function ThreadID: " + threadId);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
            finally
            {
                release(_syncTextChangeNotifyObj);
            }

            Log.Debug("Returning");
            return true;
        }

        /// <summary>
        /// Event triggered when the active foreground window changes
        /// or if focus changes within the foreground window
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        private void onFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            if (monitorInfo.FgHwnd == IntPtr.Zero)
            {
                Log.Debug("hWnd is null");
                return;
            }

            Log.Debug(monitorInfo.ToString());

            //Log.Debug(" hwnd: " + hWnd + " Title:  [" + title + "] process: " + process.ProcessName +
            //      ". focusedElement: [" +
            //  ((focusedElement != null) ? focusedElement.Current.ClassName : "null") + "]");

            bool handled = false;

            if (_currentAgent is IFunctionalAgent)
            {
                var functionalAgent = _currentAgent as IFunctionalAgent;

                Log.Debug(_currentAgent.Name + ", IsActive: " + functionalAgent.IsActive + ", IsClosing: " + functionalAgent.IsClosing);

                if (functionalAgent.IsActive && !functionalAgent.IsClosing)
                {
                    _currentAgent.OnFocusChanged(monitorInfo, ref handled);
                }

                if (EvtFocusChanged != null)
                {
                    EvtFocusChanged(this, new FocusChangedEventArgs(monitorInfo));
                }

                if (!functionalAgent.IsClosing)
                {
                    return;
                }
            }

            if (EnableAppAgentContextSwitch)
            {
                activateAppAgent(monitorInfo);
            }
            else
            {
                disallowContextSwitch(monitorInfo);
            }

            if (_currentAgent != null)
            {
                Log.Debug("CurrentAgent is " + _currentAgent.GetType());
            }

            if (EvtFocusChanged != null)
            {
                EvtFocusChanged(this, new FocusChangedEventArgs(monitorInfo));
            }
            else
            {
                Log.Debug("EVTFocusChanged is null!");
            }
        }

        /// <summary>
        /// Release previously acquired lock
        /// </summary>
        /// <param name="sync">sync object</param>
        private void release(object sync)
        {
            try
            {
                Monitor.Exit(sync);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Updates _currentAgent to the agent specified
        /// </summary>
        /// <param name="agent">sets the agent</param>
        private void setAgent(IApplicationAgent agent)
        {
            Log.Debug("Setting agent to " + ((agent != null) ? agent.Name : "null"));
            _currentAgent = agent;
        }

        /// <summary>
        /// TextChangedNotifications semaphore was released. Notify
        /// once more that text changed.  This will take care of all
        /// the calls that returned while the notifyTextChanged
        /// function was executing.
        /// </summary>
        private void TextChangedNotifications_EvtUnlocked()
        {
            uint threadId = GetCurrentThreadId();
            Log.Debug("Enter ID. calling notifytextchanged on threadID: " + threadId);

            int ii = 0;
            while (true)
            {
                Log.Debug("UNLOCKED!  Calling NotifyTextChanged");
                if (notifyTextChanged(_textControlAgent))
                {
                    Log.Debug("Returned from NotifyTextChanged()");
                    break;
                }

                Log.Debug("wait for _lock to release " + ii);

                Application.DoEvents();
                if (ii > 500)
                {
                    Log.Debug("TIMED OUT WAITING FOR LOCK");
                    break;
                }

                ii++;
            }

            Log.Debug("Leave ThreadID: " + threadId);
        }

        /// <summary>
        /// Raises an event to indicate that there was a request to
        /// display a scanner. The arg parameter contains info about
        /// which scanner to display
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">panel request info</param>
        private void triggerPanelRequest(object sender, PanelRequestEventArgs arg)
        {
            Log.Debug("PanelClass: " + arg.PanelClass);
            if (_panelChangeNotifications.OnHold())
            {
                Log.Debug("Panel change request paused.  will not change panel");
                return;
            }

            if (EvtPanelRequest != null)
            {
                EvtPanelRequest(sender, arg);
            }
            else
            {
                Log.Debug("EvtPanelrequest is null");
            }
        }

        /// <summary>
        /// Try to acquire a lock on the synch object
        /// </summary>
        /// <param name="sync">sync object</param>
        /// <returns>true if lock was acquired</returns>
        private bool tryLock(object sync)
        {
            return Monitor.TryEnter(sync);
        }

        /// <summary>
        /// Sets the current agent to the specified one and raises
        /// a text changed event.
        /// </summary>
        /// <param name="agent">agent to set as the current agent</param>
        private void updateCurrentAgentAndNotify(IApplicationAgent agent)
        {
            _textControlAgent = agent.TextControlAgent;
            setAgent(agent);
            notifyTextChanged(_textControlAgent);
        }

        /// <summary>
        /// Focus changed on the desktop to a different window or
        /// to a different control in the active window
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        private void WindowActivityMonitor_EvtFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            onFocusChanged(monitorInfo);
        }

        private bool inActivateAppAgent
        {
            get
            {
                return Interlocked.Read(ref _inActivateAppAgent) == 1;
            }
            set
            {
                Interlocked.Exchange(ref _inActivateAppAgent, Convert.ToInt64(value));
            }
        }
    }
}