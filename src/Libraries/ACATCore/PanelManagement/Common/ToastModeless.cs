////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Diagnostics;
using System.Threading;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Displays a splash screen.  The splash screen has a
    /// bitmap and three lines of text, all of which can be
    /// set by the application. The SplashScreen class is used
    /// as the splash screen form
    /// </summary>
    public class ToastModeless
    {
        /// <summary>
        /// The splash screen form
        /// </summary>
        private ToastForm _form;

        /// <summary>
        /// Thread to show the splash screen
        /// </summary>
        private Thread _thread;

        private String _message;

        /// <summary>
        /// Initializes a new instance of the class.  Fills in details
        /// from the assembly info (version, copyright etc)
        /// </summary>
        /// <param name="minUpTime">min time to stay on</param>
        public ToastModeless(String message)
        {
            _message = message;
        }

        /// <summary>
        /// Call this to dismiss the splash screen
        /// </summary>
        public void Close()
        {
            try
            {
                if (_form != null)
                {
                    Windows.CloseForm(_form);
                    _form = null;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Call this to show the splash screen.  It is display asynchronously
        /// to enable the application to do its initializating tasks
        /// </summary>
        public void Show()
        {
            _thread = new Thread(showToast) { IsBackground = true };
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }

        /// <summary>
        /// Displays the splash screen
        /// </summary>
        private void showToast()
        {
            _form = new ToastForm(_message);

            _form.ShowDialog();
        }
    }
}