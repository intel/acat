using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using Microsoft.Office.Interop.Word;
using System.Text;
using Aster.TTSManagement;
using Aster.ScreenManagement;
using Aster.ExtensionHelper;
using Aster.Utility;
using Aster.AgentManagement;
using Aster.TalkWindowManagement;

// TODO not currently sending ImmediateStop command to interrupt current speech
// we can send that if current speech should be interrupted

namespace Aster.Extensions.Hawking.AppAgents.LectureManager
{
    public partial class LectureManagerMainForm : System.Windows.Forms.Form, IPanel
	{
        public bool FileLoaded { get; set; }

        enum speakingStates { silent, speakingSentence, speakingParagraph, speakingAll };

        WindowActiveWatchdog _windowActiveWatchdog;
        private const String _speakingStatus = "Speaking...";
        private const String _silentStatus = "";

	    public LectureManagerMainForm()
		{
			InitializeComponent();
            FileLoaded = false;
            this.ShowInTaskbar = false;
		}

#region CONSTANTS

		private const int BAUDRATE = 9600;
		private const System.IO.Ports.Parity PARITY = System.IO.Ports.Parity.None;
		private const System.IO.Ports.StopBits STOPBITS = System.IO.Ports.StopBits.One;
		private const int DATABITS = 8;
		private const System.IO.Ports.Handshake HANDSHAKE = System.IO.Ports.Handshake.None;

		private const int DEFAULT_START_POS = 1;
		private const int DEFAULT_END_POS = 1;

		private const int TIMER_INTERVAL = 1000;
		//Const DEFAULT_SPEECH_RATE As Byte = 120
		private const byte DEFAULT_SPEECH_RATE = 150;
		private const int MILLISECONDS_PER_MINUTE = 60000;

		private const int ASCII_CR = 13;
		private const int ASCII_SPACE = 32;
		private const int ASCII_PERIOD = 46;

		private const int TB_POS_ZERO = 0;
		private const int TB_SELECTION_LEN_ZERO = 0;

		private const int MAX_INDEX = 255;
		private const int MIN_INDEX = 1;

        private LectureManagerAgent _lectureManagerAgent;

#endregion

#region GLOBAL VARIABLES

		private string strSpeechBuffer = String.Empty;
		private char[] SENTENCE_TERMINATORS = new char[]{'.', '!', '?'};
		private char[] PARAGRAPH_TERMINATORS = new char[]{'\r'};
        private char[] PARAGRAPH_TERMINATORS1 = new char[] { '\r', '\n' };
		private char[] WORD_TERMINATORS = new char[]{' '};

		//private string ttsVersion;
		private speakingStates currentState = speakingStates.silent;

#endregion

		private void frmMain_Load(object sender, System.EventArgs e)
		{
            LectureManagerMainFormContextMenu.SetLectureManagerForm(this);
            this.MaximizeBox = false;
            Utility.Windows.SetWindowSizePercent(Context.AppWindowPosition, this.Handle, 75);

            Log.Debug("adding adhoc agent");
            _lectureManagerAgent = new LectureManagerAgent(this);

            AgentManager.Instance.AddAgent(this.Handle, _lectureManagerAgent);

            TalkWindowManager.Instance.EvtTalkWindowPreVisibilityChanged += new TalkWindowManager.TalkWindowVisibilityChanged(Instance_EvtTalkWindowPreVisibilityChanged);

            enableWatchdogs();

            TTSManager.Instance.ActiveEngine.GetInvoker().EvtExtensionEvent += new OnExtensionEvent(TTSEngine_Event);

            //tts.EvtIndexReached += new TTSSerial.IndexReached(OnIndexReached);
		}

        void Instance_EvtTalkWindowPreVisibilityChanged(object sender, TalkWindowVisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                removeWatchdogs();
            }
            else
            {
                enableWatchdogs();
            }
        }

#region SERIAL PORT

        private void StopSpeaking()
		{            
            // TODO: speaking state should be handled in the speech api but it's
            // not working so well so just handle it here for now
            // only send stop speech if it is not already silent.  for some reason
            // stop speech requests get piled up and prevent further speech from
            // commencing if the hardware isn't already speaking


            if (currentState != speakingStates.silent)
            {
                TTSManager.Instance.ActiveEngine.Stop();
                currentState = speakingStates.silent;
                clearStatusField();
            }
        }

