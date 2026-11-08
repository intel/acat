///////////////////////////////////////////////////////////////////////////
// <copyright file="ipipeline.hpp" company="Intel Corporation">
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


#ifndef __IPIPELINE_HPP__
#define __IPIPELINE_HPP__

#include <string>
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/session.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file ipipeline.hpp
 * \brief Declaration of pipeline interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Pipeline execution mode
 */
enum PipelineMode
{
    TRAIN_PIPELINE,
    TEST_PIPELINE,
    RUNTIME_PIPELINE
};

/*! 
 * \brief Pointer to callback function used to visualize pipeline
 * \details Please note that cv::Mat parameter of the callback function
 * contains data shared between the pipeline and the function and must be retained intact.
 * If you have to modify the matrix content you should clone it and process your own copy
 * otherwise pipeline evaluation result could be incorrect.
 */
typedef void (*PipelineCallback)(cv::Mat, void *);

/*!
 * \brief Basic interface a pipeline
 * \details The Pipeline component is responsible for connecting all the rest components together and 
 * for managing the message traffic between the components.
 */
class IVCP_EXPORT IPipeline: public Filter
{
public:
    /*!
     * \brief OS thread priority enumeration
     */
    enum ProcessingPriority
    {
        LOW = -1,
        NORMAL,
        HIGH
    };

    /*!
     * \brief Pipeline settings
     */
    struct Settings
    {
        /*!
         * \brief Distance between frames in one frame sequence
         */
        unsigned int distanceBetweenFrames;
        /*!
         * \brief Distance between first frames of 2 sequential frame sequences
         */
        unsigned int distanceBetweenStartsFrameSeq; 

        /*!
         * \brief Settings constructor
         */
        Settings(unsigned int distBetweenFrames = 1, unsigned int distBetweenStartsFrameSeq = 1)
        {
            distanceBetweenFrames = distBetweenFrames;
            distanceBetweenStartsFrameSeq = distBetweenStartsFrameSeq;
        }
    };

    /*!
     * \brief Destructor
     */
    virtual ~IPipeline() {};

    /*!
     * \brief Get current pipeline mode
     * \returns Current pipeline operational mode
     */
    virtual PipelineMode getMode() = 0;

    /*!
     * \brief Configure pipeline in accordance with session
     * \param[in] session Pointer to session object
     */
    virtual void setSession(SPtr<Session> session) = 0;

    /*!
     * \brief Get pipeline associated session
     * \return Pointer to session object
     */
    virtual SPtr<Session> getSession() = 0;

    /*!
     * \brief Run the classification pipeline
     * \throw IncorrectParamException Runing of the Pipeline is failed due to incorrect or missing parameters
     */
    virtual void run() = 0;

    /*!
     * \brief Connect all filter wrappers "layer-by-layer" (ie filters from layer n shall be connected to filters on layer n+1)
     * \throw IncorrectParamException Automatic connection is failed due to missing filters or ambiguity
     */
    virtual void connectAll() = 0;

    /*!
     * \brief Add link between 2 filter wrappers
     * \param[in] from Source of messages
     * \param[in] to Destination of messages
     * \throw IncorrectParamException Could not create link since there is no filter with provided id
     */
    virtual void addLink(std::string from, std::string to) = 0;

    /*!
     * \brief Remove link between 2 filter wrappers
     * \param[in] from Source of messages
     * \param[in] to Destination of messages
     */
    virtual void removeLink(std::string from, std::string to) = 0;

    /*!
     * \brief Remove all links
     */
    virtual void removeAllLinks() = 0;

    /*!
     * \brief Get link by index (from 0 to getNumberOfLinks()-1)
     * \param[in] linkIndex Link index in the Pipeline
     * \returns pairs consist of from and to Node names
     * \throw IncorrectParamException There is no link with provided index
     */
    virtual std::pair<std::string,std::string> getLink(int linkIndex) = 0;

    /*!
     * \brief Get links quantity
     * \returns Links quantity
     */
    virtual int getNumberOfLinks() const = 0;

    /*!
     * \brief Add a debugging callback to particular node
     * \details Each time a pipeline node generates new output the callback function is called. Can be used for processing visualization
     * Please note that cv::Mat parameter of the callback function
     * contains data shared between the pipeline and the function and must be retained intact.
     * If you have to modify the matrix content you should clone it and process your own copy
     * otherwise pipeline evaluation result could be incorrect.
     * \param[in] filterId Node filter string id
     * \param[in] func Pointer to callback function
     * \param[in] context Pointer to context data for callback function
     */
    virtual void addNodeCallback(std::string filterId, PipelineCallback func, void* context ) = 0;

    /*!
     * \brief Remove a callback of the given node
     * \param[in] filterId Node filter string id
     * \param[in] func Pointer to callback function
     * \param[in] context Pointer to context data for callback function
     */ 
    virtual void removeNodeCallback(std::string filterId, PipelineCallback func, void* context ) = 0;

    /*!
     * \brief Remove all callbacks of given node
     * \param[in] filterId Node filter string id
     */
    virtual void removeNodeCallbacks(std::string filterId) = 0;

    /*!
     * \brief Get pipeline settings
     * \return Pipeline settings
     */
    virtual Settings getSettings() = 0;

    /*!
     * \brief Get frames on which the previous feature vector was calculated
     * \param[in] featureId Identifier of feature
     * \return Vector of last processed frames for feature with the given id
     */
    virtual std::vector<VideoPlaylistFrame> getLastProcessedFrames(std::string featureId) = 0;

    /*!
     * \brief Get video source index and frame index pair of current processed frame in pipeline
     * \return Current frame
     */
    virtual VideoPlaylistFrame getCurrentFrame() = 0;

    /*!
     * \brief Set new current frame processed by pipeline
     * \param[in] frame New current frame
     */
    virtual void setCurrentFrame(VideoPlaylistFrame frame) = 0;

    /*!
     * \brief Track features and classifier result to file
     * \param[in] classifier_id Id of classifier
     * \param[in] filename Path to output file. By default this parameter is set to classifier id.
     * \throw IncorrectParamException There is no session defined
     */
    virtual void trackFeatureToFile(std::string classifier_id, std::string filename = "") = 0;

    /*!
     * \brief Set execution priority for the filter
     * \details For sequential pipeline this function currently does nothing
     * \param[in] filterName Filter name, for which priority should be changed
     * \param[in] priority Priority to set (ivcp::IPipeline::LOW, ivcp::IPipeline::NORMAL, ivcp::IPipeline::HIGH)
     */
    virtual void setFilterPriority(const std::string& filterName, ProcessingPriority priority) = 0;


protected:

private:

};
};

/** @}*/
#endif 
