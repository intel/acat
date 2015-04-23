////////////////////////////////////////////////////////////////////////////
// <copyright file="AnimationsCollection.cs" company="Intel Corporation">
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
using System.IO;
using System.Xml;
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

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Represents a collection of all the animations
    /// grouped by the animation name.
    ///
    /// The hierarchy is as follows
    ///     AnimationsCollection (collection of animations indexed by animation name )
    ///        Animations  (collection of animations for a animation name)
    ///           Animation  (a single animation)
    /// </summary>
    public class AnimationsCollection : IDisposable
    {
        private readonly Dictionary<String, Animations> _animationsTable;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes the AnimationCollections object
        /// </summary>
        public AnimationsCollection()
        {
            _animationsTable = new Dictionary<String, Animations>();
        }

        /// <summary>
        /// Gets the total number of animations objects
        /// </summary>
        public int Count
        {
            get { return _animationsTable.Count; }
        }

        /// <summary>
        /// Gets the collection of all the animations
        /// </summary>
        public ICollection<Animations> Values
        {
            get { return _animationsTable.Values; }
        }

        /// <summary>
        /// Returns the Animations object for the specified name
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Animations this[String arg]
        {
            get { return !_animationsTable.ContainsKey(arg) ? null : _animationsTable[arg]; }
        }

        /// <summary>
        /// Object disposer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Load all the animations from the config file (from ACAT/Animations node)
        /// </summary>
        /// <param name="configFile">Name of the config file</param>
        /// <returns>true on success</returns>
        public bool Load(String configFile)
        {
            var doc = new XmlDocument();

            if (!File.Exists(configFile))
            {
                return false;
            }
            doc.Load(configFile);

            XmlNodeList animationsNodes = doc.SelectNodes("/ACAT/Animations");

            if (animationsNodes == null)
            {
                return false;
            }

            // load all the animations
            foreach (XmlNode node in animationsNodes)
            {
                var name = XmlUtils.GetXMLAttrString(node, "name");
                if (String.IsNullOrEmpty(name))
                {
                    name = "default";
                }

                if (!_animationsTable.ContainsKey(name))
                {
                    Animations animations = findOrCreateAnimationsEntry(name);
                    animations.Load(node);
                }
            }

            return true;
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                    foreach (Animations animations in _animationsTable.Values)
                    {
                        animations.Dispose();
                    }

                    _animationsTable.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Checks if there is already an entry for specified
        /// screen.  If so returns it otherwise, creates a new one
        /// </summary>
        /// <param name="name">Name of the screen</param>
        /// <returns>The animations object for the screen</returns>
        private Animations findOrCreateAnimationsEntry(String name)
        {
            Animations animations = this[name];
            if (animations == null)
            {
                animations = new Animations { Name = name };
                _animationsTable.Add(name, animations);
            }

            return animations;
        }
    }
}