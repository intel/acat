///////////////////////////////////////////////////////////////////////////
// <copyright file="demo_emotion.cpp" company="Intel Corporation">
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

#include <stdio.h>
#include <time.h>
#include "demo_environment.hpp"
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

enum TypesOfRegion
{
    LEFT_EYE,
    RIGHT_EYE,
    MOUTH,
    FACE
};

const Scalar BLUE(255, 0, 0);
const Scalar GREEN(0, 255, 0);
const Scalar RED(0, 0, 255);
const Scalar BLACK(0, 0, 0);

Mat drawImage;
string window;
SPtr<IClassifierResultDataSource> markup;
SPtr<const VideoPlaylist> demoPlaylist;
SPtr<IRegionCreator> lEyeRegs;
SPtr<IRegionCreator> rEyeRegs;
SPtr<IRegionCreator> mouthRegs;
SPtr<IRegionCreator> faceRegs;
VideoPlaylistFrame currentFrame;

#define USE_REAL_CAMERA
#ifndef USE_REAL_CAMERA
#define TEST_VIDEO_SOURCE_TYPE CV_16UC1
#endif

#ifdef TEST_VIDEO_SOURCE_TYPE
class FakeVideoSource: public VideoSource
{
public:
    /*!
     * \brief Default constructor
     */
    FakeVideoSource(SPtr<VideoSource> _videoSource, int _outputType): 
        videoSource(_videoSource), outputType(_outputType)
    {
    }

    virtual ~FakeVideoSource()
    {
        release();
    }

    virtual Mat getNextFrame()
    {
        Mat matOrig = videoSource->getNextFrame();
        
        if (matOrig.type() == outputType)
        {
            return matOrig;
        }

        Mat mat;
        switch (outputType)
        {
        case CV_8UC1:
            if (matOrig.type() == CV_8UC3)
            {
                cvtColor(matOrig, mat, cv::COLOR_BGR2GRAY);
                break;
            }

            if (matOrig.type() == CV_16UC1)
            {
                matOrig.convertTo(mat, CV_8UC1, 1.0/16.0);
                break;
            }

            mat = matOrig;
            break;

        case CV_8UC3:
            if (matOrig.type() == CV_8UC1)
            {
                cvtColor(matOrig, mat, cv::COLOR_GRAY2BGR);
                break;
            }

            if (matOrig.type() == CV_16UC1)
            {
                matOrig.convertTo(mat, CV_8UC1, 1.0/16.0);
                cvtColor(mat, mat, cv::COLOR_GRAY2BGR);
                break;
            }

            mat = matOrig;
            break;

        case CV_16UC1:
            if (matOrig.type() == CV_8UC1)
            {
                matOrig.convertTo(mat, CV_16UC1, 16.0);
                break;
            }

            if (matOrig.type() == CV_8UC3)
            {
                cvtColor(matOrig, mat, cv::COLOR_BGR2GRAY);
                mat.convertTo(mat, CV_16UC1, 16.0);
                break;
            }

            mat = matOrig;
            break;
        default:
            return mat = matOrig;
            break;
        }

        return mat;
    }

    virtual void seek(int frame)
    {
        videoSource->seek(frame);
    }

    virtual VideoSourceType getType()
    {
        return videoSource->getType();
    }

    virtual void open()
    {
        videoSource->open();
    }
    
    virtual void release() 
    {
        videoSource->release();
    }

    SPtr<VideoSource> videoSource;
    int outputType;
};


void replaceVideoSource(SPtr<VideoPlaylist> playlist)
{
    vector<VideoPlaylistRange> ranges;
    vector<SPtr<VideoSource> > sources;

    //Set sources
    for (int i = 0; i < playlist->getNumberOfSources(); i++)
    {
        sources.push_back(new FakeVideoSource(playlist->getSourceByIndex(i), TEST_VIDEO_SOURCE_TYPE));
    }

    //Set ranges
    for (int i = 0; i < playlist->getNumberOfRanges(); i++)
    {
        ranges.push_back(playlist->getRangeByIndex(i));
    }

    playlist->clear();

    //Add ranges
    for (int i = 0; i < ranges.size(); i++)
    {
        playlist->addSourceRange(sources[ranges[i].sourceIndex], ranges[i].sourceRange);
    }

 }
#endif

