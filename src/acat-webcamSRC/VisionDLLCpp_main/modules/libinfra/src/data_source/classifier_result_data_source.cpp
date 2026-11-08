////////////////////////////////////////////////////////////////////////////
// <copyright file="classifer_result_data_source.cpp" company="Intel Corporation">
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


#include "opencv2/opencv.hpp"
#include "libinfra/data_source/classifier_result_data_source.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
ClassType ClassifierResultDataSource::getClassifyResult(VideoPlaylistFrame frame)
{
    if(!frameMarkup.count(frame.sourceIndex))
    {
        return NOT_CLASSIFIED;
    }
    if(!lastCheck.count(frame.sourceIndex))
    {
        lastCheck[frame.sourceIndex] = frameMarkup[frame.sourceIndex].begin();
    }

    ListOfMarkedFrameRanges &sourceMarkup = frameMarkup[frame.sourceIndex];
    ListOfMarkedFrameRanges::iterator iter;

    if(frame.frameIndex >= lastCheck[frame.sourceIndex]->range.firstFrame)
    {
        iter = lastCheck[frame.sourceIndex];
    }
    else
    {
        iter = frameMarkup[frame.sourceIndex].begin();
    }

    while(iter != sourceMarkup.end() && frame.frameIndex > iter->range.lastFrame)//Both boundaries of frame range are inclusive
    {
        lastCheck[frame.sourceIndex] = iter++;
    }
    
    if(iter != sourceMarkup.end() && frame.frameIndex >= iter->range.firstFrame)
    {
        lastCheck[frame.sourceIndex] = iter;
        return iter->classId;
    }
    else
    {
        return NOT_CLASSIFIED;
    }
}

bool ClassifierResultDataSource::isClassified(VideoPlaylistFrame frame)
{
    return (getClassifyResult(frame) != NOT_CLASSIFIED);
}

