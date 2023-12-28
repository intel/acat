using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using ACAT.Lib.Core.Utility;

namespace ACAT.Lib.Core.WidgetManagement
{
    class SharpDXWidget
    {
        int trialITI = 200; // in ms
        int pauseTime = 300; //in ms
        int shortPauseTime = 300; //in msgvfiofo9hj;nm]
        int showDecisionTime = 1000; //in ms

        int numEpochs = 60; //for calibration
        int numSequences = 1;
        readonly int numRows = 4;
        readonly int numCols = 10;
        List<int[]> flashingSeq;
        List<int> markersSeq;

        // Colors
        System.Drawing.Color colorButtonOff_background = System.Drawing.Color.FromArgb(64, 64, 64);
        System.Drawing.Color colorButtonOff_foreground = System.Drawing.Color.Silver;

        List<SharpDX.Mathematics.Interop.RawRectangleF> rectBtns;
        // List<String>buttonTextList;
        SharpDX.Mathematics.Interop.RawRectangleF rectExtraButton;
        SharpDX.Mathematics.Interop.RawRectangleF rectExtraButton2;
        TextFormat buttonTextFormat;
        public enum SessionMode
        {
            Calibrate,
            TrainClassifiers,
            FreeType, // not implemented yet
            CopyPaste,
        }
        struct matrixButton
        {
            public int id { get; set; }
            public String text { get; set; }
            public String action { get; set; }
        }
        List<matrixButton> matrixButtonList;
        // Timers
        MicroTimer trialTimer;
        MicroTimer targetOffTimer;
        MicroTimer endEpochTimer;
        MicroTimer showDecisionTimer;
        MicroTimer pauseBetweenEpochsTimer;

        bool seqDone = false;
        int currSeqCount = 1;
        int currEpochCount = 1;
        int currtButtonCount = 0;
        int slotTrialTimerCount = 0;

        matrixButton currentTarget;
        matrixButton nextTarget;
        Queue<string> nextPhrases;
        String currentPhrase;
        String copiedPhrase;
        String subphraseToCopy;
        bool updatePhraseToCopyFlag;

        // SharpDX
        SharpDX.Direct3D11.Device sharpDX_device;
        SwapChain sharpDX_swapChain;
        RenderTarget sharpDX_d2dRenderTarget;
        SharpDX.Direct2D1.Factory sharpDX_d2dFactory;


