///////////////////////////////////////////////////////////////////////////
// <copyright file="face_template.cpp" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

#include "libinfra/face_template.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
map<RegionType, pair<Point2i, Point2i> > getTemplateMask(RegionCreatorType type)
{
    map<RegionType, pair<Point2i, Point2i> > templateMask;

    switch (type)
    {
    case BBOX:
        break;
    case GROUNDTRUTH:
        break;
    case NEW_REGION_CREATOR:
        break;
    case DLIB_REGION:
    case RS_REGION:
        templateMask[REGION_REYEBROW] = pair<Point2i, Point2i>(Point2i(65, 25), Point2i(225, 70));
        templateMask[REGION_LEYEBROW] = pair<Point2i, Point2i>(Point2i(278, 25), Point2i(428, 70));
        templateMask[REGION_LEYE] = pair<Point2i, Point2i>(Point2i(300, 120), Point2i(396, 150));
        templateMask[REGION_REYE] = pair<Point2i, Point2i>(Point2i(95, 120), Point2i(191, 150));
        templateMask[REGION_LCHEEK] = pair<Point2i, Point2i>(Point2i(313, 230), Point2i(446, 285));
        templateMask[REGION_RCHEEK] = pair<Point2i, Point2i>(Point2i(55, 230), Point2i(195, 285));
        templateMask[REGION_FACE] = pair<Point2i, Point2i>(Point2i(40, -40), Point2i(457, 470));
        templateMask[REGION_NOSE] = pair<Point2i, Point2i>(Point2i(192, 175), Point2i(315, 300));
        templateMask[REGION_MOUTH] = pair<Point2i, Point2i>(Point2i(145, 310), Point2i(360, 380));
        break;
    default:
        break;
    }

    return templateMask;
}

map<int, Point2i> getCanonicalLandmark()
{
    map<int, Point2i> canonLandmark;

    canonLandmark[0] = Point2i(98, 140);
    canonLandmark[1] = Point2i(183, 125);
    canonLandmark[2] = Point2i(302, 132);
    canonLandmark[3] = Point2i(390, 134);
    canonLandmark[4] = Point2i(176, 336);
    canonLandmark[5] = Point2i(350, 345);
    canonLandmark[6] = Point2i(221, 255);
    canonLandmark[7] = Point2i(273, 255);
    canonLandmark[8] = Point2i(245, 232);
    canonLandmark[9] = Point2i(250, 275);

    return canonLandmark;
}

map<int, Point2d> getTemplateGlasses()
{
    map<int, Point2d> templateGlasses;
    
    templateGlasses[1] = Point2i(290, 311);
    templateGlasses[2] = Point2i(382, 311);
    templateGlasses[3] = Point2i(508, 311);
    templateGlasses[4] = Point2i(601, 311);
    templateGlasses[5] = Point2i(419, 338);
    templateGlasses[6] = Point2i(471, 338);
    templateGlasses[7] = Point2i(400, 390);
    templateGlasses[8] = Point2i(375, 418);
    templateGlasses[9] = Point2i(480, 390);
    templateGlasses[10] = Point2i(497, 418);
    templateGlasses[11] = Point2i(300, 440);
    templateGlasses[12] = Point2i(360, 440);
    templateGlasses[13] = Point2i(535, 440);
    templateGlasses[14] = Point2i(595, 440);

    return templateGlasses; 
}

pair<Point2i, Point2i> getTemplateEyePos(RegionCreatorType type)
{
    pair<Point2i, Point2i> templateEyePos;

    switch (type)
    {
    case ivcp::BBOX:
        break;
    case ivcp::GROUNDTRUTH:
        break;
    case ivcp::NEW_REGION_CREATOR:
        break;
    case ivcp::DLIB_REGION:
        templateEyePos = pair<Point2i, Point2i> (Point2i(335, 360), Point2i(545, 360));
        break;
    case ivcp::RS_REGION:
        templateEyePos = pair<Point2i, Point2i> (Point2i(335, 350), Point2i(545, 350));
        break;
    default:
        break;
    }

    return templateEyePos;
}

}