///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_model.cpp" company="Intel Corporation">
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

#include "libivcp/classifier/svm_model.hpp"

using namespace std;
using namespace cv;
using namespace libsvm;

namespace ivcp
{
SVMModel::SVMModel()
{
    model = NULL;
}

SVMModel::~SVMModel()
{
    clear();
}

void SVMModel::clear()
{
    if (model != NULL)
    {
        delete model;
    }
}

SPtr<const SVMParameters> SVMModel::getParams()
{
    if (model == NULL)
    {
        return NULL;
    }
    else
    {
        SPtr<SVMParameters> res = new SVMParameters(model->params);
        return res;
    }
}

int SVMModel::getSupportVectorsCount()
{
    if (model == NULL)
    {
        return 0;
    }
    else
    {
        return model->sv_count;
    }
}

vector<double> SVMModel::getSupportVector(int i)
{
    vector<double> sv;
    if (model == NULL || i >= model->sv_count)
    {
        return sv;
    }
    else
    {
        sv.resize(model->SV[i].size());
        for(int j = 0; j < model->SV[i].size(); j++)
        {
            sv[j] = model->SV[i].data[j];
        }
        return sv;
    }
}

int SVMModel::getVarCount()
{
    if (model == NULL || model->sv_count == 0)
    {
        return 0;
    }
    else
    {
        return model->SV[0].size();
    }
}

vector<int> SVMModel::getClassLabels()
{
    vector<int> labels;
    if (model != NULL && model->labels != NULL)
    {
        for (int i = 0; i < model->class_count; i++)
        {
            labels.push_back(model->labels[i]);
        }
    }
    sort(labels.begin(), labels.end());
    return labels;
}

int SVMModel::getClassCount()
{
    if (model == NULL || model->sv_count == 0)
    {
        return 0;
    }
    else
    {
        return model->class_count;
    }
}


void SVMModel::setLibSvmModel(libsvm::LibSVMModel* newModel)
{
    this->model = newModel;
}

bool SVMModel::checkProbabilityModel(const LibSVMModel* candidateModel)
{
    return ((candidateModel->params.svmType == C_SVC || candidateModel->params.svmType == NU_SVC) && candidateModel->probA != NULL && candidateModel->probB != NULL) ||
        ((this->model->params.svmType == EPSILON_SVR || this->model->params.svmType == NU_SVR) && this->model->probA != NULL);
}

void SVMModel::checkParams(SVMTrainData &trainData, SVMParameters &params)
{
    int svmType = params.svmType;
    if (svmType != C_SVC &&
       svmType != NU_SVC &&
       svmType != ONE_CLASS &&
       svmType != EPSILON_SVR &&
       svmType != NU_SVR)
    {
        trainData.release();
        IVCP_EXCEPTION(IncorrectParamException, "Unknown SVM type");
    }
        
    int kernel_type = params.kernelType;
    if(kernel_type != LINEAR &&
       kernel_type != POLY &&
       kernel_type != RBF &&
       kernel_type != SIGMOID)
    {
        trainData.release();
        IVCP_EXCEPTION(IncorrectParamException, "Unknown kernel type");
    }

    if (kernel_type == POLY || kernel_type == RBF || kernel_type == SIGMOID)
    {
        if(params.gamma < 0)
        {
            trainData.release();
            IVCP_EXCEPTION(IncorrectParamException, "Parameter gamma < 0");
        }
    }

    if (kernel_type == POLY)
    {
        if (params.degree < 0)
        {
            trainData.release();
            IVCP_EXCEPTION(IncorrectParamException,  "Parameter degree of polynomial kernel < 0");
        }
    }

    if (params.cacheSize <= 0)
    {
        trainData.release();
        IVCP_EXCEPTION(IncorrectParamException,  "Parameter cacheSize <= 0");
    }

    if(params.eps <= 0)
    {
        trainData.release();
        IVCP_EXCEPTION(IncorrectParamException, "Parameter eps <= 0");
    }

    if(svmType == C_SVC || svmType == EPSILON_SVR || svmType == NU_SVR)
    {
        if (params.C <= 0)
        {
            trainData.release();
            IVCP_EXCEPTION(IncorrectParamException, "Parameter C <= 0");
        }
    }

    if (svmType == C_SVC)
    {
        if (params.weightCount != (int)params.weightLabels.size())
        {
            trainData.release();
            IVCP_EXCEPTION(IncorrectParamException, "Size of weightLabels is not equal to weightCount");
        }
        if (params.weightCount != (int)params.weights.size())
        {
            trainData.release();
            IVCP_EXCEPTION(IncorrectParamException, "Size of weights is not equal to weightCount");
        }
    }

    if (svmType == NU_SVC || svmType == ONE_CLASS || svmType == NU_SVR)
    {
        if (params.nu <= 0 || params.nu > 1)
        {
            trainData.release();
            IVCP_EXCEPTION(IncorrectParamException, "Parameter nu <= 0 or nu > 1");
        }
    }

    if (svmType == EPSILON_SVR)
    {
        if (params.p < 0)
        {
            trainData.release();
            IVCP_EXCEPTION(IncorrectParamException,  "Parameter p < 0");
        }
    }

    if (svmType == NU_SVC)
    {
        int max_class_count = 16;
        int class_count = 0;
        int *label = new int[max_class_count];
        int *count = new int[max_class_count];

        for (unsigned int i = 0; i < trainData.size(); i++)
        {
            int this_label = (int)trainData.responses[i];
            int j;
            for ( j = 0; j < class_count; j++)
                if (this_label == label[j])
                {
                    ++count[j];
                    break;
                }
            if(j == class_count)
            {
                if(class_count == max_class_count)
                {
                    int* tmp1 = new int[max_class_count * 2];
                    int* tmp2 = new int[max_class_count * 2];
                    for (int p = 0; p < max_class_count; p++)
                    {
                        tmp1[p] = label[p];
                        tmp2[p] = count[p];
                    }
                    delete[] label;
                    delete[] count;
                    label = tmp1;
                    count = tmp2;
                    max_class_count *= 2;
                }
                label[class_count] = this_label;
                count[class_count] = 1;
                ++class_count;
            }
        }
    
        for (int i = 0; i < class_count; i++)
        {
            int n1 = count[i];
            for (int j = i + 1; j < class_count; j++)
            {
                int n2 = count[j];
                if (params.nu * (n1 + n2) / 2 > min(n1, n2))
                {
                    delete[] label;
                    delete[]count;
                    trainData.release();
                    IVCP_EXCEPTION(IncorrectParamException, "Specified nu is infeasible");
                }
            }
        }
        delete[] label;
        delete[] count;
    }
}

void SVMModel::train(std::list<cv::Mat> features, std::vector<double> responses, SVMParameters params)
{
    clear();	

    SVMTrainData trainData((int)responses.size());
    trainData.set(features, responses);
    params.estimateProbability = true;

    checkParams(trainData, params);
    
    srand(1);
    model = svmTrain(&trainData, &params);

    trainData.release();
}

double SVMModel::predict(const cv::Mat &feature)
{
    if (model == NULL)
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }
    if (model->sv_count == 0 || model->SV == NULL)
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }
    if (feature.rows != 1)
    {
        IVCP_EXCEPTION(MismatchFeatureSizeException, "Number of feature matrix rows is not 1");
    }
    int varCount = model->SV[0].size();
    if (feature.cols != varCount)
    {
        stringstream message;
        message << "Feature size is mismatch (expected: 1 row, " << varCount << " columns)";
        IVCP_EXCEPTION(MismatchFeatureSizeException, message.str());
    }

    SVMVector featureVector(varCount);
    for (int i = 0; i < varCount; i++)
    {
        featureVector.data[i] = feature.row(0).at<float>(i);
    }
    double pred = svmPredict(model, featureVector);
    featureVector.release();
    return pred;
}

