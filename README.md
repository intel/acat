## Official release of ACAT is available [here](https://github.com/intel/acat/releases)

[![Build](https://github.com/intel/acat/actions/workflows/build.yml/badge.svg)](https://github.com/intel/acat/actions/workflows/build.yml)
[![Tests](https://github.com/intel/acat/actions/workflows/test.yml/badge.svg)](https://github.com/intel/acat/actions/workflows/test.yml)

## Background
The Assistive Context-Aware Toolkit (ACAT) is an open-source platform created at Intel Labs. It is designed to enhance communication for individuals with restricted speech and typing capabilities. It achieves this by providing functionalities like keyboard simulation, word/sentence prediction, and speech synthesis.

ACAT was originally developed by researchers in Intel Labs for Professor Stephen Hawking. He was instrumental to the design process and was a key contributor to the project design and validation. After deploying the system to Professor Hawking, Intel turned its attention to the larger community and continued to make ACAT more configurable to support a larger set of users.

By making this configurable platform open source, the goal is to encourage developers  to continue to expanding its capabilities by adding new user interfaces, new sensing modalities, language prediction, and other features.

ACAT is developed in C# using Microsoft Visual Studio* 2022 and .NET 4.8.1. It runs on Windows® 10 (20H2 or higher) and 11.

## Versioning
The master branch on Github will always contain the most recent release of ACAT.

The dev branch contains commits from the ACAT development team towards upcoming releases of ACAT. Do not clone the dev branch as it has changes that are still in flux and the solution may not be stable.

## Cloning the repo
This repo uses Large File Storage (LFS) and includes submodules. You may run into problems if you use GitHub Desktop to clone it. As a workaround, use Git from the command line.

Clone the repo:  
**$ git clone https://github.com/intel/acat.git**

If you are cloning for the first time:  
**$ git submodule update --init --recursive**

Pull in all the latest changes for the submodules:  
**$ git pull origin master --recurse-submodules**

## Configuration

ACAT uses JSON configuration files for flexible customization. Key configuration types include:

- **ActuatorSettings** - Input device configurations (keyboard, camera, BCI, switches)
- **Theme** - Color schemes and UI styling
- **PanelConfig** - UI panel layouts (scanners, keyboards, menus)
- **Abbreviations** - Text abbreviation expansions
- **Pronunciations** - Custom word pronunciations for text-to-speech

### Documentation

- **[JSON Configuration Guide](docs/JSON_CONFIGURATION_GUIDE.md)** - User guide for configuring ACAT
- **[Developer Guide](docs/JSON_CONFIGURATION_DEVELOPER_GUIDE.md)** - For developers extending the configuration system
- **[Quick Reference](docs/JSON_CONFIGURATION_QUICK_REFERENCE.md)** - Quick reference card
- **[Migration Guide](docs/JSON_CONFIGURATION_MIGRATION.md)** - Migrating from XML to JSON

### Modernization Documentation

- **[ACAT Modernization Plan](ACAT_MODERNIZATION_PLAN.md)** - Complete modernization roadmap
- **[Dependency Injection Guide](DEPENDENCY_INJECTION_GUIDE.md)** - DI architecture and implementation guidance
- **[Logging Migration Guide](LOGGING_MIGRATION_GUIDE.md)** - Logging migration strategy and implementation details
- **[Documentation Index](INDEX.md)** - Complete documentation index

### Configuration Files

Configuration files are located in `%APPDATA%\ACAT\Config\` and support VS Code IntelliSense for easy editing. See the [JSON Configuration Guide](docs/JSON_CONFIGURATION_GUIDE.md) for details.

## License
ACAT is distributed under the Apache License, Version 2.0.

## Project Website
Click [here](https://www.intel.com/content/www/us/en/developer/tools/open/acat/overview.html) for the ACAT project website.

## Contact the team
Click <a href="mailto:ACAT@intel.com">here</a> to contact the ACAT team.