void drawRegions(Mat mat)
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
        rectangle(drawImage, region.regionFrame, RED);
    }

    if (!rEyeRegs.isEmpty())
    {
        region = rEyeRegs->getRegion(mat);
        if(transpose && region.regionRotation.rows == 3 && region.regionRotation.cols == 3)
        {
            Mat trDraw(drawImage.rows, drawImage.cols, CV_8UC3);
            warpPerspective(drawImage, trDraw, transpose, trDraw.size(), 0);
            drawImage = trDraw;
            transpose = false;
        }
        rectangle(drawImage, region.regionFrame, GREEN);
    }

    if (!mouthRegs.isEmpty())
    {
        region = mouthRegs->getRegion(mat);
        if(transpose && region.regionRotation.rows == 3 && region.regionRotation.cols == 3)
        {
            Mat trDraw(drawImage.rows, drawImage.cols, CV_8UC3);
            warpPerspective(drawImage, trDraw, transpose, trDraw.size(), 0);
            drawImage = trDraw;
            transpose = false;
        }
        rectangle(drawImage, region.regionFrame, BLUE);
    }

    if (!faceRegs.isEmpty())
    {
        region = faceRegs->getRegion(mat);
        if(transpose && region.regionRotation.rows == 3 && region.regionRotation.cols == 3)
        {
            Mat trDraw(drawImage.rows, drawImage.cols, CV_8UC3);
            warpPerspective(drawImage, trDraw, transpose, trDraw.size(), 0);
            drawImage = trDraw;
        }
        rectangle(drawImage, region.regionFrame, BLACK);
    }
}

