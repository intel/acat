///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_feature_node.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_FEATURE_NODE_HPP__
#define __PIPELINE_FEATURE_NODE_HPP__

#include <map>
#include <queue>
#include "libpipeline/pipeline_node.hpp"
#include "libivcp/feature/ifeature.hpp"
#include "libivcp/video_source/video_playlist.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_feature_node.hpp
 * \brief Declaration of pipeline source filter class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Feature filter wrapper node implementation
 */
class IVCP_EXPORT PipelineFeatureNode: public PipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] feature Feature filter to create wrapper node for
     * \param[in] settings Pipeline settings
     */
    PipelineFeatureNode(SPtr<IFeature> feature, IPipeline::Settings settings = IPipeline::Settings());
    virtual ~PipelineFeatureNode() {};

    virtual void addSourceLink(std::string from);
    virtual void removeSourceLink(std::string from);
    
    virtual void run();
    virtual void transmitMessage(cv::Mat message, std::string caller);

    /*!
     * \brief Get frames on which the previous feature vector was calculated
     * \return Vector of frames
     */
    std::vector<VideoPlaylistFrame> getLastProcessedFrames();

protected:
    /*!
     * \brief Pointer to the wrapped feature(casted filter pointer)
     */
    SPtr<IFeature> feature;

    /*!
     * \brief Necessary quantity of frames 
     * \details Quantity of frames that should be successfuly processed prior to feature calculation
     * and frames that should be skipped
     */
    unsigned int needFrames;

    /*!
     * \brief Last processed frame index
     */
    ivcp::VideoPlaylistFrame currentFrame;

    /*!
     * \brief Regions already detected on last processed frame
     */
    std::map<std::string, cv::Mat> currentRegions;

    /*!
     * \brief Stored sequences of detected regions for to calculate feature
     */
    std::map<std::string, RegionSequence > sourceToRegionSequence;

    /*!
     * \brief Frames on which the previous feature vector was calculated
     */
    std::vector<VideoPlaylistFrame> lastProcessedFrames;

    /*!
     * \brief Pipeline settings
     * \details Distance between frames in one frame sequence
     * and distance between starts of frame sequences
     */
    IPipeline::Settings pipelineSettings;

    /*!
     * \brief Stored sequences of detected regions for previous frames
     */
    std::map<std::string, RegionSequence > framesBuffer;

    /*!
     * \brief Stored numbers of frames for framesBuffer
     */
    std::deque<VideoPlaylistFrame> frameNumbersBuffer;

    /*!
     * \brief Stored numbers of frames what will be processed by feature
     */
    std::deque<VideoPlaylistFrame> frameNumbersForProcessing;

    /*!
     * \brief Quantity of frames that should be skipped
     */
    unsigned int needSkipFrames;

private:

};

};

/** @}*/
#endif 
