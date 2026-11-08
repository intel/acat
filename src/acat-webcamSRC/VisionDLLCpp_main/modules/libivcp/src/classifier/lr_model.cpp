///////////////////////////////////////////////////////////////////////////
// <copyright file="lr_model.cpp" company="Intel Corporation">
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

#include "libivcp/classifier/lr_model.hpp"

using namespace std;
using namespace cv;
using namespace liblr;

namespace ivcp
{
LRModel::LRModel(): model_(NULL)
{
}

LRModel::~LRModel()
{
    // Do not need clear anything
}

bool LRModel::isTrained()
{
    return (NULL == model_ ? false : true);
}
    
SPtr<const LRParameters> LRModel::getParams()
{
    if ( !isTrained() )
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }
    
    SPtr<LRParameters> res = new LRParameters(model_->param);
    return res;
}

int LRModel::getVarCount()
{
    if ( !isTrained() )
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }

    return model_->nr_feature;
}

int LRModel::getClassCount()
{
    if ( !isTrained() )
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }

    return model_->nr_class;
}

vector<int> LRModel::getClassLabels()
{
    if ( !isTrained() )
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }

    vector<int> labels;
    for (int i = 0; i < model_->nr_class; i++)
    {
        labels.push_back(model_->label[i]);
    }
    sort(labels.begin(), labels.end());
    return labels;
}

void LRModel::setModel(const SPtr<LibLRModel>& model)
{
    model_ = model;
}

void LRModel::train(std::list<cv::Mat> features, std::vector<double> responses, LRParameters params)
{
    SPtr<LRTrainData> trainData = new LRTrainData();
    trainData->set(features, responses);

    checkParams(params);

    model_ = lrTrain(trainData, &params);
}

double LRModel::predict(const cv::Mat &feature)
{
    if ( !isTrained() )
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }
    if (feature.rows != 1)
    {
        IVCP_EXCEPTION(MismatchFeatureSizeException, "Number of feature matrix rows is not 1");
    }
    
    int varCount = model_->nr_feature;
    if (feature.cols != varCount)
    {
        stringstream message;
        message << "Feature size is mismatch (expected: 1 row, " << varCount << " columns)";
        IVCP_EXCEPTION(MismatchFeatureSizeException, message.str());
    }

    vector<double> featureVector(varCount);
    for (int i = 0; i < varCount; i++)
    {
        featureVector[i] = feature.row(0).at<float>(i);
    }
	double pred = lrPredict(model_, featureVector,model_->bias);

    return pred;
}

vector<float> LRModel::getProbabilities(Mat feature)
{
    if (model_ == NULL)
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }
    if (feature.rows != 1)
    {
        IVCP_EXCEPTION(MismatchFeatureSizeException, "Number of feature matrix rows is not 1");
    }
    int varCount = model_->nr_feature;
    if (feature.cols != varCount)
    {
        stringstream message;
        message << "Feature size is mismatch (expected: 1 row, " << varCount << " columns)";
        IVCP_EXCEPTION(MismatchFeatureSizeException, message.str());
    }

    vector<double> featureVector(varCount);
    for (int i = 0; i < varCount; i++)
    {
        featureVector[i] = feature.row(0).at<float>(i);
    }

    double* prob = new double[model_->nr_class];
	lrPredictProbability(model_, featureVector, model_->bias, prob);


    vector<int> sortLabels = getClassLabels();

    size_t numClasses = model_->nr_class;
    vector<float> probVector(numClasses);
    for (size_t i = 0; i < numClasses; i++)
    {
        size_t ind = 0;
        while (ind < numClasses && model_->label[ind] != sortLabels[i])
        {
            ind++;
        }

        probVector[i] = (float) prob[ind];
    }

    delete[] prob;
    return probVector;
}

