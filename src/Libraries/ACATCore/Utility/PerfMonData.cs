////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Data tracked by Perfomance Monitor
    /// </summary>
    public class PerfMonData
    {
        public PerfMonData()
        {
            FreeMemoryMB = -1f;
            CommittedMemoryMB = -1f;
            PrivateBytesMB = -1f;
            PageFileBytesMB = -1f;
            HandleCount = -1f;
            ProcessorUtilizationPercent = -1f;
        }

        public float CommittedMemoryMB { get; set; }
        public float FreeMemoryMB { get; set; }
        public float HandleCount { get; set; }
        public float PageFileBytesMB { get; set; }
        public float PrivateBytesMB { get; set; }
        public float ProcessorUtilizationPercent { get; set; }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append(FreeMemoryMB >= 0.0f ? FreeMemoryMB.ToString() : "--");
            sb.Append(",");
            sb.Append(CommittedMemoryMB >= 0.0f ? CommittedMemoryMB.ToString() : "--");
            sb.Append(",");
            sb.Append(PrivateBytesMB >= 0.0f ? PrivateBytesMB.ToString() : "--");
            sb.Append(",");
            sb.Append(PageFileBytesMB >= 0.0f ? PageFileBytesMB.ToString() : "--");
            sb.Append(",");
            sb.Append(HandleCount >= 0.0f ? HandleCount.ToString() : "--");
            sb.Append(",");
            sb.Append(ProcessorUtilizationPercent >= 0.0f ? ((int)ProcessorUtilizationPercent).ToString() : "--");

            return sb.ToString();
        }
    }
}