void ClassifierResultDataSource::addClassifyResult(VideoPlaylistFrame frame, ClassType mark)
{
    if(mark < 0)
    {
        removeClassifyResult(frame);
        return;
    }
    if(!frameMarkup.count(frame.sourceIndex))
    {
        frameMarkup[frame.sourceIndex].push_back(MarkedRange(FrameRange(frame.frameIndex,frame.frameIndex),mark));
        lastCheck[frame.sourceIndex] = frameMarkup[frame.sourceIndex].begin();
        return;
    }
    if(!lastCheck.count(frame.sourceIndex))
    {
        lastCheck[frame.sourceIndex] = frameMarkup[frame.sourceIndex].begin();
    }

    ListOfMarkedFrameRanges &sourceMarkup = frameMarkup[frame.sourceIndex];
    ListOfMarkedFrameRanges::iterator iter;

    if(frame.frameIndex >= lastCheck[frame.sourceIndex]->range.firstFrame)
    {
        iter = lastCheck[frame.sourceIndex];
    }
    else
    {
        iter = frameMarkup[frame.sourceIndex].begin();
    }

    while(iter != sourceMarkup.end() && frame.frameIndex > iter->range.lastFrame)//Both boundaries of frame range are inclusive
    {
        lastCheck[frame.sourceIndex] = iter++;
    }
    
    ListOfMarkedFrameRanges::iterator prev = iter;
    if(iter == sourceMarkup.begin())
    {
        prev = sourceMarkup.end();
    }
    else
    {
        --prev;
    }

    if(prev != sourceMarkup.end() && mark == prev->classId && frame.frameIndex == (prev->range.lastFrame+ 1) )
    {
        //New frame is adjacent to the previous range

        if(iter == sourceMarkup.end())
        {
            //And there is no next frame
            //So just add it to previous range
            prev->range.lastFrame = frame.frameIndex;
            lastCheck[frame.sourceIndex] = prev;
            return;
        }

        if(frame.frameIndex < iter->range.firstFrame )
        {
            //New frame is out of next range
            if(mark == iter->classId && frame.frameIndex == (iter->range.firstFrame- 1) )
            {
                //New frame is also ajanced to next range
                //New frame interconnects ranges so join them
                prev->range.lastFrame = iter->range.lastFrame;
                sourceMarkup.erase(iter);
                lastCheck[frame.sourceIndex] = prev;
            }
            else
            {
                //Frame is not ajanced to next range
                //So just add it to previous range
                prev->range.lastFrame = frame.frameIndex;
                lastCheck[frame.sourceIndex] = prev;
            }
        }
        else
        {
            //New frame is inside of next range
            if(frame.frameIndex == (iter->range.firstFrame) )
            {
                //It should be the first frame of the next range
                if(mark != iter->classId)
                {
                    //Frame should have other mark
                    if(iter->range.firstFrame != iter->range.lastFrame)
                    {
                        //This frame wasn't the last frame of the next range
                        //So transfer it from next range to previous one
                        prev->range.lastFrame = frame.frameIndex;
                        iter->range.firstFrame++;
                        lastCheck[frame.sourceIndex] = prev;
                    }
                    else
                    {
                        //This frame was the last frame of the next range
                        //So reinsert it as long as it can became connectivity frame
                        sourceMarkup.erase(iter);
                        lastCheck[frame.sourceIndex] = prev;
                        addClassifyResult(frame, mark);
                    }
                }
                else
                {
                    //Looks like previous and next frames are adjanced
                    //It could happens if these ranges were not joined early by mistake
                    //So join them and provide notification
                    prev->range.lastFrame = iter->range.lastFrame;
                    sourceMarkup.erase(iter);
                    lastCheck[frame.sourceIndex] = prev;
                    IVCP_LOG_WARNING("Frame ranges were not interconnected by mistake");
                }
            }
            else
            {
                //This could happens only when ranges intersects
                IVCP_EXCEPTION(IncorrectParamException, "Looks like something goes wrong with frame numbers");//Should never happens
            }
        }
    }
    else
    {
        //New frame is not adjacent to previous range

        if(iter == sourceMarkup.end())
        {
            //And there is no next frame
            //So just add new frame range
            lastCheck[frame.sourceIndex] = sourceMarkup.insert(
                iter, MarkedRange(FrameRange(frame.frameIndex,frame.frameIndex),mark));
            return;
        }

        if(frame.frameIndex < iter->range.firstFrame )
        {
            //New frame is out of next range
            if(mark == iter->classId && frame.frameIndex == (iter->range.firstFrame - 1) )
            {
                //However new frame is ajanced to next range
                //Add it to the next range
                iter->range.firstFrame = frame.frameIndex;
                lastCheck[frame.sourceIndex] = iter;
            }
            else
            {
                //Frame is not adjacent to next range
                //Add new frame range
                lastCheck[frame.sourceIndex] = sourceMarkup.insert(iter, MarkedRange(FrameRange(frame.frameIndex,frame.frameIndex),mark));
            }
        }
        else
        {
            //New frame is inside of next range
            if(mark != iter->classId)
            {
                //But this frame should have other mark
                if(frame.frameIndex == (iter->range.firstFrame) )
                {
                    if(frame.frameIndex == (iter->range.lastFrame) )
                    {
                        //It is an only frame of the next range
                        //So reinsert it as long as it can join to next frame
                        sourceMarkup.erase(iter);
                        lastCheck.erase(frame.sourceIndex);
                        addClassifyResult(frame, mark);
                    }
                    else
                    {
                        //It is the first frame of the next range
                        //So add a new frame range and remove the frame from next range
                        iter->range.firstFrame++;
                        lastCheck[frame.sourceIndex] = sourceMarkup.insert(iter, MarkedRange(FrameRange(frame.frameIndex,frame.frameIndex),mark));
                    }
                }
                else
                {
                    if(frame.frameIndex == (iter->range.lastFrame) )
                    {
                        //It is the last frame of the next range
                        //So remove the frame from next range and add a new frame range
                        iter->range.lastFrame--;
                        lastCheck[frame.sourceIndex] = iter;
                        addClassifyResult(frame, mark);
                    }
                    else
                    {
                        //It is the middle frame of the next range
                        //So the next range should be splitted to three different ranges
                        sourceMarkup.insert(iter, MarkedRange(FrameRange(iter->range.firstFrame,frame.frameIndex - 1),iter->classId));
                        lastCheck[frame.sourceIndex] = sourceMarkup.insert(iter, MarkedRange(FrameRange(frame.frameIndex,frame.frameIndex),mark));
                        iter->range.firstFrame = frame.frameIndex + 1;
                    }
                }
            }
            //Do nothing if new mark is already equal to next range mark
        }
    }
}

void ClassifierResultDataSource::removeClassifyResult(VideoPlaylistFrame frame)
{
    if(!frameMarkup.count(frame.sourceIndex))
    {
        return;
    }

    ListOfMarkedFrameRanges &sourceMarkup = frameMarkup[frame.sourceIndex];

    lastCheck.erase(frame.sourceIndex);
    auto iter = frameMarkup[frame.sourceIndex].begin();

    while(iter != sourceMarkup.end() && frame.frameIndex > iter->range.lastFrame)//Both boundaries of frame range are inclusive
    {
        iter++;
    }
    
    if(iter != sourceMarkup.end() && frame.frameIndex >= iter->range.firstFrame )
    {
        if(frame.frameIndex == (iter->range.firstFrame) )
        {
            if(frame.frameIndex == (iter->range.lastFrame) )
            {
                //It is an only frame of the next range
                //So remove the range
                sourceMarkup.erase(iter);
            }
            else
            {
                //It is the first frame of the next range
                //So remove the frame from next range
                iter->range.firstFrame++;
            }
        }
        else if(frame.frameIndex == (iter->range.lastFrame) )
        {
            //It is the last frame of the next range
            //So remove the frame from next range
            iter->range.lastFrame--;
        }
        else
        {
            //It is the middle frame of the next range
            //So the next range should be splitted to couple of ranges
            sourceMarkup.insert(iter, MarkedRange(FrameRange(iter->range.firstFrame,frame.frameIndex - 1),iter->classId));
            iter->range.firstFrame = frame.frameIndex + 1;
        }
    }

    if (sourceMarkup.begin() == sourceMarkup.end())
    {
        frameMarkup.erase(frame.sourceIndex);
    }
}

