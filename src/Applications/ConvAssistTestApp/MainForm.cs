////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MainForm.cs
//
// The main form that displays the UI for the ConvAssistTestApp application.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.DialogSenseManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using ConvAssistTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Applications.ConvAssistTestApp
{
    public partial class MainForm : Form
    {
        private readonly PredictionTypes[] _predictionTypes;

        private readonly List<Control> crgResponseListButtons = new List<Control>();
        private readonly List<Control> keywordListButtons = new List<Control>();
        private readonly List<string> predictedKeywordList = new List<string>();
        private readonly List<string> predictedResponseList = new List<string>();
        private readonly List<Control> sentenceListButtons = new List<Control>();
        private readonly List<Control> wordListButtons = new List<Control>();
        
        private readonly Color buttonBackColor;
        private bool crgMode = false;
        private Color keywordButtonBackColor;
        private int numTurns = 4;
        private bool ttsEnabled;
        private IDialogSenseAgent _activeDialogSenseAgent;

        public MainForm()
        {
            InitializeComponent();

            initializeButtonLists();

            Shown += MainForm_Shown;

            _predictionTypes = new PredictionTypes[] { PredictionTypes.Words, PredictionTypes.Sentences };

            buttonBackColor = buttonLearn.BackColor;

            numericUpDownTurns.Value = numTurns;

            Load += MainForm_Load;

            FormClosing += MainForm_FormClosing;

            _activeDialogSenseAgent = Context.AppDialogSenseManager.ActiveDialogSenseAgent;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isASRConnected())
            {
                disconnectASR();
            }
        }

        private void ActiveWordPredictor_EvtModeChanged(WordPredictionModes newMode)
        {
            tryRefreshWordPredictionsAndSetCurrentWord();
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
            else if (response.Request.PredictionType == PredictionTypes.Keywords || response.Request.PredictionType == PredictionTypes.LnRResponses)
            {
                processCRGResponse(response);
            }
        }

        private void addNewKeyword(String keyword)
        {
            predictedKeywordList.Insert(0, keyword);

            updateKeywordButtons();

            keywordListButton_Click(keywordListButtons[0] as Button, new EventArgs());
        }

        private void autoCompleteWord(String wordSelected)
        {
            try
            {
                var caretPos = Windows.GetCaretPosition(textBoxInput);
                var text = Windows.GetText(textBoxInput);

                TextUtils.GetPrevWordOffsetAutoComplete(text, caretPos, out int offset, out int count);

                bool checkInsert = TextUtils.CheckInsertOrReplaceWord(text, caretPos, out int insertOrReplaceOffset, out string wordToReplace);

                int wordToReplaceLength = wordToReplace.Length;

                if (text.Length > 0 && caretPos <= text.Length - 1)
                {
                    var ch = text[caretPos];
                    if (ch != ' ')
                    {
                        wordSelected += " ";
                    }
                }
                else
                {
                    wordSelected += " ";
                }

                if (checkInsert)
                {
                    var textBeforeCaret = text.Substring(0, caretPos);
                    var textAfterCaret = text.Substring(caretPos);

                    Windows.SetText(textBoxInput, textBeforeCaret + wordSelected + textAfterCaret);
                    Windows.SetCaretPosition(textBoxInput, caretPos + wordSelected.Length);
                }
                else
                {
                    var textBeforeCaret = text.Substring(0, insertOrReplaceOffset);
                    var textAfterCaret = text.Substring(insertOrReplaceOffset + wordToReplaceLength);

                    Windows.SetText(textBoxInput, textBeforeCaret + wordSelected + textAfterCaret);
                    Windows.SetCaretPosition(textBoxInput, insertOrReplaceOffset + wordSelected.Length);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        private void buittonClearASR_Click(object sender, EventArgs e)
        {
            Windows.SetText(textBoxASR, String.Empty);
        }

        private void buttonAddNewKeyword_Click(object sender, EventArgs e)
        {
            var dialog = new NewKeywordDialog();
            var result = dialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var newKeyword = dialog.NewKeyword;

                if (!String.IsNullOrEmpty(newKeyword))
                {
                    newKeyword = newKeyword.Trim();

                    addNewKeyword(newKeyword);
                }
            }
        }

        private void buttonClearHistory_Click(object sender, EventArgs e)
        {
            clearHistory();
        }

        private void buttonLearn_Click(object sender, EventArgs e)
        {
            var text = Windows.GetText(textBoxInput).Trim();


            TextUtils.GetParagraphAtCaret(text, Windows.GetCaretPosition(textBoxInput), out string para);

            para = para.Trim();

            if (String.IsNullOrEmpty(para))
            {
                return;
            }

            switch (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode())
            {
                case WordPredictionModes.Sentence:
                    WordPredictionManager.Instance.ActiveWordPredictor.Learn(para, WordPredictorMessageTypes.LearnWords);
                    break;

                case WordPredictionModes.Shorthand:
                    WordPredictionManager.Instance.ActiveWordPredictor.Learn(para, WordPredictorMessageTypes.LearnShorthand);
                    break;

                case WordPredictionModes.CannedPhrases:
                    WordPredictionManager.Instance.ActiveWordPredictor.Learn(para, WordPredictorMessageTypes.LearnCanned);
                    break;
            }
            Log.Debug("Learn " + para);

            WordPredictionManager.Instance.ActiveWordPredictor.Learn(para, WordPredictorMessageTypes.LearnSentence);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            crgRequestKeywords();

            crgRequestResponses();
        }

        private void buttonShowHistory_Click(object sender, EventArgs e)
        {
            var dialog = _activeDialogSenseAgent.DialogTranscript.ToString();

            dialog = dialog.Replace("\n", "\r\n\r\n");

            var historyDialog = new ShowDialogHistoryForm
            {
                History = dialog
            };
            historyDialog.ShowDialog();
        }

        private void changeMode(WordPredictionModes mode)
        {
            Context.AppWordPredictionManager.ActiveWordPredictor.SetMode(mode);

            clearSentenceList();
            clearWordList();
            Windows.SetText(textBoxInput, string.Empty);

            textBoxInput_SelectionChanged(textBoxInput, EventArgs.Empty);

            if (mode != WordPredictionModes.Sentence)
            {
                checkBoxCRG.Checked = false;
                Windows.SetVisible(checkBoxCRG, false);
            }
            else
            {
                Windows.SetVisible(checkBoxCRG, true);
            }

        }

        private void checkBoxCRG_CheckedChanged(object sender, EventArgs e)
        {
            crgOn(checkBoxCRG.Checked);
        }

        private void clearHistory()
        {
            textBoxASR.Text = String.Empty;

            clearKeywordList();

            clearCRGResponseList();

            _activeDialogSenseAgent.DialogTranscript.Clear();

            predictedKeywordList.Clear();

            predictedResponseList.Clear();

            Windows.SetText(labelConvAssistRequest, String.Empty);

            Windows.SetText(textBoxInput, String.Empty);
            Windows.SetText(textBoxASR, String.Empty);
        }

        private void clearSentenceList(bool ellipses = false)
        {
            foreach (var button in sentenceListButtons)
            {
                Windows.SetText(button, (ellipses) ? ". . . " : String.Empty);
            }
        }

        private void clearWordList(bool ellipses = false)
        {
            foreach (var button in wordListButtons)
            {
                Windows.SetText(button, (ellipses) ? ". . ." : String.Empty);
            }
        }

        private void clearKeywordList(bool ellipses = false)
        {
            foreach (var button in keywordListButtons)
            {
                button.Tag = false;
                button.BackColor = keywordButtonBackColor;
                Windows.SetText(button, (ellipses) ? ". . . " : String.Empty);
            }
        }

        private void clearCRGResponseList(bool ellipses = false)
        {
            foreach (var button in crgResponseListButtons)
            {
                Windows.SetText(button, (ellipses) ? ". . . " : String.Empty);
            }
        }

        private void crgOn(bool on)
        {
            crgMode = on;

            foreach (var button in keywordListButtons)
            {
                Windows.SetVisible(button, on);
            }

            Windows.SetVisible(labelKeywords, on);

            foreach (var button in crgResponseListButtons)
            {
                Windows.SetVisible(button, on);
            }

            Windows.SetVisible(labelCRGResponses, on);

            Windows.SetVisible(labelASR, on);

            Windows.SetVisible(textBoxASR, on);

            Windows.SetVisible(buttonAddNewKeyword, on);

            Windows.SetVisible(buttonShowHistory, on);

            Windows.SetVisible(buttonClearHistory, on);

            Windows.SetVisible(numericUpDownTurns, on);

            Windows.SetVisible(labelTurns, on);

            Windows.SetVisible(buttonRefresh, on);

            Windows.SetVisible(checkBoxASR, on);

            Windows.SetVisible(comboBoxASRModel, on);

            Windows.SetVisible(labelASRResponse, on);
/*
            if (!on)
            {
                disconnectASR();
            }
*/
            clearHistory();
        }

        private void crgRequestKeywords()
        {
            var dialog = _activeDialogSenseAgent.DialogTranscript.ToString(numTurns);

            Windows.SetText(labelConvAssistRequest, dialog);
            var request = new WordPredictionRequest(dialog,
                                                        PredictionTypes.Keywords,
                                                        Context.AppWordPredictionManager.ActiveWordPredictor.GetMode(), true);
            clearKeywordList(true);

            if (Context.AppWordPredictionManager.ActiveWordPredictor.SupportsPredictSync)
            {
                var response = Context.AppWordPredictionManager.ActiveWordPredictor.Predict(request);
                processCRGResponse(response);
            }
            else
            {
                Context.AppWordPredictionManager.ActiveWordPredictor.PredictAsync(request);
            }
        }

        private void crgRequestResponses(String keyword = null)
        {
            var dialog = _activeDialogSenseAgent.DialogTranscript.ToString(numTurns);

            var request = new WordPredictionRequest(dialog,
                                                        PredictionTypes.LnRResponses,
                                                        Context.AppWordPredictionManager.ActiveWordPredictor.GetMode(), true, keyword);

            clearCRGResponseList(true);

            if (Context.AppWordPredictionManager.ActiveWordPredictor.SupportsPredictSync)
            {
                var response = Context.AppWordPredictionManager.ActiveWordPredictor.Predict(request);
                processCRGResponse(response);
            }
            else
            {
                Context.AppWordPredictionManager.ActiveWordPredictor.PredictAsync(request);
            }
        }

        private void crgResponseListButton_Click(object sender, EventArgs e)
        {
            var text = textBoxInput.Text;
            String textToAdd = String.Empty;

            var button = sender as Button;
            if (button != null)
            {
                textToAdd = button.Text;
            }

            //int index = TextUtils.GetStartIndexCurrOrPrevSentence(text, Windows.GetCaretPosition(textBoxInput));

            int index = TextUtils.GetSentenceAtCaret(text, Windows.GetCaretPosition(textBoxInput), out string sentence);
            if (index >= 0)
            {
                if (!String.IsNullOrEmpty(textToAdd))
                {
                    var caretPos = Windows.GetCaretPosition(textBoxInput);

                    if (text[index] == '\n')
                    {
                        index++;
                    }
                    var textBeforeIndex = Windows.GetText(textBoxInput).Substring(0, index);
                    var textAfterIndex = Windows.GetText(textBoxInput).Substring(caretPos);

                    textBoxInput.Text = textBeforeIndex + textToAdd + textAfterIndex;
                }
            }
            else
            {
                textBoxInput.Text = textToAdd;

            }
            Windows.SetCaretPosition(textBoxInput, textBoxInput.Text.Length);

            Windows.SetFocus(textBoxInput);
        }

        private void enableButtonLearn(bool enable)
        {
            Windows.SetEnabled(buttonLearn, enable);

            Windows.SetBackgroundColor(buttonLearn, enable ? buttonBackColor : Color.Gray);
        }

        private void initializeButtonLists()
        {
            wordListButtons.AddRange(new Control[]
                                    {   buttonWord1, buttonWord2, buttonWord3,
                                        buttonWord4, buttonWord5, buttonWord6,
                                        buttonWord7, buttonWord8, buttonWord9,
                                        buttonWord10
                                    });

            sentenceListButtons.AddRange(new Control[]
                                        {   buttonSentence1, buttonSentence2, buttonSentence3,
                                            buttonSentence4, buttonSentence5, buttonSentence6,
                                            buttonSentence7, buttonSentence8, buttonSentence9,
                                            buttonSentence10
                                        });

            keywordListButtons.AddRange(new Control[]
                                        {   buttonKeyword1, buttonKeyword2, buttonKeyword3,
                                            buttonKeyword4, buttonKeyword5, buttonKeyword6,
                                            buttonKeyword7, buttonKeyword8, buttonKeyword9
                                        });

            foreach (var button in keywordListButtons)
            {
                button.Tag = false;
            }

            keywordButtonBackColor = keywordListButtons[0].BackColor;

            crgResponseListButtons.AddRange(new Control[]
                                        {
                                            buttonCRGResponse1, buttonCRGResponse2, buttonCRGResponse3,
                                            buttonCRGResponse4, buttonCRGResponse5, buttonCRGResponse6,
                                            buttonCRGResponse7, buttonCRGResponse8, buttonCRGResponse9,
                                            buttonCRGResponse10
                                        });
        }

        private void keywordListButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (String.IsNullOrEmpty(button.Text))
            {
                return;
            }

            foreach (var b in keywordListButtons)
            {
                b.BackColor = keywordButtonBackColor;
            }

            toggleSelectKeywordButton(button);

            bool? selected = (bool?)button.Tag;

            if ((bool)selected)
            {
                crgRequestResponses(button.Text);
            }
            else
            {
                crgRequestResponses();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CenterToScreen();

            subscribeToEvents();

            numericUpDownTurns.Minimum = 1;
            numericUpDownTurns.Maximum = 20;
            numericUpDownTurns.Value = numTurns;

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //Windows.SetTopMost(this);
            Windows.SetFocus(this);
            Windows.Activate(this);

            checkBoxCRG.Checked = false;

            crgOn(checkBoxCRG.Checked);

            comboBoxASRModel.SelectedIndex = 0;

            radioButtonSentence.Checked = true;

            textBoxInput.Focus();

            textBoxInput.SelectionChanged += textBoxInput_SelectionChanged;
            textBoxASR.SelectionChanged += textBoxASR_SelectionChanged;
        }

        private void NumericUpDownTurns_ValueChanged(object sender, EventArgs e)
        {
            numTurns = (int)numericUpDownTurns.Value;
        }

        private void processCRGResponse(WordPredictionResponse response)
        {
            bool keywordsFound = false;
            bool responseFound = false;

            var results = response.Results;

            try
            {
                string type = string.Empty;
                try
                {
                    foreach (var element in results)
                    {
                        if (element[0] == '&')
                        {
                            type = element;
                        }

                        switch (type)
                        {
                            case "&CRGKEYWORDS":
                                keywordsFound = true;
                                if (!element.Equals(type))
                                    predictedKeywordList.Add(element);
                                else
                                    predictedKeywordList.Clear();
                                break;

                            case "&CRGRESPONSES":
                                responseFound = true;
                                if (!element.Equals(type))
                                    predictedResponseList.Add(element);
                                else
                                    predictedResponseList.Clear();
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
            Log.Debug("predictedWordList count: " + results.Count());

            Invoke(new MethodInvoker(delegate
            {
                if (keywordsFound)
                {
                    updateKeywordButtons();
                }

                if (responseFound)
                {
                    updateResponseButtons();
                }
            }));
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

            int ii = 0;

            Invoke(new MethodInvoker(delegate
            {
                foreach (var sentence in predictedSentenceList)
                {
                    sentenceListButtons[ii].Text = sentence;
                    ii++;
                }

                for (; ii < sentenceListButtons.Count; ii++)
                {
                    sentenceListButtons[ii].Text = String.Empty;
                }
            }));
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

            int ii = 0;

            Invoke(new MethodInvoker(delegate
            {
                for (; ii < predictedWordList.Count(); ii++)
                {
                    wordListButtons[ii].Text = predictedWordList.ElementAt(ii);
                }

                for (; ii < wordListButtons.Count; ii++)
                {
                    wordListButtons[ii].Text = String.Empty;
                }
            }));
        }

        private void RadioButtonCannedPhrase_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCannedPhrase.Checked)
            {
                changeMode(WordPredictionModes.CannedPhrases);
            }
        }

        private void RadioButtonSentence_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSentence.Checked)
            {
                changeMode(WordPredictionModes.Sentence);
            }
        }

        private void RadioButtonShorthand_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonShorthand.Checked)
            {
                changeMode(WordPredictionModes.Shorthand);
            }
        }

        private string removeApostrophes(string inputStr)
        {
            string outputStr;
            try
            {
                outputStr = inputStr.Trim(new char[] { (char)39 });
            }
            catch (Exception)
            {
                outputStr = inputStr;
            }
            return outputStr;
        }

        private string removeSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    c == '.' ||
                    c == '_' ||
                    c == ' ' ||
                    c == '\'')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void sentenceListButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var text = button.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    text += " ";
                    var caretPos = Windows.GetCaretPosition(textBoxInput);

                    var textBeforeCaret = Windows.GetText(textBoxInput).Substring(0, caretPos);
                    var textAfterCaret = Windows.GetText(textBoxInput).Substring(caretPos);

                    textBoxInput.Text = textBeforeCaret + text + textAfterCaret;

                    Windows.SetCaretPosition(textBoxInput, caretPos + text.Length);

                    Windows.SetFocus(textBoxInput);
                }
            }

            textBoxInput.Focus();
        }

        private void subscribeToEvents()
        {
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtWordPredictionAsyncResponse += ActiveWordPredictor_EvtWordPredictionAsyncResponse;

            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged += ActiveWordPredictor_EvtModeChanged;

            radioButtonSentence.CheckedChanged += RadioButtonSentence_CheckedChanged;
            radioButtonShorthand.CheckedChanged += RadioButtonShorthand_CheckedChanged;
            radioButtonCannedPhrase.CheckedChanged += RadioButtonCannedPhrase_CheckedChanged;

            checkBoxASR.CheckedChanged += new System.EventHandler(this.checkBoxASR_CheckedChanged);

            foreach (var button in wordListButtons)
            {
                button.Click += wordListButton_Click;
            }

            foreach (var button in sentenceListButtons)
            {
                button.Click += sentenceListButton_Click;
            }

            foreach (var button in keywordListButtons)
            {
                button.Click += keywordListButton_Click;
            }

            foreach (var button in crgResponseListButtons)
            {
                button.Click += crgResponseListButton_Click;
            }

            textBoxASR.KeyPress += TextBoxASR_KeyPress;

            textBoxInput.KeyPress += TextBoxInput_KeyPress;

            numericUpDownTurns.ValueChanged += NumericUpDownTurns_ValueChanged;
        }
        private void TextBoxASR_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                requestCRG();
            }
        }

        private void TextBoxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TextUtils.GetParagraphAtCaret(Windows.GetText(textBoxInput), Windows.GetCaretPosition(textBoxInput) - 2, out String paragraph);

                _activeDialogSenseAgent.DialogTranscript.AddTurn(new DialogTurn(DialogTurnType.User, paragraph.Trim()));

                if (ttsEnabled)
                {
                    TTSManager.Instance.ActiveEngine.Speak(paragraph);
                }

                try
                {
                    tryRefreshWordPredictionsAndSetCurrentWord();
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
            }
        }
        private void textBoxInput_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                tryRefreshWordPredictionsAndSetCurrentWord();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        private void textBoxASR_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void toggleSelectKeywordButton(Button button)
        {
            bool? selected = !(bool?)button.Tag;

            button.Tag = selected;

            if ((bool)selected)
            {
                button.BackColor = Color.Green;
            }
            else
            {
                button.BackColor = keywordButtonBackColor;
            }
        }

        private bool tryRefreshWordPredictionsAndSetCurrentWord()
        {
            bool retVal = true;

            try
            {

                var text = Windows.GetText(textBoxInput);

                enableButtonLearn(!String.IsNullOrEmpty(text.Trim()));

                int caretPos = Windows.GetCaretPosition(textBoxInput);
                Log.Debug("Perform WordPrediction at caretPos " + caretPos);
                TextUtils.GetPrefixAndWordAtCaret(text, caretPos, out string nwords, out string wordAtCaret);
                Log.Debug("nwords: [" + nwords + "]");
                Log.Debug("wordAtCaret: [" + wordAtCaret + "]");

                if (String.IsNullOrEmpty(nwords) && String.IsNullOrEmpty(wordAtCaret))
                {
                    clearWordList();

                    nwords = " ";
                    wordAtCaret = String.Empty;
                }

                Log.Debug("wordatcaret length: " + wordAtCaret.Length);

                //Log.Debug("Text: [" + text + "], caretPos: " + caretPos.ToString());

                Log.Debug("Prefix: [" + nwords + "]");
                Log.Debug("CurrentWord: [" + wordAtCaret + "]");

                if (wordAtCaret.Length > 1 && Char.IsPunctuation(wordAtCaret[0]))
                {
                    wordAtCaret = wordAtCaret.Substring(1);
                }

                bool ellipses = false;
                if (String.IsNullOrEmpty(wordAtCaret.Trim()))
                {
                    ellipses = !String.IsNullOrEmpty(nwords);
                }

                clearSentenceList(ellipses);

                WordPredictionRequest request;

                String predictionContext;
                String prevWords = nwords + wordAtCaret;

                if (crgMode)
                {
                    predictionContext = _activeDialogSenseAgent.DialogTranscript.ToString(numTurns);
                    if (!String.IsNullOrEmpty(prevWords))
                    {
                        predictionContext += "User:" + prevWords + "\n";
                    }
                }
                else
                {
                    predictionContext = prevWords;
                }

                Windows.SetText(labelConvAssistRequest, predictionContext);

                //String predictionContext = nwords + wordAtCaret;
                if (_predictionTypes.Contains(PredictionTypes.Words))
                {
                    request = new WordPredictionRequest(predictionContext,
                                                        PredictionTypes.Words,
                                                        Context.AppWordPredictionManager.ActiveWordPredictor.GetMode(), crgMode);
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

                if (String.IsNullOrEmpty(wordAtCaret.Trim()) &&
                    _predictionTypes.Contains(PredictionTypes.Sentences) &&
                    Common.AppPreferences.UseSentencePrediction)
                {
                    request = new WordPredictionRequest(predictionContext,
                                                        PredictionTypes.Sentences,
                                                        Context.AppWordPredictionManager.ActiveWordPredictor.GetMode(), crgMode);
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
            return retVal;
        }

        private void updateKeywordButtons()
        {
            int ii = 0;

            for (; ii < predictedKeywordList.Count() && ii < keywordListButtons.Count; ii++)
            {
                keywordListButtons[ii].Text = predictedKeywordList.ElementAt(ii);
            }

            for (; ii < keywordListButtons.Count; ii++)
            {
                keywordListButtons[ii].Text = String.Empty;
            }

            foreach (var b in keywordListButtons)
            {
                b.BackColor = keywordButtonBackColor;
                b.Tag = false;
            }
        }

        private void updateResponseButtons()
        {
            int ii = 0;

            for (; ii < predictedResponseList.Count() && ii < crgResponseListButtons.Count; ii++)
            {
                crgResponseListButtons[ii].Text = predictedResponseList[ii];
            }

            for (; ii < crgResponseListButtons.Count; ii++)
            {
                crgResponseListButtons[ii].Text = String.Empty;
            }
        }

        private void wordListButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null && !String.IsNullOrEmpty(button.Text))
            {
                autoCompleteWord(button.Text);
            }

            textBoxInput.Focus();
        }

        private void checkBoxASR_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxASR.Checked)
            {
                comboBoxASRModel.Enabled = false;
                //Task.Run(() => connectASR());
            }
            else
            {
                comboBoxASRModel.Enabled = true;
                Task.Run(() => disconnectASR());
            }
        }

        private void setASRResponseText(String text)
        {
            clearASResponseText();
            Windows.SetText(labelASRResponse, text);
        }

        private void clearASResponseText()
        {
            Windows.SetText(labelASRResponse, String.Empty);
        }

        private bool isASRConnected()
        {
            return _activeDialogSenseAgent!= null && _activeDialogSenseAgent.IsConnected();
        }

        private bool connectASR()
        {
            if (isASRConnected())
            {
                return true;
            }

            try
            {
                setASRResponseText("Connecting to ASR...");
                _activeDialogSenseAgent.ConnectAsync().GetAwaiter().GetResult();

                _activeDialogSenseAgent.EvtJsonMessageReceived += DialogSenseAgent_EvtJsonMessageReceived;
                    
                String model = String.Empty;

                Invoke(new MethodInvoker(delegate
                {
                    model = comboBoxASRModel.SelectedItem.ToString();
                }));

                MessageBox.Show(model);

                    var msg = new StartMessage(true, 0, model, false);

                var json = JsonConvert.SerializeObject(msg);
                _activeDialogSenseAgent.SendAsync(json).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var message = "Error connnecting to ASR. " + ex.Message;
                MessageBox.Show(message);
                setASRResponseText(message);

                Invoke(new MethodInvoker(delegate
                {
                    checkBoxASR.Checked = false;
                }));

                return false;
            }

            return true;
        }

        private void DialogSenseAgent_EvtJsonMessageReceived(object sender, string jsonMessage)
        {
            setASRResponseText(jsonMessage);

            if (jsonMessage.Contains("ACAT VAD ASR"))
            {
                return;
            }
            
            var receivedMessage = JsonConvert.DeserializeObject<ASRResponse>(jsonMessage);

            var text = Windows.GetText(textBoxASR);

            text = text.TrimEnd();

            text += "\n" + receivedMessage.text + "\n";

            Windows.SetText(textBoxASR, text);

            Windows.SetCaretPosition(textBoxASR, text.Length);

            Invoke(new MethodInvoker(delegate
            {
                textBoxASR.ScrollToCaret();
            }));

            requestCRG();
        }

        private bool disconnectASR()
        {
            if (!isASRConnected())
            {
                return true;
            }

            try
            {
                _activeDialogSenseAgent.EvtMessageReceived -= DialogSenseAgent_EvtJsonMessageReceived;
                _activeDialogSenseAgent.Disconnect();

                setASRResponseText("Disconnected from ASR.");
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error disconnnecting from ASR. " + ex.ToString());
                return false;
            }

            Invoke(new MethodInvoker(delegate
            {
                checkBoxASR.Checked = false;
            }));

            return true;
        }

        private void requestCRG()
        {
            TextUtils.GetParagraphAtCaret(Windows.GetText(textBoxASR), Windows.GetCaretPosition(textBoxASR) - 2, out String paragraph);

            _activeDialogSenseAgent.DialogTranscript.AddTurn(new DialogTurn(DialogTurnType.Other, paragraph.Trim()));

            crgRequestKeywords();

            crgRequestResponses();
        }

        private void checkBoxTTS_CheckedChanged(object sender, EventArgs e)
        {
            ttsEnabled = checkBoxTTS.Checked;
        }

        
    }
}