        #region Colors
        SharpDX.Mathematics.Interop.RawColor4 clrblu = new SharpDX.Mathematics.Interop.RawColor4(0.0f, 0.0f, 1.0f, 1.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrblack = new SharpDX.Mathematics.Interop.RawColor4(0.0f, 0.0f, 0.0f, 1.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrDarkGray = new SharpDX.Mathematics.Interop.RawColor4(0.2f, 0.2f, 0.2f, 1.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrWhite = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 1.0f, 1.0f, 1.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrRed = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 0.0f, 0.0f, 1.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrGreen = new SharpDX.Mathematics.Interop.RawColor4(0.0f, 1.0f, 0.0f, 1.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrTrans = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 0.0f, 0.0f, 0.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrYellow = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 1.0f, 0.5f, 1.0f);
        SharpDX.Mathematics.Interop.RawColor4 clrGray = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 0.5f, 1f);


        SolidColorBrush solidColorBrushOn;
        SolidColorBrush solidColorBrushOff;
        SolidColorBrush solidColorBrushButtonTextOn;
        SolidColorBrush solidColorBrushButtonTextOff;
        SolidColorBrush solidColorBrushExtraBoxOn;
        SolidColorBrush solidColorBrushExtraBoxOff;
        SolidColorBrush solidColorBrushTarget;
        SolidColorBrush solidColorBrushDecision;
        SolidColorBrush solidColorBrushDecisionCorrect;
        SolidColorBrush solidColorBrushDecisionIncorrect;

        #endregion

        //liblsl.StreamOutlet outl;
        //string logText;
        //string[] lslArray = new string[1];

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        //private SignalMonitorWithMarkersForm SignalMonitorForm;

        public void InitSharpDX(Form form)
        {
            try
            {
                bool success = InitSwapChain(form);
                Debug.Write("Created swapchains for buttons");
            }
            catch (Exception ex)
            {
                String message = "Exception occurred during form initilization: " + ex.Message + " Inner exception: " + ex.InnerException.Message;
                MessageBox.Show(message, "Init Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            markersSeq = new List<int>();
            // Text for the different buttons in the matrix (this can be done diferently)
            matrixButtonList = new List<matrixButton>();
            matrixButton currMatrixButton = new matrixButton();
            for (int i = 0; i < 40; i++)
            {
                currMatrixButton.id = i + 1;
                currMatrixButton.text = Convert.ToChar(i + 65).ToString();
                currMatrixButton.action = currMatrixButton.text;
                matrixButtonList.Add(currMatrixButton); //Convert ASCII to char
            }
            currMatrixButton.id = 27; currMatrixButton.text = "_"; currMatrixButton.action = " ";
            matrixButtonList.Add(currMatrixButton);
            currMatrixButton.id = 28; currMatrixButton.text = "Del"; currMatrixButton.action = "";
            matrixButtonList.Add(currMatrixButton);
            // Text buttons
            SharpDX.DirectWrite.Factory directWriteFactory = new SharpDX.DirectWrite.Factory();
            buttonTextFormat = new TextFormat(directWriteFactory, "Arial", SharpDX.DirectWrite.FontWeight.Bold, SharpDX.DirectWrite.FontStyle.Normal, 28.0f);
            buttonTextFormat.TextAlignment = TextAlignment.Center;
            // Add rectangle buttons to rectBtns list
            rectBtns = new List<SharpDX.Mathematics.Interop.RawRectangleF>();
            int buttonIdx = 0;
            foreach (var button in form.Controls.OfType<Control>().OrderBy(x => Convert.ToInt32(x.Tag)))
            {
                if (button is ScannerButtonControl)
                {
                    if (!button.Name.Equals("btnTrigger"))
                    {
                        // Add letters
                        button.Text = matrixButtonList[buttonIdx].text;
                        button.Font = new System.Drawing.Font("Arial", 30);
                        button.BackColor = colorButtonOff_background;
                        button.ForeColor = colorButtonOff_foreground;
                        rectBtns.Add(new SharpDX.Mathematics.Interop.RawRectangleF(button.Left, button.Top, button.Right, button.Bottom));

                        buttonIdx++;
                    }
                    else if (button.Name.Equals("btnTrigger"))
                    {
                        //rectExtraButton = new SharpDX.Mathematics.Interop.RawRectangleF(button.Left, button.Top, button.Right, button.Bottom);
                        //rectExtraButton2 = new SharpDX.Mathematics.Interop.RawRectangleF(btnTrigger2.Left, btnTrigger2.Top, btnTrigger2.Right, btnTrigger2.Bottom);
                        rectExtraButton = new SharpDX.Mathematics.Interop.RawRectangleF(button.Left, button.Top, button.Right, button.Bottom);
                    }
                }
            }
            // Initialze colors 
            solidColorBrushOn = new SolidColorBrush(sharpDX_d2dRenderTarget, clrGray);
            solidColorBrushOff = new SolidColorBrush(sharpDX_d2dRenderTarget, clrDarkGray); //clrblack
            solidColorBrushExtraBoxOn = new SolidColorBrush(sharpDX_d2dRenderTarget, clrWhite);
            solidColorBrushExtraBoxOff = new SolidColorBrush(sharpDX_d2dRenderTarget, clrblack);
            solidColorBrushDecision = new SolidColorBrush(sharpDX_d2dRenderTarget, clrGreen);
            solidColorBrushDecisionCorrect = new SolidColorBrush(sharpDX_d2dRenderTarget, clrGreen);
            solidColorBrushDecisionIncorrect = new SolidColorBrush(sharpDX_d2dRenderTarget, clrRed);
            solidColorBrushTarget = new SolidColorBrush(sharpDX_d2dRenderTarget, clrblu);
            solidColorBrushButtonTextOn = new SolidColorBrush(sharpDX_d2dRenderTarget, clrblack);
            solidColorBrushButtonTextOff = new SolidColorBrush(sharpDX_d2dRenderTarget, clrWhite);
            // Create flashing sequence (row 1 row 2 row 3 column 1 column2 ... column 7)
            flashingSeq = new List<int[]>();
            for (int seqIdx = 0; seqIdx < numRows; seqIdx++)
            {
                List<int> seq = new List<int>();
                for (int c = 1; c <= numCols; c++)
                {
                    seq.Add(seqIdx * numCols + c);
                }

                flashingSeq.Add(seq.ToArray());
            }
            for (int seqIdx = 0; seqIdx < numCols; seqIdx++)
            {
                List<int> seq = new List<int>();
                for (int r = 0; r < numRows; r++)
                {
                    seq.Add(r * numCols + seqIdx + 1);
                }

                flashingSeq.Add(seq.ToArray());
            }

            nextTarget = findButtomFromChar('A');
        }
        private int findIndexFromChar(char c)
        {
            foreach (matrixButton m in matrixButtonList)
            {
                if (m.text.Equals(c.ToString()))
                    return m.id;
            }
            return 0;
        }

        private matrixButton findButtomFromIndex(int id)
        {
            foreach (matrixButton m in matrixButtonList)
            {
                if (m.id == id)
                    return m;
            }
            return new matrixButton();
        }

        private matrixButton findButtomFromChar(char c)
        {
            foreach (matrixButton m in matrixButtonList)
            {
                if (m.action.Equals(c.ToString()))
                    return m;
            }
            return new matrixButton();
        }


        /// <summary>
        /// In CopyPhraseMode, updates the phrase to be copied
        /// </summary>
        /// <returns></returns>
        private bool UpdatePhraseToCopy()
        {
            if (nextPhrases.Count > 0)
            {
                currentPhrase = nextPhrases.Dequeue();
                int posBreakPoint = currentPhrase.IndexOf("_"); //'_' is used to separate already typed text (useful for LM) from text to copy

                if (posBreakPoint >= 0)
                {
                    string prevSubPhrase = currentPhrase.Substring(0, posBreakPoint);
                    string postSubPhrase = currentPhrase.Substring(posBreakPoint + 1);
                    currentPhrase = prevSubPhrase + postSubPhrase; // removes _ that breaks the phrse
                    subphraseToCopy = postSubPhrase;
                    copiedPhrase = prevSubPhrase;
                }
                else
                {
                    subphraseToCopy = currentPhrase;
                    copiedPhrase = "";
                }
                nextTarget = findButtomFromChar(subphraseToCopy[0]);
                updatePhraseToCopyFlag = false;
            }
            return true;
        }


        /// <summary>
        ///  Run Epoch
        /// </summary>
        private void StartEpoch()
        {

            if (currEpochCount <= numEpochs)
            {
                // Select target (randomly between number of available boxes)
                Debug.Write("runEpoch() \n");

                int target = 0;
                if (updatePhraseToCopyFlag)
                    UpdatePhraseToCopy();

                target = nextTarget.id;
                currentTarget = nextTarget;
                currEpochCount += 1;
                markersSeq.Add(target + 100); //Add to markers (adding 100 for target)

                // Display target
                Debug.Write("runEpoch(). Target:" + currtButtonCount + " on \n");
                ChangeColorButtons(target, true, true, 1); //turn on

                // Start timer
                targetOffTimer.Start();
                Debug.Write("runEpoch(). Target timer start.\n");
            }

        }

        /// <summary>
        /// Run sequence
        /// </summary>
        private void StartSequence()
        {
            Debug.Write("runSequence()\n");

            if (currSeqCount <= numSequences) // Sequences in epoch
            {
                trialTimer = new MicroTimer();
                trialTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(TrialTimer_Tick);
                int interval = trialITI * 1000 / 4; // On for 75% time, Off for 25% (done in paint section)
                trialTimer.Interval = interval; // for 0.75% on, 0.25% off

                // Start seq by plotting frist trial 
                slotTrialTimerCount = 0;
                trialTimer.Start();

                Debug.Write("runSequence. trialTimer.Start()\n");

                currtButtonCount = 0;
                currSeqCount += 1;
            }

        }


        private void ProcessEndSequence()
        {
            // This block added to run w/o device, will just select a random button and show. Only 1 reptition.
            numSequences = 1;
            if (currSeqCount < numSequences) // No decision made, start new sequence
            {
                StartSequence();
            }
            else // All sequences in epoch finalize = decision made
            {
                int decisionIdx = 3; //will always display c (buttonID=3) as decision. This is to run it w/o device
                matrixButton decision = findButtomFromIndex(decisionIdx);
                Thread.Sleep(500);
                ShowDecision(decision, false);
            }
            currSeqCount = 1;
        }
        private void ShowDecision(matrixButton decision, bool correct)
        {
            // Select target (randomly between number of available boxes)
            Debug.Write("showDecision() \n");

            // Display decision
            currentTarget = decision;
            List<int> d = new List<int> { decision.id };
            Debug.Write("runEpoch(). Target:" + currtButtonCount + " on \n");
            /*if (correct)
                ChangeColorButtons(d, true, true, 3); //turn on
            else
                ChangeColorButtons(d, true, true, 4); //turn on*/

            // Start timer
            showDecisionTimer.Start();
            Debug.Write("showDecision(). showDecision timer start.\n");

        }
        private void CreateTimers()
        {
            trialTimer = new MicroTimer();
            trialTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(TrialTimer_Tick);
            int interval = trialITI * 1000 / 4; // Int32.Parse(intervalTxtBox.Text) * 1000 / 4;
            trialTimer.Interval = interval; // for 0.75% on, 0.25% off
            trialTimer.Stop();

            targetOffTimer = new MicroTimer();
            targetOffTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(TargetOffTimer_Tick);
            targetOffTimer.Interval = pauseTime * 1000;
            targetOffTimer.Stop();

            endEpochTimer = new MicroTimer();
            endEpochTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(EndEpochTimer_Tick);
            endEpochTimer.Interval = pauseTime * 1000;
            endEpochTimer.Stop();

            showDecisionTimer = new MicroTimer();
            showDecisionTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(ShowDecisionTimer_Tick);
            showDecisionTimer.Interval = showDecisionTime * 1000;
            showDecisionTimer.Stop();

            pauseBetweenEpochsTimer = new MicroTimer();
            pauseBetweenEpochsTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(PauseBetweenEpochs_Tick);
            pauseBetweenEpochsTimer.Interval = shortPauseTime * 1000;
            pauseBetweenEpochsTimer.Stop();
        }

        private void StopTimers()
        {
            trialTimer.Stop();
            targetOffTimer.Stop();
            endEpochTimer.Stop();
            showDecisionTimer.Stop();
            pauseBetweenEpochsTimer.Stop();

            endEpochTimer = null;
            trialTimer = null;
            targetOffTimer = null;
            showDecisionTimer = null;
            pauseBetweenEpochsTimer = null;
        }


        /// <summary>
        /// Stop highlight target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetOffTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            Debug.Write("targetOffTimer_Tick \n");

            if (seqDone)
            {
                Debug.Write("targetOffTimer_Tick. TtargetOffTimer Stop \n");
                targetOffTimer.Stop();
                seqDone = false;

                StartSequence();
            }
            else
            {
                // target off
                Debug.Write("targetOffTimer_Tick. Target: " + currtButtonCount + "off \n");
                List<int> t = new List<int> { currentTarget.id };
                ChangeColorButtons(t, false, true, 1); //turn off

                targetOffTimer.Stop();

                // Start sequence
                StartSequence();

            }
        }

        private void TrialTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            slotTrialTimerCount++;
            Debug.Write("trialTimer_Tick. Slot:" + slotTrialTimerCount + "\n");

            if (slotTrialTimerCount == 1)
            {

                if (currtButtonCount <= numRows + numCols)
                {
                    ChangeColorButtons(flashingSeq[currtButtonCount].ToList(), true, true, 0); //turn on
                    Debug.Write("trialTimer_Tick. Button " + currtButtonCount + " on \n");

                    // Add to markers
                    markersSeq.Add(currtButtonCount + 1);
                }
            }
            else if (slotTrialTimerCount == 3)
            {
                Debug.Write("trialTimer_Tick. Button:" + currtButtonCount + " off \n");
                ChangeColorButtons(flashingSeq[currtButtonCount].ToList(), true, true, 0, true); //turn on
            }
            else if (slotTrialTimerCount == 4)
            {
                ChangeColorButtons(flashingSeq[currtButtonCount].ToList(), false, true, 0); //turn on

                currtButtonCount += 1;

                slotTrialTimerCount = 0;

                if (currtButtonCount == numRows + numCols) // End of sequence
                {
                    Debug.Write("trialTimer_Tick. Sequence ended \n");
                    seqDone = true;
                    //added
                    trialTimer.Stop();
                    ProcessEndSequence();
                    Debug.Write("trialTimer_Tick. targetOffTimer.Start() \n");
                }
            }

        }

        private void EndEpochTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            Debug.Write("endEpochTimer \n");

            //showDecision(); 
            endEpochTimer.Stop();
            pauseBetweenEpochsTimer.Start();
        }

        private void ShowDecisionTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            Debug.Write("showDecisionTimer \n");
            // Display decision
            Debug.Write("runEpoch(). Target:" + currtButtonCount + " on \n");
            ChangeColorButtons(currentTarget.id, false, true, 2); //turn off
            showDecisionTimer.Stop();
            showDecisionTimer.Stop();
            pauseBetweenEpochsTimer.Start();
        }