		private void SendTextImmediate(string speechText)
		{
            Log.Debug("Entering...speechText>>>" + speechText + "<<<");
            if (!string.IsNullOrEmpty(speechText) && !(speechText.Equals("\r\n")))
            {
                Debug.WriteLine("SendTextImmediate() - Text to speak=" + speechText);
                //TTSManager.Instance.ActiveEngine.Speak(speechText + TextToSpeech.CMD_PREFIX + TextToSpeech.TSC_COMMENCE_OUTPUT, SpeakFlags.Default);
                TTSManager.Instance.ActiveEngine.Speak(speechText, SpeakFlags.Default);
            }
            else
            {
                Log.Debug("no text to speak!");
            }

            //Debug.WriteLine("SendTextImmediate() - Text to speak=" + speechText);
            //TTSManager.Instance.ActiveEngine.Speak(speechText, SpeakFlags.Default);

		}

#endregion

#region EVENT HANDLERS

        void TTSEngine_Event(object sender, ExtensionEventArgs args)
        {
            switch (args.EventType)
            {
                case "IndexReached":                    

                    int? index = args.GetInvoker().GetPropertyInt("Index");
                    if (index.HasValue)
                    {
                        OnIndexReached(index.Value);
                    }
                    break;

                default:
                    Log.Debug("TTSEngine raised unhandled event " + args.EventType);
                    break;
            }
        }

		private void OnIndexReached(int index)
		{
			Log.Debug("OnIndexReached() - index=" + index.ToString());

			if (currentState == speakingStates.speakingAll)
			{
                Log.Debug("OnIndexReached() - currentState=" + currentState.ToString() + " now continuing with more speech...");
				ProcessNextSpeakAllSentence();
			}
            else if (currentState == speakingStates.speakingParagraph)
            {
                Log.Debug("OnIndexReached() - currentState=" + currentState.ToString() + " now continuing with next sentence...");
                ProcessNextSpeakParagraphSentence();
            }
		}
#endregion

		private void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{

            TalkWindowManager.Instance.EvtTalkWindowPreVisibilityChanged -= new TalkWindowManager.TalkWindowVisibilityChanged(Instance_EvtTalkWindowPreVisibilityChanged);
		}

		private void MoveCursorToBeginning(ref TextBox thisTextbox)
		{
			thisTextbox.Focus();
			thisTextbox.Select(TB_POS_ZERO, TB_SELECTION_LEN_ZERO);
            thisTextbox.ScrollToCaret();
		}

		private void MoveCursorToEnd(ref TextBox thisTextbox)
		{
			thisTextbox.Focus();
			thisTextbox.Select(thisTextbox.Text.Length, TB_SELECTION_LEN_ZERO);
            thisTextbox.ScrollToCaret();
		}
		private void AddNextSentenceToSelection(ref TextBox thisTextbox, string sentenceTerminator)
		{
			string strLectureText = thisTextbox.Text;
			int currentStartPos = thisTextbox.SelectionStart;
			int currentEndPos = thisTextbox.SelectionStart + thisTextbox.SelectionLength;

			// search the rest of the buffer after the currently-selected string to find the next period
			int skipLastChar = 0;

			// don't include last selected character in search
			if ((thisTextbox.SelectionLength > 0) & (strLectureText[currentStartPos] != '.'))
			{
				skipLastChar = 1;
			}

			int indexStart = currentEndPos + 1;
			int indexCount = thisTextbox.Text.Length - (currentEndPos + 1);
			int indexPeriod = strLectureText.IndexOfAny(SENTENCE_TERMINATORS, indexStart, indexCount);

			thisTextbox.Focus();

			if (indexPeriod > -1)
			{
				thisTextbox.Select(currentStartPos, indexPeriod);
			}
			else
			{
				thisTextbox.Select(currentStartPos, thisTextbox.Text.Length);
			}
		}

