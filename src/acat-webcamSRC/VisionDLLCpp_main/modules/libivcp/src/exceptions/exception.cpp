///////////////////////////////////////////////////////////////////////////
// <copyright file="exception.cpp" company="Intel Corporation">
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
#include "libivcp/exceptions/exception.hpp"

using namespace std;

namespace ivcp
{
Exception::Exception():
    message(""), functionName("NA"), fileName("NA"), lineNumber(0)
{
}

Exception::Exception(const string& msg, const string& func, const string& file, int line):
    message(msg), functionName(func), fileName(file), lineNumber(line) 
{
}

Exception::~Exception()
{
}

string Exception::getMessage() const
{
    stringstream msgStream;

    msgStream << fileName << ":" << lineNumber << " " << message << " in function " << functionName << "\n";

    return msgStream.str();
}

const char* Exception::what() const
{
    return message.c_str();
}
}
