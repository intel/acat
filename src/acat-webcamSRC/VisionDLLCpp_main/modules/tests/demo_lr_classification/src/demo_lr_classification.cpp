///////////////////////////////////////////////////////////////////////////
// <copyright file="demo_lr_classification.cpp" company="Intel Corporation">
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


#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

static void lrTest()
{
	// Data for visual representation
	int width = 512, height = 512;
	Mat image = Mat::zeros(height, width, CV_8UC3);


    SPtr<IClassifier> lr = Factory::createLRClassifier();
    
	const size_t nSamples = 4;
    const size_t featureSize = 2;

	


    // Set up training data
    int labels[nSamples] = {1, 1, 1, 0};
    float trainingData[nSamples][featureSize] = { {501.0f, 10.0f}, {255.0f, 10.0f}, {501.0f, 255.0f}, {10.0f, 501.0f} };
    for (size_t i = 0; i < nSamples; i++)
    {
        Mat trainingDataMat(1, featureSize, CV_32FC1, &trainingData[i]);
        lr->addTrainFeature(trainingDataMat, labels[i]);
    }

    // Train the logistic regression classifier
    lr->trainModel();
    Vec3b green(0, 255, 0), blue(255, 0, 0);
    
    // Show the decision regions given by the logistic regression classifier
    for (int i = 0; i < image.rows; i++)
    {
        for (int j = 0; j < image.cols; j++)
        {
            Mat sampleMat = (Mat_<float>(1,2) << i,j);
            int response  = lr->clasify(sampleMat);
            image.at<Vec3b>(i, j) = green;
            if (response == 0)
            {
                image.at<Vec3b>(i, j) = blue;
            }
        }
    }

    // Show the training data
    int thickness = -1;
    int lineType = 8;
    for (size_t i = 0; i < nSamples; i++)
    {
        int color = labels[i] * 255;
        circle(image, Point((int)trainingData[i][0], (int)trainingData[i][1]), 5, Scalar_<int>(color, color, color), thickness, lineType);
    }

    imshow("Logistic regression classification demo", image);
    waitKey(0);
}

int main(int argc, char **argv)
{
    (void)argc;
    (void)argv;

    ivcpInit();

    lrTest();

    return 0;
}