        delegate void delegateMoveToNextTextFragment(ref TextBox thisTextbox, char[] terminators);
		private void MoveToNextTextFragment(ref TextBox thisTextbox, char[] terminators)
		{
            Log.Debug("Entering...");
            string strLectureText = String.Empty;

            strLectureText = thisTextbox.Text;
			int currentStartPos = thisTextbox.SelectionStart + thisTextbox.SelectionLength - 1;;

			// search the rest of the buffer after the currently-selected string to find the next period
			int skipLastChar = 0;

			// don't include last selected character in search
			try
			{
				if ((thisTextbox.SelectionLength > 0) & (strLectureText[currentStartPos] != '.'))
				{
					skipLastChar = 1;
				}
			}
			catch (Exception ex)
			{
                Debug.WriteLine("exception caught! ex=" + ex.Message);
				skipLastChar = 0;
			}

			int indexStart = 0;

			if (currentStartPos == 0)
			{
				indexStart = 0;
			}
			else
			{
				indexStart = currentStartPos + 1;
			}

            int indexPeriod = strLectureText.IndexOfAny(terminators, indexStart);

			if (indexPeriod > -1)
			{
                int indexEnd = (indexPeriod - indexStart) + 1;
                thisTextbox.Select(indexStart, indexEnd);
			}
			else
			{
                int indexEnd = thisTextbox.Text.Length;
                thisTextbox.Select(indexStart, indexEnd);
			}

            thisTextbox.Focus();
            //Log.Debug("jtext len=" + thisTextbox.SelectionLength);
            //Log.Debug("txt=" + thisTextbox.SelectedText);
			thisTextbox.ScrollToCaret();

            //if ((thisTextbox.SelectedText == "\r") ||
            //    (thisTextbox.SelectedText == "\n") ||
            //     (thisTextbox.SelectedText == "\r\n") ||
            //     (thisTextbox.SelectedText == "\n\r"))
            //{
            //    // TODO do this without recursion
            //    MoveToNextTextFragment(ref thisTextbox, terminators);
            //}

            Log.Debug("selected text=" + thisTextbox.SelectedText);
		}


        delegate void delegateNavigateToNextTextFragment(ref TextBox thisTextbox, char[] terminators);
        private void NavigateToNextTextFragment(ref TextBox thisTextbox, char[] terminators)
        {
            Log.Debug("Entering...");
            string strLectureText = String.Empty;

            strLectureText = thisTextbox.Text;
            int currentStartPos;
            currentStartPos = thisTextbox.SelectionStart;

            thisTextbox.Select(thisTextbox.SelectionStart, 0);

            int indexStart = 0;
            if (currentStartPos != 0)
            {
                indexStart = currentStartPos + 1;
            }

            if (indexStart < thisTextbox.Text.Length)
            {
                int indexPeriod = strLectureText.IndexOfAny(terminators, indexStart);

                if (indexPeriod > -1)
                {
                    indexPeriod++;
                    while (indexPeriod < thisTextbox.Text.Length)
                    {
                        char ch = thisTextbox.Text[indexPeriod];
                        if (terminators.Contains(ch))
                            indexPeriod++;
                        else
                            break;
                    }

                    thisTextbox.SelectionStart = indexPeriod;
                }
                else
                {
                    thisTextbox.SelectionStart = thisTextbox.Text.Length;
                }
            }

            thisTextbox.Focus();

            thisTextbox.ScrollToCaret();
        }

        private void NavigateToPreviousTextFragment(ref TextBox thisTextbox, char[] terminators)
        {
            string strLectureText = thisTextbox.Text;
            int pos = thisTextbox.SelectionStart;

            thisTextbox.Select(pos, 0);

            if (pos == 0)
            {
                return;
            }
            else
            {
                pos--;
            }

            while(pos > 0 && terminators.Contains(strLectureText[pos]))
            {
                pos--;
            }

            while (pos > 0)
            {
                if (terminators.Contains(strLectureText[pos]))
                {
                    pos++;
                    break;
                }
                pos = pos - 1;
            }

            thisTextbox.Focus();
            thisTextbox.SelectionStart = pos;
            thisTextbox.ScrollToCaret();
        }

		private void MoveToPreviousTextFragment(ref TextBox thisTextbox, char[] terminators)
		{
			string strLectureText = thisTextbox.Text;
			int currentStartPos = thisTextbox.SelectionStart;
			int currentEndPos = thisTextbox.SelectionStart;
			int pos = currentStartPos;
			bool foundChar = false;

            if (pos == 0)
            {
                return;
            }
            else
            {
                pos = pos - 1;
            }

			while (pos > 0)
			{
				char thisChar = '\0';

				thisChar = strLectureText.Substring(pos - 1, 1)[0];

                if (terminators.Contains(thisChar))
				{
					currentStartPos = pos;
					foundChar = true;
					pos = 0; // exit loop
				}

				pos = pos - 1;
			}

			if (foundChar == false)
			{
				currentStartPos = 0;
			}

			thisTextbox.Focus();
			thisTextbox.Select(currentStartPos, (currentEndPos - currentStartPos));
			thisTextbox.ScrollToCaret();
		}

