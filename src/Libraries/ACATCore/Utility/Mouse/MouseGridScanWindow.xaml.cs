////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
//using System.Drawing;
//using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using static ACAT.Lib.Core.Utility.GridMouseMover;
//using static ACAT.Lib.Core.Utility.User32Interop;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Handles all animations relative to scanning the display to
    /// enable the user to position the mouse cursor at a specific
    /// x,y location on the display.  The display is first scanned
    /// vertically to enable the user to select the Y coordinate.
    /// then the display is scanned horizontally to enable the user
    /// to select the X coordinate. Finally, the mouse cursor is moved
    /// to the X,Y location.
    /// </summary>
    public partial class MouseGridScanWindow
    {
        /// <summary>
        /// Used for the state machine
        /// </summary>
        private States _currentState = States.Begin;

        /// <summary>
        /// Time period to scan horizontal rectangle vertically
        /// </summary>
        private double _durationHoriz;

        /// <summary>
        /// Time period to scan the grid line within the rectangle
        /// </summary>
        private double _durationLine;

        /// <summary>
        /// Time period to scan the vertical rectangle horizontally
        /// </summary>
        private double _durationVert;

        /// <summary>
        /// Final X,Y position of the cursor
        /// </summary>
        private Point _finalCursorPos;

        /// <summary>
        /// How many times to scan the grid line within the grid rectangle
        /// </summary>
        private int _gridLineCycles;

        private double _gridLineSpeed;

        /// <summary>
        /// How many times to scan the grid rectangle actoss the display
        /// </summary>
        private int _gridRectangleCycles;

        /// <summary>
        /// Speed (0 to 500) of movememnt of the line
        /// </summary>
        private double _gridRectangleHeight;

        /// <summary>
        /// Speed (0 to 500) of movement of the rectangle
        /// </summary>
        private double _gridRectangleSpeed;

        /// <summary>
        /// The grid line object
        /// </summary>
        private Line _lineHoriz;

        /// <summary>
        /// Animates the horizontal line
        /// </summary>
        private DoubleAnimation _lineHorizAnimation;

        /// <summary>
        /// Where did the horizontal line pause? This is the Y
        /// coordinate of the final cursor pos
        /// </summary>
        private Point _lineHorizPausePoint;

        /// <summary>
        /// Storyboard for moving the horiz line
        /// </summary>
        private Storyboard _lineHorizStoryboard;

        /// <summary>
        /// WPF repeat behavior - controls the # of cycles
        /// </summary>
        private RepeatBehavior _lineRepeatBehavior;

        /// <summary>
        /// The vertical grid line object
        /// </summary>
        private Line _lineVert;

        /// <summary>
        /// Animates the vertical line horizontally across
        /// the display
        /// </summary>
        private DoubleAnimation _lineVertAnimation;

        /// <summary>
        /// Vertical line storyboard
        /// </summary>
        private Storyboard _lineVertStoryboard;

        /// <summary>
        /// Repeat behavior of the rectangle animation
        /// </summary>
        private RepeatBehavior _rectangleRepeatBehavior;

        /// <summary>
        /// The horizontal rectangle object
        /// </summary>
        private Rectangle _rectHoriz;

        /// <summary>
        /// Animation of the horiz rectangle
        /// </summary>
        private DoubleAnimation _rectHorizAnimation;

        /// <summary>
        /// Storyboard for the horiz rectangle animation
        /// </summary>
        private Storyboard _rectHorizStoryboard;

        /// <summary>
        /// The vertical grid rectangle
        /// </summary>
        private Rectangle _rectVert;

        /// <summary>
        /// Animates the vertical rectangle
        /// </summary>
        private DoubleAnimation _rectVertAnimation;

        /// <summary>
        /// Storyboard for vertical rectangle animation
        /// </summary>
        private Storyboard _rectVertStoryboard;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public MouseGridScanWindow()
        {
            InitializeComponent();

            init();

            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// The different states for the state machine
        /// </summary>
        private enum States
        {
            Begin,
            HorizontalRect,
            HorizontalLine,
            VerticalRect,
            VerticalLine,
            Done
        }

        /// <summary>
        /// Gets or sets the color of the grid line
        /// </summary>
        public Color GridLineColor { get; set; }

        /// <summary>
        /// Gets or sets how many times should the grid line scan?
        /// </summary>
        public int GridLineCycles
        {
            get { return _gridLineCycles; }

            set
            {
                _gridLineCycles = value;
                _lineRepeatBehavior = (_gridLineCycles <= 0)
                    ? RepeatBehavior.Forever
                    : new RepeatBehavior(_gridLineCycles);
            }
        }

        /// <summary>
        /// Gets or sets speed of movement of the grid line (1 to 500)
        /// </summary>
        public double GridLineSpeed
        {
            get { return _gridLineSpeed; }
            set
            {
                _gridLineSpeed = (value <= 0) ? 20 : value;
                calculateParams();
            }
        }

        /// <summary>
        /// Gets or sets thickness of the grid line
        /// </summary>
        public int GridLineThickness { get; set; }

        /// <summary>
        /// Gets or sets the border color of the grid rectangle while it
        /// is moving
        /// </summary>
        public Color GridRectangleBorderColor { get; set; }

        /// <summary>
        /// Gets or sets how many times should the rectangle scan the display?
        /// </summary>
        public int GridRectangleCycles
        {
            get { return _gridRectangleCycles; }

            set
            {
                _gridRectangleCycles = value;
                _rectangleRepeatBehavior = (_gridRectangleCycles <= 0)
                    ? RepeatBehavior.Forever
                    : new RepeatBehavior(_gridRectangleCycles);
            }
        }

        /// <summary>
        /// Gets or sets which direction to scan?
        /// </summary>
        public GridMouseMover.Direction GridRectangleDirection { get; set; }

        /// <summary>
        /// Gets or sets the fill color of the grid rectangle
        /// </summary>
        public Color GridRectangleFillColor { get; set; }

        /// <summary>
        /// Gets or sets the width of the grid rectangle.
        /// </summary>
        public double GridRectangleHeight
        {
            get { return _gridRectangleHeight; }
            set
            {
                _gridRectangleHeight = value;
                if (_gridRectangleHeight < 50)
                {
                    _gridRectangleHeight = 50;
                }

                calculateParams();
            }
        }

        /// <summary>
        /// Gets or sets teh rectangle border color when the rectangle
        /// is paused
        /// </summary>
        public Color GridRectanglePausedBorderColor { get; set; }

        /// <summary>
        /// Gets or sets speed of scanning of rectangle (1 to 500)
        /// </summary>
        public double GridRectangleSpeed
        {
            get
            {
                return _gridRectangleSpeed;
            }
            set
            {
                _gridRectangleSpeed = (value <= 0) ? 20 : value;
                calculateParams();
            }
        }

        /// <summary>
        /// Call this when the user triggers the switch. Switches to
        /// the next state in the grid scanning sequence
        /// </summary>
        public void Actuate()
        {
            transitionState();
        }

        /// <summary>
        /// Returns the current positon of the mouse pointer
        /// </summary>
        /// <param name="pt">the position</param>
        /// <returns>true on success</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        /// <summary>
        /// Sets the cursor pointer to the specified position
        /// </summary>
        /// <param name="X">x coordinate</param>
        /// <param name="Y">y coordinate</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        /// <summary>
        /// Adds the specified element to the canvas if not already added
        /// </summary>
        /// <param name="uiElement">object to add</param>
        private void addToCanvas(UIElement uiElement)
        {
            if (!MyCanvas.Children.Contains(uiElement))
            {
                MyCanvas.Children.Add(uiElement);
            }
        }

        private static System.Drawing.Point GetLineIntersectionOnScreen(Line horizLine, Line vertLine, Canvas canvas)
        {
            double x = (Canvas.GetLeft(vertLine));
            if (double.IsNaN(x)) x = 0;

            double y = Canvas.GetTop(horizLine);
            if (double.IsNaN(y)) y = 0;

            var screenPoint = canvas.PointToScreen(new Point(x, y));

            return new System.Drawing.Point((int)screenPoint.X, (int)screenPoint.Y);
        }

        /// <summary>
        /// Pauses horiz rectnagle animation.
        /// Moves the horizontal line within the horizontal rectangle.
        /// </summary>
        private void animateHorizontalLine()
        {
            var point = _rectHoriz.PointToScreen(new Point(0, 0));

            _rectHorizStoryboard.Pause(this);
            _rectHoriz.Opacity = 1.0;
            _rectHoriz.Stroke = new SolidColorBrush(GridRectanglePausedBorderColor);
            _rectHoriz.Fill = new SolidColorBrush();

            _lineHorizAnimation = new DoubleAnimation
            {
                From = (GridRectangleDirection == GridMouseMover.Direction.Down) ? point.Y : point.Y + _rectHoriz.Height,
                To = (GridRectangleDirection == GridMouseMover.Direction.Down) ? point.Y + _rectHoriz.Height : point.Y
            };

            MoveLine(Direction.Horizontal, point, _lineHorizAnimation);
        }

        private void animateVerticalLine()
        {
            var point = _rectVert.PointToScreen(new Point(0, 0));

            _rectVertStoryboard.Pause(this);
            _rectVert.Opacity = 1.0;
            _rectVert.Stroke = new SolidColorBrush(GridRectanglePausedBorderColor);
            _rectVert.Fill = new SolidColorBrush();

            _lineVertAnimation = new DoubleAnimation
            {
                From = point.X,
                To = point.X + _rectVert.Width
            };

            MoveLine(Direction.Vertical, point, _lineVertAnimation);

        }

        public enum Direction { Horizontal, Vertical }

        private void MoveLine(Direction direction, Point point, DoubleAnimation animation)
        {
            Line line = new Line
            {
                StrokeThickness = GridLineThickness,
                Stroke = new SolidColorBrush(GridLineColor)
            };

            Storyboard storyboard = new Storyboard();
            animation.Completed += animationOnCompleted;
            animation.Duration = new Duration(TimeSpan.FromSeconds(_durationLine));
            animation.AutoReverse = true;
            animation.RepeatBehavior = _lineRepeatBehavior;

            animation.Completed += animationOnCompleted;

            if (direction == Direction.Horizontal)
            {
                animation.From = (GridRectangleDirection == GridMouseMover.Direction.Down)
                    ? point.Y
                    : point.Y + _rectHoriz.Height;
                animation.To = (GridRectangleDirection == GridMouseMover.Direction.Down)
                    ? point.Y + _rectHoriz.Height
                    : point.Y;

                line.X1 = 0;
                line.X2 = ((Canvas)Content).RenderSize.Width;
                line.Y1 = 0;
                line.Y2 = 0;

                Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
                _lineHoriz = line;
                _lineHorizAnimation = animation;
                _lineHorizStoryboard = storyboard;
            }
            else // Vertical
            {
                animation.From = point.X;
                animation.To = point.X + _rectVert.Width;

                line.Y1 = 0;
                line.Y2 = ((Canvas)Content).RenderSize.Height;
                line.X1 = 0;
                line.X2 = 0;

                Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
                _lineVert = line;
                _lineVertAnimation = animation;
                _lineVertStoryboard = storyboard;
            }

            addToCanvas(line);
            Storyboard.SetTarget(animation, line);
            storyboard.Children.Add(animation);
            storyboard.Begin(this, true);
        }


        /// <summary>
        /// Creates and moves horiz rectangle vertically across the display
        /// </summary>
        private void animateHorizontalRectangle()
        {
            Topmost = false;
            Topmost = true;

            _rectHoriz = new Rectangle
            {
                Height = GridRectangleHeight,
                Width = ((Canvas)Content).RenderSize.Width,
                Stroke = new SolidColorBrush(GridRectangleBorderColor),
                Fill = new SolidColorBrush(GridRectangleFillColor),
                Opacity = 0.5
            };

            addToCanvas(_rectHoriz);

            _rectHorizAnimation = new DoubleAnimation
            {
                From = (GridRectangleDirection == GridMouseMover.Direction.Down) ?
                            0 : ((Canvas)Content).RenderSize.Height - _rectHoriz.Height,
                To = (GridRectangleDirection == GridMouseMover.Direction.Down) ?
                            ((Canvas)Content).RenderSize.Height - _rectHoriz.Height : 0,
                Duration = new Duration(TimeSpan.FromSeconds(_durationHoriz)),
                RepeatBehavior = _rectangleRepeatBehavior
            };

            _rectHorizAnimation.Completed += animationOnCompleted;
            Storyboard.SetTarget(_rectHorizAnimation, _rectHoriz);
            Storyboard.SetTargetProperty(_rectHorizAnimation, new PropertyPath(Canvas.TopProperty));
            _rectHorizStoryboard = new Storyboard();
            _rectHorizStoryboard.Children.Add(_rectHorizAnimation);
            _rectHorizStoryboard.Begin(this, true);
        }



        /// <summary>
        /// Simulates final movement of the cursor to the desired
        /// location.
        /// </summary>
        private void animateSetCursorPos()
        {
            _lineVertStoryboard.Pause(this);
            removeFromCanvas(_rectVert);

            double x = Canvas.GetLeft(_lineVert);
            if (double.IsNaN(x)) x = 0;
            double y = Canvas.GetTop(_lineHoriz);
            if (double.IsNaN(y)) y = 0;

            System.Windows.Point screen = MyCanvas.PointToScreen(new System.Windows.Point(x, y));
            System.Drawing.Point winFormsPoint = new System.Drawing.Point((int)screen.X, (int)screen.Y);

            Log.Debug($"Setting cursor position to {winFormsPoint}");

            System.Windows.Forms.Cursor.Position = winFormsPoint;

            executeTransitionState();
        }

        private void animateDone()
        {
            removeFromCanvas(_lineHoriz);
            removeFromCanvas (_lineVert);

            _lineHorizStoryboard.Stop(this);
            _lineVertStoryboard.Stop(this);
            _rectHorizStoryboard.Stop(this);
            _rectVertStoryboard.Stop(this);

            Close();
        }
        /// <summary>
        /// Animates movement of the vertical rectangle horizontally
        /// across the display
        /// </summary>
        private void animateVerticalRectangle()
        {
            _lineHorizStoryboard.Pause(this);

            removeFromCanvas(_rectHoriz);

            moveVerticalRectangle();
            _lineHorizPausePoint = _lineHoriz.PointToScreen(new Point(0, 0));
        }

        /// <summary>
        /// Callback function to indicate animation completed without the
        /// user triggering the switch.  Close the form.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void animationOnCompleted(object sender, EventArgs eventArgs)
        {
            Close();
        }

        /// <summary>
        /// Calculates dependent variables
        /// </summary>
        private void calculateParams()
        {
            _durationHoriz = SystemParameters.PrimaryScreenHeight / _gridRectangleSpeed;
            _durationVert = SystemParameters.PrimaryScreenWidth / _gridRectangleSpeed;
            _durationLine = _gridRectangleHeight / _gridLineSpeed;
        }


        /// <summary>
        /// Executes state transition
        /// </summary>
        private void executeTransitionState()
        {
            switch (_currentState)
            {
                case States.Begin:
                    animateHorizontalRectangle();
                    _currentState = States.HorizontalRect;
                    break;

                case States.HorizontalRect:
                    animateHorizontalLine();
                    _currentState = States.HorizontalLine;
                    break;

                case States.HorizontalLine:
                    animateVerticalRectangle();
                    _currentState = States.VerticalRect;
                    break;

                case States.VerticalRect:
                    animateVerticalLine();
                    _currentState = States.VerticalLine;
                    break;

                case States.VerticalLine:
                    animateSetCursorPos();
                    _currentState = States.Done;
                    break;

                case States.Done:
                    animateDone();
                    break;

                default:
                    throw new InvalidOperationException("Unexpected state in state machine.");
            }
        }

        /// <summary>
        /// Initializes class variables to their default values
        /// </summary>
        private void init()
        {
            GridLineCycles = 2;
            GridRectangleCycles = 2;
            GridRectangleHeight = 120;
            GridLineThickness = 2;
            GridRectangleDirection = GridMouseMover.Direction.Down;
            GridRectanglePausedBorderColor = Colors.Gray;
            GridRectangleBorderColor = Colors.Black;
            GridLineColor = Colors.DodgerBlue;
            GridRectangleFillColor = Colors.LightGray;
            GridRectangleSpeed = 60;
            GridLineSpeed = 40;
            ShowInTaskbar = false;

            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

        }

        /// <summary>
        /// Event handler for when the form is loaded
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Topmost = false;
            Topmost = true;

            transitionState();
        }

        /// <summary>
        /// Animates vertical rectangle movement horizontally across
        /// the display
        /// </summary>
        private void moveVerticalRectangle()
        {
            _rectVert = new Rectangle
            {
                Width = GridRectangleHeight,
                Height = 300,
                Stroke = new SolidColorBrush(GridRectangleBorderColor)
            };

            addToCanvas(_rectVert);

            _rectVert.Height = ((Canvas)this.Content).RenderSize.Height;
            _rectVert.Fill = new SolidColorBrush(GridRectangleFillColor);
            _rectVert.Opacity = 0.5;

            _rectVertAnimation = new DoubleAnimation
            {
                From = 0,
                To = ((Canvas)Content).RenderSize.Width - _rectVert.Width,
                Duration = new Duration(TimeSpan.FromSeconds(_durationVert)),
                RepeatBehavior = _rectangleRepeatBehavior
            };

            _rectVertAnimation.Completed += animationOnCompleted;

            Storyboard.SetTarget(_rectVertAnimation, _rectVert);
            Storyboard.SetTargetProperty(_rectVertAnimation, new PropertyPath(Canvas.LeftProperty));
            _rectVertStoryboard = new Storyboard();
            _rectVertStoryboard.Children.Add(_rectVertAnimation);
            _rectVertStoryboard.Begin(this, true);
        }

        /// <summary>
        /// Removes specified element from the canvas
        /// </summary>
        /// <param name="uiElement">element to remove</param>
        private void removeFromCanvas(UIElement uiElement)
        {
            if (MyCanvas.Children.Contains(uiElement))
            {
                MyCanvas.Children.Remove(uiElement);
            }
        }

        /// <summary>
        /// Transition state machine to the next state
        /// </summary>
        private void transitionState()
        {
            Dispatcher.BeginInvoke(new Action(executeTransitionState));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
    }
}