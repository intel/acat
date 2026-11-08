////////////////////////////////////////////////////////////////////////////
// <copyright file="gt_region_data_source.cpp" company="Intel Corporation">
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


#include "libinfra/data_source/gt_region_data_source.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
GTRegionDataSource::GTRegionDataSource()
{
}

GTRegionDataSource::~GTRegionDataSource()
{
}

Rect GTRegionDataSource::getRegion(VideoPlaylistFrame frame, ClassType regionClass)
{
    Rect rect(0, 0, 0, 0);
    auto iterOfFrames = gtRegions.begin();

    while (iterOfFrames != gtRegions.end() && iterOfFrames->first != frame)
    {
        iterOfFrames++;
    }

    if (iterOfFrames != gtRegions.end() && iterOfFrames->first == frame)
    {
        auto iterOfRegions = iterOfFrames->second.regions.begin();
        while (iterOfRegions != iterOfFrames->second.regions.end() && iterOfRegions->first != regionClass)
        {
            iterOfRegions++;
        }
        if (iterOfRegions != iterOfFrames->second.regions.end() && iterOfRegions->first == regionClass)
        {
            rect = iterOfRegions->second;
        }
    }

    return rect;
}

list <pair <ClassType, Rect> >  GTRegionDataSource::getRegionsByFrame(VideoPlaylistFrame frame)
{
    auto iterOfFrames = gtRegions.begin();
    list <pair <ClassType, Rect> > regions;

    while (iterOfFrames != gtRegions.end() && iterOfFrames->first != frame)
    {
        iterOfFrames++;
    }

    if (iterOfFrames != gtRegions.end() && iterOfFrames->first == frame)
    {
        regions = iterOfFrames->second.regions;
    }

    return regions;
}

Mat GTRegionDataSource::getTransformation(VideoPlaylistFrame frame)
{
    auto iterOfFrames = gtRegions.begin();
    Mat transformation;

    while (iterOfFrames != gtRegions.end() && iterOfFrames->first != frame)
    {
        iterOfFrames++;
    }

    if (iterOfFrames != gtRegions.end() && iterOfFrames->first == frame)
    {
        transformation = iterOfFrames->second.frameTransformation.clone();
    }

    return transformation;
}

void GTRegionDataSource::setTransformation(VideoPlaylistFrame frame, cv::Mat transformation)
{
    auto iterOfFrames = gtRegions.begin();

    while (iterOfFrames != gtRegions.end() && iterOfFrames->first != frame)
    {
        iterOfFrames++;
    }

    if (iterOfFrames != gtRegions.end() && iterOfFrames->first == frame)
    {
        iterOfFrames->second.frameTransformation = transformation.clone();
    }
}

void GTRegionDataSource::addRegion(VideoPlaylistFrame frame, Rect rect, ClassType regionClass, Mat _transformation)
{
    pair <ClassType, Rect> region(regionClass, rect);
    auto iterOfFrames = gtRegions.begin();

    while (iterOfFrames != gtRegions.end() && iterOfFrames->first != frame)
    {
        iterOfFrames++;
    }

    Mat transformation;
    if( _transformation.rows == 3 && _transformation.cols == 3 && countNonZero(_transformation - Mat::eye(3, 3, CV_64FC1)) )
    {
        transformation = _transformation;
    }

    if (iterOfFrames != gtRegions.end() && iterOfFrames->first == frame)
    {
        if( transformation.rows != iterOfFrames->second.frameTransformation.rows ||
            transformation.cols != iterOfFrames->second.frameTransformation.cols ||
            countNonZero(transformation - iterOfFrames->second.frameTransformation) )
            IVCP_EXCEPTION(IncorrectParamException, "Transformation matrixes for the frame are different" );

        iterOfFrames->second.regions.push_back(region);
    }
    else
    {
        list <pair <ClassType, Rect> > regionList;
        regionList.push_back(region);
        pair <VideoPlaylistFrame, RegionsDescription > frameRegionPair;
        frameRegionPair.first = frame;
        frameRegionPair.second.regions = regionList;
        frameRegionPair.second.frameTransformation = transformation;
        gtRegions.push_back(frameRegionPair);
    }
}

