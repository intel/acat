///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense_factory.cpp" company="Intel Corporation">
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

#include "libinfra/repository/realsense_factory.hpp"
#include "libinfra/realsense.hpp"

using namespace std;

namespace ivcp
{
SPtr<IRealSense> RealSenseFactory::getRealSense(float stabThreshold, unsigned int stabFrames, bool glassesFixEnabled, bool useTemplateRegions,
                              float glassesFixStrength, bool useAutoGlassesSearch, bool cutBackground)
{
    SPtr<IRealSense> rsHandler = NULL;
#ifdef USE_REALSENSE
    rsHandler = new RealSense(stabThreshold, stabFrames, glassesFixEnabled, useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground);
#else
    (void) stabThreshold;
    (void) stabFrames;
    (void) glassesFixEnabled;
    (void) useTemplateRegions;
    (void) glassesFixStrength;
    (void) useAutoGlassesSearch;
    (void) cutBackground;
    IVCP_LOG_ERROR("The library is compiled without RealSense support, unable to create a RealSense handler");
#endif

    return rsHandler;
}
}
