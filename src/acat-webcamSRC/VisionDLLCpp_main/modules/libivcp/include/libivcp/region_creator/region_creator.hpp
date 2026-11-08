///////////////////////////////////////////////////////////////////////////
// <copyright file="region_creator.hpp" company="Intel Corporation">
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



#ifndef __REGION_CREATOR_HPP__
#define __REGION_CREATOR_HPP__

#include "libivcp/region_creator/iregion_creator.hpp"
#include "libivcp/video_source/video_playlist.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file region_creator.hpp
 * \brief Basic class for all region creators
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */


namespace ivcp
{
/*! 
 * \brief Basic class for all region creators
 * \sa IRegionCreator
 * \sa BBoxRegionCreator
 * \sa GTRegionCreator
 */
class IVCP_EXPORT RegionCreator: public IRegionCreator
{
public:
    /*!
     * \brief Default constructor
     */
    RegionCreator();

    virtual ~RegionCreator(){};

    virtual FilterType getRootType() const;    

    virtual void setPlaylist(SPtr<const VideoPlaylist> videoPlaylist);

    virtual SPtr<const VideoPlaylist> getPlaylist();

protected:
    /*!
    * \brief Active playlist
    * \details This playlist used to check for currently processed frame
    */
    SPtr<const VideoPlaylist> activePlaylist;

private:

};
};

/** @}*/
#endif
