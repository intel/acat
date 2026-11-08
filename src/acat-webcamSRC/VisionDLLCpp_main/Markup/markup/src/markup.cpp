///////////////////////////////////////////////////////////////////////////
// <copyright file="markup.cpp" company="Intel Corporation">
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

#include <ctime>
#include <deque>
#include "demo_environment.hpp"
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

int main(int argc, char **argv)
{
    ivcpInit();

    string dataFile, sessionFile;
    if(argc == 3)
    {
        sessionFile = argv[1];
        dataFile = argv[2];
    }
    else
    {
        cout << "Using:" << endl;
        cout << "  ivcp_markup <path to input repository> <path to output repository>" << endl;
        return 0;
    }
    cout << "Left, Right - Moving on video frames" << endl;
    cout << "Press 0-9 to input current frame class mark" << endl;
    cout << "Press backspace to delete last entered mark digit" << endl;
    cout << "Press c to clear entered mark and remain previous mark state intact" << endl;
    cout << "Press u to change region mark state to unmarked" << endl;
    cout << "Press Esc to save frame class marks and exit from application" << endl;

    SPtr<IRepository> repository = Factory::getRepository(sessionFile);
    SPtr<VideoPlaylist> playlist = repository->loadVideoPlaylist();
    SPtr<IVideoPlayer> player = Factory::createVideoPlayer();
    player->setPlaylist(playlist);

    SPtr<IClassifierResultDataSource> classRes = repository->loadClassifierResultDataSource();
    if(classRes.isEmpty())
    {
        classRes = Factory::createClassifierResultDataSource("class_marks");
    }

    bool run = true;
    VideoPlaylistFrame prev = VIDEO_PLAYLIST_FRAME_EOF, current = VIDEO_PLAYLIST_FRAME_EOF;
    while(run)
    {
        prev = current;
        Mat frame = player->getNextFrame();
        current = player->getPlaylist()->getCurrentFrame();
        if( current == VIDEO_PLAYLIST_FRAME_EOF)
        {
            if( prev == VIDEO_PLAYLIST_FRAME_EOF)
            {
                IVCP_LOG_WARNING("Provided playlist is empty");
                return -1;
            }
            player->seek(prev);
            frame = player->getNextFrame();
            current = prev;
        }

        if(frame.cols == 0 || frame.rows == 0)
        {
            //if there is no frame at this frame index skip to next
            continue;

        }
        int prevMark = classRes->getClassifyResult(current);
        int mark = 0;
        bool marked = false;
        bool marking = true;
        do
        {
            Mat drawImage = frame.clone();
            stringstream strPrev, strNew;

            strPrev << "Frame was marked as:";
            if(prevMark >= 0)
            {
                strPrev << prevMark;
            }
            else
            {
                strPrev << "Unmarked";
            }

            strNew  << "Mark frame as:";
            if(marked)
            {
                if(mark >= 0)
                {
                    strNew << mark;
                }
                else
                {
                    strNew << "Unmarked";
                }
            }
            else
            {
                strNew << "None";
            }

            putText(drawImage, strPrev.str(), Point(5, 15), FONT_ITALIC, 0.35, Scalar(255) );
            putText(drawImage, strNew.str(), Point(5, 30), FONT_ITALIC, 0.35, Scalar(255) );
            imshow("Markup", drawImage);
            int key = cvWaitKey(-1) % 255;
            switch(key)
            {
            case 48://numeric key pressed
            case 49:
            case 50:
            case 51:
            case 52:
            case 53:
            case 54:
            case 55:
            case 56:
            case 57:
                if(marked)
                    mark = mark * 10 + (key - 48);
                else
                {
                    mark = key - 48;
                    marked = true;
                }
                break;
            case 8://backspace
                mark /= 10;//clear last mark digit
                if(mark == 0)
                {
                    marked = false;
                }
                break;
            case 99://"c" pressed
                marked = false;//clear new mark to None
                break;
            case 39://right
                marking = false;
                if(marked)
                {
                    classRes->addClassifyResult(current,mark);
                }
                break;
            case 37://left
                marking = false;
                if(marked)
                {
                    classRes->addClassifyResult(current,mark);
                }
                player->getPlaylist()->getPreviousFrame();
                player->getPlaylist()->getPreviousFrame();
                break;
            case 27://esc
                marking = false;
                if(marked)
                {
                    classRes->addClassifyResult(current,mark);
                }
                run = false;
                break;
            case 117://"u" pressed
                mark = -1;//change clear mark to unknown state
                marked = true;
                break;
            default:
                break;
            }
        }
        while(marking);
    }

    repository = Factory::getRepository(dataFile);
    repository->save(classRes);
    repository->save(playlist);

    return 0;
}
