///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_params.cpp" company="Intel Corporation">
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

#include "libivcp/classifier/svm_params.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
SVMParameters::SVMParameters(SVMType svmType, SVMKernelType kernelType, int degree, double gamma, 
    double coef0, double epsilon, double C, double nu, double p, 
    int weightCount, vector<int> weightLabels, vector<double> weights,
    bool useShrinking, double cache)
    : svmType(svmType),
      kernelType(kernelType),
      degree(degree),
      gamma(gamma),
      coef0(coef0),
      cacheSize(cache),
      eps(epsilon),
      C(C),
      nu(nu),
      p(p),
      weightCount(weightCount),
      weightLabels(weightLabels),
      weights(weights),
      useShrinking(useShrinking),
      estimateProbability(true) { }
};