////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Represents log entry of audit events related to
    /// text to speech conversions
    /// </summary>
    public class AuditEventTextToSpeech : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventTextToSpeech(String text = null)
            : base("TextToSpeech")
        {
            SpeechEngine = String.Empty;
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="speechEngine">name of the active speech engine</param>
        public AuditEventTextToSpeech(String speechEngine, String text = null)
            : base("TextToSpeech")
        {
            SpeechEngine = speechEngine;
            Text = text;
        }

        /// <summary>
        /// Gets or sets name of the active speech engine
        /// </summary>
        public String SpeechEngine { get; set; }

        public String Text { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            var str = String.IsNullOrEmpty(SpeechEngine) ? TTSManagement.TTSManager.Instance.ActiveEngine.Descriptor.Name : SpeechEngine;
            if (String.IsNullOrEmpty(str))
            {
                str = "<null>";
            }

            return formatEventString(str, (String.IsNullOrEmpty(Text) ? String.Empty : Text));
        }
    }
}