		private void AddPreviousSentenceToSelection(ref TextBox thisTextbox, string sentenceTerminator)
		{
			int currentStartPos = thisTextbox.SelectionStart;

			if (currentStartPos == TB_POS_ZERO)
			{
				// no previous sentence to search!
				return;
			}

			string strLectureText = thisTextbox.Text;
			int currentEndPos = thisTextbox.SelectionStart + thisTextbox.SelectionLength;

			int indexStart = currentEndPos + 1;
			int indexPeriod = strLectureText.LastIndexOfAny(SENTENCE_TERMINATORS, currentStartPos);
			indexPeriod = strLectureText.LastIndexOfAny(SENTENCE_TERMINATORS, indexPeriod - 1);

			if (indexPeriod > -1)
			{
				thisTextbox.Select(indexPeriod + 1, currentEndPos);
			}
			else
			{
				thisTextbox.Select(TB_POS_ZERO, currentEndPos);
			}
		}

		private string GetNextSentence(ref string strBuf)
		{
			string strResult = String.Empty;
			int indexPeriod = 0;

			indexPeriod = strBuf.IndexOfAny(SENTENCE_TERMINATORS);

			if (indexPeriod > -1)
			{
				strResult = strBuf.Substring(0, indexPeriod + 1);
				strBuf = strBuf.Substring(indexPeriod + 2 - 1);
			}
			else
			{
				//timerSpeech.Stop()
				strResult = strBuf;
				strBuf = String.Empty;
			}

			Debug.WriteLine("returning this string>>>" + strResult + "<<<");

			return strResult;
		}

#region BUTTON HANDLERS

		private void btnChooseFile_Click(object sender, System.EventArgs e)
		{
			//StopSpeaking();
			ProcessOpenFile();
		}

		private void btnSpeakSentence_Click(object sender, System.EventArgs e)
		{
			ProcessSpeakSentence();
		}

		private void btnSpeakParagraph_Click(object sender, System.EventArgs e)
		{
			ProcessSpeakParagraph();
		}

		private void btnNavTop_Click(object sender, System.EventArgs e)
		{
			ProcessGotoTop();
		}

		private void btnNavBottom_Click(object sender, System.EventArgs e)
		{
			txtLectureBox.Focus();
			MoveCursorToEnd(ref txtLectureBox);
			txtLectureBox.ScrollToCaret();
		}

		private void btnSelectAll_Click(object sender, System.EventArgs e)
		{
			txtLectureBox.Focus();
			txtLectureBox.Select(TB_POS_ZERO, txtLectureBox.Text.Length);
		}

		private void btnSentenceUp_Click(object sender, System.EventArgs e)
		{
			ProcessSentenceUp();
		}

		private void btnSentenceDown_Click(object sender, System.EventArgs e)
		{
			ProcessSentenceDown();
		}

		private void btnClearStatusBox_Click(object sender, System.EventArgs e)
		{
			txtStatusBox.Text = String.Empty;
		}

		private void btnReadAll_Click(object sender, System.EventArgs e)
		{
			ProcessReadAllSpeech();
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			ProcessExit();
		}

		private void btnParagraphUp_Click(object sender, System.EventArgs e)
		{
			ProcessParagraphUp();
		}

		private void btnParagraphDown_Click(object sender, System.EventArgs e)
		{
			ProcessParagraphDown();
		}

		private void btnSelectWordUp_Click(object sender, System.EventArgs e)
		{
			txtLectureBox.Focus();
			MoveToNextTextFragment(ref txtLectureBox, WORD_TERMINATORS);
		}

		private void btnSelectWordDown_Click(object sender, System.EventArgs e)
		{
			txtLectureBox.Focus();
			MoveToNextTextFragment(ref txtLectureBox, WORD_TERMINATORS);
		}

#endregion

#region KEYPRESS HANDLING

