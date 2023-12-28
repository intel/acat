////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Helper class to find widgets within a parent widget. Supports
    /// different ways of searching- by name, by the .NET class type etc.
    /// Does a recursive depth search for the widget through all the
    /// children of the parent widget
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

        /// <summary>
        /// Recursively finds the default home widget for manual scanning
        /// </summary>
        /// <returns>Child, null if not found</returns>
        public Widget FindDefaultHome()
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
                    if (child.DefaultHome)
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
                    if (child.DefaultHome)
                    {
                        return child;
                    }

                    if ((retVal = child.Finder.FindDefaultHome()) != null)
                    {
                        return retVal;
                    }
                }
            }

            return retVal;
        }
    }
}