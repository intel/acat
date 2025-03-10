﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Blocking queue on which work items can be enqueued.
    /// Dequeue is blocking until something arrives on the queue
    /// </summary>
    /// <typeparam name="T">data type to enqueue</typeparam>
    public class BlockingQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// The queue to hold the items
        /// </summary>
        private readonly Queue<T> _queue = new Queue<T>();

        /// <summary>
        /// How many itmes are in the queue?
        /// </summary>
        private int _count;

        /// <summary>
        /// Clears the queue of all pending items
        /// </summary>
        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        /// <summary>
        /// Checks if the queue has the specified object
        /// </summary>
        /// <param name="obj">object to check for</param>
        /// <returns>true if it does</returns>
        public bool Contains(T obj)
        {
            lock (_queue)
            {
                return _queue.Contains(obj);
            }
        }

        /// <summary>
        /// Removes next item. If queue is empty,
        /// blocks
        /// </summary>
        /// <returns>next item</returns>
        public T Dequeue()
        {
            lock (_queue)
            {
                while (_count <= 0) Monitor.Wait(_queue);
                _count--;
                return _queue.Dequeue();
            }
        }

        /// <summary>
        /// Enqueues the item and pulses to indicate
        /// there is something there
        /// </summary>
        /// <param name="data">item to enqueue</param>
        public void Enqueue(T data)
        {
            if (data == null) throw new ArgumentNullException("data");
            lock (_queue)
            {
                _queue.Enqueue(data);
                _count++;
                Monitor.Pulse(_queue);
            }
        }

        /// <summary>
        /// Returns enumerator for the queue
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        /// <summary>
        /// Returns enumerator to peek into the queue
        /// </summary>
        /// <returns>enumerator</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            while (true) yield return Dequeue();
        }
    }
}