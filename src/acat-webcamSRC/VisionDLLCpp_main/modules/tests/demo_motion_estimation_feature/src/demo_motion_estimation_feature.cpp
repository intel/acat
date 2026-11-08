///////////////////////////////////////////////////////////////////////////
// <copyright file="demo_motion_estimation_feature.cpp" company="Intel Corporation">
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


#include <time.h>
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

//#define USE_MOTION_ESTIMATION_MEAN_FEATURE // Uncomment this line to test MotionEstimationMeanFeature

namespace
{

SPtr<IRegionCreator> lEyeRegs;
SPtr<IRegionCreator> rEyeRegs;
Mat drawImage;
string windowName;
SPtr<const VideoPlaylist> demo_playlist;
const int nSequences = 2;

// Classifier stub class, that simply compares feature value for each region with corresponding threshold.
// If feature value is greater than threshold, then it classifies as motion, otherwise not.
// NOTE: throwing exceptions and checking data correctness are omitted for simplicity.
class MotionDetectionClassifier: public Classifier
{
public:
    MotionDetectionClassifier(const vector<float>& regionThresholds)
    {
        this->regionsThreshold = regionThresholds;
    }

    virtual vector<float> getDistribution(Mat feature) { (void)feature; return vector<float>(); }
    virtual void addTrainFeature(Mat feature, ClassType response) { (void)feature; (void)response; }
    virtual void trainModel() { }
    virtual ClassifierType getType() { return NEW_CLASSIFIER; }
    virtual bool isTrained() { return true; }
    virtual int getVariablesCount() { return nSequences; }
    virtual int getClassCount() { return 2; };

#ifndef SS_2023
    virtual void setClassifierParameters(const char*, double) { }
#endif

protected:
    virtual ClassType clasify(Mat feature)
    {
        return (ClassType)isHasMotion(feature);
    }

private:
    vector<float> regionsThreshold;
    enum { NO_MOTION = 0, HAS_MOTION };

    int isHasMotion(Mat feature)
    {
        int motionCount = 0;
        for (int i = 0; i < feature.cols; i++)
        {
            if (feature.at<float>(i) > regionsThreshold[i])
            {
                motionCount++;
            }
        }

        return motionCount == 0 ? NO_MOTION : HAS_MOTION;
    }
};

// Motion estimation feature, based on mean value form region intersection.
// This class demonstrates a way to use own algorithm implementation for motion estimation feature
class MotionEstimationMeanFeature: public MotionEstimationFeature
{
public:
    MotionEstimationMeanFeature(int sequenceCount = 1)
        : MotionEstimationFeature(sequenceCount) { }

    virtual ~MotionEstimationMeanFeature() { };

    virtual FeatureType getType() { return NEW_FEATURE; }; // FeatureType should be extended with new feature name, 
                                                           // so replace NEW_FEATURE with new feature name

protected:
    virtual float estimate(const vector<Mat> &regions) // Own new algorithm should be implemented in this method
    {
        Mat diff;
        absdiff(regions[0], regions[1], diff);
        Scalar mean;
        Scalar stddev;
        meanStdDev(diff, mean, stddev);
        return (float)mean(0);
    }
};

static void drawRegions(Mat mat)
{
    IRegionCreator::RegionDescription region;
    bool transpose = false;

    if (!lEyeRegs.isEmpty())
    {
        region = lEyeRegs->getRegion(mat);
        if(transpose && region.regionRotation.rows == 3 && region.regionRotation.cols == 3)
        {
            Mat trDraw(drawImage.rows, drawImage.cols, CV_8UC3);
            warpPerspective(drawImage, trDraw, transpose, trDraw.size(), 0);
            drawImage = trDraw;
            transpose = false;
        }
        rectangle(drawImage, region.regionFrame, Scalar(0, 0, 255));
    }

    if (!rEyeRegs.isEmpty())
    {
        region = rEyeRegs->getRegion(mat);
        if(transpose && region.regionRotation.rows == 3 && region.regionRotation.cols == 3)
        {
            Mat trDraw(drawImage.rows, drawImage.cols, CV_8UC3);
            warpPerspective(drawImage, trDraw, transpose, trDraw.size(), 0);
            drawImage = trDraw;
        }
        rectangle(drawImage, region.regionFrame, Scalar(0, 255, 0));
    }
}

static void sourceCallback(Mat mat, void* param)
{
    (void)param;

    drawImage = mat.clone();
    drawRegions(drawImage);
}

static void classifierCallback(Mat mat, void* param)
{
    (void)param;

    ClassType isHasMotion = (ClassType)mat.at<float>(0, 0);
    int delayInMs = 33;
    if (isHasMotion)
    {
        putText(drawImage, "HAS MOTION", Point(60, 60), FONT_ITALIC, 1, Scalar(255), 2);
    }
    imshow(windowName, drawImage);
    waitKey(delayInMs);
}
}

int main()
{
    ivcpInit();
    SPtr<Session> session = Factory::createSession("session");
    SPtr<VideoPlayer> videoPlayer = Factory::createVideoPlayer();
    videoPlayer->setId("player");
    SPtr<VideoPlaylist> playlist = videoPlayer->getPlaylist();

	SPtr<IVideoSource> source = 
        Factory::createVideofileVideoSource("res\\blink\\blink_7.avi");

	if (source->getOutputResolution().width < 1280 || source->getOutputResolution().height < 720)
    {
        cout << "Video file need to be at least 1280x720" << endl;
        return -2;
    }
    playlist->addSourceRange(source, FrameRange(0, END_FRAME));
    session->setVideoPlaylist(playlist);

    lEyeRegs = new BBoxRegionCreator(530, 280, 40, 40);
    lEyeRegs->setPlaylist(playlist);
    rEyeRegs = new BBoxRegionCreator(660, 280, 40, 40);
    rEyeRegs->setPlaylist(playlist);

    lEyeRegs->setId("regionLEye");
    session->addRegionCreator(lEyeRegs);
    rEyeRegs->setId("regionREye");
    session->addRegionCreator(rEyeRegs);
    
#ifndef USE_MOTION_ESTIMATION_MEAN_FEATURE
    SPtr<MotionEstimationStdDevFeature> motionStdDevFeature = new MotionEstimationStdDevFeature(nSequences);
    motionStdDevFeature->setId("mestddev");
    session->addFeature(motionStdDevFeature);
#else
    SPtr<MotionEstimationMeanFeature> motionMeanFeature = new MotionEstimationMeanFeature(nSequences);
    motionMeanFeature->setId("memean");
    session->addFeature(motionMeanFeature);
#endif

    vector<float> thresholds;
    thresholds.push_back(15);
    thresholds.push_back(7.5);
    SPtr<MotionDetectionClassifier> classifier = new MotionDetectionClassifier(thresholds);
    session->addClassifier(classifier);

    SPtr<Pipeline> pipeline = Factory::createPipeline(RUNTIME_PIPELINE);
    pipeline->setSession(session);
    pipeline->connectAll();

    windowName = "Motion estimation feature demo";
    namedWindow(windowName, cv::WINDOW_AUTOSIZE);
    demo_playlist = session->getVideoPlaylist();
    pipeline->addNodeCallback(videoPlayer->getId(), &sourceCallback, NULL);
    pipeline->addNodeCallback(classifier->getId(), &classifierCallback, NULL);
    pipeline->run();

    destroyWindow(windowName);
    return 0;
}
