////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// QueExtensions.cs
//
// Extensions to Queue type
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGUtils
{
    public static class QueueExtensions
    {
        /// <summary>
        /// Dequeues chunk (of chunk size) in queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<T> DequeueChunk<T>(this Queue<T> queue, int chunkSize)
        {
            for (int i = 0; i < chunkSize && queue.Count > 0; i++)
            {
                yield return queue.Dequeue();
            }
        }
    }
}