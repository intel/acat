////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Threading;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Thread-safe semphore-like behavior to gate
    /// access to shared resources
    /// Everytime Hold is called, counter is incremented.
    /// The OnHold call tells you whether the resource is
    /// locked.  When the counter goes back to 0, an
    /// event is raised to indicate that the resource
    /// is available. Hence the word "Trigger" in
    /// TriggerLock
    /// </summary>
    public class TriggerLock
    {
        /// <summary>
        /// How many have currently locked?
        /// </summary>
        private long _lockCount;

        /// <summary>
        /// For the event raised when unlocked
        /// </summary>
        public delegate void Unlocked();

        /// <summary>
        /// Rasied when the resource is unlocked
        /// </summary>
        public event Unlocked EvtUnlocked;

        /// <summary>
        /// Increment the lock count. Holds the resource
        /// </summary>
        public void Hold()
        {
            Interlocked.Increment(ref _lockCount);
        }

        /// <summary>
        /// Returns true if the resource currently locked
        /// </summary>
        /// <returns>true if it is</returns>
        public bool OnHold()
        {
            return Interlocked.Read(ref _lockCount) != 0;
        }

        /// <summary>
        /// Decrements lock count.  If the count is
        /// 0, raises an event to indicate that the
        /// resource is free
        /// </summary>
        public void Release()
        {
            Interlocked.Decrement(ref _lockCount);
            if (Interlocked.Read(ref _lockCount) <= 0)
            {
                Interlocked.Exchange(ref _lockCount, 0);
                if (EvtUnlocked != null)
                {
                    EvtUnlocked();
                }
            }
        }
    }
}