void GTRegionDataSource::delRegion(VideoPlaylistFrame frame, ClassType regionClass)
{
    auto iterOfFrames = gtRegions.begin();

    while (iterOfFrames != gtRegions.end() && iterOfFrames->first != frame)
    {
        iterOfFrames++;
    }

    if (iterOfFrames != gtRegions.end() && iterOfFrames->first == frame)
    {
        auto iterOfRegions = iterOfFrames->second.regions.begin();
        while (iterOfRegions != iterOfFrames->second.regions.end() && iterOfRegions->first != regionClass)
        {
            iterOfRegions++;
        }

        if (iterOfRegions != iterOfFrames->second.regions.end() && iterOfRegions->first == regionClass)
        {
            iterOfFrames->second.regions.erase(iterOfRegions);
            if (iterOfFrames->second.regions.empty())
            {
                gtRegions.erase(iterOfFrames);
            }
        }
    }
}

void GTRegionDataSource::save(FileStorage& fs)
{
    int numberOfRegions = 0;
    int numberOfFrames = 0;
    for(auto iterOfFrames = gtRegions.begin(); iterOfFrames != gtRegions.end(); iterOfFrames++ )
    {
        numberOfFrames++;
        for(auto iterOfRegions = iterOfFrames->second.regions.begin() ; iterOfRegions != iterOfFrames->second.regions.end(); iterOfRegions++ )
        {
            numberOfRegions++;
        }
    }

    Mat data(numberOfRegions, 8, CV_32SC1);
    Mat transformations(numberOfFrames, 9, CV_64FC1);

    int regionIndex = 0;
    int imageIndex = 0;
    for(auto iterOfFrames = gtRegions.begin(); iterOfFrames != gtRegions.end(); iterOfFrames++, imageIndex++ )
    {
        int sourceIndex = iterOfFrames->first.sourceIndex;
        int frameIndex = iterOfFrames->first.frameIndex;
        for(auto iterOfRegions = iterOfFrames->second.regions.begin() ; iterOfRegions != iterOfFrames->second.regions.end(); iterOfRegions++, regionIndex++ )
        {
            data.at<int>(regionIndex, 0) = sourceIndex;
            data.at<int>(regionIndex, 1) = frameIndex;
            data.at<int>(regionIndex, 2) = regionIndex;
            data.at<int>(regionIndex, 3) = iterOfRegions->first;
            data.at<int>(regionIndex, 4) = iterOfRegions->second.x;
            data.at<int>(regionIndex, 5) = iterOfRegions->second.y;
            data.at<int>(regionIndex, 6) = iterOfRegions->second.width;
            data.at<int>(regionIndex, 7) = iterOfRegions->second.height;
        }
        if(iterOfFrames->second.frameTransformation.empty())
        {
            Mat tmp = Mat::eye(3, 3, CV_64FC1);
            tmp.reshape(1, 1).convertTo(transformations.row(imageIndex), CV_64FC1);
        }
        else
        {
            iterOfFrames->second.frameTransformation.reshape(1, 1).convertTo(transformations.row(imageIndex), CV_64FC1);
        }
    }

    saveSerializeMat(data, transformations, fs);
}

void GTRegionDataSource::load(FileStorage &fs, FileNode fn)
{
    Mat data, transformations;
    loadSerializeMat(fs, fn, data, transformations);

    int numberOfRanges = data.rows;

    FileNode cnode;
    FileNodeIterator iter, regIter;
    int x, y, width, height;

    gtRegions.clear();
    int r, f;
    for(r = 0, f = 0; r < numberOfRanges; r++, f++)
    {
        pair <VideoPlaylistFrame, RegionsDescription > frame;
        frame.first.sourceIndex = data.at<int>(r, 0);
        frame.first.frameIndex = data.at<int>(r, 1);
        frame.second.frameTransformation = transformations.row(f).reshape(1, 3).clone();
        if( !countNonZero(frame.second.frameTransformation - Mat::eye(3, 3, CV_64FC1) ) )
        {
            frame.second.frameTransformation = Mat();
        }
        VideoPlaylistFrame currentFrameIndex(frame.first.sourceIndex, frame.first.frameIndex);

        for(; r < numberOfRanges; r++)
        {
            VideoPlaylistFrame followingFrameIndex(data.at<int>(r, 0),  data.at<int>(r, 1));
            if (currentFrameIndex != followingFrameIndex)
            {
                r--;
                break;
            }

            pair <ClassType, Rect> region;
            region.first = data.at<int>(r, 3);
            x = data.at<int>(r, 4);
            y = data.at<int>(r, 5);
            width = data.at<int>(r, 6);
            height = data.at<int>(r, 7);
            region.second = Rect(x, y, width, height);
            frame.second.regions.push_back(region);
        }

        gtRegions.push_back(frame);
    }
}

