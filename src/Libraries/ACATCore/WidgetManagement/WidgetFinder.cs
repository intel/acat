////////////////////////////////////////////////////////////////////////////
// <copyright file="WidgetFinder.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Helper class to find widgets within a parent widget. Supports
    /// different ways of searching- by name, by the .NET class type etc.
    /// </summary>
    public class WidgetFinder
    {
        /// <summary>
        /// THe parent widget object
        /// </summary>
        private Widget _widget;

        /// <summary>
        /// Initializes an instance of the WidgerFinder class
        /// </summary>
        /// <param name="widget"></param>
        internal WidgetFinder(Widget widget)
        {
            _widget = widget;
        }

        /// <summary>
        /// Get a list of all the buttons (direct children and
        /// below)
        /// </summary>
        /// <param name="list">Returns list of buttons</param>
        public void FindAllButtons(List<Widget> list)
        {
            if (!_widget.Children.Any())
            {
                return;
            }

            try
            {
                foreach (Widget child in _widget.Children)
                {
                    if (child is IButtonWidget)
                    {
                        list.Add(child);
                    }
                }
            }
            catch
            {
            }

            // now recursively add children
            foreach (Widget child in _widget.Children)
            {
                child.Finder.FindAllButtons(list);
            }
        }

        /// <summary>
        /// Recursively finds all children and their descendents and
        /// adds them to the list
        /// </summary>
        /// <param name="list">list to add the widgets to</param>
        public void FindAllChildren(List<Widget> list)
        {
            if (!_widget.Children.Any())
            {
                return;
            }

            try
            {
                list.AddRange(_widget.Children);
            }
            catch
            {
            }

            // now recursively descend and add children
            foreach (Widget child in _widget.Children)
            {
                child.Finder.FindAllChildren(list);
            }
        }

        /// <summary>
        /// Recursively gets all children of the specified type (eg
        /// SliderWidget, LabelWidget etc) and adds them to the list
        /// </summary>
        /// <param name="childType">Type we are looking for</param>
        /// <param name="list">Returned list</param>
        public void FindAllChildren(Type childType, List<Widget> list)
        {
            if (!_widget.Children.Any())
            {
                return;
            }

            // first check if this is a direct child
            try
            {
                foreach (Widget child in _widget.Children)
                {
                    if (childType.IsAssignableFrom(child.GetType()))
                    {
                        list.Add(child);
                    }
                }
            }
            catch
            {
            }

            // now recursively add children
            foreach (Widget child in _widget.Children)
            {
                child.Finder.FindAllChildren(childType, list);
            }
        }

        /// <summary>
        /// Recursively finds the descendent child widget with the specified
        /// name.  This means, the child does not have to be a
        /// direct child, but can be a descendent (eg child of
        /// child)
        /// </summary>
        /// <param name="name">Child to find</param>
        /// <returns>Child, null if not found</returns>
        public Widget FindChild(String name)
        {
            Widget retVal = null;

            if (!_widget.Children.Any())
            {
                return retVal;
            }

            try
            {
                // first check if this is a direct child
                foreach (Widget child in _widget.Children)
                {
                    if (String.Compare(child.Name, name) == 0)
                    {
                        retVal = child;
                        break;
                    }
                }
            }
            catch
            {
            }

            // now recursively descend and check its children
            if (retVal == null)
            {
                foreach (Widget child in _widget.Children)
                {
                    if (String.Compare(child.Name, name) == 0)
                    {
                        return child;
                    }

                    if ((retVal = child.Finder.FindChild(name)) != null)
                    {
                        return retVal;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Recursively finds the descendent child widget with the specified
        /// type.  This means, the child does not have to be a
        /// direct child, but can be a descendent (eg child of
        /// child)
        /// </summary>
        /// <param name="type">type of widget to find</param>
        /// <returns>Child, null if not found</returns>
        public Widget FindChild(Type type)
        {
            Widget retVal = null;

            if (!_widget.Children.Any())
            {
                return retVal;
            }

            // first check if this is a direct child
            try
            {
                foreach (Widget child in _widget.Children)
                {
                    if (type.IsAssignableFrom(child.GetType()))
                    {
                        retVal = child;
                        break;
                    }
                }
            }
            catch
            {
            }

            // now recursively descend and check its children
            if (retVal == null)
            {
                foreach (Widget child in _widget.Children)
                {
                    if (type.IsAssignableFrom(child.GetType()))
                    {
                        return child;
                    }

                    if ((retVal = child.Finder.FindChild(type)) != null)
                    {
                        return retVal;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Recursively finds the descendent child widgets with the specified
        /// type and adds them to the list
        /// </summary>
        /// <param name="type">Type of widget to find</param>
        /// <param name="list">list to add the widgets to</param>
        public void FindChild(Type type, List<Widget> list)
        {
            if (!_widget.Children.Any())
            {
                return;
            }

            try
            {
                foreach (Widget child in _widget.Children)
                {
                    if (type.IsAssignableFrom(child.GetType()))
                    {
                        list.Add(child);
                    }
                }
            }
            catch
            {
            }

            // now recursively descend and find children
            foreach (Widget child in _widget.Children)
            {
                child.Finder.FindChild(type, list);
            }
        }

        /// <summary>
        /// Recursively finds widgets with the specified subclass and adds them
        /// to the list.
        /// </summary>
        /// <param name="subclass"></param>
        /// <param name="list"></param>
        public void FindChild(String subclass, List<Widget> list)
        {
            if (!_widget.Children.Any())
            {
                return;
            }

            try
            {
                // first check if this is a direct child
                foreach (Widget child in _widget.Children)
                {
                    if (String.Compare(child.SubClass, subclass, true) == 0)
                    {
                        list.Add(child);
                    }
                }
            }
            catch
            {
            }

            // now recursively descend and find children
            foreach (Widget child in _widget.Children)
            {
                child.Finder.FindChild(subclass, list);
            }
        }

        /// <summary>
        /// Recursively finds the descendent child widget with the specified
        /// handle.  This means, the child does not have to be a
        /// direct child, but can be a descendent (eg child of
        /// child)
        /// </summary>
        /// <param name="handle">handle of control to find</param>
        /// <returns>Child, null if not found</returns>
        public Widget FindChild(IntPtr handle)
        {
            Widget retVal = null;

            if (!_widget.Children.Any())
            {
                return null;
            }

            try
            {
                // first check if this is a direct child
                foreach (Widget child in _widget.Children)
                {
                    if (child.UIControl != null && (child.UIControl.Handle == handle))
                    {
                        retVal = child;
                        break;
                    }
                }
            }
            catch
            {
            }

            // now recursively descend and check its children
            if (retVal == null)
            {
                foreach (Widget child in _widget.Children)
                {
                    if (child.UIControl != null && child.UIControl.Handle == handle)
                    {
                        return child;
                    }

                    if ((retVal = child.Finder.FindChild(handle)) != null)
                    {
                        return retVal;
                    }
                }
            }

            return retVal;
        }
    }
}