///////////////////////////////////////////////////////////////////////////
// <copyright file="log.cpp" company="Intel Corporation">
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



#include <sstream>
#include <fstream>
#include <stdarg.h>
#include "libivcp/log.hpp"

using namespace std;

namespace ivcp
{
string Log::logFile = "";
bool Log::writeToFile = false;
LogLevel Log::verbosityLevel = LOG_INFO;

void Log::log(LogLevel level, const string &msg, const string &func, const string &file, int line, ...)
{
    if (level < verbosityLevel)
    {
        return;
    }
    stringstream msgStream;
    switch (level)
    {
    case LOG_INFO:
        msgStream << "INFO:";
        break;
    case LOG_WARNING:
        msgStream << "WARNING:";
        break;
    case LOG_ERROR:
        msgStream << "ERROR:";
        break;
    default:
        msgStream << "UNKNOWN:";
    }

    va_list args;
    va_start(args, line);
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    int len = _vscprintf(msg.c_str(), args) + 1; 
#else
    int len = vsnprintf(NULL, 0, msg.c_str(), args) + 1; 
#endif
    va_end(args);
    va_list args1;
    va_start(args1, line);
    char* message = new char[len];
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    vsnprintf_s(message, len, _TRUNCATE, msg.c_str(), args1);
#else
    vsnprintf(message, len, msg.c_str(), args1);
#endif
    va_end(args1);

    msgStream << " " << message << " in " << func << " " << file << ":" << line <<"\n";
    delete[] message;

    if (writeToFile)
    {
        writeMessageToFile(msgStream.str());
    }
    else
    {
        writeMessageToConsole(msgStream.str());
    }
}

void Log::logToFile(const std::string &file)
{
    writeToFile = true;
    logFile = file;
}

void Log::logToConsole()
{
    logFile = "";
    writeToFile = false;
}

void Log::writeMessageToConsole(const string &msg)
{
    cout << msg;
}

void Log::writeMessageToFile(const string &msg)
{
    ofstream file(logFile, ios_base::app);
    file << msg;
    file.close();
}

void Log::setVerbosityLevel(LogLevel level)
{
    verbosityLevel = level;
}
}