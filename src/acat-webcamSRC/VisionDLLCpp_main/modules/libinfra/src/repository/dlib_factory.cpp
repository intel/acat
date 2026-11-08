///////////////////////////////////////////////////////////////////////////
// <copyright file="dlib_factory.cpp" company="Intel Corporation">
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

#include "libinfra/repository/dlib_factory.hpp"
#include "libinfra/dlib.hpp"

using namespace std;

namespace ivcp
{
SPtr<IDlib> DlibFactory::getDlib(float stabThreshold, unsigned int stabFrames, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled, bool useTemplateRegions, 
                              float glassesFixStrength, bool useAutoGlassesSearch, bool cutBackground, double resizeDownscale)
{
    SPtr<IDlib> dlibHandler = NULL;
#ifdef USE_DLIB
    dlibHandler = new Dlib(stabThreshold, stabFrames, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled, useTemplateRegions, 
        glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);
#else
    (void) stabThreshold;
    (void) stabFrames;
    (void) normFrames;
    (void) noFaceFrames;
    (void) noFaceClockwiseRotation,
    (void) glassesFixEnabled;
    (void) useTemplateRegions;
    (void) glassesFixStrength;
    (void) useAutoGlassesSearch;
    (void) cutBackground;
    (void) resizeDownscale;
    IVCP_LOG_ERROR("The library is compiled without Dlib support, unable to create a Dlib handler");
#endif

    return dlibHandler;
}
}
