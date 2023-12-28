////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Text;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents event args for the event raised to
    /// request for a scanner to be activated
    /// </summary>
    public class PanelRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PanelRequestEventArgs()
        {
            init();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">The scanner to be activted</param>
        /// <param name="monitorInfo">Contextual info about app window</param>
        public PanelRequestEventArgs(String panelClass, WindowActivityMonitorInfo monitorInfo)
        {
            init();

            PanelClass = panelClass;
            MonitorInfo = monitorInfo;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">The scanner to be activated</param>
        /// <param name="panelTitle">Title of the scanner </param>
        /// <param name="monitorInfo">Contextual info about app window</param>
        /// <param name="useCurrentScreenAsParent">Set to true to use the current panel as the parent</param>
        public PanelRequestEventArgs(String panelClass,
                                        String panelTitle,
                                        WindowActivityMonitorInfo monitorInfo,
                                        bool useCurrentScreenAsParent = false)
            : this(panelClass, monitorInfo)
        {
            Title = panelTitle;
            TargetPanel = null;
            UseCurrentScreenAsParent = useCurrentScreenAsParent;
        }

        /// <summary>
        /// Gets or sets active window information
        /// </summary>
        public WindowActivityMonitorInfo MonitorInfo { get; set; }

        /// <summary>
        /// Gets or sets the scanner class
        /// </summary>
        public String PanelClass { get; set; }

        /// <summary>
        /// Gets or sets User-defined arguments for the scanner
        /// </summary>
        public object RequestArg { get; set; }

        /// <summary>
        /// Gets or sets the dialog box or scanner for which this
        /// scanner is being activated.
        /// </summary>
        public object TargetPanel { get; set; }

        /// <summary>
        /// Gets or sets title of the scanner
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets whether to use the current scanner as the parent
        /// for the scanner being activated
        /// </summary>
        public bool UseCurrentScreenAsParent { get; set; }

        /// <summary>
        /// Converts to a string representation for debugging
        /// purposes
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            String title = Title ?? "<null>";
            sb.Append("PanelRequestEventArgs: PanelClass: " + PanelClass +
                        ",  UseAsParent: " + UseCurrentScreenAsParent +
                        " Title: " + title);
            return sb.ToString();
        }

        /// <summary>
        /// Initializes class variables
        /// </summary>
        private void init()
        {
            PanelClass = PanelClasses.None;
            UseCurrentScreenAsParent = false;
            RequestArg = null;
            Title = "Default";
            TargetPanel = null;
        }
    }
}