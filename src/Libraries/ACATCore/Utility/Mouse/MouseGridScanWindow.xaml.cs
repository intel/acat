////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

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
        /// Bitmap representation of the cursor pointer
        /// </summary>
        private Image _cursorImage;

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
            MoveMouse,
            Done
        }

        /// <summary>
        /// Gets or sets whether the vertical grid rectangle should be
        /// enabled or not.  If disabled, the mouse cursor is animated
        /// to lock the X coordinate.
        ///
        /// </summary>
        public bool EnableVerticalGridRectangle { get; set; }

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

            moveHorizontalLine(point);
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
        /// Creates and moves vertical line horizontally within the
        /// vertical rectangle
        /// </summary>
        /// <param name="point">Origin of the rectangle</param>
        private void animateLineVertical(Point point)
        {
            _lineVertAnimation = new DoubleAnimation
            {
                From = point.X,
                To = point.X + _rectVert.Width
            };

            _lineVertAnimation.Completed += animationOnCompleted;

            _lineVert = new Line
            {
                Y1 = 0,
                Y2 = ((Canvas)Content).RenderSize.Height,
                X1 = 0,
                X2 = 0,
                StrokeThickness = GridLineThickness,
                Stroke = new SolidColorBrush(GridLineColor)
            };
            addToCanvas(_lineVert);

            _lineVertAnimation.Duration = new Duration(TimeSpan.FromSeconds(_durationLine));
            _lineVertAnimation.AutoReverse = true;
            _lineVertAnimation.RepeatBehavior = _lineRepeatBehavior;
            Storyboard.SetTarget(_lineVertAnimation, _lineVert);
            Storyboard.SetTargetProperty(_lineVertAnimation, new PropertyPath(Canvas.LeftProperty));
            _lineVertStoryboard = new Storyboard();
            _lineVertStoryboard.Children.Add(_lineVertAnimation);
            _lineVertStoryboard.Begin(this, true);
        }

        /// <summary>
        /// Animates the movmement of the mouse cursor along a
        /// horizontal line
        /// </summary>
        private void animateMouseMove()
        {
            _rectVertStoryboard.Pause(this);
            _rectVert.Opacity = 1.0;
            _rectVert.Stroke = new SolidColorBrush(GridRectanglePausedBorderColor);
            _rectVert.Fill = new SolidColorBrush();
#if LineVertical
            pt = RectVert.PointToScreen(new Point(0, 0));
            animateLineVertical(pt);
#else
            var rectPosition = _rectVert.PointToScreen(new Point(0, 0));
            var from = new Point(rectPosition.X, _lineHorizPausePoint.Y);
            var to = new Point(rectPosition.X + GridRectangleHeight, _lineHorizPausePoint.Y);
            moveImage(_cursorImage, from, to);
#endif
        }

        /// <summary>
        /// Simulates final movement of the cursor to the desired
        /// location.
        /// </summary>
        private void animateSetCursorPos()
        {
            if (EnableVerticalGridRectangle)
            {
                removeFromCanvas(_rectVert);
#if LineVertical
                lineVertStoryboard.Pause(this);
#endif
            }

            _finalCursorPos = _cursorImage.PointToScreen(new Point(0, 0));

            moveImageFinal(_cursorImage, _finalCursorPos);
        }

        /// <summary>
        /// Animates movement of the vertical rectangle horizontally
        /// across the display
        /// </summary>
        private void animateVerticalRectangle()
        {
            _lineHorizStoryboard.Pause(this);

            removeFromCanvas(_rectHoriz);

            if (EnableVerticalGridRectangle)
            {
                moveVerticalRectangle();
                _lineHorizPausePoint = _lineHoriz.PointToScreen(new Point(0, 0));
            }
            else
            {
                Point point = _lineHoriz.PointToScreen(new Point(0, 0));
                moveImageAcrossDisplay(_cursorImage, point.Y);
            }
        }

        /// <summary>
        /// Callback function to indicate animation completed without the
        /// user triggering the switch.  Close the form.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void animationOnCompleted(object sender, EventArgs eventArgs)
        {
            Mouse.OverrideCursor = null;

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
        /// Converts a System.Drawing.Image image object to a WPF
        /// image
        /// </summary>
        /// <param name="gdiImage">source image</param>
        /// <returns>WPF image</returns>
        private Image convertToWPFImage(System.Drawing.Image gdiImage)
        {
            var image = new Image();
            var bitmap = new Bitmap(gdiImage);

            IntPtr hBitmap = bitmap.GetHbitmap();
            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                                                                    IntPtr.Zero,
                                                                    Int32Rect.Empty,
                                                                    BitmapSizeOptions.FromEmptyOptions());

            image.Source = bitmapSource;
            image.Width = bitmapSource.Width;
            image.Height = bitmapSource.Height;
            image.Stretch = Stretch.Fill;
            return image;
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
                    _currentState = (EnableVerticalGridRectangle) ? States.VerticalRect : States.MoveMouse;
                    break;

                case States.VerticalRect:
                    animateMouseMove();
                    _currentState = States.MoveMouse;
                    break;

                case States.MoveMouse:
                    animateSetCursorPos();
                    _currentState = States.Done;
                    break;
            }
        }

        /// <summary>
        /// Returns the current mouse pointer position
        /// </summary>
        /// <returns>x,y location of mouse pointer</returns>
        private Point getCursorPos()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
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
            EnableVerticalGridRectangle = true;
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

            _cursorImage = loadBitmap("ACAT.Lib.Core.MouseCursor.png");
        }

        /// <summary>
        /// Returns a bitmap of the resource specified
        /// </summary>
        /// <param name="imageName">filename of the bitmap</param>
        /// <returns>bitmap image object</returns>
        private Image loadBitmap(string imageName)
        {
            try
            {
                var myAssembly = Assembly.GetExecutingAssembly();
                var myStream = myAssembly.GetManifestResourceStream(imageName);
                if (myStream != null)
                {
                    var image = new Bitmap(myStream);
                    return convertToWPFImage(image);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return new Image();
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
        /// Animates the horizontal line vertically within the
        /// horiz rectangle
        /// </summary>
        /// <param name="point">origin of the horiz rectange</param>
        private void moveHorizontalLine(Point point)
        {
            _lineHorizAnimation = new DoubleAnimation
            {
                From = (GridRectangleDirection == GridMouseMover.Direction.Down) ? point.Y : point.Y + _rectHoriz.Height,
                To = (GridRectangleDirection == GridMouseMover.Direction.Down) ? point.Y + _rectHoriz.Height : point.Y
            };

            _lineHorizAnimation.Completed += animationOnCompleted;

            _lineHoriz = new Line
            {
                X1 = 0,
                X2 = ((Canvas)Content).RenderSize.Width,
                Y1 = 0,
                Y2 = 0,
                StrokeThickness = GridLineThickness,
                Stroke = new SolidColorBrush(GridLineColor)
            };
            addToCanvas(_lineHoriz);

            _lineHorizAnimation.Duration = new Duration(TimeSpan.FromSeconds(_durationLine));
            _lineHorizAnimation.RepeatBehavior = _lineRepeatBehavior;
            Storyboard.SetTarget(_lineHorizAnimation, _lineHoriz);
            Storyboard.SetTargetProperty(_lineHorizAnimation, new PropertyPath(Canvas.TopProperty));
            _lineHorizStoryboard = new Storyboard();
            _lineHorizStoryboard.Children.Add(_lineHorizAnimation);

            _lineHorizStoryboard.Begin(this, true);
        }

        /// <summary>
        /// Moves the specified image from the 'from' location to
        /// the 'to' location
        /// </summary>
        /// <param name="image">image object</param>
        /// <param name="from">starting position</param>
        /// <param name="to">final position</param>
        private void moveImage(Image image, Point from, Point to)
        {
            removeFromCanvas(_lineHoriz);
            addToCanvas(image);

            var trans = new TranslateTransform();
            image.RenderTransform = trans;
            var doubleAnimation1 = new DoubleAnimation(from.X, to.X, TimeSpan.FromSeconds(_durationLine));
            var doubleAnimation2 = new DoubleAnimation(to.Y, to.Y, TimeSpan.FromSeconds(_durationLine));
            doubleAnimation1.Completed += animationOnCompleted;
            Mouse.OverrideCursor = Cursors.None;
            doubleAnimation1.RepeatBehavior = _lineRepeatBehavior;
            trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimation1);
            trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimation2);
        }

        /// <summary>
        /// Moves image along a horizontal line at the specifed Y offset
        /// </summary>
        /// <param name="image">image to move</param>
        /// <param name="yOffset">Y offset of the line</param>
        private void moveImageAcrossDisplay(Image image, double yOffset)
        {
            removeFromCanvas(_lineHoriz);
            addToCanvas(image);

            var translateTransform = new TranslateTransform();
            _cursorImage.RenderTransform = translateTransform;
            var doubleAnimation1 = new DoubleAnimation(0, SystemParameters.PrimaryScreenWidth, TimeSpan.FromSeconds(_durationVert));
            var doubleAnimation2 = new DoubleAnimation(yOffset, yOffset, TimeSpan.FromSeconds(_durationVert));
            doubleAnimation1.Completed += animationOnCompleted;
            Mouse.OverrideCursor = Cursors.None;
            doubleAnimation1.RepeatBehavior = _lineRepeatBehavior;
            translateTransform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation1);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, doubleAnimation2);
        }

        /// <summary>
        /// Simulates final movement of the cursor
        /// </summary>
        /// <param name="image">cursor image</param>
        /// <param name="to">final position</param>
        private void moveImageFinal(Image image, Point to)
        {
            var trans = new TranslateTransform();
            image.RenderTransform = trans;

            var from = new Point(to.X + 100, to.Y + 100);
            var doubleAnimation1 = new DoubleAnimation(from.X, to.X, TimeSpan.FromSeconds(0.25));
            var doubleAnimation2 = new DoubleAnimation(from.Y, to.Y, TimeSpan.FromSeconds(0.25));

            doubleAnimation1.Completed += moveImageFinalCompleted;

            trans.BeginAnimation(TranslateTransform.XProperty, doubleAnimation1);
            trans.BeginAnimation(TranslateTransform.YProperty, doubleAnimation2);
        }

        /// <summary>
        /// Final movement of the cursor completed.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void moveImageFinalCompleted(object sender, EventArgs eventArgs)
        {
            SetCursorPos((int)_finalCursorPos.X, (int)_finalCursorPos.Y);
            animationOnCompleted(sender, eventArgs);
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