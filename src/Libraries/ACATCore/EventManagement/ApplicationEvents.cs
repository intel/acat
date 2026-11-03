////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ApplicationEvents.cs
//
// Event types for application lifecycle notifications (quit).
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Published when the application is about to quit.
    /// Replaces the legacy <c>IPanelManager.EvtAppQuit</c> delegate.
    /// </summary>
    public class AppQuitEvent : EventBase
    {
    }
}
