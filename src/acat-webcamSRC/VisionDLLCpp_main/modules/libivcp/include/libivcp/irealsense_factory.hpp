///////////////////////////////////////////////////////////////////////////
// <copyright file="irealsense_factory.hpp" company="Intel Corporation">
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
#include "libivcp/irealsense.hpp"

#ifndef __IREALSENSE_FACTORY_HPP__
#define __IREALSENSE_FACTORY_HPP__

/*!
 * \defgroup libivcp IVCP Filters Library
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file irealsense_factory.hpp
 * \brief Declaration of RealSense factory interface
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief RealSense factory interface
 * \details RealSense factory creates and gives RealSense face detector handler
 */
class IVCP_EXPORT IRealSenseFactory
{
public:
    /*!
     * \brief RealSense factory destructor
     */
    virtual ~IRealSenseFactory() {};

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
        bool cutBackground = false) = 0;

protected:

private:
};
}
/** @}*/
#endif