vector<float> SVMModel::getProbabilities(Mat feature)
{
    if (model == NULL || model->sv_count == 0 || model->SV == NULL)
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }
    if (!checkProbabilityModel(model))
    {
        IVCP_EXCEPTION(NoModelException, "Unable to get probabilities from this model");
    }

    if (feature.rows != 1)
    {
        IVCP_EXCEPTION(MismatchFeatureSizeException, "Number of feature matrix rows is not 1");
    }
    int varCount = model->SV[0].size();
    if (feature.cols != varCount)
    {
        stringstream message;
        message << "Feature size is mismatch (expected: 1 row, " << varCount << " columns)";
        IVCP_EXCEPTION(MismatchFeatureSizeException, message.str());
    }

    SVMVector featureVector(varCount);
    for (int i = 0; i < varCount; i++)
    {
        featureVector.data[i] = feature.row(0).at<float>(i);
    }

    double* prob = new double[model->class_count];
    svmPredictProbability(model, featureVector, prob);
    featureVector.release();

    vector<int> sortLabels = getClassLabels();

    vector<float> probVector(model->class_count);
    for (int i = 0; i < model->class_count; i++)
    {
        int ind = 0;
        while (ind < model->class_count && model->labels[ind] != sortLabels[i])
        {
            ind++;
        }

        probVector[i] = (float) prob[ind];
    }
    delete[] prob;

    return probVector;
}