        private void PauseBetweenEpochs_Tick(object sender, MicroTimerEventArgs e)
        {
            Debug.Write("showDecisionTimer \n");
            StartEpoch();
            pauseBetweenEpochsTimer.Stop();
        }

        private void DrawMatrix()
        {
            // Matrix
            int i = 0;
            foreach (SharpDX.Mathematics.Interop.RawRectangleF rect in rectBtns)
            {
                sharpDX_d2dRenderTarget.DrawRectangle(rect, solidColorBrushOn);

                SharpDX.Mathematics.Interop.RawRectangleF rect2 = new SharpDX.Mathematics.Interop.RawRectangleF(rect.Left, rect.Top + 50, rect.Right, rect.Bottom);
                sharpDX_d2dRenderTarget.DrawText(matrixButtonList[i].text, buttonTextFormat, rect2, solidColorBrushButtonTextOff);// highli
                i++;
            }
            // Trigger/marker button (for optical sensor)
            sharpDX_d2dRenderTarget.DrawRectangle(rectExtraButton, solidColorBrushExtraBoxOn);
        }
        private void ChangeColorButtons(int buttonID, bool highlightOn, bool useExtraBox, int type, bool turnOnNow = false)
        {
            List<int> l = new List<int> { buttonID };
            ChangeColorButtons(l, highlightOn, useExtraBox, type, turnOnNow);
        }

