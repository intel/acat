////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowContextMenu.cs" company="Intel Corporation">
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

using ACAT.ACATResources;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.AppAgents.TalkWindowAgent
{
    /// <summary>
    /// The contextual menu for the ACAT talk window. Handles
    /// Google sesrch, wikipedia search of term entered in the Talk
    /// window.  Handles volume setting and talk window zoom.
    /// </summary>
    [DescriptorAttribute("DEA66EFD-060B-4033-8760-301C7FAFCCA8",
                            "TalkWindowContextMenu",
                            "Talk Window Contextual AppMenu")]
    public partial class TalkWindowContextMenu : MenuPanel
    {
        /// <summary>
        /// Use internet explorer as the browser
        /// </summary>
        private readonly WebSearch _webSearch = new WebSearch(Common.AppPreferences.PreferredBrowser);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Name of the scanner</param>
        /// <param name="panelTitle">Title of the menu</param>
        public TalkWindowContextMenu(String panelClass, String panelTitle)
            : base(panelClass, R.GetString("TalkWindow"))
        {
            // commands supported by the contextual menu
            commandDispatcher.Commands.Add(new CommandHandler(this, "QuickSearch"));
            commandDispatcher.Commands.Add(new CommandHandler(this, "GoogleSearch"));
            commandDispatcher.Commands.Add(new CommandHandler(this, "WikiSearch"));
            commandDispatcher.Commands.Add(new CommandHandler(this, "Volume"));
            commandDispatcher.Commands.Add(new CommandHandler(this, "SpeechControl"));
            commandDispatcher.Commands.Add(new CommandHandler(this, "talkWindowZoomMenu"));
        }

        /// <summary>
        /// Returns either the selected text or the current paragraph in
        /// the talk window
        /// </summary>
        /// <returns>the text</returns>
        internal static String getTalkWindowText()
        {
            var retVal = String.Empty;

            try
            {
                if (Context.AppTalkWindowManager.IsTalkWindowVisible)
                {
                    using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                    {
                        retVal = context.TextAgent().GetSelectedText().Trim();
                        if (String.IsNullOrEmpty(retVal))
                        {
                            context.TextAgent().GetParagraphAtCaret(out retVal);
                            retVal = retVal.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return retVal;
        }

        /// <summary>
        /// Takes text from the talk window and does a google search on it
        /// </summary>
        private void handleGoogleSearch()
        {
            String text = getTalkWindowText();
            if (!String.IsNullOrEmpty(text.Trim()))
            {
                if (Context.AppTalkWindowManager.IsTalkWindowVisible)
                {
                    Context.AppTalkWindowManager.ToggleTalkWindow();
                }

                _webSearch.GoogleSearch(text);
            }
            else
            {
                DialogUtils.ShowTimedDialog(
                                PanelManager.Instance.GetCurrentPanel() as Form,
                                R.GetString("GoogleSearch"),
                                R.GetString("SearchTextEmpty"));
            }
        }

        /// <summary>
        /// Takes text from the talk window and goes to the first hit
        /// found when a search is done.
        /// </summary>
        private void handleQuickSearch()
        {
            String text = getTalkWindowText();
            if (!String.IsNullOrEmpty(text.Trim()))
            {
                if (Context.AppTalkWindowManager.IsTalkWindowVisible)
                {
                    Context.AppTalkWindowManager.ToggleTalkWindow();
                }

                _webSearch.QuickSearch(text);
            }
            else
            {
                DialogUtils.ShowTimedDialog(
                            PanelManager.Instance.GetCurrentPanel() as Form,
                            R.GetString("QuickSearch"),
                            R.GetString("SearchTextEmpty"));
            }
        }

        /// <summary>
        /// If text-to-speech is handled by hardware, this function
        /// can be used to send any escape sequences to control
        /// speech parameters in the hw.
        /// First checks if the speech controller supports
        /// the feature.
        /// </summary>
        private void handleSpeechControl()
        {
            String word = String.Empty;

            if (!Context.AppTTSManager.ActiveEngine.GetInvoker().SupportsMethod("SpeechControl") ||
                !Context.AppTTSManager.ActiveEngine.GetInvoker().SupportsMethod("IsValidSpeechControlSequence"))
            {
                return;
            }

            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    context.TextAgent().GetWordAtCaret(out word);
                }

                object ret = Context.AppTTSManager.ActiveEngine.GetInvoker()
                                    .InvokeExtensionMethod("IsValidSpeechControlSequence", word);
                bool retVal = false;
                if (ret is bool)
                {
                    retVal = (bool)ret;
                }

                if (retVal)
                {
                    var prompt = R.GetString("SendSpeechControlSequence");
                    if (Context.AppTTSManager.ActiveEngine.GetInvoker().SupportsMethod("GetSpeechControlPrompt"))
                    {
                        ret = Context.AppTTSManager.ActiveEngine.GetInvoker()
                                .InvokeExtensionMethod("GetSpeechControlPrompt", word);
                        if (ret is String)
                        {
                            prompt = ret as String;
                        }

                        if (DialogUtils.ConfirmScanner(prompt))
                        {
                            Context.AppTTSManager.ActiveEngine.GetInvoker().InvokeExtensionMethod("SpeechControl", word);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Displays the talk window zoom menu
        /// </summary>
        private void handleTalkWindowZoom()
        {
            if (Context.AppTalkWindowManager.IsTalkWindowVisible)
            {
                scannerCommon.KeepTalkWindowActive = true;

                Context.AppTalkWindowManager.ZoomModeEnter();
                showZoomMenu();
                Context.AppTalkWindowManager.ZoomModeExit();
            }
        }

        /// <summary>
        /// Does a wikipedia search on text in the talk window
        /// </summary>
        private void handleWikiSearch()
        {
            String text = getTalkWindowText();
            if (!String.IsNullOrEmpty(text.Trim()))
            {
                if (Context.AppTalkWindowManager.IsTalkWindowVisible)
                {
                    Context.AppTalkWindowManager.ToggleTalkWindow();
                }

                _webSearch.WikiSearch(text);
            }
            else
            {
                DialogUtils.ShowTimedDialog(PanelManager.Instance.GetCurrentPanel() as Form,
                                                R.GetString("WikiSearch"),
                                                R.GetString("SearchTextEmpty"));
            }
        }

        /// <summary>
        /// Displays the zoom menu for the talk window
        /// </summary>
        private void showZoomMenu()
        {
            var zoomMenuForm = Context.AppPanelManager.CreatePanel("TalkWindowZoomMenu", R.GetString("TalkWindow"));
            if (zoomMenuForm != null)
            {
                Context.AppPanelManager.ShowDialog(zoomMenuForm as IPanel);
            }
        }

        /// <summary>
        /// The command handler class that executes commands in the
        /// contextual menu for the talk window
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// The menu object
            /// </summary>
            private readonly TalkWindowContextMenu _menu;

            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="talkWindowMenu">the menu object</param>
            /// <param name="cmd">command to execute</param>
            public CommandHandler(TalkWindowContextMenu talkWindowMenu, String cmd)
                : base(cmd)
            {
                _menu = talkWindowMenu;
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">set to true if handled</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                switch (Command)
                {
                    case "QuickSearch":
                        _menu.handleQuickSearch();
                        break;

                    case "GoogleSearch":
                        _menu.handleGoogleSearch();
                        break;

                    case "WikiSearch":
                        _menu.handleWikiSearch();
                        break;

                    case "Volume":
                        DialogUtils.LaunchVolumeSettingsAgent();
                        break;

                    case "SpeechControl":
                        _menu.handleSpeechControl();
                        _menu.commandDispatcher.Dispatch("CmdGoBack", ref handled);
                        break;

                    case "talkWindowZoomMenu":
                        _menu.handleTalkWindowZoom();
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }
        }
    }
}