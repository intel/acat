///////////////////////////////////////////////////////////////////////////
// <copyright file="performance.cpp" company="Intel Corporation">
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


#include "libivcp/performance/performance.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
Performance::Performance():
    classMarkup(NULL)
{
}

void Performance::startTimer(string timerName)
{
    lock_guard<mutex> lk(m);
    activeTimers[timerName] = (double)getTickCount();
}

void Performance::stopTimer(string timerName)
{
    lock_guard<mutex> lk(m);
    auto iter = activeTimers.find(timerName);
    if( iter != activeTimers.end() )
    {
        timerStat[timerName].push_back( pair<double, double>( iter->second, (double)getTickCount() ) );
        activeTimers.erase(iter);
    }
}

void Performance::addClassifyResult(VideoPlaylistFrame frame, ClassType classType)
{
    frames.push_back(frame);
    frameEstimatedClass.push_back(classType);
    knownClasses.insert(classType);

    if( classMarkup && classMarkup->isClassified( frame ) )
    {
        frameActualClass.push_back( classMarkup->getClassifyResult( frame ) );
        knownClasses.insert( classMarkup->getClassifyResult( frame ) );
    }
    else
    {
        if (NULL == classMarkup)
        {
            IVCP_EXCEPTION(IncorrectParamException, "No Classification Result Data Source is set");
        }

        frameActualClass.push_back( NOT_CLASSIFIED );
    }

}

void Performance::setClassName(ClassType classType, string typeName)
{
    classNames[classType] = typeName;
}

void Performance::setMarkup(SPtr<IClassifierResultDataSource> markup) 
{
    classMarkup = markup;
}

bool Performance::hasMarkup()
{
    return (classMarkup != NULL);
}

FilterType Performance::getRootType() const
{
    return PERFORMANCE_FILTER;
}

map<ClassType, string> &Performance::getClassNamesMap()
{
    return classNames;
}

SPtr<IClassifierResultDataSource> Performance::getClassMarkup()
{
    return classMarkup;
}

void Performance::clear()
{
    activeTimers.clear();
    timerStat.clear();
    frames.clear();
    frameEstimatedClass.clear();
    frameActualClass.clear();
    knownClasses.clear();
    classNames.clear();
    classMarkup = NULL;
}

vector<pair <double, double> > Performance::getTimerStatForFilter(string filterId)
{
    for (auto timer = timerStat.begin(); timer != timerStat.end(); timer++)
    {
        if (timer->first == filterId)
        {
            return timer->second;
        }
    }
    return vector<pair <double, double > > ();
}

double Performance::getAverageTime(string filterId)
{
    auto timerTimes = getTimerStatForFilter(filterId);
    if (!timerTimes.empty())
    {
        double timeSum = 0;
        int ticksCount = 0;
        for (auto iter = timerTimes.begin(); iter != timerTimes.end(); iter++)
        {
            timeSum += (iter->second - iter->first);
            ticksCount++;
        }
        return ( (timeSum / ticksCount) / getTickFrequency() * 1000 );
    }

    return 0;
}

double Performance::getMaxTime(string filterId)
{   
    auto timerTimes = getTimerStatForFilter(filterId);
    if (!timerTimes.empty())
    {
        double max = 0;
        for (auto iter = timerTimes.begin(); iter != timerTimes.end(); iter++)
        {
            double time = (iter->second - iter->first);
            if (time > max)
            {
                 max = time;
            }
        }
       return ( max / getTickFrequency() * 1000 );
    }

    return 0;
}

double Performance::getTotalTime(string filterId)
{
    auto timerTimes = getTimerStatForFilter(filterId);
    if (!timerTimes.empty())
    {
        double timeSum = 0;
        for (auto iter = timerTimes.begin(); iter != timerTimes.end(); iter++)
        {
            timeSum += (iter->second - iter->first);
        }
        return ( timeSum / getTickFrequency() * 1000 );
    }

    return 0;
}

float Performance::getTotalPrecision()
{
    unsigned int matched = 0, compared = 0;
    for (auto actClassIter = frameActualClass.begin(), estClassIter = frameEstimatedClass.begin(); actClassIter != frameActualClass.end(); actClassIter++, estClassIter++ )
    {
        if (*actClassIter != NOT_CLASSIFIED)
        {
            compared++;
            if (*actClassIter == *estClassIter)
            {
                matched++;
            }
        }
    }
    if (compared > 0)
    {
        return ((float)matched / compared);
    }
    else
    {
        return 0;
    }
}

float Performance::getPrecisionByClass(ClassType cl)
{
    unsigned int totalCount = 0;
    unsigned int correctCount = 0;
    for (auto actClassIter = frameActualClass.begin(), estClassIter = frameEstimatedClass.begin(); actClassIter != frameActualClass.end(); ++actClassIter, ++estClassIter )
    {
        if(*estClassIter == cl)
        {
            totalCount++;
            if(*actClassIter == *estClassIter)
            {
                correctCount++;
            }
        }
    }
    if (totalCount > 0)
    {
        return ((float)correctCount / totalCount);
    }
    else 
    {
        return 0;
    }
}

float Performance::getRecallByClass(ClassType cl)
{
    unsigned int totalCount = 0;
    unsigned int correctCount = 0;
    for (auto actClassIter = frameActualClass.begin(), estClassIter = frameEstimatedClass.begin(); actClassIter != frameActualClass.end(); ++actClassIter, ++estClassIter )
    {
        if (*actClassIter == cl)
        {
            totalCount++;
        }
        if(*estClassIter == cl && *actClassIter == *estClassIter)
        {
            correctCount++;
        }
    }
    if (totalCount > 0)
    {
        return ((float)correctCount / totalCount);
    }
    else
    {
        return 0;
    }
}

map<pair<ClassType, ClassType>, float> Performance::getConfusionMap()
{
    map< pair<ClassType, ClassType>, unsigned int> matches;
    map<ClassType, unsigned int> marks;

    for (auto actClassIter = frameActualClass.begin(), estClassIter = frameEstimatedClass.begin(); actClassIter != frameActualClass.end(); ++actClassIter, ++estClassIter )
    {
        if(*actClassIter != NOT_CLASSIFIED)
        {
            matches[pair<ClassType, ClassType>(*actClassIter, *estClassIter)]++;
            marks[*actClassIter]++;
        }
    }

    map<pair<ClassType, ClassType>, float> confusionMap;
    for (auto knownClassIter1 = knownClasses.begin(); knownClassIter1 != knownClasses.end(); knownClassIter1++)
    {
        for (auto knownClassIter2 = knownClasses.begin(); knownClassIter2 != knownClasses.end(); knownClassIter2++)
        {
            pair<ClassType, ClassType> classPair(*knownClassIter1, *knownClassIter2);
            confusionMap[classPair] = ( (float)matches[classPair] / marks[*knownClassIter1] );
        }
    }

    return confusionMap;
}

};