void ClassifierResultDataSource::saveSerializeMat(const Mat& data, FileStorage& fs)
{
    int numberOfRanges = data.rows;

    fs << "sourceranges" << "[";
        for(int r = 0; r < numberOfRanges; r++)
        {
            fs << "{";
            int currentSourceIndex = data.at<int>(r, 0);
            fs << "sourceindex" << currentSourceIndex;
            fs << "rangemarks" << "[";
                for(; r < numberOfRanges; r++)
                {
                    if (data.at<int>(r, 0) != currentSourceIndex)
                    {
                        r--;
                        break;
                    }

                    fs << "{";
                        //Both boundaries of frame range are inclusive
                        fs << "rangestart" << data.at<int>(r, 1);
                        fs << "rangeend" << data.at<int>(r, 2);
                        fs << "rangeclass" << data.at<int>(r, 3);
                    fs << "}";
                }
            fs << "]";
            fs << "}";
        }
    fs << "]";
}

Mat ClassifierResultDataSource::loadSerializeMat(FileStorage &fs, FileNode fn)
{
    (void) fs;

    FileNode cnode;
    FileNodeIterator siter,riter;

    //Pre - calculate number of ranges
    int numberOfRanges = 0;
    cnode = fn["sourceranges"];
    for(siter = cnode.begin(); siter != cnode.end(); siter++)
    {
        for(riter = (*siter)["rangemarks"].begin(); riter != (*siter)["rangemarks"].end(); riter++)
        {
            numberOfRanges++;
        }
    }

    //Create serialization data matrix
    Mat data(numberOfRanges, 4, CV_32SC1);

    int rangeIndex = 0;
    cnode = fn["sourceranges"];
    for(siter = cnode.begin(); siter != cnode.end(); siter++)
    {
        int sourceIndex = (*siter)["sourceindex"];
        for(riter = (*siter)["rangemarks"].begin(); riter != (*siter)["rangemarks"].end(); riter++, rangeIndex++)
        {
            FileNode node = *riter;
            data.at<int>(rangeIndex, 0) = sourceIndex;
            data.at<int>(rangeIndex, 1) = (int) (node["rangestart"]);//Both boundaries of frame range are inclusive
            data.at<int>(rangeIndex, 2) = (int) (node["rangeend"]);
            data.at<int>(rangeIndex, 3) = (int) (node["rangeclass"]);
        }
    }

    return data;
}

void ClassifierResultDataSource::save(FileStorage& fs)
{
    int numberOfRanges = 0;

    //Pre - calculate number of ranges
    for(auto siter = frameMarkup.begin(); siter != frameMarkup.end(); siter++ )
    {
        for(auto riter = siter->second.begin() ; riter != siter->second.end(); riter++ )
        {
            numberOfRanges++;
        }
    }

    int rangeIndex = 0;
    Mat data(numberOfRanges, 4, CV_32SC1);

    for(auto siter = frameMarkup.begin(); siter != frameMarkup.end(); siter++ )
    {
        int sourceIndex = siter->first;
        for(auto riter = siter->second.begin() ; riter != siter->second.end(); riter++, rangeIndex++ )
        {
            //Both boundaries of frame range are inclusive
            data.at<int>(rangeIndex, 0) = sourceIndex;
            data.at<int>(rangeIndex, 1) = riter->range.firstFrame;
            data.at<int>(rangeIndex, 2) = riter->range.lastFrame;
            data.at<int>(rangeIndex, 3) = riter->classId;
        }
    }

    saveSerializeMat(data, fs);
}

void ClassifierResultDataSource::load(FileStorage &fs, FileNode fn)
{
    Mat data = loadSerializeMat(fs, fn);

    int numberOfRanges = data.rows;

    frameMarkup.clear();
    lastCheck.clear();

    for(int r = 0; r < numberOfRanges; r++)
    {
        int currentSourceIndex = data.at<int>(r, 0);

        ListOfMarkedFrameRanges ranges;
        for(; r < numberOfRanges; r++)
        {
            if (data.at<int>(r,0) != currentSourceIndex)
            {
                r--;
                break;
            }

            MarkedRange range;
            range.range.firstFrame = data.at<int>(r, 1);//Both boundaries of frame range are inclusive
            range.range.lastFrame = data.at<int>(r, 2);
            range.classId = data.at<int>(r, 3);
            ranges.push_back(range);
        }
        frameMarkup[currentSourceIndex] = ranges;
        lastCheck[currentSourceIndex] = frameMarkup[currentSourceIndex].begin();
    }
}

ClassifierResultType ClassifierResultDataSource::getType()
{
    return XML_CLASSIFIER_RESULTS;
}

};
