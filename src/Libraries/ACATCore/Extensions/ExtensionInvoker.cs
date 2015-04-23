////////////////////////////////////////////////////////////////////////////
// <copyright file="ExtensionInvoker.cs" company="Intel Corporation">
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
using System.Reflection;
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Lib.Core.Extensions
{
    /// <summary>
    /// Delegate for an event raised by the invoker
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="args">event argument</param>
    public delegate void OnExtensionEvent(object sender, ExtensionEventArgs args);

    /// <summary>
    /// This is a wrapper class for C# reflection which enables
    /// the caller to easily invoke methods, properties and raise
    /// events from any class that supports the IExtension interface.
    /// This can be used for instance to invoke methods/properties in
    /// ACAT extensions which are all DLL's.
    /// </summary>
    public class ExtensionInvoker : IExtension
    {
        /// <summary>
        /// The object whose methods/properties/events are to
        /// be invoked through reflection
        /// </summary>
        private readonly object _objThis;

        /// <summary>
        /// Stores event subscribers
        /// </summary>
        private OnExtensionEvent _eventDelegate;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ExtensionInvoker()
        {
            _objThis = this;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="obj">this object</param>
        public ExtensionInvoker(object obj)
        {
            _objThis = obj;
        }

        /// <summary>
        /// Adds or removes subscribers
        /// </summary>
        public virtual event OnExtensionEvent EvtExtensionEvent
        {
            add
            {
                _eventDelegate += value;
            }

            remove
            {
                _eventDelegate -= value;
            }
        }

        /// <summary>
        /// If the property is an boolean, returns its value.
        /// Null if invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <returns>value of the property</returns>
        public bool? GetBoolValue(String property)
        {
            object retVal = GetValue(property);
            return (retVal is bool) ? retVal as bool? : null;
        }

        /// <summary>
        /// Returns the value of the property if it is
        /// a bool. defaultValue if invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>value of the property</returns>
        public bool GetBoolValue(String property, bool defaultValue)
        {
            return GetBoolValue(property) ?? defaultValue;
        }

        /// <summary>
        /// If the property is an integer, returns its value
        /// null if invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <returns>value of the property</returns>
        public int? GetIntValue(String property)
        {
            object retVal = GetValue(property);
            return (retVal is int) ? retVal as int? : null;
        }

        /// <summary>
        /// Returns the value of the property if it is
        /// an integer. defaultValue if invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>value of the property</returns>
        public int GetIntValue(String property, int defaultValue)
        {
            return GetIntValue(property) ?? defaultValue;
        }

        /// <summary>
        /// Gets the extension invoker object from the target.
        /// This will be used to access methods/properties
        /// through reflection
        /// </summary>
        /// <returns>the extension invoker object</returns>
        public virtual ExtensionInvoker GetInvoker()
        {
            return (_objThis is ExtensionInvoker) ? _objThis as ExtensionInvoker : null;
        }

        /// <summary>
        /// If the property is a String, returns its value.
        /// null if invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <returns>value of the property</returns>
        public String GetStringValue(String property)
        {
            object retVal = GetValue(property);
            return (retVal is String) ? retVal as String : null;
        }

        /// <summary>
        /// Returns the value of the property if it is
        /// a string. defaultValue if invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>value of the property</returns>
        public String GetStringValue(String property, String defaultValue)
        {
            return GetStringValue(property) ?? defaultValue;
        }

        /// <summary>
        /// Returns the value of the property. Null if
        /// invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <returns>value of the property</returns>
        public object GetValue(String property)
        {
            try
            {
                MemberInfo[] member = _objThis.GetType().GetMember(property);
                if (member.Length > 0)
                {
                    switch (member[0].MemberType)
                    {
                        case MemberTypes.Field:
                            FieldInfo fieldInfo = _objThis.GetType().GetField(property);
                            if (fieldInfo != null)
                            {
                                return fieldInfo.GetValue(_objThis);
                            }

                            break;

                        case MemberTypes.Property:
                            return _objThis.GetType().GetProperty(property).GetValue(_objThis, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return null;
        }

        /// <summary>
        /// Returns the value of the property. defaultValue if
        /// invalid property
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>value of the property</returns>
        public object GetValue(String property, object defaultValue)
        {
            return GetValue(property) ?? defaultValue;
        }

        /// <summary>
        /// Invokes methodName through reflection.  Args are
        /// the optional arguments to the method.  Returns null
        /// if method is invalid
        /// </summary>
        /// <param name="methodName">name of the method </param>
        /// <param name="args">arguments</param>
        /// <returns>the return value of the method</returns>
        public object InvokeExtensionMethod(string methodName, params object[] args)
        {
            object result = null;

            try
            {
                var methodInfo = _objThis.GetType().GetMethod(methodName);
                if (methodInfo != null)
                {
                    result = methodInfo.Invoke(_objThis, args);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Raises an event synchronously
        /// </summary>
        /// <param name="args">Event args</param>
        public void NotifyEvent(ExtensionEventArgs args)
        {
            if (_eventDelegate != null)
            {
                _eventDelegate(_objThis, args);
            }
        }

        /// <summary>
        /// Raises an event asynchronously
        /// </summary>
        /// <param name="args">event args</param>
        public void NotifyEventAsync(ExtensionEventArgs args)
        {
            if (_eventDelegate != null)
            {
                _eventDelegate.BeginInvoke(_objThis, args, null, null);
            }
        }

        /// <summary>
        /// Sets the value of the property to value
        /// </summary>
        /// <param name="property">name of the property</param>
        /// <param name="value">value to set</param>
        /// <returns>true on success</returns>
        public bool SetValue(String property, object value)
        {
            bool retVal = true;
            try
            {
                MemberInfo[] member = _objThis.GetType().GetMember(property);
                if (member.Length > 0)
                {
                    switch (member[0].MemberType)
                    {
                        case MemberTypes.Field:
                            FieldInfo fieldInfo = _objThis.GetType().GetField(property);
                            if (fieldInfo != null)
                            {
                                fieldInfo.SetValue(_objThis, value);
                            }

                            break;

                        case MemberTypes.Property:
                            _objThis.GetType().GetProperty(property).SetValue(_objThis, value, null);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Check whether the class has a method with
        /// the name methodName
        /// </summary>
        /// <param name="methodName">name of the method</param>
        /// <returns>true if it does</returns>
        public bool SupportsMethod(String methodName)
        {
            return _objThis.GetType().GetMethod(methodName) != null;
        }
    }
}