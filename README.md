# Medical Specialization Tracking App

![Build Status](https://github.com/konrad-n/mock/actions/workflows/dotnet.yml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=konrad-n_mock&metric=alert_status)](https://sonarcloud.io/dashboard?id=konrad-n_mock)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=konrad-n_mock&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=konrad-n_mock)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=konrad-n_mock&metric=security_rating)](https://sonarcloud.io/dashboard?id=konrad-n_mock)

## Project Description

ŚledzSpecke is a comprehensive mobile application that allows resident physicians to track their progress in medical specialization programs. The application supports both the old and new SMK (Education Monitoring System) formats, offering an intuitive interface to manage all aspects of medical specialty training.

## Features

- **Specialization module management** - support for basic and specialist modules
- **Clinical rotations** - adding, editing, and tracking internship progress
- **Medical shifts** - entering and monitoring duty hours
- **Medical procedures** - registering procedures (as operator and assistant)
- **Specialization courses** - managing mandatory and additional courses
- **Self-education** - documenting self-learning activities
- **Publications** - recording scientific activities
- **Educational activities** - tracking participation in conferences and workshops
- **Data export** - generating reports compatible with SMK requirements
- **Statistics and analytics** - visualization of specialization progress
- **Offline functionality** - full functionality without requiring constant internet connection
- **Security** - safe storage of specialization data

## Technologies

- **.NET MAUI** - cross-platform framework for mobile application development
- **SQLite** - local database
- **C#** - main programming language
- **MVVM** - Model-View-ViewModel architecture
- **JSON** - format for storing specialization templates
- **Dependency Injection** - test-oriented architecture

## System Requirements

- **iOS**: 15.0 or newer
- **Android**: 5.0 (API 21) or newer
- **Windows**: 10.0.17763.0 or newer
- **MacOS**: 10.15 or newer (via MacCatalyst)

## Installation

### For Developers

1. Clone the repository:
   ```
   git clone https://github.com/konrad-n/sledzspecke.git
   ```

2. Open the solution in Visual Studio 2022 (or newer):
   ```
   cd sledzspecke
   start SledzSpecke.sln
   ```

3. Install required MAUI workloads:
   ```
   dotnet workload install maui
   ```

4. Build the project:
   ```
   dotnet build
   ```

5. Run the application on the selected platform:
   ```
   dotnet run -f net9.0-android
   ```
   or
   ```
   dotnet run -f net9.0-ios
   ```
   or
   ```
   dotnet run -f net9.0-windows10.0.19041.0
   ```

### For Users

The application will be available for download through:
- Google Play Store (Android)
- Apple App Store (iOS)
- Microsoft Store (Windows)
- Direct installation links on the project website

## Using the Application

1. **Registration** - Create an account with selection of specialization and SMK version
2. **Specialization configuration** - Enter start date and choose specialization program
3. **Adding activities** - Record internships, shifts, procedures, courses, etc.
4. **Progress monitoring** - Track completion of specialization requirements
5. **Data export** - Generate reports for the SMK system

## Support and Development

The application is continuously being developed. Bug reports and feature suggestions can be submitted through the Issues tab on GitHub.

## License

© 2025 Konrad N. All rights reserved.

## Acknowledgements

We thank all resident physicians who contributed to the development of the application through testing and improvement suggestions.