void SVMModel::write(FileStorage &fs, const string &name)
{
    if (model == NULL)
    {
        IVCP_EXCEPTION(NoModelException, "SVMModel is not yet trained");
    }
    fs.startWriteStruct(name.c_str(), FileNode::MAP, "svm-model");
    switch (model->params.svmType)
    {
    case C_SVC:
        fs << "svm_type" << "C_SVC";
        break;
    case NU_SVC:
        fs << "svm_type" << "NU_SVC";
        break;
    case ONE_CLASS:
        fs << "svm_type" << "ONE_CLASS";
        break;
    case EPSILON_SVR:
        fs << "svm_type" << "EPSILON_SVR";
        break;
    case NU_SVR:
        fs << "svm_type" << "NU_SVR";
        break;
    }    
    
    fs.startWriteStruct("kernel", FileNode::MAP);
    switch (model->params.kernelType)
    {
    case LINEAR:
        fs << "type" << "LINEAR";
        break;
    case POLY:
        fs << "type" << "POLY";
        break;
    case RBF:
        fs << "type" << "RBF";
        break;
    case SIGMOID:
        fs << "type" << "SIGMOID";
    }  

    if (model->params.kernelType == POLY)
    {
        fs << "degree" << model->params.degree;
    }
    if (model->params.kernelType != LINEAR)
    {
        fs << "gamma" << model->params.gamma;
    }
    if (model->params.kernelType == POLY || model->params.kernelType == SIGMOID)
    {
        fs << "coef0" << model->params.coef0;
    }
    fs.endWriteStruct();  

    fs << "epsilon" << model->params.eps;

    fs << "class_count" << model->class_count; 
    if (model->class_count > 0)
    {         
        if (model->labels != NULL)
        {
            fs.startWriteStruct("class_labels", FileNode::SEQ + FileNode::FLOW);
            fs.writeRaw("i", model->labels, static_cast<size_t>(model->class_count) * sizeof(int));
            fs.endWriteStruct();
        }
    }      

    fs << "sv_count" << model->sv_count;
    if (model->sv_count > 0)
    {    
        fs << "var_count" << model->SV[0].size();
        fs.startWriteStruct("support_vectors", FileNode::SEQ);
        for (int i = 0; i < model->sv_count; i++)
        {
            fs.startWriteStruct(String(), FileNode::SEQ + FileNode::FLOW);
            fs.writeRaw("d", model->SV[i].data, static_cast<size_t>(model->SV[i].size()) * sizeof(double));
            fs.endWriteStruct();
        }
        fs.endWriteStruct();
    }

    if (model->numSVForClasses != NULL)
    {
        fs.startWriteStruct("sv_count_for_classes", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("i", model->numSVForClasses, static_cast<size_t>(model->class_count) * sizeof(int));
        fs.endWriteStruct();
    }
    
    int combCount = model->class_count * (model->class_count - 1) / 2;

    fs.startWriteStruct("decision_functions", FileNode::MAP);
    if (model->rho != NULL)
    {
        fs.startWriteStruct("rho", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("d", model->rho, static_cast<size_t>(combCount) * sizeof(double));
        fs.endWriteStruct();
    }
    if (model->sv_coef != NULL)
    {
        fs.startWriteStruct("sv_coef", FileNode::SEQ);
        for (int i = 0; i < model->class_count - 1; i++)
        {
            fs.startWriteStruct(String(), FileNode::SEQ + FileNode::FLOW);
            fs.writeRaw("d", model->sv_coef[i], static_cast<size_t>(model->sv_count) * sizeof(double));
            fs.endWriteStruct();
        }
        fs.endWriteStruct();
    }
    fs.endWriteStruct();

    fs.startWriteStruct("probability_model", FileNode::MAP);
    if (model->probA != NULL)
    {
        fs.startWriteStruct("prob_a", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("d", model->probA, static_cast<size_t>(combCount) * sizeof(double));
        fs.endWriteStruct();
    }
    if (model->probB != NULL)
    {
        fs.startWriteStruct("prob_b", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("d", model->probB, static_cast<size_t>(combCount) * sizeof(double));
        fs.endWriteStruct();
    }
    fs.endWriteStruct();
    fs.endWriteStruct();
}

void SVMModel::save(const string &filename, const string &name)
{
    FileStorage fs(filename, FileStorage::WRITE);  
    write(fs, name);
    fs.release();
}

void SVMModel::read(FileStorage &fs, FileNode fn)
{
    clear();
    model = new LibSVMModel();

    FileNode tmpNode = fn["svm_type"];
    if (tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Tag svm_type is not found");
    }
    string type = "";
    tmpNode >> type;
    int svmType =
        type == "C_SVC" ? C_SVC :
        type == "NU_SVC" ? NU_SVC :
        type == "ONE_CLASS" ? ONE_CLASS :
        type == "EPSILON_SVR" ? EPSILON_SVR :
        type == "NU_SVR" ? NU_SVR : -1;
    if (svmType < 0)
    {
        IVCP_EXCEPTION(IOException, "Invalid SVM type");
    }

    model->params.svmType = (SVMType) svmType;

    FileNode kernel = fn["kernel"];
    if (kernel.empty())
    {
        IVCP_EXCEPTION(IOException, "SVM kernel is not found");
    }
    tmpNode = kernel["type"];
    if (tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Kernel type is not found");
    }
    type = "";
    tmpNode >> type;
    int kernelType = 
        type == "LINEAR" ? LINEAR :
        type == "POLY" ? POLY :
        type == "RBF" ? RBF :
        type == "SIGMOID" ? SIGMOID : -1;
    if (kernelType < 0)
    {
        IVCP_EXCEPTION(IOException, "Invalid SVM kernel type");
    }
    model->params.kernelType = (SVMKernelType)kernelType;
    model->params.degree = (kernel["degree"].empty() ? static_cast<int>(0) : static_cast<int>(kernel["degree"]));
    model->params.gamma = (kernel["gamma"].empty() ? static_cast<double>(0) : static_cast<double>(kernel["gamma"]));
    model->params.coef0 = (kernel["coef0"].empty() ? static_cast<double>(0) : static_cast<double>(kernel["coef0"]));

    model->params.eps = (fn["epsilon"].empty() ? static_cast<double>(FLT_EPSILON) : static_cast<double>(fn["epsilon"]));
    model->class_count = (fn["class_count"].empty() ? static_cast<int>(0) : static_cast<int>(fn["class_count"]));
    model->sv_count = (fn["sv_count"].empty() ? static_cast<int>(0) : static_cast<int>(fn["sv_count"]));
    int var_count = (fn["var_count"].empty() ? static_cast<int>(0) : static_cast<int>(fn["var_count"]));
    if (model->class_count < 0 || model->sv_count <= 0 || var_count <= 0)
    {
        IVCP_EXCEPTION(IOException, "Invalid SVM model, check class_count, sv_count and var_count");
    }

    tmpNode = fn["class_labels"];
    if (model->class_count > 0 && tmpNode.empty())
    {
        if (model->params.svmType != EPSILON_SVR && model->params.svmType != NU_SVR)
        {
            IVCP_EXCEPTION(IOException, "Class labels are missing");
        }
    }
    if (model->class_count > 0 && !tmpNode.empty())
    {
        model->labels = new int[model->class_count];
        tmpNode.readRaw("i", model->labels, static_cast<size_t>(model->class_count) * sizeof(int)); 
    }

    tmpNode = fn["support_vectors"];
    if (tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Support vectors are missing");
    }
    model->SV = new SVMVector[model->sv_count];
    int i = 0;
    for (auto iter = tmpNode.begin(); iter != tmpNode.end(); iter++, i++)
    {
        model->SV[i].resize(var_count);
        (*iter).readRaw("d", model->SV[i].data, static_cast<size_t>(var_count) * sizeof(double));
    }

    tmpNode = fn["sv_count_for_classes"];
    if (model->class_count > 0 && tmpNode.empty())
    {
        if (model->params.svmType != EPSILON_SVR && model->params.svmType != NU_SVR)
        {
            IVCP_EXCEPTION(IOException, "The numbers of support vectors for each classes are missing");
        }
    }
    if (model->class_count > 0 && !tmpNode.empty())
    {
        model->numSVForClasses = new int[model->class_count];
        tmpNode.readRaw("i", model->numSVForClasses, static_cast<size_t>(model->class_count) * sizeof(int));
    }

    FileNode dfNode = fn["decision_functions"];
    if (dfNode.empty())
    {
        IVCP_EXCEPTION(IOException, "Tag decision_function is missing");
    }
    tmpNode = dfNode["rho"];
    if (tmpNode.empty())
    {
        IVCP_EXCEPTION(IOException, "rho is missing");
    }
    int combCount = model->class_count * (model->class_count-1) / 2;
    model->rho = new double[combCount];
    tmpNode.readRaw("d", model->rho, static_cast<size_t>(combCount) * sizeof(double));
    tmpNode = dfNode["sv_coef"];
    if (model->class_count > 1)
    {
        if (tmpNode.empty())
        {
            IVCP_EXCEPTION(IOException, "Tag sv_coef is missing");
        }
        model->sv_coef = new double*[model->class_count - 1];
        i = 0;
        for (auto iter = tmpNode.begin(); iter != tmpNode.end(); iter++, i++)
        {
            model->sv_coef[i] = new double[model->sv_count];
            (*iter).readRaw("d", model->sv_coef[i], static_cast<size_t>(model->sv_count) * sizeof(double));
        }
    }

    FileNode probNode = fn["probability_model"];
    if (!probNode.empty())
    {
        tmpNode = probNode["prob_a"];
        if (!tmpNode.empty())
        {
            model->probA = new double[combCount];
            tmpNode.readRaw("d", model->probA, static_cast<size_t>(combCount) * sizeof(double));
        }
        tmpNode = probNode["prob_b"];
        if (!tmpNode.empty())
        {
            model->probB = new double[combCount];
            tmpNode.readRaw("d", model->probB, static_cast<size_t>(combCount) * sizeof(double));
        }
    }
}

void SVMModel::load(const string &filename, const string &name)
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

};