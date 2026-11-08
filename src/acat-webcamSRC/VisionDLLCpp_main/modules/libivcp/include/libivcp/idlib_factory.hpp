///////////////////////////////////////////////////////////////////////////
// <copyright file="idlib_factory.hpp" company="Intel Corporation">
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


#include <string>
#include "libivcp/sptr.hpp"
#include "libivcp/idlib.hpp"

#ifndef __IDLIB_FACTORY_HPP__
#define __IDLIB_FACTORY_HPP__

/*!
 * \defgroup libivcp IVCP Filters Library
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file idlib_factory.hpp
 * \brief Declaration of Dlib factory interface
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Dlib factory interface
 * \details Dlib factory creates and gives Dlib face detector handler
 */
class IVCP_EXPORT IDlibFactory
{
public:
    /*!
     * \brief Dlib factory destructor
     */
    virtual ~IDlibFactory() {};

    /*!
     * \brief Get Dlib handler
     * \param[in] stabThreshold Movement threshold to start change regions on face movement
     * \param[in] stabFrames Frame number to check for face movement during face stabilization
     * \param[in] normFrames Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames).
     * In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed
     * \param[in] noFaceFrames Frequency of applying the "no face" Dlib workaround
     * \param[in] noFaceClockwiseRotation If true in the "no face" workaround we rotate the image clockwise, otherwise we rotate counter-clockwise
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true Dlib improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \param[in] resizeDownscale Downscale for every frame image to decrease face detection time
     * \return Dlib handler
     * \throw IncorrectParamException Dlib is not found
     * \throw IOException Dlib internal error
     */
    virtual SPtr<IDlib> getDlib(float stabThreshold = 0.0,
        unsigned int stabFrames = 10,
        int normFrames = -1,
        int noFaceFrames = -1,
        bool noFaceClockwiseRotation = false,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false,
        double resizeDownscale = 1.0) = 0;

protected:

private:
};
}
/** @}*/
#endif