void LRModel::write(FileStorage &fs, const string &name)
{
    if (model_ == NULL)
    {
        IVCP_EXCEPTION(NoModelException, "LRModel is not yet trained");
    }
    fs.startWriteStruct(name.c_str(), FileNode::MAP, "lr-model");
    switch (model_->param.solver_type)
    {
    case L2R_LR:
        fs<< "lr_type" << "L2R_LR";
        break;
    case L1R_LR:
        fs<< "lr_type" << "L1R_LR";
        break;
    case L2R_LR_DUAL:
        fs<< "lr_type" << "L2R_LR_DUAL";
        break;
    }    
    
    fs << "epsilon" << model_->param.eps;
    fs << "C" << model_->param.C;
    fs << "weight_count" << model_->param.nr_weight;
    if ( (model_->param.nr_weight != 0) && (NULL != model_->param.weight_label) && (NULL != model_->param.weight) )
    {
        fs.startWriteStruct("weight_labels", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("i", model_->param.weight_label, static_cast<size_t>(model_->param.nr_weight) * sizeof(int));
        fs.endWriteStruct();
        fs.startWriteStruct("weights", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("d", model_->param.weight, static_cast<size_t>(model_->param.nr_weight) * sizeof(double));
        fs.endWriteStruct();
    }
    
    fs << "class_count" << model_->nr_class;
    fs << "feature_size" << model_->nr_feature;
    fs << "bias" << model_->bias;

    if (model_->w != NULL)
    {
        fs.startWriteStruct("beta_parameters", FileNode::SEQ + FileNode::FLOW);
        if (model_->nr_class > 2)
        {
            fs.writeRaw("d", model_->w, static_cast<size_t>(model_->nr_class * (model_->nr_feature + (model_->bias < 0 ? 0 : 1))) * sizeof(double));
        }
        else
        {
            fs.writeRaw("d", model_->w, static_cast<size_t>(model_->nr_feature + (model_->bias < 0 ? 0 : 1)) * sizeof(double));
        }
        fs.endWriteStruct();
    }
    
    if (model_->nr_class > 0)
    {         
        if (model_->label != NULL)
        {
            fs.startWriteStruct("class_labels", FileNode::SEQ + FileNode::FLOW);
            fs.writeRaw("i", model_->label, static_cast<size_t>(model_->nr_class) * sizeof(int));
            fs.endWriteStruct();
        }
    }      

    fs.endWriteStruct();
}

void LRModel::save(const string &filename, const string &name)
{
    FileStorage fs(filename, FileStorage::WRITE);  
    write(fs, name);
    fs.release();
}

void LRModel::read(FileStorage &fs, FileNode fn)
{
    model_ = new LibLRModel();

    FileNode tmpNode = fn["lr_type"];
    if (tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Tag lr_type is not found");
    }
    string type = "";
    tmpNode >> type;
    int lrType =
        type == "L2R_LR" ? L2R_LR :
        type == "L1R_LR" ? L1R_LR :
        type == "L2R_LR_DUAL" ? L2R_LR_DUAL : -1;
    if (lrType < 0)
    {
        IVCP_EXCEPTION(IOException, "Invalid LR type");
    }

    model_->param.solver_type = (LRType) lrType;

    model_->param.eps = (fn["epsilon"].empty() ? static_cast<double>(FLT_EPSILON) : static_cast<double>(fn["epsilon"]));
    
    model_->param.C = (fn["C"].empty() ? static_cast<double>(FLT_EPSILON) : static_cast<double>(fn["C"]));
    
    model_->param.nr_weight = (fn["weight_count"].empty() ? static_cast<int>(0) : static_cast<int>(fn["weight_count"]));
    
    tmpNode = fn["weight_labels"];
    if ( (model_->param.nr_weight != 0) && tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Class labels are missing");
    }
    if ( (model_->param.nr_weight != 0) && !tmpNode.empty())
    {
        model_->param.weight_label = new int[model_->param.nr_weight];
        tmpNode.readRaw("i", model_->param.weight_label, static_cast<size_t>(model_->param.nr_weight) * sizeof(int));
    }
    
    tmpNode = fn["weights"];
    if ( (model_->param.nr_weight != 0) && tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Class labels are missing");
    }
    if ( (model_->param.nr_weight != 0) && !tmpNode.empty())
    {
        model_->param.weight = new double[model_->param.nr_weight];
        tmpNode.readRaw("d", model_->param.weight, static_cast<size_t>(model_->param.nr_weight) * sizeof(double)); 
    }

    model_->nr_class = (fn["class_count"].empty() ? static_cast<int>(0) : static_cast<int>(fn["class_count"]));
    
    model_->nr_feature = (fn["feature_size"].empty() ? static_cast<int>(0) : static_cast<int>(fn["feature_size"]));

    model_->bias = (fn["bias"].empty() ? static_cast<double>(FLT_EPSILON) : static_cast<double>(fn["bias"]));

    tmpNode = fn["beta_parameters"];
    if (tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Beta parameters are missing");
    }
    int w_size;
    if (model_->nr_class > 2)
    {
        w_size = model_->nr_class*(model_->nr_feature + (model_->bias < 0 ? 0 : 1));
    }
    else
    {
        w_size = model_->nr_feature + (model_->bias < 0 ? 0 : 1);
    }
    model_->w = new double[w_size];
    tmpNode.readRaw("d", model_->w, static_cast<size_t>(w_size) * sizeof(double));

    tmpNode = fn["class_labels"];
    if (model_->nr_class != 0 && tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Beta parameters are missing");
    }
    if (model_->nr_class != 0 && !tmpNode.empty())
    {
        model_->label = new int[model_->nr_class];
        tmpNode.readRaw("i", model_->label, static_cast<size_t>(model_->nr_class) * sizeof(int));
    }
}

void LRModel::load(const string &filename, const string &name)
{
    FileStorage fs(filename, FileStorage::READ);
    if (!fs.isOpened())
    {
        IVCP_EXCEPTION(IOException, "Unable to open file with model");
    }
    FileNode root;
    if (name.empty())
    {
        root = fs.getFirstTopLevelNode();
    }
    else
    {
        root = fs[name];
    }
    if (root.empty())
    {
        stringstream message;
        message << "FileNode " << name << " is not found";
        IVCP_EXCEPTION(IOException, message.str());
    }
    read(fs, root);
    fs.release();
}

void LRModel::checkParams(LRParameters &params)
{
    const char * errorMessage = lrCheckParameter(&params);
    if ( NULL != errorMessage )
    {
        IVCP_EXCEPTION(IncorrectParamException, errorMessage);
    }
}

};
