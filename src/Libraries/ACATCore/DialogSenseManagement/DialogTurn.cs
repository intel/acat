////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Dialog.cs
//
// Holds a text of dialog
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.DialogSenseManagement
{
    public enum DialogTurnType
    {
        User,
        Other
    }

    public class DialogTurn
    {
        public DialogTurn(DialogTurnType turnType, String text)
        {
            TurnType = turnType;
            Text = text;
        }

        public String Text { get; set; }
        public DialogTurnType TurnType { get; set; }
    }
}