////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

namespace ACAT.Lib.Core.UserControlManagement
{
    public delegate void AnimationPlayerStateChanged(IUserControl userControl, PlayerStateChangedEventArgs e);

    /// <summary>
    /// ALL UserControls panels in ACAT must implement this.
    /// </summary>
    public interface IUserControl
    {
        event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the Descriptor for the panel. Descriptor
        /// contains name and ID of the panel
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Used for synchronization
        /// </summary>
        SyncLock SyncObj { get; }

        /// <summary>
        /// Handles functions that are common to all ACAT user controls
        /// </summary>
        IUserControlCommon UserControlCommon { get; }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner);

        void OnLoad();

        /// <summary>
        /// Pauses animation
        /// </summary>
        void OnPause();

        /// <summary>
        /// Resumes animations
        /// </summary>
        void OnResume();

        /// <summary>
        /// Called when a widget is triggered
        /// </summary>
        /// <param name="e"></param>
        /// <param name="handled"></param>
        void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled);
    }
}