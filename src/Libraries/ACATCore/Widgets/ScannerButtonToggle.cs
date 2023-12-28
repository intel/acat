////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// A widget that uses a ScannerButton as the UI control.
    /// </summary>
    public class ScannerButtonToggle : ScannerButtonBase, IToggleButtonWidget
    {
        /// <summary>
        /// Event raised which this widget is actuated
        /// </summary>
        public event EventHandler EvtToggleStateChanged;

        internal bool _toggleState = false;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ScannerButtonToggle(Control uiControl)
            : base(uiControl)
        {
            if (ThemeManager.Instance.ActiveTheme.Colors.Exists("ScannerButton"))
            {
                Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme("ScannerButton");
            }
        }

        public bool ToggleState
        {
            get
            {
                return _toggleState;
            }
            set
            {
                _toggleState = value;
                EvtToggleStateChanged?.Invoke(this, new EventArgs());

                var list = new List<Widget>();
                WidgetLayout.RootWidget.Finder.FindAllButtons(list);
                foreach (var widget in list)
                {
                    if (widget == this)
                    {
                        setToggleColor(false);
                        continue;
                    }

                    var toggleButton = widget as IToggleButtonWidget;
                    if (toggleButton != null)
                    {
                        if (!String.IsNullOrEmpty(toggleButton.ToggleGroup) &&
                            !String.IsNullOrEmpty(ToggleGroup) &&
                            toggleButton.ToggleGroup == ToggleGroup)
                        {
                            if (toggleButton is ScannerButtonToggle)
                            {
                                (toggleButton as ScannerButtonToggle).setToggleState(false);
                            }
                            else
                            {
                                toggleButton.ToggleState = false;
                            }
                        }
                    }
                }
            }
        }

        internal void setToggleState(bool state)
        {
            _toggleState = state;
            setToggleColor(false);
        }

        public String ToggleGroup { get; set; }

        public override void Actuate(bool repeatActuate)
        {
            base.Actuate(repeatActuate);

            if (Enabled)
            {
                ToggleState = !ToggleState;
                setToggleColor(false);
            }
        }

        public override void Load(XmlNode node)
        {
            ToggleGroup = XmlUtils.GetXMLAttrString(node, "toggleGroup");
        }

        private void setToggleColor(bool isHighlightOn)
        {
            if (button == null)
            {
                return;
            }

            if (!button.Enabled)
            {
                base.HighlightOff();
                return;
            }

            if (isHighlightOn)
            {
                if (ToggleState)
                {
                    button.BackColor = HighlightSelectedBackgroundColor;
                    button.ForeColor = HighlightSelectedForegroundColor;
                }
                else
                {
                    // button.BackColor = HighlightBackground;
                    button.FlatAppearance.BorderSize = 10;
                    button.FlatAppearance.BorderColor = HighlightBackground;
                    button.ForeColor = HighlightForegroundColor;
                }
            }
            else
            {
                if (ToggleState)
                {
                    button.BackColor = SelectedBackgroundColor;
                    button.ForeColor = SelectedForegroundColor;
                }
                else
                {
                    button.BackColor = BackgroundColor;
                    button.ForeColor = ForegroundColor;
                    button.FlatAppearance.BorderSize = 0;
                }
            }
        }

        /// <summary>
        /// Unhighlights the widget
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOff()
        {
            bool retVal = true;

            if (button != null)
            {
                setToggleColor(false);
            }
            else
            {
                retVal = base.highlightOff();
            }

            return retVal;
        }

        /// <summary>
        /// Highlights the widget.
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOn()
        {
            bool retVal = true;
            if (button != null)
            {
                setToggleColor(true);
            }
            else
            {
                retVal = base.highlightOn();
            }

            return retVal;
        }

        /// <summary>
        /// Turns selected highlight on
        /// </summary>
        /// <returns>true</returns>
        protected override bool selectedHighlightOn()
        {
            bool retVal = true;

            if (button != null)
            {
                if (HighlightSelectedBackgroundImage != null)
                {
                    button.BackgroundImage = HighlightSelectedBackgroundImage;
                    button.ForeColor = HighlightSelectedForegroundColor;
                }
                else
                {
                    button.BackgroundImage = null;
                    retVal = base.selectedHighlightOn();
                }
            }
            else
            {
                retVal = base.highlightOn();
            }

            return retVal;
        }
    }
}