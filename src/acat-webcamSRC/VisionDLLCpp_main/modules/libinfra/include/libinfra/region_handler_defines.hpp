////////////////////////////////////////////////////////////////////////////
// <copyright file="region_handler_defines.hpp" company="Intel Corporation">
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

#ifndef __REGIONHANDLERDEFINES_HPP__
#define __REGIONHANDLERDEFINES_HPP__

/*!
 * \defgroup libinfra IVCP Infrastructure Library
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file region_handler_defines.hpp
 * \brief Declaration of defines for glasses search workarounds and etc.
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

/*!
 * \brief Number of frame to start normalization
 */
#define START_FRAME_NORMALIZATION 10

/*!
 * \brief The maximum number of iterations/elements
 */
#define SIMPLEX_MAX_COUNT 100

/*!
 * \brief The desired accuracy
 */
#define SIMPLEX_EPSILON 0.0001

/*!
 * \brief Simplex initial step: dx
 */
#define SIMPLEX_INITIAL_STEP_DX 1.0

/*!
 * \brief Simplex initial step: dy
 */
#define SIMPLEX_INITIAL_STEP_DY 20.0

/*!
 * \brief Simplex initial point
 */
#define SIMPLEX_INITIAL_POINT 0.0

/*!
 * \brief Min scale for glasses template
 */
#define MIN_SCALE 0.99

/*!
 * \brief Max scale for glasses template
 */
#define MAX_SCALE 1.02

/*!
 * \brief Step for scale for glasses template
 */
#define STEP_SCALE 0.01

/*!
 * \brief Min value for glasses template rotation
 */
#define MIN_ALPHA -0.15

/*!
 * \brief Max value for glasses template rotation
 */
#define MAX_ALPHA 0.1

/*!
 * \brief Step of angles for glasses template rotation
 */
#define STEP_ALPHA 0.025

/*!
 * \brief Max global dx value for glasses template
 */
#define MAX_G_DX 5

/*!
 * \brief Min global dx value for glasses template
 */
#define MIN_G_DX -5

/*!
 * \brief Max global dy value for glasses template
 */
#define MAX_G_DY 40

 /*!
 * \brief Min global dy value for glasses template
 */
#define MIN_G_DY -20

/*!
 * \brief Number of the first glasses point in glasses template
 */
#define FIRST_GLASSES_POINT 1

/*!
 * \brief Number of the second glasses point in glasses template
 */
#define SECOND_GLASSES_POINT 4

/*!
 * \brief Number of the first down rim point in glasses template
 */
#define FIRST_DOWN_RIM_POINT 11

/*!
 * \brief Number of the second down rim point in glasses template
 */
#define SECOND_DOWN_RIM_POINT 14

/*!
 * \brief Min value to make cheek region downer with glasses search wa
 */
#define MIN_CHEEK_REGION_PART 1 / 7

/*!
 * \brief Max value to make cheek region downer with glasses search wa
 */
#define MAX_CHEEK_REGION_PART 1 / 4

/*!
 * \brief Min value to make cheek region upper with glasses search wa
 */
#define MIN_EYEBROW_REGION_PART 1 / 7

/*!
 * \brief Max value to make cheek region upper with glasses search wa
 */
#define MAX_EYEBROW_REGION_PART 4 / 7

/*!
 * \brief Value for make center of rotation upper
 */
#define UP_CENTER_ROTATION 200

/*!
 * \brief Max number of possible false not detected face cases
 */
#define MAX_BAD_ANGLE_COUNT 4

/*!
 * \brief The maximum number of start angle for face search wa
 */
#define MAX_ANGLE_WA 40

/*!
 * \brief Min number of frame with detected face for face search wa (Dlib handler)
 */
#define MIN_FRAME_COUNT_GOOD_ANGLE 10

/** @}*/
#endif
