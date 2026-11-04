////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading.Tasks;

namespace ACAT.Core.Utility
{
    public static class TaskExtensions
    {
        public static void Forget(this Task task, Action<Exception> onException)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var awaiter = task.ConfigureAwait(false).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                try
                {
                    awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    try
                    {
                        onException?.Invoke(ex);
                    }
                    catch
                    {
                    }
                }
            });
        }
    }
}
