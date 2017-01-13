////////////////////////////////////////////////////////////////////////////
// <copyright file="NotepadAgent.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension.AppAgents.Notepad;

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.NotepadAgent
{
    /// <summary>
    /// Application agent for the Windows Notepad application.
    /// Base class does all the heavy-lifting.  Override functions
    /// as required customize
    /// </summary>
    [DescriptorAttribute("A9CB65E6-C63B-4C47-B1DB-2010955FFD17",
                            "Notepad Agent",
                            "Manages interactions with Windows Notepad to edit text files")]
    internal class NotepadAgent : NotepadAgentBase
    {
    }
}