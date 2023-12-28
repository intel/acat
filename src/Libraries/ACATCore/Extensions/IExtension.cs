////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;

namespace ACAT.Lib.Core.Extensions
{
    /// <summary>
    /// Interface to enable callers to invoke methods/properties
    /// and events from a class through reflection.  Derive your
    /// class from this interface to enable ACAT call methods/properties
    /// from your class. (see ExtensionInvoker class)
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Returns the IDescriptor object for the class
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns></returns>
        ExtensionInvoker GetInvoker();
    }
}