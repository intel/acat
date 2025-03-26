using System;
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////
using ACATResources;

namespace ACAT.Lib.Core.Utility
{
    public static class FatalErrorHandler
    {
        public static event FatalErrorDelegate EvtFatalError;

        /// <summary>
        /// Delegate for the fatal error event
        /// </summary>
        /// <param name="reason"></param>
        public delegate void FatalErrorDelegate(String reason);

        /// <summary>
        /// Call this to trigger a fatal error which will cause ACAT
        /// to exit immediately.  Use this only for unrecoverable error
        /// </summary>
        /// <param name="reason"></param>
        public static void OnFatalError(String reason)
        {
            EvtFatalError?.Invoke(reason);
        }

        public static void SymlinkOrJunctionError(String filename)
        {
            OnFatalError(String.Format(Resources.SymlinkFatalError, filename));

        }
    }
}