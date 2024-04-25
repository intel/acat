## Official release of ACAT 3.10 is now available [here](https://github.com/intel/acat/releases)

## Background
The Assistive Context-Aware Toolkit (ACAT) is an open-source platform created at Intel Labs. It is designed to enhance communication for individuals with restricted speech and typing capabilities. It achieves this by providing functionalities like keyboard simulation, word/sentence prediction, and speech synthesis.

ACAT was originally developed by researchers in Intel Labs for Professor Stephen Hawking. He was instrumental to the design process and was a key contributor to the project design and validation. After deploying the system to Professor Hawking, Intel turned its attention to the larger community and continued to make ACAT more configurable to support a larger set of users.

By making this configurable platform open source, the goal is to encourage developers  to continue to expanding its capabilities by adding new user interfaces, new sensing modalities, language prediction, and other features.

ACAT is developed in C# using Microsoft Visual Studio* 2022 and .NET 4.8.1. It runs on Windows® 10 (20H2 or higher) and 11.

## Versioning
The master branch on Github will always contain the most recent release of ACAT.

## Cloning the repo
This repo uses Large File Storage (LFS) and includes submodules. You may run into problems if you use GitHub Desktop to clone it. As a workaround, use Git from the command line.

Clone the repo:  
**$ git clone https://github.com/intel/acat.git**

If you are cloning for the first time:  
**$ git submodule update --init --recursive**

Pull in all the latest changes for the submodules:  
**$ git pull origin master --recurse-submodules**

## License
ACAT is distributed under the Apache License, Version 2.0.

## Project Website
Click [here](https://www.intel.com/content/www/us/en/developer/tools/open/acat/overview.html) for the ACAT project website.

## Contact the team
Click <a href="mailto:ACAT@intel.com">here</a> to contact the ACAT team.

