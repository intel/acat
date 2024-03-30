////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACAT.Extensions.BCI.Common.AnimationSharp
{
    public class BCIUtils
    {
        /// <summary>
        /// Array of ID to be used as targe for calibration
        /// </summary>
        private int[] _IDs = null;

        /// <summary>
        /// Original length when creating the id array
        /// </summary>
        private int _OriginalLengthOfArray = 0;

        /// <summary>
        /// Create a entry log used by BCI
        /// </summary>
        /// <param name="highlight">highlight button/box</param>
        /// <param name="triggerbox">trigger highlihted</param>
        /// <param name="idButtons">list of buttons ID's</param>
        /// <param name="idRowColumn">ID of active row or column</param>
        /// <param name="targetdecision">is target or decision</param>
        public string GetEntryLogStr(BCIModes bCIModes, BCIScanSections bCIScanSections, bool highlight, bool triggerbox, List<int> idButtons, int idRowColumn, bool targetdecision)
        {
            StringBuilder sb = new StringBuilder();
            string highlightstr = "ON,";
            string triggerboxstr = "ON,";
            try
            {
                sb.Append(bCIModes.ToString() + ",");
                sb.Append(bCIScanSections.ToString() + ",");
                switch (bCIModes)
                {
                    case BCIModes.CALIBRATION:
                        if (targetdecision)
                            sb.Append("Target,");
                        else
                            sb.Append("Trial,");
                        break;

                    case BCIModes.TYPING:
                        if (targetdecision)
                            sb.Append("Decision,");
                        else
                            sb.Append("Trial,");
                        break;
                }
                if (!highlight)
                    highlightstr = "OFF,";
                if (!triggerbox)
                    triggerboxstr = "OFF,";
                sb.Append(highlightstr);
                sb.Append(triggerboxstr);
                if (targetdecision)
                {
                    sb.Append("1000");
                    if (bCIScanSections == BCIScanSections.Box)
                        sb.Append("," + (idButtons[0] + 1));
                    else
                    {
                        for (int i = 0; i < idButtons.Count; i++)
                        {
                            sb.Append("," + (idButtons[i] + 1));
                        }
                    }
                }
                else
                {
                    sb.Append((idRowColumn + 1));
                    if (bCIScanSections == BCIScanSections.Box)
                        sb.Append("," + (idRowColumn + 1));
                    else
                    {
                        for (int i = 0; i < idButtons.Count; i++)
                        {
                            sb.Append("," + idButtons[i]);
                        }
                    }
                }
            }
            catch (Exception es)
            {
                Log.Debug("Error no highlight log saved: " + es);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get a random id from the available targets of the calibration id's
        /// </summary>
        /// <returns>ID</returns>
        public int GetRandomID()
        {
            return GetAndRemoveRandomValue();
        }

        /// <summary>
        /// Initialize the array of id's
        /// </summary>
        /// <param name="length"></param>
        public void SetTargetValuesForCalibration(int length)
        {
            _OriginalLengthOfArray = length;
            _IDs = new int[_OriginalLengthOfArray];
            for (int i = 0; i < _OriginalLengthOfArray; i++)
            {
                _IDs[i] = i;
            }
        }

        /// <summary>
        /// Gets an id and removes it from the array so next value is random and different
        /// </summary>
        /// <returns></returns>
        private int GetAndRemoveRandomValue()
        {
            try
            {
                if (_IDs == null || _IDs.Length == 0)
                {
                    _IDs = Enumerable.Range(0, _OriginalLengthOfArray).ToArray();
                }
                Random rand = new Random();
                int randomIndex = rand.Next(0, _IDs.Length);
                int randomValue = _IDs[randomIndex];

                int lastIndex = _IDs.Length - 1;
                _IDs[randomIndex] = _IDs[lastIndex];
                Array.Resize(ref _IDs, lastIndex);

                return randomValue;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}