void sourceCallback(Mat mat, void* param)
{
    (void)param;

    currentFrame = demoPlaylist->getCurrentFrame();
    drawImage = mat.clone();

    if( markup->isClassified(currentFrame) )
    {
        switch( markup->getClassifyResult(currentFrame) )
        {
        case 0:
            putText(drawImage, "mark: blink", Point(5, 15), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
            break;
        case 1:
            putText(drawImage, "mark: smile", Point(5, 15), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
            break;
        default:
            putText(drawImage, "mark: unknown", Point(5, 15), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
            break;
        }
    }
    
    drawRegions(mat);
};

void classifierCallback(Mat mat, void* param)
{
    (void)param;

    if( markup->isClassified(currentFrame) )
    {
        ClassType callData = (ClassType)mat.at<float>(0, 0);
        switch(callData)
        {
        case 0:
            putText(drawImage, "pred: blink", Point(5, 30), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
            break;
        case 1:
            putText(drawImage, "pred: smile", Point(5, 30), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
            break;
        default:
            putText(drawImage, "pred: unknown", Point(5, 30), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
            break;
        }
    }

    imshow(window, drawImage);
    cv::waitKey(1);    
};

void addMarkingRegionsToSession(SPtr<Session> session)
{
    SPtr<IRegionDataSource> regions = session->getDataSource("gt_regions");
    SPtr<IRegionCreator> leftEyes = Factory::createGTRegionCreator(regions, LEFT_EYE, "leftEyes");
    SPtr<IRegionCreator> rightEyes = Factory::createGTRegionCreator(regions, RIGHT_EYE, "rightEyes");
    SPtr<IRegionCreator> mouths = Factory::createGTRegionCreator(regions, MOUTH, "mouths");

    session->addRegionCreator(leftEyes);
    session->addRegionCreator(rightEyes);
    session->addRegionCreator(mouths);

    lEyeRegs = leftEyes;
    rEyeRegs = rightEyes;
    mouthRegs = mouths;
}

void addFeatureToSession(SPtr<Session> session, FeatureType fType)
{
    SPtr<Feature> feature, tmpFeature;
    SPtr<ConcatenatorFeature> concatenator;
    SPtr<IClassifier> internalClassifier;
    SPtr<IRepository> rep;
    switch (fType)
    {
    case NORMALFLOW:
        feature = Factory::createNormalFlowFeature(0.1f, "feature");
        break;
    case STLBP:
        feature = Factory::createSTLBPFeature(STLBPFeature::Settings(3, 1, 1, 1, 4, 4, 4, 4, 4), "feature");
        break;
    case CONCATENATOR:
        feature = Factory::createConcatenatorFeature(4, "feature");
        ((SPtr<ConcatenatorFeature>) feature)->setFeature(Factory::createNormalFlowFeature(0.1f));
        break;
    case AGGREGATOR:
        feature = Factory::createAggregatorFeature(3, 1, "feature");
        tmpFeature = Factory::createConcatenatorFeature(3);
        ((SPtr<ConcatenatorFeature>) tmpFeature)->setFeature(Factory::createNormalFlowFeature(0.1f));
        ((SPtr<AggregatorFeature>) feature)->addFeature(tmpFeature);
        ((SPtr<AggregatorFeature>) feature)->addFeature(Factory::createSTLBPFeature(STLBPFeature::Settings(3, 1, 1, 1, 4, 4, 4, 4, 4)));
        break;
    case CLASSIFIER_FEATURE:
        tmpFeature = Factory::createSTLBPFeature(STLBPFeature::Settings(3, 1, 1, 1, 4, 4, 4, 4, 4));
        feature = Factory::createClassifierFeature(session->getNumberOfRegionCreators(), PROBABILISTIC, "feature");
        rep = Factory::getRepository("stlbp_model.xml");
        internalClassifier = rep->loadClassifier();
        internalClassifier->setId("internal_classifier");
        ((SPtr<ClassifierFeature>) feature)->setFeature(tmpFeature);
        ((SPtr<ClassifierFeature>) feature)->setClassifier(internalClassifier);
        break;
    case SUMMATOR:       
        feature = Factory::createSummatorFeature(SummatorFeature::Settings(5, true), "feature");
        ((SPtr<SummatorFeature>) feature)->setFeature(Factory::createSTLBPFeature(STLBPFeature::Settings(3, 1, 1, 1, 4, 4, 4, 4, 4)));
        break;
    default: 
        IVCP_EXCEPTION(IncorrectParamException, "Unknown feature type");
    }

    session->addFeature(feature);
}

SPtr<Session> createSession(string sessionFile, FeatureType fType, SPtr<Classifier> classifier)
{
    SPtr<IRepository> repository = Factory::getRepository(sessionFile);
    SPtr<Session> session = repository->loadSession();

    addMarkingRegionsToSession(session);

    SPtr<IClassifierResultDataSource> classifier_result = repository->loadDataSource("class_res");
    session->addClassifier(classifier);
    session->addDataSource(classifier_result);

    addFeatureToSession(session, fType);

    return session;
}

void releaseRegions()
{
    lEyeRegs.release();
    rEyeRegs.release();
    mouthRegs.release();
}

void trainEmotionClassification(FeatureType fType, string modelFile)
{
    SPtr<Classifier> classifier = Factory::createClassifier(ivcp::SVM, "classifier");
    SPtr<Session> session = createSession(TRAIN_SESSION_FILE, fType, classifier);
#ifdef TEST_VIDEO_SOURCE_TYPE
    replaceVideoSource(session->getVideoPlaylist());
#endif
         
    SPtr<Pipeline> pipeline = new Pipeline(session, TRAIN_PIPELINE);

    pipeline->addLink("player", "mouths");
    pipeline->addLink("player", "leftEyes");
    pipeline->addLink("player", "rightEyes");
    pipeline->addLink("leftEyes", "feature");
    pipeline->addLink("rightEyes", "feature");
    pipeline->addLink("mouths", "feature");
    pipeline->addLink("feature", "classifier");
    pipeline->addLink("class_res", "classifier");

    pipeline->run();    

    //Just in case we already have this file
    remove(modelFile.c_str());

    SPtr<IRepository> trainedClassifierRepo = Factory::getRepository(modelFile);
    trainedClassifierRepo->save(classifier);
    releaseRegions();
}

void testEmotionClassification(FeatureType fType, string modelFile, string perfFile, string qltyFile)
{
    SPtr<IRepository> trainedClassifierRepo = Factory::getRepository(modelFile);
    SPtr<Classifier> classifier = trainedClassifierRepo->loadClassifier();

    SPtr<Session> session = createSession(TEST_SESSION_FILE, fType, classifier);
#ifdef TEST_VIDEO_SOURCE_TYPE
    replaceVideoSource(session->getVideoPlaylist());
#endif

    SPtr<Performance> performance = Factory::createPerformance(TEXT_REPORT, "performance");

    demoPlaylist = session->getVideoPlaylist();
    markup = session->getDataSource("class_res"); 

    session->addPerformanceFilter(performance);

    SPtr<Pipeline> pipeline = new Pipeline(session, TEST_PIPELINE);

    pipeline->addLink("player", "mouths");
    pipeline->addLink("player", "leftEyes");
    pipeline->addLink("player", "rightEyes");
    pipeline->addLink("leftEyes", "feature");
    pipeline->addLink("rightEyes", "feature");
    pipeline->addLink("mouths", "feature");
    pipeline->addLink("feature", "classifier");
    pipeline->addLink("class_res", "classifier");

    pipeline->addLink("feature", "performance");
    pipeline->addLink("classifier", "performance");
    pipeline->addLink("class_res", "performance");
    for (int i = 0; i < session->getNumberOfRegionCreators(); i++)
    {
        SPtr<IRegionCreator> region = session->getRegionCreator(i);
        region->setPlaylist(demoPlaylist);
        pipeline->addLink(region->getId(), "performance");
    }

    pipeline->addNodeCallback("player", &sourceCallback, NULL);
    pipeline->addNodeCallback("classifier", &classifierCallback, NULL);
    pipeline->run();

    performance->generatePerformanceReport(perfFile);
    performance->generateQualityReport(qltyFile);

    releaseRegions();
}

int main(int argc, char **argv)
{
    (void)argc;
    (void)argv;

    ivcpInit();
    IVCP_LOG_TO_FILE("demo_emotion_log.txt");

    try
    {
        window = "NormalFlow feature";
        cout << "Used feature: NormalFlow" << endl;
        cout << "Training..." << endl;
        trainEmotionClassification(NORMALFLOW, "nf_model.xml");
        cout << "Test..." << endl;
        testEmotionClassification(NORMALFLOW, "nf_model.xml", "nf_perf_rep.txt", "nf_qlty_rep.txt");
        cout << "Test finished" << endl << endl;
        destroyWindow(window);

        window = "STLBP feature";
        cout << "Used feature: STLBP" << endl;
        cout << "Training..." << endl;
        trainEmotionClassification(STLBP, "stlbp_model.xml");
        cout << "Test..." << endl;
        testEmotionClassification(STLBP, "stlbp_model.xml", "stlbp_perf_rep.txt", "stlbp_qlty_rep.txt");
        cout << "Test finished" << endl << endl;
        destroyWindow(window);

        window = "Concatenator feature";
        cout << "Used feature: Concatenator (NormalFlow, 3 frames)" << endl;
        cout << "Training..." << endl;
        trainEmotionClassification(CONCATENATOR, "concat_model.xml");
        cout << "Test..." << endl;
        testEmotionClassification(CONCATENATOR, "concat_model.xml", "concat_perf_rep.txt", "concat_qlty_rep.txt");
        cout << "Test finished" << endl << endl;
        destroyWindow(window);

        window = "Aggregator feature";
        cout << "Used feature: Aggregator (Concatenator with NormalFlow + STLBP)" << endl;
        cout << "Training..." << endl;
        trainEmotionClassification(AGGREGATOR, "aggr_model.xml");
        cout << "Test..." << endl;
        testEmotionClassification(AGGREGATOR, "aggr_model.xml", "aggr_perf_rep.txt", "aggr_qlty_rep.txt");
        cout << "Test finished" << endl << endl;
        destroyWindow(window);

        window = "Classifier feature";
        cout << "Used feature: Classifier (with STLBPFeature and classifier from stlbp_model.xml)" << endl;
        cout << "Training..." << endl;
        trainEmotionClassification(CLASSIFIER_FEATURE, "classifier_model.xml");
        cout << "Test..." << endl;
        testEmotionClassification(CLASSIFIER_FEATURE, "classifier_model.xml", "classifier_perf_rep.txt", "classifier_qlty_rep.txt");
        cout << "Test finished" << endl << endl;
        destroyWindow(window);

        window = "Summator feature";
        cout << "Used feature: Summator (with STLBP, 5 regions, countMode = true)" << endl;
        cout << "Training..." << endl;
        trainEmotionClassification(SUMMATOR, "summator_model.xml");
        cout << "Test..." << endl;
        testEmotionClassification(SUMMATOR, "summator_model.xml", "summator_perf_rep.txt", "summator_qlty_rep.txt");
        cout << "Test finished" << endl << endl;
        destroyWindow(window);
    }
    catch (exception &e)
    {
        cout << endl << "Error: " << e.what() << endl;
        getchar();
    }

    return 0;
}

