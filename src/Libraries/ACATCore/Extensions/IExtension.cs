////////////////////////////////////////////////////////////////////////////
// <copyright file="IExtension.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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