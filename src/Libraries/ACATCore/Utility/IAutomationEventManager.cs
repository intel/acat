////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Automation;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Interface for AutomationEventManager to support dependency injection.
    /// Wrapper class for the .NET UI Automation API.
    /// </summary>
    public interface IAutomationEventManager : IDisposable
    {
        /// <summary>
        /// Initializes the automation event manager
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();

        /// <summary>
        /// Starts the automation event handler thread
        /// </summary>
        void Start();
    }
}
