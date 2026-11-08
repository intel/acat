///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_HPP__
#define __PIPELINE_HPP__

#include <string>
#include "libivcp/video_source/video_playlist.hpp"
#include "libivcp/ipipeline.hpp"
#include "libpipeline/pipeline_node.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline.hpp
 * \brief Declaration of pipeline class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief Pipeline implementation
 * \details The Pipeline component is responsible for connecting all the rest components together and 
 * for managing the message traffic between the components.
*/

class IVCP_EXPORT Pipeline: public IPipeline
{
public:
    /*!
     * \brief Constructor
     * \details Operational mode should be defined
     * \param[in] mode Pipeline operational mode
     * \param[in] settings Pipeline settings
     */
    Pipeline(PipelineMode mode, Settings settings = Settings());

    /*!
     * \brief Constructor
     * \details Operational mode should be defined
     * \param[in] session Session to search for filters
     * \param[in] mode Pipeline operational mode
     * \param[in] settings Pipeline settings
     */
    Pipeline(SPtr<Session > session, PipelineMode mode, Settings settings = Settings());

    /*!
     * \brief Destructor
     */
    virtual ~Pipeline();

    virtual PipelineMode getMode();

    virtual void setSession(SPtr<Session> session);
    virtual SPtr<Session> getSession();

    virtual void run();

    virtual void connectAll();

    virtual void addNodeCallback(std::string filterId, PipelineCallback func, void* context );

    virtual void removeNodeCallback(std::string filterId, PipelineCallback func, void* context );

    virtual void removeNodeCallbacks(std::string filterId);

    virtual void addLink(std::string from, std::string to);

    virtual void removeLink(std::string from, std::string to);

    virtual void removeAllLinks();

    virtual std::pair<std::string,std::string> getLink(int linkIndex);

    virtual int getNumberOfLinks() const;

    virtual void trackFeatureToFile(std::string classifier_id, std::string filename = "");

    virtual FilterType getRootType() const;

    virtual Settings getSettings();

    virtual std::vector<VideoPlaylistFrame> getLastProcessedFrames(std::string featureId);

    virtual VideoPlaylistFrame getCurrentFrame();

    virtual void setCurrentFrame(VideoPlaylistFrame frame);

    virtual void setFilterPriority(const std::string& filterName, ProcessingPriority priority);

protected:
    /*!
     * \brief Pipeline operational mode
     */
    PipelineMode currentMode;

    /*!
     * \brief Pipeline settings
     * \details Distance between frames in one frame sequence
     * and distance between starts of frame sequences
     */
    Settings pipelineSettings;
    
    /*!
     * \brief Session used to search for filters
     */
    SPtr<Session > currentSession;

    /*!
     * \brief Pointer to processed playlist
     */
    SPtr<const VideoPlaylist > playlist;

    /*!
     * \brief Array of pipeline link descriptions
     */
    std::vector<std::pair<std::string,std::string> > links;

    /*!
     * \brief Array of created filter wrapper nodes
     */
    std::vector<SPtr<IPipelineNode> > nodes;

    /*!
     * \brief Node callbacks: a map from filter id to a set of pairs 
     * (callback function, context pointer)
     */
    std::map< std::string, std::set<std::pair<PipelineCallback, void* > > > nodeCallbacks;

	/*!
     * \brief Frame processed on current pipeline step  
     */
    VideoPlaylistFrame currentFrame;

    /*!
     * \brief Checks whether it is possible to connect particular filters types
     * \param[in] sourceType Type of filter from which link should be created
     * \param[in] destinationType Type of filter to which link should be created
     */
    virtual bool areConnectable(FilterType sourceType, FilterType destinationType);

    /*!
     * \brief Create nodes for all used filters
     * \throw IncorrectParamException Session is not set or no necessary filters
     */
    void createNodes();

    /*!
     * \brief Remove all created nodes
     * \throw IncorrectParamException Session is not set
     */
    void clearNodes();

    /*!
     * \brief Create a node for provided filter
     * \param[in] filter Filter to create wrapper node for
     * \returns Pointer to created node or empty pointer if node could not be created
     * \throw IncorrectParamException No playlist is set or error in creating Pipeline Nodes
     */
    virtual SPtr<IPipelineNode> addNode(SPtr<IFilter> filter);

    /*!
     * \brief Get a previously created node
     * \param[in] filterId Previously created node name
     * \returns Pointer to previously created node or empty pointer if there is no node with such name
     */
    SPtr<IPipelineNode> getNode(std::string filterId);

    /*!
     * \brief Map for translation classifier ID to output filename
     */
    std::map<std::string, std::string> classifierToOutputFile;

private:

};
};

/** @}*/
#endif

