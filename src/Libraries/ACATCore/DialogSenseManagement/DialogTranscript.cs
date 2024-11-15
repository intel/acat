////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DialogContext.cs
//
// Maintains a history of the dialog
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Lib.Core.DialogSenseManagement
{
    public class DialogTranscript
    {
        private List<DialogTurn> Turns;


        public delegate void TurnAddedDelegate(DialogTurn turn);

        public event TurnAddedDelegate EvtTurnAdded;

        public delegate void TranscriptClearedDelegate();

        public event TranscriptClearedDelegate EvtTranscriptCleared;

        public DialogTranscript()
        {
            Clear();
        }

        public void AddTurn(DialogTurn turn)
        {
            Turns.Add(turn);

            EvtTurnAdded?.Invoke(turn);
        }

        public void Clear()
        {
            Turns = new List<DialogTurn>();

            EvtTranscriptCleared?.Invoke();
        }

        public override String ToString()
        {
            return ToString(0);
        }

        public String ToString(int numTurns)
        {
            if (numTurns <= 0)
            {
                numTurns = Turns.Count;
            }

            int startIndex = Turns.Count - numTurns;

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            StringBuilder sb = new StringBuilder();

            for (int ii = startIndex; ii < Turns.Count; ii++)
            {
                sb.Append(Turns[ii].TurnType + ":" + Turns[ii].Text + "\n");
            }

            return sb.ToString();
        }
    }
}
