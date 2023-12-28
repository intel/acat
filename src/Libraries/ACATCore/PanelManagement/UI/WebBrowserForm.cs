////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// A form that has an embedded WebBrowser control
    /// </summary>
    public partial class WebBrowserForm : Form
    {
        public WebBrowserForm()
        {
            InitializeComponent();
            Load += WebBrowserForm_Load;
        }

        public String Link { get; set; }

        private void WebBrowserForm_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Link))
            {
                webBrowser.Navigate(Link);
            }
        }
    }
}