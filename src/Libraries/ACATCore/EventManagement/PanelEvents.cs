////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PanelEvents.cs
//
// Event types for panel lifecycle notifications (show, hide, activate).
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Published when a panel is shown.
    /// </summary>
    public class PanelShowEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PanelShowEvent"/>.
        /// </summary>
        /// <param name="panelClass">The class identifier of the panel being shown.</param>
        public PanelShowEvent(string panelClass)
        {
            PanelClass = panelClass;
        }

        /// <summary>
        /// Gets the class identifier of the panel that was shown.
        /// </summary>
        public string PanelClass { get; }
    }

    /// <summary>
    /// Published when a panel is hidden.
    /// </summary>
    public class PanelHideEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PanelHideEvent"/>.
        /// </summary>
        /// <param name="panelClass">The class identifier of the panel being hidden.</param>
        public PanelHideEvent(string panelClass)
        {
            PanelClass = panelClass;
        }

        /// <summary>
        /// Gets the class identifier of the panel that was hidden.
        /// </summary>
        public string PanelClass { get; }
    }

    /// <summary>
    /// Published when a panel is activated (brought to the foreground).
    /// </summary>
    public class PanelActivateEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PanelActivateEvent"/>.
        /// </summary>
        /// <param name="panelClass">The class identifier of the panel being activated.</param>
        public PanelActivateEvent(string panelClass)
        {
            PanelClass = panelClass;
        }

        /// <summary>
        /// Gets the class identifier of the panel that was activated.
        /// </summary>
        public string PanelClass { get; }
    }

    /// <summary>
    /// Published when the display settings (e.g. resolution) change.
    /// Replaces the legacy <c>IPanelManager.EvtDisplaySettingsChanged</c> delegate.
    /// </summary>
    public class DisplaySettingsChangedEvent : EventBase
    {
    }
}
