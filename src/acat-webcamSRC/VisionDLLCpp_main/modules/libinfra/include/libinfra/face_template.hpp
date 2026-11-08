////////////////////////////////////////////////////////////////////////////
// <copyright file="face_template.hpp" company="Intel Corporation">
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

#ifndef __FACETEMPLATE_HPP__
#define __FACETEMPLATE_HPP__

/*!
 * \defgroup libinfra IVCP Infrastructure Library
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file face_template.hpp
 * \brief Declaration of template face mask and canonical landmarks
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

#include "libivcp/types.hpp"
#include <map>

namespace ivcp
{
/*!
 * \brief Get map with coordinates of regions in template mask
 */
IVCP_EXPORT std::map<RegionType, std::pair<cv::Point2i, cv::Point2i> > getTemplateMask(RegionCreatorType type);

/*!
 * \brief Get map with coordinates of canonical landmarks
 */
IVCP_EXPORT std::map<int, cv::Point2i> getCanonicalLandmark();

/*!
 * \brief Get map with coordinates of points in glasses template
 */
IVCP_EXPORT std::map<int, cv::Point2d> getTemplateGlasses();

/*!
 * \brief Get pair with coordinates of eyes in glasses template
 */
IVCP_EXPORT std::pair<cv::Point2i, cv::Point2i> getTemplateEyePos(RegionCreatorType type);

};

/** @}*/
#endif
