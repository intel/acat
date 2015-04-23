////////////////////////////////////////////////////////////////////////////
// <copyright file="Parser.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.Interpreter
{
    /// <summary>
    /// A script parser class. Generates intermediate pCode
    /// after interpreting.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// State of parsing
        /// </summary>
        private enum ParseState
        {
            Begin,
            ActionName,
            Parameter,
            Invalid
        }

        /// <summary>
        /// Parses an input script into intermediate code (PCode)
        /// The script is essentially a semi-colon delimited set of
        /// functions with arguments.
        ///
        /// Each function can have 0 or more arguments, each argument
        /// should be delimited by a comma.  There are no argument
        /// types.  All arguments are treated as strings.
        ///
        /// An example of a script is:
        ///   "highlightSelected(@SelectedWidget, false); select(@SelectedBox); transition(RowRotation)"
        /// This script has three function calls:
        ///     "highlightSelected" with arguments "@SelectedWidget" and "false"
        ///     "select" with argument "@SelectedBox"
        ///     "transition" with argument "RowRotation"
        /// </summary>
        /// <param name="inputString">Input script</param>
        /// <param name="pCode">Returns interpreted form</param>
        /// <returns>true if parsed successfully, false otherwise</returns>
        public bool Parse(String inputString, ref PCode pCode)
        {
            bool retVal = true;

            if (String.IsNullOrEmpty(inputString))
            {
                return retVal;
            }

            pCode.Script = inputString.Trim();
            if (String.IsNullOrEmpty(pCode.Script))
            {
                return retVal;
            }

            // semi-colon is the function separator
            var tokens = pCode.Script.Split(';');

            // now parse each function
            foreach (var token in tokens)
            {
                var trimmedToken = token.Trim();
                if (trimmedToken.Length > 0)
                {
                    var actionVerb = new ActionVerb();

                    // function may have comma delimited arguments.
                    // extract them
                    bool ret = parseActionVerb(trimmedToken, ref actionVerb);
                    if (ret)
                    {
                        pCode.ActionVerbList.Add(actionVerb);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!retVal)
            {
                pCode.ActionVerbList.Clear();
            }

            return retVal;
        }

        /// <summary>
        /// If the parseString is func(a, b, c), returns  func as the
        /// action and "a, b, c" as the retArguments
        /// </summary>
        /// <param name="parseString">Input string</param>
        /// <param name="retAction">Function name</param>
        /// <param name="retArguments">Arguments</param>
        /// <returns>true on success</returns>
        private bool getActionAndArguments(String parseString, ref String retAction, ref String retArguments)
        {
            bool retVal = true;
            bool done = false;
            int index;
            var state = ParseState.Begin;
            var action = new StringBuilder();
            var arguments = new StringBuilder();

            for (index = 0; index < parseString.Length && !done; )
            {
                char ch = parseString[index];

                switch (state)
                {
                    case ParseState.Begin:
                        state = ParseState.ActionName;
                        break;

                    case ParseState.ActionName:
                        // argument list start now
                        if (ch == '(')
                        {
                            state = ParseState.Parameter;
                            index++;
                        }
                        else if (!Char.IsLetterOrDigit(ch))
                        {
                            state = ParseState.Invalid;
                        }
                        else
                        {
                            action.Append(ch);
                            index++;
                        }

                        break;

                    case ParseState.Parameter:
                        // end of argument list
                        if (ch == ')')
                        {
                            index++;
                            done = true;
                        }
                        else
                        {
                            arguments.Append(ch);
                            index++;
                        }

                        break;

                    case ParseState.Invalid:
                        Log.Error("Parse error at index " + index + ". String: " + parseString);
                        retVal = false;
                        done = true;
                        break;
                }
            }

            if (!done)
            {
                Log.Error("Parse error at index " + index + ". String: " + parseString);
                retVal = false;
            }

            if (retVal && index < parseString.Length)
            {
                Log.Error("Parse error at index " + index + ". String: " + parseString);
                retVal = false;
            }

            if (retVal)
            {
                retAction = action.ToString().Trim();

                retVal = !String.IsNullOrEmpty(retAction) && isValidToken(retAction);
            }

            if (retVal)
            {
                retArguments = arguments.ToString().Trim();
            }

            return retVal;
        }

        /// <summary>
        /// Checks if the token is entirely made up
        /// of digits and letters
        /// </summary>
        /// <param name="token">Input token</param>
        /// <returns>true of OK</returns>
        private bool isValidToken(String token)
        {
            bool retVal = true;

            for (int index = 0; index < token.Length; index++)
            {
                if (!Char.IsLetterOrDigit(token[index]))
                {
                    retVal = false;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Takes a single function as a script, extracts the function name and the
        /// list of arguments and returns an actionVerb object the encapsulates
        /// the function name and the args
        ///
        ///  For eg, if the input script for the function is:
        ///    "highlightSelected(@SelectedWidget, false)
        /// This will result in an ActionVerb with the Name set to "highlightSelected"
        /// with argument list "@SelectedWidget" and "false"
        /// </summary>
        /// <param name="inputString">Input script</param>
        /// <param name="actionVerb">Output func name and args</param>
        /// <returns>true on success, false if there was an error</returns>
        private bool parseActionVerb(String inputString, ref ActionVerb actionVerb)
        {
            var action = String.Empty;
            var arguments = String.Empty;

            // extract the func name and the arguments
            bool retVal = getActionAndArguments(inputString, ref action, ref arguments);
            if (retVal)
            {
                // now, split the args and get a list
                var argList = new List<string>();
                retVal = splitArguments(arguments, ref argList);

                if (retVal)
                {
                    actionVerb.Action = action;
                    actionVerb.ArgList = argList;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Takes a comma delimted argument array and splits them
        /// into individual arguments
        /// </summary>
        /// <param name="strArguments">Argument string (eg "a, b, c"</param>
        /// <param name="argList">Returns "a", "b" and "c"</param>
        /// <returns></returns>
        private bool splitArguments(String strArguments, ref List<string> argList)
        {
            var arguments = strArguments.Split(',');
            foreach (var argument in arguments)
            {
                var trimmedArg = argument.Trim();
                if (!String.IsNullOrEmpty(trimmedArg))
                {
                    argList.Add(trimmedArg);
                }
            }

            return true;
        }
    }
}