///////////////////////////////////////////////////////////////////////////
// <copyright file="textfile_performance.cpp" company="Intel Corporation">
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


#include <fstream>
#include <set>
#include "libivcp/performance/textfile_performance.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
void TextPerformance::generateQualityReport(string filename)
{
    ofstream report( filename );
    report.setf(ios_base::fixed);
    report.unsetf(ios_base::right);
    report.setf(ios_base::left);
    auto width = report.width();

    unsigned int matched = 0, compared = 0;
    map< pair<ClassType, ClassType>, unsigned int> matches;
    map<ClassType, unsigned int> predicts;
    map<ClassType, unsigned int> marks;

    //Not sure whether it is necessary
    for (auto knownClassIter1 = knownClasses.begin(); knownClassIter1 != knownClasses.end(); ++knownClassIter1 )
    {
        marks[*knownClassIter1] = 0;
        predicts[*knownClassIter1] = 0;
        for (auto knownClassIter2 = knownClasses.begin(); knownClassIter2 != knownClasses.end(); ++knownClassIter2 )
        {
            matches[pair<ClassType, ClassType>(*knownClassIter1, *knownClassIter2)] = 0;
        }
    }

    for (auto actClassIter = frameActualClass.begin(), estClassIter = frameEstimatedClass.begin(); actClassIter != frameActualClass.end(); ++actClassIter, ++estClassIter )
    {
        if(*actClassIter != NOT_CLASSIFIED)
        {
            ++compared;
            matches[pair<ClassType, ClassType>(*actClassIter, *estClassIter)]++;
            marks[*actClassIter]++;
            predicts[*estClassIter]++;
            if(*actClassIter == *estClassIter)
            {
                ++matched;
            }
        }
    }

    report << "Total precision (% correct): " << ((float)matched / compared) << "\n";
    report << "\n";
    report << "Results for each class:\n";
    report << "----------------------------------------------\n";
    report << "| Class Name   | Precision(%) | Recall(%)    |\n";
    report << "----------------------------------------------\n";

    for (auto knownClassIter = knownClasses.begin(); knownClassIter != knownClasses.end(); ++knownClassIter )
    {
        report << "| ";
        report.width(12);
        if (classNames[*knownClassIter] != "")
        {
            report << classNames[*knownClassIter];
        }
        else
        {
            report << *knownClassIter;
        }
        report.width(width);
        report << " | ";

        report.unsetf(ios_base::left);
        report.setf(ios_base::right);
        report.width(12);
        if (predicts[*knownClassIter] != 0)
        {
            report << ( (float)matches[pair<ClassType, ClassType>(*knownClassIter, *knownClassIter)] / predicts[*knownClassIter] );
        }
        else
        {
            report << 0.0;
        }
        report.width(width);
        report << " | ";
        report.width(12);

        if ( marks[*knownClassIter] != 0)
        {
            report << ( (float)matches[pair<ClassType, ClassType>(*knownClassIter, *knownClassIter)] / marks[*knownClassIter] );
        }
        else
        {
            report << "NA";
        }
        report.width(width);
        report.unsetf(ios_base::right);
        report.setf(ios_base::left);

        report << " |\n";
    }

    report << "----------------------------------------------\n";
    report << "\n";
    report << "Confusion matrix:\n";
    report << "--------------------------";

    for (auto knownClassIter = knownClasses.begin(); knownClassIter != knownClasses.end(); ++knownClassIter )
    {
        report << "---------------";
    }

    report << "\n";
    report << "|   Predicted Class Name |";

    for (auto knownClassIter = knownClasses.begin(); knownClassIter != knownClasses.end(); ++knownClassIter )
    {
        report << " ";
        report.width(12);
        if(classNames[*knownClassIter] != "")
        {
            report << classNames[*knownClassIter];
        }
        else
        {
            report << *knownClassIter;
        }
        report.width(width);
        report << " |";
    }
    
    report<< "\n";
    report << "| Correct Class Name     |";
    
    for (auto knownClassIter = knownClasses.begin(); knownClassIter != knownClasses.end(); ++knownClassIter )
    {
        report << "              |";
    }
    
    report << "\n";
    report << "--------------------------";
    
    for (auto knownClassIter = knownClasses.begin(); knownClassIter != knownClasses.end(); ++knownClassIter )
    {
        report << "---------------";
    }
    
    report << "\n";

    for (auto knownClassIter1 = knownClasses.begin(); knownClassIter1 != knownClasses.end(); ++knownClassIter1 )
    {
        report << "| ";
        report.width(12);

        if (classNames[*knownClassIter1] != "")
        {
            report << classNames[*knownClassIter1];
        }
        else
        {
            report << *knownClassIter1;
        }

        report.width(width);
        report << "           |";
        report.unsetf(ios_base::left);
        report.setf(ios_base::right);

        for (auto knownClassIter2 = knownClasses.begin(); knownClassIter2 != knownClasses.end(); ++knownClassIter2 )
        {
            report << " ";
            report.width(12);
            report << ( (float)matches[pair<ClassType, ClassType>(*knownClassIter1, *knownClassIter2)] / marks[*knownClassIter1] );
            report.width(width);
            report << " |";
        }

        report.unsetf(ios_base::right);
        report.setf(ios_base::left);
        report << "\n";
    }
    report << "--------------------------";

    for (auto knownClassIter = knownClasses.begin(); knownClassIter != knownClasses.end(); ++knownClassIter )
    {
        report << "---------------";
    }
    report << "\n";
}

void TextPerformance::generatePerformanceReport(string filename)
{
    ofstream report( filename );
    report.setf(ios_base::fixed);
    report.unsetf(ios_base::right);
    report.setf(ios_base::left);
    auto width = report.width();

    report << "------------------------------------------------------------------------------------\n";
    report << "| Metric name                    | Avg time (ms) | Max time (ms) | Total time (ms) |\n";
    report << "------------------------------------------------------------------------------------\n";

    for (auto timerIter = timerStat.begin(); timerIter != timerStat.end(); ++timerIter)
    {
        report << "| ";
        report.width(30);
        report << timerIter->first;
        report.width(width);
        report << " | ";

        report.unsetf(ios_base::left);
        report.setf(ios_base::right);
        report.width(13);
        report << getAverageTime(timerIter->first);
        report.width(width);
        report << " | ";
        report.width(13);
        report << getMaxTime(timerIter->first);
        report.width(width);
        report << " | ";
        report.width(15);
        report << getTotalTime(timerIter->first);
        report.width(width);
        report.unsetf(ios_base::right);
        report.setf(ios_base::left);

        report << " |\n";
    }
    report << "------------------------------------------------------------------------------------\n";
}

PerformanceType TextPerformance::getType()
{
    return TEXT_REPORT;
}
};
