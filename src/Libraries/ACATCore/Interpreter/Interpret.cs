////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ACAT.Lib.Core.Interpreter
{
    /// <summary>
    /// Interprets the PCode.  Uses a function table that maps
    /// the string form of a function name (aka verb) with the
    /// corresponding action.  Executes the action with the
    /// optional list of supplied arguments.  Note that WHAT the
    /// action does is not implemented here.  It uses an
    /// event model to notify subscribes of which
    /// action to execute.  The event handler in the
    /// subscriber does the actual execution.
    ///
    /// Each action verb function triggers an event indicating
    /// which function was executed and the list of arguments
    /// is passed as a parameter to the event.  That's how
    /// subscribers to the event are notified.
    /// </summary>
    public class Interpret
    {
#pragma warning disable IDE0051
        /// <summary>
        /// The XML file has a Scripts section where script
        /// functions can be defined and resused elsewhere
        /// in the XML
        /// </summary>
        private Scripts _scripts;

        /// <summary>
        /// "Actuates" action associated with a widget.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Actuate(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Play beep sound
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Beep(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Close the scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Close(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Highlights or Unhighlights the widget
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Highlight(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Highlights or Unhighlights the widget
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void HighlightSelected(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Launch an application
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Launch(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Run a script.  Triggered when the script name is actually
        /// a reference
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Run(object sender, InterpreterRunEventArgs e);

        /// <summary>
        /// Delegate for the hook function to the run command
        /// </summary>
        /// <param name="args">run command arguments</param>
        /// <param name="handled">was it handled?</param>
        public delegate void RunCommandHook(InterpreterRunEventArgs args, ref bool handled);

        /// <summary>
        /// Selects a widget.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Select(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Shows a scanner as a popup
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void ShowPopup(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Stop animation
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Stop(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Indicates a transition to an animiation.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Transition(object sender, InterpreterEventArgs e);

        /// <summary>
        /// Event triggered for the hook for the run command
        /// </summary>
        public static event RunCommandHook EvtRunCommandHook;

        /// <summary>
        /// Raised to actuate a widget
        /// </summary>
        public event Actuate EvtActuateNotify;

        /// <summary>
        /// Raised to play a beep sound
        /// </summary>
        public event Beep EvtBeep;

        /// <summary>
        /// Raised to close the active scanner
        /// </summary>
        public event Close EvtCloseNotify;

        /// <summary>
        /// Raised to highlight a widget
        /// </summary>
        public event Highlight EvtHighlightNotify;

        /// <summary>
        /// Raised to highlight/unhighlight a selected widget
        /// </summary>
        public event Highlight EvtHighlightSelectedNotify;

        /// <summary>
        /// Raised to launch an application
        /// </summary>
        public event Launch EvtLaunchNotify;

        /// <summary>
        /// Raised to run a command
        /// </summary>
        public event Run EvtRun;

        /// <summary>
        /// Raised to select a widget
        /// </summary>
        public event Select EvtSelectNotify;

        /// <summary>
        /// Raised to show a scanner as popup
        /// </summary>
        public event ShowPopup EvtShowPopup;

        /// <summary>
        /// Raised to stop the current animation
        /// </summary>
        public event Stop EvtStopNotify;

        /// <summary>
        /// Raised to transition to a new animation sequence
        /// </summary>
        public event Transition EvtTransitionNotify;

        /// <summary>
        /// Executes the all the actions in the pCode specified
        /// </summary>
        /// <param name="pCode">pCode containing the list of actions to perform</param>
        /// <returns>true on success, false otherwise</returns>
        public bool Execute(PCode pCode)
        {
            if (!pCode.HasCode())
            {
                return true;
            }

            // step through the list of action verbs
            foreach (ActionVerb actionVerb in pCode.ActionVerbList)
            {
                Execute(actionVerb);
            }

            return true;
        }

        /// <summary>
        /// Executes the action for the specified action verb. It
        /// contains one function with arguments
        /// </summary>
        /// <param name="actionVerb">action to perform</param>
        /// <returns>true on success</returns>
        public bool Execute(ActionVerb actionVerb)
        {
            bool retVal = true;
            try
            {
                // use reflection to get the method to execute
                MethodInfo mi = GetType().GetMethod(actionVerb.Action, BindingFlags.NonPublic | BindingFlags.Instance);
                if (mi != null)
                {
                    mi.Invoke(this, new object[] { actionVerb.ArgList });
                }
                else
                {
                    Log.Debug("Error executing verb " + actionVerb.Action + ". Mi is null");
                }
            }
            catch (Exception e)
            {
                Log.Debug("Error executing verb " + actionVerb.Action + ". Exception: " + e.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Loads scripts from the XML file
        /// </summary>
        /// <param name="configFile">Name of the xml file</param>
        /// <returns>true on success</returns>
        public bool LoadScripts(String configFile)
        {
            _scripts = new Scripts();
            return _scripts.Load(configFile);
        }

        /// <summary>
        /// Triggers an event to actuate a widget
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool actuate(List<String> args)
        {
            EvtActuateNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        /// <summary>
        /// Plays a beep sound
        /// </summary>
        /// <param name="args">argument list</param>
        /// <returns>true on success</returns>
        private bool beep(List<String> args)
        {
            EvtBeep(this, new InterpreterEventArgs(args));
            return true;
        }

        /// <summary>
        /// Close scanner window
        /// </summary>
        /// <param name="args">optional arguments</param>
        /// <returns>true on success</returns>
        private bool close(List<String> args)
        {
            EvtCloseNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        /// <summary>
        /// Triggers an event to highlight a widget
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool highlight(List<String> args)
        {
            EvtHighlightNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        /// <summary>
        /// Triggers an event to highlight a selected widget
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool highlightSelected(List<String> args)
        {
            EvtHighlightSelectedNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        /// <summary>
        /// Launch an external application
        /// </summary>
        /// <param name="args">Arguments - name of app</param>
        /// <returns>true on success</returns>
        private bool launch(List<String> args)
        {
            EvtLaunchNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        /// <summary>
        /// Notifies the subscribers of the hook event that a run command
        /// was encountered.  Give them a chance to act on it first
        /// </summary>
        /// <param name="args">run command args</param>
        /// <param name="handled">did any of them handle it?</param>
        private void notifyRunCommandHookSubscribers(InterpreterRunEventArgs args, ref bool handled)
        {
            if (EvtRunCommandHook == null)
            {
                return;
            }

            var delegates = EvtRunCommandHook.GetInvocationList();
            foreach (var del in delegates)
            {
                var hookDelegate = (RunCommandHook)del;
                hookDelegate.Invoke(args, ref handled);
                if (handled)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Runs scripts through the "run" command.  Each element
        /// in the args list is a reference to either a script name
        /// or the command itself.  If an arg begins with the '@' symbol,
        /// it is the name of a scirpt otherwise, it;s a command
        /// </summary>
        /// <param name="args">argument list</param>
        /// <returns>true on success</returns>
        private bool run(List<String> args)
        {
            if (args.Count == 0)
            {
                return false;
            }

            foreach (String scriptName in args)
            {
                if (scriptName[0] != '@')
                {
                    if (_scripts.ContainsKey(scriptName))
                    {
                        var pCode = (PCode)_scripts[scriptName];
                        if (pCode != null)
                        {
                            return Execute(pCode);
                        }
                    }
                    else
                    {
                        EvtRun(this, new InterpreterRunEventArgs(scriptName));
                    }
                }
                else
                {
                    // first notify the hook subscribers.  If none of them handled it
                    // then invoke the event

                    bool handled = false;
                    notifyRunCommandHookSubscribers(new InterpreterRunEventArgs(scriptName), ref handled);

                    if (!handled)
                    {
                        EvtRun(this, new InterpreterRunEventArgs(scriptName));
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Triggers an event to select a widget
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool select(List<String> args)
        {
            EvtSelectNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        private bool showPopup(List<String> args)
        {
            EvtShowPopup?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        /// <summary>
        /// Stops animation
        /// </summary>
        /// <returns>true on success</returns>
        private bool stop(List<String> args)
        {
            EvtStopNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }

        /// <summary>
        /// Triggers an event to transition animation
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool transition(List<String> args)
        {
            EvtTransitionNotify?.Invoke(this, new InterpreterEventArgs(args));

            return true;
        }
#pragma warning restore IDE0051
    }
}