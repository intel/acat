///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense_factory.hpp" company="Intel Corporation">
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

#include "libivcp/irealsense_factory.hpp"

#ifndef __REALSENSE_CREATOR_HPP__
#define __REALSENSE_CREATOR_HPP__

/*!
 * \defgroup libinfra IVCP Infrastructure Library
 * \brief IVCP infrastructure library
 * @{
 */

/*!
 * \file realsense_factory.hpp
 * \brief Declaration of RealSense factory implementation
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief RealSense factory implementation
 */
class IVCP_EXPORT RealSenseFactory: public IRealSenseFactory
{
public:
    /*!
     * \brief Get RealSense handler
     * \param[in] stabThreshold Movement threshold to start change regions on face movement
     * \param[in] stabFrames Frame number to check for face movement during face stabilization
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true RealSense improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \return RealSense handler
     * \throw IncorrectParamException RealSense is not found
     * \throw IOException RealSense internal error
     */
    virtual SPtr<IRealSense> getRealSense(float stabThreshold = 0.0,
        unsigned int stabFrames = 10,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false);

protected:

private:
};
}
/** @}*/
#endif
