////////////////////////////////////////////////////////////////////////////
// <copyright file="TimerQueue.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// A wrapper class for Windows timer queue.  .NET timers
    /// are inherently inaccurate.  Their fidelity is on the
    /// order of 15 ms.  This timer gives us almost millisecond
    /// level accuracy
    /// </summary>
    public class TimerQueue : IDisposable
    {
        /// <summary>
        /// Windows constant
        /// </summary>
        private const int ErrorIOPending = 997;

        /// <summary>
        /// Callback function for the timer
        /// </summary>
        private readonly WaitOrTimerDelegate _callback;

        /// <summary>
        /// When will the timer expire
        /// </summary>
        private readonly uint _dueTime;

        /// <summary>
        /// The timer period, how often to fire after
        /// the first time
        /// </summary>
        private readonly uint _period;

        /// <summary>
        /// Disposed yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Is the timer running?
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// Handle to the timer
        /// </summary>
        private IntPtr _timerHandle;

        /// <summary>
        /// Constructor.  Use this for a oneshot timer that will
        /// expire at dueTime.
        /// </summary>
        /// <param name="dueTime">When to trigger</param>
        /// <param name="callback">callback to invoke</param>
        public TimerQueue(int dueTime, WaitOrTimerDelegate callback)
        {
            _dueTime = (uint)dueTime;
            _period = 0;
            _callback = callback;
        }

        /// <summary>
        /// Use this for periodic timer.  DueTime is when it's
        /// fired first (can be 0 for immediate) and period is
        /// how often to fire subsequently
        /// </summary>
        /// <param name="dueTime">Time of first firing</param>
        /// <param name="period">Periodicity</param>
        /// <param name="callback">callback function</param>
        public TimerQueue(int dueTime, int period, WaitOrTimerDelegate callback)
        {
            _dueTime = (uint)dueTime;
            _period = (uint)period;
            _callback = callback;
        }

        /// <summary>
        /// Timer callback delegate
        /// </summary>
        /// <param name="lpParameter">a tag opaque object</param>
        /// <param name="timerOrWaitFired"></param>
        public delegate void WaitOrTimerDelegate(IntPtr lpParameter, bool timerOrWaitFired);

        private enum Flag
        {
            WT_EXECUTEDEFAULT = 0x00000000,
            WT_EXECUTEINIOTHREAD = 0x00000001,
            WT_EXECUTEONLYONCE = 0x00000008,
            WT_EXECUTELONGFUNCTION = 0x00000010,
            WT_EXECUTEINTIMERTHREAD = 0x00000020,
            WT_EXECUTEINPERSISTENTTHREAD = 0x00000080,
        }

        /// <summary>
        /// Dispose off the timer
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            return Start(IntPtr.Zero);
        }

        /// <summary>
        /// Start the timer. Optional param will be sent
        /// to the callback function
        /// </summary>
        /// <param name="param">parameter to send to callback</param>
        /// <returns>true on success</returns>
        public bool Start(IntPtr param)
        {
            int getLastError;

            if (_isRunning == true)
            {
                // already running
                Log.Debug("Timer is already running. returning");
                return true;
            }

            bool retVal = CreateTimerQueueTimer(
                            out _timerHandle,
                            IntPtr.Zero,
                            _callback,
                            param,
                            _dueTime,
                            _period,
                            (uint)Flag.WT_EXECUTEINIOTHREAD);

            if (retVal)
            {
                _isRunning = true;
            }
            else
            {
                getLastError = Marshal.GetLastWin32Error();
                Log.Debug("Error while starting timer.  getLastError=" + getLastError);
            }

            return retVal;
        }

        /// <summary>
        /// Stop the timer and delete it
        /// </summary>
        public bool Stop()
        {
            int getLastError = 0;

            if (_isRunning == false)
            {
                // already stopped
                Log.Debug("Not running. returning");
                return true;
            }

            bool retVal = true;
            try
            {
                retVal = DeleteTimerQueueTimer(IntPtr.Zero, _timerHandle, IntPtr.Zero);

                _isRunning = false;

                if (!retVal)
                {
                    if (getLastError == ErrorIOPending)
                    {
                        retVal = true;
                    }
                }
            }
            catch
            {
            }

            return retVal;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreateTimerQueueTimer(
                            out IntPtr handle,
                            IntPtr timerQueue,
                            WaitOrTimerDelegate callback,
                            IntPtr param,
                            uint dueTime,
                            uint period,
                            uint flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeleteTimerQueueTimer(IntPtr timerQueue, IntPtr timer, IntPtr completionEvent);
    }
}