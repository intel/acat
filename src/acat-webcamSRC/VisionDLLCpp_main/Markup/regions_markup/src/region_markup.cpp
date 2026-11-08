///////////////////////////////////////////////////////////////////////////
// <copyright file="region_markup.cpp" company="Intel Corporation">
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

#include "demo_environment.hpp"
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

SPtr<IVideoPlayer> player;
SPtr<IRegionDataSource> gtRegionSource;
SPtr<DlibRegionCreator> faceLocator;
vector<Mat> detectedTransformations;
int selectedFrame;
bool normalizeImg;
int normalizeCurrent;

Mat transformation;
bool isDraw = false;
bool isMoved = false;
int xleft, ytop, xright, ybottom;
Mat origFrame;
Mat drawFrame;
Mat tmpFrame;
string window = "RegionMarkup";
bool delPressed = false;

const int classCount = 10;
Scalar colors[] = { Scalar(0, 0, 255),
                    Scalar(0, 255, 0),
                    Scalar(255, 0, 0),
                    Scalar(0, 255, 255),
                    Scalar(255, 0, 255),
                    Scalar(255, 255, 0),         
                    Scalar(20, 100, 200),
                    Scalar(200, 20, 100),
                    Scalar(255, 255, 255),
                    Scalar(0, 0, 0) };
string colorNames[] = { "red", "green", "blue", "yellow", "pink", "light blue", "brown", "violet", "white", "black" };
int classType = 0;

enum Keys
{
    ESC = 27,
    LEFT = 37,
    RIGHT = 39,
    DEL = 46,
    N = 110
};

bool isClassBusy(int cl)
{
    VideoPlaylistFrame frame = player->getPlaylist()->getCurrentFrame();

    list <pair <ClassType, Rect> > regions = gtRegionSource->getRegionsByFrame(frame);
    for (auto iter = regions.begin(); iter != regions.end(); iter++)
    {
        if (cl == iter->first)
        {
            return true;
        }
    }

    return false;
}

void setClass(ClassType cl)
{
    if ( isClassBusy(cl) )
    {
        cout << "Class " << cl << " is already used" << endl;
    }
    else
    {
        classType = cl;
        cout << "Current class: " << classType << endl;
    }
}

void showCurrentFrame()
{
    drawFrame = origFrame.clone();

    VideoPlaylistFrame frame = player->getPlaylist()->getCurrentFrame();
    list <pair <ClassType, Rect> > regions = gtRegionSource->getRegionsByFrame(frame);

    transformation = Mat();
    if(normalizeCurrent == 1)
        transformation = gtRegionSource->getTransformation(frame);
    if( (normalizeCurrent == 1 && transformation.cols == 0 && transformation.rows == 0 && regions.empty() ) || normalizeCurrent == 2 )
    {
        if(normalizeImg)
        {
            transformation = detectedTransformations[selectedFrame];
        }
    }
    if(transformation.rows == 3 && transformation.cols == 3)
    {
        Mat trDraw(drawFrame.rows, drawFrame.cols, CV_8UC3);
        warpPerspective(drawFrame, trDraw, transformation, trDraw.size(), 0);
        drawFrame = trDraw;
    }

    for (auto iter = regions.begin(); iter != regions.end(); ++iter)
    {
        rectangle(drawFrame, iter->second, colors[iter->first]);
    }
    
    imshow(window, drawFrame);
}

void startShowFrames()
{
    origFrame = player->getNextFrame();
    showCurrentFrame();
}

void addRegionToCurrentFrame(Rect region)
{
    VideoPlaylistFrame frame = player->getPlaylist()->getCurrentFrame();
    Mat savedTransformation = gtRegionSource->getTransformation(frame);
    if( ( savedTransformation.cols != transformation.cols || savedTransformation.rows != transformation.rows ||
        countNonZero(savedTransformation - transformation) ) && !gtRegionSource->getRegionsByFrame(frame).empty())
    {
        cout << "Attention!!! Frame transformation was redefined." << endl;
        gtRegionSource->setTransformation(frame, transformation);
    }

    gtRegionSource->addRegion(frame, region, classType, transformation);
}

void deleteRegionOfCurrentFrame(int regionClass)
{
    VideoPlaylistFrame frame = player->getPlaylist()->getCurrentFrame();
    gtRegionSource->delRegion(frame, regionClass);
    showCurrentFrame();
}

void toNextFrame()
{
    if (player->haveNextFrame())
    {
        origFrame = player->getNextFrame();
        selectedFrame++;
        showCurrentFrame();
    }
}

void toPrevFrame()
{
    try
    {
        origFrame = player->getPreviousFrame();
        if(selectedFrame)selectedFrame--;
        showCurrentFrame();
    } 
    catch (IncorrectParamException &e)
    {
        IVCP_LOG_INFO(e.message);
    }
}

void onKey()
{
    char key;
    VideoPlaylistFrame frame;
    list <pair <ClassType, Rect> > regions;

    for (;;)
    {
        key = waitKey(0) % 255;
        switch (key)
        {
        case DEL:
            delPressed = true;
            break;
        case ESC:
            return;
        case LEFT: 
            delPressed = false;
            normalizeCurrent = 1;
            toPrevFrame();
            break;
        case RIGHT:
            delPressed = false;
            normalizeCurrent = 1;
            toNextFrame();            
            break;
        case N:
            if(normalizeImg)
            {
                normalizeCurrent = (normalizeCurrent + 1) % 3;
            }
            else
            {
                normalizeCurrent = (normalizeCurrent + 1) % 2;
            }
            showCurrentFrame();
            break;
        default:
            if ( key >= '0' && key <= '9')
            {
                char strKey[] = {key, '\0'};
                int intKey = atoi(strKey);
                if (delPressed)
                {
                    deleteRegionOfCurrentFrame(intKey);
                }
                else
                {
                    setClass(intKey);
                }
                delPressed = false;
            } 
        }
    }
}