		private void frmMain_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			ProcessKeyPress(e.KeyChar);
			e.Handled = true;
		}

		private void txtLectureBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

			ProcessKeyPress(e.KeyChar);
			e.Handled = true;

		}

		private void ProcessKeyPress(char ch)
		{
            Log.Debug(ch.ToString());
			if (currentState == speakingStates.speakingAll)
			{
				Debug.WriteLine("ProcessKeyPress() - stopping \"read all\" command!");
				StopSpeaking();
                currentState = speakingStates.silent;
                clearStatusField();
				return;
			}

			if (true == (((ch == 'u') | (ch == 'U'))))
			{
				// start reading all
				ProcessReadAllSpeech();
			}
			else if (true == (((ch == 'r') | (ch == 'R'))))
			{
				// read sentence
				ProcessSpeakSentence();
			}
			else if (true == ((ch == ' ')))
			{
				// read paragraph
			    //ProcessSpeakSentence();
                ProcessSpeakParagraph();
			}
			else if (true == (((ch == 'm') | (ch == 'M'))))
			{
				// move up a paragraph
				ProcessParagraphUp();
			}
			else if (true == (((ch == 'x') | (ch == 'X'))))
			{
				// move up a sentence
				ProcessSentenceUp();
			}
			else if (true == (((ch == 'k') | (ch == 'K'))))
			{
				// move down a paragraph
				ProcessParagraphDown();
			}
			else if (true == (((ch == 'w') | (ch == 'W'))))
			{
				// move down a sentence
				ProcessSentenceDown();
			}
			else if (true == (((ch == 'z') | (ch == 'Z'))))
			{
                // open file
                this.Invoke(new MethodInvoker(delegate()
                {
                    ProcessOpenFile();
                }));
			}
			else if (true == (((ch == 'i') | (ch == 'I'))))
			{
				// go to top
				MoveCursorToBeginning(ref txtLectureBox);
			}
			else if (true == (((ch == 'q') | (ch == 'Q'))))
			{
			    // quit app
                Log.Debug("Quit");
                ProcessExit();
			}
			else
			{
				// TODO
			}
		}

#endregion