void GTRegionDataSource::saveSerializeMat(const Mat& data, const Mat& transformations, FileStorage& fs) 
{
    int numberOfRegions = data.rows;

    fs << "frames" << "[";
        int r, f;
        for (r = 0, f = 0; r < numberOfRegions; r++, f++)
        {
            VideoPlaylistFrame currentFrame(data.at<int>(r, 0), data.at<int>(r, 1));

            fs << "{";
                fs << "sourceindex" << data.at<int>(r, 0);
                fs << "frameindex" << data.at<int>(r, 1);
                fs << "transformation" << transformations.row(f);
                fs << "regions" << "[";
                    for(; r < numberOfRegions; r++)
                    {
                        VideoPlaylistFrame followingFrame(data.at<int>(r, 0), data.at<int>(r, 1));
                        if (followingFrame != currentFrame)
                        {
                            r--;
                            break;
                        }

                        fs << "{";
                            fs << "class" << data.at<int>(r, 3);
                            fs << "x" << data.at<int>(r, 4);
                            fs << "y" << data.at<int>(r, 5);
                            fs << "width" << data.at<int>(r, 6);
                            fs << "height" << data.at<int>(r, 7);
                        fs << "}";
                    }
                fs << "]";
            fs << "}";
        }
    fs << "]";
}

void GTRegionDataSource::loadSerializeMat(FileStorage &fs, FileNode fn, Mat &data, Mat &transformations)
{
    (void) fs;

    FileNode cnode;
    FileNodeIterator iter, regIter;

    //Pre-compute number of regions
    int numberOfRegions = 0;
    int numberOfFrames = 0;
    cnode = fn["frames"];
    for(iter = cnode.begin(); iter != cnode.end(); iter++)
    {
        FileNode frameNode = *iter;
        FileNode regNode = frameNode["regions"];
        for(regIter = regNode.begin(); regIter != regNode.end(); regIter++)
        {
            numberOfRegions++;
        }
        numberOfFrames++;
    }

    data = Mat(numberOfRegions, 8, CV_32SC1);
    transformations = Mat(numberOfFrames, 9, CV_64FC1);

    int regionIndex = 0;
    int imageIndex = 0;
    for(iter = cnode.begin(); iter != cnode.end(); iter++, imageIndex++)
    {
        FileNode frameNode = *iter;
        if (frameNode["sourceindex"].empty() || frameNode["frameindex"].empty())
        {
            IVCP_EXCEPTION(IOException, "Unable to read \"sourceindex\" or \"frameindex\" tag. Data is possibly broken");
        }

        int sourceIndex = (int) (frameNode["sourceindex"]);
        int frameIndex = (int) (frameNode["frameindex"]);

        Mat tmp;
        if ( !frameNode["transformation"].empty() )
        {
            frameNode["transformation"] >> tmp;
        }
        if ( tmp.empty() || tmp.cols != 9 || tmp.rows != 1 )
        {
            tmp = Mat::eye(3, 3, CV_64FC1);
            tmp.reshape(1, 1).convertTo(transformations.row(imageIndex), CV_64FC1);
        }
        else
        {
            tmp.convertTo(transformations.row(imageIndex), CV_64FC1);
        }

        FileNode regNode = frameNode["regions"];

        for(regIter = regNode.begin(); regIter != regNode.end(); regIter++, regionIndex++)
        {
            FileNode node = *regIter;

            data.at<int>(regionIndex, 0) = sourceIndex;
            data.at<int>(regionIndex, 1) = frameIndex;
            data.at<int>(regionIndex, 2) = regionIndex;
            data.at<int>(regionIndex, 3) = (ClassType) (node["class"]);
            data.at<int>(regionIndex, 4) = (int) (node["x"]);
            data.at<int>(regionIndex, 5) = (int) (node["y"]);
            data.at<int>(regionIndex, 6) = (int) (node["width"]);
            data.at<int>(regionIndex, 7) = (int) (node["height"]);
        }
    }
}

GTRegionsType GTRegionDataSource::getType()
{
    return XML_GROUNDTRUTH_REGIONS;
}
}
