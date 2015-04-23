////////////////////////////////////////////////////////////////////////////
// <copyright file="Widget.cs" company="Intel Corporation">
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.ThemeManagement;
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

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Used for widget notification events (eg actuation, child added)
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event arguments</param>
    public delegate void WidgetEventDelegate(object sender, WidgetEventArgs e);

    /// <summary>
    /// For the event to indicate that the widget highlight turned on
    /// </summary>
    /// <param name="widget">source widget</param>
    /// <param name="handled">did the subscirber handle it?</param>
    public delegate void HighlightOnDelegate(Widget widget, out bool handled);

    /// <summary>
    /// For the event to indicate that the widget highlight turned off
    /// </summary>
    /// <param name="widget">source widget</param>
    /// <param name="handled">did the subscirber handle it?</param>
    public delegate void HighlightOffDelegate(Widget widget, out bool handled);

    /// <summary>
    /// This is the base class for all the UI widgets in
    /// the application.  The UI widgets include for example,
    /// the boxes, rows, buttons, etc.  This class provides useful
    /// helper functions and also core attributes that are common
    /// to all widgets.  Each of the derived classes enhance this
    /// by adding their own specific attributes.
    /// The class contains functions to enumerate child widgets,
    /// recursively turn highlight on/off for the widget and all
    /// its children/descendents, raises events etc.
    /// </summary>
    public class Widget : IDisposable
    {
        /// <summary>
        /// The enabled state of the widget.
        /// </summary>
        internal EnabledStates EnabledState;

        /// <summary>
        /// List of children of this widget
        /// </summary>
        protected List<Widget> _children;

        /// <summary>
        /// To draw rounded control and border
        /// </summary>
        protected GraphicsPath graphicsPath;

        /// <summary>
        /// Name of the widget
        /// </summary>
        protected String widgetName;

        /// <summary>
        /// Is this widget being disposed
        /// </summary>
        protected bool isDisposing;

        /// <summary>
        /// The layout that this widget is a part of
        /// </summary>
        private Layout _layout;

        /// <summary>
        /// Has this object been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// List of widgets that have the 'contextual' attribute
        /// enabled.  For these widgets, the 'enabled' state will
        /// be determined by the current context
        /// </summary>
        private readonly List<Widget> _contextualWidgets;

        /// <summary>
        /// Helper class to find widgets
        /// </summary>
        private readonly WidgetFinder _finder;

        /// <summary>
        /// Is this widget enabled or not
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// Initializes an instance of the widget class
        /// </summary>
        /// <param name="control">The .NET Control that represents this widget</param>
        public Widget(Control control)
        {
            UIControl = control;

            _children = new List<Widget>();

            Value = String.Empty;
            SubClass = String.Empty;
            Panel = String.Empty;

            Colors = null;
            graphicsPath = null;
            LayoutXmlNode = null;
            _layout = null;
            Parent = null;

            AddForAnimation = true;
            IsHighlightOn = false;
            DrawBorder = false;
            IsSelectedHighlightOn = false;
            _enabled = true;

            EnabledState = EnabledStates.Enabled;
            _contextualWidgets = new List<Widget>();
            _finder = new WidgetFinder(this);

            if (control != null)
            {
                widgetName = UIControl.Name;
                if (this is IButtonWidget)
                {
                    control.MouseClick += control_MouseClick;
                }

                control.Resize += control_Resize;
            }
            else
            {
                widgetName = string.Empty;
            }
        }

        /// <summary>
        /// Initializes an instance of the Widget class with the name
        /// </summary>
        /// <param name="name"></param>
        public Widget(String name)
            : this((Control)null)
        {
            widgetName = name;
        }

        /// <summary>
        /// Event raised when a child is added to
        /// this widget
        /// </summary>
        /// <param name="child"></param>
        public event WidgetEventDelegate EvtChildAdded;

        /// <summary>
        /// Event raised which this widget is actuated
        /// </summary>
        public event WidgetEventDelegate EvtActuated;

        /// <summary>
        /// Event raised when the widget value changes
        /// </summary>
        public event WidgetEventDelegate EvtValueChanged;

        /// <summary>
        /// Event raised when the widget is highlighlighted
        /// </summary>
        public event HighlightOnDelegate EvtHighlightOn;

        /// <summary>
        /// Event raised when the widget is unhighlighted
        /// </summary>
        public event HighlightOffDelegate EvtHighlightOff;

        /// <summary>
        /// Raised when a mouse click is detected
        /// </summary>
        public event WidgetEventDelegate EvtMouseClicked;

        /// <summary>
        /// The enabled states the widget can be in.
        /// </summary>
        internal enum EnabledStates
        {
            /// <summary>
            /// Widget is enabled
            /// </summary>
            Enabled,

            /// <summary>
            /// Widget is disabled
            /// </summary>
            Disabled,

            /// <summary>
            /// The enable/disable state is dynamically
            /// determined by the current context.
            /// </summary>
            Contextual
        }

        /// <summary>
        /// Gets the parent of this widget (null if no parent)
        /// </summary>
        public Widget Parent { get; internal set; }

        /// <summary>
        /// Gets the .NET UI control object that represents (e.g
        /// Label, Button etc
        /// this widget
        /// </summary>
        public Control UIControl { get; protected set; }

        /// <summary>
        /// Gets or sets the string value of the widget
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Gets or sets the widget subclass.
        /// </summary>
        public String SubClass { get; set; }

        /// <summary>
        /// Default color scheme can be overriden for a widget
        /// </summary>
        public ColorScheme Colors { get; set; }

        /// <summary>
        /// Gets or sets the colorscheme for the widget when it
        /// is disabled
        /// </summary>
        public ColorScheme DisabledButtonColors { get; set; }

        /// <summary>
        /// Gets or sets User data associated with the widget
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// Should this widget be added for animations?
        /// </summary>
        public bool AddForAnimation { get; protected set; }

        /// <summary>
        /// Gets or sets the xml node of the layout in the screen
        /// config file
        /// </summary>
        public XmlNode LayoutXmlNode { get; set; }

        /// <summary>
        /// Is the highlight on for this widget?
        /// </summary>
        public bool IsHighlightOn { get; protected set; }

        /// <summary>
        /// Is selected highlight on for this widget?
        /// </summary>
        public bool IsSelectedHighlightOn { get; protected set; }

        /// <summary>
        /// Should we draw a border around this control?
        /// </summary>
        public bool DrawBorder { get; private set; }

        /// <summary>
        /// Gets or sets the panel type of the widget.  This
        /// is applicable to scanners.
        /// </summary>
        public String Panel { get; private set; }

        public bool DefaultEnabled { get; set; }

        /// <summary>
        /// Gets the name of the widget.
        /// </summary>
        public String Name
        {
            get { return widgetName; }
        }

        /// <summary>
        /// Gets the width of the widget
        /// </summary>
        public virtual int Width
        {
            get { return (UIControl != null) ? UIControl.Width : 0; }
        }

        /// <summary>
        /// Gets the height of the widget
        /// </summary>
        public virtual int Height
        {
            get { return (UIControl != null) ? UIControl.Height : 0; }
        }

        /// <summary>
        /// Gets or sets the 'enabled' state of the widget and all
        /// its descendents.  If the widgets and its descendents
        /// highlight is on, it also turns them off
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                bool oldValue = _enabled;

                if (EnabledState == EnabledStates.Enabled ||
                    EnabledState == EnabledStates.Contextual)
                {
                    _enabled = value;
                }
                else
                {
                    _enabled = false;
                }

                if (oldValue != _enabled)
                {
                    foreach (var widget in _children)
                    {
                        widget.Enabled = _enabled;
                    }

                    if (!IsHighlightOn && !IsSelectedHighlightOn)
                    {
                        HighlightOff();
                    }
                }

                if (!CoreGlobals.AppPreferences.ScanDisabledElements)
                {
                    AddForAnimation = _enabled;
                }
            }
        }

        /// <summary>
        /// Gets the widget finder object
        /// </summary>
        public WidgetFinder Finder
        {
            get { return _finder; }
        }

        /// <summary>
        /// Returns an array of children
        /// </summary>
        public IEnumerable<Widget> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Returns all the contextual widgets.  Contextual widgets
        /// are those whose enabled state is set to "contextual".
        /// </summary>
        public IEnumerable ContextualWidgets
        {
            get { return _contextualWidgets; }
        }

        /// <summary>
        /// Returns whether a mouse click needs to be detected
        /// </summary>
        public bool IsMouseClickActuateOn
        {
            get
            {
                if (this is IButtonWidget)
                {
                    IButtonWidget button = (IButtonWidget)this;
                    return (button.GetWidgetAttribute() != null) && button.GetWidgetAttribute().MouseClickActuate;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the PCode for the mouse click event. null if none
        /// has been defined.
        /// </summary>
        public PCode OnMouseClick
        {
            get
            {
                if (this is IButtonWidget)
                {
                    IButtonWidget button = (IButtonWidget)this;
                    return (button.GetWidgetAttribute() != null) ?
                            button.GetWidgetAttribute().OnMouseClick :
                            null;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets normal background color.  Uses layout
        /// color scheme if this widget doesn't have a specific one
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.Background :
                            WidgetLayout.Colors.Background;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.Background :
                        WidgetLayout.DisabledButtonColors.Background;
            }
        }

        /// <summary>
        /// Gets background image to be used in the unhighlighted state
        /// </summary>
        public Image BackgroundImage
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.BackgroundImage :
                            WidgetLayout.Colors.BackgroundImage;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.BackgroundImage :
                        WidgetLayout.DisabledButtonColors.BackgroundImage;
            }
        }

        /// <summary>
        /// Gets normal foreground color.  Uses layout
        /// color scheme if this widget doesn't have a specific one
        /// </summary>
        public Color ForegroundColor
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.Foreground :
                            WidgetLayout.Colors.Foreground;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.Foreground :
                        WidgetLayout.DisabledButtonColors.Foreground;
            }
        }

        /// <summary>
        /// Gets highlighted background color.  Uses layout
        /// color scheme if this widget doesn't have a specific one
        /// </summary>
        public Color HighlightBackground
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.HighlightBackground :
                            WidgetLayout.Colors.HighlightBackground;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.HighlightBackground :
                        WidgetLayout.DisabledButtonColors.HighlightBackground;
            }
        }

        /// <summary>
        /// Gets the background image when the widget is in the
        /// highlighted state
        /// </summary>
        public Image HighlightBackgroundImage
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.HighlightBackgroundImage :
                            WidgetLayout.Colors.HighlightBackgroundImage;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.HighlightBackgroundImage :
                        WidgetLayout.DisabledButtonColors.HighlightBackgroundImage;
            }
        }

        /// <summary>
        /// Gets highlighted foreground color.  Uses layout
        /// color scheme if this widget doesn't have a specific one
        /// </summary>
        public Color HighlightForegroundColor
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.HighlightForeground :
                            WidgetLayout.Colors.HighlightForeground;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.HighlightForeground :
                        WidgetLayout.DisabledButtonColors.HighlightForeground;
            }
        }

        /// <summary>
        /// Gets highlight selected background color.  Uses layout
        /// color scheme if this widget doesn't have a specific one
        /// </summary>
        public Color HighlightSelectedBackgroundColor
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.HighlightSelectedBackground :
                            WidgetLayout.Colors.HighlightSelectedBackground;
                }

                return (DisabledButtonColors != null) ?
                    DisabledButtonColors.HighlightSelectedBackground :
                    WidgetLayout.DisabledButtonColors.HighlightSelectedBackground;
            }
        }

        /// <summary>
        /// Gets the background image to be used in the selected state
        /// </summary>
        public Image HighlightSelectedBackgroundImage
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.HighlightSelectedBackgroundImage :
                            WidgetLayout.Colors.HighlightSelectedBackgroundImage;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.HighlightSelectedBackgroundImage :
                        WidgetLayout.DisabledButtonColors.HighlightSelectedBackgroundImage;
            }
        }

        /// <summary>
        /// Returns highlight selected foreground color.  Uses layout
        /// color scheme if this widget doesn't have a specific one
        /// </summary>
        public Color HighlightSelectedForegroundColor
        {
            get
            {
                if (Enabled)
                {
                    return (Colors != null) ?
                            Colors.HighlightSelectedForeground :
                            WidgetLayout.Colors.HighlightSelectedForeground;
                }

                return (DisabledButtonColors != null) ?
                        DisabledButtonColors.HighlightSelectedForeground :
                        WidgetLayout.DisabledButtonColors.HighlightSelectedForeground;
            }
        }

        /// <summary>
        ///  Gets the parent layout object
        /// </summary>
        public Layout WidgetLayout
        {
            get { return _layout; }
        }

        /// <summary>
        /// Recursively sets the scale factor to make the widget
        /// larger or smaller
        /// </summary>
        /// <param name="newScaleFactor"></param>
        public virtual void SetScaleFactor(float newScaleFactor)
        {
            if (_children.Count != 0)
            {
                foreach (Widget child in _children)
                {
                    // set scale factor for each child widget
                    child.SetScaleFactor(newScaleFactor);
                }
            }
        }

        /// <summary>
        /// Dispose off the widget, release resources
        /// </summary>
        public void Dispose()
        {
            isDisposing = true;

            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Load widget-specific xml data.  Reads widget attributes
        /// from the xml file
        /// </summary>
        /// <param name="node"></param>
        public virtual void Load(XmlNode node)
        {
            SubClass = XmlUtils.GetXMLAttrString(node, "subclass");
            String colorScheme = XmlUtils.GetXMLAttrString(node, "colorScheme");
            if (!String.IsNullOrEmpty(colorScheme))
            {
                Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(colorScheme);
            }

            colorScheme = XmlUtils.GetXMLAttrString(node, "disabledButtonColorScheme");
            if (!String.IsNullOrEmpty(colorScheme))
            {
                DisabledButtonColors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(colorScheme);
            }

            DrawBorder = XmlUtils.GetXMLAttrBool(node, "drawBorder", DrawBorder);

            Panel = XmlUtils.GetXMLAttrString(node, "panel");

            EnabledState = parseEnabledValue(XmlUtils.GetXMLAttrString(node, "enabled", "true"));

            DefaultEnabled = XmlUtils.GetXMLAttrBool(node, "defaultEnabled", false);

            Enabled = true;
        }

        /// <summary>
        /// Called after all the children for this widget have been loaded
        /// </summary>
        public virtual void PostLoad()
        {
        }

        /// <summary>
        /// Adds the specified UI control object as a child widget of this
        /// widget
        /// </summary>
        /// <param name="control">The .NET UI Control</param>
        public void AddChild(Control control)
        {
            AddChild(new Widget(control));
        }

        /// <summary>
        /// Adds the specified widget object as a child
        /// </summary>
        /// <param name="widget">The widget to add</param>
        public void AddChild(Widget widget)
        {
            widget.Parent = this;
            _children.Add(widget);

            if (widget.EnabledState == EnabledStates.Contextual)
            {
                _contextualWidgets.Add(widget);
            }

            if (EvtChildAdded != null)
            {
                EvtChildAdded(this, new WidgetEventArgs(widget));
            }
        }

        /// <summary>
        /// Adds an array of .NET UI control objects
        /// as children
        /// </summary>
        /// <param name="children">The controls to add</param>
        public void AddChildren(Control[] children)
        {
            foreach (Control control in children)
            {
                AddChild(control);
            }
        }

        /// <summary>
        /// Adds an array of widgets as children
        /// </summary>
        /// <param name="children">Array to add</param>
        public void AddChildren(Widget[] children)
        {
            foreach (Widget widget in children)
            {
                AddChild(widget);
            }
        }

        /// <summary>
        /// Disposes off all the child widgets and clears
        /// the list of children
        /// </summary>
        public void ClearAllChildren()
        {
            foreach (Widget child in _children)
            {
                child.Dispose(true);
            }

            _children.Clear();
        }

        /// <summary>
        /// Uses color scheme to un-highlight the widget and
        /// all its descendent children.  So this is a recursive function.  For
        /// eg, if a box is to be highlighted, this also un-highlights all
        /// the rows and buttons
        /// </summary>
        /// <returns>true</returns>
        ///
        public bool HighlightOff()
        {
            try
            {
                if (_layout.RootWidget.UIControl.InvokeRequired)
                {
                    _layout.RootWidget.UIControl.Invoke(new MethodInvoker(delegate()
                    {
                        highlightOff();
                    }));
                }
                else
                {
                    highlightOff();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            return true;
        }

        /// <summary>
        /// Uses color scheme to highlight the widget and
        /// all its descendent children.  So this is a recursive function.  For
        /// eg, if a box is to be highlighted, this also highlights all
        /// the rows and buttons
        /// </summary>
        /// <returns>true</returns>
        ///
        public bool HighlightOn()
        {
            try
            {
                _layout.RootWidget.UIControl.Invoke(new MethodInvoker(delegate
                {
                    highlightOn();
                }));
            }
            catch
            {
            }

            return true;
        }

        /// <summary>
        /// Uses color scheme to highlight the selected widget and
        /// all its descendent children.  So this is a recursive function.  For
        /// eg, if a box is to be highlighted, this also highlights all
        /// the rows and buttons
        /// </summary>
        /// <returns>true</returns>
        public bool SelectedHighlightOn()
        {
            try
            {
                _layout.RootWidget.UIControl.Invoke(new MethodInvoker(delegate
                {
                    selectedHighlightOn();
                }));
            }
            catch
            {
            }

            return true;
        }

        /// <summary>
        /// Turn selected highlight off for this widget and all its children
        /// </summary>
        /// <returns></returns>
        public bool SelectedHighlightOff()
        {
            try
            {
                _layout.RootWidget.UIControl.Invoke(new MethodInvoker(delegate()
                {
                    selectedHighlightOff();
                }));
            }
            catch
            {
            }

            return true;
        }

        /// <summary>
        /// Sets the text value of the widget.
        /// </summary>
        /// <param name="text">text to set</param>
        public virtual void SetText(String text)
        {
            if (this.UIControl != null)
            {
                Windows.SetText(this.UIControl, text);
            }
        }

        /// <summary>
        /// Returns the text value of the widget.
        /// </summary>
        public virtual String GetText()
        {
            return (UIControl != null) ? Windows.GetText(UIControl) : String.Empty;
        }

        /// <summary>
        /// Checks if at least on child of this widget can
        /// be added for animation. Override this behavior for
        /// specific derived class behavior
        /// </summary>
        /// <returns>True if can</returns>
        public virtual bool CanAddForAnimation()
        {
            bool retVal = AddForAnimation;
            if (!retVal)
            {
                foreach (Widget child in _children)
                {
                    retVal = child.CanAddForAnimation();
                    if (retVal)
                    {
                        break;
                    }
                }
            }
            //Log.Debug("WidgetName: " + Name + ", retVal : " + retVal);
            return retVal;
        }

        /// <summary>
        /// Display this widget
        /// </summary>
        public void Show()
        {
            if (UIControl != null)
            {
                Windows.SetVisible(UIControl, true);
            }

            AddForAnimation = true;
        }

        /// <summary>
        /// Hide this widget
        /// </summary>
        public void Hide()
        {
            if (UIControl != null)
            {
                Windows.SetVisible(UIControl, false);
            }

            AddForAnimation = false;
        }

        /// <summary>
        /// Widget was actuated.  Trigger an event to inform
        /// the subscribers about this
        /// </summary>
        public virtual void Actuate()
        {
            Log.Debug(widgetName);

            if (!Enabled)
            {
                Log.Debug(widgetName + " is not enabled.  Will not actuate");
            }
            else if (EvtActuated != null)
            {
                Log.Debug("EvtActuated is not null.  Calling actuate for " + widgetName);
                EvtActuated(this, new WidgetEventArgs(this));
            }
            else
            {
                Log.Debug("EvtActuated is null for " + widgetName);
            }
        }

        /// <summary>
        /// Recursively prints the name of this widget and all its children
        /// </summary>
        public void Dump()
        {
            Log.Debug("Widget Name: " + Name);

            if (_children.Count == 0)
            {
                return;
            }

            foreach (Widget child in _children)
            {
                child.Dump();
            }
        }

#if DRAW_BORDER_SUPPORT
        /// <summary>
        /// Override this to draw a border around the widget
        /// </summary>
        protected virtual void DrawBorderOn()
        {
        }

        /// <summary>
        /// Override this to remove border around the widget
        /// </summary>

        protected virtual void DrawBorderOff()
        {
        }
#endif

        /// <summary>
        /// Sets the layout that this widget is a part of
        /// </summary>
        /// <param name="layout"></param>
        internal void SetLayout(Layout layout)
        {
            _layout = layout;
        }

        /// <summary>
        /// Let the derived classes handle this
        /// </summary>
        protected virtual void onResize()
        {
        }

        /// <summary>
        /// Raises the event to indicate that the widget highlight needs
        /// to be turned on.  If any of the event subscribers handle the
        /// highlighing themselves, they should set handled to true.
        /// </summary>
        /// <param name="handled"></param>
        protected void triggerEvtHighlightOn(out bool handled)
        {
            handled = false;
            if (EvtHighlightOn != null)
            {
                Log.Debug("EvtHighlightOn is not null. Triggering event");

                Delegate[] delegates = EvtHighlightOn.GetInvocationList();
                bool eventHandled = false;
                foreach (Delegate del in delegates)
                {
                    var highlightOnDelegate = (HighlightOnDelegate)del;
                    highlightOnDelegate.Invoke(this, out handled);

                    if (handled)
                    {
                        eventHandled = true;
                    }
                }

                handled = eventHandled;
                Log.Debug("EvtHighlightOn returned. handled = " + handled);
            }
            else
            {
                Log.Debug("EvtHighlightOn is null!");
            }
        }

        /// <summary>
        /// Raises the event to indicate that the widget highlight needs
        /// to be turned off.  If any of the event subscribers handle the
        /// highlighing themselves, they should set handled to true.
        /// </summary>
        /// <param name="handled"></param>
        protected void triggerEvtHighlightOff(out bool handled)
        {
            handled = false;
            if (EvtHighlightOff != null)
            {
                Delegate[] delegates = EvtHighlightOff.GetInvocationList();
                bool eventHandled = false;
                foreach (Delegate del in delegates)
                {
                    var highlightOffDelegate = (HighlightOffDelegate)del;

                    highlightOffDelegate.Invoke(this, out handled);
                    if (handled)
                    {
                        eventHandled = true;
                    }
                }

                handled = eventHandled;
            }
        }

        /// <summary>
        /// Creates a rounded corner version of this widget
        /// </summary>
        /// <param name="drawBorder">set to true to draw a border</param>
        /// <param name="radius">radius of the rounded corner</param>
        protected void createRoundedControl(bool drawBorder, int radius = 8)
        {
            if (UIControl != null)
            {
#if SUPPORTS_BORDER
                if (drawBorder)
                {
                    graphicsPath = RoundedCornerControl.Create(0, 0, UIControl.Width, UIControl.Height, 0, RoundedCornerControl.Corners.None);
                }
                else
#endif
                {
                    graphicsPath = RoundedCornerControl.Create(-1, -1, UIControl.Width - 1, UIControl.Height - 1, radius);
                }

                if (graphicsPath != null)
                {
                    var reg = new Region(graphicsPath);
                    Windows.SetRegion(UIControl, reg);
                }
            }
        }

        /// <summary>
        /// Turn highlight on for this widget and recursively for
        /// all its children.  Override this to handle highlighting
        /// in the derived class
        /// </summary>
        /// <returns></returns>
        protected virtual bool highlightOn()
        {
            bool handled = false;

            if (EvtHighlightOn != null)
            {
                EvtHighlightOn(this, out handled);
            }

            IsHighlightOn = true;

            if (!handled)
            {
                UIControl.BackColor = HighlightBackground;
                UIControl.ForeColor = HighlightForegroundColor;

                foreach (Widget widget in _children)
                {
                    widget.IsHighlightOn = true;
                    widget.highlightOn();
                }
            }

            return true;
        }

        /// <summary>
        /// Turn highlight off for this widget and recursively for
        /// all its children.  Override this to handle highlighting
        /// in the derived class
        /// </summary>
        /// <returns></returns>
        protected virtual bool highlightOff()
        {
            bool handled;

            triggerEvtHighlightOff(out handled);
            IsHighlightOn = false;

            if (!handled)
            {
                UIControl.BackColor = BackgroundColor;
                UIControl.ForeColor = ForegroundColor;
                foreach (Widget widget in _children)
                {
                    widget.IsHighlightOn = false;
                    widget.highlightOff();
                }
            }

            return true;
        }

        /// <summary>
        /// Turn selected highlight on for this widget and recursively for
        /// all its children.  Override this to handle highlighting
        /// in the derived class
        /// </summary>
        /// <returns></returns>
        protected virtual bool selectedHighlightOn()
        {
            IsSelectedHighlightOn = true;

            UIControl.BackColor = HighlightSelectedBackgroundColor;
            UIControl.ForeColor = HighlightSelectedForegroundColor;
#if DRAW_BORDER_SUPPORT
            DrawBorderOn();
#endif
            foreach (Widget widget in _children)
            {
                widget.selectedHighlightOn();
            }

            return true;
        }

        /// <summary>
        /// Uses color scheme to un-highlight the selected widget and
        /// all its descendent children.  So this is a recursive function.  For
        /// eg, if a box is to be un-highlighted, this also un-highlights all
        /// the rows and buttons
        /// </summary>
        /// <returns>true</returns>
        protected virtual bool selectedHighlightOff()
        {
            IsSelectedHighlightOn = false;

            UIControl.BackColor = BackgroundColor;
            UIControl.ForeColor = ForegroundColor;
#if DRAW_BORDER_SUPPORT
            DrawBorderOff();
#endif
            foreach (Widget widget in _children)
            {
                widget.selectedHighlightOff();
            }

            return true;
        }

        /// <summary>
        /// Raises an event if the value of this widget has changed
        /// </summary>
        protected void notifyValueChanged()
        {
            if (EvtValueChanged != null)
            {
                EvtValueChanged(this, new WidgetEventArgs(this));
            }
        }

        /// <summary>
        /// Sets the background color of the widget.  Takes care
        /// of cross-thread invocations that would result in
        /// .NET exceptions
        /// </summary>
        /// <param name="color">Color to set</param>
        protected virtual void setBackgroundColor(Color color)
        {
            if (UIControl != null)
            {
                Windows.SetBackgroundColor(UIControl, color);
            }
        }

        /// <summary>
        /// Sets the foreground color of the widget.  Takes care
        /// of cross-thread invocations that would result in
        /// .NET exceptions
        /// </summary>
        /// <param name="color">Color to set</param>
        protected virtual void setForegroundColor(Color color)
        {
            if (UIControl != null)
            {
                Windows.SetForegroundColor(UIControl, color);
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
                    // dispose all managed resources.
                    unInit();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Triggered when the user clicks the mouse on this widget.
        /// Actuate the widget
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void control_MouseClick(object sender, MouseEventArgs e)
        {
            if (Enabled && EvtMouseClicked != null)
            {
                EvtMouseClicked(this, new WidgetEventArgs(this));
            }
        }

        /// <summary>
        /// Dispose off children as well
        /// </summary>
        private void unInit()
        {
            if (UIControl != null)
            {
                if (this is IButtonWidget)
                {
                    UIControl.MouseClick -= control_MouseClick;
                }

                UIControl.Resize -= control_Resize;
            }

            foreach (Widget child in _children)
            {
                child.Dispose(true);
            }

            if (graphicsPath != null)
            {
                graphicsPath.Dispose();
            }
        }

        /// <summary>
        /// Parses the enabled value as a string and returns the enum version.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private EnabledStates parseEnabledValue(String value)
        {
            EnabledStates retVal;

            switch (value.ToLower())
            {
                case "false":
                    retVal = EnabledStates.Disabled;
                    break;

                case "contextual":
                    retVal = EnabledStates.Contextual;
                    break;

                default:
                    retVal = EnabledStates.Enabled;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Event handler for when the UI control is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void control_Resize(object sender, EventArgs e)
        {
            onResize();
        }
    }
}