#region PROCESSORS

		// these processors were created to centralize functionality that is invoked from both a button
		// and a keypress

		// OPEN FILE PROCESSOR

		private void ProcessOpenFile()
		{
            
            //string initialDir = @"C:\Program Files\Words+, Inc\EZ KeysXP\Eq";
            string initialDir = Common.AppPreferences.LectureManagerHomeDirectory;
            
            removeWatchdogs();

            AgentManager.Instance.RemoveAgent(this.Handle);

            this.Hide();

            LectureManagerOpenFileForm dlgOpenFile = new LectureManagerOpenFileForm(initialDir);

            DialogResult result = dlgOpenFile.ShowDialog();

            AgentManager.Instance.AddAgent(this.Handle, new LectureManagerAgent(this));

            this.Show();

            enableWatchdogs();

            if (result == System.Windows.Forms.DialogResult.OK)
			{
                lblChosenFile.Text = dlgOpenFile.FileName;

                string fileExtension = Path.GetExtension(lblChosenFile.Text).ToLower();

                try
                {
                    if (fileExtension.Equals(".txt"))
                    {
                        strSpeechBuffer = File.ReadAllText(dlgOpenFile.FileName);
                    }
                    else if ((fileExtension.Equals(".doc")) || (fileExtension.Equals(".docx")))
                    {
                        strSpeechBuffer = GetTextFromWordFile(dlgOpenFile.FileName);
                    }
                    FileLoaded = true;
                }
                catch (Exception ex)
                {
                    Log.Debug("Error reading from file " + dlgOpenFile.FileName + ", ex: " + ex.ToString());
                }

				txtLectureBox.Text = strSpeechBuffer;
				btnNavTop.PerformClick();
			}
		}

        private string GetTextFromWordFile(string wordFileName)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object isMissing = System.Reflection.Missing.Value;
            object path = wordFileName;
            object readOnly = true;

            Microsoft.Office.Interop.Word.Document doc = word.Documents.Open(ref path, ref isMissing, 
                                                                                ref readOnly, ref isMissing, 
                                                                                ref isMissing, ref isMissing, 
                                                                                ref isMissing, ref isMissing, 
                                                                                ref isMissing, ref isMissing, 
                                                                                ref isMissing, ref isMissing, 
                                                                                ref isMissing, ref isMissing, 
                                                                                ref isMissing, ref isMissing);
            StringBuilder returnText = new StringBuilder();

            for (int i = 0; i < doc.Paragraphs.Count; i++)
            {
                returnText.Append(" \r\n " + doc.Paragraphs[i + 1].Range.Text.ToString());
            }

            //Console.WriteLine(returnText);
            
            doc.Close();
            word.Quit();

            return returnText.ToString();
        }

		// READ MENU PROCESSORS

		private void ProcessReadAllSpeech()
		{
            currentState = speakingStates.speakingAll;
            updateStatusField(_speakingStatus);
			strSpeechBuffer = txtLectureBox.Text;
			ProcessNextSpeakAllSentence();
		}

		private void ProcessSpeakParagraph()
		{
            Log.Debug("Entering...");

            currentState = speakingStates.speakingParagraph;
            updateStatusField(_speakingStatus);
			//txtLectureBox.Invoke(new delegateMoveToNextTextFragment(MoveToNextTextFragment),
            //                        txtLectureBox, PARAGRAPH_TERMINATORS);

            txtLectureBox.Invoke(new delegateMoveToNextTextFragment(MoveToNextTextFragment),
                                    txtLectureBox, SENTENCE_TERMINATORS);

            txtStatusBox.Invoke((MethodInvoker)delegate { txtStatusBox.Text = txtLectureBox.SelectedText; });

            Log.Debug("selected text=" + txtLectureBox.SelectedText);
            SendTextImmediate(txtLectureBox.SelectedText);
            txtLectureBox.Focus();
            
		}

        //private void ProcessSpeakParagraph2()
        //{
        //    Log.Debug("Entering...");
            
        //    currentState = speakingStates.speakingParagraph;
        //    updateStatusField(_speakingStatus);
        //    txtLectureBox.Invoke(new delegateMoveToNextTextFragment(MoveToNextTextFragment),
        //                            txtLectureBox, PARAGRAPH_TERMINATORS);

        //    // use txtStatusBox to store the sentences used to speak this paragraph
        //    // one sentence at a time
        //    txtStatusBox.Invoke((MethodInvoker)delegate { txtStatusBox.Text = txtLectureBox.SelectedText; });

        //    Log.Debug("txtStatusBox.Text=" + txtStatusBox.Text);

        //    SendTextImmediate(txtLectureBox.SelectedText);
        //    txtLectureBox.Focus();
        //}

		private void ProcessSpeakSentence()
		{
			//StopSpeaking();
            updateStatusField(_speakingStatus);

            // TODO make sure we only need to call invoke when moving to next text fragment
            // as this is the only call used when the TTS reports back index reached
            txtLectureBox.Invoke(new delegateMoveToNextTextFragment(MoveToNextTextFragment), 
                                    txtLectureBox, SENTENCE_TERMINATORS);

			//MoveToNextTextFragment(ref txtLectureBox, SENTENCE_TERMINATOR_SEARCH_STRING);
            txtStatusBox.Invoke((MethodInvoker)delegate { txtStatusBox.Text = txtLectureBox.SelectedText; });
			SendTextImmediate(txtLectureBox.SelectedText);
			txtLectureBox.Focus();
		}

		private void ProcessNextSpeakAllSentence()
		{
            Log.Debug("entering...");
			MoveToNextTextFragment(ref txtLectureBox, SENTENCE_TERMINATORS);
			txtStatusBox.Text = txtLectureBox.SelectedText;

            if (String.IsNullOrEmpty(txtStatusBox.Text))
            {
                currentState = speakingStates.silent;
                clearStatusField();
            }
            else
            {
                updateStatusField(_speakingStatus);
                SendTextImmediate(txtLectureBox.SelectedText);
                txtLectureBox.Focus();
            }
		}

        private void ProcessNextSpeakParagraphSentence()
        {
            Log.Debug("Entering...");
            Log.Debug("txtLectureBox.SelectionStart =" + txtLectureBox.SelectionStart);
            Log.Debug("txtLectureBox.SelectionLength=" + txtLectureBox.SelectionLength);
            Log.Debug("txtLectureBox.Text.Length=" + txtLectureBox.Text.Length);

            string strLectureText = String.Empty;
            strLectureText = txtLectureBox.Text;
            int currentStartPos = txtLectureBox.SelectionStart + txtLectureBox.SelectionLength;

            if (currentStartPos > txtLectureBox.Text.Length)
            {
                currentState = speakingStates.silent;
                clearStatusField();
                return;
            }

            Log.Debug("currentStartPos text is >>>" + (int)strLectureText[currentStartPos] + "<<<");

            if ( (int) strLectureText[currentStartPos] == 13)
            {
                Log.Debug("REACHED END OF PARAGRAPH!");
                currentState = speakingStates.silent;
                clearStatusField();
                txtLectureBox.SelectionStart = currentStartPos + 1;
                txtLectureBox.SelectionLength = 0;
                txtLectureBox.Focus();
                return;
            }
            else
            {
                Log.Debug("moving to next sentence...");
                txtLectureBox.Invoke(new delegateMoveToNextTextFragment(MoveToNextTextFragment),
                txtLectureBox, SENTENCE_TERMINATORS);

                txtStatusBox.Invoke((MethodInvoker)delegate { txtStatusBox.Text = txtLectureBox.SelectedText; });

                Log.Debug("selected text=" + txtLectureBox.SelectedText + "len=" + txtLectureBox.SelectedText.Length.ToString());
                SendTextImmediate(txtLectureBox.SelectedText);
                txtLectureBox.Focus();
            }

            //if (String.IsNullOrEmpty(txtStatusBox.Text))
            //{
            //    currentState = speakingStates.silent;
            //    clearStatusField();
            //}
            //else
            //{
            //    updateStatusField(_speakingStatus);
            //    SendTextImmediate(txtStatusBox.SelectedText);
            //    txtLectureBox.Focus();
            //}
        }

		private void ProcessExit()
		{
            if (LectureManagerAgent.Confirm("Exit Lecture Manager?"))
            {
                AgentManager.Instance.RemoveAgent(this.Handle);

                StopSpeaking();

                removeWatchdogs();

                Aster.Utility.Windows.RestorePreviousWindow();
                WindowActivityMonitor.GetActiveWindow();

                this.Close();
            }
		}

		// CURSOR MENU PROCESSORS

		private void ProcessParagraphUp()
		{
			//StopSpeaking();

			txtLectureBox.Focus();
            NavigateToPreviousTextFragment(ref txtLectureBox, PARAGRAPH_TERMINATORS1);
		}

		private void ProcessParagraphDown()
		{
			//StopSpeaking();

			txtLectureBox.Focus();
			NavigateToNextTextFragment(ref txtLectureBox, PARAGRAPH_TERMINATORS1);
		}

		private void ProcessSentenceUp()
		{
			//StopSpeaking();

			txtLectureBox.Focus();
			MoveToPreviousTextFragment(ref txtLectureBox, SENTENCE_TERMINATORS);
		}

		private void ProcessSentenceDown()
		{
			//StopSpeaking();

			txtLectureBox.Focus();
			MoveToNextTextFragment(ref txtLectureBox, SENTENCE_TERMINATORS);
			//AddNextSentenceToSelection(txtLectureBox, SENTENCE_TERMINATOR_SEARCH_STRING)
		}

		private void ProcessGotoTop()
		{

			//StopSpeaking();

			txtLectureBox.Focus();
			MoveCursorToBeginning(ref txtLectureBox);
			txtLectureBox.ScrollToCaret();

		}

#endregion

		private static LectureManagerMainForm _DefaultInstance;
		public static LectureManagerMainForm DefaultInstance
		{
			get
			{
				if (_DefaultInstance == null)
					_DefaultInstance = new LectureManagerMainForm();

				return _DefaultInstance;
			}
		}

        public void OnPause()
        {
            
        }

        public void OnResume()
        {
        }

        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }
        }

        private void removeWatchdogs()
        {
            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
                _windowActiveWatchdog = null;
            }
        }

        void updateStatusField(String status)
        {
            Utility.Windows.SetText(lblStatus, status);
        }

        void clearStatusField()
        {
            updateStatusField(String.Empty);
        }


        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void txtStatusBox_TextChanged(object sender, EventArgs e)
        {
        
        }

        private void txtLectureBox_TextChanged(object sender, EventArgs e)
        {
        
        }
	}

}