////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.PanelManagement.CommandDispatcher
{
    /// <summary>
    /// Interface for command dispatchers
    /// </summary>
    public interface IRunCommandDispatcher
    {
        /// <summary>
        /// Gets or sets the active scanner
        /// </summary>
        IScannerPanel Scanner { get; set; }
    }
}