void onMouse(int event, int x, int y, int, void* )
{
    switch (event)
    {
    case CV_EVENT_LBUTTONDOWN:
        if ( isClassBusy(classType) )
        {
            cout << "Class " << classType << " is already used" << endl;
        }
        else
        {
            xleft = xright = x;
            ytop = ybottom = y;
            tmpFrame = drawFrame.clone();
            isDraw = true;
            isMoved = false;
            imshow(window, tmpFrame);
        }
        break;
    case CV_EVENT_MOUSEMOVE:
        if (isDraw)
        {
            xright = x;
            ybottom = y;
            tmpFrame = drawFrame.clone();
            rectangle(tmpFrame, Point(xleft, ytop), Point(x, y), colors[classType]);
            isMoved = true;
            imshow(window, tmpFrame);
        }
        break;
    case CV_EVENT_LBUTTONUP:
        if (isDraw && isMoved)
        {
            tmpFrame = drawFrame.clone();
            Rect rect(Point(xleft, ytop), Point(xright, ybottom));
            rectangle(tmpFrame, rect, colors[classType]);
            rectangle(drawFrame, rect, colors[classType]);
            isDraw = false;
            isMoved = false;
            addRegionToCurrentFrame(rect);
            imshow(window, tmpFrame);
        }      
        break;
    }
}

void printHelp()
{
    cout << "Left, Right - Moving on video frames" << endl;
    cout << "Press n to select face normalization mode(original frame rotation, no rotation)" << endl;
    cout << "Press 0-9 to select class" << endl;
    cout << "To delete region of class <class_number>, press Del and then press <class_number>" << endl;
    cout << "Press Esc to save regions and exit from application" << endl;
    cout << "Colors of classes: " << endl;
    for (int i = 0; i < classCount; i++)
    {
        cout << "  " << i << " - " << colorNames[i] << endl;
    }
    cout << endl << endl;
    cout << "Current class: " << classType << endl;
}

int main(int argc, char **argv)
{
    ivcpInit();

    IVCP_LOG_TO_FILE("region_markup_log.txt");

    string input_rep = INPUT_REPOSITORY;
    string output_rep = OUTPUT_REPOSITORY;

    normalizeImg = false;
    normalizeCurrent = 1;

    if (argc == 3)
    {
        input_rep = argv[1];
        output_rep = argv[2];
    }
    else if (argc == 4 && strcmp(argv[1], "--normalize") == 0)
    {
        normalizeImg = true;
        input_rep = argv[2];
        output_rep = argv[3];
    }
    else
    {
        cout << "Using:" << endl;
        cout << "  ivcp_region_markup [--normalize] <path to input repository> <path to output repository>" << endl;
        return 0;
    }

    SPtr<IRepository> repository = Factory::getRepository(input_rep);
    SPtr<VideoPlaylist> playlist = repository->loadVideoPlaylist();
    player = Factory::createVideoPlayer();
    player->setPlaylist(playlist);

    gtRegionSource = repository->loadDataSource("gt_regions");
    if (gtRegionSource == NULL)
    {
        gtRegionSource = Factory::createRegionDataSource("gt_regions");
    }

    if(normalizeImg)
    {
		faceLocator = Factory::createDlibRegionCreator( DlibRegionCreator::Settings(REGION_FACE, 1.1f, 1.2f, true), "face_locator");
        faceLocator->setPlaylist( player->getPlaylist() );
    }

    if (player->haveNextFrame())
    { 
        if(normalizeImg)
        {
            SPtr<VideoPlaylist> plist = player->getPlaylist();
            cout << "Frame normalization" << endl;
            int frameCount = 0;
            for(int i = 0; i < plist->getNumberOfRanges(); i++)
            {
                cout << "Preparing range " << i << " of " << plist->getNumberOfRanges() << "\r";
                FrameRange rng = plist->getRangeByIndex(i).sourceRange;
                frameCount += rng.endFrame - rng.firstFrame;
            }

            int frameCurrent = 1;
            cout << "Processing frame " << frameCurrent << " of " << frameCount << "\r";
            Mat img = player->getNextFrame();
            VideoPlaylistFrame frame = plist->getCurrentFrame();
            auto faceRegion = faceLocator->getRegion(img);
            detectedTransformations.push_back(faceRegion.regionRotation);
            frameCurrent++;
            while(player->haveNextFrame())
            {
                cout << "Processing frame " << frameCurrent << " of " << frameCount << "\r";
                img = player->getNextFrame();
                faceRegion = faceLocator->getRegion(img);
                detectedTransformations.push_back(faceRegion.regionRotation);
                frameCurrent++;
            }
            selectedFrame = 0;
            player->seek(frame);
        }

        printHelp();        
        namedWindow(window, CV_WINDOW_AUTOSIZE);    
        startShowFrames();
        setMouseCallback(window, onMouse);
        onKey();        
        repository = Factory::getRepository(output_rep);
        repository->save(playlist);
        repository->save(gtRegionSource);
    } 
    else
    {
        cout << "Frames are missing" << endl;
    }
    return 0;
}