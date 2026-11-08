////////////////////////////////////////////////////////////////////////////
// <copyright file="gt_region_data_source.hpp" company="Intel Corporation">
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

#ifndef __GT_REGION_DATA_SOURCE_HPP__
#define __GT_REGION_DATA_SOURCE_HPP__

#include <list>
#include "opencv2/opencv.hpp"
#include "libivcp/data_source/iregion_data_source.hpp"
#include "libivcp/video_source/video_playlist_frame.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file gt_region_data_source.hpp
 * \brief Declaration of class to read and save ground truth regions in XML format
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */


namespace ivcp
{
/*!
* \brief Data source for the ground truth regions
* \details Data source contains ground truth regions and provides reading and saving these regions
* \sa DataSource
* \sa ClassifierResultDataSource
*/
class IVCP_EXPORT GTRegionDataSource: public IRegionDataSource
{
public:
    /*!
    * \brief Frame regions and transformation description structure
    */
    struct RegionsDescription
    {
        /*!
        * \brief List of regions marked on the frame
        */
        std::list <std::pair <ClassType, cv::Rect> > regions;
        /*!
        * \brief Transformation matrix for the frame
        */
        cv::Mat frameTransformation;
    };
        
    /*!
    * \brief Default constructor
    */
    GTRegionDataSource();

    /*!
     * \brief Destructor
     */
    virtual ~GTRegionDataSource();

    /*!
    * \brief Get ground truth region
    * \param[in] frame Frame that contains region
    * \param[in] regionClass Class of region
    * \return Bounding rectangle of region
    */
    virtual cv::Rect getRegion(VideoPlaylistFrame frame, ClassType regionClass);

    /*!
    * \brief Get all regions in frame
    * \param[in] frame Frame that contains the regions
    * \return List of regions
    */
    virtual std::list <std::pair <ClassType, cv::Rect> > getRegionsByFrame(VideoPlaylistFrame frame);

    /*!
    * \brief Get transformation of frame related to extraction of ground truth regions
    * \param[in] frame Frame that contains regions
    * \return Transformation matrix for the frame
    */
    virtual cv::Mat getTransformation(VideoPlaylistFrame frame);

    /*!
    * \brief Set transformation of frame related to extraction of ground truth regions
    * \details There should be at least one region related to the frame to store transformation information.
    * If there is no region defined on the frame nothing would be done.
    * \param[in] frame Frame that contains regions
    * \param[in] transformation Transformation matrix for the frame
    */
    virtual void setTransformation(VideoPlaylistFrame frame, cv::Mat transformation);

    /*!
    * \brief Add new region in region data source
    * \param[in] frame Frame that contains region
    * \param[in] rect Bounding rectangle of region
    * \param[in] regionClass Class of region
    * \param[in] transformation Matrix of transformation that should be applied to image prior to region extraction
    */
    virtual void addRegion(VideoPlaylistFrame frame, cv::Rect rect, ClassType regionClass, cv::Mat transformation = cv::Mat());

    /*!
    * \brief Delete region from region data source
    * \param[in] frame Frame that contains region
    * \param[in] regionClass Class of region
    */
    virtual void delRegion(VideoPlaylistFrame frame, ClassType regionClass);

    /*!
    * \brief Save ground truth regions to a file
    * \param[in] fs FileStorage where regions shall be saved
    * \throw IOException Unable to save data source to this file storage
    */
    virtual void save(cv::FileStorage& fs);

   /*!
    * \brief Load ground truth regions from a file
    * \param[in] fs FileStorage where was saved regions
    * \param[in] fn FileNode where was saved regions
    * \throw IOException Unable to load regions from the file storage
    */
    virtual void load(cv::FileStorage &fs, cv::FileNode fn);

    /*!
    * \brief Save content of the serialization matrix to the File storage
    * \details Serialization matrix is used by XMLSerializer classes
    * \param[in] data Region data serialization matrix
    * \param[in] transformations Frame transformation serialization matrix
    * \param[in, out] fs File Storage where content of the serialization matrix is saved
    * \throw IOException Unable to save serialization matrix to the file storage
    */
    static void saveSerializeMat(const cv::Mat& data, const cv::Mat& transformations, cv::FileStorage &fs);
    
    /*!
    * \brief Load serialization matrix from File storage
    * \details Serialization matrix is used by XMLSerializer classes 
    * \param[in] fn FileNode that contains the serialization parameters
    * \param[in] fs File Storage that contains the serialization parameters
    * \param[out] data Region data serialization matrix
    * \param[out] transformations Frame transformation serialization matrix
    * \throw IOException Unable to load serialization matrix from the file storage
    */
    static void loadSerializeMat(cv::FileStorage &fs, cv::FileNode fn, cv::Mat &data, cv::Mat &transformations);

    /*!
     * \brief Get type of ground truth regions
     * \return XML_GROUNDTRUTH_REGIONS
     */
    virtual GTRegionsType getType();

protected:

private:
    std::list <std::pair <VideoPlaylistFrame, RegionsDescription > > gtRegions;
};
};

/** @}*/
#endif