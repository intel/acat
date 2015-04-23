////////////////////////////////////////////////////////////////////////////
// <copyright file="TriggerLock.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;
using System.Threading;

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
        /// Is the resource currently locked?
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