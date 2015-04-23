////////////////////////////////////////////////////////////////////////////
// <copyright file="Bresenham.cs" company="Intel Corporation">
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

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
    public class Bresenham
    {
        /// <summary>
        /// A helper method used to observe operations in the debug output window
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point addPoint(Point point1, Point point2)
        {
            point1.X += point2.X;
            point1.Y += point2.Y;

            return point1;
        }

        public static List<Point> GetBresenhamLine(Point startPoint, Point endPoint)
        {
            IEnumerable<Point> originalLine; // Represent an enumerable line
            List<Point> translatedLine = new List<Point>(); // Represent an enumerable line

            int origX1 = startPoint.X;
            int origY1 = startPoint.Y;

            int origX2 = endPoint.X;
            int origY2 = endPoint.Y;

            // calculate how far the original point is away from the origin
            int originXDisplacement;
            int originYDisplacement;

            if (origX1 >= 0)
            {
                originXDisplacement = -origX1;
            }
            else
            {
                originXDisplacement = origX1;
            }

            if (origY1 >= 0)
            {
                originYDisplacement = -origY1;
            }
            else
            {
                originYDisplacement = origY1;
            }

            // use 0,0 as first point and then translate second point over
            int transX2 = origX2 + originXDisplacement;
            int transY2 = origY2 + originYDisplacement;

            // the bresenham algorithm seems to only work in cartesian quadrant I, so we
            // need to flip the negative axes over to do perform the point calculations
            // they will be flipped back afterwards
            const int NO_AXIS_FLIP = 1;
            const int AXIS_FLIP = -1;

            int flipXAxis;
            int flipYAxis;

            if (transX2 < 0)
            {
                transX2 = transX2 * AXIS_FLIP;
                flipXAxis = AXIS_FLIP;
            }
            else
            {
                flipXAxis = NO_AXIS_FLIP;
            }

            if (transY2 < 0)
            {
                transY2 = transY2 * AXIS_FLIP;
                flipYAxis = AXIS_FLIP;
            }
            else
            {
                flipYAxis = NO_AXIS_FLIP;
            }

            originalLine = BresenhamLine.RenderLine(new Point(0, 0), new Point(transX2, transY2));

            int newX;
            int newY;
            Point newPoint;

            foreach (Point myPoint in originalLine)
            {
                newX = (myPoint.X * flipXAxis) - originXDisplacement;
                newY = (myPoint.Y * flipYAxis) - originYDisplacement;

                newPoint = new Point(newX, newY);
                translatedLine.Add(newPoint);
            }

            Point translatedPoint;

            for (int x = 0; x < translatedLine.Count; x++)
            {
                translatedPoint = translatedLine[x];
            }

            return translatedLine;
        }

        /// <summary>
        /// This method iterates over an IEnumerable of points and displays them
        /// in the log.
        /// </summary>
        /// <param name="points"></param>
        private static void showPoints(IEnumerable<Point> points)
        {
            foreach (Point p in points)
            {
                Log.Debug(p.ToString() + " ");
            }
        }

        /// <summary>
        /// This function puts a BEGIN and END tag around a list of points from a line
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        private static void showPointsForLine(Point begin, Point end)
        {
            IEnumerable<Point> myLine = BresenhamLine.RenderLine(begin, end);
            Log.Debug("<<START>>: " + begin.ToString() + " *\t");
            showPoints(myLine);
            Log.Debug("\t* <<FINISH>>: " + end.ToString() + " *" + Environment.NewLine);
        }
    }
}