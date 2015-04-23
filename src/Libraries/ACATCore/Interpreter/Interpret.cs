////////////////////////////////////////////////////////////////////////////
// <copyright file="Interpret.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

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
        /// Selects a widget.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        public delegate void Select(object sender, InterpreterEventArgs e);

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
        /// Executes the action for the specificed action verb. It
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
        /// Load scripts from the XML file
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
            if (EvtActuateNotify != null)
            {
                EvtActuateNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }

        /// <summary>
        /// Play a beep sound
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
            if (EvtCloseNotify != null)
            {
                EvtCloseNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }

        /// <summary>
        /// Triggers an event to highlight a widget
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool highlight(List<String> args)
        {
            if (EvtHighlightNotify != null)
            {
                EvtHighlightNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }

        /// <summary>
        /// Triggers an event to highlight a selected widget
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool highlightSelected(List<String> args)
        {
            if (EvtHighlightSelectedNotify != null)
            {
                EvtHighlightSelectedNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }

        /// <summary>
        /// Launch an external application
        /// </summary>
        /// <param name="args">Arguments - name of app</param>
        /// <returns>true on success</returns>
        private bool launch(List<String> args)
        {
            if (EvtLaunchNotify != null)
            {
                EvtLaunchNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }

        /// <summary>
        /// Run scripts through the "run" command.  Each element
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
                    EvtRun(this, new InterpreterRunEventArgs(scriptName));
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
            if (EvtSelectNotify != null)
            {
                EvtSelectNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }

        /// <summary>
        /// Stop animation
        /// </summary>
        /// <returns>true on success</returns>
        private bool stop(List<String> args)
        {
            if (EvtStopNotify != null)
            {
                EvtStopNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }

        /// <summary>
        /// Triggers an event to transition animation
        /// </summary>
        /// <param name="args">Argument list</param>
        /// <returns>true on success</returns>
        private bool transition(List<String> args)
        {
            if (EvtTransitionNotify != null)
            {
                EvtTransitionNotify(this, new InterpreterEventArgs(args));
            }

            return true;
        }
    }
}