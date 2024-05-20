////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Core.WordPredictionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// This is a helper class exclusively for Alphabet scanners.
    /// It does a lot of the heavy lifting required for word predictions
    /// for instance.  This eases coding for Alphabet scanners.
    /// Create an object of this type in the Alphabet scanner class and call
    /// the functions here when needed
    /// </summary>
    public class UserControlWordPredictionCommon : IDisposable
    {
        /// <summary>
        /// Widget that represents the alphabet scanner
        /// </summary>
        public Widget _rootWidget;

        /// <summary>
        /// The parent alphabet scanner form
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// The widget that holds the word currently being typed
        /// </summary>
        private CurrentWordWidget _currentWordWidget;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Widget that holds the prediction letter list
        /// </summary>
        private LetterListWidget _letterListWidgetWidget;

        private readonly PredictionTypes[] _predictionTypes;

        private SentenceListWidget _sentenceListWidget;

        private readonly TextController _textController;

        /// <summary>
        /// Widget that holds the prediction word list
        /// </summary>
        private WordListWidget _wordListWidgetWidget;

        /// <summary>
        /// Constructor. Initialize the various controls and
        /// display the UI
        /// </summary>
        public UserControlWordPredictionCommon(IUserControl userControl, TextController textController, IScannerPanel scannerPanel, PredictionTypes[] predictionTypes)
        {
            _form = scannerPanel.Form;
            _textController = textController;
            _predictionTypes = predictionTypes;
        }

        /// <summary>
        /// Gets or sets how many predicted letters to display
        /// </summary>
        public int LetterPredictionLetterCountMax { get; set; }

        public int SentencePredictionCountMax { get; set; }

        /// <summary>
        /// Gets or sets how many predicted words to display
        /// </summary>
        public int WordPredictionWordCountMax { get; set; }

        /// <summary>
        /// Autocompletes by selecting the first word in the
        /// word prediciton list
        /// </summary>
        public void AutocompleteWithFirstWord()
        {
            List<Widget> list = new List<Widget>();
            _rootWidget.Finder.FindAllChildren(typeof(WordListItemWidget), list);

            if (list.Any())
            {
                autoComplete(list[0] as WordListItemWidget);
            }
        }

        /// <summary>
        /// Call this in the Dispose() function of the Alphabet scanner
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Call this in the Initialize function of the Alphabet scanner
        /// </summary>
        /// <param name="startupArg">startup arguments</param>
        /// <returns>true on success</returns>
        public bool Initialize(Widget rootWidget)
        {
            _rootWidget = rootWidget;

            return true;
        }

        /// <summary>
        /// Release resources and stop threads/timers. Call this in the OnClosing()
        /// function in the Alphabet scanner.
        /// </summary>
        public void OnClosing(object sender, FormClosingEventArgs e)
        {
            Log.Debug();
        }

        /// <summary>
        /// Scanner is closing. Call this in the OnFormClosing
        /// function in the Alphabet scanner.
        /// </summary>
        /// <param name="e">event args</param>
        public void OnFormClosing(FormClosingEventArgs e)
        {
            Context.AppAgentMgr.EvtTextChanged -= AppAgent_EvtTextChanged;
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtWordPredictionAsyncResponse -= ActiveWordPredictor_EvtWordPredictionAsyncResponse;
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged -= ActiveWordPredictor_EvtModeChanged;
            KeyStateTracker.EvtKeyStateChanged -= KeyStateTracker_EvtKeyStateChanged;
        }

        /// <summary>
        /// The form has loaded.  Start the animation sequence. Call this from
        /// the OnLoad function in the Alphabet scanner
        /// </summary>
        public void OnLoad()
        {
            subscribeToEvents();

            _currentWordWidget = (CurrentWordWidget)_rootWidget.Finder.FindChild(typeof(CurrentWordWidget));
            _wordListWidgetWidget = (WordListWidget)_rootWidget.Finder.FindChild(typeof(WordListWidget));
            _sentenceListWidget = (SentenceListWidget)_rootWidget.Finder.FindChild(typeof(SentenceListWidget));
            _letterListWidgetWidget = (LetterListWidget)_rootWidget.Finder.FindChild(typeof(LetterListWidget));

            if (WordPredictionWordCountMax <= 0 && _wordListWidgetWidget != null)
            {
                WordPredictionWordCountMax = _wordListWidgetWidget.Children.Count();
            }
            if (LetterPredictionLetterCountMax <= 0 && _letterListWidgetWidget != null)
            {
                LetterPredictionLetterCountMax = _letterListWidgetWidget.Children.Count();
            }
            if (SentencePredictionCountMax <= 0 && _sentenceListWidget != null)
            {
                SentencePredictionCountMax = _sentenceListWidget.Children.Count();
            }

            refreshWordPredictionsAndSetCurrentWord();
        }

        /// <summary>
        /// Pause the application. Call this in the OnPause
        /// function in the Alphabet scanner.
        /// </summary>
        public void OnPause()
        {
            Log.Debug();
        }

        /// <summary>
        /// Resumes the application. Call this in the OnResume
        /// function in the Alphabet scanner.
        /// </summary>
        public void OnResume()
        {
            Log.Debug();

            refreshWordPredictionsAndSetCurrentWord();
        }

        /// <summary>
        /// Triggered when a widget is actuated. Call this in the OnWidgetActuated
        /// function in the Alphabet scanner.
        /// </summary>
        /// <param name="widget">widget that was actuated</param>
        /// <param name="handled">set to true if handled</param>
        public virtual void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            if (e.SourceWidget is WordListItemWidget)
            {
                CoreGlobals.Stopwatch1.Reset();
                CoreGlobals.Stopwatch1.Start();

                _form.Invoke(new MethodInvoker(delegate
                {
                    autoComplete(e.SourceWidget as WordListItemWidget);
                }));

                CoreGlobals.Stopwatch1.Stop();

                Log.Debug("TimeElapsed 3 : " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);

                handled = true;
            }
            else
            {
                handled = false;
            }
            if (e.SourceWidget is LetterListItemWidget)
            {
                CoreGlobals.Stopwatch1.Reset();
                CoreGlobals.Stopwatch1.Start();

                _form.Invoke(new MethodInvoker(delegate
                {
                    autoComplete(e.SourceWidget as LetterListItemWidget);
                }));

                CoreGlobals.Stopwatch1.Stop();

                Log.Debug("TimeElapsed 3 : " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);

                handled = true;
            }
            else
            {
                handled = false;
            }
            if (e.SourceWidget is SentenceListItemWidget)
            {
                CoreGlobals.Stopwatch1.Reset();
                CoreGlobals.Stopwatch1.Start();

                _form.Invoke(new MethodInvoker(delegate
                {
                    autoComplete(e.SourceWidget as SentenceListItemWidget);
                }));

                CoreGlobals.Stopwatch1.Stop();

                Log.Debug("TimeElapsed 3 : " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);

                handled = true;
            }
            else
            {
                handled = false;
            }
        }

        /// <summary>
        /// Disposer. Releases resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                    Context.AppWordPredictionManager.ActiveWordPredictor.EvtWordPredictionAsyncResponse -= ActiveWordPredictor_EvtWordPredictionAsyncResponse;
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        private void ActiveWordPredictor_EvtModeChanged(WordPredictionModes newMode)
        {
            refreshWordPredictionsAndSetCurrentWord();
        }

        private void ActiveWordPredictor_EvtWordPredictionAsyncResponse(WordPredictionResponse response)
        {
            if (response.Request.PredictionType == PredictionTypes.Words)
            {
                processWordPredictionResponse(response);
            }
            else if (response.Request.PredictionType == PredictionTypes.Sentences)
            {
                processSentencePredictionResponse(response);
            }
        }

        /// <summary>
        /// Something changed in the output text window.  Sest the
        /// word in the "CurrentWordWidget" box and do a fresh
        /// word prediction
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppAgent_EvtTextChanged(object sender, EventArgs e)
        {
            Log.Debug();
            try
            {
                if (_form.Visible)
                {
                    refreshWordPredictionsAndSetCurrentWord();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            Log.Debug("returning");
        }

        private void autoComplete(WordListItemWidget wordListItemWidget)
        {
            Log.Debug("wordListItemName : " + wordListItemWidget.Name + ", value: " + wordListItemWidget.Value);

            var wordSelected = wordListItemWidget.Value.Trim();

            if (!String.IsNullOrEmpty(wordSelected))
            {
                KeyStateTracker.ClearAlt();
                KeyStateTracker.ClearCtrl();

                _textController.AutoCompleteWord(wordSelected);
                AuditLog.Audit(new AuditEventAutoComplete(wordListItemWidget.Name));
            }
        }

        private void autoComplete(LetterListItemWidget letterListItemWidget)
        {
            Log.Debug("leterListItemName : " + letterListItemWidget.Name + ", value: " + letterListItemWidget.Value);

            var letterSelected = letterListItemWidget.Value.Trim();

            if (!String.IsNullOrEmpty(letterSelected))
            {
                KeyStateTracker.ClearAlt();
                KeyStateTracker.ClearCtrl();
                _textController.HandleAlphaNumericChar(null, letterListItemWidget.Value[0]);
            }
        }

        private void autoComplete(SentenceListItemWidget sentenceListItemWidget)
        {
            Log.Debug("sentenceListItemName : " + sentenceListItemWidget.Name + ", value: " + sentenceListItemWidget.Value);

            var wordSelected = sentenceListItemWidget.Value.Trim();

            if (!String.IsNullOrEmpty(wordSelected))
            {
                KeyStateTracker.ClearAlt();
                KeyStateTracker.ClearCtrl();
                _textController.AutoCompleteWord(wordSelected);
                AuditLog.Audit(new AuditEventAutoComplete(sentenceListItemWidget.Name));
            }
        }

        /// <summary>
        /// Checks if the word is in the list
        /// </summary>
        /// <param name="list">list of words</param>
        /// <param name="count">how many words?</param>
        /// <param name="word">word to check</param>
        /// <returns>true if it does</returns>
        private bool contains(IEnumerable<String> list, int count, String word)
        {
            for (int ii = 0; ii < count && ii < list.Count(); ii++)
            {
                if (String.Compare(word, list.ElementAt(ii), true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Some modifier key was pressed. Handled it.
        /// </summary>
        private void KeyStateTracker_EvtKeyStateChanged()
        {
            try
            {
                // turn off select mode.  If select mode is on,
                // as the user moves the cursor, ACAT selects
                // text in the target window
                if (!KeyStateTracker.IsShiftOn())
                {
                    using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                    {
                        context.TextAgent().SetSelectMode(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        private void processSentencePredictionResponse(WordPredictionResponse response)
        {
            List<string> predictedSentenceList1 = new List<string>();

            var predictedSentenceList = response.Results;

            try
            {
                string type = string.Empty;
                try
                {
                    foreach (var element in predictedSentenceList)
                    {
                        if (element[0] == '&')
                            type = element;
                        switch (type)
                        {
                            case "&SENTENCES":
                                if (!element.Equals(type))
                                    predictedSentenceList1.Add(element);
                                predictedSentenceList = predictedSentenceList1;
                                break;

                            case "&SENTENCESLETTERS":
                                if (!element.Equals(type))
                                    predictedSentenceList1.Add(element);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            if (_sentenceListWidget != null)
            {
                int ii = 0;
                foreach (var sentence in predictedSentenceList)
                {
                    if (ii >= predictedSentenceList.Count() || ii >= SentencePredictionCountMax)
                    {
                        break;
                    }
                    if (_sentenceListWidget.Children.ElementAt(ii) is SentenceListItemWidget)
                    {
                        _sentenceListWidget.Children.ElementAt(ii).SetText(sentence);
                        _sentenceListWidget.Children.ElementAt(ii).Value = sentence;
                    }
                    ii++;
                }

                // if we had less than max sentences, clear out the remaining sentence list items
                _sentenceListWidget.ClearEntries(ii);
            }
        }

        private void processWordPredictionResponse(WordPredictionResponse response)
        {
            IEnumerable<string> predictedLettersList = new List<string>();
            List<string> predictedWordsList1 = new List<string>();
            List<string> predictedLettersList1 = new List<string>();

            var predictedWordList = response.Results;

            try
            {
                string type = string.Empty;
                try
                {
                    foreach (var element in predictedWordList)
                    {
                        if (element[0] == '&')
                        {
                            type = element;
                        }

                        switch (type)
                        {
                            case "&WORDS":
                                if (!element.Equals(type))
                                    predictedWordsList1.Add(element);
                                break;

                            case "&LETTERS":
                                if (!element.Equals(type))
                                    predictedLettersList1.Add(element);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
                predictedWordList = predictedWordsList1;
                predictedLettersList = predictedLettersList1;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
            Log.Debug("predictedWordList count: " + predictedWordList.Count());

            // check if the current word is a possessive word. If not, we need to create
            // a possessive version of the word and add it as the last word
            // in the predicton list.
            
            var possessiveWord = String.Empty;

            /*
            var wordAtCaret = response.Request.CurrentWord;  // check if it is the same

            var charAtCaret = '\0';

            using (var agentContext = Context.AppAgentMgr.ActiveContext())
            {
                agentContext.TextAgent().GetCharAtCaret(out charAtCaret);
                Log.Debug("charAtCaret: [" + charAtCaret + "]");
            }

            if (!String.IsNullOrEmpty(wordAtCaret) &&
                R.IsCurrentCultureEnglish() &&
                Context.AppAgentMgr.CurrentEditingMode == EditingMode.Edit &&
                !wordAtCaret.EndsWith("'s", StringComparison.InvariantCultureIgnoreCase) &&
                (charAtCaret == '\0' || charAtCaret == 0x0D || charAtCaret == 0x0A ||
                TextUtils.IsPunctuationOrWhiteSpace(charAtCaret) ||
                TextUtils.IsTerminatorOrWhiteSpace(charAtCaret)))
            {
                possessiveWord = wordAtCaret + "'s";
            }
            */
            int ii = 0;
            if (_wordListWidgetWidget != null)
            {
                foreach (var word in predictedWordList)
                {
                    if (ii >= predictedWordList.Count() || ii >= WordPredictionWordCountMax)
                    {
                        break;
                    }

                    Log.Debug("setting Word for " + ii + ":  " + word + " ");
                    if (_wordListWidgetWidget.Children.ElementAt(ii) is WordListItemWidget)
                    {
                        _wordListWidgetWidget.Children.ElementAt(ii).SetText(word);
                        _wordListWidgetWidget.Children.ElementAt(ii).Value = word;
                    }
                    ii++;
                }
                // if there is a possessive form of the word, add
                // it to the list.
                if (!String.IsNullOrEmpty(possessiveWord) &&
                    !contains(predictedWordList, ii, possessiveWord))
                {
                    int index;
                    if (ii >= WordPredictionWordCountMax)
                    {
                        index = WordPredictionWordCountMax - 1;
                    }
                    else
                    {
                        index = ii;
                        ii++;
                    }

                    if (_wordListWidgetWidget.Children.ElementAt(index) is WordListItemWidget)
                    {
                        _wordListWidgetWidget.Children.ElementAt(index).SetText(possessiveWord);
                        _wordListWidgetWidget.Children.ElementAt(index).Value = possessiveWord;
                    }
                }

                // if we had less than max words, clear out the remaining word list items
                _wordListWidgetWidget.ClearEntries(ii);
            }

            /*
            if (_currentWordWidget != null)
            {
                Log.Debug("Calling SetCurrentWord with : [" + wordAtCaret + "]");
                _currentWordWidget.SetCurrentWord(wordAtCaret);
            }
            */
            // Validation for the character predictions if there any value the proceed to add the element to the childs
            if (_letterListWidgetWidget != null)
            {
                ii = 0;
                foreach (var letter in predictedLettersList)
                {
                    if (ii >= predictedLettersList.Count() || ii >= LetterPredictionLetterCountMax)
                    {
                        break;
                    }
                    Log.Debug("setting letter for " + ii + ":  " + letter.Trim('\'') + " ");
                    if (_letterListWidgetWidget.Children.ElementAt(ii) is LetterListItemWidget)
                    {
                        string textValue = letter.Trim('\'', ' ');
                        if (textValue.Length > 0)
                        {
                            _letterListWidgetWidget.Children.ElementAt(ii).SetText(letter.Trim('\''));
                            _letterListWidgetWidget.Children.ElementAt(ii).Value = letter.Trim('\'');
                        }
                    }
                    ii++;
                }
                // if we had less than max words, clear out the remaining word list items
                _letterListWidgetWidget.ClearEntries(ii);
            }
        }

        /// <summary>
        /// Fetch new set of words into the prediction list box
        /// </summary>
        private void refreshWordPredictionsAndSetCurrentWord()
        {
            CoreGlobals.Stopwatch3.Reset();
            CoreGlobals.Stopwatch3.Start();

            tryRefreshWordPredictionsAndSetCurrentWord();

            CoreGlobals.Stopwatch3.Stop();

            Log.Debug("TimeElapsed for tryRefreshWordPredictionsAndSetCurrentWord: " + CoreGlobals.Stopwatch3.ElapsedMilliseconds);
        }

        /// <summary>
        /// Subscribes to all the events triggered by the
        /// widgets and the interpreter
        /// </summary>
        private void subscribeToEvents()
        {
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtWordPredictionAsyncResponse += ActiveWordPredictor_EvtWordPredictionAsyncResponse;
            Context.AppAgentMgr.EvtTextChanged += AppAgent_EvtTextChanged;
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged += ActiveWordPredictor_EvtModeChanged;
            KeyStateTracker.EvtKeyStateChanged += KeyStateTracker_EvtKeyStateChanged;
        }

        /// <summary>
        /// Refreshes the word prediction list and also updates the
        /// current word box with the word that is currently being typed.
        /// </summary>
        /// <returns>true onf success</returns>
        private bool tryRefreshWordPredictionsAndSetCurrentWord()
        {
            bool retVal = true;

            if (_wordListWidgetWidget == null && _letterListWidgetWidget == null && _sentenceListWidget == null)
            {
                return true;
            }

            try
            {
                String wordAtCaret;
                String nwords;
                var charAtCaret = '\0';

                using (var agentContext = Context.AppAgentMgr.ActiveContext())
                {
                    // we need the word at the cursor and also the previous n-words
                    // in the current sentence
                    int caretPos = agentContext.TextAgent().GetCaretPos();
                    Log.Debug("Perform WordPrediction at caretPos " + caretPos);
                    agentContext.TextAgent().GetPrefixAndWordAtCaret(out nwords, out wordAtCaret);
                    Log.Debug("wordAtCaret: [" + wordAtCaret + "]");

                    agentContext.TextAgent().GetCharAtCaret(out charAtCaret);
                    Log.Debug("charAtCaret: [" + charAtCaret + "]");
                }

                if (String.IsNullOrEmpty(nwords) && String.IsNullOrEmpty(wordAtCaret))
                {
                    if (_currentWordWidget != null)
                    {
                        _currentWordWidget.SetCurrentWord(String.Empty);
                    }

                    if (_wordListWidgetWidget != null)
                    {
                        _wordListWidgetWidget.ClearEntries();
                    }

                    nwords = " ";
                    wordAtCaret = String.Empty;
                }

                Log.Debug("wordatcaret length: " + wordAtCaret.Length);

                //Log.Debug("Text: [" + text + "], caretPos: " + caretPos.ToString());

                Log.Debug("Prefix: [" + nwords + "]");
                Log.Debug("CurrentWord: [" + wordAtCaret + "]");

                /*
                if (_wordListWidgetWidget != null)
                {
                    _wordListWidgetWidget.ClearEntriesWithEllipses();
                }
                */

                // do the word prediction
                if (wordAtCaret.Length > 1 && Char.IsPunctuation(wordAtCaret[0]))
                {
                    wordAtCaret = wordAtCaret.Substring(1);
                }

                if (_sentenceListWidget != null)
                {
                    bool ellipses = false;
                    if (String.IsNullOrEmpty(wordAtCaret.Trim()))
                    {
                        ellipses = !String.IsNullOrEmpty(nwords);
                    }

                    if (ellipses)
                    {
                        _sentenceListWidget.ClearEntriesWithEllipses();
                    }
                    else
                    {
                        _sentenceListWidget.ClearEntries();
                    }
                }

                WordPredictionRequest request;

                if (_predictionTypes.Contains(PredictionTypes.Words))
                {
                    request = new WordPredictionRequest(nwords + wordAtCaret, PredictionTypes.Words, Context.AppWordPredictionManager.ActiveWordPredictor.GetMode());
                    if (Context.AppWordPredictionManager.ActiveWordPredictor.SupportsPredictSync)
                    {
                        var response = Context.AppWordPredictionManager.ActiveWordPredictor.Predict(request);
                        processWordPredictionResponse(response);
                    }
                    else
                    {
                        Context.AppWordPredictionManager.ActiveWordPredictor.PredictAsync(request);
                    }
                }

                if (_predictionTypes.Contains(PredictionTypes.Sentences) && Common.AppPreferences.UseSentencePrediction)
                {
                    request = new WordPredictionRequest(nwords + wordAtCaret, PredictionTypes.Sentences, Context.AppWordPredictionManager.ActiveWordPredictor.GetMode());
                    if (Context.AppWordPredictionManager.ActiveWordPredictor.SupportsPredictSync)
                    {
                        var response = Context.AppWordPredictionManager.ActiveWordPredictor.Predict(request);
                        processSentencePredictionResponse(response);
                    }
                    else
                    {
                        Context.AppWordPredictionManager.ActiveWordPredictor.PredictAsync(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            Log.Debug("Returning");
            return retVal;
        }
    }
}