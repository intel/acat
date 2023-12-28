////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Represents a collection of animations for a panel
    ///
    /// The hierarchy is as follows
    ///     AnimationsCollection (collection of animations indexed by animation namename)
    ///        Animations  (collection of animations for a specific animation name)
    ///           Animation  (a single animation)
    /// </summary>
    public class Animations : IDisposable
    {
        private readonly Dictionary<String, Animation> _animationTable;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes the Animations object
        /// </summary>
        public Animations()
        {
            _animationTable = new Dictionary<String, Animation>();
        }

        /// <summary>
        /// Gets or sets the name of the collection
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets a the list of all aniamtion objects in this collection
        /// </summary>
        public ICollection<Animation> Values
        {
            get { return _animationTable.Values; }
        }

        /// <summary>
        /// Gets the animation object at the specified index
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>animation object</returns>
        public Animation this[String index]
        {
            get
            {
                Animation retVal;
                _animationTable.TryGetValue(index, out retVal);
                return retVal;
            }
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the first animation sequence in the
        /// collection.  This is the one that has the
        /// "start" attribute set to true in the xml file
        /// </summary>
        /// <returns>first animation object</returns>
        public Animation GetFirst()
        {
            return _animationTable.Values.FirstOrDefault(animation => animation.IsFirst);
        }

        /// <summary>
        /// Load animations from xml file and creats a list of animation objects
        /// </summary>
        /// <param name="animationsRootNode">The xml node containing the list</param>
        /// <returns>true on success</returns>
        public bool Load(XmlNode animationsRootNode)
        {
            XmlNodeList animationNodes = animationsRootNode.SelectNodes("Animation");

            if (animationNodes == null)
            {
                return false;
            }

            // load all the animation elements
            foreach (XmlNode node in animationNodes)
            {
                Animation animation = createAndLoadAnimationEntry(node);
                if (!_animationTable.ContainsKey(animation.Name))
                {
                    _animationTable.Add(animation.Name, animation);
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
                if (disposing)
                {
                    // dispose all managed resources.
                    foreach (Animation animation in _animationTable.Values)
                    {
                        animation.Dispose();
                    }

                    _animationTable.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Creates an animation entry reading info from the xml node
        /// </summary>
        /// <param name="xmlNode">The input xml node</param>
        /// <returns>Animation entry</returns>
        private Animation createAndLoadAnimationEntry(XmlNode xmlNode)
        {
            var animation = new Animation();

            animation.Load(xmlNode);

            return animation;
        }
    }
}