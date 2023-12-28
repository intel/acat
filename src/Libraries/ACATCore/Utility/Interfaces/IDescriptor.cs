////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Represents the name, a brief description and a GUID
    /// for a class. Every class that is dynamically discovered
    /// and loaded must implement this interface.  Examples are
    /// scanners, application agents, word predictors, actuators etc.
    /// </summary>
    public interface IDescriptor
    {
        /// <summary>
        /// Category of the module
        /// </summary>
        String Category { get; }

        /// <summary>
        /// A brief description
        /// </summary>
        String Description { get; }

        /// <summary>
        /// GUID used as a unique ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Name of the module
        /// </summary>
        String Name { get; }
    }
}