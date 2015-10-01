////////////////////////////////////////////////////////////////////////////
// <copyright file="DescriptorAttribute.cs" company="Intel Corporation">
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
    /// All dynamically discovered and loaded classes such as
    /// Scanners, Dialogs, Menus, App Agents, Actuators, Word Predictors
    /// should have a descriptor attribute that includes a unique
    /// GUID, a friendly name and a friendly description.  This class
    /// encapsulates all this information.
    /// </summary>
    public class DescriptorAttribute : Attribute, IDescriptor
    {
        /// <summary>
        /// Friendly description
        /// </summary>
        private String _description;

        /// <summary>
        /// Unique identifier
        /// </summary>
        private Guid _guid;

        /// <summary>
        /// Friendly name
        /// </summary>
        private String _name;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="id">GUID id</param>
        /// <param name="name">friendly name</param>
        /// <param name="description">description</param>
        public DescriptorAttribute(String id, String name, String description)
        {
            _name = name;
            _description = description;
            if (!Guid.TryParse(id, out _guid))
            {
                _guid = Guid.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// Gets or sets the unique id
        /// </summary>
        public Guid Id
        {
            get
            {
                return _guid;
            }
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Regurns the descriptor object for the class bu querying
        /// custom attributes and looking for the one that is of
        /// type DescriptorAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DescriptorAttribute GetDescriptor(Type type)
        {
            foreach (object attribute in type.GetCustomAttributes(true))
            {
                if (attribute is DescriptorAttribute)
                {
                    return attribute as DescriptorAttribute;
                }
            }

            return null;
        }
    }
}