        private void ChangeColorButtons(List<int> buttonIDs, bool highlightOn, bool useExtraBox, int type, bool turnOnNow = false)
        {

            SolidColorBrush colorRectExtraBox, colorBorder, colorRect, colorBorderExtraBox;

            if (highlightOn)
            {
                colorBorderExtraBox = solidColorBrushExtraBoxOn; //solidColorBrushExtraBoxOn
                colorRectExtraBox = solidColorBrushExtraBoxOn;
                colorBorder = solidColorBrushOn; //solidColorBrushOff
                colorRect = solidColorBrushOn;

                if (type == 0) // trial
                {
                    colorBorder = solidColorBrushOn; //solidColorBrushOff
                    colorRect = solidColorBrushOn;
                }
                if (type == 1)
                {
                    colorBorder = solidColorBrushOn; //solidColorBrushOff
                    colorRect = solidColorBrushTarget;
                }
                if (type == 2) // decision
                {
                    colorBorder = solidColorBrushOn; //solidColorBrushOff
                    colorRect = solidColorBrushDecision;
                }
                if (type == 3) //correct decision
                {
                    colorBorder = solidColorBrushOn;
                    colorRect = solidColorBrushDecisionCorrect;
                }
                if (type == 4) // incorrect decision
                {
                    colorBorder = solidColorBrushOn;
                    colorRect = solidColorBrushDecisionIncorrect;
                }

            }
            else
            {
                colorBorder = solidColorBrushOn; //solidColorBrushOn
                colorRect = solidColorBrushOff;
                colorBorderExtraBox = solidColorBrushExtraBoxOff; //solidColorBrushExtraBoxOn
                colorRectExtraBox = solidColorBrushExtraBoxOff;
            }

            SharpDX.DirectWrite.Factory directWriteFactory = new SharpDX.DirectWrite.Factory();


            sharpDX_d2dRenderTarget.BeginDraw();
            sharpDX_d2dRenderTarget.Clear(clrDarkGray);
            DrawMatrix();
            // SharpDX.DirectWrite.Factory directWriteFactory = new SharpDX.DirectWrite.Factory();
            foreach (int buttonID in buttonIDs)
            {
                sharpDX_d2dRenderTarget.DrawRectangle(rectBtns[buttonID - 1], colorBorder);
                sharpDX_d2dRenderTarget.FillRectangle(rectBtns[buttonID - 1], colorRect);

                SharpDX.Mathematics.Interop.RawRectangleF rect = new SharpDX.Mathematics.Interop.RawRectangleF(rectBtns[buttonID - 1].Left, rectBtns[buttonID - 1].Top + 50, rectBtns[buttonID - 1].Right, rectBtns[buttonID - 1].Bottom);
                sharpDX_d2dRenderTarget.DrawText(matrixButtonList[buttonID - 1].text, buttonTextFormat, rect, solidColorBrushButtonTextOff);// highlightOn? solidColorBrushButtonTextOn : solidColorBrushButtonTextOff);
            }

            if (useExtraBox) //(for optical sensor)
            {
                sharpDX_d2dRenderTarget.DrawRectangle(rectExtraButton, colorBorderExtraBox);
                sharpDX_d2dRenderTarget.FillRectangle(rectExtraButton, colorRectExtraBox);

                sharpDX_d2dRenderTarget.DrawRectangle(rectExtraButton2, colorBorderExtraBox);
                sharpDX_d2dRenderTarget.FillRectangle(rectExtraButton2, colorRectExtraBox);

                if (turnOnNow)
                {
                    sharpDX_d2dRenderTarget.DrawRectangle(rectExtraButton, solidColorBrushExtraBoxOff);
                    sharpDX_d2dRenderTarget.FillRectangle(rectExtraButton, solidColorBrushExtraBoxOff);
                }
            }
            sharpDX_d2dRenderTarget.EndDraw();

            sharpDX_swapChain.Present(1, PresentFlags.None);
            // directWriteFactory.Dispose();
        }


