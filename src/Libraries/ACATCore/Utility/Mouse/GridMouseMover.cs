////////////////////////////////////////////////////////////////////////////
// <copyright file="GridMouseMover.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Handles mouse scanning across the display. Methods in
    /// this class can be used to move the mouse cursor to a specific
    /// point on the display.
    /// </summary>
    public class GridMouseMover
    {
        /// <summary>
        /// The window that animates grid scanning on the display
        /// </summary>
        private MouseGridScanWindow _window;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public GridMouseMover()
        {
            init();
        }

        /// <summary>
        /// Which direction to scan? Top down or bottom up
        /// </summary>
        public enum Direction
        {
            Down,
            Up
        }

        /// <summary>
        /// Get or sets whether vertical rectangle should be display to
        /// scan the display horizontally
        /// </summary>
        public bool EnableVerticalGridRectangle { get; set; }

        /// <summary>
        /// Get or sets how many times should the grid line scan?
        /// </summary>
        public int GridLineCycles { get; set; }

        /// <summary>
        /// Get or sets speed of movement of the grid line (1 to 500)
        /// </summary>
        public double GridLineSpeed { get; set; }

        /// <summary>
        /// Gets/sets thickness of the grid line
        /// </summary>
        public int GridLineThickness { get; set; }

        /// <summary>
        /// Gets/sets how many times should the rectangle scan the display?
        /// </summary>
        public int GridRectangleCycles { get; set; }

        /// <summary>
        /// Gets or sets which direction to scan?
        /// </summary>
        public Direction GridRectangleDirection { get; set; }

        /// <summary>
        /// Gets or sets the height of the mouse grid rectangle
        /// </summary>
        public double GridRectangleHeight { get; set; }

        /// <summary>
        /// Gets/sets speed of scanning of rectangle (1 to 500)
        /// </summary>
        public double GridRectangleSpeed { get; set; }

        /// <summary>
        /// Call this when the user activates the switch trigger
        /// </summary>
        public void Actuate()
        {
            if (_window != null)
            {
                _window.Actuate();
            }
        }

        /// <summary>
        /// Starts mouse grid scanning
        /// </summary>
        /// <returns>true on success</returns>
        public bool Start()
        {
            _window = new MouseGridScanWindow
            {
                EnableVerticalGridRectangle = EnableVerticalGridRectangle,
                GridLineCycles = GridLineCycles,
                GridLineSpeed = GridLineSpeed,
                GridLineThickness = GridLineThickness,
                GridRectangleCycles = GridRectangleCycles,
                GridRectangleDirection = GridRectangleDirection,
                GridRectangleSpeed = GridRectangleSpeed,
                GridRectangleHeight = GridRectangleHeight
            };

            _window.ShowDialog();

            _window = null;
            return true;
        }

        /// <summary>
        /// Initializes state variables to their default values
        /// </summary>
        private void init()
        {
            EnableVerticalGridRectangle = true;
            GridLineCycles = 2;
            GridLineSpeed = 20;
            GridLineThickness = 2;
            GridRectangleCycles = 2;
            GridRectangleDirection = Direction.Down;
            GridRectangleSpeed = 40;
            GridRectangleHeight = 120;
        }
    }
}