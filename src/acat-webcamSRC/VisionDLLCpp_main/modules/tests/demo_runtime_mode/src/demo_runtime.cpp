///////////////////////////////////////////////////////////////////////////
// <copyright file="demo_runtime.cpp" company="Intel Corporation">
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
#include "demo_environment.hpp"
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

namespace 
{
class MockVideoSource: public VideoSource
{
public:
    MockVideoSource( int _type, int _duration, int _count )
    {
        time_t t;
        rnd( (unsigned int)time(&t) );
        type = _type;
        duration = _duration;
        count = _count;
        current = -1;
        i = 0;
        j = 0;

        lsx = 0;
        lsy = 0;
        psx = 0;
        psy = 0;
        ldx = 0;
        ldy = 0;
        lfdx = 0;
        lfdy = 0;
    }

    virtual ~MockVideoSource() {}

    virtual Mat getNextFrame()
    {
        return generate_frame();
    }
                
    virtual void seek(int frame)
    {
        current = frame - 1;
    }

    virtual VideoSourceType getType()
    {
        return VIDEOFILE;
    }

    virtual void open()
    {
    }

    virtual void release()
    {
    }

protected:
    int type, duration, count, current;
    int i, j;
    RNG rnd;
    unsigned int lsx, lsy, psx, psy;
    int ldx, ldy;
    float lfdx, lfdy;

    //0 - random, 1 - point, 2 - line
    Mat generate_frame()
    {
        Mat img;
        current++;

        if (j < count)
        {
            int genline = (type == 0) ? (1 - j % 2) : (type - 1);
            if(genline)
            {
                if (i % duration == 0)
                {
                    lsx = rnd.uniform(60, 140);
                    lsy = rnd.uniform(60, 140);
                    ldx = rnd.uniform(20, 40);
                    ldy = rnd.uniform(20, 40);
                    int s = 1;
                    float lnrm = sqrt((float)ldy * ldy + ldx * ldx);
                    lfdx = (ldy * s / lnrm);
                    lfdy = (-ldx * s / lnrm);
                    i = 0;
                }
                unsigned int x = (int)(lsx + i*lfdx);
                unsigned int y = (int)(lsy + i*lfdy);
                img = Mat::zeros(200, 200, CV_8UC3);
                line(img, Point(x, y), Point(x + ldx, y + ldy), Scalar(255, 255, 255));
                i++;
            }
            else
            {
                if (i % duration == 0)
                {
                    psx = rnd.uniform(20, 180);
                    psy = rnd.uniform(20, 180);
                    i = 0;
                }
                float dx = 1;
                float dy = 1;
                unsigned int x = (int)(psx + i * dx);
                unsigned int y = (int)(psy + i * dy);
                img = Mat::zeros(200, 200, CV_8UC3);
                circle(img, Point(x, y), 2, Scalar(255, 255, 255));
                i++;
            }

            if (!img.empty() && img.channels() == 1)
            {
                cvtColor(img, img, cv::COLOR_GRAY2BGR);
            }

            if (i % duration == 0)
            {
                j++;
            }
        }
        return img;
    }
};
}

int main(int argc, char **argv)
{
    ivcpInit();

    string videoFile;
    if(argc == 2)
    {
        videoFile = argv[1];
    }
    else
    {
        videoFile = "captured_video.avi";
    }

    SPtr<TextPerformance> performance = Factory::createPerformance(TEXT_REPORT, "performance");
    SPtr<MockVideoSource> videoline = new MockVideoSource(2, 60, 100);
    SPtr<MockVideoSource> videopoint = new MockVideoSource(1, 60, 100);
    SPtr<MockVideoSource> videotest = new MockVideoSource(0, 60, 100);
    SPtr<VideoPlaylist> playlist = Factory::createVideoPlayer()->getPlaylist();
    FrameRange videorange;

    videorange.firstFrame = 0;
    videorange.endFrame = 600;
    playlist->addSourceRange(videoline, videorange);
    playlist->addSourceRange(videopoint, videorange);
    playlist->addSourceRange(videotest, videorange);

    SPtr<IRepository> repository = Factory::getRepository(SESSION_FILE);
    SPtr<Session> session = repository->loadSession();
    session->addPerformanceFilter(performance);

    session->setVideoPlaylist(playlist);
    SPtr<Pipeline> pipeline = Factory::createPipeline(RUNTIME_PIPELINE);
    pipeline->setSession(session);
    pipeline->connectAll();

    SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
    visualize->setWindowName("Pipeline runtime demo");
    pipeline->run();
    
    performance->generatePerformanceReport("runtime_performance.txt");

    if( visualize->haveVideoOutput() )
    {
        visualize->generateVideoOutput(videoFile, 0, Size(200, 200));
    }

    return 0;
}