////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Core.ActuatorManagement
{
    /// <summary>
    /// Factory interface for creating ActuatorManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface IActuatorManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the ActuatorManager instance
        /// </summary>
        /// <returns>The ActuatorManager instance</returns>
        IActuatorManager Create();
    }

    /// <summary>
    /// Default factory implementation for ActuatorManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class ActuatorManagerFactory : IActuatorManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the ActuatorManager singleton instance
        /// </summary>
        /// <returns>The ActuatorManager singleton instance</returns>
        public IActuatorManager Create()
        {
            return ActuatorManager.Instance;
        }
    }
}