        public void startSequences()
        {
            currEpochCount = 1;
            currSeqCount = 0;
            CreateTimers();
            markersSeq = new List<int>();
            Debug.Write("Starting...");
            StartEpoch();
        }

        private void stopSequences()
        {
            currEpochCount = 0;
            currSeqCount = 0;
            StopTimers();
        }


        public void formClossing()
        {
            // Stop the timer, wait for up to 1 sec for current event to finish,
            //  if it does not finish within this time abort the timer thread
            try
            {
                if (!trialTimer.StopAndWait(1000))
                {
                    trialTimer.Abort();
                }
                if (trialTimer != null)
                    trialTimer.Stop();

                //dipose of all objects
                sharpDX_device.Dispose();
                sharpDX_swapChain.Dispose();
                stopSequences();
            }
            catch (Exception es)
            {

            }

        }

        private SwapChainDescription CreateSwapChainDesc(Form frm)
        {
            //try
            //{
            return new SwapChainDescription()
            {
                BufferCount = 3,                                 //how many buffers are used for writing. it's recommended to have at least 2 buffers but this is an example
                Flags = SwapChainFlags.None,
                IsWindowed = true,                               //it's windowed
                ModeDescription = new ModeDescription(
              frm.ClientSize.Width,                       //windows veiwable width
              frm.ClientSize.Height,                      //windows veiwable height
              new Rational(60, 1),                          //refresh rate
              Format.R8G8B8A8_UNorm),                      //pixel format, need to research this
                OutputHandle = frm.Handle,                      //the magic 
                SampleDescription = new SampleDescription(1, 0), //the first number is how many samples to take, anything above one is multisampling.
                SwapEffect = SwapEffect.FlipSequential, //Discard
                Usage = Usage.RenderTargetOutput
            };
        }
        private bool InitSwapChain(Form form)
        {

            SwapChainDescription swpChn = CreateSwapChainDesc(form);
            //create device with swap chain handle set to the form
            SharpDX.Direct3D11.Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware,//hardware if you have a graphics card otherwise you can use software
                        DeviceCreationFlags.BgraSupport,
                        swpChn,                                 //the swapchain description made above
                                out sharpDX_device, out sharpDX_swapChain                        //our directx objects
                                );
            //create the 2D device 
            sharpDX_d2dFactory = new SharpDX.Direct2D1.Factory();
            SharpDX.DXGI.Factory factory = sharpDX_swapChain.GetParent<SharpDX.DXGI.Factory>();

            // New RenderTargetView from the backbuffer
            Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(sharpDX_swapChain, 0);
            var renderView = new RenderTargetView(sharpDX_device, backBuffer);
            Surface surface = backBuffer.QueryInterface<Surface>();
            sharpDX_d2dRenderTarget = new RenderTarget(sharpDX_d2dFactory, surface,
                                                                    new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            return true;
        }



















    }
}
