////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Base class for all application agents.  Implements some of the
    /// methods in IApplicationAgent
    /// </summary>
    public abstract class AgentBase : IApplicationAgent
    {
        /// <summary>
        /// Used to invoke methods/properties in the agent
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Default text interface
        /// </summary>
        private readonly ITextControlAgent _nullTextInterface = new TextControlAgentBase();

        /// <summary>
        /// Which features does this support?
        /// </summary>
        private readonly String[] _supportedCommands = { "CmdContextMenu" };

        /// <summary>
        /// Has this been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Name of the agent
        /// </summary>
        private String _name;

        /// <summary>
        /// Text control agent object
        /// </summary>
        private ITextControlAgent _textInterface;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected AgentBase()
        {
            _name = Descriptor.Name;
            _invoker = new ExtensionInvoker(this);
        }

        /// <summary>
        /// Event raised when an agent closed
        /// </summary>
        public event AgentClose EvtAgentClose;

        /// <summary>
        /// Event raised to request for activating a panel
        /// </summary>
        public event PanelRequest EvtPanelRequest;

        /// <summary>
        /// Event raised to indicate text changed in the app window
        /// </summary>
        public event TextChangedDelegate EvtTextChanged;

        /// <summary>
        /// Gets the descriptor object for this agent
        /// </summary>
        public virtual IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets name of the agent
        /// </summary>
        public virtual String Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent agent
        /// </summary>
        public IApplicationAgent Parent { get; set; }

        /// <summary>
        /// Gets the list of processes supported by this agent.
        /// Override this function.
        /// </summary>
        public virtual IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new AgentProcessInfo[] { }; }
        }

        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public virtual bool SupportsPreferencesDialog
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the text control agent object
        /// </summary>
        public ITextControlAgent TextControlAgent
        {
            get { return _textInterface ?? _nullTextInterface; }
        }

        /// <summary>
        /// Invoked to check if a command  should be enabled or not.  This depends
        /// on the context and the agent can decide whether the command should
        /// be enabled or not.  The widget's subclass field contains the context
        /// and should be used as the clue to set the widget state.  For instance
        /// if the talk window is already empty, disable the "Clear" button
        /// </summary>
        /// <returns>true to enable, false to disable</returns>
        public virtual void CheckCommandEnabled(CommandEnabledArg arg)
        {
            if (_supportedCommands.Contains(arg.Command))
            {
                arg.Handled = true;
                arg.Enabled = true;
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this to returns the default preferences for the agent
        /// </summary>
        /// <returns>default preferences</returns>
        public virtual IPreferences GetDefaultPreferences()
        {
            return null;
        }

        /// <summary>
        /// Returns invoker used to access methods and properties through
        /// reflection
        /// </summary>
        /// <returns></returns>
        public virtual ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Returns the preferences object for the agent
        /// </summary>
        /// <returns>preferences object</returns>
        public virtual IPreferences GetPreferences()
        {
            return null;
        }

        /// <summary>
        /// Implement this to display a contexutal menu for
        /// the currently active process
        /// </summary>
        /// <param name="monitorInfo">Info  about the active process/window</param>
        public abstract void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Invoked whenever focus changes in the target application
        /// window - either when the active window changes or when the
        /// focus within a window changes from one control to another.
        /// eg, use tabs between edit fields
        /// </summary>
        /// <param name="monitorInfo">Contains all the info about the control in focus</param>
        /// <param name="handled">Set this to true if the agent handled it.</param>
        public virtual void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            handled = false;
        }

        /// <summary>
        /// Invoked when the agent is de-activated.  Occurs for e.g. when
        /// the active window changes focus and the agent manager switches agents.
        /// </summary>
        public virtual void OnFocusLost()
        {
        }

        /// <summary>
        /// Invoked when the currently active scanner is closed
        /// </summary>
        /// <param name="panelClass">name/class of the scanner</param>
        /// <param name="monitorInfo">Active focused window info</param>
        public virtual void OnPanelClosed(String panelClass, WindowActivityMonitorInfo monitorInfo)
        {
        }

        /// <summary>
        /// Override this to pause the agent
        /// </summary>
        public virtual void OnPause()
        {
        }

        /// <summary>
        /// Override this to resume the agent
        /// </summary>
        public virtual void OnResume()
        {
        }

        /// <summary>
        /// Override this to handle a request to run a command.  Set handled
        /// to true if the command was handled, false otherwise
        /// </summary>
        /// <param name="command">The command verb</param>
        /// <param name="arg">optional arguments</param>
        /// <param name="handled">set appropriately</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public virtual void OnRunCommand(String command, object arg, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "OpenFile":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.O);
                    break;

                case "NewFile":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.N);
                    break;

                case "SaveFile":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.S);
                    break;

                case "SaveFileAs":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F);
                    AgentManager.Instance.Keyboard.Send(Keys.A);
                    break;

                case "CmdFind":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.H);
                    break;

                case "DesktopSearch":
                    AgentManager.Instance.Keyboard.Send(Keys.LWin, Keys.F);
                    break;

                case "WindowsStartMenu":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Escape);
                    break;

                case "PageUp":
                    AgentManager.Instance.Keyboard.Send(Keys.PageUp);
                    break;

                case "PageDown":
                    AgentManager.Instance.Keyboard.Send(Keys.PageDown);
                    break;

                case "Up":
                    AgentManager.Instance.Keyboard.Send(Keys.Up);
                    break;

                case "Down":
                    AgentManager.Instance.Keyboard.Send(Keys.Down);
                    break;

                case "Left":
                    AgentManager.Instance.Keyboard.Send(Keys.Left);
                    break;

                case "Right":
                    AgentManager.Instance.Keyboard.Send(Keys.Right);
                    break;

                case "CmdMouseScanner":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var panelArg = new PanelRequestEventArgs(PanelClasses.Mouse, monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };
                        showPanel(this, panelArg);
                    }

                    break;

                case "CmdCursorScanner":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var panelArg = new PanelRequestEventArgs(PanelClasses.Cursor, monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };
                        showPanel(this, panelArg);
                    }

                    break;

                default:
                    handled = false;
                    break;
            }
        }

        /// <summary>
        /// Invoked before the agent is deactivated. Override this and return true if it is
        /// OK to deactivate the agent, false otherwise
        /// </summary>
        /// <param name="newAgent">Agent that will be activated</param>
        /// <returns>true/false</returns>
        public virtual bool QueryAgentSwitch(IApplicationAgent newAgent)
        {
            return true;
        }

        /// <summary>
        /// Shows the preferences dialog
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool ShowPreferencesDialog()
        {
            return true;
        }

        /// <summary>
        /// Checks if the command should to be enabled or not. Check
        /// our supported features list.
        /// </summary>
        /// <param name="arg">widget info</param>
        protected void checkCommandEnabled(String[] supportedCommands, CommandEnabledArg arg)
        {
            if (supportedCommands.Contains(arg.Command))
            {
                arg.Handled = true;
                arg.Enabled = true;
            }
        }

        /// <summary>
        /// Raises an event to indicate the agent is deactivated
        /// </summary>
        /// <param name="e">event args</param>
        protected void notifyAgentClose(AgentCloseEventArgs e)
        {
            if (EvtAgentClose != null)
            {
                EvtAgentClose(this, e);
            }
        }

        /// <summary>
        /// Handle disposal
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Sets the text control agent to the specifed one, null agent
        /// if NULL
        /// </summary>
        /// <param name="textInterface">Text interface to set</param>
        protected void setTextInterface(TextControlAgentBase textInterface = null)
        {
            if (_textInterface != null)
            {
                Log.Debug("Disposing " + _textInterface.GetType().Name);
                _textInterface.Dispose();
            }

            Log.Debug("Setting textinterface to " + ((textInterface != null) ? textInterface.GetType().Name : "null"));

            _textInterface = textInterface ?? _nullTextInterface;
            AgentManager.Instance.TextControlAgent = _textInterface;
        }

        /// <summary>
        /// Raises an event to request for displaying the specified
        /// scanner in 'arg'.
        /// </summary>
        /// <param name="sender">event source</param>
        /// <param name="arg">event arg</param>
        protected void showPanel(object sender, PanelRequestEventArgs arg)
        {
            if (EvtPanelRequest != null)
            {
                EvtPanelRequest.Invoke(this, arg);
            }
        }

        /// <summary>
        /// Call this function to raise events to indicated that something changed
        /// in the text window either due to editing or due to cursor movement. Event
        /// raised is synchronous
        /// </summary>
        /// <param name="textInterface">text agent</param>
        protected void triggerTextChanged(ITextControlAgent textInterface)
        {
            if (EvtTextChanged != null)
            {
                EvtTextChanged(this, new TextChangedEventArgs(textInterface));
            }
        }

        /// <summary>
        /// Call this function to raise events to indicated that something changed
        /// in the text window either due to editing or due to cursor movement. Event
        /// raised is asynchronous
        /// </summary>
        /// <param name="textInterface">text agent</param>
        protected void triggerTextChangedAsync(ITextControlAgent textInterface)
        {
            if (EvtTextChanged != null)
            {
                EvtTextChanged.BeginInvoke(this, new TextChangedEventArgs(textInterface), null, null);
            }
        }

        /// <summary>
        /// Dispose object
        /// </summary>
        /// <param name="disposing">dispose if true</param>
        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    if (_nullTextInterface != null)
                    {
                        _nullTextInterface.Dispose();
                    }

                    OnDispose();
                }
                // Release unmanaged resources.
            }

            _disposed = true;
        }
    }
}