////////////////////////////////////////////////////////////////////////////
// <copyright file="GridMouseMover.cs" company="Intel Corporation">
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

//#define PERFORM_STATE_IDLE_CHECK

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Moves the mouse along a horizontal line across the width
    /// of the display.  First, a line sweeps from top to
    /// bottom. When the user acutates a switch, the mouse
    /// travels from left to right.  The user actuates the switch
    /// once more to select the mouse operation-click, double click
    /// etc.
    /// </summary>
    public class GridMouseMover : IDisposable
    {
        /// <summary>
        /// Width of the grid line
        /// </summary>
        private const int DefaultLineWidth = 3;

        /// <summary>
        /// The timer tick interval
        /// </summary>
        private const int DefaultTimerInterval = 15;

        /// <summary>
        /// How much to move the mouse each step?
        /// </summary>
        private const int GridMouseStep = 5;

        /// <summary>
        /// X origin of the grid line
        /// </summary>
        private const int GridOriginX = 0;

        /// <summary>
        /// Y origin of the grid line
        /// </summary>
        private const int GridOriginY = 0;

        /// <summary>
        /// How much to scroll the line each tiemr tick
        /// </summary>
        private const int LineScrollAmount = 5;

        /// <summary>
        /// Upper bound for speed of mouse movement
        /// </summary>
        private const int MaxMouseMoveSpeed = 20;

        /// <summary>
        /// Upper bound for the speed of grid movement
        /// </summary>
        private const int MaxSweepSpeed = 20;

        /// <summary>
        /// Msecs per secons
        /// </summary>
        private const int MSecsPerSecond = 1000;

        /// <summary>
        /// Height of the invisible form
        /// </summary>
        private int _boundHeight = Screen.PrimaryScreen.Bounds.Height;

        /// <summary>
        /// Width of the invisible form
        /// </summary>
        private int _boundWidth = Screen.PrimaryScreen.Bounds.Width;

        /// <summary>
        /// Mouse move cycle count
        /// </summary>
        private int _currentMouseCycle;

        /// <summary>
        /// Current state of the grid mouse
        /// </summary>
        private MouseMoverStates _currentState;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Invisible form on which the grid is drawn
        /// </summary>
        private Form _drawForm;

        /// <summary>
        /// Is the form closing?
        /// </summary>
        private bool _drawFormClosing;

        /// <summary>
        /// Where was the grid last drawn?
        /// </summary>
        private Point _gridDrawnStartPoint;

        /// <summary>
        /// End point of the grid
        /// </summary>
        private Point _gridEndPoint;

        /// <summary>
        /// Direction of the movement of the mouse
        /// </summary>
        private GridMouseMoveDirections _gridMouseMoveDirection;

        /// <summary>
        /// Where the grid is now
        /// </summary>
        private Point _gridMovingPoint;

        /// <summary>
        /// Pen used to draw the grid
        /// </summary>
        private Pen _gridPen;

        /// <summary>
        /// Color of the pen
        /// </summary>
        private Color _gridPenColor;

        /// <summary>
        /// Where did the grid start?
        /// </summary>
        private Point _gridStartPoint;

        /// <summary>
        /// Direction of the movement of the grid
        /// </summary>
        private GridSweepDirections _gridSweepDirection;

        /// <summary>
        /// Timer to move the mouse
        /// </summary>
        private TimerQueue _mouseMoverTimer;

        /// <summary>
        /// How many milliseonds per step
        /// </summary>
        private int _timerMillisecondsPerStep = 1500;

        /// <summary>
        /// When did the timer start?
        /// </summary>
        private double _whenTimerStarted;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GridMouseMover()
        {
            ScanSpeed = MaxSweepSpeed;
            MouseSpeed = 10;
            LineWidth = DefaultLineWidth;
            Cycles = 2;
            Sweeps = 1;
            StartFromLastCursorPos = true;
            MouseMoveSpeedMultiplier = 6;
            GridScanSpeedMultiplier = 14;
        }

        /// <summary>
        /// Raised when the state changes
        /// </summary>
        public event MouseMoverStateChanged EvtMouseMoverStateChanged;

        /// <summary>
        /// Which way to move the mouse? Left to right or right to left
        /// </summary>
        public enum GridMouseMoveDirections
        {
            AtoB,
            BtoA
        }

        /// <summary>
        /// Which way to sweep the grid?
        /// </summary>
        public enum GridSweepDirections
        {
            TopDown,
            BottomUp,
            LeftRight,
            RightLeft
        }

        /// <summary>
        /// Gets or sets the number of cycles
        /// </summary>
        public int Cycles { get; set; }

        /// <summary>
        /// Multiplier to calculate speed of the grid
        /// </summary>
        public int GridScanSpeedMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the line width
        /// </summary>
        public int LineWidth { get; set; }

        /// <summary>
        /// Multiplier to calculate speed of mouse movement
        /// </summary>
        public int MouseMoveSpeedMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the speed of the mouse movement
        /// </summary>
        public int MouseSpeed { get; set; }

        /// <summary>
        /// Gets or sets the speed of scanning
        /// </summary>
        public int ScanSpeed { get; set; }

        /// <summary>
        /// Gets or sets whether to start the grid from where
        /// the mouse cursor is.
        /// </summary>
        public bool StartFromLastCursorPos { get; set; }

        /// <summary>
        /// Gets the current state
        /// </summary>
        public MouseMoverStates State
        {
            get { return _currentState; }
        }

        /// <summary>
        /// Gets or sets the sweep count
        /// </summary>
        public int Sweeps { get; set; }

        /// <summary>
        /// To be called when the user actuates a switch. Depending
        /// on the current state, it handles appropriately.
        /// </summary>
        /// <returns>true on success</returns>
        public bool Actuate()
        {
            if (_currentState == MouseMoverStates.Grid)
            {
                setStateAndNotify(MouseMoverStates.PreGridMouseMove);
                return true;
            }

            if (_currentState == MouseMoverStates.PreGridMouseMove)
            {
                timerStop();
                setStateAndNotify(MouseMoverStates.Idle);
                return true;
            }

            if (_currentState == MouseMoverStates.GridMouseMove)
            {
                timerStop();
                setStateAndNotify(MouseMoverStates.Idle);
                closeDrawForm();
                return true;
            }

            if (_currentState == MouseMoverStates.Idle)
            {
                // don't do anything
                return false;
            }

            Log.Warn("invalid state detected! state=" + _currentState.ToString());

            timerStop();
            setStateAndNotify(MouseMoverStates.Idle);
            return true;
        }

        /// <summary>
        /// Object disposer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the class variables
        /// </summary>
        public void Init()
        {
            // don't use setState because we don't want to trigger events before
            // it is prudent to do so
            _currentState = MouseMoverStates.Idle;

            _boundWidth = Screen.PrimaryScreen.Bounds.Width;
            _boundHeight = Screen.PrimaryScreen.Bounds.Height;
            Log.Debug("boundWidth=" + _boundWidth + " boundHeight=" + _boundHeight);

            _gridPenColor = Color.FromArgb(255, 255, 0, 0);

            initGridLine();

            var timerDelegate = new TimerQueue.WaitOrTimerDelegate(mmTimer_Tick);
            _mouseMoverTimer = new TimerQueue(DefaultTimerInterval, DefaultTimerInterval, timerDelegate);
        }

        /// <summary>
        /// Checks if the current state is idle
        /// </summary>
        /// <returns>true if it is</returns>
        public bool IsIdle()
        {
            return _currentState == MouseMoverStates.Idle;
        }

        /// <summary>
        /// Sets the direction of grid sweep
        /// </summary>
        /// <param name="direction">direction of sweep</param>
        public void SetGridSweepDirection(GridSweepDirections direction)
        {
            _gridSweepDirection = direction;
            switch (direction)
            {
                case GridSweepDirections.LeftRight:
                    _gridStartPoint.X = 0;
                    _gridStartPoint.Y = 0;
                    _gridEndPoint.X = 0;
                    _gridEndPoint.Y = _boundHeight - 1;
                    break;

                case GridSweepDirections.RightLeft:
                    _gridStartPoint.X = _boundWidth - 1;
                    _gridStartPoint.Y = 0;
                    _gridEndPoint.X = _boundWidth - 1;
                    _gridEndPoint.Y = _boundHeight - 1;
                    break;

                case GridSweepDirections.TopDown:
                    _gridStartPoint.X = 0;
                    _gridStartPoint.Y = 0;
                    _gridEndPoint.X = _boundWidth - 1;
                    _gridEndPoint.Y = 0;
                    break;

                case GridSweepDirections.BottomUp:
                    _gridStartPoint.X = 0;
                    _gridStartPoint.Y = _boundHeight - 1;
                    _gridEndPoint.X = _boundWidth - 1;
                    _gridEndPoint.Y = _boundHeight - 1;
                    break;
            }
        }

        /// <summary>
        /// Starts grid scanning. Creates the invisible form
        /// to draw the line and starts the timer
        /// </summary>
        /// <returns>true on success</returns>
        public bool Start()
        {
            const int gridStartPointOrigin = 0;

            Log.Debug("starting mouse grid...");

            createAndShowDrawForm();

            _currentMouseCycle = 0;
            createPen(_gridPenColor, LineWidth);

            if (StartFromLastCursorPos)
            {
                _gridStartPoint = new Point(gridStartPointOrigin, Cursor.Position.Y);
                _gridEndPoint = new Point(_boundWidth, Cursor.Position.Y);
            }

            Log.Debug("MouseMover.cs::StartMouseGrid() - ScanSpeed=" + ScanSpeed + "  MouseSpeed=" + MouseSpeed);

            _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;

            if ((_gridSweepDirection == GridSweepDirections.BottomUp) ||
                    (_gridSweepDirection == GridSweepDirections.TopDown))
            {
                _timerMillisecondsPerStep = (((MaxSweepSpeed + 1) - ScanSpeed) * MSecsPerSecond * GridScanSpeedMultiplier) / _boundHeight;
                Log.Debug("timerMillisecondsPerStep=" + _timerMillisecondsPerStep + " ScanSpeed=" + ScanSpeed + " boundHeight=" + _boundHeight);
            }

            if ((_gridSweepDirection == GridSweepDirections.LeftRight) ||
                    (_gridSweepDirection == GridSweepDirections.RightLeft))
            {
                _timerMillisecondsPerStep = (((MaxSweepSpeed + 1) - ScanSpeed) * MSecsPerSecond * GridScanSpeedMultiplier) / _boundWidth;
                Log.Debug("timerMillisecondsPerStep=" + _timerMillisecondsPerStep + " ScanSpeed=" + ScanSpeed + " boundHeight=" + _boundHeight);
            }

            setStateAndNotify(MouseMoverStates.Grid);
            timerStart();

            return true;
        }

        /// <summary>
        /// Toggles the direction of mouse movement
        /// </summary>
        /// <returns>the new direction</returns>
        public GridMouseMoveDirections ToggleGridMouseMoveDirection()
        {
            var gmmDirection = _gridMouseMoveDirection;

            gmmDirection = gmmDirection == GridMouseMoveDirections.AtoB ?
                            GridMouseMoveDirections.BtoA :
                            GridMouseMoveDirections.AtoB;

            _gridMouseMoveDirection = gmmDirection;

            return gmmDirection;
        }

        /// <summary>
        /// Toggles the direction of grid sweep
        /// </summary>
        /// <returns>new direction</returns>
        public GridSweepDirections ToggleGridSweepDirection()
        {
            var direction = _gridSweepDirection;

            switch (direction)
            {
                case GridSweepDirections.BottomUp:
                    direction = GridSweepDirections.LeftRight;
                    _gridStartPoint.X = GridOriginX;
                    _gridStartPoint.Y = GridOriginY;
                    _gridEndPoint.X = GridOriginX;
                    _gridEndPoint.Y = _boundHeight - 1;
                    break;

                case GridSweepDirections.LeftRight:
                    direction = GridSweepDirections.RightLeft;
                    _gridStartPoint.X = _boundWidth - 1;
                    _gridStartPoint.Y = GridOriginY;
                    _gridEndPoint.X = _boundWidth - 1;
                    _gridEndPoint.Y = _boundHeight - 1;
                    break;

                case GridSweepDirections.RightLeft:
                    direction = GridSweepDirections.TopDown;
                    _gridStartPoint.X = GridOriginX;
                    _gridStartPoint.Y = GridOriginY;
                    _gridEndPoint.X = _boundWidth - 1;
                    _gridEndPoint.Y = GridOriginY;
                    break;

                case GridSweepDirections.TopDown:
                    direction = GridSweepDirections.BottomUp;
                    _gridStartPoint.X = GridOriginX;
                    _gridStartPoint.Y = _boundHeight - 1;
                    _gridEndPoint.X = _boundWidth - 1;
                    _gridEndPoint.Y = _boundHeight - 1;
                    break;
            }

            _gridSweepDirection = direction;

            return direction;
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
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
                    try
                    {
                        if (_mouseMoverTimer != null)
                        {
                            _mouseMoverTimer.Stop();
                            _mouseMoverTimer.Dispose();
                            _mouseMoverTimer = null;
                        }

                        if (_gridPen != null)
                        {
                            _gridPen.Dispose();
                        }

                        closeDrawForm();
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Closes the invisible form
        /// </summary>
        private void closeDrawForm()
        {
            if (_drawForm != null)
            {
                _drawFormClosing = true;
                _drawForm.Paint -= formPaint;
                Windows.CloseForm(_drawForm);
                _drawForm = null;
            }
        }

        /// <summary>
        /// Creates the invisible form on which the grid line
        /// will be drawn
        /// </summary>
        private void createAndShowDrawForm()
        {
            closeDrawForm();

            _drawFormClosing = false;
            _drawForm = new Form
            {
                BackColor = Color.Magenta,
                TransparencyKey = Color.Magenta,
                Opacity = 0.5f,
                TopMost = false
            };

            _drawForm.TopMost = true;
            _drawForm.ShowInTaskbar = false;
            _drawForm.Paint += formPaint;
            _drawForm.FormBorderStyle = FormBorderStyle.None;
            _drawForm.ControlBox = false;
            _drawForm.Text = String.Empty;
            _drawForm.WindowState = FormWindowState.Maximized;
            _drawForm.ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            _drawForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Left, Screen.PrimaryScreen.WorkingArea.Top);
            _drawForm.Show();
        }

        /// <summary>
        /// Creates a pen with the specifed color and width
        /// </summary>
        /// <param name="penColor">color of the pen</param>
        /// <param name="penWidth">line width</param>
        private void createPen(Color penColor, int penWidth = DefaultLineWidth)
        {
            _gridPen = new Pen(penColor, penWidth);
        }

        /// <summary>
        /// Paint handler for the invisible form
        /// </summary>
        /// <param name="sender">event sencer</param>
        /// <param name="paintEventArgs">event args</param>
        private void formPaint(object sender, PaintEventArgs paintEventArgs)
        {
            if (_currentState == MouseMoverStates.Grid)
            {
                formPaintGrid(paintEventArgs);
            }
        }

        /// <summary>
        /// Draws the grid line
        /// </summary>
        /// <param name="paintEventArgs">Paint event args</param>
        private void formPaintGrid(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.DrawLine(_gridPen, _gridStartPoint, _gridEndPoint);
            _gridDrawnStartPoint = _gridStartPoint;
            switch (_gridSweepDirection)
            {
                case GridSweepDirections.LeftRight:

                    if (_gridEndPoint.X > _boundWidth)
                    {
                        _gridStartPoint.X = 0;
                        _gridEndPoint.X = 0;
                        _currentMouseCycle++;
                    }
                    else
                    {
                        _gridEndPoint.X += LineScrollAmount;
                        _gridStartPoint.X += LineScrollAmount;
                    }

                    break;

                case GridSweepDirections.RightLeft:

                    if (_gridEndPoint.X <= 0)
                    {
                        _gridStartPoint.X = _boundWidth;
                        _gridEndPoint.X = _boundWidth;
                        _currentMouseCycle++;
                    }
                    else
                    {
                        _gridEndPoint.X -= LineScrollAmount;
                        _gridStartPoint.X -= LineScrollAmount;
                    }

                    break;

                case GridSweepDirections.TopDown:

                    if (_gridEndPoint.Y > _boundHeight)
                    {
                        _gridStartPoint.Y = 0;
                        _gridEndPoint.Y = 0;
                        _currentMouseCycle++;
                    }
                    else
                    {
                        _gridEndPoint.Y += LineScrollAmount;
                        _gridStartPoint.Y += LineScrollAmount;
                    }

                    break;

                case GridSweepDirections.BottomUp:

                    if (_gridEndPoint.Y <= 0)
                    {
                        _gridStartPoint.Y = _boundHeight - 1;
                        _gridEndPoint.Y = _boundHeight - 1;
                        _currentMouseCycle++;
                    }
                    else
                    {
                        _gridEndPoint.Y -= LineScrollAmount;
                        _gridStartPoint.Y -= LineScrollAmount;
                    }

                    break;
            }
        }

        /// <summary>
        /// Moves the mouse one step depending on the
        /// direction of movement
        /// </summary>
        /// <param name="step">how much to move</param>
        private void gridStepCursor(int step)
        {
            int curX = _gridMovingPoint.X;
            int curY = _gridMovingPoint.Y;

            // first determine the sweep type.  if it is right-to-left or left-to-right then we will just be
            // modifying the Y component as the mouse will be travelling vertically
            // if it is top-down or bottom-up then the mouse will be travelling horizontally

            if ((_gridSweepDirection == GridSweepDirections.RightLeft) ||
                (_gridSweepDirection == GridSweepDirections.LeftRight))
            {
                switch (_gridMouseMoveDirection)
                {
                    case GridMouseMoveDirections.AtoB:

                        if (curY > (_boundHeight - 1))
                        {
                            curY = 0;
                            _currentMouseCycle++;
                        }
                        else
                        {
                            curY += step;
                        }

                        break;

                    case GridMouseMoveDirections.BtoA:

                        if (curY < 1)
                        {
                            curY = _boundHeight - 1;
                            _currentMouseCycle++;
                        }
                        else
                        {
                            curY -= step;
                        }

                        break;
                }
            }

            if ((_gridSweepDirection == GridSweepDirections.TopDown) ||
                (_gridSweepDirection == GridSweepDirections.BottomUp))
            {
                switch (_gridMouseMoveDirection)
                {
                    case GridMouseMoveDirections.AtoB:

                        if (curX > (_boundWidth - 1))
                        {
                            curX = 0;
                            _currentMouseCycle++;
                        }
                        else
                        {
                            curX += step;
                        }

                        break;

                    case GridMouseMoveDirections.BtoA:

                        if (curX < 1)
                        {
                            curX = _boundWidth - 1;
                            _currentMouseCycle++;
                        }
                        else
                        {
                            curX -= step;
                        }

                        break;
                }
            }

            // now that we have the appropriate coordinate delta for the sweep type and movement direction
            // let's actually set the new cursor position accordingly

            _gridMovingPoint.X = curX;
            _gridMovingPoint.Y = curY;

            Log.Debug("Setting cursor position gridMovingPoint.X " + _gridMovingPoint.X + ", GridMOvyingPOint.Y: " + _gridMovingPoint.Y);

            Cursor.Position = new Point(Convert.ToInt32(curX), Convert.ToInt32(curY));
        }

        /// <summary>
        /// Handles the state where the grid line is draw. If the
        /// total cycle count has exeeded, stops the timer
        /// </summary>
        private void handleStateGrid()
        {
            if (_currentMouseCycle >= Cycles)
            {
                timerStop();
                setStateAndNotify(MouseMoverStates.Idle);
                return;
            }

            var elapsedMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds - _whenTimerStarted;

            if (elapsedMilliseconds > _timerMillisecondsPerStep)
            {
                _drawForm.Invalidate();
                _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }
        }

        /// <summary>
        /// Handles the case where the mouse is moving along the
        /// grid line.  If the max count of sweeps has exceeded, stops the timer
        /// otherwise move the mouse one step.
        /// </summary>
        private void handleStateGridMouseMove()
        {
            var elapsedMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds - _whenTimerStarted;

            if (elapsedMilliseconds > _timerMillisecondsPerStep)
            {
                gridStepCursor(GridMouseStep);
                _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }

            if (_currentMouseCycle >= Sweeps)
            {
                timerStop();
                setStateAndNotify(MouseMoverStates.Idle);
            }
        }

        /// <summary>
        /// Handles the case before the grid line is drawn. Calculates
        /// the timer step depending on the speed parameter
        /// </summary>
        private void handleStatePreGridMouseMove()
        {
            Cursor.Position = new Point(_gridStartPoint.X, _gridStartPoint.Y);
            _gridMovingPoint = _gridDrawnStartPoint;
            _gridMovingPoint.Y = _gridMovingPoint.Y - 1;
            _currentMouseCycle = 0;
            _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;

            if ((_gridSweepDirection == GridSweepDirections.BottomUp) || (_gridSweepDirection == GridSweepDirections.TopDown))
            {
                _timerMillisecondsPerStep = (((MaxMouseMoveSpeed + 1) - MouseSpeed) * MSecsPerSecond * MouseMoveSpeedMultiplier) / _boundHeight;
            }

            if ((_gridSweepDirection == GridSweepDirections.LeftRight) || (_gridSweepDirection == GridSweepDirections.RightLeft))
            {
                _timerMillisecondsPerStep = (((MaxMouseMoveSpeed + 1) - MouseSpeed) * MSecsPerSecond * MouseMoveSpeedMultiplier) / _boundWidth;
            }

            _drawForm.Invalidate();

            setStateAndNotify(MouseMoverStates.GridMouseMove);
        }

        /// <summary>
        /// Creates a pen and initializes grid coordinates
        /// </summary>
        private void initGridLine()
        {
            createPen(_gridPenColor, LineWidth);

            _gridStartPoint = new Point(0, 0);
            _gridEndPoint = new Point(0, _boundHeight);

            _gridMouseMoveDirection = GridMouseMoveDirections.AtoB;
            _gridSweepDirection = GridSweepDirections.LeftRight;
            _gridStartPoint.X = GridOriginX;
            _gridStartPoint.Y = GridOriginY;
            _gridEndPoint.X = GridOriginX;
            _gridEndPoint.Y = _boundHeight - 1;
        }

        /// <summary>
        /// Timer tick function.  Depending on the
        /// state, performs the necessary action
        /// </summary>
        /// <param name="param">optional parameter</param>
        /// <param name="status">optional status</param>
        private void mmTimer_Tick(IntPtr param, bool status)
        {
            //Log.Debug("current time=" + DateTime.Now.TimeOfDay.TotalMilliseconds.ToString() + "  currentAppState=" + currentAppState);

            try
            {
                if (_drawForm == null || _drawFormClosing)
                {
                    return;
                }

                _drawForm.Invoke(new MethodInvoker(delegate
                {
                    switch (_currentState)
                    {
                        case MouseMoverStates.Idle:
                            break;

                        case MouseMoverStates.Grid:
                            handleStateGrid();
                            break;

                        case MouseMoverStates.PreGridMouseMove:
                            handleStatePreGridMouseMove();
                            break;

                        case MouseMoverStates.GridMouseMove:
                            handleStateGridMouseMove();
                            break;

                        default:
                            Log.Debug("invalid state entered! state=" + _currentState);
                            break;
                    }
                }));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sets the grid state and raises event that the
        /// state has changed.
        /// </summary>
        /// <param name="state"></param>
        private void setStateAndNotify(MouseMoverStates state)
        {
            _currentState = state;
            if (EvtMouseMoverStateChanged != null)
            {
                EvtMouseMoverStateChanged(this, new MouseMoverStateChangedEventArgs(state));
            }
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        private void timerStart()
        {
            _mouseMoverTimer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        private void timerStop()
        {
            Log.Debug();
            _mouseMoverTimer.Stop();
        }
    }
}