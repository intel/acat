////////////////////////////////////////////////////////////////////////////
// <copyright file="Animation.cs" company="Intel Corporation">
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Represents a single animation sequence.
    ///
    /// The hierarchy is as follows
    ///     AnimationsCollection (collection of animations indexed by screen name)
    ///        Animations  (collection of animations for a screen)
    ///           Animation  (a single animation)
    /// </summary>
    public class Animation : IDisposable
    {
        /// <summary>
        /// Code to execute when the user hits the back button at any
        /// point during this animation sequence
        /// </summary>
        public PCode OnBack;

        /// <summary>
        /// Code to execute when the animation sequence ends without the
        /// user having selected anything during the sequence
        /// </summary>
        public PCode OnEnd;

        /// <summary>
        /// Code to execute before starting the animation sequence
        /// </summary>
        public PCode OnEnter;

        /// <summary>
        /// Code to execute if the user selects a widget during
        /// this animation sequence
        /// </summary>
        public PCode OnSelect;

        internal bool OnStart = true;

        /// <summary>
        /// To parse the code associated with this animation sequence
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// String representation of hesitate time as read from the XML file
        /// </summary>
        private string _hesitateTimeVariableName;

        /// <summary>
        /// String representation of stepping time as read from the XML file
        /// </summary>
        private string _steppingTimeVariableName;

        /// <summary>
        /// The xml node list from the config file that contains
        /// a list of all the widgets that participate in this
        /// animation
        /// </summary>
        private XmlNodeList _widgetXMLNodeList;

        /// <summary>
        /// Initialzies an instance of the class
        /// </summary>
        public Animation()
        {
            _parser = new Parser();
            Screen = String.Empty;
            Iterations = "1";
            OnEnterExecutionNotDone = false;
            AnimationWidgetList = new List<AnimationWidget>();
            OnBack = new PCode();
            OnEnd = new PCode();
            OnSelect = new PCode();
            OnEnter = new PCode();
            SteppingTime = CoreGlobals.AppPreferences.SteppingTime;
            HesitateTime = CoreGlobals.AppPreferences.HesitateTime;
            CoreGlobals.AppPreferences.EvtPreferencesChanged += AppPreferences_EvtPreferencesChanged;
        }

        /// <summary>
        /// Delegate for the event raised when an widget
        /// is added to the animation sequence
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public delegate void AnimationWidgetAdded(object sender, AnimationWidgetAddedEventArgs e);

        /// <summary>
        /// Delegate for the event raised to resolve the widgets that particpate
        /// in the animation sequence
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        public delegate void ResolveWidgetChildren(object sender, ResolveWidgetChildrenEventArgs e);

        /// <summary>
        /// Raised when an widget is added to the animation sequence
        /// </summary>
        public event AnimationWidgetAdded EvtAnimationWidgetAdded;

        /// <summary>
        /// Raised to resolve the widgets that particpate
        /// in the animation sequence
        /// </summary>
        public event ResolveWidgetChildren EvtResolveWidgetChildren;

        /// <summary>
        /// Gets or sets the list of widgets that participate in this animation
        /// sequence
        /// </summary>
        public List<AnimationWidget> AnimationWidgetList { get; set; }

        /// <summary>
        /// Gets or sets whether to autostart the animation sequence
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Gets or sets additional amount of time to keep the first animation
        /// widget in the sequence highlighted
        /// </summary>
        public int HesitateTime { get; set; }

        /// <summary>
        /// Gets or sets whether this is the first animation in the sequence
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// Gets or sets the number of times to execute this animation sequence
        /// </summary>
        public String Iterations { get; set; }

        /// <summary>
        /// Gets or sets the name of this animation sequence.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets whether the OnEnter code been executed
        /// </summary>
        public bool OnEnterExecutionNotDone { get; set; }

        /// <summary>
        /// Gets or sets the name of the scanner to which this animation belongs.
        /// </summary>
        public String Screen { get; set; }

        /// <summary>
        /// Gets or sets the amount of time a widget stays highlighted
        /// </summary>
        public int SteppingTime { get; set; }

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
        /// Returns the outer animation widget for the specified
        /// ui widget
        /// </summary>
        /// <param name="widget">ui widget</param>
        /// <returns>animation widget</returns>
        public AnimationWidget GetAnimationWidget(Widget widget)
        {
            return AnimationWidgetList.FirstOrDefault(animationWidget => animationWidget.UIWidget == widget);
        }

        /// <summary>
        /// Load the animation sequence from the XML file
        /// </summary>
        /// <param name="xmlNode">XML node that represents this animation seq</param>
        public void Load(XmlNode xmlNode)
        {
            OnStart = true;

            Name = XmlUtils.GetXMLAttrString(xmlNode, "name");
            IsFirst = XmlUtils.GetXMLAttrBool(xmlNode, "start", false);
            AutoStart = !IsFirst || XmlUtils.GetXMLAttrBool(xmlNode, "autoStart", true);

            var onBack = XmlUtils.GetXMLAttrString(xmlNode, "onBack");
            _parser.Parse(onBack, ref OnBack);

            var onEnd = XmlUtils.GetXMLAttrString(xmlNode, "onEnd");
            _parser.Parse(onEnd, ref OnEnd);

            var onSelect = XmlUtils.GetXMLAttrString(xmlNode, "onSelect");
            _parser.Parse(onSelect, ref OnSelect);

            var onEnter = XmlUtils.GetXMLAttrString(xmlNode, "onEnter");
            _parser.Parse(onEnter, ref OnEnter);

            Iterations = XmlUtils.GetXMLAttrString(xmlNode, "iterations");

            _hesitateTimeVariableName = XmlUtils.GetXMLAttrString(xmlNode, "hesitateTime");
            HesitateTime = CoreGlobals.AppPreferences.ResolveVariableInt(_hesitateTimeVariableName, 0, 0);

            _steppingTimeVariableName = XmlUtils.GetXMLAttrString(xmlNode, "steppingTime");
            SteppingTime = CoreGlobals.AppPreferences.ResolveVariableInt(_steppingTimeVariableName,
                                                                CoreGlobals.AppPreferences.SteppingTime,
                                                                CoreGlobals.AppPreferences.SteppingTime);

            _widgetXMLNodeList = xmlNode.SelectNodes("Widget");
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public void Stop()
        {
            clearAnimationWidgetList();
        }

        /// <summary>
        /// The animation sequence can contain wildcard names for the
        /// UI widgets or even references (such as @SelectedWidget for the
        /// currently selected widget).  Resolves all these referneces to their
        /// actual names and adds the actual widgets to the child list
        /// </summary>
        internal void ResolveUIWidgetsReferences(Widget rootWidget, Variables variables)
        {
            clearAnimationWidgetList();

            Log.Debug(rootWidget.Name + ". widgetXMLNodeList count: " + _widgetXMLNodeList.Count);

            foreach (XmlNode xmlNode in _widgetXMLNodeList)
            {
                resolveWildCardReferences(rootWidget, variables, xmlNode);
                resolveNonWildCardReferences(rootWidget, variables, xmlNode);
            }
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    clearAnimationWidgetList();

                    // dispose all managed resources.
                    CoreGlobals.AppPreferences.EvtPreferencesChanged -= AppPreferences_EvtPreferencesChanged;
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Event is invoked whenever global preferences change.  Update our
        /// settings
        /// </summary>
        /// <param name="pref">Changed preferences</param>
        private void AppPreferences_EvtPreferencesChanged()
        {
            HesitateTime = CoreGlobals.AppPreferences.ResolveVariableInt(_hesitateTimeVariableName, 0, 0);
            SteppingTime = CoreGlobals.AppPreferences.ResolveVariableInt(_steppingTimeVariableName,
                                                                CoreGlobals.AppPreferences.SteppingTime,
                                                                CoreGlobals.AppPreferences.SteppingTime);
        }

        /// <summary>
        /// Clears the widget list and disposes off all
        /// the animation objects
        /// </summary>
        private void clearAnimationWidgetList()
        {
            foreach (AnimationWidget animationWidget in AnimationWidgetList)
            {
                animationWidget.Dispose();
            }

            AnimationWidgetList.Clear();
        }

        /// <summary>
        /// Creates and adds an animation widget entry that will contain the
        /// specified ui widget.  UI widget represents a windows control
        /// such as a button in the scanner.  Raises an event that the
        /// widget was added
        /// </summary>
        /// <param name="uiWidget">the ui widget</param>
        /// <returns>animation widget</returns>
        private AnimationWidget createAndAddAnimationWidget(Widget uiWidget)
        {
            var retVal = new AnimationWidget { UIWidget = uiWidget };
            AnimationWidgetList.Add(retVal);
            if (EvtAnimationWidgetAdded != null)
            {
                EvtAnimationWidgetAdded(this, new AnimationWidgetAddedEventArgs(retVal));
            }

            return retVal;
        }

        /// <summary>
        /// Wild card names can be @variablename/* or widgetname/*. Parses
        /// the string and returns the name of the container widget
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="wildCard">wildcard to resolve</param>
        /// <returns>container widget</returns>
        private Widget getContainerWidget(Widget rootWidget, Variables variables, string wildCard)
        {
            Widget retVal = null;
            String[] wildCardPatterns =
            {
                "\\@[a-zA-Z0-9]*/\\*",      // @variablename/*
                "[a-zA-Z0-9]*/\\*",         // widgename/*"
                "\\*"                       // *
            };

            String[] extractPatterns =
            {
                "\\@[a-zA-Z0-9]*",
                "[a-zA-Z0-9]*",
                "\\*"
            };

            bool done = false;
            for (int ii = 0; !done && ii < wildCardPatterns.Length; ii++)
            {
                if (!Regex.IsMatch(wildCard, wildCardPatterns[ii]))
                {
                    continue;
                }

                Match match = Regex.Match(wildCard, extractPatterns[ii]);
                if (String.IsNullOrEmpty(match.Value))
                {
                    continue;
                }

                done = true;
                String widgetName;
                switch (ii)
                {
                    case 0:
                        widgetName = match.Value.Substring(1);
                        if (String.Compare(widgetName, Variables.SelectedWidget) == 0)
                        {
                            retVal = (Widget)variables.Get(Variables.SelectedWidget);
                        }

                        break;

                    case 1:
                        widgetName = match.Value;
                        if (String.Compare(widgetName, rootWidget.Name, true) == 0)
                        {
                            retVal = rootWidget;
                        }
                        else
                        {
                            retVal = rootWidget.Finder.FindChild(widgetName);
                        }

                        break;

                    case 2:
                        retVal = (Widget)variables.Get(Variables.SelectedWidget);
                        break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Resolves any variable names
        /// </summary>
        /// <param name="name">name of the variable</param>
        /// <returns>its value</returns>
        private String resolveName(Variables variables, String name)
        {
            var retVal = name;
            if (String.Compare(name, "@SelectedWidget", true) == 0)
            {
                var widget = (Widget)variables.Get(Variables.SelectedWidget);
                retVal = widget.Name;
            }

            return retVal;
        }

        /// <summary>
        /// Resolves non-wild card entries to their acutal names.  For instance,
        /// a @SelectedWidget would get resolved to the name of the widget that
        /// is currently selected
        /// </summary>
        /// <param name="rootWidget">The root widget of the scanner</param>
        /// <param name="variables">variable references to resolve</param>
        /// <param name="xmlNode">the xmlNode to parse</param>
        private void resolveNonWildCardReferences(Widget rootWidget, Variables variables, XmlNode xmlNode)
        {
            var name = XmlUtils.GetXMLAttrString(xmlNode, "name");

            Log.Debug("name=" + name);
            if (!String.IsNullOrEmpty(name) && !name.Contains("*"))
            {
                var widgetName = resolveName(variables, name);
                Log.Debug("Resolved name : " + widgetName);

                var uiWidget = rootWidget.Finder.FindChild(widgetName);
                if (uiWidget != null)
                {
                    Log.Debug("Found child name : " + widgetName);
                    var animationWidget = createAndAddAnimationWidget(uiWidget);
                    if (animationWidget != null)
                    {
                        animationWidget.Load(xmlNode);
                    }
                }
                else
                {
                    Log.Debug("Did not find child " + widgetName);
                }
            }

            return;
        }

        /// <summary>
        /// Resolves all widgets with wild card references (ends with an asterisk).
        /// The format for a wild card reference is <widgetname>/* where widgnetname
        /// is the parent widget and * represents all its children
        /// Eg.  Box1/* would resolve to Row1, Row2, Row3 and Row4
        /// </summary>
        /// <param name="rootWidget">The root widget of the scanner</param>
        /// <param name="variables">variable references to resolve</param>
        /// <param name="xmlNode">the xmlNode to parse</param>
        private void resolveWildCardReferences(Widget rootWidget, Variables variables, XmlNode xmlNode)
        {
            var name = XmlUtils.GetXMLAttrString(xmlNode, "name");

            if (name.Contains("*"))
            {
                Log.Debug("name=" + name);

                var containerWidget = getContainerWidget(rootWidget, variables, name);
                if (containerWidget != null)
                {
                    Log.Debug("containerWidget: " + containerWidget.Name);

                    if (EvtResolveWidgetChildren != null)
                    {
                        EvtResolveWidgetChildren(this, new ResolveWidgetChildrenEventArgs(rootWidget, containerWidget, xmlNode));
                    }

                    foreach (var childWidget in containerWidget.Children)
                    {
                        Log.Debug("Found child name : " + childWidget.Name);
                        var animationWidget = createAndAddAnimationWidget(childWidget);
                        if (animationWidget != null)
                        {
                            animationWidget.Load(xmlNode);
                        }
                    }
                }
            }
        }
    }
}