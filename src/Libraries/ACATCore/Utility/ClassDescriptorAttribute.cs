////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// All dynamically discovered and loaded classes such as
    /// Scanners, Dialogs, Menus, App Agents, Actuators, Word Predictors
    /// should have a descriptor attribute that includes a unique
    /// GUID, a friendly name and a friendly description.  This class
    /// encapsulates all this information.
    /// </summary>
    public class ClassDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Friendly name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Friendly description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Category (user-defined)
        /// </summary>
        public String Category { get; set; }

        public ClassDescriptorAttribute(string id)
        {
            if (!Guid.TryParse(id, out Guid temp))
            {
                throw new ArgumentException("Invalid GUID format", nameof(id));
            }
            else
            {
                Id = temp;
            }
        }

        public ClassDescriptorAttribute(string id, string name) : this(id)
        {
            Name = name;
            Category = String.Empty;
            Description = String.Empty;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="id">GUID id</param>
        /// <param name="name">friendly name</param>
        /// <param name="description">description</param>
        public ClassDescriptorAttribute(String id, String name, String description) : this(id, name)
        {
            Description = description;
            Category = String.Empty;
        }

        public ClassDescriptorAttribute(String id, String name, String description, String category) : this(id, name, description)
        {
            Category = category;
        }

        /// <summary>
        /// Returns the descriptor object for the class by querying
        /// custom attributes and looking for the one that is of
        /// type ClassDescriptorAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ClassDescriptorAttribute GetDescriptor(Type type)
        {
            foreach (object attribute in type.GetCustomAttributes(true))
            {
                if (attribute is ClassDescriptorAttribute)
                {
                    return attribute as ClassDescriptorAttribute;
                }
            }

            return null;
        }
    }
}