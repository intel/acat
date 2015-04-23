////////////////////////////////////////////////////////////////////////////
// <copyright file="RadarMouseMover.cs" company="Intel Corporation">
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

//#define DRAW_RADAR_LINE_DURING_MOVE
//#define PERFORM_STATE_IDLE_CHECK

using System;
using System.Collections.Generic;
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
    /// Moves the mouse in a radial direction.  First, a radar
    /// line sweeps across the display. When the user actuates
    /// a switch, it stops the radar and the mouse moves along
    /// the radar line.  When the user actuates a switch once
    /// again, mouse stops moving. The user can then perform
    /// mouse operations such as click, double click etc
    /// </summary>
    public class RadarMouseMover : IDisposable
    {
        /// <summary>
        /// Clockwise direction
        /// </summary>
        private const int Clockwise = 1;

        /// <summary>
        /// Counter clockwise direction
        /// </summary>
        private const int CounterClockwise = -1;

        /// <summary>
        /// Pen width
        /// </summary>
        private const int DefaultPenWidth = 3;

        /// <summary>
        /// Timer interval
        /// </summary>
        private const int DefaultTimerInterval = 15;

        /// <summary>
        /// Max speed of the mouse movement along the
        /// radar line
        /// </summary>
        private const int MaxRadialSpeed = 20;

        /// <summary>
        /// Max speed of the radar line
        /// </summary>
        private const int MaxRotateSpeed = 20;

        /// <summary>
        /// Upper bound of step used for radar move increment
        /// </summary>
        private const int MaxRotationStep = 10;

        /// <summary>
        /// Edge case. If the radar origin is less than this
        /// amount, the radar movement is handled differently.
        /// </summary>
        private const float MinDistanceFromEdgeInches = 0.2f;

        /// <summary>
        /// Lower bound of step used for radar move increment
        /// </summary>
        private const int MinRotationStep = 1;

        /// <summary>
        /// By how much to move the mouse along the radial line
        /// </summary>
        private const int MouseStep = 5;

        /// <summary>
        /// How many msecs per second
        /// </summary>
        private const int MsecsPerSec = 1000;

        /// <summary>
        /// Height of the invisible form on which the radar line is drawn
        /// </summary>
        private int _boundHeight = Screen.PrimaryScreen.Bounds.Height;

        /// <summary>
        /// Width of the invisible form on which the radar line is drawn
        /// </summary>
        private int _boundWidth = Screen.PrimaryScreen.Bounds.Width;

        /// <summary>
        /// Total angle covered by the radar line
        /// </summary>
        private int _cumulativeAngle;

        /// <summary>
        /// Current radar angle
        /// </summary>
        private int _currentAngle;

        /// <summary>
        /// How many cycles has the mouse done so far?
        /// </summary>
        private int _currentMouseCycle;

        /// <summary>
        /// The index of the point in the Bresehnam line where
        /// the cursor is currently at
        /// </summary>
        private int _currentRadialPoint;

        /// <summary>
        /// Current state of the radar mouse mover
        /// </summary>
        private MouseMoverStates _currentState;

        /// <summary>
        /// Has this object been disposed yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Invisible form on which the radar line is drawn
        /// </summary>
        private Form _drawForm;

        /// <summary>
        /// The invisible form is closing
        /// </summary>
        private bool _drawFormClosing;

        /// <summary>
        /// Final point of the radar line
        /// </summary>
        private Point _finalPoint;

        /// <summary>
        /// Initial angle of the radar line
        /// </summary>
        private int _initialAngle;

        /// <summary>
        /// Origin of the radar line
        /// </summary>
        private Point _initialPoint;

        /// <summary>
        /// Converts MinDistanceFromEdgeInches to pixels using
        /// horizontal DPI of the display
        /// </summary>
        private int _minDistanceFromEdgeX;

        /// <summary>
        /// Converts MinDistanceFromEdgeInches to pixels using
        /// vertical DPI of the display
        /// </summary>
        private int _minDistanceFromEdgeY;

        /// <summary>
        /// Timer for moving the mouse
        /// </summary>
        private TimerQueue _mouseMoverTimer;

        /// <summary>
        /// Pen to use to draw the radar line
        /// </summary>
        private Pen _pen;

        /// <summary>
        /// Color of the pen
        /// </summary>
        private Color _penColor;

        /// <summary>
        /// Which direction to move the radar
        /// </summary>
        private int _radarDirection = Clockwise;

        /// <summary>
        /// Index of the last point in the Bresenham line that
        /// lies within the bounds of the display
        /// </summary>
        private int _radialIndexMax;

        /// <summary>
        /// List of Bresenham points along which the
        /// mouse will move
        /// </summary>
        private List<Point> _radialPoints;

        /// <summary>
        /// Radius of the radar line
        /// </summary>
        private int _radius = 100;

        /// <summary>
        /// How much to move the radar each step?
        /// </summary>
        private int _rotationStep = 1;

        /// <summary>
        /// How many milliseconds for each step of radar movement
        /// </summary>
        private int _timerMillisecondsPerStep = 1500;

        /// <summary>
        /// When did we start the timer
        /// </summary>
        private double _whenTimerStarted;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public RadarMouseMover()
        {
            RotatingSpeed = 15;
            RadialSpeed = 100;
            RadialSweeps = 1;
            LineWidth = DefaultPenWidth;
            RotatingSweeps = 2;
            StartFromLastCursorPos = true;
            RotatingSpeedMultiplier = 4;
            RadialSpeedMultiplier = 12;
        }

        /// <summary>
        /// Raised when the mouse mover state changes
        /// </summary>
        public event MouseMoverStateChanged EvtMouseMoverStateChanged;

        /// <summary>
        /// Which way to move the radar
        /// </summary>
        public enum RadarSweepDirections
        {
            ClockWise,
            CounterClockwise
        }

        /// <summary>
        /// Gets or sets the radar line width
        /// </summary>
        public int LineWidth { get; set; }

        /// <summary>
        /// Gets or sets the radial speed of the mouse
        /// cursor along the radial line
        /// </summary>
        public int RadialSpeed { get; set; }

        /// <summary>
        /// Gets or sets the multiplier to calculate the speed of the mouse
        /// along the radar line
        /// </summary>
        public int RadialSpeedMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the number of sweeps of the
        /// mouse cursor
        /// </summary>
        public int RadialSweeps { get; set; }

        /// <summary>
        /// Gets or sets the radar speed
        /// </summary>
        public int RotatingSpeed { get; set; }

        /// <summary>
        /// Gets or sets the multiplier to calculate the radar rotating speed
        /// </summary>
        public int RotatingSpeedMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the number of radar sweeps
        /// </summary>
        public int RotatingSweeps { get; set; }

        /// <summary>
        /// Gets or sets how much to move the radar each step
        /// </summary>
        public int RotationStep
        {
            get
            {
                return _rotationStep;
            }

            set
            {
                if (value < MinRotationStep)
                {
                    _rotationStep = MinRotationStep;
                }
                else if (value > MaxRotationStep)
                {
                    _rotationStep = MaxRotationStep;
                }
                else
                {
                    _rotationStep = value;
                }
            }
        }

        /// <summary>
        /// Should the radar origin start at the position of
        /// the cursor?  Otherwise, it starts at the middle
        /// of the display
        /// </summary>
        public bool StartFromLastCursorPos { get; set; }

        /// <summary>
        /// Gets or sets the current state
        /// </summary>
        public MouseMoverStates State
        {
            get { return _currentState; }
        }

        /// <summary>
        /// Call this whenever the user actuates the trigger switch
        /// </summary>
        /// <returns>true on success</returns>
        public bool Actuate()
        {
            bool retVal = true;

            switch (_currentState)
            {
                case MouseMoverStates.Radar:
                    setStateAndNotify(MouseMoverStates.PreRadarMouseMove);
                    break;

                case MouseMoverStates.PreRadarMouseMove:
                    timerStop();
                    setStateAndNotify(MouseMoverStates.Idle);
                    break;

                case MouseMoverStates.RadarMouseMove:
                    timerStop();
                    setStateAndNotify(MouseMoverStates.Idle);
                    closeDrawForm();
                    break;

                case MouseMoverStates.Idle:
                    retVal = false;
                    break;

                default:
                    timerStop();
                    setStateAndNotify(MouseMoverStates.Idle);
                    break;
            }

            return retVal;
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
        /// Initializes mouse mover
        /// </summary>
        public void Init()
        {
            _currentState = MouseMoverStates.Idle;

            _boundWidth = Screen.PrimaryScreen.Bounds.Width;
            _boundHeight = Screen.PrimaryScreen.Bounds.Height;
            Log.Debug("boundWidth=" + _boundWidth + " boundHeight=" + _boundHeight);

            // we could use the radius of the current form/window
            // or allow the controlling app to pass in a specific radius
            //radius = boundWidth; // half the screen
            _radius = 20000;

            _penColor = Color.FromArgb(255, 255, 0, 0);

            initRadarLine();

            var timerDelegate = new TimerQueue.WaitOrTimerDelegate(mmTimer_Tick);
            _mouseMoverTimer = new TimerQueue(DefaultTimerInterval, DefaultTimerInterval, timerDelegate);
        }

        /// <summary>
        /// Checks if the radar move mover is idle
        /// </summary>
        /// <returns>true if it is</returns>
        public bool IsIdle()
        {
            return _currentState == MouseMoverStates.Idle;
        }

        /// <summary>
        /// Sets the radar sweep direction
        /// </summary>
        /// <param name="direction">Sweep direction</param>
        public void SetSweepDirection(RadarSweepDirections direction)
        {
            _radarDirection = (direction == RadarSweepDirections.CounterClockwise) ? CounterClockwise : Clockwise;
        }

        /// <summary>
        /// Starts radar movement
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            Log.Debug();

            createAndShowDrawForm();

            calcMinDistanceFromEdge();

            if (!StartFromLastCursorPos)
            {
                Cursor.Position = new Point(_boundWidth / 2, _boundHeight / 2);
                _initialPoint = Cursor.Position;
            }
            else
            {
                _initialPoint = Cursor.Position;
            }

            _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;

            _timerMillisecondsPerStep = (((MaxRotateSpeed + 1) - RotatingSpeed) * MsecsPerSec * RotatingSpeedMultiplier) / 360;

            Log.Debug("whenTimerStarted=" + _whenTimerStarted +
                        "  timerMillisecondsPerStep=" + _timerMillisecondsPerStep +
                        " RotatingSpeed=" + RotatingSpeed);

            _currentMouseCycle = 0;

            setStateAndNotify(MouseMoverStates.Radar);

            _initialAngle = (_radarDirection == CounterClockwise) ? 90 : 270;
            _currentAngle = _initialAngle;
            _cumulativeAngle = 0;

            timerStart();
            return true;
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

                        if (_pen != null)
                        {
                            _pen.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }

                    closeDrawForm();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Converts MinDistanceFromEdgeInches to pixels using
        /// horizontal and vertical dpi of the display
        /// </summary>
        private void calcMinDistanceFromEdge()
        {
            float dx;
            float dy;

            var g = _drawForm.CreateGraphics();
            try
            {
                dx = g.DpiX;
                dy = g.DpiY;
            }
            catch
            {
                dx = 0.0f;
                dy = 0.0f;
            }
            finally
            {
                g.Dispose();
            }

            _minDistanceFromEdgeX = (dx == 0.0f) ? 10 : (int)(dx * MinDistanceFromEdgeInches);
            _minDistanceFromEdgeY = (dy == 0.0f) ? 10 : (int)(dy * MinDistanceFromEdgeInches);

            Log.Debug("minDistanceFromEdgeX : " + _minDistanceFromEdgeX + ", minDistanceFromEdgeY: " + _minDistanceFromEdgeY);
        }

        /// <summary>
        /// Closes the form
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
        /// Creates the invisible form on which the radar
        /// line is drawn
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
        /// Creates the pen to draw the radar
        /// </summary>
        /// <param name="color">color of the pen</param>
        /// <param name="penWidth">width of the line</param>
        private void createPen(Color color, int penWidth = DefaultPenWidth)
        {
            _pen = new Pen(color, penWidth);
        }

        /// <summary>
        /// Form paint handler. Draws the radar line
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="paintEventArgs">event args</param>
        private void formPaint(object sender, PaintEventArgs paintEventArgs)
        {
            if (_currentState == MouseMoverStates.Radar)
            {
                paintRadar(paintEventArgs);
            }
        }

        /// <summary>
        /// Handles the case where the radar has stopped and before
        /// the mouse begins to move along the radar line. Calculates
        /// the Bresenham line
        /// </summary>
        private void handleStatePreRadarMouseMove()
        {
            _currentMouseCycle = 0;
            _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;

            _timerMillisecondsPerStep = (((MaxRadialSpeed + 1) - RadialSpeed) * MsecsPerSec * RadialSpeedMultiplier) / _boundWidth;
            _radialPoints = Bresenham.GetBresenhamLine(_initialPoint, _finalPoint);
            Log.Debug("initialPoint: " + _initialPoint.X + ", " +
                        _initialPoint.Y + ", final Point: " +
                        _finalPoint.X + ", " + _finalPoint.Y);
            _currentRadialPoint = 0;

            int last = _radialPoints.Count - 1;
            _radialIndexMax = 0;

            for (var ii = last; ii >= 0; ii--)
            {
                if (Screen.PrimaryScreen.Bounds.Contains(_radialPoints[ii]))
                {
                    _radialIndexMax = ii;
                    Log.Debug("RadialIndexMax " + _radialIndexMax);
                    break;
                }
            }

            setStateAndNotify(MouseMoverStates.RadarMouseMove);
            _drawForm.Invalidate();
        }

        /// <summary>
        /// Increment/decrement the angle of the radar line depending
        /// on the direction
        /// </summary>
        private void handleStateRadar()
        {
            double elapsedMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds - _whenTimerStarted;
            if (!(elapsedMilliseconds > _timerMillisecondsPerStep))
            {
                return;
            }

            if (_radarDirection == Clockwise)
            {
                incrementClockwiseRadarAngle();
            }
            else
            {
                incrementCounterClockwiseRadarAngle();
            }

            ////Log.Debug("After: currentAngle " + currentAngle + ", cumulativeAngle: " + cumulativeAngle);
            if (_cumulativeAngle >= 360 * RotatingSweeps)
            {
                _drawForm.Invalidate();
                timerStop();
                setStateAndNotify(MouseMoverStates.Idle);
                return;
            }

            _drawForm.Invalidate();
            _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        /// <summary>
        /// Moves the mouse along the radar line using the
        /// Bresenham line.  Moves the mouse one step along the line
        /// </summary>
        private void handleStateRadarMouseMove()
        {
            double elapsedMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds - _whenTimerStarted;

            if (elapsedMilliseconds > _timerMillisecondsPerStep)
            {
                _currentRadialPoint += MouseStep;
                _whenTimerStarted = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }

            if (_currentMouseCycle == RadialSweeps)
            {
                timerStop();
                setStateAndNotify(MouseMoverStates.Idle);
                return;
            }

            Log.Debug("currentRadialPoint: " + _currentRadialPoint);
            if (_currentRadialPoint >= _radialIndexMax)
            {
                _currentRadialPoint = 0;
                moveCursorTo(_radialPoints[_currentRadialPoint]);
                _currentMouseCycle++;
            }
            else
            {
                moveCursorTo(_radialPoints[_currentRadialPoint]);
            }
        }

        /// <summary>
        /// Increments the radar angle for clockwise movement.
        /// There are a number of corner cases as the comments
        /// indicate.  We have to make sure that the radar line
        /// does not draw outside the bounds of the display.
        /// </summary>
        private void incrementClockwiseRadarAngle()
        {
            if (_initialPoint.Y < _minDistanceFromEdgeY &&
                    Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    _currentAngle > 180)
            {
                // top right corner
                _currentAngle = 90;
                _cumulativeAngle += 180;
            }
            else if (Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    Math.Abs(_initialPoint.Y - Screen.PrimaryScreen.Bounds.Height) < _minDistanceFromEdgeY &&
                    _currentAngle > 270)
            {
                // bottom right corner
                _currentAngle = 180;
                _cumulativeAngle += 270;
            }
            else if (Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    _currentAngle > 270)
            {
                // right edge
                _currentAngle = 90;
                _cumulativeAngle += 90;
            }
            else if (Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    Math.Abs(_initialPoint.Y - Screen.PrimaryScreen.Bounds.Height) < _minDistanceFromEdgeY &&
                    (_currentAngle < 180 || _currentAngle >= 270))
            {
                // bottom right corner
                _currentAngle = 180;
                _cumulativeAngle += 180;
            }
            else if (Math.Abs(_initialPoint.Y - Screen.PrimaryScreen.Bounds.Height) < _minDistanceFromEdgeY &&
                    _currentAngle < 180)
            {
                // bottom edge
                _currentAngle = 180;
                _cumulativeAngle += 180;
            }
            else if (_initialPoint.Y < _minDistanceFromEdgeY && _currentAngle > 180 &&
                    _currentAngle < 360)
            {
                // top edge
                _currentAngle = 0;
                _cumulativeAngle += 180;
            }
            else if (_initialPoint.X < _minDistanceFromEdgeX &&
                    Math.Abs(_initialPoint.Y - Screen.PrimaryScreen.Bounds.Height) < _minDistanceFromEdgeY &&
                        _currentAngle < 180)
            {
                // bottom left corner
                _currentAngle = 0;
                _cumulativeAngle += 270;
            }
            else if (_initialPoint.X < _minDistanceFromEdgeX && _currentAngle > 90 && _currentAngle < 270)
            {
                // left edge
                _currentAngle = 270;
                _cumulativeAngle += 180;
            }
            else
            {
                _currentAngle = (_currentAngle + _rotationStep) % 360;
                _cumulativeAngle += _rotationStep;
            }
        }

        /// <summary>
        /// Increments the radar angle for counter-clockwise movement.
        /// There are a number of corner cases as the comments
        /// indicate.  We have to make sure that the radar line
        /// does not draw outside the bounds of the display.
        /// </summary>
        private void incrementCounterClockwiseRadarAngle()
        {
            if (_initialPoint.X < _minDistanceFromEdgeX &&
                _initialPoint.Y < _minDistanceFromEdgeY && _currentAngle >= 90 &&
                _currentAngle < 270)
            {
                // top left corner
                _currentAngle = 270;
                _cumulativeAngle += 180;
            }
            else if (_initialPoint.Y < _minDistanceFromEdgeY &&
                     Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    _currentAngle < 180)
            {
                // top right corner
                _currentAngle = 180;
                _cumulativeAngle += 180;
            }
            else if (Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    _currentAngle < 90)
            {
                // right edge
                _currentAngle = 90;
                _cumulativeAngle += 90;
            }
            else if (Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    _currentAngle >= 270)
            {
                // right edge
                _currentAngle = 90;
                _cumulativeAngle += 180;
            }
            else if (Math.Abs(_initialPoint.X - Screen.PrimaryScreen.Bounds.Right) < _minDistanceFromEdgeX &&
                    Math.Abs(_initialPoint.Y - Screen.PrimaryScreen.Bounds.Height) < _minDistanceFromEdgeY &&
                    _currentAngle > 180)
            {
                // bottom right corner
                _currentAngle = 90;
                _cumulativeAngle += 270;
            }
            else if (Math.Abs(_initialPoint.Y - Screen.PrimaryScreen.Bounds.Height) < _minDistanceFromEdgeY &&
                    _currentAngle > 180)
            {
                // bottom edge
                _currentAngle = 0;
                _cumulativeAngle += 180;
            }
            else if (_initialPoint.Y < _minDistanceFromEdgeY && _currentAngle > 0 &&
                    _currentAngle < 180)
            {
                // top edge
                _currentAngle = 180;
                _cumulativeAngle += 180;
            }
            else if (_initialPoint.X < _minDistanceFromEdgeX &&
                                Math.Abs(_initialPoint.Y - Screen.PrimaryScreen.Bounds.Height) < _minDistanceFromEdgeY
                                && _currentAngle > 90)
            {
                // bottom left corner
                _currentAngle = 0;
                _cumulativeAngle += 270;
            }
            else if (_initialPoint.X < _minDistanceFromEdgeX && _currentAngle > 90 && _currentAngle < 270)
            {
                // left edge
                _currentAngle = 270;
                _cumulativeAngle += 180;
            }
            else
            {
                _currentAngle = (_currentAngle + _rotationStep) % 360;
                _cumulativeAngle += _rotationStep;
            }
        }

        /// <summary>
        /// Sets the initial and final points of the radar line
        /// </summary>
        private void initRadarLine()
        {
            createPen(_penColor, LineWidth);

            _finalPoint = new Point();
            _initialPoint = new Point(_boundWidth / 2, _boundHeight / 2);
        }

        /// <summary>
        /// Timer tick. Handled appropriately depending on the current state
        ///  of the radar mouse mover
        /// </summary>
        /// <param name="param">optional parameter</param>
        /// <param name="status">optional parameter</param>
        private void mmTimer_Tick(IntPtr param, bool status)
        {
            if (_drawForm == null || _drawFormClosing)
            {
                return;
            }

            try
            {
                _drawForm.Invoke(new MethodInvoker(delegate
                {
                    switch (_currentState)
                    {
                        case MouseMoverStates.Idle:
                            break;

                        case MouseMoverStates.Radar:
                            handleStateRadar();
                            break;

                        case MouseMoverStates.PreRadarMouseMove:
                            handleStatePreRadarMouseMove();
                            break;

                        case MouseMoverStates.RadarMouseMove:
                            handleStateRadarMouseMove();
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
        /// Moves the cursor to the specified point
        /// </summary>
        /// <param name="destinationPoint">where to move?</param>
        /// <returns>Postion after moving</returns>
        private Point moveCursorTo(Point destinationPoint)
        {
            Cursor.Position = new Point(Convert.ToInt32(destinationPoint.X), Convert.ToInt32(destinationPoint.Y));
            return Cursor.Position;
        }

        /// <summary>
        /// Draws the radar line
        /// </summary>
        /// <param name="paintEventArgs">paint event args</param>
        private void paintRadar(PaintEventArgs paintEventArgs)
        {
            float radarAux = (float)(Math.PI / 180.0f) * (float)_currentAngle * (float)_radarDirection;
            _finalPoint.X = _initialPoint.X + Convert.ToInt32((float)_radius * Math.Cos(radarAux));
            _finalPoint.Y = _initialPoint.Y + Convert.ToInt32((float)_radius * Math.Sin(radarAux));

            paintEventArgs.Graphics.DrawLine(_pen, _initialPoint, _finalPoint);

            //Log.Debug("radarCurrentAngle=" + radarCurrentAngle + " radarDirection=" + radarDirection);
            //Log.Debug("radarAux=" + radarAux + " radarFinalPoint.X=" + radarFinalPoint.X + " radarFinalPoint.Y=" + radarFinalPoint.Y);
            //Log.Debug(" radarInitialPoint.X=" + radarInitialPoint.X + " radarInitialPoint.Y=" + radarInitialPoint.Y);
            //Log.Debug(" radarRotationStep=" + radarRotationStep + " radarCurrentAngle=" + radarCurrentAngle);
        }

        /// <summary>
        /// Sets the mouse mover state and raises event
        /// to notify
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
        /// stops the timer
        /// </summary>
        private void timerStop()
        {
            Log.Debug();
            _mouseMoverTimer.Stop();
        }
    }
}