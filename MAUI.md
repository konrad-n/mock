This file is a merged representation of the entire codebase, combined into a single document by Repomix.
The content has been processed where security check has been disabled.

# File Summary

## Purpose
This file contains a packed representation of the entire repository's contents.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Security check has been disabled - content may contain sensitive information
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
Converters/
  BoolToBackgroundColorConverter.cs
  BoolToColorConverter.cs
  BoolToTextColorConverter.cs
  BoolToTextConverter.cs
  BoolToYesNoConverter.cs
  CodeToColorConverter.cs
  DateRangeConverter.cs
  InvertedBoolConverter.cs
  ItemCountToHeightConverter.cs
  NotNullConverter.cs
  StatusToColorConverter.cs
  StringMatchConverter.cs
  StringMatchToBgColorConverter.cs
  StringMatchToTextColorConverter.cs
  StringToBoolConverter.cs
  StringToIntConverter.cs
Exceptions/
  AppBaseException.cs
  BusinessRuleViolationException.cs
  DataAccessException.cs
  DatabaseConnectionException.cs
  DatabaseException.cs
  DomainLogicException.cs
  InvalidInputException.cs
  NetworkException.cs
  ResourceNotFoundException.cs
Helpers/
  Constants.cs
  ExceptionContextExtensions.cs
  ModuleHelper.cs
  ModuleTypeJsonConverter.cs
  ProgressCalculator.cs
  SettingsHelper.cs
  SpecializationLoader.cs
Models/
  Enums/
    AbsenceType.cs
    CourseType.cs
    EducationalActivityType.cs
    ModuleType.cs
    ProcedureStatus.cs
    RecognitionType.cs
    SMKVersion.cs
    SyncStatus.cs
  Absence.cs
  BasicInfo.cs
  Course.cs
  CourseRequirement.cs
  EducationalActivity.cs
  ExaminationComponent.cs
  ExaminationInfo.cs
  ExportOptions.cs
  HolidaysInfo.cs
  Internship.cs
  InternshipRequirement.cs
  MedicalShift.cs
  MedicalShiftsInfo.cs
  MedicalShiftsSummary.cs
  Module.cs
  ModuleInfo.cs
  ModuleStructure.cs
  NotificationSettings.cs
  Procedure.cs
  ProcedureCodeDescription.cs
  ProcedureDefinition.cs
  ProcedureRequirement.cs
  ProcedureSummary.cs
  Publication.cs
  RealizedInternshipBase.cs
  RealizedInternshipNewSMK.cs
  RealizedInternshipOldSMK.cs
  RealizedMedicalShiftBase.cs
  RealizedMedicalShiftNewSMK.cs
  RealizedMedicalShiftOldSMK.cs
  RealizedProcedureBase.cs
  RealizedProcedureNewSMK.cs
  RealizedProcedureOldSMK.cs
  Recognition.cs
  SelfEducation.cs
  SelfEducationInfo.cs
  Specialization.cs
  SpecializationProgram.cs
  SpecializationStatistics.cs
  SpecializationStructure.cs
  TotalDuration.cs
  User.cs
Platforms/
  Android/
    Resources/
      values/
        colors.xml
    AndroidManifest.xml
    MainActivity.cs
    MainApplication.cs
  iOS/
    AppDelegate.cs
    Info.plist
    Program.cs
  MacCatalyst/
    AppDelegate.cs
    Entitlements.plist
    Info.plist
    Program.cs
  Tizen/
    Main.cs
    tizen-manifest.xml
  Windows/
    app.manifest
    App.xaml
    App.xaml.cs
    Package.appxmanifest
Properties/
  launchSettings.json
Resources/
  Raw/
    SpecializationTemplates/
      cardiology_new.json
      cardiology_old.json
      psychiatry_new.json
      psychiatry_old.json
  Styles/
    Colors.xaml
    iOSStyles.xaml
    MaterialDesignStyles.xaml
    Styles.xaml
Services/
  Authentication/
    AuthService.cs
    IAuthService.cs
  Database/
    DatabaseService.cs
    DatabaseService.Internship.cs
    DatabaseService.MedicalShift.cs
    DatabaseService.Module.cs
    DatabaseService.Procedure.cs
    DatabaseService.RealizedInternship.cs
    DatabaseService.Specialization.cs
    DatabaseService.User.cs
    DatabaseServiceExtensions.cs
    IDatabaseService.cs
  Dialog/
    DialogService.cs
    IDialogService.cs
  Exceptions/
    ExceptionHandlerService.cs
    IExceptionHandlerService.cs
  FileSystem/
    FileSystemService.cs
    IFileSystemService.cs
  Logging/
    ILoggingService.cs
    LoggingService.cs
  MedicalShifts/
    IMedicalShiftsService.cs
    MedicalShiftsService.cs
    YearResult.cs
  Procedures/
    IProcedureService.cs
    ProcedureService.cs
  Specialization/
    ISpecializationService.cs
    ModuleInitializer.cs
    SpecializationService.cs
  Storage/
    ISecureStorageService.cs
    SecureStorageService.cs
  BaseService.cs
ViewModels/
  Authentication/
    LoginViewModel.cs
    RegisterViewModel.cs
  Base/
    BaseViewModel.cs
  Dashboard/
    DashboardViewModel.cs
  Internships/
    AddEditRealizedInternshipViewModel.cs
    InternshipRequirementViewModel.cs
    InternshipsSelectorViewModel.cs
    InternshipStageViewModel.cs
    NewSMKInternshipsListViewModel.cs
    OldSMKInternshipsListViewModel.cs
  MedicalShifts/
    AddEditMedicalShiftViewModel.cs
    AddEditOldSMKMedicalShiftViewModel.cs
    MedicalShiftsListViewModel.cs
    MedicalShiftsSelectorViewModel.cs
    NewSMKMedicalShiftsListViewModel.cs
    OldSMKMedicalShiftsListViewModel.cs
  Procedures/
    AddEditNewSMKProcedureViewModel.cs
    AddEditOldSMKProcedureViewModel.cs
    NewSMKProceduresListViewModel.cs
    OldSMKProceduresListViewModel.cs
    ProcedureGroupViewModel.cs
    ProcedureRequirementViewModel.cs
    ProcedureSelectorViewModel.cs
Views/
  Authentication/
    LoginPage.xaml
    LoginPage.xaml.cs
    RegisterPage.xaml
    RegisterPage.xaml.cs
  Dashboard/
    DashboardPage.xaml
    DashboardPage.xaml.cs
  Internships/
    AddEditRealizedInternshipPage.xaml
    AddEditRealizedInternshipPage.xaml.cs
    InternshipsSelectorPage.xaml
    InternshipsSelectorPage.xaml.cs
    NewSMKInternshipsListPage.xaml
    NewSMKInternshipsListPage.xaml.cs
    OldSMKInternshipsListPage.xaml
    OldSMKInternshipsListPage.xaml.cs
  MedicalShifts/
    AddEditOldSMKMedicalShiftPage.xaml
    AddEditOldSMKMedicalShiftPage.xaml.cs
    MedicalShiftsSelectorPage.xaml
    MedicalShiftsSelectorPage.xaml.cs
    NewSMKMedicalShiftsPage.xaml
    NewSMKMedicalShiftsPage.xaml.cs
    OldSMKMedicalShiftsPage.xaml
    OldSMKMedicalShiftsPage.xaml.cs
  Procedures/
    AddEditNewSMKProcedurePage.xaml
    AddEditNewSMKProcedurePage.xaml.cs
    AddEditOldSMKProcedurePage.xaml
    AddEditOldSMKProcedurePage.xaml.cs
    NewSMKProceduresListPage.xaml
    NewSMKProceduresListPage.xaml.cs
    OldSMKProceduresListPage.xaml
    OldSMKProceduresListPage.xaml.cs
    ProcedureSelectorPage.xaml
    ProcedureSelectorPage.xaml.cs
App.xaml
App.xaml.cs
AppShell.xaml
AppShell.xaml.cs
MauiProgram.cs
SledzSpecke.csproj
SledzSpecke.net9.0-android35.0.v3.ncrunchproject
SledzSpecke.net9.0-ios.v3.ncrunchproject
SledzSpecke.net9.0-maccatalyst.v3.ncrunchproject
SplashPage.xaml
SplashPage.xaml.cs
```

# Files

## File: Converters/BoolToBackgroundColorConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Colors.Gray;
            }

            try
            {
                int selectedValue = (int)parameter;
                int currentValue = (int)value;

                return selectedValue == currentValue ? Colors.DarkBlue : Colors.LightSlateGray;
            }
            catch
            {
                return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/BoolToColorConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? Color.FromArgb("#24C1DE") : Colors.Transparent;
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/BoolToTextColorConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? Colors.White : Color.FromArgb("#082044");
            }

            return Color.FromArgb("#082044");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/BoolToTextConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
            {
                return string.Empty;
            }

            if (parameter is not string textParameter)
            {
                return boolValue ? "Tak" : "Nie";
            }

            var parts = textParameter.Split(',');
            if (parts.Length < 2)
            {
                return boolValue ? "Tak" : "Nie";
            }

            return boolValue ? parts[0] : parts[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/BoolToYesNoConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Tak" : "Nie";
            }

            return "Nie";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.Equals("Tak", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
```

## File: Converters/CodeToColorConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class CodeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string code)
            {
                return code switch
                {
                    "A" => Color.FromArgb("#24C1DE"),
                    "B" => Color.FromArgb("#F59E0B"),
                    _ => Color.FromArgb("#547E9E"),
                };
            }

            return Color.FromArgb("#547E9E");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/DateRangeConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class DateRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Models.RealizedProcedureNewSMK procedure)
            {
                return string.Empty;
            }

            return $"{procedure.StartDate:dd.MM.yyyy} - {procedure.EndDate:dd.MM.yyyy}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/InvertedBoolConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
```

## File: Converters/ItemCountToHeightConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class ItemCountToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count && int.TryParse(parameter?.ToString(), out int heightPerItem))
            {
                return count * heightPerItem;
            }

            // Domyślna wysokość
            return 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/NotNullConverter.cs
```csharp
using System;
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class NotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/StatusToColorConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                if (status.Contains("Ukończon") || status.Contains("Zatwierdzon"))
                {
                    return Colors.Green;
                }
                else if (status.Contains("Oczekując") || status.Contains("Wymaga"))
                {
                    return Colors.Orange;
                }
            }

            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
```

## File: Converters/StringMatchConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StringMatchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string parameterValue)
            {
                return stringValue.Equals(parameterValue, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue && parameter is string stringValue)
            {
                return stringValue;
            }

            return null;
        }
    }
}
```

## File: Converters/StringMatchToBgColorConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StringMatchToBgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string parameterValue)
            {
                return stringValue.Equals(parameterValue, StringComparison.OrdinalIgnoreCase) ?
                    Application.Current.Resources["PrimaryColor"] : Colors.Transparent;
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
```

## File: Converters/StringMatchToTextColorConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StringMatchToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string parameterValue)
            {
                return stringValue.Equals(parameterValue, StringComparison.OrdinalIgnoreCase) ?
                    Colors.White : Application.Current.Resources["PrimaryColor"];
            }

            return Application.Current.Resources["PrimaryColor"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
```

## File: Converters/StringToBoolConverter.cs
```csharp
using System;
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

## File: Converters/StringToIntConverter.cs
```csharp
using System.Globalization;

namespace SledzSpecke.App.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && int.TryParse(stringValue, out int result))
            {
                return result;
            }

            return 0;
        }
    }
}
```

## File: Exceptions/AppBaseException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public abstract class AppBaseException : Exception
    {
        public string UserFriendlyMessage { get; }
        public Dictionary<string, object> ErrorDetails { get; }

        protected AppBaseException(
            string message, 
            string userFriendlyMessage = "Wystąpił błąd w aplikacji.", 
            Exception innerException = null, 
            Dictionary<string, object> errorDetails = null) 
            : base(message, innerException)
        {
            UserFriendlyMessage = userFriendlyMessage;
            ErrorDetails = errorDetails ?? new Dictionary<string, object>();
        }
    }
}
```

## File: Exceptions/BusinessRuleViolationException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class BusinessRuleViolationException : DomainLogicException
    {
        public BusinessRuleViolationException(
            string message,
            string userFriendlyMessage = "Naruszono regułę biznesową.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Exceptions/DataAccessException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class DataAccessException : AppBaseException
    {
        public DataAccessException(
            string message,
            string userFriendlyMessage = "Wystąpił problem z dostępem do danych.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Exceptions/DatabaseConnectionException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class DatabaseConnectionException : DataAccessException
    {
        public DatabaseConnectionException(
            string message,
            string userFriendlyMessage = "Nie można połączyć się z bazą danych.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Exceptions/DatabaseException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class DatabaseException : DataAccessException
    {
        public DatabaseException(
            string message,
            string userFriendlyMessage = "Wystąpił problem z bazą danych.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Exceptions/DomainLogicException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class DomainLogicException : AppBaseException
    {
        public DomainLogicException(
            string message,
            string userFriendlyMessage = "Wystąpił błąd logiki biznesowej.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Exceptions/InvalidInputException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class InvalidInputException : AppBaseException
    {
        public InvalidInputException(
            string message,
            string userFriendlyMessage = "Wprowadzono nieprawidłowe dane.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Exceptions/NetworkException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class NetworkException : AppBaseException
    {
        public NetworkException(
            string message,
            string userFriendlyMessage = "Wystąpił problem z połączeniem sieciowym. Sprawdź swoje połączenie i spróbuj ponownie.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Exceptions/ResourceNotFoundException.cs
```csharp
namespace SledzSpecke.App.Exceptions
{
    public class ResourceNotFoundException : AppBaseException
    {
        public ResourceNotFoundException(
            string message,
            string userFriendlyMessage = "Nie znaleziono żądanego zasobu.",
            Exception innerException = null,
            Dictionary<string, object> errorDetails = null)
            : base(message, userFriendlyMessage, innerException, errorDetails)
        {
        }
    }
}
```

## File: Helpers/Constants.cs
```csharp
using Microsoft.Maui.Controls.PlatformConfiguration;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Helpers
{
    public static class Constants
    {
        private static IFileSystemService fileSystemService;
        private static IExceptionHandlerService exceptionHandlerService;
        private static IDialogService dialogService;
        private static ILoggingService loggingService;

        static Constants()
        {
            fileSystemService = new FileSystemService();
        }

        public static void SetFileSystemService(IFileSystemService service)
        {
            fileSystemService = service ?? throw new ArgumentNullException(nameof(service));
        }

        public const string CurrentUserIdKey = "CurrentUserId";
        public const string CurrentModuleIdKey = "CurrentModuleId";
        public const string DatabaseFilename = "sledzspecke.db3";
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(fileSystemService.AppDataDirectory, DatabaseFilename);
    }
}
```

## File: Helpers/ExceptionContextExtensions.cs
```csharp
// Helpers/ExceptionContextExtensions.cs
namespace SledzSpecke.App.Helpers
{
    public static class ExceptionContextExtensions
    {
        public static Dictionary<string, object> ToContext(this int value, string key = "Id")
        {
            return new Dictionary<string, object> { { key, value } };
        }

        public static Dictionary<string, object> ToContext(this string value, string key = "Name")
        {
            return new Dictionary<string, object> { { key, value } };
        }

        public static Dictionary<string, object> AddContext(this Dictionary<string, object> context, string key, object value)
        {
            if (context == null)
            {
                context = new Dictionary<string, object>();
            }

            if (!context.ContainsKey(key) && value != null)
            {
                context[key] = value;
            }

            return context;
        }
    }
}
```

## File: Helpers/ModuleHelper.cs
```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public static class ModuleHelper
    {
        public static async Task<List<Module>> CreateModulesForSpecializationAsync(
            string specializationCode,
            DateTime startDate,
            SmkVersion smkVersion,
            int specializationId)
        {
            if (string.IsNullOrEmpty(specializationCode))
            {
                return new List<Module>();
            }

            var specializationProgram = await SpecializationLoader.LoadSpecializationProgramAsync(specializationCode, smkVersion);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new ModuleTypeJsonConverter(),
                }
            };

            var modules = new List<Module>();

            var jsonDocument = JsonDocument.Parse(specializationProgram.Structure);
            if (!jsonDocument.RootElement.TryGetProperty("modules", out var modulesElement) ||
                modulesElement.ValueKind != JsonValueKind.Array)
            {
                System.Diagnostics.Debug.WriteLine("Nie znaleziono tablicy 'modules' w JSON specjalizacji.");
            }

            int moduleIndex = 0;
            DateTime currentStartDate = startDate;

            foreach (var moduleElement in modulesElement.EnumerateArray())
            {
                moduleIndex++;

                string moduleName = string.Empty;
                string moduleCode = string.Empty;
                ModuleType moduleType = ModuleType.Specialistic;
                int durationMonths = 0;
                int workingDays = 0;

                if (moduleElement.TryGetProperty("name", out var nameElement))
                {
                    moduleName = nameElement.GetString();
                }

                if (moduleElement.TryGetProperty("moduleType", out var typeElement))
                {
                    string typeString = typeElement.GetString();
                    if (!string.IsNullOrEmpty(typeString))
                    {
                        if (typeString.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                        {
                            moduleType = ModuleType.Basic;
                        }
                        else if (typeString.Equals("Specialistic", StringComparison.OrdinalIgnoreCase))
                        {
                            moduleType = ModuleType.Specialistic;
                        }
                    }
                }

                if (string.IsNullOrEmpty(moduleName))
                {
                    if (moduleType == ModuleType.Basic)
                    {
                        moduleName = "Moduł podstawowy";
                    }
                    else
                    {
                        moduleName = $"Moduł specjalistyczny w zakresie {specializationCode}";
                    }
                }

                if (moduleElement.TryGetProperty("duration", out var durationElement))
                {
                    if (durationElement.TryGetProperty("years", out var yearsElement))
                    {
                        durationMonths = yearsElement.GetInt32() * 12;
                    }
                    if (durationElement.TryGetProperty("months", out var monthsElement))
                    {
                        durationMonths += monthsElement.GetInt32();
                    }
                }
                else if (moduleElement.TryGetProperty("durationMonths", out var monthsElement))
                {
                    durationMonths = monthsElement.GetInt32();
                }

                if (durationMonths == 0)
                {
                    if (moduleType == ModuleType.Basic)
                    {
                        durationMonths = 24;
                    }
                    else
                    {
                        durationMonths = 36;
                    }
                }

                DateTime endDate = currentStartDate.AddMonths(durationMonths);

                int totalInternships = 0;
                int totalCourses = 0;
                int totalProceduresA = 0;
                int totalProceduresB = 0;

                if (moduleElement.TryGetProperty("internships", out var internshipsElement) &&
                    internshipsElement.ValueKind == JsonValueKind.Array)
                {
                    totalInternships = internshipsElement.GetArrayLength();
                }

                if (moduleElement.TryGetProperty("courses", out var coursesElement) &&
                    coursesElement.ValueKind == JsonValueKind.Array)
                {
                    totalCourses = coursesElement.GetArrayLength();
                }

                if (moduleElement.TryGetProperty("procedures", out var proceduresElement) &&
                    proceduresElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var procedureElement in proceduresElement.EnumerateArray())
                    {
                        if (procedureElement.TryGetProperty("requiredCountA", out var countAElement))
                        {
                            totalProceduresA += countAElement.GetInt32();
                        }

                        if (procedureElement.TryGetProperty("requiredCountB", out var countBElement))
                        {
                            totalProceduresB += countBElement.GetInt32();
                        }
                    }
                }

                int requiredShiftHours = 0;
                double hoursPerWeek = 0;
                string medicalShiftsDescription = null;

                if (moduleElement.TryGetProperty("medicalShifts", out var medicalShiftsElement) &&
                    medicalShiftsElement.ValueKind == JsonValueKind.Object)
                {
                    if (medicalShiftsElement.TryGetProperty("requiredShiftHours", out var requiredHoursElement))
                    {
                        requiredShiftHours = requiredHoursElement.GetInt32();
                    }

                    if (medicalShiftsElement.TryGetProperty("hoursPerWeek", out var hoursPerWeekElement))
                    {
                        hoursPerWeek = hoursPerWeekElement.GetDouble();
                    }

                    if (medicalShiftsElement.TryGetProperty("description", out var descriptionElement))
                    {
                        medicalShiftsDescription = descriptionElement.GetString();
                    }
                }

                int selfEducationDays = 0;

                if (moduleElement.TryGetProperty("selfEducation", out var selfEducationElement) &&
                    selfEducationElement.ValueKind == JsonValueKind.Object)
                {
                    if (selfEducationElement.TryGetProperty("totalDays", out var totalDaysElement))
                    {
                        selfEducationDays = totalDaysElement.GetInt32();
                    }
                    else if (selfEducationElement.TryGetProperty("daysPerYear", out var daysPerYearElement))
                    {
                        int daysPerYear = daysPerYearElement.GetInt32();
                        int years = (int)Math.Ceiling(durationMonths / 12.0);
                        selfEducationDays = daysPerYear * years;
                    }
                }

                var module = new Module
                {
                    Name = moduleName,
                    Type = moduleType,
                    StartDate = currentStartDate,
                    EndDate = endDate,
                    Structure = moduleElement.ToString(),
                    SmkVersion = smkVersion,
                    Version = moduleElement.GetProperty("version").GetString(),
                    SpecializationId = specializationId,
                    CompletedInternships = 0,
                    TotalInternships = totalInternships,
                    CompletedCourses = 0,
                    TotalCourses = totalCourses,
                    CompletedProceduresA = 0,
                    TotalProceduresA = totalProceduresA,
                    CompletedProceduresB = 0,
                    TotalProceduresB = totalProceduresB,
                    CompletedShiftHours = 0,
                    RequiredShiftHours = requiredShiftHours,
                    WeeklyShiftHours = hoursPerWeek,
                    CompletedSelfEducationDays = 0,
                    TotalSelfEducationDays = selfEducationDays
                };

                if (module.RequiredShiftHours == 0 && module.WeeklyShiftHours > 0)
                {
                    TimeSpan duration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(duration.TotalDays / 7));
                    module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                }
                else if (module.RequiredShiftHours == 0 && module.WeeklyShiftHours == 0)
                {
                    double defaultWeeklyHours = 10.083;

                    TimeSpan duration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(duration.TotalDays / 7));

                    module.WeeklyShiftHours = defaultWeeklyHours;
                    module.RequiredShiftHours = (int)Math.Round(defaultWeeklyHours * weeks);
                }

                modules.Add(module);

                currentStartDate = endDate.AddDays(1);
            }

            if (modules.Count > 0)
            {
                return modules;
            }

            return new List<Module>();
        }
    }
}
```

## File: Helpers/ModuleTypeJsonConverter.cs
```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public class ModuleTypeJsonConverter : JsonConverter<ModuleType>
    {
        public override ModuleType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                return ModuleType.Specialistic;
            }

            return value.Equals("Basic", StringComparison.OrdinalIgnoreCase) ? ModuleType.Basic : ModuleType.Specialistic;
        }

        public override void Write(Utf8JsonWriter writer, ModuleType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
```

## File: Helpers/ProgressCalculator.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public class ProgressCalculator
    {
        public static async Task UpdateModuleProgressAsync(
            IDatabaseService database,
            int moduleId)
        {
            var module = await database.GetModuleAsync(moduleId);
            if (module == null)
            {
                return;
            }

            var internships = await database.GetInternshipsAsync(moduleId: moduleId);
            var completedInternships = internships.Count(i => i.IsCompleted);
            var procedures = new List<Procedure>();

            foreach (var internship in internships)
            {
                var internshipProcedures = await database.GetProceduresAsync(internshipId: internship.InternshipId);
                procedures.AddRange(internshipProcedures);
            }

            var proceduresA = procedures.Count(p => p.OperatorCode == "A");
            var proceduresB = procedures.Count(p => p.OperatorCode == "B");
            var shifts = new List<MedicalShift>();

            foreach (var internship in internships)
            {
                var internshipShifts = await database.GetMedicalShiftsAsync(internshipId: internship.InternshipId);
                shifts.AddRange(internshipShifts);
            }

            double totalShiftHours = shifts.Sum(s => s.Hours + ((double)s.Minutes / 60.0));
            int completedShiftHours = (int)Math.Round(totalShiftHours);
            ModuleStructure moduleStructure = null;

            if (!string.IsNullOrEmpty(module.Structure))
            {
                moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        Converters = { new JsonStringEnumConverter() }
                    });
            }

            module.CompletedInternships = completedInternships;
            module.TotalInternships = moduleStructure?.Internships?.Count ?? 0;
            module.TotalCourses = moduleStructure?.Courses?.Count ?? 0;
            module.CompletedProceduresA = proceduresA;
            module.TotalProceduresA = moduleStructure?.Procedures?.Sum(p => p.RequiredCountA) ?? 0;
            module.CompletedProceduresB = proceduresB;
            module.TotalProceduresB = moduleStructure?.Procedures?.Sum(p => p.RequiredCountB) ?? 0;
            module.CompletedShiftHours = completedShiftHours;

            if (module.RequiredShiftHours <= 0)
            {
                if (moduleStructure?.RequiredShiftHours > 0)
                {
                    module.RequiredShiftHours = moduleStructure.RequiredShiftHours;
                }
                else if (moduleStructure?.MedicalShifts?.HoursPerWeek > 0)
                {
                    TimeSpan moduleDuration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));
                    module.WeeklyShiftHours = moduleStructure.MedicalShifts.HoursPerWeek;
                    module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                }
                else
                {
                    module.WeeklyShiftHours = 10.083;
                    TimeSpan moduleDuration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));
                    module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                }
            }

            if (module.TotalSelfEducationDays <= 0 && moduleStructure?.SelfEducationDays > 0)
            {
                module.TotalSelfEducationDays = moduleStructure.SelfEducationDays;
            }

            await database.UpdateModuleAsync(module);
            await UpdateSpecializationProgressAsync(database, module.SpecializationId);
        }

        public static async Task UpdateSpecializationProgressAsync(
            IDatabaseService database,
            int specializationId)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return;
            }

            var modules = await database.GetModulesAsync(specializationId);
            specialization.CompletedInternships = modules.Sum(m => m.CompletedInternships);
            specialization.TotalInternships = modules.Sum(m => m.TotalInternships);
            specialization.CompletedCourses = modules.Sum(m => m.CompletedCourses);
            specialization.TotalCourses = modules.Sum(m => m.TotalCourses);
            await database.UpdateSpecializationAsync(specialization);
        }

        public static async Task<SpecializationStatistics> CalculateFullStatisticsAsync(
            IDatabaseService database,
            int specializationId,
            int? moduleId = null)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return new SpecializationStatistics();
            }

            var stats = new SpecializationStatistics();
            SpecializationStructure structure = null;

            if (!string.IsNullOrEmpty(specialization.ProgramStructure))
            {
                structure = JsonSerializer.Deserialize<SpecializationStructure>(specialization.ProgramStructure);
            }

            if (moduleId.HasValue)
            {
                var module = await database.GetModuleAsync(moduleId.Value);

                if (module == null)
                {
                    return stats;
                }

                stats.CompletedInternships = module.CompletedInternships;
                stats.RequiredInternships = module.TotalInternships;
                stats.CompletedCourses = module.CompletedCourses;
                stats.RequiredCourses = module.TotalCourses;
                stats.CompletedProceduresA = module.CompletedProceduresA;
                stats.RequiredProceduresA = module.TotalProceduresA;
                stats.CompletedProceduresB = module.CompletedProceduresB;
                stats.RequiredProceduresB = module.TotalProceduresB;
                ModuleStructure moduleStructure = null;

                if (!string.IsNullOrEmpty(module.Structure))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        Converters = { new JsonStringEnumConverter() }
                    };

                    moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                }

                var internships = await database.GetInternshipsAsync(moduleId: moduleId);
                var completedInternships = internships.Where(i => i.IsCompleted).ToList();

                stats.CompletedInternshipDays = completedInternships.Sum(i => i.DaysCount);
                stats.RequiredInternshipWorkingDays = moduleStructure?.Internships?.Sum(i => i.WorkingDays) ?? 0;

                var oldSmkShiftsQuery = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ?";
                var oldSmkShifts = await database.QueryAsync<RealizedMedicalShiftOldSMK>(oldSmkShiftsQuery, specialization.SpecializationId);

                var newSmkShiftsQuery = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ?";
                var newSmkShifts = await database.QueryAsync<RealizedMedicalShiftNewSMK>(newSmkShiftsQuery, specialization.SpecializationId);

                double totalShiftHours = 0;

                foreach (var shift in oldSmkShifts)
                {
                    totalShiftHours += shift.Hours + ((double)shift.Minutes / 60.0);
                }

                foreach (var shift in newSmkShifts)
                {
                    totalShiftHours += shift.Hours + ((double)shift.Minutes / 60.0);
                }

                stats.CompletedShiftHours = (int)Math.Round(totalShiftHours);
                stats.RequiredShiftHours = module.RequiredShiftHours;

                if (stats.RequiredShiftHours == 0)
                {
                    if (moduleStructure != null && moduleStructure.RequiredShiftHours > 0)
                    {
                        stats.RequiredShiftHours = moduleStructure.RequiredShiftHours;
                        module.RequiredShiftHours = stats.RequiredShiftHours;
                        await database.UpdateModuleAsync(module);
                    }
                    else
                    {
                        double weeklyHours = module.WeeklyShiftHours;

                        if (weeklyHours <= 0)
                        {
                            if (moduleStructure?.MedicalShifts != null && moduleStructure.MedicalShifts.HoursPerWeek > 0)
                            {
                                weeklyHours = moduleStructure.MedicalShifts.HoursPerWeek;
                                module.WeeklyShiftHours = weeklyHours;
                            }
                            else
                            {
                                weeklyHours = 10.083;
                                module.WeeklyShiftHours = weeklyHours;
                            }
                        }

                        TimeSpan moduleDuration = module.EndDate - module.StartDate;
                        int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));
                        stats.RequiredShiftHours = (int)Math.Round(weeklyHours * weeks);
                        module.RequiredShiftHours = stats.RequiredShiftHours;
                        await database.UpdateModuleAsync(module);
                    }
                }

                return stats;
            }
            else
            {
                var modules = await database.GetModulesAsync(specializationId);

                stats.CompletedInternships = modules.Sum(m => m.CompletedInternships);
                stats.RequiredInternships = modules.Sum(m => m.TotalInternships);
                stats.CompletedCourses = modules.Sum(m => m.CompletedCourses);
                stats.RequiredCourses = modules.Sum(m => m.TotalCourses);
                stats.CompletedProceduresA = modules.Sum(m => m.CompletedProceduresA);
                stats.RequiredProceduresA = modules.Sum(m => m.TotalProceduresA);
                stats.CompletedProceduresB = modules.Sum(m => m.CompletedProceduresB);
                stats.RequiredProceduresB = modules.Sum(m => m.TotalProceduresB);

                var internships = new List<Internship>();
                foreach (var module in modules)
                {
                    var moduleInternships = await database.GetInternshipsAsync(moduleId: module.ModuleId);
                    internships.AddRange(moduleInternships.Where(i => i.IsCompleted));
                }

                stats.CompletedInternshipDays = internships.Sum(i => i.DaysCount);
                stats.RequiredInternshipWorkingDays = structure?.TotalWorkingDays ?? 0;

                var allShifts = new List<MedicalShift>();
                foreach (var internship in internships)
                {
                    var shifts = await database.GetMedicalShiftsAsync(internship.InternshipId);
                    allShifts.AddRange(shifts);
                }

                stats.CompletedShiftHours = (int)Math.Round(allShifts.Sum(s => s.Hours + ((double)s.Minutes / 60.0)));
                stats.RequiredShiftHours = (int)Math.Round(
                    CalculateRequiredShiftHours(structure, specialization.PlannedEndDate - specialization.StartDate));
            }

            return stats;
        }

        private static double CalculateRequiredShiftHours(SpecializationStructure structure, TimeSpan duration)
        {
            if (structure?.MedicalShifts == null)
            {
                double defaultHoursPerWeek = 10.083;
                int weeksInSpec = Math.Max(1, (int)(duration.TotalDays / 7));
                double defaultValue = defaultHoursPerWeek * weeksInSpec;

                return defaultValue;
            }

            double weeklyHours = structure.MedicalShifts.HoursPerWeek;

            if (weeklyHours < 0.1)
            {
                weeklyHours = 10.083;
            }

            int weeks = Math.Max(1, (int)(duration.TotalDays / 7));
            double result = weeklyHours * weeks;

            System.Diagnostics.Debug.WriteLine($"CalculateRequiredShiftHours: Final calculation: {weeklyHours} * {weeks} = {result}");
            return result;
        }

        public static async Task<double> GetOverallProgressAsync(
            IDatabaseService database,
            int specializationId,
            int? moduleId = null)
        {
            if (moduleId.HasValue)
            {
                var module = await database.GetModuleAsync(moduleId.Value);
                if (module == null)
                {
                    return 0;
                }

                const double internshipWeight = 0.35;
                const double courseWeight = 0.25;
                const double procedureWeight = 0.30;
                const double otherWeight = 0.10;

                double internshipProgress = module.TotalInternships > 0
                    ? (double)module.CompletedInternships / module.TotalInternships
                    : 0;

                double courseProgress = module.TotalCourses > 0
                    ? (double)module.CompletedCourses / module.TotalCourses
                    : 0;

                double procedureProgressA = module.TotalProceduresA > 0
                    ? (double)module.CompletedProceduresA / module.TotalProceduresA
                    : 0;

                double procedureProgressB = module.TotalProceduresB > 0
                    ? (double)module.CompletedProceduresB / module.TotalProceduresB
                    : 0;

                double procedureProgress;

                if (module.TotalProceduresA + module.TotalProceduresB > 0)
                {
                    procedureProgress =
                        (procedureProgressA * module.TotalProceduresA +
                         procedureProgressB * module.TotalProceduresB) /
                        (module.TotalProceduresA + module.TotalProceduresB);
                }
                else
                {
                    procedureProgress = 0;
                }

                double overallProgress = (internshipProgress * internshipWeight) +
                                         (courseProgress * courseWeight) +
                                         (procedureProgress * procedureWeight);

                return Math.Min(1.0, overallProgress);
            }
            else
            {
                var stats = await CalculateFullStatisticsAsync(database, specializationId);
                return stats.GetOverallProgress();
            }
        }
    }
}
```

## File: Helpers/SettingsHelper.cs
```csharp
using SledzSpecke.App.Services.Storage;

namespace SledzSpecke.App.Helpers
{
    public static class SettingsHelper
    {
        private static ISecureStorageService secureStorageService;

        static SettingsHelper()
        {
            secureStorageService = new SecureStorageService();
        }

        public static void SetSecureStorageService(ISecureStorageService service)
        {
            secureStorageService = service ?? throw new ArgumentNullException(nameof(service));
        }

        public static async Task<int> GetCurrentUserIdAsync()
        {
            return await secureStorageService.GetAsync(Constants.CurrentUserIdKey) is string idStr &&
                   int.TryParse(idStr, out int id) ? id : 0;
        }

        public static async Task SetCurrentUserIdAsync(int userId)
        {
            if (userId > 0)
            {
                await secureStorageService.SetAsync(Constants.CurrentUserIdKey, userId.ToString());
            }
            else
            {
                secureStorageService.Remove(Constants.CurrentUserIdKey);
            }
        }

        public static async Task<int> GetCurrentModuleIdAsync()
        {
            return await secureStorageService.GetAsync(Constants.CurrentModuleIdKey) is string idStr &&
                   int.TryParse(idStr, out int id) ? id : 0;
        }

        public static async Task SetCurrentModuleIdAsync(int moduleId)
        {
            if (moduleId > 0)
            {
                await secureStorageService.SetAsync(Constants.CurrentModuleIdKey, moduleId.ToString());
            }
            else
            {
                secureStorageService.Remove(Constants.CurrentModuleIdKey);
            }
        }
    }
}
```

## File: Helpers/SpecializationLoader.cs
```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public static class SpecializationLoader
    {
        private static readonly string ResourcePrefix = "SledzSpecke.App.Resources.Raw.SpecializationTemplates";

        public static async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            string fileName = $"{code.ToLowerInvariant()}_{(smkVersion == SmkVersion.New ? "new" : "old")}.json";
            string templatesPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
            Directory.CreateDirectory(templatesPath);
            string filePath = Path.Combine(templatesPath, fileName);

            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                var program = DeserializeSpecializationProgram(json);

                if (program != null)
                {
                    program.SmkVersion = smkVersion;
                    return program;
                }
            }

            var assembly = typeof(SpecializationLoader).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            string resourceName = $"{ResourcePrefix}.{fileName}";

            if (!resourceNames.Contains(resourceName))
            {
                resourceName = resourceNames
                    .FirstOrDefault(r => r.Contains(code.ToLowerInvariant()) &&
                                        r.Contains(smkVersion == SmkVersion.New ? "new" : "old"));
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string json = await reader.ReadToEndAsync();
                            var program = DeserializeSpecializationProgram(json);

                            if (program != null)
                            {
                                program.SmkVersion = smkVersion;

                                await File.WriteAllTextAsync(filePath, json);

                                return program;
                            }
                        }
                    }
                }
            }

            return new SpecializationProgram();
        }

        public static async Task<List<SpecializationProgram>> LoadAllSpecializationProgramsForVersionAsync(SmkVersion smkVersion)
        {
            var programs = new List<SpecializationProgram>();

            string targetPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
            Directory.CreateDirectory(targetPath);

            if (Directory.Exists(targetPath))
            {
                var files = Directory.GetFiles(targetPath, "*.json");

                foreach (var file in files)
                {
                    string json = await File.ReadAllTextAsync(file);
                    var program = DeserializeSpecializationProgram(json);

                    if (program != null)
                    {
                        string fileName = Path.GetFileName(file);
                        if (fileName.Contains("_new") && smkVersion == SmkVersion.New)
                        {
                            program.SmkVersion = SmkVersion.New;
                            programs.Add(program);
                        }
                        else if (fileName.Contains("_old") && smkVersion == SmkVersion.Old)
                        {
                            program.SmkVersion = SmkVersion.Old;
                            programs.Add(program);
                        }
                    }
                }
            }

            if (programs.Count == 0)
            {
                var assembly = typeof(SpecializationLoader).Assembly;
                var resourceNames = assembly.GetManifestResourceNames();

                var matchingResources = resourceNames
                    .Where(r => r.StartsWith(ResourcePrefix) &&
                            r.EndsWith(".json") &&
                            r.Contains(smkVersion == SmkVersion.New ? "_new" : "_old"))
                    .ToList();

                foreach (var resourceName in matchingResources)
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                string json = await reader.ReadToEndAsync();
                                var program = DeserializeSpecializationProgram(json);

                                if (program != null)
                                {
                                    program.SmkVersion = smkVersion;
                                    programs.Add(program);
                                    string fileName = Path.GetFileName(resourceName);

                                    if (string.IsNullOrEmpty(fileName))
                                    {
                                        fileName = $"{program.Code?.ToLowerInvariant() ?? "unknown"}_{(smkVersion == SmkVersion.New ? "new" : "old")}.json";
                                    }

                                    string filePath = Path.Combine(targetPath, fileName);
                                    if (!File.Exists(filePath))
                                    {
                                        try
                                        {
                                            await File.WriteAllTextAsync(filePath, json);
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Błąd zapisywania do pliku: {ex.Message}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (programs.Count == 0)
            {
                throw new InvalidDataException("Nie udało się załadować żadnych programów specjalizacji.");
            }

            return programs;
        }

        private static SpecializationProgram DeserializeSpecializationProgram(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                Converters = { new JsonStringEnumConverter() }
            };

            var program = JsonSerializer.Deserialize<SpecializationProgram>(json, options);
            program.Structure = json;

            return program;
        }
    }
}
```

## File: Models/Enums/AbsenceType.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum AbsenceType
    {
        Sick,        // Zwolnienie lekarskie (L4)
        Maternity,   // Urlop macierzyński
        Paternity,   // Urlop ojcowski/rodzicielski
        Vacation,    // Urlop wypoczynkowy (nie wpływa na przedłużenie)
        Unpaid,      // Urlop bezpłatny
        Training,    // Urlop szkoleniowy
        Recognition, // Uznanie (skrócenie specjalizacji)
        Other,       // Inne nieobecności
    }
}
```

## File: Models/Enums/CourseType.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum CourseType
    {
        Mandatory,    // Kurs obowiązkowy
        Optional,     // Kurs opcjonalny
        Introductory, // Kurs wprowadzający
        Attestation,  // Kurs atestacyjny
        Recognition,  // Kurs uznany z wcześniejszej specjalizacji
    }
}
```

## File: Models/Enums/EducationalActivityType.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum EducationalActivityType
    {
        Conference, // Konferencja naukowa
        Workshop,   // Warsztaty/szkolenie
        Teaching,   // Nauczanie studentów/staż dydaktyczny
        Research,   // Badania naukowe
        Other,      // Inne formy aktywności edukacyjnej
    }
}
```

## File: Models/Enums/ModuleType.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum ModuleType
    {
        Basic,        // Podstawowy
        Specialistic, // Specjalistyczny
    }
}
```

## File: Models/Enums/ProcedureStatus.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum ProcedureStatus
    {
        Completed,          // Ukończona
        PartiallyCompleted, // Częściowo ukończona
        Approved,           // Zatwierdzona przez kierownika
        NotApproved,        // Nie zatwierdzona
        Pending,            // Oczekująca
    }
}
```

## File: Models/Enums/RecognitionType.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum RecognitionType
    {
        BasicInternship,          // Uznanie stażu podstawowego
        SpecializationInternship, // Uznanie stażu kierunkowego
        Courses,                  // Uznanie kursów
        PreviousSpecialization,   // Uznanie poprzedniej specjalizacji
        WorkExperience,           // Uznanie doświadczenia zawodowego
        Other,                    // Inne uznania
    }
}
```

## File: Models/Enums/SMKVersion.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum SmkVersion
    {
        New,
        Old,
    }
}
```

## File: Models/Enums/SyncStatus.cs
```csharp
namespace SledzSpecke.App.Models.Enums
{
    public enum SyncStatus
    {
        NotSynced,  // Nie zsynchronizowane z SMK
        Synced,     // Zsynchronizowane z SMK
        SyncFailed, // Próba synchronizacji nie powiodła się
        Modified,   // Zsynchronizowane, ale później zmodyfikowane
    }
}
```

## File: Models/Absence.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Absence
    {
        [PrimaryKey]
        [AutoIncrement]
        public int AbsenceId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public AbsenceType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }

        [Ignore]
        public int DaysCount => (this.EndDate - this.StartDate).Days + 1;

        [Ignore]
        public bool ExtendsSpecialization =>
            this.Type == AbsenceType.Sick ||
            this.Type == AbsenceType.Maternity ||
            this.Type == AbsenceType.Paternity;
    }
}
```

## File: Models/BasicInfo.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class BasicInfo
    {
        public string TargetGroup { get; set; }
        public string QualificationProcedure { get; set; }
    }
}
```

## File: Models/Course.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class Course
    {
        [PrimaryKey]
        [AutoIncrement]
        public int CourseId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        [MaxLength(50)]
        public string CourseType { get; set; }

        [MaxLength(100)]
        public string CourseName { get; set; }

        [MaxLength(20)]
        public string CourseNumber { get; set; }

        [MaxLength(100)]
        public string InstitutionName { get; set; }

        public DateTime CompletionDate { get; set; }

        public int Year { get; set; }

        public int CourseSequenceNumber { get; set; }

        // Pola dla certyfikatów
        public bool HasCertificate { get; set; }

        public string CertificateNumber { get; set; }

        public DateTime? CertificateDate { get; set; }

        // Nowe pole dla rodzaju uznania kursu (stary SMK)
        [MaxLength(100)]
        public string RecognitionType { get; set; }

        // Czy kurs wymaga akceptacji kierownika (stary SMK)
        public bool RequiresApproval { get; set; }

        // Nowe pole dla daty wystawienia certyfikatu (stary SMK)
        public DateTime? CertificateIssueDate { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; } // JSON
    }
}
```

## File: Models/CourseRequirement.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class CourseRequirement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weeks { get; set; }
        public int WorkingDays { get; set; }
        public bool Required { get; set; }
    }
}
```

## File: Models/EducationalActivity.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class EducationalActivity
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ActivityId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public EducationalActivityType Type { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public SyncStatus SyncStatus { get; set; }
    }
}
```

## File: Models/ExaminationComponent.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ExaminationComponent
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
```

## File: Models/ExaminationInfo.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ExaminationInfo
    {
        public string ExamType { get; set; }
        public List<ExaminationComponent> Components { get; set; }
    }
}
```

## File: Models/ExportOptions.cs
```csharp
namespace SledzSpecke.App.Models
{
    public class ExportOptions
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IncludeShifts { get; set; }

        public bool IncludeProcedures { get; set; }

        public bool IncludeInternships { get; set; }

        public bool IncludeCourses { get; set; }

        public bool IncludeSelfEducation { get; set; }

        public bool IncludePublications { get; set; }

        public bool IncludeAbsences { get; set; }

        public bool IncludeEducationalActivities { get; set; }

        public bool IncludeRecognitions { get; set; }

        public bool FormatForOldSMK { get; set; }

        public int? ModuleId { get; set; }
    }
}
```

## File: Models/HolidaysInfo.cs
```csharp
namespace SledzSpecke.App.Models
{
    public class HolidaysInfo
    {
        // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

        public int VacationDays { get; set; }
        public int NationalHolidays { get; set; }
        public int? ExamPreparationDays { get; set; }
    }
}
```

## File: Models/Internship.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class Internship
    {
        [PrimaryKey]
        [AutoIncrement]
        public int InternshipId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        [MaxLength(100)]
        public string InstitutionName { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(100)]
        public string InternshipName { get; set; }

        public int Year { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DaysCount { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRecognition { get; set; }

        public string RecognitionReason { get; set; }

        public int RecognitionDaysReduction { get; set; }

        public bool IsPartialRealization { get; set; }

        public string SupervisorName { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }
    }
}
```

## File: Models/InternshipRequirement.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class InternshipRequirement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weeks { get; set; }
        public int WorkingDays { get; set; }
        public bool IsBasic { get; set; }
        public string Location { get; set; }
        public List<string> ProcedureCodes { get; set; }
    }
}
```

## File: Models/MedicalShift.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class MedicalShift
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ShiftId { get; set; }

        [Indexed]
        public int InternshipId { get; set; }

        public DateTime Date { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public int Year { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [MaxLength(100)]
        public string ApproverName { get; set; }

        [MaxLength(100)]
        public string ApproverRole { get; set; }

        public bool IsApproved
        {
            get
            {
                return this.SyncStatus == SyncStatus.Synced && this.ApprovalDate.HasValue;
            }
        }
        public bool CanBeDeleted
        {
            get
            {
                return !this.IsApproved;
            }
        }
    }
}
```

## File: Models/MedicalShiftsInfo.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class MedicalShiftsInfo
    {
        public double HoursPerWeek { get; set; }
        public string Description { get; set; }
    }
}
```

## File: Models/MedicalShiftsSummary.cs
```csharp
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class MedicalShiftsSummary
    {
        public int TotalHours { get; set; }

        public int TotalMinutes { get; set; }

        public int ApprovedHours { get; set; }

        public int ApprovedMinutes { get; set; }

        public void NormalizeTime()
        {
            if (this.TotalMinutes >= 60)
            {
                this.TotalHours += this.TotalMinutes / 60;
                this.TotalMinutes %= 60;
            }

            if (this.ApprovedMinutes >= 60)
            {
                this.ApprovedHours += this.ApprovedMinutes / 60;
                this.ApprovedMinutes %= 60;
            }
        }

        public static MedicalShiftsSummary CalculateFromShifts(List<MedicalShift> shifts)
        {
            var summary = new MedicalShiftsSummary();

            foreach (var shift in shifts)
            {
                summary.TotalHours += shift.Hours;
                summary.TotalMinutes += shift.Minutes;

                if (shift.SyncStatus == SyncStatus.Synced)
                {
                    summary.ApprovedHours += shift.Hours;
                    summary.ApprovedMinutes += shift.Minutes;
                }
            }

            summary.NormalizeTime();

            return summary;
        }
    }
}
```

## File: Models/Module.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Module
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ModuleId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public ModuleType Type { get; set; }

        [Indexed]
        public SmkVersion SmkVersion { get; set; }

        public string Version { get; set; } // np. "CMKP 2014" lub "CMKP 2023"

        [MaxLength(100)]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Structure { get; set; }

        public int CompletedInternships { get; set; }

        public int TotalInternships { get; set; }

        public int CompletedCourses { get; set; }

        public int TotalCourses { get; set; }

        public int CompletedProceduresA { get; set; }

        public int TotalProceduresA { get; set; }

        public int CompletedProceduresB { get; set; }

        public int TotalProceduresB { get; set; }

        public int CompletedShiftHours { get; set; }

        public int RequiredShiftHours { get; set; }

        public double WeeklyShiftHours { get; set; }

        public int CompletedSelfEducationDays { get; set; }

        public int TotalSelfEducationDays { get; set; }
    }
}
```

## File: Models/ModuleInfo.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ModuleInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
```

## File: Models/ModuleStructure.cs
```csharp
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ModuleStructure
    {
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public ModuleType ModuleType { get; set; }
        public string Version { get; set; }
        public TotalDuration Duration { get; set; }
        public int WorkingDays { get; set; }
        public List<CourseRequirement> Courses { get; set; }
        public List<InternshipRequirement> Internships { get; set; }
        public List<ProcedureRequirement> Procedures { get; set; }
        public SelfEducationInfo SelfEducation { get; set; }
        public HolidaysInfo Holidays { get; set; }
        public MedicalShiftsInfo MedicalShifts { get; set; }
        public ProcedureCodeDescription ProcedureCodeDescription { get; set; }
        public ExaminationInfo ExaminationInfo { get; set; }

        public int RequiredShiftHours { get; set; }
        public int SelfEducationDays { get; set; }
    }
}
```

## File: Models/NotificationSettings.cs
```csharp
namespace SledzSpecke.App.Models
{
    public class NotificationSettings
    {
        public bool NotificationsEnabled { get; set; }

        public bool ShiftRemindersEnabled { get; set; }

        public bool CourseRemindersEnabled { get; set; }

        public bool DeadlineRemindersEnabled { get; set; }

        public int ReminderDaysInAdvance { get; set; } = 7;
    }
}
```

## File: Models/Procedure.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Procedure
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProcedureId { get; set; }

        [Indexed]
        public int InternshipId { get; set; }

        public DateTime Date { get; set; }

        public int Year { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(10)]
        public string OperatorCode { get; set; }

        [MaxLength(100)]
        public string PerformingPerson { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [MaxLength(10)]
        public string PatientInitials { get; set; }

        [MaxLength(1)]
        public string PatientGender { get; set; }

        public string AssistantData { get; set; }

        public string ProcedureGroup { get; set; }

        [MaxLength(20)]
        public string Status { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }
    }
}
```

## File: Models/ProcedureCodeDescription.cs
```csharp
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ProcedureCodeDescription
    {
        public string CodeA { get; set; }
        public string CodeB { get; set; }
    }
}
```

## File: Models/ProcedureDefinition.cs
```csharp
namespace SledzSpecke.App.Models
{
    public class ProcedureDefinition
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int RequiredCountA { get; set; }

        public int RequiredCountB { get; set; }

        public string Group { get; set; }

        public int? InternshipTypeId { get; set; }
    }
}
```

## File: Models/ProcedureRequirement.cs
```csharp
namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class ProcedureRequirement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int RequiredCountA { get; set; }
        public int RequiredCountB { get; set; }
        public int? InternshipId { get; set; }
    }
}
```

## File: Models/ProcedureSummary.cs
```csharp
namespace SledzSpecke.App.Models
{
    public class ProcedureSummary
    {
        public int RequiredCountA { get; set; }
        public int RequiredCountB { get; set; }
        public int CompletedCountA { get; set; }
        public int CompletedCountB { get; set; }
        public int ApprovedCountA { get; set; }
        public int ApprovedCountB { get; set; }

        public int RemainingCountA => this.RequiredCountA - this.CompletedCountA;
        public int RemainingCountB => this.RequiredCountB - this.CompletedCountB;
    }
}
```

## File: Models/Publication.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Publication
    {
        [PrimaryKey]
        [AutoIncrement]
        public int PublicationId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }
    }
}
```

## File: Models/RealizedInternshipBase.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public abstract class RealizedInternshipBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int RealizedInternshipId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [MaxLength(100)]
        public string InstitutionName { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(100)]
        public string InternshipName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DaysCount { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsApproved { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }

        [MaxLength(100)]
        public string SupervisorName { get; set; }

        [Ignore]
        public string DateRange => $"{this.StartDate:dd.MM.yyyy} - {this.EndDate:dd.MM.yyyy}";
    }
}
```

## File: Models/RealizedInternshipNewSMK.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedInternshipNewSMK : RealizedInternshipBase
    {
        [Indexed]
        public int InternshipRequirementId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public bool IsRecognition { get; set; }

        public string RecognitionReason { get; set; }

        public int RecognitionDaysReduction { get; set; }

        public bool IsPartialRealization { get; set; }
    }
}
```

## File: Models/RealizedInternshipOldSMK.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedInternshipOldSMK : RealizedInternshipBase
    {
        public int Year { get; set; }

        public bool RequiresApproval { get; set; }
    }
}
```

## File: Models/RealizedMedicalShiftBase.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public abstract class RealizedMedicalShiftBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ShiftId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public SyncStatus SyncStatus { get; set; }

        [Ignore]
        public string FormattedTime => $"{this.Hours} godz. {this.Minutes} min.";
    }
}
```

## File: Models/RealizedMedicalShiftNewSMK.cs
```csharp
using System;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedMedicalShiftNewSMK : RealizedMedicalShiftBase
    {
        [Indexed]
        public int InternshipRequirementId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Ignore]
        public string InternshipName { get; set; }
    }
}
```

## File: Models/RealizedMedicalShiftOldSMK.cs
```csharp
using System;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedMedicalShiftOldSMK : RealizedMedicalShiftBase
    {
        public int Year { get; set; }
        public DateTime StartDate { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }
    }
}
```

## File: Models/RealizedProcedureBase.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public abstract class RealizedProcedureBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProcedureId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public DateTime Date { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public string AdditionalFields { get; set; }
    }
}
```

## File: Models/RealizedProcedureNewSMK.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedProcedureNewSMK : RealizedProcedureBase
    {
        [Indexed]
        public int ProcedureRequirementId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public int CountA { get; set; }
        public int CountB { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Ignore]
        public string ProcedureName { get; set; }

        [Ignore]
        public string InternshipName { get; set; }

        [Ignore]
        public string DateRange => $"{this.StartDate:dd.MM.yyyy} - {this.EndDate:dd.MM.yyyy}";
    }
}
```

## File: Models/RealizedProcedureOldSMK.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedProcedureOldSMK : RealizedProcedureBase
    {
        public int Year { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(100)]
        public string PerformingPerson { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [Indexed]
        public int InternshipId { get; set; }

        [MaxLength(100)]
        public string InternshipName { get; set; }

        [MaxLength(10)]
        public string PatientInitials { get; set; }

        [MaxLength(1)]
        public string PatientGender { get; set; }

        public string AssistantData { get; set; }

        public string ProcedureGroup { get; set; }

        [Indexed]
        public int? ProcedureRequirementId { get; set; }
    }
}
```

## File: Models/Recognition.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Recognition
    {
        [PrimaryKey]
        [AutoIncrement]
        public int RecognitionId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public RecognitionType Type { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DaysReduction { get; set; }

        public SyncStatus SyncStatus { get; set; }
    }
}
```

## File: Models/SelfEducation.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class SelfEducation
    {
        [PrimaryKey]
        [AutoIncrement]
        public int SelfEducationId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public int Year { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Publisher { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }
    }
}
```

## File: Models/SelfEducationInfo.cs
```csharp
namespace SledzSpecke.App.Models
{
    public class SelfEducationInfo
    {
        public int DaysPerYear { get; set; }

        public int TotalDays { get; set; }

        public string Description { get; set; }
    }
}
```

## File: Models/Specialization.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Specialization
    {
        [PrimaryKey]
        [AutoIncrement]
        public int SpecializationId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string ProgramCode { get; set; }

        public SmkVersion SmkVersion { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime PlannedEndDate { get; set; }

        public DateTime CalculatedEndDate { get; set; }

        public string ProgramStructure { get; set; }

        public int? CurrentModuleId { get; set; }

        [Ignore]
        public List<Module> Modules { get; set; } = new List<Module>();

        public int CompletedInternships { get; set; }

        public int TotalInternships { get; set; }

        public int CompletedCourses { get; set; }

        public int TotalCourses { get; set; }

        public int DurationYears { get; set; }
    }
}
```

## File: Models/SpecializationProgram.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class SpecializationProgram
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProgramId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        public string Version { get; set; }

        public string Structure { get; set; }

        public SmkVersion SmkVersion { get; set; }

        public bool HasBasicModule { get; set; }

        public string BasicModuleCode { get; set; }

        public string SpecialisticModuleCode { get; set; }

        public int DurationYears { get; set; }

        public int DurationMonths { get; set; }

        public int DurationDays { get; set; }

        public int TotalWorkingDays { get; set; }

        [Ignore]
        public TotalDuration TotalDuration
        {
            get
            {
                return new TotalDuration
                {
                    Years = this.DurationYears,
                    Months = this.DurationMonths,
                    Days = this.DurationDays
                };
            }
            set
            {
                if (value != null)
                {
                    this.DurationYears = value.Years;
                    this.DurationMonths = value.Months;
                    this.DurationDays = value.Days;
                }
            }
        }
    }
}
```

## File: Models/SpecializationStatistics.cs
```csharp
using System;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class SpecializationStatistics
    {
        public int CompletedInternships { get; set; }

        public int RequiredInternships { get; set; }

        public int CompletedInternshipDays { get; set; }

        public int RequiredInternshipWorkingDays { get; set; }
        public int CompletedCourses { get; set; }

        public int RequiredCourses { get; set; }

        public int CompletedShiftHours { get; set; }

        public int RequiredShiftHours { get; set; }

        public int CompletedProceduresA { get; set; }

        public int RequiredProceduresA { get; set; }

        public int CompletedProceduresB { get; set; }

        public int RequiredProceduresB { get; set; }

        public int SelfEducationDaysUsed { get; set; }

        public int SelfEducationDaysTotal { get; set; }

        public int EducationalActivitiesCompleted { get; set; }

        public int PublicationsCompleted { get; set; }

        public int AbsenceDays { get; set; }

        public int AbsenceDaysExtendingSpecialization { get; set; }

        public double GetOverallProgress()
        {
            const double internshipWeight = 0.35;
            const double courseWeight = 0.25;
            const double procedureWeight = 0.30;
            const double otherWeight = 0.10;

            double internshipProgress = this.RequiredInternships > 0
                ? (double)this.CompletedInternships / this.RequiredInternships
                : 0;

            double courseProgress = this.RequiredCourses > 0
                ? (double)this.CompletedCourses / this.RequiredCourses
                : 0;

            double procedureProgress;

            if (this.RequiredProceduresA + this.RequiredProceduresB > 0)
            {
                double procedureAProgress = this.RequiredProceduresA > 0
                    ? (double)this.CompletedProceduresA / this.RequiredProceduresA
                    : 0;

                double procedureBProgress = this.RequiredProceduresB > 0
                    ? (double)this.CompletedProceduresB / this.RequiredProceduresB
                    : 0;

                procedureProgress =
                    (procedureAProgress * this.RequiredProceduresA +
                     procedureBProgress * this.RequiredProceduresB) /
                    (this.RequiredProceduresA + this.RequiredProceduresB);
            }
            else
            {
                procedureProgress = 0;
            }

            double otherActivitiesProgress = 0;
            int totalOtherItems = this.PublicationsCompleted + this.EducationalActivitiesCompleted;

            if (this.SelfEducationDaysTotal > 0)
            {
                double selfEducationProgress = Math.Min(1.0, (double)this.SelfEducationDaysUsed / this.SelfEducationDaysTotal);

                if (totalOtherItems == 0)
                {
                    otherActivitiesProgress = selfEducationProgress;
                }
                else
                {
                    double otherProgress = Math.Min(1.0, totalOtherItems / 10.0);
                    otherActivitiesProgress = (selfEducationProgress + otherProgress) / 2.0;
                }
            }
            else if (totalOtherItems > 0)
            {
                otherActivitiesProgress = Math.Min(1.0, totalOtherItems / 10.0);
            }

            double overallProgress =
                (internshipProgress * internshipWeight) +
                (courseProgress * courseWeight) +
                (procedureProgress * procedureWeight) +
                (otherActivitiesProgress * otherWeight);

            return Math.Min(1.0, overallProgress);
        }
    }
}
```

## File: Models/SpecializationStructure.cs
```csharp
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class SpecializationStructure
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Version { get; set; }
        public TotalDuration TotalDuration { get; set; }
        public int TotalWorkingDays { get; set; }
        public BasicInfo BasicInfo { get; set; }
        public List<ModuleStructure> Modules { get; set; }
        public SelfEducationInfo SelfEducation { get; set; }
        public HolidaysInfo Holidays { get; set; }
        public MedicalShiftsInfo MedicalShifts { get; set; }
        public ProcedureCodeDescription ProcedureCodeDescription { get; set; }
        public ExaminationInfo ExaminationInfo { get; set; }
    }
}
```

## File: Models/TotalDuration.cs
```csharp
using SQLite;

namespace SledzSpecke.App.Models
{
    public class TotalDuration
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }

        [Ignore]
        public int TotalMonths => (Years * 12) + Months;
    }
}
```

## File: Models/User.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class User
    {
        [PrimaryKey]
        [AutoIncrement]
        public int UserId { get; set; }

        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public SmkVersion SmkVersion { get; set; }

        public int SpecializationId { get; set; }

        public DateTime RegistrationDate { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }
    }
}
```

## File: Platforms/Android/Resources/values/colors.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
  <color name="colorPrimary">#24C1DE</color>
  <color name="colorPrimaryDark">#0D759C</color>
  <color name="colorAccent">#30DDE8</color>
</resources>
```

## File: Platforms/Android/AndroidManifest.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-sdk 
        android:minSdkVersion="21" 
        android:targetSdkVersion="35" />
    <application
        android:label="SledzSpecke"
        android:allowBackup="true" 
        android:icon="@mipmap/appicon"
        android:roundIcon="@mipmap/appicon_round"
        android:supportsRtl="true"
        android:enableOnBackInvokedCallback="true">
    </application>
    
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

    
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

    
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    <uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
</manifest>
```

## File: Platforms/Android/MainActivity.cs
```csharp
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SledzSpecke.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
```

## File: Platforms/Android/MainApplication.cs
```csharp
using Android.App;
using Android.Runtime;

namespace SledzSpecke.App.Platforms.Android;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(nint handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## File: Platforms/iOS/AppDelegate.cs
```csharp
using Foundation;

namespace SledzSpecke.App;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## File: Platforms/iOS/Info.plist
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>LSRequiresIPhoneOS</key>
    <true/>
    <key>UIDeviceFamily</key>
    <array>
        <integer>1</integer>
        <integer>2</integer>
    </array>
    <key>UIRequiredDeviceCapabilities</key>
    <array>
        <string>arm64</string>
    </array>
    <key>UISupportedInterfaceOrientations</key>
    <array>
        <string>UIInterfaceOrientationPortrait</string>
        <string>UIInterfaceOrientationLandscapeLeft</string>
        <string>UIInterfaceOrientationLandscapeRight</string>
    </array>
    <key>UISupportedInterfaceOrientations~ipad</key>
    <array>
        <string>UIInterfaceOrientationPortrait</string>
        <string>UIInterfaceOrientationPortraitUpsideDown</string>
        <string>UIInterfaceOrientationLandscapeLeft</string>
        <string>UIInterfaceOrientationLandscapeRight</string>
    </array>
    <key>XSAppIconAssets</key>
    <string>Assets.xcassets/appicon.appiconset</string>
    
    <key>NSFaceIDUsageDescription</key>
    <string>This app uses Face ID to secure your medical data and provide quick access.</string>
</dict>
</plist>
```

## File: Platforms/iOS/Program.cs
```csharp
using UIKit;

namespace SledzSpecke.App;

public static class Program
{
    public static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
```

## File: Platforms/MacCatalyst/AppDelegate.cs
```csharp
using Foundation;

namespace SledzSpecke.App.Platforms.MacCatalyst;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## File: Platforms/MacCatalyst/Entitlements.plist
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
    
    <dict>
        
        <key>com.apple.security.app-sandbox</key>
        <true/>
        
        <key>com.apple.security.network.client</key>
        <true/>
    </dict>
</plist>
```

## File: Platforms/MacCatalyst/Info.plist
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    
    
    
    

    
    
    
    
    <key>UIDeviceFamily</key>
    <array>
        <integer>2</integer>
    </array>
    <key>UIRequiredDeviceCapabilities</key>
    <array>
        <string>arm64</string>
    </array>
    <key>UISupportedInterfaceOrientations</key>
    <array>
        <string>UIInterfaceOrientationPortrait</string>
        <string>UIInterfaceOrientationLandscapeLeft</string>
        <string>UIInterfaceOrientationLandscapeRight</string>
    </array>
    <key>UISupportedInterfaceOrientations~ipad</key>
    <array>
        <string>UIInterfaceOrientationPortrait</string>
        <string>UIInterfaceOrientationPortraitUpsideDown</string>
        <string>UIInterfaceOrientationLandscapeLeft</string>
        <string>UIInterfaceOrientationLandscapeRight</string>
    </array>
    <key>XSAppIconAssets</key>
    <string>Assets.xcassets/appicon.appiconset</string>
</dict>
</plist>
```

## File: Platforms/MacCatalyst/Program.cs
```csharp
using UIKit;

namespace SledzSpecke.App.Platforms.MacCatalyst;

public static class Program
{
    public static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
```

## File: Platforms/Tizen/Main.cs
```csharp
using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace SledzSpecke.App;

class Program : MauiApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    static void Main(string[] args)
    {
        var app = new Program();
        app.Run(args);
    }
}
```

## File: Platforms/Tizen/tizen-manifest.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest package="maui-application-id-placeholder" version="0.0.0" api-version="9" xmlns="http://tizen.org/ns/packages">
  <profile name="common" />
  <ui-application appid="maui-application-id-placeholder" exec="SledzSpecke.App.dll" multiple="false" nodisplay="false" taskmanage="true" type="dotnet" launch_mode="single">
    <label>maui-application-title-placeholder</label>
    <icon>maui-appicon-placeholder</icon>
    <metadata key="http://tizen.org/metadata/prefer_dotnet_aot" value="true" />
  </ui-application>
  <shortcut-list />
  <privileges>
    <privilege>http://tizen.org/privilege/internet</privilege>
  </privileges> 
  <dependencies />
  <provides-appdefined-privileges />
</manifest>
```

## File: Platforms/Windows/app.manifest
```
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <assemblyIdentity version="1.0.0.0" name="SledzSpecke.App.WinUI.app"/>
  <application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/PM</dpiAware>
      <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2, PerMonitor</dpiAwareness>
    </windowsSettings>
  </application>
</assembly>
```

## File: Platforms/Windows/App.xaml
```
<maui:MauiWinUIApplication
    x:Class="SledzSpecke.App.WinUI.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:maui="using:Microsoft.Maui"
    xmlns:local="using:SledzSpecke.App.WinUI">

</maui:MauiWinUIApplication>
```

## File: Platforms/Windows/App.xaml.cs
```csharp
namespace SledzSpecke.App.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## File: Platforms/Windows/Package.appxmanifest
```
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">

  <Identity Name="maui-package-name-placeholder" Publisher="CN=User Name" Version="0.0.0.0" />

  <mp:PhoneIdentity PhoneProductId="CB25DF1C-9023-45E6-ACBD-9F8B009AE8A8" PhonePublisherId="00000000-0000-0000-0000-000000000000"/>

  <Properties>
    <DisplayName>$placeholder$</DisplayName>
    <PublisherDisplayName>User Name</PublisherDisplayName>
    <Logo>$placeholder$.png</Logo>
  </Properties>

  <Dependencies>
    <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
  </Dependencies>

  <Resources>
    <Resource Language="x-generate" />
  </Resources>

  <Applications>
    <Application Id="App" Executable="$targetnametoken$.exe" EntryPoint="$targetentrypoint$">
      <uap:VisualElements
        DisplayName="$placeholder$"
        Description="$placeholder$"
        Square150x150Logo="$placeholder$.png"
        Square44x44Logo="$placeholder$.png"
        BackgroundColor="transparent">
        <uap:DefaultTile Square71x71Logo="$placeholder$.png" Wide310x150Logo="$placeholder$.png" Square310x310Logo="$placeholder$.png" />
        <uap:SplashScreen Image="$placeholder$.png" />
      </uap:VisualElements>
    </Application>
  </Applications>

  <Capabilities>
    <rescap:Capability Name="runFullTrust" />
  </Capabilities>

</Package>
```

## File: Properties/launchSettings.json
```json
{
  "profiles": {
    "Windows Machine": {
      "commandName": "Project",
      "nativeDebugging": false
    }
  }
}
```

## File: Resources/Raw/SpecializationTemplates/cardiology_new.json
```json
{
  "name": "Kardiologia",
  "code": "cardiology",
  "version": "CMKP 2023",
  "totalDuration": {
    "years": 5,
    "months": 0,
    "days": 0
  },
  "totalWorkingDays": 1182,
  "basicInfo": {
    "targetGroup": "Lekarze nieposiadający specjalizacji I lub II stopnia lub tytułu specjalisty w odpowiedniej dziedzinie medycyny",
    "qualificationProcedure": "wiosna 2023"
  },
  "modules": [
    {
      "moduleId": 1,
      "name": "Moduł podstawowy w zakresie chorób wewnętrznych",
      "code": "internal_medicine",
      "moduleType": "Basic",
      "version": "CMKP 2023",
      "duration": {
        "years": 2,
        "months": 0,
        "days": 0
      },
      "workingDays": 444,
      "courses": [
        {
          "id": 1,
          "name": "Kurs: \"Diagnostyka obrazowa\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 2,
          "name": "Kurs: \"Alergologia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 3,
          "name": "Kurs: \"Onkologia kliniczna\"",
          "weeks": 0.8,
          "workingDays": 4,
          "required": true
        },
        {
          "id": 4,
          "name": "Kurs: \"Medycyna paliatywna\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 5,
          "name": "Kurs: \"Toksykologia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 6,
          "name": "Kurs: \"Geriatria\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 7,
          "name": "Kurs: \"Diabetologia\"",
          "weeks": 0.8,
          "workingDays": 4,
          "required": true
        },
        {
          "id": 8,
          "name": "Kurs: \"Przetaczanie krwi i jej składników\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 9,
          "name": "Kurs: \"Orzecznictwo lekarskie\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 10,
          "name": "Kurs: \"Profilaktyka i promocja zdrowia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        }
      ],
      "internships": [
        {
          "id": 1,
          "name": "Staż podstawowy w zakresie chorób wewnętrznych",
          "weeks": 67,
          "workingDays": 335,
          "isBasic": true,
          "location": "oddział chorób wewnętrznych"
        },
        {
          "id": 2,
          "name": "Staż kierunkowy w zakresie intensywnej opieki medycznej",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": null
        },
        {
          "id": 3,
          "name": "Staż kierunkowy w szpitalnym oddziale ratunkowym",
          "weeks": 12,
          "workingDays": 60,
          "isBasic": false,
          "location": "szpitalny oddział ratunkowy (SOR), który posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie medycyny ratunkowej"
        }
      ],
      "procedures": [
        {
          "id": 1,
          "name": "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
          "type": "Staż podstawowy",
          "requiredCountA": 3,
          "requiredCountB": 3,
          "internshipId": 1
        },
        {
          "id": 2,
          "name": "nakłucie jamy opłucnej w przypadku płynu",
          "type": "Staż podstawowy",
          "requiredCountA": 10,
          "requiredCountB": 3,
          "internshipId": 1
        },
        {
          "id": 3,
          "name": "nakłucie jamy otrzewnej w przypadku wodobrzusza",
          "type": "Staż podstawowy",
          "requiredCountA": 10,
          "requiredCountB": 3,
          "internshipId": 1
        },
        {
          "id": 4,
          "name": "nakłucie żył obwodowych – iniekcje dożylne, pobrania krwi obwodowej",
          "type": "Staż podstawowy",
          "requiredCountA": 30,
          "requiredCountB": 5,
          "internshipId": 1
        },
        {
          "id": 5,
          "name": "nakłucie tętnicy obwodowej w celu pobrania krwi do badania gazometrycznego",
          "type": "Staż podstawowy",
          "requiredCountA": 30,
          "requiredCountB": 5,
          "internshipId": 1
        },
        {
          "id": 6,
          "name": "pomiar ośrodkowego ciśnienia żylnego",
          "type": "Staż podstawowy",
          "requiredCountA": 6,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 7,
          "name": "cewnikowanie pęcherza moczowego",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 4,
          "internshipId": 1
        },
        {
          "id": 8,
          "name": "badanie per rectum",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 9,
          "name": "przetoczenie krwi lub preparatu krwiopochodnego",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 10,
          "name": "wprowadzenie zgłębnika do żołądka",
          "type": "Staż podstawowy",
          "requiredCountA": 5,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 11,
          "name": "wykonanie i interpretacja 12-odprowadzeniowego EKG",
          "type": "Staż podstawowy",
          "requiredCountA": 30,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 12,
          "name": "badanie palpacyjne gruczołu piersiowego",
          "type": "Staż podstawowy",
          "requiredCountA": 10,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 13,
          "name": "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 1,
          "internshipId": 3
        },
        {
          "id": 14,
          "name": "kardiowersja elektryczna",
          "type": "Staże kierunkowe",
          "requiredCountA": 3,
          "requiredCountB": 3,
          "internshipId": 2
        },
        {
          "id": 15,
          "name": "intubacja dotchawicza",
          "type": "Staże kierunkowe",
          "requiredCountA": 10,
          "requiredCountB": 3,
          "internshipId": 2
        },
        {
          "id": 16,
          "name": "pomiar szczytowego przepływu wydechowego",
          "type": "Staże kierunkowe",
          "requiredCountA": 3,
          "requiredCountB": 3,
          "internshipId": 3
        }
      ],
      "selfEducation": {
        "daysPerYear": 6,
        "totalDays": 12,
        "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
      },
      "holidays": {
        "vacationDays": 52,
        "nationalHolidays": 26,
        "examPreparationDays": null
      },
      "medicalShifts": {
        "hoursPerWeek": 10.08,
        "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym."
      },
      "procedureCodeDescription": {
        "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
        "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
      }
    },
    {
      "moduleId": 2,
      "name": "Moduł specjalistyczny w zakresie kardiologii",
      "code": "cardiology_specialistic",
      "moduleType": "Specialistic",
      "version": "CMKP 2023",
      "duration": {
        "years": 3,
        "months": 0,
        "days": 0
      },
      "workingDays": 660,
      "courses": [
        {
          "id": 1,
          "name": "Kurs wprowadzający: \"Wprowadzenie do specjalizacji w dziedzinie kardiologii\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 2,
          "name": "Kurs: \"Patofizjologia chorób sercowo-naczyniowych\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 3,
          "name": "Kurs: \"Farmakoterapia chorób sercowo-naczyniowych\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 4,
          "name": "Kurs: \"Nieinwazyjna diagnostyka elektrokardiograficzna\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 5,
          "name": "Kurs: \"Diagnostyka obrazowa – echokardiografia\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 6,
          "name": "Kurs: \"Diagnostyka obrazowa – nowe techniki obrazowania\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 7,
          "name": "Kurs: \"Intensywna terapia kardiologiczna\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 8,
          "name": "Kurs: \"Elektrofizjologia i elektroterapia\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 9,
          "name": "Kurs: \"Diagnostyka inwazyjna i leczenie interwencyjne\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 10,
          "name": "Kurs: \"Wrodzone i nabyte wady serca\"",
          "weeks": 0.8,
          "workingDays": 4,
          "required": true
        },
        {
          "id": 11,
          "name": "Kurs: \"Onkologia w kardiologii\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 12,
          "name": "Kurs: \"Przewlekły zespół wieńcowy\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 13,
          "name": "Kurs: \"Nadciśnienie tętnicze\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 14,
          "name": "Kurs: \"Nadciśnienie płucne i niewydolność prawej komory serca\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 15,
          "name": "Kurs: \"Niewydolność serca\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 16,
          "name": "Kurs: \"Diagnostyka i leczenie chorób naczyń obwodowych\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 17,
          "name": "Kurs: \"Choroby rzadkie w kardiologii\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 18,
          "name": "Kurs atestacyjny (podsumowujący): \"Kardiologia\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        }
      ],
      "internships": [
        {
          "id": 1,
          "name": "Staż podstawowy w zakresie kardiologii",
          "weeks": 64,
          "workingDays": 320,
          "isBasic": true,
          "location": "oddział kardiologii posiadający akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiologii oraz poradnia będąca w strukturze oddziału/jednostki"
        },
        {
          "id": 2,
          "name": "Staż kierunkowy w zakresie intensywnej terapii kardiologicznej",
          "weeks": 16,
          "workingDays": 80,
          "isBasic": false,
          "location": "oddział intensywnej terapii kardiologicznej lub oddział kardiologii z łóżkami intensywnego nadzoru kardiologicznego"
        },
        {
          "id": 3,
          "name": "Staż kierunkowy w zakresie elektrofizjologii i elektroterapii",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "zakład lub pracownia elektrofizjologii będąca w strukturze oddziału/szpitala posiadającego akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiologii"
        },
        {
          "id": 4,
          "name": "Staż kierunkowy w zakresie kardiologii interwencyjnej",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "zakład lub pracownia kardiologii interwencyjnej będąca w strukturze oddziału/szpitala posiadającego akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiologii"
        },
        {
          "id": 5,
          "name": "Staż kierunkowy w zakresie echokardiografii",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "zakład lub pracownia echokardiograficzna będąca w strukturze oddziału/szpitala posiadającego akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiologii"
        },
        {
          "id": 6,
          "name": "Staż kierunkowy w zakresie kardiologii dziecięcej",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "oddział kardiologii dziecięcej posiadający akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiologii dziecięcej"
        },
        {
          "id": 7,
          "name": "Staż kierunkowy w zakresie nabytych i wrodzonych wad serca u dorosłych",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "oddział kardiologii zajmujący się diagnostyką i leczeniem wad serca posiadający akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiologii"
        },
        {
          "id": 8,
          "name": "Staż kierunkowy w zakresie kardiochirurgii",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "oddział kardiochirurgii posiadający akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiochirurgii"
        },
        {
          "id": 9,
          "name": "Staż kierunkowy w zakresie nieinwazyjnej diagnostyki elektrokardiograficznej",
          "weeks": 6,
          "workingDays": 30,
          "isBasic": false,
          "location": "oddział kardiologii posiadający akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie kardiologii"
        },
        {
          "id": 10,
          "name": "Staż kierunkowy w zakresie radiologii i diagnostyki obrazowej",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia specjalizacji w dziedzinie radiologii i diagnostyki obrazowej"
        },
        {
          "id": 11,
          "name": "Staż kierunkowy w zakresie angiologii",
          "weeks": 2,
          "workingDays": 10,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie angiologii lub chirurgii naczyniowej"
        }
      ],
      "procedures": [
        {
          "id": 1,
          "name": "badanie elektrokardiograficzne",
          "type": "Staż podstawowy",
          "requiredCountA": 200,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 2,
          "name": "24-godzinne ambulatoryjne monitorowanie EKG",
          "type": "Staż podstawowy",
          "requiredCountA": 50,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 3,
          "name": "24-godzinne ambulatoryjne monitorowanie ciśnienia tętniczego",
          "type": "Staż podstawowy",
          "requiredCountA": 50,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 4,
          "name": "próba wysiłkowa elektrokardiograficzna",
          "type": "Staż podstawowy",
          "requiredCountA": 50,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 5,
          "name": "badanie echokardiograficzne przezklatkowe",
          "type": "Staż podstawowy",
          "requiredCountA": 200,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 6,
          "name": "badanie echokardiograficzne przezprzełykowe",
          "type": "Staż podstawowy",
          "requiredCountA": 0,
          "requiredCountB": 50,
          "internshipId": 5
        },
        {
          "id": 7,
          "name": "badanie echokardiograficzne obciążeniowe",
          "type": "Staż podstawowy",
          "requiredCountA": 0,
          "requiredCountB": 50,
          "internshipId": 5
        },
        {
          "id": 8,
          "name": "nakłucie osierdzia, opłucnej, otrzewnej",
          "type": "Staż podstawowy",
          "requiredCountA": 25,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 9,
          "name": "wszczepienie stymulatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "internshipId": 3
        },
        {
          "id": 10,
          "name": "wszczepienie kardiowertera-defibrylatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "internshipId": 3
        },
        {
          "id": 11,
          "name": "kontrola stymulatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 3
        },
        {
          "id": 12,
          "name": "kontrola kardiowertera-defibrylatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 3
        },
        {
          "id": 13,
          "name": "badanie elektrofizjologiczne",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "internshipId": 3
        },
        {
          "id": 14,
          "name": "ablacja podłoża zaburzeń rytmu",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "internshipId": 3
        },
        {
          "id": 15,
          "name": "koronarografia",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 50,
          "internshipId": 4
        },
        {
          "id": 16,
          "name": "wprowadzenie elektrody endokawitarnej do stymulacji zewnętrznej",
          "type": "Wykonanie zabiegów ratujących życie",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 2
        },
        {
          "id": 17,
          "name": "wykonanie centralnego wkłucia żylnego",
          "type": "Wykonanie zabiegów ratujących życie",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 2
        },
        {
          "id": 18,
          "name": "wykonanie wkłucia do tętnicy obwodowej",
          "type": "Wykonanie zabiegów ratujących życie",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 2
        },
        {
          "id": 19,
          "name": "intubacja dotchawicza",
          "type": "Wykonanie zabiegów ratujących życie",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 2
        },
        {
          "id": 20,
          "name": "prowadzenie resuscytacji, różnych metod stymulacji serca, kardiowersji i defibrylacji serca",
          "type": "Wykonanie zabiegów ratujących życie",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 2
        },
        {
          "id": 21,
          "name": "nakłucie osierdzia, opłucnej, otrzewnej",
          "type": "Wykonanie zabiegów ratujących życie",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 2
        }
      ],
      "selfEducation": {
        "daysPerYear": 6,
        "totalDays": 18,
        "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
      },
      "holidays": {
        "vacationDays": 78,
        "nationalHolidays": 39,
        "examPreparationDays": 6
      },
      "medicalShifts": {
        "hoursPerWeek": 10.08,
        "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym."
      },
      "procedureCodeDescription": {
        "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
        "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
      },
      "examinationInfo": {
        "examType": "Państwowy Egzamin Specjalizacyjny",
        "components": [
          {
            "name": "egzamin testowy",
            "description": "zbiór pytań z zakresu wymaganej wiedzy określonej w programie specjalizacji, zawierających pięć wariantów odpowiedzi, z których tylko jeden jest prawidłowy"
          },
          {
            "name": "egzamin ustny",
            "description": "pytania problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji oraz ocenę trzech elektrokardiogramów, dwóch koronarografii, dwóch badań obrazowych (w tym co najmniej jedno badanie echokardiograficzne)"
          }
        ]
      }
    }
  ],
  "selfEducation": {
    "daysPerYear": 6,
    "totalDays": 30,
    "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
  },
  "holidays": {
    "vacationDays": 130,
    "nationalHolidays": 65,
    "examPreparationDays": 6
  },
  "medicalShifts": {
    "hoursPerWeek": 10.08,
    "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym."
  },
  "procedureCodeDescription": {
    "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
    "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
  },
  "examinationInfo": {
    "examType": "Państwowy Egzamin Specjalizacyjny",
    "components": [
      {
        "name": "egzamin testowy",
        "description": "zbiór pytań z zakresu wymaganej wiedzy określonej w programie specjalizacji, zawierających pięć wariantów odpowiedzi, z których tylko jeden jest prawidłowy"
      },
      {
        "name": "egzamin ustny",
        "description": "pytania problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji oraz ocenę trzech elektrokardiogramów, dwóch koronarografii, dwóch badań obrazowych (w tym co najmniej jedno badanie echokardiograficzne)"
      }
    ]
  }
}
```

## File: Resources/Raw/SpecializationTemplates/cardiology_old.json
```json
{
  "name": "Kardiologia",
  "code": "cardiology",
  "version": "CMKP 2014",
  "totalDuration": {
    "years": 5,
    "months": 0,
    "days": 0
  },
  "totalWorkingDays": 1305,
  "basicInfo": {
    "targetGroup": "Lekarze nieposiadający specjalizacji I lub II stopnia lub tytułu specjalisty w odpowiedniej dziedzinie medycyny",
    "qualificationProcedure": "CMKP 2014"
  },
  "modules": [
    {
      "moduleId": 1,
      "name": "Moduł podstawowy w zakresie chorób wewnętrznych",
      "code": "internal_medicine",
      "moduleType": "Basic",
      "version": "CMKP 2014",
      "duration": {
        "years": 2,
        "months": 0,
        "days": 0
      },
      "workingDays": 522,
      "courses": [
        {
          "id": 1,
          "name": "Kurs: \"Diagnostyka obrazowa\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 2,
          "name": "Kurs: \"Alergologia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 3,
          "name": "Kurs: \"Onkologia kliniczna\"",
          "weeks": 0.8,
          "workingDays": 4,
          "required": true
        },
        {
          "id": 4,
          "name": "Kurs: \"Medycyna paliatywna\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 5,
          "name": "Kurs: \"Toksykologia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 6,
          "name": "Kurs: \"Geriatria\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 7,
          "name": "Kurs: \"Diabetologia\"",
          "weeks": 0.8,
          "workingDays": 4,
          "required": true
        },
        {
          "id": 8,
          "name": "Kurs: \"Przetaczanie krwi i jej składników\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 9,
          "name": "Kurs: \"Orzecznictwo lekarskie\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 10,
          "name": "Kurs: \"Profilaktyka i promocja zdrowia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        }
      ],
      "internships": [
        {
          "id": 1,
          "name": "Staż podstawowy w zakresie chorób wewnętrznych",
          "weeks": 70,
          "workingDays": 350,
          "isBasic": true,
          "location": "oddział chorób wewnętrznych"
        },
        {
          "id": 2,
          "name": "Staż kierunkowy w zakresie intensywnej opieki medycznej",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": null
        },
        {
          "id": 3,
          "name": "Staż kierunkowy w zakresie kardiologii",
          "weeks": 16,
          "workingDays": 80,
          "isBasic": false,
          "location": null
        },
        {
          "id": 4,
          "name": "Staż kierunkowy w zakresie chorób płuc",
          "weeks": 6,
          "workingDays": 30,
          "isBasic": false,
          "location": null
        },
        {
          "id": 5,
          "name": "Staż kierunkowy w zakresie gastroenterologii",
          "weeks": 6,
          "workingDays": 30,
          "isBasic": false,
          "location": null
        },
        {
          "id": 6,
          "name": "Staż kierunkowy w zakresie endokrynologii",
          "weeks": 6,
          "workingDays": 30,
          "isBasic": false,
          "location": null
        },
        {
          "id": 7,
          "name": "Staż kierunkowy w zakresie nefrologii",
          "weeks": 3,
          "workingDays": 15,
          "isBasic": false,
          "location": null
        },
        {
          "id": 8,
          "name": "Staż kierunkowy w zakresie hematologii",
          "weeks": 3,
          "workingDays": 15,
          "isBasic": false,
          "location": null
        },
        {
          "id": 9,
          "name": "Staż kierunkowy w zakresie reumatologii",
          "weeks": 3,
          "workingDays": 15,
          "isBasic": false,
          "location": null
        },
        {
          "id": 10,
          "name": "Staż kierunkowy w zakresie chorób zakaźnych",
          "weeks": 3,
          "workingDays": 15,
          "isBasic": false,
          "location": null
        },
        {
          "id": 11,
          "name": "Staż kierunkowy w zakresie neurologii",
          "weeks": 3,
          "workingDays": 15,
          "isBasic": false,
          "location": null
        },
        {
          "id": 12,
          "name": "Staż kierunkowy w zakresie psychiatrii",
          "weeks": 2,
          "workingDays": 10,
          "isBasic": false,
          "location": null
        }
      ],
      "procedures": [
        {
          "id": 1,
          "name": "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
          "type": "Staż podstawowy",
          "requiredCountA": 3,
          "requiredCountB": 3,
          "internshipId": 1
        },
        {
          "id": 2,
          "name": "nakłucie jamy opłucnej w przypadku płynu",
          "type": "Staż podstawowy",
          "requiredCountA": 10,
          "requiredCountB": 3,
          "internshipId": 1
        },
        {
          "id": 3,
          "name": "nakłucie jamy otrzewnej w przypadku wodobrzusza",
          "type": "Staż podstawowy",
          "requiredCountA": 10,
          "requiredCountB": 3,
          "internshipId": 1
        },
        {
          "id": 4,
          "name": "nakłucie żył obwodowych – iniekcje dożylne, pobranie krwi obwodowej",
          "type": "Staż podstawowy",
          "requiredCountA": 30,
          "requiredCountB": 5,
          "internshipId": 1
        },
        {
          "id": 5,
          "name": "nakłucie tętnicy obwodowej w celu pobrania krwi do badania gazometrycznego",
          "type": "Staż podstawowy",
          "requiredCountA": 30,
          "requiredCountB": 5,
          "internshipId": 1
        },
        {
          "id": 6,
          "name": "pomiar ośrodkowego ciśnienia żylnego",
          "type": "Staż podstawowy",
          "requiredCountA": 6,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 7,
          "name": "cewnikowanie pęcherza moczowego",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 4,
          "internshipId": 1
        },
        {
          "id": 8,
          "name": "badanie per rectum",
          "type": "Staż podstawowy",
          "requiredCountA": 30,
          "requiredCountB": 5,
          "internshipId": 1
        },
        {
          "id": 9,
          "name": "przetoczenie krwi lub preparatu krwiopochodnego",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 10,
          "name": "wprowadzenie zgłębnika do żołądka",
          "type": "Staż podstawowy",
          "requiredCountA": 5,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 11,
          "name": "wykonanie i interpretacja 12-odprowadzeniowego EKG",
          "type": "Staż podstawowy",
          "requiredCountA": 30,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 12,
          "name": "badanie palpacyjne gruczołu piersiowego",
          "type": "Staż podstawowy",
          "requiredCountA": 10,
          "requiredCountB": 2,
          "internshipId": 1
        },
        {
          "id": 13,
          "name": "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 1,
          "internshipId": null
        },
        {
          "id": 14,
          "name": "kardiowersja elektryczna",
          "type": "Staże kierunkowe",
          "requiredCountA": 3,
          "requiredCountB": 3,
          "internshipId": null
        },
        {
          "id": 15,
          "name": "intubacja dotchawicza",
          "type": "Staże kierunkowe",
          "requiredCountA": 10,
          "requiredCountB": 3,
          "internshipId": null
        },
        {
          "id": 16,
          "name": "pomiar szczytowego przepływu wydechowego",
          "type": "Staże kierunkowe",
          "requiredCountA": 3,
          "requiredCountB": 3,
          "internshipId": null
        }
      ],
      "selfEducation": {
        "daysPerYear": 6,
        "totalDays": 12,
        "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
      },
      "holidays": {
        "vacationDays": 52,
        "nationalHolidays": 26,
        "examPreparationDays": null
      },
      "medicalShifts": {
        "hoursPerWeek": 10.08,
        "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym."
      },
      "procedureCodeDescription": {
        "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
        "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
      }
    },
    {
      "moduleId": 2,
      "name": "Moduł specjalistyczny w zakresie kardiologii",
      "code": "cardiology_specialistic",
      "moduleType": "Specialistic",
      "version": "CMKP 2014",
      "duration": {
        "years": 3,
        "months": 0,
        "days": 0
      },
      "workingDays": 783,
      "courses": [
        {
          "id": 1,
          "name": "Kurs wprowadzający: \"Wprowadzenie do specjalizacji w dziedzinie kardiologii\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 2,
          "name": "Kurs: \"Patofizjologia chorób sercowo-naczyniowych\"",
          "weeks": 1.0,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 3,
          "name": "Kurs: \"Farmakoterapia chorób sercowo-naczyniowych\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 4,
          "name": "Kurs: \"Nieinwazyjna diagnostyka elektrokardiograficzna\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 5,
          "name": "Kurs: \"Diagnostyka obrazowa – echokardiografia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 6,
          "name": "Kurs: \"Diagnostyka obrazowa – nowe techniki obrazowania\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 7,
          "name": "Kurs: \"Intensywna terapia kardiologiczna\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 8,
          "name": "Kurs: \"Elektrofizjologia i elektroterapia\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 9,
          "name": "Kurs: \"Diagnostyka inwazyjna i leczenie interwencyjne\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 10,
          "name": "Kurs: \"Wrodzone i nabyte wady serca\"",
          "weeks": 0.8,
          "workingDays": 4,
          "required": true
        },
        {
          "id": 11,
          "name": "Kurs: \"Onkologia w kardiologii\"",
          "weeks": 0.2,
          "workingDays": 1,
          "required": true
        },
        {
          "id": 12,
          "name": "Kurs: \"Przewlekły zespół wieńcowy\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 13,
          "name": "Kurs: \"Nadciśnienie tętnicze\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 14,
          "name": "Kurs: \"Nadciśnienie płucne i niewydolność prawej komory serca\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 15,
          "name": "Kurs: \"Niewydolność serca\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 16,
          "name": "Kurs: \"Diagnostyka i leczenie chorób naczyń obwodowych\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 17,
          "name": "Kurs: \"Choroby rzadkie w kardiologii\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 18,
          "name": "Kurs atestacyjny (podsumowujący): \"Kardiologia\"",
          "weeks": 1.0,
          "workingDays": 5,
          "required": true
        }
      ],
      "internships": [
        {
          "id": 1,
          "name": "Staż kierunkowy w zakresie intensywnej terapii kardiologicznej",
          "weeks": 22,
          "workingDays": 110,
          "isBasic": false,
          "location": "oddział intensywnej terapii kardiologicznej lub oddział kardiologii z łóżkami intensywnego nadzoru kardiologicznego"
        },
        {
          "id": 2,
          "name": "Staż kierunkowy w zakresie elektrofizjologii i elektroterapii",
          "weeks": 18,
          "workingDays": 90,
          "isBasic": false,
          "location": "zakład lub pracownia elektrofizjologii"
        },
        {
          "id": 3,
          "name": "Staż kierunkowy w zakresie kardiologii interwencyjnej",
          "weeks": 22,
          "workingDays": 110,
          "isBasic": false,
          "location": "zakład lub pracownia kardiologii interwencyjnej"
        },
        {
          "id": 4,
          "name": "Staż kierunkowy w zakresie echokardiografii",
          "weeks": 22,
          "workingDays": 110,
          "isBasic": false,
          "location": "zakład lub pracownia echokardiografii"
        },
        {
          "id": 5,
          "name": "Staż kierunkowy w zakresie kardiologii dziecięcej",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "oddział kardiologii dziecięcej"
        },
        {
          "id": 6,
          "name": "Staż kierunkowy w zakresie nabytych i wrodzonych wad serca u dorosłych",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "oddział kardiologii zajmujący się diagnostyką i leczeniem wad serca"
        },
        {
          "id": 7,
          "name": "Staż kierunkowy w zakresie kardiochirurgii",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "oddział kardiochirurgii"
        },
        {
          "id": 8,
          "name": "Staż kierunkowy w zakresie nieinwazyjnej diagnostyki elektrokardiograficznej",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "oddział kardiologii"
        },
        {
          "id": 9,
          "name": "Staż kierunkowy w zakresie radiologii i diagnostyki obrazowej",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia specjalizacji w dziedzinie radiologii i diagnostyki obrazowej"
        },
        {
          "id": 10,
          "name": "Staż kierunkowy w zakresie angiologii",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie angiologii lub chirurgii naczyniowej"
        },
        {
          "id": 11,
          "name": "Staż kierunkowy w zakresie kardiologii ambulatoryjnej",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "oddział kardiologii"
        }
      ],
      "procedures": [
        {
          "id": 1,
          "name": "badanie elektrokardiograficzne",
          "type": "Staż podstawowy",
          "requiredCountA": 200,
          "requiredCountB": 0,
          "competencyLevel": "Wykonanie, interpretacja"
        },
        {
          "id": 2,
          "name": "24-godzinne monitorowanie EKG metodą Holtera",
          "type": "Staż podstawowy",
          "requiredCountA": 50,
          "requiredCountB": 0,
          "competencyLevel": "Wykonanie, interpretacja"
        },
        {
          "id": 3,
          "name": "24-godzinne ambulatoryjne monitorowanie ciśnienia tętniczego",
          "type": "Staż podstawowy",
          "requiredCountA": 50,
          "requiredCountB": 0,
          "competencyLevel": "Wykonanie, interpretacja"
        },
        {
          "id": 4,
          "name": "próba wysiłkowa elektrokardiograficzna",
          "type": "Staż podstawowy",
          "requiredCountA": 50,
          "requiredCountB": 0,
          "competencyLevel": "Wykonanie, interpretacja"
        },
        {
          "id": 5,
          "name": "badanie echokardiograficzne przezklatkowe",
          "type": "Staż podstawowy",
          "requiredCountA": 200,
          "requiredCountB": 0,
          "competencyLevel": "Wykonanie, interpretacja"
        },
        {
          "id": 6,
          "name": "badanie echokardiograficzne przezprzełykowe",
          "type": "Staż podstawowy",
          "requiredCountA": 0,
          "requiredCountB": 50,
          "competencyLevel": "Asystowanie, interpretacja"
        },
        {
          "id": 7,
          "name": "badanie echokardiograficzne obciążeniowe",
          "type": "Staż podstawowy",
          "requiredCountA": 0,
          "requiredCountB": 50,
          "competencyLevel": "Asystowanie, interpretacja"
        },
        {
          "id": 8,
          "name": "nakłucie osierdzia, opłucnej, otrzewnej",
          "type": "Staż podstawowy",
          "requiredCountA": 25,
          "requiredCountB": 0,
          "competencyLevel": "Wykonanie"
        },
        {
          "id": 9,
          "name": "wszczepienie stymulatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "competencyLevel": "Asystowanie"
        },
        {
          "id": 10,
          "name": "wszczepienie kardiowertera-defibrylatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "competencyLevel": "Asystowanie"
        },
        {
          "id": 11,
          "name": "kontrola stymulatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "competencyLevel": "Wykonanie, interpretacja"
        },
        {
          "id": 12,
          "name": "kontrola kardiowertera-defibrylatora",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "competencyLevel": "Asystowanie"
        },
        {
          "id": 13,
          "name": "badanie elektrofizjologiczne",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "competencyLevel": "Asystowanie, interpretacja"
        },
        {
          "id": 14,
          "name": "ablacja podłoża zaburzeń rytmu",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 5,
          "competencyLevel": "Asystowanie"
        },
        {
          "id": 15,
          "name": "koronarografia",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 50,
          "competencyLevel": "Asystowanie, interpretacja"
        }
      ],
      "selfEducation": {
        "daysPerYear": 6,
        "totalDays": 18,
        "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
      },
      "holidays": {
        "vacationDays": 78,
        "nationalHolidays": 39,
        "examPreparationDays": 6
      },
      "medicalShifts": {
        "hoursPerWeek": 10.08,
        "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym."
      },
      "procedureCodeDescription": {
        "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
        "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
      },
      "examinationInfo": {
        "examType": "Państwowy Egzamin Specjalizacyjny",
        "components": [
          {
            "name": "egzamin testowy",
            "description": "zbiór pytań z zakresu wymaganej wiedzy określonej w programie specjalizacji, zawierających pięć wariantów odpowiedzi, z których tylko jeden jest prawidłowy"
          },
          {
            "name": "egzamin ustny",
            "description": "pytania problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji oraz ocenę trzech elektrokardiogramów, dwóch koronarografii, dwóch badań obrazowych (w tym co najmniej jedno badanie echokardiograficzne)"
          }
        ]
      }
    }
  ],
  "selfEducation": {
    "daysPerYear": 6,
    "totalDays": 30,
    "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
  },
  "holidays": {
    "vacationDays": 130,
    "nationalHolidays": 65,
    "examPreparationDays": 6
  },
  "medicalShifts": {
    "hoursPerWeek": 10.08,
    "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym."
  },
  "procedureCodeDescription": {
    "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
    "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
  },
  "examinationInfo": {
    "examType": "Państwowy Egzamin Specjalizacyjny",
    "components": [
      {
        "name": "egzamin testowy",
        "description": "zbiór pytań z zakresu wymaganej wiedzy określonej w programie specjalizacji, zawierających pięć wariantów odpowiedzi, z których tylko jeden jest prawidłowy"
      },
      {
        "name": "egzamin ustny",
        "description": "pytania problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji oraz ocenę trzech elektrokardiogramów, dwóch koronarografii, dwóch badań obrazowych (w tym co najmniej jedno badanie echokardiograficzne)"
      }
    ]
  }
}
```

## File: Resources/Raw/SpecializationTemplates/psychiatry_new.json
```json
{
  "name": "Psychiatria",
  "code": "psychiatry",
  "version": "CMKP 2023",
  "totalDuration": {
    "years": 5,
    "months": 0,
    "days": 0
  },
  "totalWorkingDays": 1305,
  "basicInfo": {
    "targetGroup": "Lekarze nieposiadający specjalizacji I lub II stopnia lub tytułu specjalisty w odpowiedniej dziedzinie medycyny",
    "qualificationProcedure": "wiosna 2023"
  },
  "modules": [
    {
      "moduleId": 1,
      "name": "Moduł specjalistyczny w zakresie psychiatrii",
      "code": "psychiatry_specialistic",
      "moduleType": "Specialistic",
      "version": "CMKP 2023",
      "duration": {
        "years": 5,
        "months": 0,
        "days": 0
      },
      "workingDays": 1104,
      "courses": [
        {
          "id": 1,
          "name": "Kurs wprowadzający: \"Wprowadzenie do specjalizacji w dziedzinie psychiatrii\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 2,
          "name": "Kurs: \"Psychiatria sądowa i opiniowanie sądowo-psychiatryczne\"",
          "weeks": 2,
          "workingDays": 10,
          "required": true
        },
        {
          "id": 3,
          "name": "Kurs: \"Psychiatria środowiskowa i rehabilitacja psychiatryczna\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 4,
          "name": "Kurs: \"Wybrane zagadnienia z zakresu psychiatrii klinicznej\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 5,
          "name": "Kurs: \"Wprowadzenie do psychoterapii\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 6,
          "name": "Kurs: \"Orzecznictwo lekarskie\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        },
        {
          "id": 7,
          "name": "Kurs: \"Profilaktyka i promocja zdrowia\"",
          "weeks": 0.4,
          "workingDays": 2,
          "required": true
        },
        {
          "id": 8,
          "name": "Kurs atestacyjny (podsumowujący): \"Psychiatria\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        }
      ],
      "internships": [
        {
          "id": 1,
          "name": "Staż podstawowy w zakresie psychiatrii",
          "weeks": 148,
          "workingDays": 740,
          "isBasic": true,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii – oddział ogólnopsychiatryczny, poradnia zdrowia psychicznego (PZP), zespół leczenia środowiskowego (ZLŚ), oddział dzienny lub centrum zdrowia psychicznego (CZP)"
        },
        {
          "id": 2,
          "name": "Staż kierunkowy w zakresie chorób wewnętrznych",
          "weeks": 6,
          "workingDays": 30,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie chorób wewnętrznych lub ww. stażu – oddział chorób wewnętrznych – zalecany I rok szkolenia"
        },
        {
          "id": 3,
          "name": "Staż kierunkowy w zakresie neurologii",
          "weeks": 4,
          "workingDays": 20,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie neurologii lub ww. stażu – oddział neurologiczny – zalecany I-II rok szkolenia"
        },
        {
          "id": 4,
          "name": "Staż kierunkowy w zakresie psychiatrii dzieci i młodzieży",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii dzieci i młodzieży lub ww. stażu – II lub III poziom referencyjności (poradnia dla dzieci i młodzieży - czynna we wszystkie dni robocze w tygodniu lub oddział dzienny dla dzieci i młodzieży lub oddział całodobowy dla dzieci i młodzieży)"
        },
        {
          "id": 5,
          "name": "Staż kierunkowy w zakresie zaburzeń nerwicowych i psychoterapii",
          "weeks": 12,
          "workingDays": 60,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii lub ww. stażu – oddział całodobowy lub oddział dzienny leczenia nerwic lub poradnia leczenia zaburzeń nerwicowych – zalecany II – V rok szkolenia"
        },
        {
          "id": 6,
          "name": "Staż kierunkowy w Centrum Zdrowia Psychicznego",
          "weeks": 12,
          "workingDays": 60,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii posiadająca Centrum Zdrowia Psychicznego lub ww. stażu"
        },
        {
          "id": 7,
          "name": "Staż kierunkowy w zakresie leczenia uzależnień",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii lub ww. stażu – oddział terapii uzależnień, oddział detoksykacyjny, poradnia leczenia uzależnień"
        },
        {
          "id": 8,
          "name": "Staż kierunkowy w zakresie psychiatrii konsultacyjnej",
          "weeks": 6,
          "workingDays": 30,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii lub ww. stażu – oddział psychiatryczny w szpitalu wielospecjalistycznym – zalecany III – V rok szkolenia"
        },
        {
          "id": 9,
          "name": "Staż kierunkowy w zakresie psychiatrii sądowej",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "jednostka posiadająca akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii posiadająca oddział o podstawowym lub wzmocnionym lub maksymalnym stopniu zabezpieczenia – po II roku szkolenia specjalizacyjnego"
        }
      ],
      "procedures": [
        {
          "id": 1,
          "name": "ocena stanu psychicznego za pomocą standaryzowanych skal klinicznych",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 2,
          "name": "kwalifikacja i przygotowanie pacjentów oraz przeprowadzenie zabiegów elektrowstrząsowych",
          "type": "Staż podstawowy",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 3,
          "name": "przygotowanie opinii w sprawie zasadności przyjęcia bez zgody do szpitala psychiatrycznego",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 4,
          "name": "przygotowanie opinii sądowo-psychiatrycznych w sprawach karnych",
          "type": "Staż podstawowy",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 5,
          "name": "przygotowanie opinii sądowo-psychiatrycznych w sprawach cywilnych",
          "type": "Staż podstawowy",
          "requiredCountA": 3,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 6,
          "name": "przeprowadzenie konsultacji psychiatrycznych dla innych lekarzy",
          "type": "Staż podstawowy",
          "requiredCountA": 20,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 7,
          "name": "prowadzenie pacjentów leczonych klozapiną",
          "type": "Staż podstawowy",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 8,
          "name": "prowadzenie pacjentów leczonych litem",
          "type": "Staż podstawowy",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 9,
          "name": "zastosowanie środków przymusu bezpośredniego zgodnie z UoOZP",
          "type": "Staż podstawowy",
          "requiredCountA": 10,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 10,
          "name": "prowadzenie pacjentów przyjętych bez zgody do szpitala psychiatrycznego",
          "type": "Staż podstawowy",
          "requiredCountA": 5,
          "requiredCountB": 0,
          "internshipId": 1
        },
        {
          "id": 11,
          "name": "przeprowadzenie ogólnego badania internistycznego",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 10,
          "internshipId": 2
        },
        {
          "id": 12,
          "name": "przeprowadzenie ogólnego badania neurologicznego",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 10,
          "internshipId": 3
        },
        {
          "id": 13,
          "name": "badanie psychiatryczne osób poniżej 18 roku życia",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 10,
          "internshipId": 4
        },
        {
          "id": 14,
          "name": "prowadzenie pacjentów z powikłanymi zespołami abstynencyjnymi",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 5,
          "internshipId": 7
        },
        {
          "id": 15,
          "name": "tworzenie planu terapii zdrowienia",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 10,
          "internshipId": 6
        },
        {
          "id": 16,
          "name": "udzielanie świadczeń za pomocą narzędzi teleinformatycznych",
          "type": "Staże kierunkowe",
          "requiredCountA": 10,
          "requiredCountB": 5,
          "internshipId": 6
        },
        {
          "id": 17,
          "name": "ocena wskazań do farmakoterapii lub psychoterapii oraz dobór technik psychoterapeutycznych",
          "type": "Staże kierunkowe",
          "requiredCountA": 5,
          "requiredCountB": 10,
          "internshipId": 5
        },
        {
          "id": 18,
          "name": "współudział w konsultacjach psychiatrycznych dla lekarzy innych specjalizacji",
          "type": "Staże kierunkowe",
          "requiredCountA": 0,
          "requiredCountB": 20,
          "internshipId": 8
        }
      ],
      "selfEducation": {
        "daysPerYear": 6,
        "totalDays": 30,
        "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
      },
      "holidays": {
        "vacationDays": 130,
        "nationalHolidays": 65,
        "examPreparationDays": 6
      },
      "medicalShifts": {
        "hoursPerWeek": 10.08,
        "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym. Wymagane jest pełnienie dyżurów w oddziałach psychiatrycznych."
      },
      "procedureCodeDescription": {
        "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
        "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
      },
      "examinationInfo": {
        "examType": "Państwowy Egzamin Specjalizacyjny",
        "components": [
          {
            "name": "egzamin testowy",
            "description": "zbiór pytań z zakresu wymaganej wiedzy określonej w programie specjalizacji"
          },
          {
            "name": "egzamin ustny",
            "description": "pytania problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji"
          }
        ]
      }
    }
  ],
  "selfEducation": {
    "daysPerYear": 6,
    "totalDays": 30,
    "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
  },
  "holidays": {
    "vacationDays": 130,
    "nationalHolidays": 65,
    "examPreparationDays": 6
  },
  "medicalShifts": {
    "hoursPerWeek": 10.08,
    "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym. Wymagane jest pełnienie dyżurów w oddziałach psychiatrycznych."
  },
  "procedureCodeDescription": {
    "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
    "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
  },
  "examinationInfo": {
    "examType": "Państwowy Egzamin Specjalizacyjny",
    "components": [
      {
        "name": "egzamin testowy",
        "description": "zbiór pytań z zakresu wymaganej wiedzy określonej w programie specjalizacji"
      },
      {
        "name": "egzamin ustny",
        "description": "pytania problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji"
      }
    ]
  }
}
```

## File: Resources/Raw/SpecializationTemplates/psychiatry_old.json
```json
{
  "name": "Psychiatria",
  "code": "psychiatry",
  "version": "CMKP 2014",
  "totalDuration": {
    "years": 5,
    "months": 0,
    "days": 0
  },
  "totalWorkingDays": 1306,
  "basicInfo": {
    "targetGroup": "Lekarze nieposiadający specjalizacji I lub II stopnia lub tytułu specjalisty w odpowiedniej dziedzinie medycyny",
    "qualificationProcedure": "CMKP 2014"
  },
  "modules": [
    {
      "moduleId": 1,
      "name": "Moduł specjalistyczny w zakresie psychiatrii",
      "code": "psychiatry_specialistic",
      "moduleType": "Specialistic",
      "version": "CMKP 2014",
      "duration": {
        "years": 5,
        "months": 0,
        "days": 0
      },
      "workingDays": 1111,
      "courses": [
        {
          "id": 1,
          "name": "Kurs wprowadzający: \"Wprowadzenie do specjalizacji w dziedzinie psychiatrii\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 2,
          "name": "Kurs: \"Psychiatria sądowa i orzecznictwo sądowo-psychiatryczne\"",
          "weeks": 2,
          "workingDays": 10,
          "required": true
        },
        {
          "id": 3,
          "name": "Kurs: \"Psychogeriatria\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 4,
          "name": "Kurs: \"Wprowadzenie do psychoterapii\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 5,
          "name": "Kurs: \"Ratownictwo medyczne\"",
          "weeks": 1,
          "workingDays": 5,
          "required": true
        },
        {
          "id": 6,
          "name": "Kurs: \"Zdrowie publiczne\"",
          "weeks": 1.6,
          "workingDays": 8,
          "required": true
        },
        {
          "id": 7,
          "name": "Kurs: \"Prawo medyczne\"",
          "weeks": 0.6,
          "workingDays": 3,
          "required": true
        }
      ],
      "internships": [
        {
          "id": 1,
          "name": "Staż podstawowy w zakresie psychiatrii - ROK I",
          "weeks": 15,
          "workingDays": 75,
          "isBasic": true,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii – oddział psychiatryczny lub kliniczny oddział psychiatryczny"
        },
        {
          "id": 2,
          "name": "Staż podstawowy w zakresie psychiatrii (cd.) - ROK II",
          "weeks": 27,
          "workingDays": 135,
          "isBasic": true,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii – oddział psychiatryczny ogólny (kliniczny lub szpitalny) dorosłych"
        },
        {
          "id": 3,
          "name": "Staż kierunkowy w zakresie neurologii",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie neurologii lub ww. stażu – oddział neurologiczny (kliniczny, szpitalny)"
        },
        {
          "id": 4,
          "name": "Staż kierunkowy w zakresie psychiatrii dzieci i młodzieży",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii dzieci i młodzieży lub ww. stażu – oddział lub poradnia czynna we wszystkie dni robocze w tygodniu lub oddział dzienny psychiatrii dzieci i młodzieży"
        },
        {
          "id": 5,
          "name": "Staż podstawowy w zakresie psychiatrii (cd.) - ROK III",
          "weeks": 26,
          "workingDays": 130,
          "isBasic": true,
          "location": null
        },
        {
          "id": 6,
          "name": "Staż kierunkowy w zakresie zaburzeń nerwicowych i psychoterapii",
          "weeks": 12,
          "workingDays": 60,
          "isBasic": false,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii lub ww. stażu – oddział (lub poradnia lub oddział dzienny) nerwic"
        },
        {
          "id": 7,
          "name": "Staż podstawowy w zakresie psychiatrii (cd.) - ROK IV",
          "weeks": 32,
          "workingDays": 160,
          "isBasic": true,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii – oddział psychiatryczny ogólny (kliniczny lub szpitalny) dorosłych lub centrum zdrowia psychicznego"
        },
        {
          "id": 8,
          "name": "Staż kierunkowy w zakresie leczenia uzależnień",
          "weeks": 8,
          "workingDays": 40,
          "isBasic": false,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii lub ww. stażu – ośrodek (oddział) leczenia uzależnień"
        },
        {
          "id": 9,
          "name": "Staż podstawowy w zakresie psychiatrii (cd.) - ROK V",
          "weeks": 34,
          "workingDays": 170,
          "isBasic": true,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii – oddział psychiatryczny ogólny (kliniczny lub szpitalny) dorosłych lub centrum zdrowia psychicznego"
        },
        {
          "id": 10,
          "name": "Staż podstawowy w zakresie psychiatrii (cd.) - ROK V cd.",
          "weeks": 42,
          "workingDays": 210,
          "isBasic": true,
          "location": "jednostka, która uzyskała akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie psychiatrii – oddział psychiatryczny ogólny (kliniczny lub szpitalny) dorosłych lub centrum zdrowia psychicznego"
        }
      ],
      "procedures": [
        {
          "id": 1,
          "name": "oceny za pomocą nie mniej niż 3 różnych standaryzowanych skal klinicznych (np. PANSS, HDRS, MADRS, CGI, MMSE itp.)",
          "type": "Procedury medyczne",
          "requiredCountA": 20,
          "requiredCountB": 0
        },
        {
          "id": 2,
          "name": "współudział w przygotowaniu i przeprowadzeniu co najmniej 3 zabiegów elektrowstrząsowych",
          "type": "Procedury medyczne",
          "requiredCountA": 3,
          "requiredCountB": 0
        },
        {
          "id": 3,
          "name": "współudział w przygotowaniu co najmniej 20 opinii w sprawie zasadności przyjęcia bez zgody do szpitala psychiatrycznego",
          "type": "Procedury medyczne",
          "requiredCountA": 20,
          "requiredCountB": 0
        },
        {
          "id": 4,
          "name": "współudział w przygotowaniu 15 opinii sądowo-psychiatrycznych w sprawach karnych",
          "type": "Procedury medyczne",
          "requiredCountA": 15,
          "requiredCountB": 0
        },
        {
          "id": 5,
          "name": "współudział w przygotowaniu 3 opinii sądowo-psychiatrycznych w sprawach cywilnych",
          "type": "Procedury medyczne",
          "requiredCountA": 3,
          "requiredCountB": 0
        },
        {
          "id": 6,
          "name": "współudział w co najmniej 20 konsultacjach psychiatrycznych dla innych lekarzy",
          "type": "Procedury medyczne",
          "requiredCountA": 20,
          "requiredCountB": 0
        }
      ],
      "selfEducation": {
        "daysPerYear": 6,
        "totalDays": 30,
        "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
      },
      "holidays": {
        "vacationDays": 130,
        "nationalHolidays": 65,
        "examPreparationDays": 0
      },
      "medicalShifts": {
        "hoursPerWeek": 10.08,
        "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym. Wymagane jest pełnienie dyżurów w oddziałach psychiatrycznych."
      },
      "procedureCodeDescription": {
        "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
        "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
      },
      "examinationInfo": {
        "examType": "Państwowy Egzamin Specjalizacyjny",
        "components": [
          {
            "name": "egzamin testowy",
            "description": "zbiór pytań testowych wielokrotnego wyboru z zakresu wymaganej wiedzy określonej w programie specjalizacji"
          },
          {
            "name": "egzamin ustny",
            "description": "pytania ustne problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji"
          }
        ]
      }
    }
  ],
  "selfEducation": {
    "daysPerYear": 6,
    "totalDays": 30,
    "description": "Dodatkowe dni na samokształcenie przeznaczone na udział w konferencjach, kursach naukowych i doskonalących i innych szkoleniach w danej dziedzinie specjalizacji do wyboru lekarza"
  },
  "holidays": {
    "vacationDays": 130,
    "nationalHolidays": 65,
    "examPreparationDays": null
  },
  "medicalShifts": {
    "hoursPerWeek": 10.08,
    "description": "Lekarz pełni dyżury medyczne w wymiarze przeciętnie 10 godzin 5 minut na tydzień lub wykonuje pracę w systemie zmianowym lub równoważnym czasie pracy, w maksymalnym czasie pracy dopuszczonym w przepisach o działalności leczniczej, tj. w wymiarze przeciętnie 48 godzin na tydzień, w tym dyżur medyczny, w przyjętym okresie rozliczeniowym. Wymagane jest pełnienie dyżurów w oddziałach psychiatrycznych."
  },
  "procedureCodeDescription": {
    "codeA": "Wykonywanie samodzielnie z asystą lub pod nadzorem kierownika specjalizacji albo lekarza specjalisty przez niego wyznaczonego (liczba)",
    "codeB": "W których lekarz uczestniczy jako pierwsza asysta (liczba)"
  },
  "examinationInfo": {
    "examType": "Państwowy Egzamin Specjalizacyjny",
    "components": [
      {
        "name": "egzamin testowy",
        "description": "zbiór pytań testowych wielokrotnego wyboru z zakresu wymaganej wiedzy określonej w programie specjalizacji"
      },
      {
        "name": "egzamin ustny",
        "description": "pytania ustne problemowe, dotyczące wymaganej wiedzy określonej w programie specjalizacji"
      }
    ]
  }
}
```

## File: Resources/Styles/Colors.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    
    <Color x:Key="Primary">#24C1DE</Color>
    <Color x:Key="PrimaryDark">#0D759C</Color>
    <Color x:Key="PrimaryLight">#30DDE8</Color>
    <Color x:Key="PrimaryDarkText">#082044</Color>

    
    <Color x:Key="Secondary">#547E9E</Color>
    <Color x:Key="SecondaryDark">#082044</Color>
    <Color x:Key="SecondaryLight">#95B8CE</Color>
    <Color x:Key="SecondaryDarkText">#082044</Color>
    <Color x:Key="Tertiary">#4AF5F7</Color>

    
    <Color x:Key="White">White</Color>
    <Color x:Key="Black">#000000</Color>
    <Color x:Key="OffBlack">#082044</Color>
    <Color x:Key="Magenta">#24C1DE</Color>
    <Color x:Key="Background">#E4F0F5</Color>
    <Color x:Key="BackgroundDark">#082044</Color>
    <Color x:Key="MidnightBlue">#082044</Color>

    
    <Color x:Key="AccentBlue">#149CD0</Color>
    <Color x:Key="AccentOrange">#FF9800</Color>

    
    <Color x:Key="Gray100">#E4F0F5</Color>
    <Color x:Key="Gray200">#D9E2EC</Color>
    <Color x:Key="Gray300">#BCCCDC</Color>
    <Color x:Key="Gray400">#9FB3C8</Color>
    <Color x:Key="Gray500">#95B8CE</Color>
    <Color x:Key="Gray600">#547E9E</Color>
    <Color x:Key="Gray700">#0D759C</Color>
    <Color x:Key="Gray800">#149CD0</Color>
    <Color x:Key="Gray900">#082044</Color>
    <Color x:Key="Gray950">#061833</Color>

    
    <SolidColorBrush x:Key="PrimaryBrush"
                     Color="{StaticResource Primary}"/>
    <SolidColorBrush x:Key="PrimaryDarkBrush"
                     Color="{StaticResource PrimaryDark}"/>
    <SolidColorBrush x:Key="PrimaryLightBrush"
                     Color="{StaticResource PrimaryLight}"/>
    <SolidColorBrush x:Key="TertiaryBrush"
                     Color="{StaticResource Tertiary}"/>
    <SolidColorBrush x:Key="SecondaryBrush"
                     Color="{StaticResource Secondary}"/>
    <SolidColorBrush x:Key="BackgroundBrush"
                     Color="{StaticResource Background}"/>
    <SolidColorBrush x:Key="WhiteBrush"
                     Color="{StaticResource White}"/>
    <SolidColorBrush x:Key="BlackBrush"
                     Color="{StaticResource Black}"/>

    
    <SolidColorBrush x:Key="Gray100Brush"
                     Color="{StaticResource Gray100}"/>
    <SolidColorBrush x:Key="Gray200Brush"
                     Color="{StaticResource Gray200}"/>
    <SolidColorBrush x:Key="Gray300Brush"
                     Color="{StaticResource Gray300}"/>
    <SolidColorBrush x:Key="Gray400Brush"
                     Color="{StaticResource Gray400}"/>
    <SolidColorBrush x:Key="Gray500Brush"
                     Color="{StaticResource Gray500}"/>
    <SolidColorBrush x:Key="Gray600Brush"
                     Color="{StaticResource Gray600}"/>
    <SolidColorBrush x:Key="Gray700Brush"
                     Color="{StaticResource Gray700}"/>
    <SolidColorBrush x:Key="Gray800Brush"
                     Color="{StaticResource Gray800}"/>
    <SolidColorBrush x:Key="Gray900Brush"
                     Color="{StaticResource Gray900}"/>
    <SolidColorBrush x:Key="Gray950Brush"
                     Color="{StaticResource Gray950}"/>
</ResourceDictionary>
```

## File: Resources/Styles/iOSStyles.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary 
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:ios="clr-namespace:Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;assembly=Microsoft.Maui.Controls">

    

    
    <OnPlatform x:Key="iOSFontFamily" x:TypeArguments="x:String">
        <On Platform="iOS" Value="-apple-system,BlinkMacSystemFont" />
    </OnPlatform>

    
    <x:Double x:Key="iOSBodyFontSize">17</x:Double>
    <x:Double x:Key="iOSCalloutFontSize">16</x:Double>
    <x:Double x:Key="iOSSubheadlineFontSize">15</x:Double>
    <x:Double x:Key="iOSFootnoteFontSize">13</x:Double>
    <x:Double x:Key="iOSCaption1FontSize">12</x:Double>
    <x:Double x:Key="iOSCaption2FontSize">11</x:Double>
    <x:Double x:Key="iOSTitle1FontSize">28</x:Double>
    <x:Double x:Key="iOSTitle2FontSize">22</x:Double>
    <x:Double x:Key="iOSTitle3FontSize">20</x:Double>
    <x:Double x:Key="iOSHeadlineFontSize">17</x:Double>
    <x:Double x:Key="iOSLargeTitleFontSize">34</x:Double>

    
    <x:Double x:Key="iOSMinimumTouchSize">44</x:Double>

    
    <Style x:Key="iOSListCell" TargetType="Grid">
        <Setter Property="Padding" Value="16,11" />
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=White, Dark=#1C1C1E}" />
    </Style>

    <Style x:Key="iOSListSeparator" TargetType="BoxView">
        <Setter Property="HeightRequest" Value="0.5" />
        <Setter Property="Color" Value="{AppThemeBinding Light=#95B8CE, Dark=#547E9E}" />
        <Setter Property="Margin" Value="16,0,0,0" />
    </Style>

    
    <Style x:Key="iOSButtonStyle" TargetType="Button">
        <Setter Property="HeightRequest" Value="44" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="16,0" />
        <Setter Property="FontFamily" Value="{StaticResource iOSFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource iOSBodyFontSize}" />
        <Setter Property="FontAttributes" Value="Bold" />
    </Style>

    <Style x:Key="iOSPrimaryButtonStyle" TargetType="Button" BasedOn="{StaticResource iOSButtonStyle}">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=#24C1DE, Dark=#30DDE8}" />
        <Setter Property="TextColor" Value="White" />
    </Style>

    <Style x:Key="iOSSecondaryButtonStyle" TargetType="Button" BasedOn="{StaticResource iOSButtonStyle}">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#24C1DE, Dark=#30DDE8}" />
        <Setter Property="BorderColor" Value="{AppThemeBinding Light=#24C1DE, Dark=#30DDE8}" />
        <Setter Property="BorderWidth" Value="1" />
    </Style>

    <Style x:Key="iOSDestructiveButtonStyle" TargetType="Button" BasedOn="{StaticResource iOSButtonStyle}">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#FF3B30, Dark=#FF453A}" />
    </Style>

    
    <Style x:Key="iOSSwitchStyle" TargetType="Switch">
        <Setter Property="OnColor" Value="{AppThemeBinding Light=#24C1DE, Dark=#30DDE8}" />
    </Style>

    
    <Style x:Key="iOSFormEntryStyle" TargetType="Entry">
        <Setter Property="FontFamily" Value="{StaticResource iOSFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource iOSBodyFontSize}" />
        <Setter Property="HeightRequest" Value="44" />
        <Setter Property="BackgroundColor" Value="Transparent" />
    </Style>

    <Style x:Key="iOSFormPickerStyle" TargetType="Picker">
        <Setter Property="FontFamily" Value="{StaticResource iOSFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource iOSBodyFontSize}" />
        <Setter Property="HeightRequest" Value="44" />
        <Setter Property="BackgroundColor" Value="Transparent" />
    </Style>

    <Style x:Key="iOSFormLabelStyle" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource iOSFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource iOSBodyFontSize}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#547E9E, Dark=#95B8CE}" />
        <Setter Property="Opacity" Value="0.6" />
    </Style>

    
    <Style x:Key="iOSPageStyle" TargetType="ContentPage">
        <Setter Property="ios:Page.UseSafeArea" Value="True" />
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=#E4F0F5, Dark=#082044}" />
    </Style>

    
    <Style x:Key="iOSNavigationStyle" TargetType="NavigationPage">
        <Setter Property="BarBackgroundColor" Value="{AppThemeBinding Light=White, Dark=#082044}" />
        <Setter Property="BarTextColor" Value="{AppThemeBinding Light=#082044, Dark=White}" />
        <Setter Property="ios:NavigationPage.PrefersLargeTitles" Value="True" />
    </Style>

    
    <Style x:Key="iOSCardStyle" TargetType="Border">
        <Setter Property="StrokeShape" Value="RoundRectangle 8" />
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=White, Dark=#082044}" />
        <Setter Property="Stroke" Value="{AppThemeBinding Light=#95B8CE, Dark=#547E9E}" />
        <Setter Property="StrokeThickness" Value="1" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="Margin" Value="0,8" />
    </Style>
</ResourceDictionary>
```

## File: Resources/Styles/MaterialDesignStyles.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <x:Double x:Key="MaterialHeadline1">96</x:Double>
    <x:Double x:Key="MaterialHeadline2">60</x:Double>
    <x:Double x:Key="MaterialHeadline3">48</x:Double>
    <x:Double x:Key="MaterialHeadline4">34</x:Double>
    <x:Double x:Key="MaterialHeadline5">24</x:Double>
    <x:Double x:Key="MaterialHeadline6">20</x:Double>
    <x:Double x:Key="MaterialSubtitle1">16</x:Double>
    <x:Double x:Key="MaterialSubtitle2">14</x:Double>
    <x:Double x:Key="MaterialBody1">16</x:Double>
    <x:Double x:Key="MaterialBody2">14</x:Double>
    <x:Double x:Key="MaterialCaption">12</x:Double>
    <x:Double x:Key="MaterialButton">14</x:Double>
    <x:Double x:Key="MaterialOverline">10</x:Double>
    <x:String x:Key="MaterialFontFamily">OpenSansRegular</x:String>
    <x:String x:Key="MaterialFontFamilyMedium">OpenSansSemibold</x:String>
    <Thickness x:Key="MaterialMarginExtraSmall">4</Thickness>
    <Thickness x:Key="MaterialMarginSmall">8</Thickness>
    <Thickness x:Key="MaterialMarginMedium">16</Thickness>
    <Thickness x:Key="MaterialMarginLarge">24</Thickness>
    <Thickness x:Key="MaterialMarginExtraLarge">32</Thickness>
    <Thickness x:Key="MaterialPaddingExtraSmall">4</Thickness>
    <Thickness x:Key="MaterialPaddingSmall">8</Thickness>
    <Thickness x:Key="MaterialPaddingMedium">16</Thickness>
    <Thickness x:Key="MaterialPaddingLarge">24</Thickness>
    <Thickness x:Key="MaterialPaddingExtraLarge">32</Thickness>
    <x:Double x:Key="MaterialElevation0">0</x:Double>
    <x:Double x:Key="MaterialElevation1">1</x:Double>
    <x:Double x:Key="MaterialElevation2">2</x:Double>
    <x:Double x:Key="MaterialElevation3">3</x:Double>
    <x:Double x:Key="MaterialElevation4">4</x:Double>
    <x:Double x:Key="MaterialElevation6">6</x:Double>
    <x:Double x:Key="MaterialElevation8">8</x:Double>
    <x:Double x:Key="MaterialElevation12">12</x:Double>
    <x:Double x:Key="MaterialElevation16">16</x:Double>
    <x:Double x:Key="MaterialElevation24">24</x:Double>
    <Style x:Key="MaterialCard" TargetType="Frame">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=White, Dark=#121212}" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="{StaticResource MaterialPaddingMedium}" />
        <Setter Property="Margin" Value="{StaticResource MaterialMarginSmall}" />
        <Setter Property="HasShadow" Value="True" />
        <Setter Property="BorderColor" Value="{AppThemeBinding Light=#E0E0E0, Dark=#333333}" />
    </Style>
    <Style x:Key="MaterialButtonBase" TargetType="Button">
        <Setter Property="CornerRadius" Value="4" />
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamilyMedium}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialButton}" />
        <Setter Property="Padding" Value="16,10" />
        <Setter Property="MinimumHeightRequest" Value="36" />
        <Setter Property="TextTransform" Value="Uppercase" />
    </Style>
    <Style x:Key="MaterialButtonPrimary" TargetType="Button" BasedOn="{StaticResource MaterialButtonBase}">
        <Setter Property="BackgroundColor" Value="{StaticResource Primary}" />
        <Setter Property="TextColor" Value="White" />
    </Style>
    <Style x:Key="MaterialButtonSecondary" TargetType="Button" BasedOn="{StaticResource MaterialButtonBase}">
        <Setter Property="BackgroundColor" Value="{StaticResource Secondary}" />
        <Setter Property="TextColor" Value="White" />
    </Style>
    <Style x:Key="MaterialButtonOutlined" TargetType="Button" BasedOn="{StaticResource MaterialButtonBase}">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{StaticResource Primary}" />
        <Setter Property="BorderColor" Value="{StaticResource Primary}" />
        <Setter Property="BorderWidth" Value="1" />
    </Style>
    <Style x:Key="MaterialButtonText" TargetType="Button" BasedOn="{StaticResource MaterialButtonBase}">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{StaticResource Primary}" />
        <Setter Property="BorderWidth" Value="0" />
    </Style>
    <Style x:Key="MaterialButtonDanger" TargetType="Button" BasedOn="{StaticResource MaterialButtonBase}">
        <Setter Property="BackgroundColor" Value="#F44336" />
        <Setter Property="TextColor" Value="White" />
    </Style>
    <Style x:Key="MaterialEntryBase" TargetType="Entry">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialBody1}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="PlaceholderColor" Value="{AppThemeBinding Light=#9E9E9E, Dark=#757575}" />
        <Setter Property="MinimumHeightRequest" Value="48" />
    </Style>
    <Style x:Key="MaterialLabelHeadline5" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialHeadline5}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="Margin" Value="0,8" />
    </Style>
    <Style x:Key="MaterialLabelHeadline6" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialHeadline6}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="Margin" Value="0,4" />
    </Style>
    <Style x:Key="MaterialLabelSubtitle1" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialSubtitle1}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
    </Style>
    <Style x:Key="MaterialLabelSubtitle2" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialSubtitle2}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
    </Style>
    <Style x:Key="MaterialLabelBody1" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialBody1}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
    </Style>
    <Style x:Key="MaterialLabelBody2" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialBody2}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
    </Style>
    <Style x:Key="MaterialLabelCaption" TargetType="Label">
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialCaption}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#757575, Dark=#BDBDBD}" />
    </Style>
    <Style x:Key="MaterialListItem" TargetType="Frame">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=White, Dark=#121212}" />
        <Setter Property="Padding" Value="{StaticResource MaterialPaddingMedium}" />
        <Setter Property="Margin" Value="0,4" />
        <Setter Property="HasShadow" Value="False" />
        <Setter Property="BorderColor" Value="Transparent" />
    </Style>
    <Style x:Key="MaterialDivider" TargetType="BoxView">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=#E0E0E0, Dark=#333333}" />
        <Setter Property="HeightRequest" Value="1" />
        <Setter Property="Margin" Value="0,8" />
    </Style>
    <Style x:Key="MaterialSearchBar" TargetType="SearchBar">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light=#F5F5F5, Dark=#333333}" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="PlaceholderColor" Value="{AppThemeBinding Light=#9E9E9E, Dark=#757575}" />
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialBody1}" />
    </Style>
    <Style x:Key="MaterialPicker" TargetType="Picker">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="TitleColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialBody1}" />
        <Setter Property="MinimumHeightRequest" Value="48" />
    </Style>
    <Style x:Key="MaterialDatePicker" TargetType="DatePicker">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialBody1}" />
        <Setter Property="MinimumHeightRequest" Value="48" />
    </Style>
    <Style x:Key="MaterialTimePicker" TargetType="TimePicker">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{AppThemeBinding Light=#212121, Dark=#FAFAFA}" />
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
        <Setter Property="FontSize" Value="{StaticResource MaterialBody1}" />
        <Setter Property="MinimumHeightRequest" Value="48" />
    </Style>
    <Style x:Key="MaterialIconButton" TargetType="ImageButton">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="WidthRequest" Value="48" />
        <Setter Property="HeightRequest" Value="48" />
        <Setter Property="Padding" Value="12" />
    </Style>
    <Style x:Key="MaterialFAB" TargetType="Button">
        <Setter Property="BackgroundColor" Value="{StaticResource Primary}" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="CornerRadius" Value="28" />
        <Setter Property="WidthRequest" Value="56" />
        <Setter Property="HeightRequest" Value="56" />
        <Setter Property="Padding" Value="0" />
        <Setter Property="Margin" Value="0,0,16,16" />
        <Setter Property="HorizontalOptions" Value="End" />
        <Setter Property="VerticalOptions" Value="End" />
    </Style>
    <Style x:Key="MaterialErrorText" TargetType="Label">
        <Setter Property="TextColor" Value="#F44336" />
        <Setter Property="FontSize" Value="{StaticResource MaterialCaption}" />
        <Setter Property="FontFamily" Value="{StaticResource MaterialFontFamily}" />
    </Style>
</ResourceDictionary>
```

## File: Resources/Styles/Styles.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Style TargetType="ActivityIndicator">
        <Setter Property="Color"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Primary}}"/>
    </Style>
    <Style TargetType="IndicatorView">
        <Setter Property="IndicatorColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}"/>
        <Setter Property="SelectedIndicatorColor"
                Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray100}}"/>
    </Style>
    <Style TargetType="Border">
        <Setter Property="Stroke"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}"/>
        <Setter Property="StrokeShape"
                Value="Rectangle"/>
        <Setter Property="StrokeThickness"
                Value="1"/>
    </Style>
    <Style TargetType="BoxView">
        <Setter Property="BackgroundColor"
                Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray200}}"/>
    </Style>
    <Style TargetType="Button">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource PrimaryDarkText}}"/>
        <Setter Property="BackgroundColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="BorderWidth"
                Value="0"/>
        <Setter Property="CornerRadius"
                Value="8"/>
        <Setter Property="Padding"
                Value="14,10"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray200}}"/>
                            <Setter Property="BackgroundColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="PointerOver"/>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="CheckBox">
        <Setter Property="Color"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="Color"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="DatePicker">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Editor">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="PlaceholderColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Entry">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="PlaceholderColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Frame">
        <Setter Property="HasShadow"
                Value="False"/>
        <Setter Property="BorderColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray950}}"/>
        <Setter Property="CornerRadius"
                Value="8"/>
        <Setter Property="BackgroundColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}"/>
    </Style>
    <Style TargetType="ImageButton">
        <Setter Property="Opacity"
                Value="1"/>
        <Setter Property="BorderColor"
                Value="Transparent"/>
        <Setter Property="BorderWidth"
                Value="0"/>
        <Setter Property="CornerRadius"
                Value="0"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="Opacity"
                                    Value="0.5"/>
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="PointerOver"/>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Label">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Span">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}"/>
    </Style>
    <Style TargetType="Label"
           x:Key="Headline">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource BackgroundDark}, Dark={StaticResource White}}"/>
        <Setter Property="FontSize"
                Value="32"/>
        <Setter Property="HorizontalOptions"
                Value="Center"/>
        <Setter Property="HorizontalTextAlignment"
                Value="Center"/>
    </Style>
    <Style TargetType="Label"
           x:Key="SubHeadline">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource BackgroundDark}, Dark={StaticResource White}}"/>
        <Setter Property="FontSize"
                Value="24"/>
        <Setter Property="HorizontalOptions"
                Value="Center"/>
        <Setter Property="HorizontalTextAlignment"
                Value="Center"/>
    </Style>
    <Style TargetType="ListView">
        <Setter Property="SeparatorColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}"/>
        <Setter Property="RefreshControlColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource Gray200}}"/>
    </Style>
    <Style TargetType="Picker">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}"/>
        <Setter Property="TitleColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource Gray200}}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="TitleColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="ProgressBar">
        <Setter Property="ProgressColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="ProgressColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="RadioButton">
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="RefreshView">
        <Setter Property="RefreshColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource Gray200}}"/>
    </Style>
    <Style TargetType="SearchBar">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}"/>
        <Setter Property="PlaceholderColor"
                Value="{StaticResource Gray500}"/>
        <Setter Property="CancelButtonColor"
                Value="{StaticResource Gray500}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="PlaceholderColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="SearchHandler">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}"/>
        <Setter Property="PlaceholderColor"
                Value="{StaticResource Gray500}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="PlaceholderColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Shadow">
        <Setter Property="Radius"
                Value="15"/>
        <Setter Property="Opacity"
                Value="0.5"/>
        <Setter Property="Brush"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource White}}"/>
        <Setter Property="Offset"
                Value="10,10"/>
    </Style>
    <Style TargetType="Slider">
        <Setter Property="MinimumTrackColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="MaximumTrackColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray600}}"/>
        <Setter Property="ThumbColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="MinimumTrackColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="MaximumTrackColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="ThumbColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="SwipeItem">
        <Setter Property="BackgroundColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}"/>
    </Style>
    <Style TargetType="Switch">
        <Setter Property="OnColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="ThumbColor"
                Value="{StaticResource White}"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="OnColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="ThumbColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="On">
                        <VisualState.Setters>
                            <Setter Property="OnColor"
                                    Value="{AppThemeBinding Light={StaticResource Secondary}, Dark={StaticResource Gray200}}"/>
                            <Setter Property="ThumbColor"
                                    Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Off">
                        <VisualState.Setters>
                            <Setter Property="ThumbColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray400}, Dark={StaticResource Gray500}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="TimePicker">
        <Setter Property="TextColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}"/>
        <Setter Property="BackgroundColor"
                Value="Transparent"/>
        <Setter Property="FontFamily"
                Value="OpenSansRegular"/>
        <Setter Property="FontSize"
                Value="14"/>
        <Setter Property="MinimumHeightRequest"
                Value="44"/>
        <Setter Property="MinimumWidthRequest"
                Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal"/>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor"
                                    Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    <Style TargetType="Page"
           ApplyToDerivedTypes="True">
        <Setter Property="Padding"
                Value="0"/>
        <Setter Property="BackgroundColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource OffBlack}}"/>
    </Style>
    <Style TargetType="Shell"
           ApplyToDerivedTypes="True">
        <Setter Property="Shell.BackgroundColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource OffBlack}}"/>
        <Setter Property="Shell.ForegroundColor"
                Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource SecondaryDarkText}}"/>
        <Setter Property="Shell.TitleColor"
                Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource SecondaryDarkText}}"/>
        <Setter Property="Shell.DisabledColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray950}}"/>
        <Setter Property="Shell.UnselectedColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray200}}"/>
        <Setter Property="Shell.NavBarHasShadow"
                Value="False"/>
        <Setter Property="Shell.TabBarBackgroundColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}"/>
        <Setter Property="Shell.TabBarForegroundColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="Shell.TabBarTitleColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="Shell.TabBarUnselectedColor"
                Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource Gray200}}"/>
    </Style>
    <Style TargetType="NavigationPage">
        <Setter Property="BarBackgroundColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource OffBlack}}"/>
        <Setter Property="BarTextColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource White}}"/>
        <Setter Property="IconColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource White}}"/>
    </Style>
    <Style TargetType="TabbedPage">
        <Setter Property="BarBackgroundColor"
                Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Gray950}}"/>
        <Setter Property="BarTextColor"
                Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}"/>
        <Setter Property="UnselectedTabColor"
                Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray950}}"/>
        <Setter Property="SelectedTabColor"
                Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray200}}"/>
    </Style>
    <Style x:Key="SplashScreenStyle"
           TargetType="ContentPage">
        <Setter Property="BackgroundColor"
                Value="{StaticResource PrimaryDark}"/>
        <Setter Property="Shell.NavBarIsVisible"
                Value="False"/>
    </Style>
</ResourceDictionary>
```

## File: Services/Authentication/AuthService.cs
```csharp
using System.Security.Cryptography;
using System.Text;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Services.Authentication
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IDatabaseService databaseService;
        private User currentUser;

        public AuthService(
            IDatabaseService databaseService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService logger) : base(exceptionHandler, logger)
        {
            this.databaseService = databaseService;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                if (this.currentUser != null)
                {
                    return this.currentUser;
                }

                int userId = await SettingsHelper.GetCurrentUserIdAsync();
                if (userId <= 0)
                {
                    return null;
                }

                this.currentUser = await this.databaseService.GetUserAsync(userId);
                return this.currentUser;
            }, "Nie udało się pobrać danych użytkownika.");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.GetCurrentUserAsync();
                return user != null;
            }, "Nie udało się sprawdzić stanu uwierzytelnienia.");
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.databaseService.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return false;
                }

                if (!this.VerifyPassword(password, user.PasswordHash))
                {
                    return false;
                }

                await SettingsHelper.SetCurrentUserIdAsync(user.UserId);
                this.currentUser = user;

                return true;
            }, "Nie udało się zalogować użytkownika.");
        }

        public async Task LogoutAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await SettingsHelper.SetCurrentUserIdAsync(0);
                this.currentUser = null;
                await SettingsHelper.SetCurrentModuleIdAsync(0);
            }, "Nie udało się wylogować użytkownika.");
        }

        public async Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (user == null)
                {
                    throw new InvalidInputException("User cannot be null", "Użytkownik nie może być pusty.");
                }

                var existingUser = await this.databaseService.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    return false;
                }

                try
                {
                    specialization.SpecializationId = 0;
                    int specializationId = await this.databaseService.SaveSpecializationAsync(specialization);
                    user.SpecializationId = specializationId;
                    user.PasswordHash = HashPassword(password);
                    user.RegistrationDate = DateTime.Now;
                    await this.databaseService.SaveUserAsync(user);

                    foreach (var module in specialization.Modules)
                    {
                        module.ModuleId = 0;
                        module.SpecializationId = specializationId;
                        await this.databaseService.SaveModuleAsync(module);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Błąd podczas rejestracji użytkownika",
                        new Dictionary<string, object> { { "Username", user.Username } });
                    throw;
                }
            }, "Nie udało się zarejestrować użytkownika.");
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
```

## File: Services/Authentication/IAuthService.cs
```csharp
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);

        Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization);

        Task LogoutAsync();

        Task<User> GetCurrentUserAsync();

        Task<bool> IsAuthenticatedAsync();
    }
}
```

## File: Services/Database/DatabaseService.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;
using SQLite;
using System.Diagnostics;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection database;
        private bool isInitialized = false;
        private readonly Dictionary<int, Models.Specialization> _specializationCache = new();
        private readonly Dictionary<int, List<Module>> _moduleCache = new();
        private readonly IExceptionHandlerService _exceptionHandler;
        private readonly ILoggingService _loggingService;

        public DatabaseService(IExceptionHandlerService exceptionHandler, ILoggingService loggingService)
        {
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var databasePath = Constants.DatabasePath;
                var databaseDirectory = Path.GetDirectoryName(databasePath);

                if (!Directory.Exists(databaseDirectory))
                {
                    Directory.CreateDirectory(databaseDirectory);
                }

                this.database = new SQLiteAsyncConnection(databasePath, Constants.Flags);

                await this.database.CreateTableAsync<User>();
                await this.database.CreateTableAsync<Models.Specialization>();
                await this.database.CreateTableAsync<Module>();
                await this.database.CreateTableAsync<Internship>();
                await this.database.CreateTableAsync<MedicalShift>();
                await this.database.CreateTableAsync<Procedure>();
                await this.database.CreateTableAsync<Course>();
                await this.database.CreateTableAsync<SelfEducation>();
                await this.database.CreateTableAsync<Publication>();
                await this.database.CreateTableAsync<EducationalActivity>();
                await this.database.CreateTableAsync<Absence>();
                await this.database.CreateTableAsync<Models.Recognition>();
                await this.database.CreateTableAsync<SpecializationProgram>();
                await this.database.CreateTableAsync<RealizedMedicalShiftOldSMK>();
                await this.database.CreateTableAsync<RealizedMedicalShiftNewSMK>();
                await this.database.CreateTableAsync<RealizedProcedureNewSMK>();
                await this.database.CreateTableAsync<RealizedProcedureOldSMK>();
                await this.database.CreateTableAsync<RealizedInternshipNewSMK>();
                await this.database.CreateTableAsync<RealizedInternshipOldSMK>();

                this.isInitialized = true;
            }, null, "Nie udało się zainicjalizować bazy danych.", 3, 1000);
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.QueryAsync<T>(query, args);
            },
            new Dictionary<string, object> { { "Query", query } },
            "Nie udało się wykonać zapytania do bazy danych");
        }

        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.ExecuteAsync(query, args);
            },
            new Dictionary<string, object> { { "Query", query } },
            "Nie udało się wykonać polecenia w bazie danych");
        }

        public async Task<int> UpdateAsync<T>(T item)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (item == null)
                {
                    throw new InvalidInputException(
                        "Item cannot be null",
                        "Element nie może być pusty");
                }

                return await this.database.UpdateAsync(item);
            },
            new Dictionary<string, object> { { "ItemType", typeof(T).Name } },
            "Nie udało się zaktualizować danych w bazie");
        }

        public void ClearCache()
        {
            _specializationCache.Clear();
            _moduleCache.Clear();
            _loggingService.LogInformation("Cache wyczyszczony");
        }

        private class TableInfo
        {
            public string name { get; set; }
        }
    }
}
```

## File: Services/Database/DatabaseService.Internship.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Internship> GetInternshipAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<Internship>().FirstOrDefaultAsync(i => i.InternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Internship with ID {id} not found",
                        $"Nie znaleziono stażu o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "InternshipId", id } },
            $"Nie udało się pobrać stażu o ID {id}");
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Internship>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(i => i.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> {
                { "SpecializationId", specializationId },
                { "ModuleId", moduleId }
            },
            "Nie udało się pobrać listy staży");
        }

        public async Task<int> SaveInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Staż nie może być pusty");
                }

                if (internship.InternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "Internship", internship?.InternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "ModuleId", internship?.ModuleId }
            },
            "Nie udało się zapisać danych stażu");
        }

        public async Task<int> DeleteInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Staż nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "Internship", internship?.InternshipId } },
            "Nie udało się usunąć stażu");
        }
    }
}
```

## File: Services/Database/DatabaseService.MedicalShift.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<MedicalShift> GetMedicalShiftAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var shift = await this.database.Table<MedicalShift>().FirstOrDefaultAsync(s => s.ShiftId == id);
                if (shift == null)
                {
                    throw new ResourceNotFoundException(
                        $"MedicalShift with ID {id} not found",
                        $"Nie znaleziono dyżuru o ID {id}");
                }
                return shift;
            },
            new Dictionary<string, object> { { "ShiftId", id } },
            $"Nie udało się pobrać dyżuru o ID {id}");
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<MedicalShift>();

                if (internshipId.HasValue)
                {
                    query = query.Where(s => s.InternshipId == internshipId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "InternshipId", internshipId } },
            "Nie udało się pobrać listy dyżurów");
        }

        public async Task<int> SaveMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "MedicalShift cannot be null",
                        "Dyżur nie może być pusty");
                }

                if (shift.ShiftId != 0)
                {
                    return await this.database.UpdateAsync(shift);
                }
                else
                {
                    return await this.database.InsertAsync(shift);
                }
            },
            new Dictionary<string, object> { { "Shift", shift?.ShiftId }, { "InternshipId", shift?.InternshipId } },
            "Nie udało się zapisać danych dyżuru");
        }

        public async Task<int> DeleteMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "MedicalShift cannot be null",
                        "Dyżur nie może być pusty");
                }

                return await this.database.DeleteAsync(shift);
            },
            new Dictionary<string, object> { { "Shift", shift?.ShiftId } },
            "Nie udało się usunąć dyżuru");
        }

        public async Task MigrateShiftDataForModulesAsync()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                bool columnExists = false;

                try
                {
                    var testQuery = "SELECT ModuleId FROM RealizedMedicalShiftNewSMK LIMIT 1";
                    await this.database.ExecuteScalarAsync<int>(testQuery);
                    columnExists = true;
                }
                catch
                {
                    await this.database.ExecuteAsync("ALTER TABLE RealizedMedicalShiftNewSMK ADD COLUMN ModuleId INTEGER");
                }

                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE ModuleId IS NULL OR ModuleId = 0";
                var shiftsToUpdate = await this.QueryAsync<RealizedMedicalShiftNewSMK>(query);

                if (shiftsToUpdate.Count == 0)
                {
                    return;
                }

                var specializations = await this.GetAllSpecializationsAsync();

                foreach (var specializationId in specializations.Select(x => x.SpecializationId))
                {
                    var modules = await this.GetModulesAsync(specializationId);
                    if (modules.Count == 0) continue;
                    var specializationShifts = shiftsToUpdate.Where(s => s.SpecializationId == specializationId).ToList();
                    if (specializationShifts.Count == 0) continue;

                    foreach (var shift in specializationShifts)
                    {
                        Module appropriateModule = null;
                        foreach (var module in modules)
                        {
                            if (string.IsNullOrEmpty(module.Structure)) continue;

                            var options = new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                AllowTrailingCommas = true,
                                ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                            };

                            var moduleStructure = System.Text.Json.JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                            if (moduleStructure?.Internships != null &&
                                moduleStructure.Internships.Any(i => i.Id == shift.InternshipRequirementId))
                            {
                                appropriateModule = module;
                                break;
                            }
                        }

                        if (appropriateModule == null && modules.Count > 0)
                        {
                            appropriateModule = modules[0];
                        }

                        if (appropriateModule != null)
                        {
                            shift.ModuleId = appropriateModule.ModuleId;
                            await this.UpdateAsync(shift);
                        }
                    }
                }
            },
            null, "Nie udało się zmigrować danych dyżurów do modułów", 2, 2000);
        }
    }
}
```

## File: Services/Database/DatabaseService.Module.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var module = await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
                if (module == null)
                {
                    throw new ResourceNotFoundException(
                        $"Module with ID {id} not found",
                        $"Nie znaleziono modułu o ID {id}");
                }
                return module;
            },
            new Dictionary<string, object> { { "ModuleId", id } },
            $"Nie udało się pobrać modułu o ID {id}");
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (_moduleCache.TryGetValue(specializationId, out var cachedModules))
                {
                    return cachedModules;
                }

                var modules = await database.Table<Module>()
                    .Where(m => m.SpecializationId == specializationId)
                    .ToListAsync();

                if (modules != null)
                {
                    _moduleCache[specializationId] = modules;
                }

                return modules ?? new List<Module>();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się pobrać listy modułów dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (module == null)
                {
                    throw new InvalidInputException(
                        "Module cannot be null",
                        "Moduł nie może być pusty");
                }

                if (module.ModuleId != 0)
                {
                    return await this.database.UpdateAsync(module);
                }
                else
                {
                    return await this.database.InsertAsync(module);
                }
            },
            new Dictionary<string, object> { { "Module", module?.ModuleId }, { "SpecializationId", module?.SpecializationId } },
            "Nie udało się zapisać danych modułu");
        }

        public async Task<int> UpdateModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (module == null)
                {
                    throw new InvalidInputException(
                        "Module cannot be null",
                        "Moduł nie może być pusty");
                }

                // Clear cache after update
                if (_moduleCache.TryGetValue(module.SpecializationId, out _))
                {
                    _moduleCache.Remove(module.SpecializationId);
                }

                return await this.database.UpdateAsync(module);
            },
            new Dictionary<string, object> { { "Module", module?.ModuleId }, { "SpecializationId", module?.SpecializationId } },
            "Nie udało się zaktualizować danych modułu");
        }
    }
}
```

## File: Services/Database/DatabaseService.Procedure.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Procedure>();

                if (internshipId.HasValue)
                {
                    query = query.Where(p => p.InternshipId == internshipId);
                }

                var procedures = await query.ToListAsync();

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLowerInvariant();
                    procedures = procedures.Where(p =>
                        p.Code.ToLowerInvariant().Contains(searchText) ||
                        p.Location.ToLowerInvariant().Contains(searchText) ||
                        (p.PatientInitials != null && p.PatientInitials.ToLowerInvariant().Contains(searchText)) ||
                        (p.ProcedureGroup != null && p.ProcedureGroup.ToLowerInvariant().Contains(searchText))
                    ).ToList();
                }

                return procedures;
            },
            new Dictionary<string, object> { { "InternshipId", internshipId }, { "SearchText", searchText } },
            "Nie udało się pobrać listy procedur");
        }

        public async Task<int> SaveProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new InvalidInputException(
                        "Procedure cannot be null",
                        "Procedura nie może być pusta");
                }

                if (procedure.ProcedureId != 0)
                {
                    return await this.database.UpdateAsync(procedure);
                }
                else
                {
                    return await this.database.InsertAsync(procedure);
                }
            },
            new Dictionary<string, object> { { "Procedure", procedure?.ProcedureId }, { "InternshipId", procedure?.InternshipId } },
            "Nie udało się zapisać danych procedury");
        }

        public async Task<int> DeleteProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new InvalidInputException(
                        "Procedure cannot be null",
                        "Procedura nie może być pusta");
                }

                return await this.database.DeleteAsync(procedure);
            },
            new Dictionary<string, object> { { "Procedure", procedure?.ProcedureId } },
            "Nie udało się usunąć procedury");
        }
    }
}
```

## File: Services/Database/DatabaseService.RealizedInternship.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipNewSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipNewSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w nowym SMK o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "RealizedInternshipId", id } },
            $"Nie udało się pobrać zrealizowanego stażu w nowym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? specializationId = null, int? moduleId = null, int? internshipRequirementId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<RealizedInternshipNewSMK>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(i => i.ModuleId == moduleId);
                }

                if (internshipRequirementId.HasValue)
                {
                    query = query.Where(i => i.InternshipRequirementId == internshipRequirementId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> {
                { "SpecializationId", specializationId },
                { "ModuleId", moduleId },
                { "InternshipRequirementId", internshipRequirementId }
            },
            "Nie udało się pobrać listy zrealizowanych staży w nowym SMK");
        }

        public async Task<int> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipNewSMK cannot be null",
                        "Zrealizowany staż w nowym SMK nie może być pusty");
                }

                if (internship.RealizedInternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "RealizedInternship", internship?.RealizedInternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "ModuleId", internship?.ModuleId },
                { "InternshipRequirementId", internship?.InternshipRequirementId }
            },
            "Nie udało się zapisać danych zrealizowanego stażu w nowym SMK");
        }

        public async Task<int> DeleteRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipNewSMK cannot be null",
                        "Zrealizowany staż w nowym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "RealizedInternship", internship?.RealizedInternshipId } },
            "Nie udało się usunąć zrealizowanego stażu w nowym SMK");
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipOldSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipOldSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w starym SMK o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "RealizedInternshipId", id } },
            $"Nie udało się pobrać zrealizowanego stażu w starym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? specializationId = null, int? year = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<RealizedInternshipOldSMK>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (year.HasValue)
                {
                    query = query.Where(i => i.Year == year);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "Year", year } },
            "Nie udało się pobrać listy zrealizowanych staży w starym SMK");
        }

        public async Task<int> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipOldSMK cannot be null",
                        "Zrealizowany staż w starym SMK nie może być pusty");
                }

                if (internship.RealizedInternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "RealizedInternship", internship?.RealizedInternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "Year", internship?.Year }
            },
            "Nie udało się zapisać danych zrealizowanego stażu w starym SMK");
        }

        public async Task<int> DeleteRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipOldSMK cannot be null",
                        "Zrealizowany staż w starym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "RealizedInternship", internship?.RealizedInternshipId } },
            "Nie udało się usunąć zrealizowanego stażu w starym SMK");
        }

        public async Task MigrateInternshipDataAsync()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                // Sprawdzenie istnienia kolumny w tabeli
                bool internshipRequirementIdExists = false;
                bool moduleIdExists = false;

                try
                {
                    var testQuery = "SELECT InternshipRequirementId FROM RealizedInternshipNewSMK LIMIT 1";
                    await this.database.ExecuteScalarAsync<int>(testQuery);
                    internshipRequirementIdExists = true;
                }
                catch
                {
                    await this.database.ExecuteAsync("ALTER TABLE RealizedInternshipNewSMK ADD COLUMN InternshipRequirementId INTEGER");
                }

                try
                {
                    var testQuery = "SELECT ModuleId FROM RealizedInternshipNewSMK LIMIT 1";
                    await this.database.ExecuteScalarAsync<int>(testQuery);
                    moduleIdExists = true;
                }
                catch
                {
                    await this.database.ExecuteAsync("ALTER TABLE RealizedInternshipNewSMK ADD COLUMN ModuleId INTEGER");
                }

                // Sprawdź i napraw istniejące realizacje z null InternshipName
                try
                {
                    var realizationsWithNullNames = await this.database.Table<RealizedInternshipOldSMK>()
                        .Where(r => r.InternshipName == null)
                        .ToListAsync();

                    foreach (var realization in realizationsWithNullNames)
                    {
                        _loggingService.LogInformation($"Znaleziono realizację z pustą nazwą stażu, ID: {realization.RealizedInternshipId}");
                        // Próba naprawy - szukamy w oryginalnych stażach
                        var originalInternship = await this.database.Table<Internship>()
                            .FirstOrDefaultAsync(i => i.SpecializationId == realization.SpecializationId &&
                                                      i.DaysCount == realization.DaysCount);

                        if (originalInternship != null && !string.IsNullOrEmpty(originalInternship.InternshipName))
                        {
                            realization.InternshipName = originalInternship.InternshipName;
                            await this.database.UpdateAsync(realization);
                            _loggingService.LogInformation($"Naprawiono nazwę stażu: {realization.InternshipName}");
                        }
                        else
                        {
                            // Jeśli nie udało się znaleźć odpowiedniego stażu, użyj wartości domyślnej
                            realization.InternshipName = "Staż bez nazwy";
                            await this.database.UpdateAsync(realization);
                            _loggingService.LogInformation($"Ustawiono domyślną nazwę stażu dla ID: {realization.RealizedInternshipId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.LogError(ex, $"Błąd podczas naprawy realizacji: {ex.Message}", new Dictionary<string, object> { { "ExceptionDetails", ex } });
                }

                // Pobierz aktualną wersję SMK użytkownika
                var userId = await Helpers.SettingsHelper.GetCurrentUserIdAsync();
                var user = await this.GetUserAsync(userId);

                if (user == null)
                {
                    return;
                }

                // Pobranie wszystkich istniejących staży
                var internships = await this.database.Table<Internship>().Where(i => i.InternshipId > 0).ToListAsync();

                // Sprawdź, czy już istnieją realizacje dla tych staży
                var existingNewSMK = await this.database.Table<RealizedInternshipNewSMK>().ToListAsync();
                var existingOldSMK = await this.database.Table<RealizedInternshipOldSMK>().ToListAsync();

                // Jeśli realizacje już istnieją, pomijamy migrację
                if ((user.SmkVersion == SmkVersion.New && existingNewSMK.Count > 0) ||
                    (user.SmkVersion == SmkVersion.Old && existingOldSMK.Count > 0))
                {
                    return;
                }

                // Migracja danych
                foreach (var internship in internships)
                {
                    if (user.SmkVersion == SmkVersion.New)
                    {
                        // Ignorujemy staże z ID < 0 (to są wymagania stażowe, nie realizacje)
                        if (internship.InternshipId < 0)
                        {
                            continue;
                        }

                        var existingInternship = existingNewSMK
                            .FirstOrDefault(i => i.SpecializationId == internship.SpecializationId &&
                                               i.InternshipName == internship.InternshipName);

                        if (existingInternship != null)
                        {
                            continue;
                        }

                        var realizedInternship = new RealizedInternshipNewSMK
                        {
                            SpecializationId = internship.SpecializationId,
                            ModuleId = internship.ModuleId,
                            InternshipRequirementId = internship.InternshipId, // Ustawienie ID wymagania
                            InternshipName = internship.InternshipName ?? "Staż bez nazwy", // Upewniamy się, że nazwa nie jest null
                            InstitutionName = internship.InstitutionName,
                            DepartmentName = internship.DepartmentName,
                            StartDate = internship.StartDate,
                            EndDate = internship.EndDate,
                            DaysCount = internship.DaysCount,
                            IsCompleted = internship.IsCompleted,
                            IsApproved = internship.IsApproved,
                            IsRecognition = internship.IsRecognition,
                            RecognitionReason = internship.RecognitionReason,
                            RecognitionDaysReduction = internship.RecognitionDaysReduction,
                            IsPartialRealization = internship.IsPartialRealization,
                            SupervisorName = internship.SupervisorName,
                            SyncStatus = internship.SyncStatus,
                            AdditionalFields = internship.AdditionalFields
                        };

                        await this.database.InsertAsync(realizedInternship);
                    }
                    else // Stary SMK
                    {
                        // Ignorujemy staże z ID < 0 (to są wymagania stażowe, nie realizacje)
                        if (internship.InternshipId < 0)
                        {
                            continue;
                        }

                        var existingInternship = existingOldSMK
                            .FirstOrDefault(i => i.SpecializationId == internship.SpecializationId &&
                                               i.InternshipName == internship.InternshipName &&
                                               i.Year == internship.Year);

                        if (existingInternship != null)
                        {
                            continue;
                        }

                        var realizedInternship = new RealizedInternshipOldSMK
                        {
                            SpecializationId = internship.SpecializationId,
                            InternshipName = internship.InternshipName ?? "Staż bez nazwy", // Upewniamy się, że nazwa nie jest null
                            InstitutionName = internship.InstitutionName,
                            DepartmentName = internship.DepartmentName,
                            StartDate = internship.StartDate,
                            EndDate = internship.EndDate,
                            DaysCount = internship.DaysCount,
                            IsCompleted = internship.IsCompleted,
                            IsApproved = internship.IsApproved,
                            Year = internship.Year,
                            RequiresApproval = false, // Domyślna wartość
                            SupervisorName = internship.SupervisorName,
                            SyncStatus = internship.SyncStatus,
                            AdditionalFields = internship.AdditionalFields
                        };

                        await this.database.InsertAsync(realizedInternship);
                    }
                }
            },
            null, "Nie udało się zmigrować danych stażowych", 2, 2000);
        }
    }
}
```

## File: Services/Database/DatabaseService.Specialization.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Models.Specialization> GetSpecializationAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (_specializationCache.TryGetValue(id, out var cachedSpecialization))
                {
                    return cachedSpecialization;
                }

                var specialization = await database.Table<Models.Specialization>()
                    .FirstOrDefaultAsync(s => s.SpecializationId == id);

                if (specialization != null)
                {
                    _specializationCache[id] = specialization;
                }

                return specialization ?? new Models.Specialization();
            },
            new Dictionary<string, object> { { "SpecializationId", id } },
            $"Nie udało się pobrać specjalizacji o ID {id}");
        }

        public async Task<Models.Specialization> GetSpecializationWithModulesAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var specialization = await GetSpecializationAsync(id);
                if (specialization != null)
                {
                    specialization.Modules = await GetModulesAsync(specialization.SpecializationId);
                }
                return specialization;
            },
            new Dictionary<string, object> { { "SpecializationId", id } },
            $"Nie udało się pobrać specjalizacji z modułami o ID {id}");
        }

        public async Task<int> SaveSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (specialization == null)
                {
                    throw new InvalidInputException(
                        "Specialization cannot be null",
                        "Specjalizacja nie może być pusta");
                }

                if (specialization.SpecializationId != 0)
                {
                    await this.database.UpdateAsync(specialization);

                    // Clear cache after update
                    if (_specializationCache.ContainsKey(specialization.SpecializationId))
                    {
                        _specializationCache.Remove(specialization.SpecializationId);
                    }

                    return specialization.SpecializationId;
                }
                else
                {
                    await this.database.InsertAsync(specialization);
                    var lastId = await this.database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    specialization.SpecializationId = lastId;
                    return lastId;
                }
            },
            new Dictionary<string, object> { { "Specialization", specialization?.SpecializationId } },
            "Nie udało się zapisać danych specjalizacji");
        }

        public async Task<List<Models.Specialization>> GetAllSpecializationsAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<Models.Specialization>().ToListAsync();
            }, null, "Nie udało się pobrać listy specjalizacji");
        }

        public async Task<int> UpdateSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (specialization == null)
                {
                    throw new InvalidInputException(
                        "Specialization cannot be null",
                        "Specjalizacja nie może być pusta");
                }

                var result = await this.database.UpdateAsync(specialization);

                // Clear cache after update
                if (_specializationCache.ContainsKey(specialization.SpecializationId))
                {
                    _specializationCache.Remove(specialization.SpecializationId);
                }

                return result;
            },
            new Dictionary<string, object> { { "Specialization", specialization?.SpecializationId } },
            "Nie udało się zaktualizować danych specjalizacji");
        }
    }
}
```

## File: Services/Database/DatabaseService.User.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<User> GetUserAsync(int id)
        {
            await this.InitializeAsync();

            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var user = await this.database.Table<User>().FirstOrDefaultAsync(u => u.UserId == id);
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        $"User with ID {id} not found",
                        $"Nie znaleziono użytkownika o ID {id}",
                        null,
                        new Dictionary<string, object> { { "UserId", id } });
                }
                return user;
            },
            new Dictionary<string, object> { { "UserId", id } },
            $"Nie udało się pobrać użytkownika o ID {id}",
            3, 800);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new InvalidInputException(
                        "Username cannot be null or empty",
                        "Nazwa użytkownika nie może być pusta");
                }

                return await this.database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            },
            new Dictionary<string, object> { { "Username", username } },
            $"Nie udało się pobrać użytkownika o nazwie {username}");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<User>().ToListAsync();
            }, null, "Nie udało się pobrać listy użytkowników");
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (user == null)
                {
                    throw new InvalidInputException(
                        "User cannot be null",
                        "Użytkownik nie może być pusty");
                }

                if (user.UserId != 0)
                {
                    await this.database.UpdateAsync(user);
                    return user.UserId;
                }
                else
                {
                    await this.database.InsertAsync(user);
                    var lastId = await this.database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    user.UserId = lastId;
                    return lastId;
                }
            },
            new Dictionary<string, object> { { "User", user?.UserId } },
            "Nie udało się zapisać danych użytkownika");
        }
    }
}
```

## File: Services/Database/DatabaseServiceExtensions.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SledzSpecke.App.Services.Database;
using SQLite;

namespace SledzSpecke.App.Services.Database
{
    public static class DatabaseServiceExtensions
    {
        public static async Task<List<T>> QueryAsync<T>(this IDatabaseService databaseService, string query, params object[] args)
            where T : new()
        {
            await databaseService.InitializeAsync();
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.QueryAsync<T>(query, args);
        }

        public static async Task<int> ExecuteAsync(this IDatabaseService databaseService, string query, params object[] args)
        {
            await databaseService.InitializeAsync();
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.ExecuteAsync(query, args);
        }

        public static async Task<int> InsertAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            await databaseService.InitializeAsync();
            await CreateTableIfNotExistsAsync<T>(databaseService);

            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.InsertAsync(obj);
        }

        public static async Task<int> UpdateAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            await databaseService.InitializeAsync();
            await CreateTableIfNotExistsAsync<T>(databaseService);

            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.UpdateAsync(obj);
        }

        public static async Task<int> DeleteAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            await databaseService.InitializeAsync();
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.DeleteAsync(obj);
        }

        private static async Task CreateTableIfNotExistsAsync<T>(IDatabaseService databaseService)
            where T : new()
        {
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            await database.CreateTableAsync<T>();
        }
    }
}
```

## File: Services/Database/IDatabaseService.cs
```csharp
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public interface IDatabaseService
    {
        Task InitializeAsync();

        Task<User> GetUserAsync(int id);

        Task<User> GetUserByUsernameAsync(string username);

        Task<int> SaveUserAsync(User user);

        Task<List<User>> GetAllUsersAsync();

        Task<Models.Specialization> GetSpecializationAsync(int id);

        Task<int> SaveSpecializationAsync(Models.Specialization specialization);

        Task<List<Models.Specialization>> GetAllSpecializationsAsync();

        Task<Module> GetModuleAsync(int id);

        Task<List<Module>> GetModulesAsync(int specializationId);

        Task<int> SaveModuleAsync(Module module);

        Task<int> UpdateModuleAsync(Module module);

        Task<Internship> GetInternshipAsync(int id);

        Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null);

        Task<int> SaveInternshipAsync(Internship internship);

        Task<int> DeleteInternshipAsync(Internship internship);

        Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id);

        Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? specializationId = null, int? moduleId = null, int? internshipRequirementId = null);

        Task<int> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<int> DeleteRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id);

        Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? specializationId = null, int? year = null);

        Task<int> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task<int> DeleteRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task MigrateInternshipDataAsync();

        Task<MedicalShift> GetMedicalShiftAsync(int id);

        Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null);

        Task<int> SaveMedicalShiftAsync(MedicalShift shift);

        Task<int> DeleteMedicalShiftAsync(MedicalShift shift);

        Task MigrateShiftDataForModulesAsync();

        Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null);

        Task<int> UpdateSpecializationAsync(Models.Specialization specialization);

        void ClearCache();
    }
}
```

## File: Services/Dialog/DialogService.cs
```csharp
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SledzSpecke.App.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            var tcs = new TaskCompletionSource<bool>();

            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    var result = await page.DisplayAlert(title, message, accept, cancel);
                    tcs.SetResult(result);
                }
                else
                {
                    tcs.SetResult(false);
                }
            });

            return await tcs.Task;
        }

        public async Task DisplayAlertAsync(string title, string message, string accept)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    await page.DisplayAlert(title, message, accept);
                }
            });
        }

        public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            var tcs = new TaskCompletionSource<string>();

            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    var result = await page.DisplayActionSheet(title, cancel, destruction, buttons);
                    tcs.SetResult(result);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return await tcs.Task;
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            var tcs = new TaskCompletionSource<string>();

            await MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Shell.Current ?? Application.Current?.MainPage;
                if (page != null)
                {
                    var result = await page.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
                    tcs.SetResult(result);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return await tcs.Task;
        }
    }
}
```

## File: Services/Dialog/IDialogService.cs
```csharp
namespace SledzSpecke.App.Services.Dialog
{
    public interface IDialogService
    {
        Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);

        Task DisplayAlertAsync(string title, string message, string accept);

        Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);

        Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "");
    }
}
```

## File: Services/Exceptions/ExceptionHandlerService.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Logging;
using System.Runtime.CompilerServices;

namespace SledzSpecke.App.Services.Exceptions
{
    public class ExceptionHandlerService : IExceptionHandlerService
    {
        private readonly IDialogService _dialogService;
        private readonly ILoggingService _loggingService;
        private readonly SemaphoreSlim _dialogSemaphore = new SemaphoreSlim(1, 1);
        private readonly List<string> _recentErrors = new List<string>();
        private DateTime _lastErrorTime = DateTime.MinValue;

        public ExceptionHandlerService(
            IDialogService dialogService,
            ILoggingService loggingService)
        {
            _dialogService = dialogService;
            _loggingService = loggingService;
        }

        public async Task HandleExceptionAsync(System.Exception exception,
                                              [CallerMemberName] string callerMemberName = "")
        {
            string title = "Błąd";
            string message;
            Dictionary<string, object> logProperties = new Dictionary<string, object>();

            // Add caller info
            if (!string.IsNullOrEmpty(callerMemberName))
            {
                logProperties["CallerMethod"] = callerMemberName;
            }

            // Handling specific exception types
            if (exception is AppBaseException appException)
            {
                message = appException.UserFriendlyMessage;

                // Add exception details to log
                foreach (var detail in appException.ErrorDetails)
                {
                    logProperties[detail.Key] = detail.Value;
                }

                // Log with the original technical message
                _loggingService.LogError(exception, appException.Message, logProperties, callerMemberName);
            }
            else
            {
                // Default handling for non-app exceptions
                message = "Wystąpił nieoczekiwany błąd w aplikacji.";
                _loggingService.LogError(exception, exception.Message, logProperties, callerMemberName);
            }

            // Sprawdź czy pokazywać dialogi czy nie
            await ShowErrorToUserAsync(title, message);
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation,
                                           Dictionary<string, object> contextInfo = null,
                                           string userFriendlyMessage = null,
                                           [CallerMemberName] string operationName = "")
        {
            contextInfo ??= new Dictionary<string, object>();
            contextInfo["OperationName"] = operationName;

            try
            {
                // Log operation start if needed
                _loggingService.LogInformation($"Starting operation: {operationName}", contextInfo, operationName);

                // Execute operation
                var result = await operation();

                // Log operation end if needed
                _loggingService.LogInformation($"Completed operation: {operationName}", contextInfo, operationName);

                return result;
            }
            catch (System.Exception ex)
            {
                // Transform standard exceptions to app exceptions
                var appException = TransformException(ex, userFriendlyMessage, contextInfo);

                // Handle the exception
                await HandleExceptionAsync(appException, operationName);

                // Default value for the return type
                return default;
            }
        }

        public async Task ExecuteAsync(Func<Task> operation,
                                     Dictionary<string, object> contextInfo = null,
                                     string userFriendlyMessage = null,
                                     [CallerMemberName] string operationName = "")
        {
            contextInfo ??= new Dictionary<string, object>();
            contextInfo["OperationName"] = operationName;

            try
            {
                // Log operation start if needed
                _loggingService.LogInformation($"Starting operation: {operationName}", contextInfo, operationName);

                // Execute operation
                await operation();

                // Log operation end if needed
                _loggingService.LogInformation($"Completed operation: {operationName}", contextInfo, operationName);
            }
            catch (System.Exception ex)
            {
                // Transform standard exceptions to app exceptions
                var appException = TransformException(ex, userFriendlyMessage, contextInfo);

                // Handle the exception
                await HandleExceptionAsync(appException, operationName);
            }
        }

        // Nowa metoda do wykonywania operacji z ponawianiem
        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation,
                                                Dictionary<string, object> contextInfo = null,
                                                string userFriendlyMessage = null,
                                                int retryCount = 3,
                                                int delayMilliseconds = 500,
                                                [CallerMemberName] string operationName = "")
        {
            contextInfo ??= new Dictionary<string, object>();
            contextInfo["OperationName"] = operationName;
            contextInfo["RetryCount"] = retryCount;

            Exception lastException = null;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    if (i > 0)
                    {
                        _loggingService.LogInformation($"Retry attempt {i + 1} for operation: {operationName}", contextInfo, operationName);
                    }

                    return await operation();
                }
                catch (System.Exception ex)
                {
                    lastException = ex;

                    // Sprawdź czy wyjątek kwalifikuje się do ponowienia
                    if (!IsTransientException(ex) || i >= retryCount - 1)
                    {
                        // Ostatnia próba lub nie-tymczasowy wyjątek - nie próbuj ponownie
                        break;
                    }

                    // Loguj informację o ponowieniu
                    _loggingService.LogWarning($"Transient error occurred, retrying (attempt {i + 1}/{retryCount}): {ex.Message}",
                        contextInfo, operationName);

                    // Odczekaj przed ponowieniem (ze zwiększającym się opóźnieniem)
                    await Task.Delay(delayMilliseconds * (i + 1));
                }
            }

            // Jeśli dotarliśmy tutaj, to wszystkie próby się nie powiodły
            var appException = TransformException(lastException, userFriendlyMessage, contextInfo);
            await HandleExceptionAsync(appException, operationName);
            return default;
        }

        // Bez zwracanego wyniku
        public async Task ExecuteWithRetryAsync(Func<Task> operation,
                                          Dictionary<string, object> contextInfo = null,
                                          string userFriendlyMessage = null,
                                          int retryCount = 3,
                                          int delayMilliseconds = 500,
                                          [CallerMemberName] string operationName = "")
        {
            await ExecuteWithRetryAsync<object>(async () => {
                await operation();
                return null;
            }, contextInfo, userFriendlyMessage, retryCount, delayMilliseconds, operationName);
        }

        private bool IsTransientException(System.Exception ex)
        {
            // Check if it's a SQLite exception without referencing SQLite.Result enum
            return ex is SQLite.SQLiteException sqlEx &&
                   (sqlEx.Result == (SQLite.SQLite3.Result)5 ||   // Result.Busy
                    sqlEx.Result == (SQLite.SQLite3.Result)6 ||   // Result.Locked
                    sqlEx.Result == (SQLite.SQLite3.Result)19 ||  // Result.Constraint
                    sqlEx.Result == (SQLite.SQLite3.Result)10)    // Result.IOError
                || ex is System.Net.Http.HttpRequestException
                || ex is System.Net.WebException
                || ex is System.IO.IOException
                || ex is System.TimeoutException
                || (ex.Message?.Contains("network", StringComparison.OrdinalIgnoreCase) == true)
                || (ex.Message?.Contains("connection", StringComparison.OrdinalIgnoreCase) == true);
        }

        private AppBaseException TransformException(System.Exception exception,
                                                  string userFriendlyMessage,
                                                  Dictionary<string, object> contextInfo)
        {
            // Return if it's already an app exception
            if (exception is AppBaseException appEx)
            {
                return appEx;
            }

            // Transform common .NET exceptions to app exceptions
            if (exception is SQLite.SQLiteException)
            {
                return new DatabaseConnectionException(
                    $"SQLite error: {exception.Message}",
                    userFriendlyMessage ?? "Wystąpił problem z bazą danych.",
                    exception,
                    new Dictionary<string, object>(contextInfo));
            }

            if (exception is KeyNotFoundException)
            {
                return new ResourceNotFoundException(
                    $"Resource not found: {exception.Message}",
                    userFriendlyMessage ?? "Nie znaleziono żądanego zasobu.",
                    exception,
                    new Dictionary<string, object>(contextInfo));
            }

            if (exception is ArgumentException || exception is FormatException)
            {
                return new InvalidInputException(
                    $"Invalid input: {exception.Message}",
                    userFriendlyMessage ?? "Nieprawidłowe dane wejściowe.",
                    exception,
                    new Dictionary<string, object>(contextInfo));
            }

            if (exception is System.Net.WebException || exception is System.Net.Http.HttpRequestException)
            {
                return new NetworkException(
                    $"Network error: {exception.Message}",
                    userFriendlyMessage ?? "Wystąpił problem z połączeniem sieciowym. Sprawdź swoje połączenie i spróbuj ponownie.",
                    exception,
                    new Dictionary<string, object>(contextInfo));
            }

            // Default transformation
            return new DomainLogicException(
                exception.Message,
                userFriendlyMessage ?? "Wystąpił błąd w aplikacji.",
                exception,
                new Dictionary<string, object>(contextInfo));
        }

        // Metoda do pokazywania błędów użytkownikowi z ograniczeniem liczby dialogów
        private async Task ShowErrorToUserAsync(string title, string message)
        {
            try
            {
                // Próbujemy zdobyć semafor
                bool semaphoreAcquired = await _dialogSemaphore.WaitAsync(100);

                if (!semaphoreAcquired)
                {
                    // Nie udało się zdobyć semafora - możliwy wyścig lub inny dialog jest już pokazywany
                    _loggingService.LogWarning($"Dialog semaphore acquisition failed for error: {message}",
                        new Dictionary<string, object> { { "Title", title } });
                    return;
                }

                try
                {
                    // Oczyść starą listę błędów (starsze niż 5 sekund)
                    if ((DateTime.Now - _lastErrorTime).TotalSeconds > 5)
                    {
                        _recentErrors.Clear();
                    }

                    // Sprawdź czy ten sam błąd nie został już wyświetlony ostatnio
                    if (_recentErrors.Contains(message))
                    {
                        return;
                    }

                    // Dodaj do listy ostatnich błędów
                    _recentErrors.Add(message);
                    _lastErrorTime = DateTime.Now;

                    // Pokaż dialog
                    await _dialogService.DisplayAlertAsync(title, message, "OK");
                }
                finally
                {
                    // Zwolnij semafor
                    _dialogSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                // Coś poszło nie tak przy wyświetlaniu błędu, logujemy to, ale nie pokazujemy 
                // użytkownikowi (aby uniknąć pętli)
                _loggingService.LogError(ex, $"Error displaying error dialog: {ex.Message}",
                    new Dictionary<string, object> { { "OriginalMessage", message } });
            }
        }
    }
}
```

## File: Services/Exceptions/IExceptionHandlerService.cs
```csharp
using SledzSpecke.App.Exceptions;

namespace SledzSpecke.App.Services.Exceptions
{
    public interface IExceptionHandlerService
    {
        Task HandleExceptionAsync(System.Exception exception, string callerMemberName = "");
        Task<T> ExecuteAsync<T>(Func<Task<T>> operation, Dictionary<string, object> contextInfo = null,
                               string userFriendlyMessage = null, string operationName = "");
        Task ExecuteAsync(Func<Task> operation, Dictionary<string, object> contextInfo = null,
                         string userFriendlyMessage = null, string operationName = "");

        // Nowe metody do wykonywania operacji z ponawianiem
        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, Dictionary<string, object> contextInfo = null,
                                        string userFriendlyMessage = null, int retryCount = 3,
                                        int delayMilliseconds = 500, string operationName = "");
        Task ExecuteWithRetryAsync(Func<Task> operation, Dictionary<string, object> contextInfo = null,
                                  string userFriendlyMessage = null, int retryCount = 3,
                                  int delayMilliseconds = 500, string operationName = "");
    }
}
```

## File: Services/FileSystem/FileSystemService.cs
```csharp
namespace SledzSpecke.App.Services.FileSystem
{
    public class FileSystemService : IFileSystemService
    {
        public string AppDataDirectory => Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
    }
}
```

## File: Services/FileSystem/IFileSystemService.cs
```csharp
namespace SledzSpecke.App.Services.FileSystem
{
    public interface IFileSystemService
    {
        string AppDataDirectory { get; }
    }
}
```

## File: Services/Logging/ILoggingService.cs
```csharp
namespace SledzSpecke.App.Services.Logging
{
    public interface ILoggingService
    {
        void LogInformation(string message, Dictionary<string, object> properties = null, string callerMemberName = "");
        void LogWarning(string message, Dictionary<string, object> properties = null, string callerMemberName = "");
        void LogError(System.Exception exception, string message, Dictionary<string, object> properties = null, string callerMemberName = "");
    }
}
```

## File: Services/Logging/LoggingService.cs
```csharp
using System.Runtime.CompilerServices;

namespace SledzSpecke.App.Services.Logging
{
    public class LoggingService : ILoggingService
    {
        public void LogInformation(string message, Dictionary<string, object> properties = null,
                                 [CallerMemberName] string callerMemberName = "")
        {
            // Simple debug logging - can be replaced with a more sophisticated logging system
            System.Diagnostics.Debug.WriteLine($"INFO [{callerMemberName}]: {message}");
            LogProperties(properties);
        }

        public void LogWarning(string message, Dictionary<string, object> properties = null,
                              [CallerMemberName] string callerMemberName = "")
        {
            System.Diagnostics.Debug.WriteLine($"WARNING [{callerMemberName}]: {message}");
            LogProperties(properties);
        }

        public void LogError(System.Exception exception, string message, Dictionary<string, object> properties = null,
                           [CallerMemberName] string callerMemberName = "")
        {
            System.Diagnostics.Debug.WriteLine($"ERROR [{callerMemberName}]: {message}");
            System.Diagnostics.Debug.WriteLine($"Exception: {exception.GetType().Name}: {exception.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {exception.StackTrace}");

            if (exception.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {exception.InnerException.GetType().Name}: {exception.InnerException.Message}");
            }

            LogProperties(properties);
        }

        private void LogProperties(Dictionary<string, object> properties)
        {
            if (properties != null && properties.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Context Properties:");
                foreach (var property in properties)
                {
                    System.Diagnostics.Debug.WriteLine($"  {property.Key}: {property.Value}");
                }
            }
        }
    }
}
```

## File: Services/MedicalShifts/IMedicalShiftsService.cs
```csharp
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public interface IMedicalShiftsService
    {
        Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? year = null, int? internshipRequirementId = null);
        Task<List<int>> GetAvailableYearsAsync();
        Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year);
        Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId);
        Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift);
        Task<bool> DeleteOldSMKShiftAsync(int shiftId);
        Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync();
        Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId);
        Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift);
    }
}
```

## File: Services/MedicalShifts/MedicalShiftsService.cs
```csharp
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Exceptions;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public partial class MedicalShiftsService : BaseService, IMedicalShiftsService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly ISpecializationService specializationService;

        public MedicalShiftsService(
            IDatabaseService databaseService,
            IAuthService authService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService loggingService)
            : base(exceptionHandler, loggingService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.specializationService = specializationService ?? throw new ArgumentNullException(nameof(specializationService));
        }

        public async Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania dyżurów bez zalogowanego użytkownika",
                        new Dictionary<string, object> { { "Year", year } });
                    return new List<RealizedMedicalShiftOldSMK>();
                }

                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ? ORDER BY StartDate DESC";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, user.SpecializationId, year);
                var filteredShifts = shifts.Where(s => s.SpecializationId == user.SpecializationId).ToList();

                Logger.LogInformation($"Pobrano {filteredShifts.Count} dyżurów dla roku {year}",
                    new Dictionary<string, object> { { "Year", year }, { "Count", filteredShifts.Count } });

                return filteredShifts;
            },
            $"Wystąpił błąd podczas pobierania dyżurów dla roku {year}",
            new Dictionary<string, object> { { "Year", year } },
            withRetry: true);
        }

        public async Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania dyżuru bez zalogowanego użytkownika",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });
                    return null;
                }

                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, shiftId, user.SpecializationId);

                if (shifts.Count > 0)
                {
                    Logger.LogInformation($"Pobrano dyżur o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });
                    return shifts[0];
                }
                else
                {
                    var checkQuery = "SELECT COUNT(*) FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ?";
                    var count = await this.databaseService.QueryAsync<CountResult>(checkQuery, shiftId);

                    if (count.Count > 0 && count[0].Count > 0)
                    {
                        // Dyżur istnieje, ale należy do innego użytkownika
                        Logger.LogWarning($"Próba dostępu do dyżuru o ID {shiftId} należącego do innego użytkownika",
                            new Dictionary<string, object> { { "ShiftId", shiftId }, { "UserId", user.UserId } });

                        throw new ResourceNotFoundException(
                            $"Medical shift with ID {shiftId} not found for the current user",
                            $"Nie znaleziono dyżuru o ID {shiftId} dla bieżącego użytkownika.",
                            null,
                            new Dictionary<string, object> { { "ShiftId", shiftId }, { "UserId", user.UserId } });
                    }

                    Logger.LogInformation($"Nie znaleziono dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });
                    return null;
                }
            },
            $"Wystąpił błąd podczas pobierania dyżuru o ID {shiftId}",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "Shift cannot be null",
                        "Obiekt dyżuru nie może być pusty.");
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba zapisania dyżuru bez zalogowanego użytkownika");
                    throw new InvalidInputException(
                        "User not logged in",
                        "Aby zapisać dyżur, musisz być zalogowany.");
                }

                shift.SpecializationId = user.SpecializationId;

                if (shift.ShiftId == 0)
                {
                    // Nowy dyżur
                    Logger.LogInformation("Dodawanie nowego dyżuru",
                        new Dictionary<string, object> { { "SpecializationId", shift.SpecializationId } });

                    int result = await this.databaseService.InsertAsync(shift);
                    return result > 0;
                }
                else
                {
                    // Edycja istniejącego dyżuru
                    var existingShift = await this.GetOldSMKShiftAsync(shift.ShiftId);
                    if (existingShift != null && existingShift.SpecializationId != user.SpecializationId)
                    {
                        Logger.LogWarning("Próba edycji dyżuru należącego do innego użytkownika",
                            new Dictionary<string, object> {
                                { "ShiftId", shift.ShiftId },
                                { "RequestedSpecId", shift.SpecializationId },
                                { "ActualSpecId", existingShift.SpecializationId }
                            });

                        throw new ResourceNotFoundException(
                            "Cannot edit shift belonging to another user",
                            "Nie można edytować dyżuru należącego do innego użytkownika.");
                    }

                    Logger.LogInformation($"Aktualizacja dyżuru o ID {shift.ShiftId}",
                        new Dictionary<string, object> { { "ShiftId", shift.ShiftId } });

                    int result = await this.databaseService.UpdateAsync(shift);
                    return result > 0;
                }
            },
            "Wystąpił błąd podczas zapisywania dyżuru",
            new Dictionary<string, object> {
                { "ShiftId", shift?.ShiftId ?? 0 },
                { "IsNew", shift?.ShiftId == 0 }
            });
        }

        public async Task<bool> DeleteOldSMKShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba usunięcia dyżuru bez zalogowanego użytkownika",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    throw new InvalidInputException(
                        "User not logged in",
                        "Aby usunąć dyżur, musisz być zalogowany.");
                }

                var shift = await this.GetOldSMKShiftAsync(shiftId);
                if (shift == null)
                {
                    Logger.LogWarning($"Próba usunięcia nieistniejącego dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    return false;
                }

                if (shift.SyncStatus == SyncStatus.Synced)
                {
                    Logger.LogWarning($"Próba usunięcia zsynchronizowanego dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    throw new BusinessRuleViolationException(
                        "Cannot delete synced shift",
                        "Nie można usunąć zsynchronizowanego dyżuru.");
                }

                Logger.LogInformation($"Usuwanie dyżuru o ID {shiftId}",
                    new Dictionary<string, object> { { "ShiftId", shiftId } });

                var query = "DELETE FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
                int result = await this.databaseService.ExecuteAsync(query, shiftId, user.SpecializationId);

                return result > 0;
            },
            $"Wystąpił błąd podczas usuwania dyżuru o ID {shiftId}",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                int? moduleId = currentModule?.ModuleId;

                if (!moduleId.HasValue)
                {
                    Logger.LogWarning("Próba pobrania dyżurów bez aktywnego modułu",
                        new Dictionary<string, object> { { "InternshipRequirementId", internshipRequirementId } });

                    return new List<RealizedMedicalShiftNewSMK>();
                }

                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE InternshipRequirementId = ? AND ModuleId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, internshipRequirementId, moduleId);

                Logger.LogInformation($"Pobrano {shifts.Count} dyżurów dla wymagania staży o ID {internshipRequirementId}",
                    new Dictionary<string, object> {
                        { "InternshipRequirementId", internshipRequirementId },
                        { "ModuleId", moduleId },
                        { "Count", shifts.Count }
                    });

                return shifts;
            },
            $"Wystąpił błąd podczas pobierania dyżurów dla wymagania staży o ID {internshipRequirementId}",
            new Dictionary<string, object> { { "InternshipRequirementId", internshipRequirementId } },
            withRetry: true);
        }

        public async Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "Shift cannot be null",
                        "Obiekt dyżuru nie może być pusty.");
                }

                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule == null)
                {
                    Logger.LogWarning("Próba zapisania dyżuru bez aktywnego modułu");

                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktywnego modułu specjalizacji.");
                }

                shift.ModuleId = currentModule.ModuleId;

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    Logger.LogWarning("Próba zapisania dyżuru bez aktywnej specjalizacji");

                    throw new ResourceNotFoundException(
                        "Current specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                shift.SpecializationId = specialization.SpecializationId;

                Logger.LogInformation($"{(shift.ShiftId > 0 ? "Aktualizacja" : "Dodawanie")} dyżuru dla modułu {currentModule.ModuleId}",
                    new Dictionary<string, object> {
                        { "ShiftId", shift.ShiftId },
                        { "ModuleId", currentModule.ModuleId },
                        { "SpecializationId", specialization.SpecializationId }
                    });

                int result = await this.databaseService.InsertAsync(shift);
                return result > 0;
            },
            "Wystąpił błąd podczas zapisywania dyżuru",
            new Dictionary<string, object> {
                { "ShiftId", shift?.ShiftId ?? 0 },
                { "InternshipRequirementId", shift?.InternshipRequirementId ?? 0 }
            });
        }

        public async Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? year = null, int? internshipRequirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var summary = new MedicalShiftsSummary();

                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                int? moduleId = currentModule?.ModuleId;

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    Logger.LogWarning("Próba pobrania podsumowania dyżurów bez aktywnej specjalizacji");
                    return summary;
                }

                if (internshipRequirementId.HasValue)
                {
                    string query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND InternshipRequirementId = ? AND ModuleId = ?";
                    var newSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                        query, specialization.SpecializationId, internshipRequirementId.Value, moduleId);

                    foreach (var shift in newSmkShifts)
                    {
                        summary.TotalHours += shift.Hours;
                        summary.TotalMinutes += shift.Minutes;

                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedHours += shift.Hours;
                            summary.ApprovedMinutes += shift.Minutes;
                        }
                    }

                    Logger.LogInformation($"Pobrano podsumowanie dyżurów dla wymagania staży o ID {internshipRequirementId}",
                        new Dictionary<string, object> {
                            { "InternshipRequirementId", internshipRequirementId },
                            { "TotalHours", summary.TotalHours },
                            { "TotalMinutes", summary.TotalMinutes }
                        });
                }
                else if (year.HasValue)
                {
                    string query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ?";
                    var oldSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(
                        query, specialization.SpecializationId, year.Value);

                    foreach (var shift in oldSmkShifts)
                    {
                        summary.TotalHours += shift.Hours;
                        summary.TotalMinutes += shift.Minutes;

                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedHours += shift.Hours;
                            summary.ApprovedMinutes += shift.Minutes;
                        }
                    }

                    Logger.LogInformation($"Pobrano podsumowanie dyżurów dla roku {year}",
                        new Dictionary<string, object> {
                            { "Year", year },
                            { "TotalHours", summary.TotalHours },
                            { "TotalMinutes", summary.TotalMinutes }
                        });
                }
                else
                {
                    string query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND ModuleId = ?";
                    var newSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                        query, specialization.SpecializationId, moduleId);

                    foreach (var shift in newSmkShifts)
                    {
                        summary.TotalHours += shift.Hours;
                        summary.TotalMinutes += shift.Minutes;

                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedHours += shift.Hours;
                            summary.ApprovedMinutes += shift.Minutes;
                        }
                    }

                    Logger.LogInformation($"Pobrano podsumowanie wszystkich dyżurów dla modułu {moduleId}",
                        new Dictionary<string, object> {
                            { "ModuleId", moduleId },
                            { "TotalHours", summary.TotalHours },
                            { "TotalMinutes", summary.TotalMinutes }
                        });
                }

                summary.NormalizeTime();

                return summary;
            },
            "Wystąpił błąd podczas pobierania podsumowania dyżurów",
            new Dictionary<string, object> {
                { "Year", year },
                { "InternshipRequirementId", internshipRequirementId }
            });
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    Logger.LogWarning("Próba pobrania dostępnych lat bez aktywnej specjalizacji");

                    throw new ResourceNotFoundException(
                        "Current specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters = { new JsonStringEnumConverter() }
                };

                var specializationStructure = JsonSerializer.Deserialize<SpecializationStructure>(
                    specialization.ProgramStructure, options);

                int totalYears = 0;
                if (specializationStructure.TotalDuration != null)
                {
                    totalYears = specializationStructure.TotalDuration.Years;
                    if (specializationStructure.TotalDuration.Months > 0)
                    {
                        totalYears++;
                    }
                }
                else
                {
                    totalYears = specializationStructure.Modules?.Sum(m => m.Duration?.Years ?? 0) ?? 0;
                    int additionalMonths = specializationStructure.Modules?.Sum(m => m.Duration?.Months ?? 0) ?? 0;
                    if (additionalMonths > 0)
                    {
                        totalYears += (additionalMonths / 12) + (additionalMonths % 12 > 0 ? 1 : 0);
                    }
                }

                totalYears = Math.Max(1, totalYears);
                totalYears = Math.Min(6, totalYears);

                var years = Enumerable.Range(1, totalYears).ToList();

                Logger.LogInformation($"Pobrano {years.Count} dostępnych lat dla specjalizacji",
                    new Dictionary<string, object> { { "TotalYears", totalYears } });

                return years;
            },
            "Wystąpił błąd podczas pobierania dostępnych lat",
            new Dictionary<string, object>());
        }

        public async Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var module = await this.specializationService.GetCurrentModuleAsync();
                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    Logger.LogWarning("Próba pobrania wymagań stażowych bez aktywnego modułu lub z pustą strukturą");

                    throw new ResourceNotFoundException(
                        "Current module not found or has empty structure",
                        "Nie znaleziono aktywnego modułu specjalizacji lub moduł ma pustą strukturę.");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters = { new JsonStringEnumConverter() }
                };

                var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                var requirements = moduleStructure?.Internships ?? new List<InternshipRequirement>();

                Logger.LogInformation($"Pobrano {requirements.Count} wymagań stażowych dla modułu {module.ModuleId}",
                    new Dictionary<string, object> { { "ModuleId", module.ModuleId }, { "Count", requirements.Count } });

                return requirements;
            },
            "Wystąpił błąd podczas pobierania wymagań stażowych",
            new Dictionary<string, object>());
        }

        private class CountResult
        {
            public int Count { get; set; }
        }
    }
}
```

## File: Services/MedicalShifts/YearResult.cs
```csharp
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public partial class MedicalShiftsService
    {
        private class YearResult
        {
            public int Year { get; set; }
        }
    }
}
```

## File: Services/Procedures/IProcedureService.cs
```csharp
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Procedures
{
    public interface IProcedureService
    {
        Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null);
        Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? procedureRequirementId = null);
        Task<(int completed, int total)> GetProcedureStatisticsForModuleAsync(int moduleId);
        Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null, int? requirementId = null);
        Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId);
        Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure);
        Task<bool> DeleteOldSMKProcedureAsync(int procedureId);
        Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? procedureRequirementId = null);
        Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId);
        Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure);
        Task<bool> DeleteNewSMKProcedureAsync(int procedureId);
    }
}
```

## File: Services/Procedures/ProcedureService.cs
```csharp
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;
using SledzSpecke.App.Services.Specialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SledzSpecke.App.Services.Procedures
{
    public class ProcedureService : BaseService, IProcedureService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly ISpecializationService specializationService;

        public ProcedureService(
            IDatabaseService databaseService,
            IAuthService authService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService logger) : base(exceptionHandler, logger)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.specializationService = specializationService ?? throw new ArgumentNullException(nameof(specializationService));
        }

        public async Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (!moduleId.HasValue)
                {
                    Logger.LogInformation("Zwrócono pustą listę wymagań procedurowych - nie podano ID modułu");
                    return new List<ProcedureRequirement>();
                }

                var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    Logger.LogWarning($"Moduł o ID {moduleId.Value} nie istnieje lub nie ma struktury");
                    return new List<ProcedureRequirement>();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters =
                    {
                        new JsonStringEnumConverter(),
                        new ModuleTypeJsonConverter()
                    }
                };

                var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);

                if (moduleStructure?.Procedures == null)
                {
                    Logger.LogInformation($"Moduł o ID {moduleId.Value} nie ma zdefiniowanych procedur");
                    return new List<ProcedureRequirement>();
                }

                return moduleStructure.Procedures;
            }, "Nie udało się pobrać dostępnych procedur",
               new Dictionary<string, object> { { "ModuleId", moduleId } },
               withRetry: true);
        }

        public async Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null, int? requirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania procedur bez zalogowanego użytkownika");
                    return new List<RealizedProcedureOldSMK>();
                }

                var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ?";
                var parameters = new List<object> { user.SpecializationId };

                if (moduleId.HasValue)
                {
                    var internships = await this.databaseService.GetInternshipsAsync(moduleId: moduleId);

                    if (internships.Any())
                    {
                        sql += " AND (";
                        for (int i = 0; i < internships.Count; i++)
                        {
                            if (i > 0) sql += " OR ";
                            sql += "InternshipId = ?";
                            parameters.Add(internships[i].InternshipId);
                        }
                        sql += ")";
                    }
                }

                if (year.HasValue)
                {
                    sql += " AND Year = ?";
                    parameters.Add(year.Value);
                }

                if (requirementId.HasValue)
                {
                    sql += " AND ProcedureRequirementId = ?";
                    parameters.Add(requirementId.Value);
                }

                sql += " ORDER BY Date DESC";
                var result = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, parameters.ToArray());
                Logger.LogInformation($"Pobrano {result.Count} procedur dla starego SMK",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "Year", year },
                        { "RequirementId", requirementId },
                        { "SpecializationId", user.SpecializationId }
                    });

                return result;
            }, "Nie udało się pobrać procedur",
               new Dictionary<string, object> {
                   { "ModuleId", moduleId },
                   { "Year", year },
                   { "RequirementId", requirementId }
               });
        }

        public async Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var sql = "SELECT * FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
                var results = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, procedureId);

                var procedure = results.FirstOrDefault();
                if (procedure == null)
                {
                    Logger.LogWarning($"Nie znaleziono procedury o ID {procedureId}");
                }

                return procedure;
            }, "Nie udało się pobrać szczegółów procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } },
               withRetry: true);
        }

        public async Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new ArgumentNullException(nameof(procedure), "Procedura nie może być pusta");
                }

                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        Logger.LogWarning("Próba zapisu procedury bez zalogowanego użytkownika");
                        return false;
                    }

                    procedure.SpecializationId = user.SpecializationId;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    procedure.SyncStatus = SyncStatus.Modified;
                }
                else if (procedure.SyncStatus != SyncStatus.Modified)
                {
                    procedure.SyncStatus = SyncStatus.NotSynced;
                }

                int result;
                if (procedure.ProcedureId == 0)
                {
                    result = await this.databaseService.InsertAsync(procedure);
                    Logger.LogInformation($"Utworzono nową procedurę z ID {result}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "InternshipId", procedure.InternshipId },
                            { "Code", procedure.Code }
                        });
                }
                else
                {
                    result = await this.databaseService.UpdateAsync(procedure);
                    Logger.LogInformation($"Zaktualizowano procedurę o ID {procedure.ProcedureId}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "InternshipId", procedure.InternshipId },
                            { "Code", procedure.Code }
                        });
                }

                return result > 0;
            }, "Nie udało się zapisać procedury",
               new Dictionary<string, object> {
                   { "ProcedureId", procedure?.ProcedureId },
                   { "SpecializationId", procedure?.SpecializationId }
               });
        }

        public async Task<bool> DeleteOldSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var procedure = await this.GetOldSMKProcedureAsync(procedureId);
                if (procedure == null)
                {
                    Logger.LogWarning($"Próba usunięcia nieistniejącej procedury o ID {procedureId}");
                    return false;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    Logger.LogWarning($"Próba usunięcia zsynchronizowanej procedury o ID {procedureId}");
                    return false;
                }

                var sql = "DELETE FROM RealizedProcedureOldSMK WHERE ProcedureId = ?";
                int result = await this.databaseService.ExecuteAsync(sql, procedureId);

                Logger.LogInformation($"Usunięto procedurę o ID {procedureId}",
                    new Dictionary<string, object> {
                        { "SpecializationId", procedure.SpecializationId },
                        { "InternshipId", procedure.InternshipId },
                        { "Code", procedure.Code }
                    });

                return result > 0;
            }, "Nie udało się usunąć procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } });
        }

        public async Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? requirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania procedur bez zalogowanego użytkownika");
                    return new List<RealizedProcedureNewSMK>();
                }

                var sql = "SELECT * FROM RealizedProcedureNewSMK WHERE SpecializationId = ?";
                var parameters = new List<object> { user.SpecializationId };

                if (moduleId.HasValue)
                {
                    sql += " AND ModuleId = ?";
                    parameters.Add(moduleId.Value);
                }

                if (requirementId.HasValue)
                {
                    sql += " AND ProcedureRequirementId = ?";
                    parameters.Add(requirementId.Value);
                }

                sql += " ORDER BY Date DESC";

                var procedures = await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(sql, parameters.ToArray());

                if (moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    foreach (var procedure in procedures)
                    {
                        var requirement = requirements.FirstOrDefault(r => r.Id == procedure.ProcedureRequirementId);
                        if (requirement != null)
                        {
                            procedure.ProcedureName = requirement.Name;
                            procedure.InternshipName = string.Empty;
                        }
                    }
                }

                Logger.LogInformation($"Pobrano {procedures.Count} procedur dla nowego SMK",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "RequirementId", requirementId },
                        { "SpecializationId", user.SpecializationId }
                    });

                return procedures;
            }, "Nie udało się pobrać procedur",
               new Dictionary<string, object> {
                   { "ModuleId", moduleId },
                   { "RequirementId", requirementId }
               });
        }

        public async Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var sql = "SELECT * FROM RealizedProcedureNewSMK WHERE ProcedureId = ?";
                var results = await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(sql, procedureId);

                var procedure = results.FirstOrDefault();
                if (procedure != null && procedure.ModuleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(procedure.ModuleId);
                    var requirement = requirements.FirstOrDefault(r => r.Id == procedure.ProcedureRequirementId);
                    if (requirement != null)
                    {
                        procedure.ProcedureName = requirement.Name;
                        procedure.InternshipName = string.Empty;
                    }
                }
                else
                {
                    Logger.LogWarning($"Nie znaleziono procedury o ID {procedureId}");
                }

                return procedure;
            }, "Nie udało się pobrać szczegółów procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } },
               withRetry: true);
        }

        public async Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new ArgumentNullException(nameof(procedure), "Procedura nie może być pusta");
                }

                if (procedure.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user == null)
                    {
                        Logger.LogWarning("Próba zapisu procedury bez zalogowanego użytkownika");
                        return false;
                    }

                    procedure.SpecializationId = user.SpecializationId;
                }

                if (procedure.Date == default)
                {
                    procedure.Date = DateTime.Now;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    procedure.SyncStatus = SyncStatus.Modified;
                }
                else if (procedure.SyncStatus != SyncStatus.Modified)
                {
                    procedure.SyncStatus = SyncStatus.NotSynced;
                }

                int result;
                if (procedure.ProcedureId == 0)
                {
                    result = await this.databaseService.InsertAsync(procedure);
                    Logger.LogInformation($"Utworzono nową procedurę z ID {result}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "ModuleId", procedure.ModuleId },
                            { "ProcedureRequirementId", procedure.ProcedureRequirementId },
                            { "CountA", procedure.CountA },
                            { "CountB", procedure.CountB }
                        });
                }
                else
                {
                    result = await this.databaseService.UpdateAsync(procedure);
                    Logger.LogInformation($"Zaktualizowano procedurę o ID {procedure.ProcedureId}",
                        new Dictionary<string, object> {
                            { "SpecializationId", procedure.SpecializationId },
                            { "ModuleId", procedure.ModuleId },
                            { "ProcedureRequirementId", procedure.ProcedureRequirementId },
                            { "CountA", procedure.CountA },
                            { "CountB", procedure.CountB }
                        });
                }

                return result > 0;
            }, "Nie udało się zapisać procedury",
               new Dictionary<string, object> {
                   { "ProcedureId", procedure?.ProcedureId },
                   { "SpecializationId", procedure?.SpecializationId },
                   { "ModuleId", procedure?.ModuleId }
               });
        }

        public async Task<bool> DeleteNewSMKProcedureAsync(int procedureId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var procedure = await this.GetNewSMKProcedureAsync(procedureId);
                if (procedure == null)
                {
                    Logger.LogWarning($"Próba usunięcia nieistniejącej procedury o ID {procedureId}");
                    return false;
                }

                if (procedure.SyncStatus == SyncStatus.Synced)
                {
                    Logger.LogWarning($"Próba usunięcia zsynchronizowanej procedury o ID {procedureId}");
                    return false;
                }

                var sql = "DELETE FROM RealizedProcedureNewSMK WHERE ProcedureId = ?";
                int result = await this.databaseService.ExecuteAsync(sql, procedureId);

                Logger.LogInformation($"Usunięto procedurę o ID {procedureId}",
                    new Dictionary<string, object> {
                        { "SpecializationId", procedure.SpecializationId },
                        { "ModuleId", procedure.ModuleId },
                        { "ProcedureRequirementId", procedure.ProcedureRequirementId }
                    });

                return result > 0;
            }, "Nie udało się usunąć procedury",
               new Dictionary<string, object> { { "ProcedureId", procedureId } });
        }

        public async Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? requirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var summary = new ProcedureSummary();

                if (requirementId.HasValue && moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    var requirement = requirements.FirstOrDefault(r => r.Id == requirementId.Value);
                    if (requirement != null)
                    {
                        summary.RequiredCountA = requirement.RequiredCountA;
                        summary.RequiredCountB = requirement.RequiredCountB;
                    }
                }
                else if (moduleId.HasValue)
                {
                    var requirements = await this.GetAvailableProcedureRequirementsAsync(moduleId);
                    summary.RequiredCountA = requirements.Sum(r => r.RequiredCountA);
                    summary.RequiredCountB = requirements.Sum(r => r.RequiredCountB);
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba obliczenia statystyk procedur bez zalogowanego użytkownika");
                    return summary;
                }

                if (user.SmkVersion == SmkVersion.Old)
                {
                    if (moduleId.HasValue)
                    {
                        var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                        if (module != null)
                        {
                            int startYear = module.Type == ModuleType.Basic ? 1 : 3;
                            int endYear = module.Type == ModuleType.Basic ? 2 : 6;
                            var modules = await this.databaseService.GetModulesAsync(user.SpecializationId);
                            if (!modules.Any(m => m.Type == ModuleType.Basic))
                            {
                                startYear = 1;
                            }

                            var sql = "SELECT Code, SyncStatus FROM RealizedProcedureOldSMK WHERE SpecializationId = ? AND Year BETWEEN ? AND ?";
                            var parameters = new List<object> { user.SpecializationId, startYear, endYear };

                            if (requirementId.HasValue)
                            {
                                sql += " AND ProcedureRequirementId = ?";
                                parameters.Add(requirementId.Value);
                            }
                            else
                            {
                                var internships = await this.databaseService.GetInternshipsAsync(moduleId: moduleId);

                                if (internships.Any())
                                {
                                    sql += " AND (";
                                    for (int i = 0; i < internships.Count; i++)
                                    {
                                        if (i > 0) sql += " OR ";
                                        sql += "InternshipId = ?";
                                        parameters.Add(internships[i].InternshipId);
                                    }
                                    sql += ")";
                                }
                            }

                            var procedures = await this.databaseService.QueryAsync<RealizedProcedureOldSMK>(sql, parameters.ToArray());

                            foreach (var procedure in procedures)
                            {
                                if (procedure.Code == "A - operator")
                                {
                                    summary.CompletedCountA++;
                                    if (procedure.SyncStatus == SyncStatus.Synced)
                                    {
                                        summary.ApprovedCountA++;
                                    }
                                }
                                else if (procedure.Code == "B - asysta")
                                {
                                    summary.CompletedCountB++;
                                    if (procedure.SyncStatus == SyncStatus.Synced)
                                    {
                                        summary.ApprovedCountB++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var sql = "SELECT CountA, CountB, SyncStatus FROM RealizedProcedureNewSMK WHERE SpecializationId = ?";
                    var parameters = new List<object> { user.SpecializationId };

                    if (moduleId.HasValue)
                    {
                        sql += " AND ModuleId = ?";
                        parameters.Add(moduleId.Value);
                    }

                    if (requirementId.HasValue)
                    {
                        sql += " AND ProcedureRequirementId = ?";
                        parameters.Add(requirementId.Value);
                    }

                    var procedures = await this.databaseService.QueryAsync<RealizedProcedureNewSMK>(sql, parameters.ToArray());

                    foreach (var procedure in procedures)
                    {
                        summary.CompletedCountA += procedure.CountA;
                        summary.CompletedCountB += procedure.CountB;

                        if (procedure.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedCountA += procedure.CountA;
                            summary.ApprovedCountB += procedure.CountB;
                        }
                    }
                }

                Logger.LogInformation($"Obliczono statystyki procedur",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "RequirementId", requirementId },
                        { "CompletedA", summary.CompletedCountA },
                        { "CompletedB", summary.CompletedCountB },
                        { "RequiredA", summary.RequiredCountA },
                        { "RequiredB", summary.RequiredCountB }
                    });

                return summary;
            }, "Nie udało się obliczyć statystyk procedur",
               new Dictionary<string, object> {
                   { "ModuleId", moduleId },
                   { "RequirementId", requirementId }
               });
        }

        public async Task<(int completed, int total)> GetProcedureStatisticsForModuleAsync(int moduleId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba obliczenia statystyk procedur bez zalogowanego użytkownika");
                    return (0, 0);
                }

                var module = await this.databaseService.GetModuleAsync(moduleId);
                if (module == null)
                {
                    Logger.LogWarning($"Próba obliczenia statystyk procedur dla nieistniejącego modułu o ID {moduleId}");
                    return (0, 0);
                }

                int completedCount = 0;
                int totalRequired = module.TotalProceduresA + module.TotalProceduresB;

                if (user.SmkVersion == SmkVersion.Old)
                {
                    var procedures = await this.GetOldSMKProceduresAsync(moduleId: moduleId);
                    completedCount = procedures.Count;
                }
                else
                {
                    var procedures = await this.GetNewSMKProceduresAsync(moduleId: moduleId);
                    completedCount = procedures.Sum(p => p.CountA + p.CountB);
                }

                Logger.LogInformation($"Obliczono statystyki procedur dla modułu",
                    new Dictionary<string, object> {
                        { "ModuleId", moduleId },
                        { "Completed", completedCount },
                        { "Total", totalRequired }
                    });

                return (completedCount, totalRequired);
            }, "Nie udało się obliczyć statystyk procedur dla modułu",
               new Dictionary<string, object> { { "ModuleId", moduleId } });
        }
    }
}
```

## File: Services/Specialization/ISpecializationService.cs
```csharp
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Specialization
{
    public interface ISpecializationService
    {
        Task<Models.Specialization> GetCurrentSpecializationAsync(bool includeModules = true);

        Task<Module> GetCurrentModuleAsync();

        Task SetCurrentModuleAsync(int moduleId);

        Task<List<Module>> GetModulesAsync(int specializationId);

        Task<bool> InitializeSpecializationModulesAsync(int specializationId);

        Task<List<Internship>> GetInternshipsAsync(int? moduleId = null);

        Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null);

        Task<bool> AddMedicalShiftAsync(MedicalShift shift);

        Task<bool> UpdateMedicalShiftAsync(MedicalShift shift);

        Task<bool> DeleteMedicalShiftAsync(int shiftId);

        event EventHandler<int> CurrentModuleChanged;

        Task<SpecializationStatistics> GetSpecializationStatisticsAsync(int? moduleId = null);

        Task UpdateModuleProgressAsync(int moduleId);

        Task<int> GetShiftCountAsync(int? moduleId = null);

        Task<int> GetInternshipCountAsync(int? moduleId = null);

        Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? moduleId = null, int? internshipRequirementId = null);

        Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? year = null);

        Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id);

        Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id);

        Task<bool> AddRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<bool> AddRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task<bool> UpdateRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<bool> UpdateRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task<bool> DeleteRealizedInternshipNewSMKAsync(int id);

        Task<bool> DeleteRealizedInternshipOldSMKAsync(int id);
    }
}
```

## File: Services/Specialization/ModuleInitializer.cs
```csharp
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Services.Specialization
{
    public class ModuleInitializer
    {
        private readonly IDatabaseService databaseService;

        public ModuleInitializer(IDatabaseService databaseService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public async Task<bool> InitializeModulesIfNeededAsync(int specializationId)
        {
            var specialization = await databaseService.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return false;
            }

            var existingModules = await databaseService.GetModulesAsync(specializationId);
            if (existingModules?.Count > 0)
            {
                return true;
            }

            var modules = await ModuleHelper.CreateModulesForSpecializationAsync(
                specialization.ProgramCode,
                specialization.StartDate,
                specialization.SmkVersion,
                specializationId);

            databaseService.ClearCache();

            return true;
        }
    }
}
```

## File: Services/Specialization/SpecializationService.cs
```csharp
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Services.Specialization
{
    public class SpecializationService : BaseService, ISpecializationService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ModuleInitializer moduleInitializer;
        private Models.Specialization _cachedSpecialization;
        private List<Module> _cachedModules;

        public SpecializationService(
            IDatabaseService databaseService,
            IAuthService authService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService Logger) : base(exceptionHandler, Logger)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.moduleInitializer = new ModuleInitializer(databaseService);
        }

        public async Task<Models.Specialization> GetCurrentSpecializationAsync(bool includeModules = true)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (_cachedSpecialization != null)
                {
                    return _cachedSpecialization;
                }

                var user = await authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return null;
                }

                var specialization = await databaseService.GetSpecializationAsync(user.SpecializationId);
                if (specialization != null && includeModules)
                {
                    specialization.Modules = await GetModulesAsync(specialization.SpecializationId);
                }

                _cachedSpecialization = specialization;
                return specialization;
            },
            "Nie udało się pobrać aktualnej specjalizacji.",
            new Dictionary<string, object> { { "IncludeModules", includeModules } },
            withRetry: true);
        }

        public async Task<Module> GetCurrentModuleAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await GetCurrentSpecializationAsync(false);
                if (specialization?.CurrentModuleId == null)
                {
                    return null;
                }

                return await databaseService.GetModuleAsync(specialization.CurrentModuleId.Value);
            },
            "Nie udało się pobrać aktualnego modułu.",
            withRetry: true);
        }

        public async Task<int> GetInternshipCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return 0;
                }

                // Pobierz aktualny moduł
                Module currentModule = null;
                if (moduleId.HasValue)
                {
                    currentModule = await this.databaseService.GetModuleAsync(moduleId.Value);
                }
                else
                {
                    currentModule = await this.GetCurrentModuleAsync();
                }

                if (currentModule == null)
                {
                    return 0;
                }

                Logger.LogInformation($"GetInternshipCountAsync: Module={currentModule.Name}, ID={currentModule.ModuleId}, Type={currentModule.Type}");

                // Pobierz wymagania stażowe dla danego modułu
                var internshipRequirements = await this.GetInternshipsAsync(currentModule.ModuleId);
                if (internshipRequirements == null || internshipRequirements.Count == 0)
                {
                    return 0;
                }

                Logger.LogInformation($"GetInternshipCountAsync: Znaleziono {internshipRequirements.Count} wymagań stażowych");

                int completedCount = 0;

                // Dla każdego wymagania stażowego
                for (int i = 0; i < internshipRequirements.Count; i++)
                {
                    var requirement = internshipRequirements[i];
                    int requiredDays = requirement.DaysCount;

                    Logger.LogInformation($"GetInternshipCountAsync: Przetwarzanie wymagania {i + 1}/{internshipRequirements.Count}: {requirement.InternshipName}, WymaganeDni={requiredDays}");

                    int introducedDays = 0;

                    if (user.SmkVersion == SmkVersion.New)
                    {
                        // Dla nowego SMK, pobierz realizacje po ID wymagania
                        var realizations = await this.GetRealizedInternshipsNewSMKAsync(
                            moduleId: currentModule.ModuleId,
                            internshipRequirementId: requirement.InternshipId);

                        Logger.LogInformation($"GetInternshipCountAsync: Znaleziono {realizations.Count} realizacji dla nowego SMK");

                        introducedDays = realizations.Sum(r => r.DaysCount);
                    }
                    else
                    {
                        // Dla starego SMK, pobierz wszystkie realizacje dla lat odpowiadających modułowi
                        int startYear = 1;
                        int endYear = 2;

                        if (currentModule.Type == ModuleType.Specialistic)
                        {
                            startYear = 3;
                            endYear = 6;
                        }

                        Logger.LogInformation($"GetInternshipCountAsync: Zakres lat dla modułu: {startYear}-{endYear}");

                        // Pobierz realizacje dla tych lat
                        List<RealizedInternshipOldSMK> allRealizations = new List<RealizedInternshipOldSMK>();
                        for (int year = startYear; year <= endYear; year++)
                        {
                            var yearRealizations = await this.GetRealizedInternshipsOldSMKAsync(year);
                            Logger.LogInformation($"GetInternshipCountAsync: Rok {year}: znaleziono {yearRealizations.Count} realizacji");
                            allRealizations.AddRange(yearRealizations);
                        }

                        // Dodaj również realizacje z year=0 (nieprzypisane do konkretnego roku)
                        var yearZeroRealizations = await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                            "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ? AND Year = 0",
                            specialization.SpecializationId);

                        Logger.LogInformation($"GetInternshipCountAsync: Rok 0 (nieprzypisane): znaleziono {yearZeroRealizations.Count} realizacji");

                        allRealizations.AddRange(yearZeroRealizations);

                        // Filtruj realizacje dla tego konkretnego stażu po nazwie
                        var realizationsForThisRequirement = allRealizations
                            .Where(r => {
                                // Przygotuj nazwy do lepszego porównania
                                string realizationName = r.InternshipName ?? "null";
                                string requirementName = requirement.InternshipName ?? "null";

                                // Jeśli nazwa jest pusta lub "Staż bez nazwy", próbujemy dopasować ją z innymi danymi
                                if (string.IsNullOrEmpty(r.InternshipName)
                                    || r.InternshipName == "Staż bez nazwy")
                                {
                                    bool isFirstRequirement = i == 0;

                                    Logger.LogInformation($"GetInternshipCountAsync: Pusta nazwa realizacji, przypisana do pierwszego wymagania: {isFirstRequirement}");

                                    return isFirstRequirement;
                                }

                                // Standardowe porównanie nazw
                                bool exactMatch = r.InternshipName != null
                                    && r.InternshipName.Equals(requirement.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool realizationContainsRequirement = r.InternshipName != null
                                    && r.InternshipName.Contains(requirement.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool requirementContainsRealization = requirement.InternshipName != null
                                    && requirement.InternshipName.Contains(r.InternshipName, StringComparison.OrdinalIgnoreCase);

                                // Dodatkowe porównanie: usuń spacje i znaki specjalne
                                string cleanRealizationName = realizationName
                                    .Replace(" ", string.Empty)
                                    .Replace("-", string.Empty)
                                    .Replace("_", string.Empty)
                                    .ToLowerInvariant();
                                string cleanRequirementName = requirementName
                                    .Replace(" ", string.Empty)
                                    .Replace("-", string.Empty)
                                    .Replace("_", string.Empty)
                                    .ToLowerInvariant();
                                bool fuzzyMatch = cleanRealizationName.Contains(cleanRequirementName)
                                    || cleanRequirementName.Contains(cleanRealizationName);

                                bool result = exactMatch
                                    || realizationContainsRequirement
                                    || requirementContainsRealization
                                    || fuzzyMatch;

                                Logger.LogInformation($"GetInternshipCountAsync: Porównanie '{realizationName}' do '{requirementName}': ExactMatch={exactMatch}, RealizationContainsRequirement={realizationContainsRequirement}, RequirementContainsRealization={requirementContainsRealization}, FuzzyMatch={fuzzyMatch}, Wynik={result}");

                                return result;
                            }).ToList();

                        Logger.LogInformation($"GetInternshipCountAsync: Znaleziono {realizationsForThisRequirement.Count} realizacji dla tego wymagania");

                        foreach (var realization in realizationsForThisRequirement)
                        {
                            Logger.LogInformation($"GetInternshipCountAsync: Realizacja: {realization.InternshipName}, Dni={realization.DaysCount}");
                        }

                        introducedDays = realizationsForThisRequirement.Sum(r => r.DaysCount);
                    }

                    Logger.LogInformation($"GetInternshipCountAsync: WprowadzoneDni={introducedDays}, WymaganeDni={requiredDays}, CzyUkończone={introducedDays >= requiredDays}");

                    // Jeśli liczba wprowadzonych dni >= wymagana liczba dni, zwiększ licznik ukończonych
                    if (introducedDays >= requiredDays)
                    {
                        completedCount++;
                    }
                }

                Logger.LogInformation($"GetInternshipCountAsync: Końcowy wynik completedCount={completedCount}");

                return completedCount;
            },
            "Nie udało się obliczyć liczby ukończonych staży.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetShiftCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                if (!moduleId.HasValue && specialization.CurrentModuleId.HasValue)
                {
                    moduleId = specialization.CurrentModuleId.Value;
                }

                if (!moduleId.HasValue)
                {
                    var modules = await this.databaseService.GetModulesAsync(specialization.SpecializationId);
                    if (modules.Count > 0)
                    {
                        moduleId = modules[0].ModuleId;
                    }
                }

                if (!moduleId.HasValue)
                {
                    return 0;
                }

                var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                if (module == null)
                {
                    return 0;
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return 0;
                }

                double totalHoursDouble = 0;

                if (user.SmkVersion == SmkVersion.New)
                {
                    var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND ModuleId = ?";
                    var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, specialization.SpecializationId, moduleId.Value);

                    foreach (var shift in shifts)
                    {
                        totalHoursDouble += shift.Hours + ((double)shift.Minutes / 60.0);
                    }
                }
                else
                {
                    int startYear = 1;
                    int endYear = 6;

                    if (module.Type == ModuleType.Basic)
                    {
                        startYear = 1;
                        endYear = 2;
                    }
                    else if (module.Type == ModuleType.Specialistic)
                    {
                        var modules = await this.databaseService.GetModulesAsync(specialization.SpecializationId);
                        bool hasBasicModule = modules.Any(m => m.Type == ModuleType.Basic);

                        if (hasBasicModule)
                        {
                            startYear = 3;
                            endYear = 6;
                        }
                        else
                        {
                            startYear = 1;
                            endYear = 6;
                        }
                    }

                    for (int year = startYear; year <= endYear; year++)
                    {
                        var yearQuery = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ?";
                        var yearShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(yearQuery, specialization.SpecializationId, year);

                        foreach (var shift in yearShifts)
                        {
                            totalHoursDouble += shift.Hours + ((double)shift.Minutes / 60.0);
                        }
                    }
                }

                int totalHours = (int)Math.Round(totalHoursDouble);
                return totalHours;
            },
            "Nie udało się obliczyć liczby godzin dyżurów.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<SpecializationStatistics> GetSpecializationStatisticsAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return new SpecializationStatistics();
                }
                var stats = await ProgressCalculator.CalculateFullStatisticsAsync(
                    this.databaseService,
                    specialization.SpecializationId,
                    moduleId);

                if (stats == null)
                {
                    return new SpecializationStatistics();
                }
                return stats;
            },
            "Nie udało się pobrać statystyk specjalizacji.",
            new Dictionary<string, object> { { "ModuleId", moduleId } },
            withRetry: true);
        }

        public async Task UpdateModuleProgressAsync(int moduleId)
        {
            await SafeExecuteAsync(async () =>
            {
                await ProgressCalculator.UpdateModuleProgressAsync(this.databaseService, moduleId);
            },
            "Nie udało się zaktualizować postępu modułu.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            return await SafeExecuteAsync(async () =>
            {
                return await this.databaseService.GetModulesAsync(specializationId);
            },
            "Nie udało się pobrać modułów specjalizacji.",
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            withRetry: true);
        }

        public async Task<bool> InitializeSpecializationModulesAsync(int specializationId)
        {
            return await SafeExecuteAsync(async () =>
            {
                return await this.moduleInitializer.InitializeModulesIfNeededAsync(specializationId);
            },
            "Nie udało się zainicjować modułów specjalizacji.",
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            withRetry: true);
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                return await this.databaseService.GetMedicalShiftsAsync(internshipId);
            },
            "Nie udało się pobrać dyżurów medycznych.",
            new Dictionary<string, object> { { "InternshipId", internshipId } });
        }

        public async Task<bool> AddMedicalShiftAsync(MedicalShift shift)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "Shift cannot be null",
                        "Dane dyżuru nie mogą być puste.");
                }

                int result = await this.databaseService.SaveMedicalShiftAsync(shift);

                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się dodać dyżuru medycznego.",
            new Dictionary<string, object> {
                { "ShiftId", shift?.ShiftId },
                { "InternshipId", shift?.InternshipId }
            });
        }

        public async Task<bool> UpdateMedicalShiftAsync(MedicalShift shift)
        {
            return await AddMedicalShiftAsync(shift);
        }

        public async Task<bool> DeleteMedicalShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (shift == null)
                {
                    return false;
                }

                int result = await this.databaseService.DeleteMedicalShiftAsync(shift);
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się usunąć dyżuru medycznego.",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var results = new List<Internship>();
                var currentSpecialization = await this.GetCurrentSpecializationAsync();

                if (currentSpecialization == null)
                {
                    return results;
                }

                if (!moduleId.HasValue && currentSpecialization.CurrentModuleId.HasValue)
                {
                    moduleId = currentSpecialization.CurrentModuleId.Value;
                }

                Module module = null;
                if (moduleId.HasValue)
                {
                    module = await this.databaseService.GetModuleAsync(moduleId.Value);
                }
                else
                {
                    var modules = await this.databaseService.GetModulesAsync(currentSpecialization.SpecializationId);
                    if (modules.Count > 0)
                    {
                        module = modules[0];
                    }
                }

                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    return results;
                }

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                ModuleStructure moduleStructure = null;

                moduleStructure = System.Text.Json.JsonSerializer.Deserialize<ModuleStructure>(
                    module.Structure, options);

                if (moduleStructure?.Internships == null)
                {
                    return results;
                }

                var userInternships = await this.databaseService.GetInternshipsAsync(
                    currentSpecialization.SpecializationId,
                    moduleId);

                int id = 1;
                foreach (var requirement in moduleStructure.Internships)
                {
                    var existingInternship = userInternships.FirstOrDefault(
                        i => i.InternshipName == requirement.Name);

                    if (existingInternship != null)
                    {
                        results.Add(existingInternship);
                    }
                    else
                    {
                        results.Add(new Internship
                        {
                            InternshipId = id,
                            SpecializationId = currentSpecialization.SpecializationId,
                            ModuleId = moduleId,
                            InternshipName = requirement.Name,
                            DaysCount = requirement.WorkingDays,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(requirement.WorkingDays),
                            Year = 1,
                            IsCompleted = false,
                            IsApproved = false
                        });
                        id++;
                    }
                }

                return results;
            },
            "Nie udało się pobrać listy staży.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public event EventHandler<int> CurrentModuleChanged;

        public async Task SetCurrentModuleAsync(int moduleId)
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Current specialization not found",
                        "Nie znaleziono aktualnej specjalizacji.");
                }

                var module = await this.databaseService.GetModuleAsync(moduleId);
                if (module == null || module.SpecializationId != specialization.SpecializationId)
                {
                    throw new ResourceNotFoundException(
                        $"Module with ID {moduleId} not found or belongs to different specialization",
                        "Nie znaleziono modułu o podanym ID lub należy on do innej specjalizacji.");
                }

                specialization.CurrentModuleId = moduleId;
                await this.databaseService.UpdateSpecializationAsync(specialization);
                await SettingsHelper.SetCurrentModuleIdAsync(moduleId);

                this.CurrentModuleChanged?.Invoke(this, moduleId);
            },
            "Nie udało się ustawić aktualnego modułu.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? moduleId = null, int? internshipRequirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return new List<RealizedInternshipNewSMK>();
                }

                return await this.databaseService.GetRealizedInternshipsNewSMKAsync(
                    currentSpecialization.SpecializationId,
                    moduleId,
                    internshipRequirementId);
            },
            "Nie udało się pobrać listy zrealizowanych staży (Nowy SMK).",
            new Dictionary<string, object> {
                { "ModuleId", moduleId },
                { "InternshipRequirementId", internshipRequirementId }
            });
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? year = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return new List<RealizedInternshipOldSMK>();
                }

                string query = "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ?";

                if (year.HasValue)
                {
                    // Uwzględnij również realizacje z Year=0 (nieprzypisane do konkretnego roku)
                    query += " AND (Year = ? OR Year = 0)";
                    return await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                        query, currentSpecialization.SpecializationId, year.Value);
                }
                else
                {
                    return await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                        query, currentSpecialization.SpecializationId);
                }
            },
            "Nie udało się pobrać listy zrealizowanych staży (Stary SMK).",
            new Dictionary<string, object> { { "Year", year } });
        }

        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }
                return internship;
            },
            "Nie udało się pobrać zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }
                return internship;
            },
            "Nie udało się pobrać zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<bool> AddRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                if (internship.SpecializationId <= 0)
                {
                    var currentSpecialization = await this.GetCurrentSpecializationAsync();
                    if (currentSpecialization == null)
                    {
                        return false;
                    }

                    internship.SpecializationId = currentSpecialization.SpecializationId;
                }

                int result = await this.databaseService.SaveRealizedInternshipNewSMKAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się dodać zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "ModuleId", internship?.ModuleId }
            });
        }

        public async Task<bool> AddRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                if (internship.SpecializationId <= 0)
                {
                    var currentSpecialization = await this.GetCurrentSpecializationAsync();
                    if (currentSpecialization == null)
                    {
                        return false;
                    }

                    internship.SpecializationId = currentSpecialization.SpecializationId;
                }

                int result = await this.databaseService.SaveRealizedInternshipOldSMKAsync(internship);

                // Aktualizacja postępu modułu na podstawie roku stażu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się dodać zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "Year", internship?.Year }
            });
        }

        public async Task<bool> UpdateRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                int result = await this.databaseService.SaveRealizedInternshipNewSMKAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się zaktualizować zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "ModuleId", internship?.ModuleId }
            });
        }

        public async Task<bool> UpdateRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                int result = await this.databaseService.SaveRealizedInternshipOldSMKAsync(internship);

                // Aktualizacja postępu modułu na podstawie roku stażu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się zaktualizować zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "Year", internship?.Year }
            });
        }

        public async Task<bool> DeleteRealizedInternshipNewSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }

                int result = await this.databaseService.DeleteRealizedInternshipNewSMKAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się usunąć zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<bool> DeleteRealizedInternshipOldSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }

                int result = await this.databaseService.DeleteRealizedInternshipOldSMKAsync(internship);

                // Aktualizacja postępu modułu na podstawie roku stażu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się usunąć zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }
    }
}
```

## File: Services/Storage/ISecureStorageService.cs
```csharp
namespace SledzSpecke.App.Services.Storage
{
    public interface ISecureStorageService
    {
        Task<string> GetAsync(string key);

        Task SetAsync(string key, string value);

        void Remove(string key);

        void RemoveAll();
    }
}
```

## File: Services/Storage/SecureStorageService.cs
```csharp
namespace SledzSpecke.App.Services.Storage
{

    public class SecureStorageService : ISecureStorageService
    {
        public async Task<string> GetAsync(string key)
        {
            return await SecureStorage.GetAsync(key);
        }

        public async Task SetAsync(string key, string value)
        {
            await SecureStorage.SetAsync(key, value);
        }

        public void Remove(string key)
        {
            SecureStorage.Remove(key);
        }

        public void RemoveAll()
        {
            SecureStorage.RemoveAll();
        }
    }
}
```

## File: Services/BaseService.cs
```csharp
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Services
{
    public abstract class BaseService
    {
        protected readonly IExceptionHandlerService ExceptionHandler;
        protected readonly ILoggingService Logger;

        protected BaseService(IExceptionHandlerService exceptionHandler, ILoggingService logger)
        {
            ExceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<T> SafeExecuteAsync<T>(
            Func<Task<T>> operation,
            string userFriendlyMessage = null,
            Dictionary<string, object> context = null,
            bool withRetry = false,
            int retryCount = 3,
            int delayMilliseconds = 500)
        {
            if (withRetry)
            {
                return await ExceptionHandler.ExecuteWithRetryAsync(
                    operation, context, userFriendlyMessage, retryCount, delayMilliseconds);
            }
            else
            {
                return await ExceptionHandler.ExecuteAsync(
                    operation, context, userFriendlyMessage);
            }
        }

        protected async Task SafeExecuteAsync(
            Func<Task> operation,
            string userFriendlyMessage = null,
            Dictionary<string, object> context = null,
            bool withRetry = false,
            int retryCount = 3,
            int delayMilliseconds = 500)
        {
            if (withRetry)
            {
                await ExceptionHandler.ExecuteWithRetryAsync(
                    operation, context, userFriendlyMessage, retryCount, delayMilliseconds);
            }
            else
            {
                await ExceptionHandler.ExecuteAsync(
                    operation, context, userFriendlyMessage);
            }
        }
    }
}
```

## File: ViewModels/Authentication/LoginViewModel.cs
```csharp
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;

        private string username;
        private string password;
        private bool rememberMe;

        public LoginViewModel(
            IAuthService authService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.authService = authService;
            this.dialogService = dialogService;
            this.Title = "Logowanie";
            this.LoginCommand = new AsyncRelayCommand(this.OnLoginAsync, this.CanLogin);
            this.GoToRegisterCommand = new AsyncRelayCommand(this.OnGoToRegisterAsync);
        }

        public string Username
        {
            get => this.username;
            set
            {
                if (this.SetProperty(ref this.username, value))
                {
                    ((AsyncRelayCommand)this.LoginCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => this.password;
            set
            {
                if (this.SetProperty(ref this.password, value))
                {
                    ((AsyncRelayCommand)this.LoginCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool RememberMe
        {
            get => this.rememberMe;
            set => this.SetProperty(ref this.rememberMe, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(this.Username) &&
                   !string.IsNullOrWhiteSpace(this.Password);
        }

        private async Task OnLoginAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Walidacja danych wejściowych
                if (string.IsNullOrWhiteSpace(Username))
                {
                    throw new InvalidInputException(
                        "Username is required",
                        "Nazwa użytkownika jest wymagana.");
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    throw new InvalidInputException(
                        "Password is required",
                        "Hasło jest wymagane.");
                }

                // Próba logowania z ponawianiem - używamy nowej metody z ponawianiem
                // dla operacji sieciowych lub bazodanowych
                bool success = await SafeExecuteWithRetryAsync(
                    async () => await this.authService.LoginAsync(this.Username, this.Password),
                    "Próba logowania zakończyła się niepowodzeniem. Sprawdź dane logowania i spróbuj ponownie."
                );

                if (success)
                {
                    var appShell = IPlatformApplication.Current.Services.GetService<AppShell>();
                    if (appShell != null)
                    {
                        Application.Current.MainPage = appShell;
                    }
                    else
                    {
                        Application.Current.MainPage = new AppShell(this.authService);
                    }
                }
                else
                {
                    // To będzie obsłużone przez ExceptionHandler, jeśli logowanie nie powiedzie się z wyjątkiem,
                    // ale obsługujemy wynik false jawnie
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd logowania",
                        "Nieprawidłowa nazwa użytkownika lub hasło.",
                        "OK");
                }
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnGoToRegisterAsync()
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                if (Application.Current.MainPage is NavigationPage navigationPage)
                {
                    var registerViewModel = IPlatformApplication.Current.Services.GetRequiredService<RegisterViewModel>();
                    var registerPage = new Views.Authentication.RegisterPage(registerViewModel);
                    await navigationPage.PushAsync(registerPage);
                }
            }, "Nie można przejść do ekranu rejestracji.");

            this.IsBusy = false;
        }
    }
}
```

## File: ViewModels/Authentication/RegisterViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Authentication
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private string username;
        private string name;
        private string password;
        private string confirmPassword;
        private string email;
        private ObservableCollection<SpecializationProgram> availableSpecializations;
        private SpecializationProgram selectedSpecialization;
        private bool isOldSmkVersion;
        private bool isNewSmkVersion = true;
        private bool passwordsNotMatch;

        public RegisterViewModel(
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;
            this.Title = "Rejestracja";
            this.AvailableSpecializations = new ObservableCollection<SpecializationProgram>();
            this.RegisterCommand = new AsyncRelayCommand(this.OnRegisterAsync, this.CanRegister);
            this.GoToLoginCommand = new AsyncRelayCommand(OnGoToLoginAsync);
        }

        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                await this.LoadSpecializationsAsync();
            }, "Nie udało się załadować dostępnych specjalizacji.");

            this.IsBusy = false;
        }

        public string Username
        {
            get => this.username;
            set
            {
                if (this.SetProperty(ref this.username, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string Name
        {
            get => this.name;
            set
            {
                if (this.SetProperty(ref this.name, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => this.password;
            set
            {
                if (this.SetProperty(ref this.password, value))
                {
                    this.ValidatePasswords();
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string ConfirmPassword
        {
            get => this.confirmPassword;
            set
            {
                if (this.SetProperty(ref this.confirmPassword, value))
                {
                    this.ValidatePasswords();
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string Email
        {
            get => this.email;
            set
            {
                if (this.SetProperty(ref this.email, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<SpecializationProgram> AvailableSpecializations
        {
            get => this.availableSpecializations;
            set => this.SetProperty(ref this.availableSpecializations, value);
        }

        public SpecializationProgram SelectedSpecialization
        {
            get => this.selectedSpecialization;
            set
            {
                if (this.SetProperty(ref this.selectedSpecialization, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsNewSmkVersion
        {
            get => this.isNewSmkVersion;
            set
            {
                if (this.SetProperty(ref this.isNewSmkVersion, value) && value)
                {
                    this.IsOldSmkVersion = false;
                    this.LoadSpecializationsAsync().ConfigureAwait(false);

                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set
            {
                if (this.SetProperty(ref this.isOldSmkVersion, value) && value)
                {
                    this.IsNewSmkVersion = false;
                    this.LoadSpecializationsAsync().ConfigureAwait(false);

                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool PasswordsNotMatch
        {
            get => this.passwordsNotMatch;
            set => this.SetProperty(ref this.passwordsNotMatch, value);
        }

        public ICommand RegisterCommand { get; }

        public ICommand GoToLoginCommand { get; }

        private async Task LoadSpecializationsAsync()
        {
            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                SmkVersion currentVersion = this.IsNewSmkVersion ? SmkVersion.New : SmkVersion.Old;
                var programs = await SpecializationLoader.LoadAllSpecializationProgramsForVersionAsync(currentVersion);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.AvailableSpecializations.Clear();
                    foreach (var program in programs)
                    {
                        this.AvailableSpecializations.Add(program);
                    }

                    if (this.AvailableSpecializations.Count > 0)
                    {
                        this.SelectedSpecialization = this.AvailableSpecializations[0];
                    }
                    else
                    {
                        this.SelectedSpecialization = null;
                    }
                });
            }, "Nie udało się załadować dostępnych specjalizacji.");

            this.IsBusy = false;
        }

        private void ValidatePasswords()
        {
            bool passwordsMatch = !string.IsNullOrEmpty(this.Password) &&
                                 !string.IsNullOrEmpty(this.ConfirmPassword) &&
                                 this.Password == this.ConfirmPassword;
            this.PasswordsNotMatch = !passwordsMatch;
            ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
        }

        private bool CanRegister()
        {
            bool isUsernameValid = !string.IsNullOrWhiteSpace(this.Username);
            bool isNameValid = !string.IsNullOrWhiteSpace(this.Name);
            bool isPasswordValid = !string.IsNullOrWhiteSpace(this.Password);
            bool isConfirmPasswordValid = !string.IsNullOrWhiteSpace(this.ConfirmPassword);
            bool isEmailValid = !string.IsNullOrWhiteSpace(this.Email);
            bool arePasswordsMatching = !this.PasswordsNotMatch;
            bool isSpecializationSelected = this.SelectedSpecialization != null;
            bool isSmkVersionSelected = this.IsOldSmkVersion || this.IsNewSmkVersion;

            return isUsernameValid && isNameValid && isPasswordValid && isConfirmPasswordValid &&
                   isEmailValid && arePasswordsMatching && isSpecializationSelected &&
                   isSmkVersionSelected;
        }

        private async Task OnRegisterAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Input validation with domain exceptions
                if (string.IsNullOrWhiteSpace(this.Username))
                {
                    throw new InvalidInputException(
                        "Username is required",
                        "Nazwa użytkownika jest wymagana.");
                }

                if (string.IsNullOrWhiteSpace(this.Name))
                {
                    throw new InvalidInputException(
                        "Name is required",
                        "Imię i nazwisko jest wymagane.");
                }

                if (string.IsNullOrWhiteSpace(this.Password))
                {
                    throw new InvalidInputException(
                        "Password is required",
                        "Hasło jest wymagane.");
                }

                if (string.IsNullOrWhiteSpace(this.Email))
                {
                    throw new InvalidInputException(
                        "Email is required",
                        "Adres email jest wymagany.");
                }

                if (this.PasswordsNotMatch)
                {
                    throw new InvalidInputException(
                        "Passwords do not match",
                        "Hasła nie są identyczne.");
                }

                if (this.SelectedSpecialization == null)
                {
                    throw new InvalidInputException(
                        "No specialization selected",
                        "Nie wybrano specjalizacji. Wybierz specjalizację przed rejestracją.");
                }

                await SafeExecuteAsync(async () =>
                {
                    var user = new User
                    {
                        Username = this.Username,
                        Name = this.Name,
                        Email = this.Email,
                        RegistrationDate = DateTime.Now,
                        SmkVersion = this.IsNewSmkVersion ? SmkVersion.New : SmkVersion.Old,
                    };

                    if (string.IsNullOrEmpty(this.SelectedSpecialization.Structure))
                    {
                        var fullSpecialization = await SpecializationLoader.LoadSpecializationProgramAsync(
                            this.SelectedSpecialization.Code,
                            user.SmkVersion);

                        if (fullSpecialization != null && !string.IsNullOrEmpty(fullSpecialization.Structure))
                        {
                            this.SelectedSpecialization.Structure = fullSpecialization.Structure;
                        }
                        else
                        {
                            this.SelectedSpecialization.Structure = $"{{ \"name\": \"{this.SelectedSpecialization.Name}\", \"code\": \"{this.SelectedSpecialization.Code}\" }}";
                        }
                    }

                    var specialization = new Models.Specialization
                    {
                        Name = this.SelectedSpecialization.Name,
                        ProgramCode = this.SelectedSpecialization.Code,
                        SmkVersion = user.SmkVersion,
                        StartDate = DateTime.Now,
                        PlannedEndDate = DateTime.Now.AddMonths(
                            this.SelectedSpecialization.TotalDuration != null && this.SelectedSpecialization.TotalDuration.TotalMonths > 0
                                ? this.SelectedSpecialization.TotalDuration.TotalMonths
                                : 60),
                        ProgramStructure = this.SelectedSpecialization.Structure,
                        DurationYears = this.SelectedSpecialization.DurationYears,
                    };

                    var modules = await ModuleHelper.CreateModulesForSpecializationAsync(
                        specialization.ProgramCode,
                        specialization.StartDate,
                        user.SmkVersion,
                        specialization.SpecializationId);

                    if (modules != null && modules.Count > 0)
                    {
                        specialization.Modules = modules;
                    }
                    else
                    {
                        specialization.Modules = new List<Models.Module>();
                    }

                    specialization.CalculatedEndDate = specialization.PlannedEndDate;
                    bool success = await this.authService.RegisterAsync(user, this.Password, specialization);

                    if (success)
                    {
                        await this.specializationService.InitializeSpecializationModulesAsync(specialization.SpecializationId);
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            "Rejestracja zakończona pomyślnie. Zaloguj się, aby rozpocząć korzystanie z aplikacji.",
                            "OK");

                        await OnGoToLoginAsync();
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Registration failed",
                            "Nie udało się zarejestrować. Sprawdź podane dane i spróbuj ponownie.");
                    }
                }, "Wystąpił problem podczas rejestracji. Sprawdź podane dane i spróbuj ponownie.");
            }
            catch (InvalidInputException)
            {
                // These will be handled by the SafeExecuteAsync method
                throw;
            }
            catch (Exception ex)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił nieoczekiwany błąd podczas rejestracji.",
                    "OK");
                System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private static async Task OnGoToLoginAsync()
        {
            if (Application.Current?.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.PopAsync();
            }
            else
            {
                var loginViewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Authentication.LoginViewModel>();
                if (loginViewModel != null)
                {
                    var loginPage = new SledzSpecke.App.Views.Authentication.LoginPage(loginViewModel);
                    Application.Current.MainPage = new NavigationPage(loginPage);
                }
            }
        }
    }
}
```

## File: ViewModels/Base/BaseViewModel.cs
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using SledzSpecke.App.Services.Exceptions;

namespace SledzSpecke.App.ViewModels.Base
{
    public class BaseViewModel : ObservableObject
    {
        private bool isBusy;
        private string title;

        protected readonly IExceptionHandlerService ExceptionHandler;

        public BaseViewModel(IExceptionHandlerService exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler;
        }

        public bool IsBusy
        {
            get => this.isBusy;
            set => this.SetProperty(ref this.isBusy, value);
        }

        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację z automatyczną obsługą błędów
        /// </summary>
        protected async Task SafeExecuteAsync(Func<Task> operation, string userFriendlyMessage = null)
        {
            if (ExceptionHandler != null)
            {
                await ExceptionHandler.ExecuteAsync(operation, null, userFriendlyMessage);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteAsync)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację zwracającą wartość z automatyczną obsługą błędów
        /// </summary>
        protected async Task<T> SafeExecuteAsync<T>(Func<Task<T>> operation, string userFriendlyMessage = null)
        {
            if (ExceptionHandler != null)
            {
                return await ExceptionHandler.ExecuteAsync(operation, null, userFriendlyMessage);
            }
            else
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteAsync)}: {ex.Message}");
                    return default;
                }
            }
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację z ponawianiem i automatyczną obsługą błędów
        /// </summary>
        protected async Task SafeExecuteWithRetryAsync(Func<Task> operation, string userFriendlyMessage = null,
                                                    int retryCount = 3, int delayMilliseconds = 500)
        {
            if (ExceptionHandler != null)
            {
                await ExceptionHandler.ExecuteWithRetryAsync(operation, null, userFriendlyMessage,
                                                           retryCount, delayMilliseconds);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteWithRetryAsync)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację zwracającą wartość z ponawianiem i automatyczną obsługą błędów
        /// </summary>
        protected async Task<T> SafeExecuteWithRetryAsync<T>(Func<Task<T>> operation, string userFriendlyMessage = null,
                                                          int retryCount = 3, int delayMilliseconds = 500)
        {
            if (ExceptionHandler != null)
            {
                return await ExceptionHandler.ExecuteWithRetryAsync(operation, null, userFriendlyMessage,
                                                                  retryCount, delayMilliseconds);
            }
            else
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteWithRetryAsync)}: {ex.Message}");
                    return default;
                }
            }
        }
    }
}
```

## File: ViewModels/Dashboard/DashboardViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel, IDisposable
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;
        private readonly IProcedureService procedureService;

        private int currentModuleId;
        private Models.Specialization currentSpecialization;
        private Module currentModule;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasTwoModules;
        private ObservableCollection<ModuleInfo> availableModules;
        private double overallProgress;
        private double internshipProgress;
        private double courseProgress;
        private double procedureProgress;
        private double shiftProgress;
        private string internshipCount;
        private string procedureCount;
        private string courseCount;
        private string shiftStats;
        private int selfEducationCount;
        private int publicationCount;
        private string moduleTitle;
        private string specializationInfo;
        private string dateRangeInfo;
        private string progressText;

        public DashboardViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService,
            IProcedureService procedureService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;
            this.procedureService = procedureService;

            this.AvailableModules = new ObservableCollection<ModuleInfo>();

            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.NavigateToInternshipsCommand = new AsyncRelayCommand(NavigateToInternshipsAsync);
            this.NavigateToProceduresCommand = new AsyncRelayCommand(NavigateToProceduresAsync);
            this.NavigateToShiftsCommand = new AsyncRelayCommand(NavigateToShiftsAsync);
            this.NavigateToCoursesCommand = new AsyncRelayCommand(NavigateToCoursesAsync);
            this.NavigateToSelfEducationCommand = new AsyncRelayCommand(NavigateToSelfEducationAsync);
            this.NavigateToPublicationsCommand = new AsyncRelayCommand(NavigateToPublicationsAsync);
            this.NavigateToAbsencesCommand = new AsyncRelayCommand(NavigateToAbsencesAsync);
            this.NavigateToStatisticsCommand = new AsyncRelayCommand(NavigateToStatisticsAsync);
            this.NavigateToExportCommand = new AsyncRelayCommand(NavigateToExportAsync);
            this.NavigateToRecognitionsCommand = new AsyncRelayCommand(NavigateToRecognitionsAsync);

            this.specializationService.CurrentModuleChanged += this.OnModuleChanged;

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                if (this.SetProperty(ref this.currentModuleId, value))
                {
                    this.LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public Models.Specialization CurrentSpecialization
        {
            get => this.currentSpecialization;
            set => this.SetProperty(ref this.currentSpecialization, value);
        }

        public Module CurrentModule
        {
            get => this.currentModule;
            set => this.SetProperty(ref this.currentModule, value);
        }

        public ObservableCollection<ModuleInfo> AvailableModules
        {
            get => this.availableModules;
            set => this.SetProperty(ref this.availableModules, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public double OverallProgress
        {
            get => this.overallProgress;
            set => this.SetProperty(ref this.overallProgress, value);
        }

        public double InternshipProgress
        {
            get => this.internshipProgress;
            set => this.SetProperty(ref this.internshipProgress, value);
        }

        public double CourseProgress
        {
            get => this.courseProgress;
            set => this.SetProperty(ref this.courseProgress, value);
        }

        public double ProcedureProgress
        {
            get => this.procedureProgress;
            set => this.SetProperty(ref this.procedureProgress, value);
        }

        public double ShiftProgress
        {
            get => this.shiftProgress;
            set => this.SetProperty(ref this.shiftProgress, value);
        }

        public string InternshipCount
        {
            get => this.internshipCount;
            set => this.SetProperty(ref this.internshipCount, value);
        }

        public string ProcedureCount
        {
            get => this.procedureCount;
            set => this.SetProperty(ref this.procedureCount, value);
        }

        public string CourseCount
        {
            get => this.courseCount;
            set => this.SetProperty(ref this.courseCount, value);
        }

        public string ShiftStats
        {
            get => this.shiftStats;
            set => this.SetProperty(ref this.shiftStats, value);
        }

        public int SelfEducationCount
        {
            get => this.selfEducationCount;
            set => this.SetProperty(ref this.selfEducationCount, value);
        }

        public int PublicationCount
        {
            get => this.publicationCount;
            set => this.SetProperty(ref this.publicationCount, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public string SpecializationInfo
        {
            get => this.specializationInfo;
            set => this.SetProperty(ref this.specializationInfo, value);
        }

        public string DateRangeInfo
        {
            get => this.dateRangeInfo;
            set => this.SetProperty(ref this.dateRangeInfo, value);
        }

        public string ProgressText
        {
            get => this.progressText;
            set => this.SetProperty(ref this.progressText, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }
        public ICommand NavigateToInternshipsCommand { get; }
        public ICommand NavigateToProceduresCommand { get; }
        public ICommand NavigateToShiftsCommand { get; }
        public ICommand NavigateToCoursesCommand { get; }
        public ICommand NavigateToSelfEducationCommand { get; }
        public ICommand NavigateToPublicationsCommand { get; }
        public ICommand NavigateToAbsencesCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand NavigateToExportCommand { get; }
        public ICommand NavigateToRecognitionsCommand { get; }

        private async void OnModuleChanged(object sender, int moduleId)
        {
            this.CurrentModuleId = moduleId;

            await SafeExecuteAsync(async () =>
            {
                var module = await this.databaseService.GetModuleAsync(moduleId);
                await this.LoadDataAsync();
            }, "Wystąpił problem podczas zmiany modułu.");
        }

        public void Dispose()
        {
            this.specializationService.CurrentModuleChanged -= this.OnModuleChanged;
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                this.CurrentSpecialization = await this.specializationService.GetCurrentSpecializationAsync();

                if (this.CurrentSpecialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji. Proszę skontaktować się z administratorem.");
                }

                this.HasTwoModules = this.CurrentSpecialization.Modules.Any(x => x.Type == ModuleType.Basic);
                await this.specializationService.InitializeSpecializationModulesAsync(this.CurrentSpecialization.SpecializationId);
                var modules = await this.databaseService.GetModulesAsync(this.CurrentSpecialization.SpecializationId);
                this.AvailableModules.Clear();

                foreach (var module in modules)
                {
                    this.AvailableModules.Add(new ModuleInfo
                    {
                        Id = module.ModuleId,
                        Name = module.Name,
                    });
                }

                if (this.CurrentModuleId == 0)
                {
                    if (this.CurrentSpecialization.CurrentModuleId.HasValue && this.CurrentSpecialization.CurrentModuleId.Value > 0)
                    {
                        this.CurrentModuleId = this.CurrentSpecialization.CurrentModuleId.Value;
                    }
                    else
                    {
                        int savedModuleId = await Helpers.SettingsHelper.GetCurrentModuleIdAsync();
                        if (savedModuleId > 0 && modules.Any(m => m.ModuleId == savedModuleId))
                        {
                            this.CurrentModuleId = savedModuleId;
                        }
                        else if (modules.Count > 0)
                        {
                            this.CurrentModuleId = modules[0].ModuleId;
                        }
                    }
                }

                if (this.CurrentModuleId > 0)
                {
                    await this.specializationService.SetCurrentModuleAsync(this.CurrentModuleId);
                    this.CurrentModule = await this.specializationService.GetCurrentModuleAsync();

                    if (this.CurrentModule != null)
                    {
                        this.BasicModuleSelected = this.CurrentModule.Type == ModuleType.Basic;
                        this.SpecialisticModuleSelected = this.CurrentModule.Type == ModuleType.Specialistic;
                    }
                }

                await this.LoadStatisticsAsync();
                this.UpdateUIText();
            }, "Wystąpił problem podczas ładowania danych. Spróbuj ponownie.");

            this.IsBusy = false;
        }

        private async Task LoadStatisticsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                int? moduleId = this.CurrentModuleId;
                this.OverallProgress = await Helpers.ProgressCalculator.GetOverallProgressAsync(
                    this.databaseService,
                    this.CurrentSpecialization.SpecializationId,
                    moduleId);
                int completedInternships = await this.specializationService.GetInternshipCountAsync(moduleId);
                int totalInternships = 0;

                if (this.CurrentModule != null)
                {
                    totalInternships = this.CurrentModule.TotalInternships;
                }

                this.InternshipCount = $"{completedInternships}/{totalInternships}";
                this.InternshipProgress = totalInternships > 0
                    ? (double)completedInternships / totalInternships
                    : 0;
                var procedureStats = await this.procedureService.GetProcedureStatisticsForModuleAsync(this.CurrentModuleId);
                int completedProcedures = procedureStats.completed;
                int totalProcedures = procedureStats.total;

                this.ProcedureCount = $"{completedProcedures}/{totalProcedures}";
                this.ProcedureProgress = totalProcedures > 0
                    ? (double)completedProcedures / totalProcedures
                    : 0;

                int totalCourses = 0;

                if (this.CurrentModule != null)
                {
                    totalCourses = this.CurrentModule.TotalCourses;
                }

                int completedShiftHours = await this.specializationService.GetShiftCountAsync(moduleId);
                SpecializationStatistics stats = await this.specializationService.GetSpecializationStatisticsAsync(moduleId);

                if (stats.RequiredShiftHours > 0)
                {
                    this.ShiftStats = $"{completedShiftHours}/{stats.RequiredShiftHours}h";
                    this.ShiftProgress = Math.Min(1.0, (double)completedShiftHours / stats.RequiredShiftHours);
                }
                else
                {
                    this.ShiftStats = $"{completedShiftHours}h";
                    this.ShiftProgress = 0;
                }

                if (this.CurrentModule != null)
                {
                    double internshipWeight = 0.35;
                    double courseWeight = 0.25;
                    double procedureWeight = 0.30;
                    double otherWeight = 0.10;

                    this.OverallProgress =
                        (this.InternshipProgress * internshipWeight) +
                        (this.CourseProgress * courseWeight) +
                        (this.ProcedureProgress * procedureWeight) +
                        (this.ShiftProgress * otherWeight);

                    this.OverallProgress = Math.Min(1.0, this.OverallProgress);
                }
            }, "Wystąpił problem podczas obliczania statystyk.");
        }

        private void UpdateUIText()
        {
            if (this.CurrentSpecialization == null)
            {
                return;
            }

            if (this.CurrentModule != null)
            {
                this.ModuleTitle = this.CurrentModule.Name;
            }
            else
            {
                this.ModuleTitle = this.CurrentSpecialization.Name;
            }

            this.SpecializationInfo = $"{this.CurrentSpecialization.Name}";
            string startDate = this.CurrentSpecialization.StartDate.ToString("dd-MM-yyyy");
            string endDate = this.CurrentSpecialization.CalculatedEndDate.ToString("dd-MM-yyyy");
            this.DateRangeInfo = $"{startDate} - {endDate}";
            int progressPercent = (int)(this.OverallProgress * 100);
            this.ProgressText = $"Ukończono {progressPercent}%";
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            if (this.CurrentSpecialization == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                var modules = await this.databaseService.GetModulesAsync(this.CurrentSpecialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        this.CurrentModuleId = basicModule.ModuleId;
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        this.CurrentModuleId = specialisticModule.ModuleId;
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                }

                await Helpers.SettingsHelper.SetCurrentModuleIdAsync(this.CurrentModuleId);

                if (this.CurrentSpecialization.CurrentModuleId != this.CurrentModuleId)
                {
                    this.CurrentSpecialization.CurrentModuleId = this.CurrentModuleId;
                    await this.databaseService.UpdateSpecializationAsync(this.CurrentSpecialization);
                }

                await this.LoadDataAsync();
            }, "Nie udało się przełączyć modułu. Spróbuj ponownie.");
        }

        private static async Task NavigateToInternshipsAsync()
        {
            await Shell.Current.GoToAsync("/internships");
        }

        private static async Task NavigateToProceduresAsync()
        {
            await Shell.Current.GoToAsync("/ProcedureSelector");
        }

        private static async Task NavigateToShiftsAsync()
        {
            await Shell.Current.GoToAsync("///medicalshifts");
        }

        private static async Task NavigateToCoursesAsync()
        {
            await Shell.Current.GoToAsync("courses");
        }

        private static async Task NavigateToSelfEducationAsync()
        {
            await Shell.Current.GoToAsync("selfeducation");
        }

        private static async Task NavigateToPublicationsAsync()
        {
            await Shell.Current.GoToAsync("publications");
        }

        private static async Task NavigateToAbsencesAsync()
        {
            await Shell.Current.GoToAsync("absences");
        }

        private static async Task NavigateToStatisticsAsync()
        {
            await Shell.Current.GoToAsync("statistics");
        }

        private static async Task NavigateToExportAsync()
        {
            await Shell.Current.GoToAsync("export");
        }

        private async Task NavigateToRecognitionsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                if (!this.SpecialisticModuleSelected)
                {
                    throw new BusinessRuleViolationException(
                        "Recognitions only available in specialist module",
                        "Uznania i skrócenia są dostępne tylko dla modułu specjalistycznego. Przełącz się na moduł specjalistyczny, aby uzyskać dostęp do tej funkcji.");
                }

                await Shell.Current.GoToAsync("Recognitions");
            }, "Wystąpił problem podczas przechodzenia do uznań i skróceń.");
        }
    }
}
```

## File: ViewModels/Internships/AddEditRealizedInternshipViewModel.cs
```csharp
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    [QueryProperty(nameof(RealizedInternshipId), nameof(RealizedInternshipId))]
    [QueryProperty(nameof(InternshipName), nameof(InternshipName))]
    [QueryProperty(nameof(DaysCount), nameof(DaysCount))]
    [QueryProperty(nameof(ModuleId), nameof(ModuleId))]
    [QueryProperty(nameof(Year), nameof(Year))]
    public class AddEditRealizedInternshipViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;

        private bool isEdit;
        private bool isNewSMK;
        private RealizedInternshipNewSMK newSMKInternship;
        private RealizedInternshipOldSMK oldSMKInternship;
        private int internshipRequirementId;
        private string internshipName;
        private int daysCount;
        private int moduleId;
        private int year;

        public AddEditRealizedInternshipViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;

            this.newSMKInternship = new RealizedInternshipNewSMK
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DaysCount = 30,
                SyncStatus = SyncStatus.NotSynced
            };

            this.oldSMKInternship = new RealizedInternshipOldSMK
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DaysCount = 30,
                Year = 1,
                SyncStatus = SyncStatus.NotSynced
            };

            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);

            // Check SMK version
            this.CheckSMKVersionAsync().ConfigureAwait(false);
        }

        public string RealizedInternshipId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.LoadRealizedInternshipAsync(id).ConfigureAwait(false);
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj realizację stażu";
                }
            }
        }

        public string InternshipName
        {
            set
            {
                this.internshipName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (this.IsNewSMK)
                    {
                        this.NewSMKInternship.InternshipName = value;
                    }
                    else
                    {
                        this.OldSMKInternship.InternshipName = value;
                    }
                }
            }
        }

        public string DaysCount
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int days))
                {
                    this.daysCount = days;
                    if (this.IsNewSMK)
                    {
                        this.NewSMKInternship.DaysCount = days;
                        this.NewSMKInternship.EndDate = this.NewSMKInternship.StartDate.AddDays(days - 1);
                    }
                    else
                    {
                        this.OldSMKInternship.DaysCount = days;
                        this.OldSMKInternship.EndDate = this.OldSMKInternship.StartDate.AddDays(days - 1);
                    }
                }
            }
        }

        public string ModuleId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.moduleId = id;
                    this.newSMKInternship.ModuleId = id;
                }
            }
        }

        public string Year
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int yearVal) && yearVal > 0)
                {
                    this.year = yearVal;
                    this.oldSMKInternship.Year = yearVal;
                }
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public bool IsNewSMK
        {
            get => this.isNewSMK;
            set => this.SetProperty(ref this.isNewSMK, value);
        }

        public RealizedInternshipNewSMK NewSMKInternship
        {
            get => this.newSMKInternship;
            set => this.SetProperty(ref this.newSMKInternship, value);
        }

        public RealizedInternshipOldSMK OldSMKInternship
        {
            get => this.oldSMKInternship;
            set => this.SetProperty(ref this.oldSMKInternship, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task CheckSMKVersionAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono zalogowanego użytkownika.");
                }
                this.IsNewSMK = user.SmkVersion == SmkVersion.New;
            }, "Wystąpił problem z identyfikacją wersji SMK.");
        }

        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                // Additional initialization code if needed
            }, "Nie udało się zainicjalizować ekranu.");

            this.IsBusy = false;
        }

        private async Task LoadRealizedInternshipAsync(int realizedInternshipId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                if (this.IsNewSMK)
                {
                    var loadedInternship = await this.specializationService.GetRealizedInternshipNewSMKAsync(realizedInternshipId);
                    if (loadedInternship != null)
                    {
                        this.IsEdit = true;
                        this.Title = "Edytuj realizację stażu";
                        this.NewSMKInternship = loadedInternship;
                    }
                    else
                    {
                        this.IsEdit = false;
                        this.Title = "Dodaj realizację stażu";
                        throw new ResourceNotFoundException(
                            $"Realized internship with ID {realizedInternshipId} not found",
                            "Nie znaleziono realizacji stażu o podanym identyfikatorze.");
                    }
                }
                else
                {
                    var loadedInternship = await this.specializationService.GetRealizedInternshipOldSMKAsync(realizedInternshipId);
                    if (loadedInternship != null)
                    {
                        this.IsEdit = true;
                        this.Title = "Edytuj realizację stażu";
                        this.OldSMKInternship = loadedInternship;
                    }
                    else
                    {
                        this.IsEdit = false;
                        this.Title = "Dodaj realizację stażu";
                        throw new ResourceNotFoundException(
                            $"Realized internship with ID {realizedInternshipId} not found",
                            "Nie znaleziono realizacji stażu o podanym identyfikatorze.");
                    }
                }
            }, "Nie udało się załadować realizacji stażu.");

            this.IsBusy = false;
        }

        private async Task SaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    bool validationPassed;

                    if (this.IsNewSMK)
                    {
                        validationPassed = await this.ValidateNewSMKInternshipAsync();
                    }
                    else
                    {
                        validationPassed = await this.ValidateOldSMKInternshipAsync();
                    }

                    if (!validationPassed)
                    {
                        throw new InvalidInputException(
                            "Validation failed",
                            "Walidacja danych nie powiodła się.");
                    }

                    bool success;

                    if (this.IsNewSMK)
                    {
                        // Calculate days
                        TimeSpan duration = this.NewSMKInternship.EndDate - this.NewSMKInternship.StartDate;
                        this.NewSMKInternship.DaysCount = duration.Days + 1;

                        if (this.IsEdit)
                        {
                            success = await this.specializationService.UpdateRealizedInternshipNewSMKAsync(this.NewSMKInternship);
                        }
                        else
                        {
                            this.NewSMKInternship.ModuleId = this.moduleId;
                            success = await this.specializationService.AddRealizedInternshipNewSMKAsync(this.NewSMKInternship);
                        }
                    }
                    else
                    {
                        // Calculate days
                        TimeSpan duration = this.OldSMKInternship.EndDate - this.OldSMKInternship.StartDate;
                        this.OldSMKInternship.DaysCount = duration.Days + 1;

                        // Make sure year is set correctly
                        if (this.OldSMKInternship.Year == 0)
                        {
                            var currentModule = await this.specializationService.GetCurrentModuleAsync();
                            if (currentModule != null)
                            {
                                this.OldSMKInternship.Year = currentModule.Type == ModuleType.Basic ? 1 : 3;
                            }
                            else
                            {
                                this.OldSMKInternship.Year = 1;
                            }
                        }

                        if (this.IsEdit)
                        {
                            success = await this.specializationService.UpdateRealizedInternshipOldSMKAsync(this.OldSMKInternship);
                        }
                        else
                        {
                            success = await this.specializationService.AddRealizedInternshipOldSMKAsync(this.OldSMKInternship);
                        }
                    }

                    if (success)
                    {
                        await this.dialogService.DisplayAlertAsync(
                           "Sukces",
                           this.IsEdit ? "Realizacja stażu została zaktualizowana." : "Realizacja stażu została dodana.",
                           "OK");

                        if (this.IsNewSMK)
                        {
                            await Shell.Current.GoToAsync("/NewSMKInternships");
                        }
                        else
                        {
                            await Shell.Current.GoToAsync("/OldSMKInternships");
                        }
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Saving internship failed",
                            "Nie udało się zapisać realizacji stażu. Sprawdź poprawność danych.");
                    }
                }, "Wystąpił problem podczas zapisywania realizacji stażu.");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task<bool> ValidateNewSMKInternshipAsync()
        {
            if (string.IsNullOrWhiteSpace(this.NewSMKInternship.InstitutionName))
            {
                throw new InvalidInputException(
                    "Institution name is required",
                    "Nazwa placówki realizującej szkolenie jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.NewSMKInternship.DepartmentName))
            {
                throw new InvalidInputException(
                    "Department name is required",
                    "Nazwa komórki organizacyjnej jest wymagana.");
            }

            if (this.NewSMKInternship.DaysCount <= 0)
            {
                throw new InvalidInputException(
                    "Days count must be greater than zero",
                    "Liczba dni musi być większa od zera.");
            }

            if (this.NewSMKInternship.EndDate < this.NewSMKInternship.StartDate)
            {
                throw new InvalidInputException(
                    "End date cannot be earlier than start date",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
            }

            return true;
        }

        private async Task<bool> ValidateOldSMKInternshipAsync()
        {
            if (string.IsNullOrWhiteSpace(this.OldSMKInternship.InstitutionName))
            {
                throw new InvalidInputException(
                    "Institution name is required",
                    "Nazwa placówki realizującej szkolenie jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.OldSMKInternship.DepartmentName))
            {
                throw new InvalidInputException(
                    "Department name is required",
                    "Nazwa komórki organizacyjnej jest wymagana.");
            }

            if (this.OldSMKInternship.DaysCount <= 0)
            {
                throw new InvalidInputException(
                    "Days count must be greater than zero",
                    "Liczba dni musi być większa od zera.");
            }

            if (this.OldSMKInternship.EndDate < this.OldSMKInternship.StartDate)
            {
                throw new InvalidInputException(
                    "End date cannot be earlier than start date",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
            }

            return true;
        }

        private async Task CancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Nie udało się wykonać operacji anulowania.");
        }
    }
}
```

## File: ViewModels/Internships/InternshipRequirementViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke;
using SledzSpecke.App;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipRequirementViewModel : ObservableObject
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IDialogService dialogService;
        private readonly IExceptionHandlerService exceptionHandler;

        private InternshipRequirement requirement;
        private MedicalShiftsSummary summary;
        private ObservableCollection<RealizedMedicalShiftNewSMK> shifts;
        private bool isExpanded;
        private RealizedMedicalShiftNewSMK currentShift;
        private bool isEditing;

        public InternshipRequirementViewModel(
            InternshipRequirement requirement,
            MedicalShiftsSummary summary,
            List<RealizedMedicalShiftNewSMK> shifts,
            IMedicalShiftsService medicalShiftsService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler,
            int? currentModuleId)
        {
            this.requirement = requirement;
            this.summary = summary;
            this.medicalShiftsService = medicalShiftsService;
            this.dialogService = dialogService;
            this.exceptionHandler = exceptionHandler;

            Shifts = new ObservableCollection<RealizedMedicalShiftNewSMK>(shifts);
            currentShift = new RealizedMedicalShiftNewSMK
            {
                InternshipRequirementId = requirement.Id,
                ModuleId = currentModuleId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Hours = summary.TotalHours,
                Minutes = summary.TotalMinutes
            };
            ToggleExpandCommand = new RelayCommand(ToggleExpand);
            SaveCommand = new AsyncRelayCommand(SaveShiftAsync);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        public string Name => requirement.Name;
        public int Id => requirement.Id;
        public string FormattedTime => $"{summary.TotalHours} godz. {summary.TotalMinutes} min.";
        public string Title => $"Dyżury do stażu\n{Name}";
        public string Summary => $"Czas wprowadzony:\n{FormattedTime}";

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public RealizedMedicalShiftNewSMK CurrentShift
        {
            get => currentShift;
            set => SetProperty(ref currentShift, value);
        }

        public ObservableCollection<RealizedMedicalShiftNewSMK> Shifts
        {
            get => shifts;
            set => SetProperty(ref shifts, value);
        }

        public bool IsEditing
        {
            get => isEditing;
            set => SetProperty(ref isEditing, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
            IsEditing = IsExpanded;
        }

        private async Task SaveShiftAsync()
        {
            try
            {
                // Validate shift data
                if (CurrentShift.Hours < 0 || CurrentShift.Minutes < 0 || CurrentShift.Minutes >= 60)
                {
                    throw new InvalidInputException(
                        "Invalid shift time",
                        "Niepoprawny czas dyżuru. Godziny muszą być nieujemne, a minuty w zakresie 0-59.");
                }

                if (CurrentShift.EndDate < CurrentShift.StartDate)
                {
                    throw new InvalidInputException(
                        "End date before start date",
                        "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
                }

                // Use exception handler to safely execute the save operation
                bool success = exceptionHandler != null
                    ? await exceptionHandler.ExecuteAsync(
                        async () => await medicalShiftsService.SaveNewSMKShiftAsync(CurrentShift),
                        new Dictionary<string, object> { { "InternshipRequirementId", Id } },
                        "Nie udało się zapisać dyżuru. Sprawdź poprawność danych.")
                    : await medicalShiftsService.SaveNewSMKShiftAsync(CurrentShift);

                if (success)
                {
                    // Safely retrieve updated shifts using the exception handler
                    var updatedShifts = exceptionHandler != null
                        ? await exceptionHandler.ExecuteAsync(
                            async () => await medicalShiftsService.GetNewSMKShiftsAsync(Id),
                            null,
                            "Nie udało się pobrać zaktualizowanych danych.")
                        : await medicalShiftsService.GetNewSMKShiftsAsync(Id);

                    Shifts.Clear();
                    foreach (var shift in updatedShifts)
                    {
                        Shifts.Add(shift);
                    }

                    // Safely update summary
                    var updatedSummary = exceptionHandler != null
                        ? await exceptionHandler.ExecuteAsync(
                            async () => await medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: Id),
                            null,
                            "Nie udało się zaktualizować podsumowania.")
                        : await medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: Id);

                    if (updatedSummary != null)
                    {
                        summary = updatedSummary;
                        OnPropertyChanged(nameof(FormattedTime));
                        OnPropertyChanged(nameof(Summary));
                    }

                    IsEditing = false;
                    IsExpanded = false;
                }
                else
                {
                    await dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać dyżuru.",
                        "OK");
                }
            }
            catch (InvalidInputException ex)
            {
                // These will be caught and handled by the exception handler if available
                if (exceptionHandler != null)
                {
                    throw;
                }
                else
                {
                    await dialogService.DisplayAlertAsync("Błąd", ex.UserFriendlyMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                // Fallback error handling if exceptionHandler not available
                if (exceptionHandler == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving shift: {ex.Message}");
                    await dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Wystąpił nieoczekiwany błąd podczas zapisywania dyżuru.",
                        "OK");
                }
                else
                {
                    throw; // Let the exceptionHandler handle it
                }
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            IsExpanded = false;
        }
    }
}
```

## File: ViewModels/Internships/InternshipsSelectorViewModel.cs
```csharp
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipsSelectorViewModel : BaseViewModel
    {
        public InternshipsSelectorViewModel(IExceptionHandlerService exceptionHandler = null) : base(exceptionHandler)
        {
            this.Title = "Staże";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        private async Task NavigateToOldSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("OldSMKInternships");
            }, "Nie udało się przejść do okna staży (Stary SMK).");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("NewSMKInternships");
            }, "Nie udało się przejść do okna staży (Nowy SMK).");
        }
    }
}
```

## File: ViewModels/Internships/InternshipStageViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipStageViewModel : ObservableObject
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;
        private readonly IExceptionHandlerService exceptionHandler;

        private Internship requirement;
        private List<RealizedInternshipNewSMK> newSMKRealizations;
        private List<RealizedInternshipOldSMK> oldSMKRealizations;
        private bool isExpanded;
        private bool isNewSMK;
        private int? currentModuleId;
        private bool isLoading;
        private bool hasLoadedData;

        public ICommand EditRealizationCommand { get; }
        public ICommand DeleteRealizationCommand { get; }

        public InternshipStageViewModel(
            Internship requirement,
            List<RealizedInternshipNewSMK> newSMKRealizations,
            List<RealizedInternshipOldSMK> oldSMKRealizations,
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService,
            IExceptionHandlerService exceptionHandler = null,
            int? currentModuleId = null)
        {
            this.requirement = requirement;
            this.newSMKRealizations = newSMKRealizations ?? new List<RealizedInternshipNewSMK>();
            this.oldSMKRealizations = oldSMKRealizations ?? new List<RealizedInternshipOldSMK>();
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;
            this.exceptionHandler = exceptionHandler;
            this.currentModuleId = currentModuleId;
            this.isLoading = false;
            this.hasLoadedData = false;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.AddRealizationCommand = new AsyncRelayCommand(this.AddRealizationAsync);

            this.EditRealizationCommand = new AsyncRelayCommand<int>(this.EditRealizationAsync);
            this.DeleteRealizationCommand = new AsyncRelayCommand<int>(this.DeleteRealizationAsync);

            this.CheckSMKVersionAsync().ConfigureAwait(false);
        }

        private async Task CheckSMKVersionAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                this.isNewSMK = user?.SmkVersion == SmkVersion.New;
            }, "Nie udało się określić wersji SMK.");
        }

        private async Task EditRealizationAsync(int realizationId)
        {
            if (realizationId <= 0)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                // Logika nawigacji do strony edycji
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RealizedInternshipId", realizationId.ToString() },
                    { "InternshipRequirementId", this.requirement.InternshipId.ToString() }
                };

                if (!this.isNewSMK)
                {
                    var currentModule = await this.specializationService.GetCurrentModuleAsync();
                    if (currentModule != null)
                    {
                        int year = 1;
                        if (currentModule.Type == ModuleType.Basic)
                        {
                            year = 1; // Pierwszy rok dla modułu podstawowego
                        }
                        else
                        {
                            year = 3; // Trzeci rok dla modułu specjalistycznego 
                        }
                        navigationParameter.Add("Year", year.ToString());
                    }
                }
                else if (this.currentModuleId.HasValue)
                {
                    navigationParameter.Add("ModuleId", this.currentModuleId.Value.ToString());
                }

                await Shell.Current.GoToAsync("//AddEditRealizedInternship", navigationParameter);
            }, "Wystąpił problem podczas próby edycji realizacji stażu.");
        }

        private async Task DeleteRealizationAsync(int realizationId)
        {
            if (realizationId <= 0)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                bool confirmed = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę realizację stażu?",
                    "Tak",
                    "Nie");

                if (!confirmed)
                {
                    return;
                }

                bool success = false;

                if (this.isNewSMK)
                {
                    success = await this.specializationService.DeleteRealizedInternshipNewSMKAsync(realizationId);
                }
                else
                {
                    success = await this.specializationService.DeleteRealizedInternshipOldSMKAsync(realizationId);
                }

                if (success)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        "Realizacja stażu została usunięta.",
                        "OK");

                    if (this.isNewSMK)
                    {
                        this.newSMKRealizations = await this.specializationService.GetRealizedInternshipsNewSMKAsync(
                            moduleId: this.currentModuleId,
                            internshipRequirementId: this.requirement.InternshipId);
                    }
                    else
                    {
                        var currentModule = await this.specializationService.GetCurrentModuleAsync();
                        int startYear = 1;
                        int endYear = 2;

                        if (currentModule != null && currentModule.Type == ModuleType.Specialistic)
                        {
                            startYear = 3;
                            endYear = 6;
                        }

                        List<RealizedInternshipOldSMK> allRealizations = new List<RealizedInternshipOldSMK>();
                        for (int year = startYear; year <= endYear; year++)
                        {
                            var yearRealizations = await this.specializationService.GetRealizedInternshipsOldSMKAsync(year);
                            allRealizations.AddRange(yearRealizations);
                        }

                        this.oldSMKRealizations = allRealizations
                            .Where(r => r.InternshipName != null &&
                                   r.InternshipName.Equals(this.requirement.InternshipName, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }

                    this.OnPropertyChanged(nameof(NewSMKRealizationsCollection));
                    this.OnPropertyChanged(nameof(OldSMKRealizationsCollection));
                    this.OnPropertyChanged(nameof(IntroducedDays));
                    this.OnPropertyChanged(nameof(RemainingDays));
                    this.OnPropertyChanged(nameof(FormattedStatistics));
                }
                else
                {
                    throw new DomainLogicException(
                        "Failed to delete internship realization",
                        "Nie udało się usunąć realizacji stażu.");
                }
            }, "Wystąpił problem podczas usuwania realizacji stażu.");
        }

        // Properties remain unchanged
        public string Name => requirement.InternshipName;
        public int Id => requirement.InternshipId;
        public int RequiredDays => requirement.DaysCount;
        public string FormattedStatistics => GetFormattedStatistics();
        public string Title => $"Staż: {Name}";

        public int IntroducedDays => isNewSMK
            ? newSMKRealizations?.Sum(i => i.DaysCount) ?? 0
            : oldSMKRealizations?.Sum(i => i.DaysCount) ?? 0;

        public int RecognizedDays => isNewSMK
            ? newSMKRealizations?.Where(i => i.IsRecognition).Sum(i => i.RecognitionDaysReduction) ?? 0
            : 0;

        public int SelfEducationDays => 0; // Placeholder, do zaimplementowania jeśli potrzebne

        public int RemainingDays => RequiredDays - IntroducedDays - RecognizedDays;

        public ObservableCollection<RealizedInternshipNewSMK> NewSMKRealizationsCollection =>
            new ObservableCollection<RealizedInternshipNewSMK>(this.newSMKRealizations);

        public ObservableCollection<RealizedInternshipOldSMK> OldSMKRealizationsCollection =>
            new ObservableCollection<RealizedInternshipOldSMK>(this.oldSMKRealizations);

        private string GetFormattedStatistics()
        {
            int introduced = IntroducedDays;
            string competionMark = introduced >= RequiredDays ? "✔️" : "";
            return $"Zrealizowano {introduced} z {RequiredDays} dni {competionMark}";
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand AddRealizationCommand { get; }

        private async Task OnToggleExpandAsync()
        {
            if (this.isExpanded)
            {
                this.IsExpanded = false;
                return;
            }

            if (!this.hasLoadedData && !this.isLoading)
            {
                this.isLoading = true;
                this.IsExpanded = true;

                await SafeExecuteAsync(async () =>
                {
                    await Task.Run(async () =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            this.hasLoadedData = true;
                            this.isLoading = false;
                            this.OnPropertyChanged(nameof(this.IsLoading));
                        });
                    });
                }, "Wystąpił problem podczas ładowania danych stażu.");
            }
            else
            {
                this.IsExpanded = !this.isExpanded;
            }
        }

        private async Task AddRealizationAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                // Pobieramy nazwę stażu, zamiast polegać na ID
                var navigationParameter = new Dictionary<string, object>
                {
                    { "InternshipName", this.requirement.InternshipName },
                    { "DaysCount", this.requirement.DaysCount.ToString() }
                };

                // Wypisz dla debugowania
                System.Diagnostics.Debug.WriteLine($"Przekazuję dane stażu: Nazwa: {this.requirement.InternshipName}, Dni: {this.requirement.DaysCount}");

                if (this.currentModuleId.HasValue)
                {
                    navigationParameter.Add("ModuleId", this.currentModuleId.Value.ToString());
                }

                await Shell.Current.GoToAsync("//AddEditRealizedInternship", navigationParameter);
            }, "Wystąpił problem podczas dodawania nowej realizacji stażu.");
        }

        public void Refresh(List<RealizedInternshipNewSMK> newSMKRealizations, List<RealizedInternshipOldSMK> oldSMKRealizations)
        {
            if (isNewSMK && newSMKRealizations != null)
            {
                this.newSMKRealizations = newSMKRealizations.Where(r =>
                    r.InternshipName.Equals(this.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else if (!isNewSMK && oldSMKRealizations != null)
            {
                this.oldSMKRealizations = oldSMKRealizations.Where(r =>
                    r.InternshipName.Equals(this.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Odświeżenie właściwości
            OnPropertyChanged(nameof(IntroducedDays));
            OnPropertyChanged(nameof(FormattedStatistics));
            OnPropertyChanged(nameof(RemainingDays));
            OnPropertyChanged(nameof(NewSMKRealizationsCollection));
            OnPropertyChanged(nameof(OldSMKRealizationsCollection));
        }

        /// <summary>
        /// Safely executes an operation with automatic error handling
        /// </summary>
        private async Task SafeExecuteAsync(Func<Task> operation, string userFriendlyMessage = null)
        {
            if (exceptionHandler != null)
            {
                await exceptionHandler.ExecuteAsync(operation, null, userFriendlyMessage);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteAsync)}: {ex.Message}");
                    await dialogService.DisplayAlertAsync("Błąd", userFriendlyMessage ?? "Wystąpił nieoczekiwany błąd.", "OK");
                }
            }
        }

        /// <summary>
        /// Safely executes an operation that returns a value with automatic error handling
        /// </summary>
        private async Task<T> SafeExecuteAsync<T>(Func<Task<T>> operation, string userFriendlyMessage = null)
        {
            if (exceptionHandler != null)
            {
                return await exceptionHandler.ExecuteAsync(operation, null, userFriendlyMessage);
            }
            else
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteAsync)}: {ex.Message}");
                    await dialogService.DisplayAlertAsync("Błąd", userFriendlyMessage ?? "Wystąpił nieoczekiwany błąd.", "OK");
                    return default;
                }
            }
        }
    }
}
```

## File: ViewModels/Internships/NewSMKInternshipsListViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class NewSMKInternshipsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;

        private ObservableCollection<InternshipStageViewModel> internshipRequirements;
        private bool isRefreshing;
        private string moduleTitle;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasTwoModules;
        private bool isLoading = false;

        public NewSMKInternshipsListViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;

            this.Title = "Staże (Nowy SMK)";
            this.InternshipRequirements = new ObservableCollection<InternshipStageViewModel>();
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<InternshipStageViewModel> InternshipRequirements
        {
            get => this.internshipRequirements;
            set => this.SetProperty(ref this.internshipRequirements, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Basic module not found",
                            "Nie znaleziono modułu podstawowego.");
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Specialistic module not found",
                            "Nie znaleziono modułu specjalistycznego.");
                    }
                }

                await this.LoadDataAsync();
            }, $"Nie udało się przełączyć na moduł {moduleType}.");
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy || isLoading)
            {
                return;
            }

            isLoading = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        throw new ResourceNotFoundException(
                            "Specialization not found",
                            "Nie znaleziono aktywnej specjalizacji.");
                    }

                    var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                    this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);
                    var currentModule = await this.specializationService.GetCurrentModuleAsync();

                    if (currentModule != null)
                    {
                        this.ModuleTitle = currentModule.Name;
                        this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                        this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Current module not found",
                            "Nie znaleziono aktualnego modułu.");
                    }

                    var internships = await this.specializationService.GetInternshipsAsync(currentModule?.ModuleId);

                    var viewModels = new List<InternshipStageViewModel>();

                    foreach (var internship in internships)
                    {
                        // For new SMK
                        var realizedInternships = await this.specializationService.GetRealizedInternshipsNewSMKAsync(
                            moduleId: currentModule?.ModuleId,
                            internshipRequirementId: internship.InternshipId);

                        // Get the exception handler to pass to the InternshipStageViewModel
                        var exceptionHandler = IPlatformApplication.Current.Services.GetService<IExceptionHandlerService>();

                        var viewModel = new InternshipStageViewModel(
                            internship,
                            realizedInternships,
                            null, // Empty for new SMK
                            this.specializationService,
                            this.dialogService,
                            this.authService,
                            exceptionHandler,
                            currentModule?.ModuleId);

                        viewModels.Add(viewModel);
                    }

                    this.InternshipRequirements.Clear();
                    foreach (var viewModel in viewModels)
                    {
                        this.InternshipRequirements.Add(viewModel);
                    }
                }, "Wystąpił błąd podczas ładowania danych. Spróbuj ponownie.");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
                isLoading = false;
            }
        }
    }
}
```

## File: ViewModels/Internships/OldSMKInternshipsListViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class OldSMKInternshipsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;

        private ObservableCollection<InternshipStageViewModel> internshipRequirements;
        private bool isRefreshing;
        private string moduleTitle;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasTwoModules;
        private bool isLoading = false;

        public OldSMKInternshipsListViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;

            this.Title = "Staże (Stary SMK)";
            this.InternshipRequirements = new ObservableCollection<InternshipStageViewModel>();
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<InternshipStageViewModel> InternshipRequirements
        {
            get => this.internshipRequirements;
            set => this.SetProperty(ref this.internshipRequirements, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Basic module not found",
                            "Nie znaleziono modułu podstawowego.");
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Specialistic module not found",
                            "Nie znaleziono modułu specjalistycznego.");
                    }
                }

                await this.LoadDataAsync();
            }, $"Nie udało się przełączyć na moduł {moduleType}.");
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy || isLoading)
            {
                return;
            }

            isLoading = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        throw new ResourceNotFoundException(
                            "Specialization not found",
                            "Nie znaleziono aktywnej specjalizacji.");
                    }

                    var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                    this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);
                    var currentModule = await this.specializationService.GetCurrentModuleAsync();

                    if (currentModule != null)
                    {
                        this.ModuleTitle = currentModule.Name;
                        this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                        this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Current module not found",
                            "Nie znaleziono aktualnego modułu.");
                    }

                    var internships = await this.specializationService.GetInternshipsAsync(currentModule?.ModuleId);

                    int startYear = 1;
                    int endYear = 2;

                    if (currentModule != null && currentModule.Type == ModuleType.Specialistic)
                    {
                        startYear = 3;
                        endYear = 6;
                    }

                    var dbService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Database.IDatabaseService>();
                    var allDbRealizations = await dbService.QueryAsync<RealizedInternshipOldSMK>(
                        "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ?", specialization.SpecializationId);
                    var allRealizedInternships = new List<RealizedInternshipOldSMK>();

                    // Get realizations from Year=0 first (not assigned to a specific year)
                    var yearZeroRealizations = await dbService.QueryAsync<RealizedInternshipOldSMK>(
                        "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ? AND Year = 0",
                        specialization.SpecializationId);

                    foreach (var r in yearZeroRealizations)
                    {
                        allRealizedInternships.Add(r);
                    }

                    for (int year = startYear; year <= endYear; year++)
                    {
                        var yearRealizations = await this.specializationService.GetRealizedInternshipsOldSMKAsync(year);
                        allRealizedInternships.AddRange(yearRealizations.Where(r => r.Year == year));
                    }

                    var viewModels = new List<InternshipStageViewModel>();

                    // Get the exception handler to pass to the InternshipStageViewModel
                    var exceptionHandler = IPlatformApplication.Current.Services.GetService<IExceptionHandlerService>();

                    foreach (var internship in internships)
                    {
                        var realizationsForThisInternship = allRealizedInternships
                            .Where(r => {
                                string realizationName = r.InternshipName ?? "null";
                                string requirementName = internship.InternshipName ?? "null";

                                if (string.IsNullOrEmpty(r.InternshipName) || r.InternshipName == "Staż bez nazwy")
                                {
                                    if (internships.IndexOf(internship) == 0)
                                    {
                                        return true;
                                    }
                                    return false;
                                }

                                bool exactMatch = r.InternshipName != null &&
                                    r.InternshipName.Equals(internship.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool realizationContainsRequirement = r.InternshipName != null &&
                                    r.InternshipName.Contains(internship.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool requirementContainsRealization = internship.InternshipName != null &&
                                    internship.InternshipName.Contains(r.InternshipName, StringComparison.OrdinalIgnoreCase);

                                string cleanRealizationName = realizationName
                                    .Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
                                string cleanRequirementName = requirementName
                                    .Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
                                bool fuzzyMatch = cleanRealizationName.Contains(cleanRequirementName) ||
                                                cleanRequirementName.Contains(cleanRealizationName);

                                bool matches = exactMatch || realizationContainsRequirement || requirementContainsRealization || fuzzyMatch;

                                return matches;
                            })
                            .ToList();

                        var viewModel = new InternshipStageViewModel(
                            internship,
                            null, // Empty for old SMK
                            realizationsForThisInternship,
                            this.specializationService,
                            this.dialogService,
                            this.authService,
                            exceptionHandler,
                            currentModule?.ModuleId);

                        viewModels.Add(viewModel);
                    }

                    this.InternshipRequirements.Clear();
                    foreach (var viewModel in viewModels)
                    {
                        this.InternshipRequirements.Add(viewModel);
                    }
                }, "Wystąpił błąd podczas ładowania danych. Spróbuj ponownie.");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
                isLoading = false;
            }
        }
    }
}
```

## File: ViewModels/MedicalShifts/AddEditMedicalShiftViewModel.cs
```csharp
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class AddEditMedicalShiftViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private bool isEdit;
        private MedicalShift shift;
        private Internship selectedInternship;
        private ObservableCollection<Internship> availableInternships;
        private ObservableCollection<KeyValuePair<string, string>> yearOptions;
        private string selectedYear;

        public AddEditMedicalShiftViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            this.AvailableInternships = new ObservableCollection<Internship>();
            this.YearOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            this.shift = new MedicalShift
            {
                Date = DateTime.Now,
                Hours = 10,
                Minutes = 5,
                Year = 1,
                SyncStatus = SyncStatus.NotSynced
            };
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public MedicalShift Shift
        {
            get => this.shift;
            set => this.SetProperty(ref this.shift, value);
        }

        public ObservableCollection<Internship> AvailableInternships
        {
            get => this.availableInternships;
            set => this.SetProperty(ref this.availableInternships, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set
            {
                if (this.SetProperty(ref this.selectedInternship, value) && value != null)
                {
                    this.Shift.InternshipId = value.InternshipId;
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        public string SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value) && !string.IsNullOrEmpty(value))
                {
                    this.Shift.Year = int.Parse(value);
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public async Task InitializeAsync(int? shiftId = null)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                this.IsEdit = shiftId.HasValue && shiftId.Value > 0;
                this.Title = this.IsEdit ? "Edytuj dyżur" : "Dodaj nowy dyżur";

                await this.LoadYearOptionsAsync();
                await this.LoadInternshipsAsync();

                if (this.IsEdit && shiftId.HasValue)
                {
                    var existingShift = await this.databaseService.GetMedicalShiftAsync(shiftId.Value);
                    if (existingShift != null)
                    {
                        this.Shift = existingShift;
                        this.SelectedInternship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == existingShift.InternshipId);
                        this.SelectedYear = existingShift.Year.ToString();
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            $"Medical shift with ID {shiftId.Value} not found",
                            "Nie znaleziono dyżuru o podanym identyfikatorze.");
                    }
                }
                else
                {
                    if (this.AvailableInternships.Count > 0)
                    {
                        this.SelectedInternship = this.AvailableInternships[0];
                    }
                    this.SelectedYear = "1";
                }
            }, "Wystąpił problem podczas inicjalizacji formularza dyżuru.");

            this.IsBusy = false;
        }

        private async Task LoadYearOptionsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Current specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                this.YearOptions.Clear();
                for (int i = 1; i <= specialization.DurationYears; i++)
                {
                    this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
                }
            }, "Wystąpił problem podczas ładowania opcji lat specjalizacji.");
        }

        private async Task LoadInternshipsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                var internships = await this.specializationService.GetInternshipsAsync(moduleId: currentModule?.ModuleId);
                var userInternships = internships.Where(i => i.InternshipId > 0).ToList();

                this.AvailableInternships.Clear();
                foreach (var internship in userInternships)
                {
                    this.AvailableInternships.Add(internship);
                }

                if (this.AvailableInternships.Count == 0)
                {
                    throw new BusinessRuleViolationException(
                        "No internships available",
                        "Brak dostępnych staży. Należy najpierw dodać staż, aby móc dodać dyżur.");
                }
            }, "Wystąpił problem podczas ładowania listy dostępnych staży.");
        }

        private bool CanSave()
        {
            return this.Shift != null
                && this.Shift.Hours > 0
                && this.SelectedInternship != null;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    if (this.SelectedInternship == null)
                    {
                        throw new InvalidInputException(
                            "No internship selected",
                            "Nie wybrano stażu. Wybierz staż, aby dodać dyżur.");
                    }

                    if (this.Shift.Hours <= 0 && this.Shift.Minutes <= 0)
                    {
                        throw new InvalidInputException(
                            "Invalid shift duration",
                            "Czas dyżuru musi być większy od zera.");
                    }

                    this.Shift.InternshipId = this.SelectedInternship.InternshipId;

                    bool success;
                    if (this.IsEdit)
                    {
                        success = await this.specializationService.UpdateMedicalShiftAsync(this.Shift);
                    }
                    else
                    {
                        success = await this.specializationService.AddMedicalShiftAsync(this.Shift);
                    }

                    if (success)
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            this.IsEdit ? "Dyżur został zaktualizowany." : "Dyżur został dodany.",
                            "OK");

                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to save medical shift",
                            "Nie udało się zapisać dyżuru. Sprawdź poprawność danych.");
                    }
                }, "Wystąpił problem podczas zapisywania dyżuru.");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Wystąpił problem podczas anulowania edycji.");
        }
    }
}
```

## File: ViewModels/MedicalShifts/AddEditOldSMKMedicalShiftViewModel.cs
```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    [QueryProperty(nameof(ShiftId), nameof(ShiftId))]
    [QueryProperty(nameof(YearParam), nameof(YearParam))]
    public class AddEditOldSMKMedicalShiftViewModel : BaseViewModel
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly IDatabaseService databaseService;

        private bool isEdit;
        private RealizedMedicalShiftOldSMK shift;
        private int year;
        private string shiftId;
        private bool lastLocationLoaded = false;

        public AddEditOldSMKMedicalShiftViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService,
            IDatabaseService databaseService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.databaseService = databaseService;
            this.shift = new RealizedMedicalShiftOldSMK
            {
                StartDate = DateTime.Today,
                Hours = 24,
                Minutes = 0,
                Year = 1,
            };
            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);
        }

        private string yearParam;
        public string ShiftId
        {
            set
            {
                this.shiftId = value;
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.LoadShiftAsync(id).ConfigureAwait(false);
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj dyżur";
                }
            }
        }

        public string YearParam
        {
            set
            {
                this.yearParam = value;

                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int yearValue))
                {
                    this.year = yearValue;
                    this.shift.Year = yearValue;

                    this.OnPropertyChanged(nameof(Shift));
                    this.OnPropertyChanged(nameof(Shift.Year));
                }
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedMedicalShiftOldSMK Shift
        {
            get => this.shift;
            set
            {
                if (this.SetProperty(ref this.shift, value))
                {
                    if (!this.IsEdit && !this.lastLocationLoaded)
                    {
                        this.LoadLastLocationAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadLastLocationAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                this.lastLocationLoaded = true;
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono aktywnego użytkownika.");
                }

                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? ORDER BY ShiftId DESC LIMIT 1";
                var lastShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, user.SpecializationId);

                if (lastShifts.Count > 0)
                {
                    this.shift.SpecializationId = user.SpecializationId;

                    if (string.IsNullOrEmpty(this.shift.Location))
                    {
                        this.shift.Location = lastShifts[0].Location;
                        this.OnPropertyChanged(nameof(Shift));
                    }
                }
            }, "Nie udało się załadować ostatniej lokalizacji dyżuru.");
        }

        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                bool isEditMode = !string.IsNullOrEmpty(this.shiftId) &&
                                  int.TryParse(this.shiftId, out int shiftIdValue) &&
                                  shiftIdValue > 0;
                this.IsEdit = isEditMode;
                this.Title = this.IsEdit ? "Edytuj dyżur" : "Dodaj dyżur";

                if (this.IsEdit)
                {
                    await this.LoadShiftAsync(int.Parse(this.shiftId));
                }
                else
                {
                    if (this.year > 0)
                    {
                        this.shift.Year = this.year;
                        this.OnPropertyChanged(nameof(Shift));
                        this.OnPropertyChanged(nameof(Shift.Year));
                    }

                    var user = await this.authService.GetCurrentUserAsync();
                    if (user != null)
                    {
                        this.shift.SpecializationId = user.SpecializationId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "User not found",
                            "Nie znaleziono aktywnego użytkownika.");
                    }

                    if (!this.lastLocationLoaded)
                    {
                        await this.LoadLastLocationAsync();
                    }
                }
            }, "Wystąpił problem podczas inicjalizacji formularza dyżuru.");

            this.IsBusy = false;
        }

        private async Task LoadShiftAsync(int shiftId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                var loadedShift = await this.medicalShiftsService.GetOldSMKShiftAsync(shiftId);
                if (loadedShift != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj dyżur";
                    this.Shift = loadedShift;
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj dyżur";
                    throw new ResourceNotFoundException(
                        $"Medical shift with ID {shiftId} not found",
                        "Nie znaleziono dyżuru o podanym identyfikatorze.");
                }
            }, "Wystąpił problem podczas ładowania danych dyżuru.");

            this.IsBusy = false;
        }

        private async Task SaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    if (this.shift.Hours <= 0 && this.shift.Minutes <= 0)
                    {
                        throw new InvalidInputException(
                            "Invalid shift duration",
                            "Czas dyżuru musi być większy od zera.");
                    }

                    if (string.IsNullOrWhiteSpace(this.shift.Location))
                    {
                        throw new InvalidInputException(
                            "Location is required",
                            "Nazwa komórki organizacyjnej jest wymagana.");
                    }

                    if (this.shift.Year <= 0)
                    {
                        this.shift.Year = this.year;
                    }

                    var user = await this.authService.GetCurrentUserAsync();
                    if (user != null)
                    {
                        this.shift.SpecializationId = user.SpecializationId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "User not found",
                            "Nie znaleziono aktywnego użytkownika.");
                    }

                    bool success = await this.medicalShiftsService.SaveOldSMKShiftAsync(this.shift);

                    if (success)
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            this.IsEdit ? "Dyżur został zaktualizowany." : "Dyżur został dodany.",
                            "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to save medical shift",
                            "Nie udało się zapisać dyżuru.");
                    }
                }, "Wystąpił problem podczas zapisywania dyżuru.");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Wystąpił problem podczas anulowania edycji.");
        }
    }
}
```

## File: ViewModels/MedicalShifts/MedicalShiftsListViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class MedicalShiftsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;

        private ObservableCollection<MedicalShift> shifts;
        private MedicalShift selectedShift;
        private bool isRefreshing;
        private MedicalShiftsSummary summary;
        private List<Internship> internships;
        private bool isNewSmk;
        private Internship selectedInternship;
        private string shiftsDescription;
        private int totalShiftHours;
        private int requiredShiftHours;
        private double shiftProgress;
        private string moduleTitle;

        public MedicalShiftsListViewModel(
            ISpecializationService specializationService,
            IAuthService authService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.authService = authService;
            this.dialogService = dialogService;

            this.Title = "Dyżury medyczne";
            this.Shifts = new ObservableCollection<MedicalShift>();
            this.Internships = new List<Internship>();
            this.Summary = new MedicalShiftsSummary();
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.AddShiftCommand = new AsyncRelayCommand(this.AddShiftAsync);
            this.EditShiftCommand = new AsyncRelayCommand<MedicalShift>(this.EditShiftAsync);
            this.DeleteShiftCommand = new AsyncRelayCommand<MedicalShift>(this.DeleteShiftAsync);
            this.SelectInternshipCommand = new AsyncRelayCommand<Internship>(this.SelectInternshipAsync);

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<MedicalShift> Shifts
        {
            get => this.shifts;
            set => this.SetProperty(ref this.shifts, value);
        }

        public MedicalShift SelectedShift
        {
            get => this.selectedShift;
            set => this.SetProperty(ref this.selectedShift, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public MedicalShiftsSummary Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public List<Internship> Internships
        {
            get => this.internships;
            set => this.SetProperty(ref this.internships, value);
        }

        public bool IsNewSmk
        {
            get => this.isNewSmk;
            set => this.SetProperty(ref this.isNewSmk, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set => this.SetProperty(ref this.selectedInternship, value);
        }

        public string ShiftsDescription
        {
            get => this.shiftsDescription;
            set => this.SetProperty(ref this.shiftsDescription, value);
        }

        public int TotalShiftHours
        {
            get => this.totalShiftHours;
            set => this.SetProperty(ref this.totalShiftHours, value);
        }

        public int RequiredShiftHours
        {
            get => this.requiredShiftHours;
            set => this.SetProperty(ref this.requiredShiftHours, value);
        }

        public double ShiftProgress
        {
            get => this.shiftProgress;
            set => this.SetProperty(ref this.shiftProgress, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddShiftCommand { get; }
        public ICommand EditShiftCommand { get; }
        public ICommand DeleteShiftCommand { get; }
        public ICommand SelectInternshipCommand { get; }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            this.IsRefreshing = true;

            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();

                if (user != null)
                {
                    if (user.SmkVersion == SmkVersion.Old)
                    {
                        this.IsNewSmk = false;
                    }
                    else
                    {
                        this.IsNewSmk = true;
                    }
                }
                else
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono aktywnego użytkownika.");
                }

                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    this.ModuleTitle = currentModule.Name;
                }

                var stats = await this.specializationService.GetSpecializationStatisticsAsync(
                    currentModule?.ModuleId);

                this.TotalShiftHours = stats.CompletedShiftHours;
                this.RequiredShiftHours = stats.RequiredShiftHours;

                if (this.RequiredShiftHours > 0)
                {
                    this.ShiftProgress = Math.Min(1.0, (double)this.TotalShiftHours / this.RequiredShiftHours);
                }
                else
                {
                    this.ShiftProgress = 0;
                }

                if (this.IsNewSmk && currentModule != null)
                {
                    this.Internships = await this.specializationService.GetInternshipsAsync(currentModule.ModuleId);
                }
                else
                {
                    this.Internships = await this.specializationService.GetInternshipsAsync();
                }

                if (this.SelectedInternship == null && this.Internships.Count > 0)
                {
                    await this.SelectInternshipAsync(this.Internships[0]);
                }
                else if (this.SelectedInternship != null)
                {
                    await this.LoadShiftsForInternshipAsync(this.SelectedInternship.InternshipId);
                }
                else
                {
                    var shifts = await this.specializationService.GetMedicalShiftsAsync();
                    this.Shifts.Clear();
                    foreach (var shift in shifts)
                    {
                        this.Shifts.Add(shift);
                    }

                    this.Summary = MedicalShiftsSummary.CalculateFromShifts(shifts);
                    this.ShiftsDescription = "Wszystkie dyżury";
                }
            }, "Wystąpił problem podczas ładowania danych dyżurów.");

            this.IsBusy = false;
            this.IsRefreshing = false;
        }

        private async Task LoadShiftsForInternshipAsync(int internshipId)
        {
            await SafeExecuteAsync(async () =>
            {
                var shifts = await this.specializationService.GetMedicalShiftsAsync(internshipId);
                this.Shifts.Clear();
                foreach (var shift in shifts)
                {
                    this.Shifts.Add(shift);
                }

                this.Summary = MedicalShiftsSummary.CalculateFromShifts(shifts);

                if (this.SelectedInternship != null)
                {
                    if (this.IsNewSmk)
                    {
                        this.ShiftsDescription = $"Dyżury do stażu {this.SelectedInternship.InternshipName}";
                    }
                    else
                    {
                        this.ShiftsDescription = "Lista zrealizowanych dyżurów medycznych";
                    }
                }
            }, $"Wystąpił problem podczas ładowania dyżurów dla stażu (ID: {internshipId}).");
        }

        private async Task SelectInternshipAsync(Internship internship)
        {
            if (internship == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                this.SelectedInternship = internship;
                await this.LoadShiftsForInternshipAsync(internship.InternshipId);
            }, "Wystąpił problem podczas wybierania stażu.");
        }

        private async Task AddShiftAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                if (this.SelectedInternship == null && this.Internships.Count > 0)
                {
                    var result = await this.dialogService.DisplayActionSheetAsync(
                        "Wybierz staż",
                        "Anuluj",
                        null,
                        this.Internships.Select(i => i.InternshipName).ToArray());

                    if (result == "Anuluj" || result == null)
                    {
                        return;
                    }

                    this.SelectedInternship = this.Internships.FirstOrDefault(i => i.InternshipName == result);
                }
                else if (this.Internships.Count == 0)
                {
                    throw new BusinessRuleViolationException(
                        "No internships available",
                        "Nie można dodać dyżuru, ponieważ nie ma zdefiniowanych staży. Najpierw dodaj staż.");
                }

                if (this.SelectedInternship != null)
                {
                    await this.AddMedicalShiftAsync(this.SelectedInternship.InternshipId);
                }
            }, "Wystąpił problem podczas dodawania nowego dyżuru.");
        }

        public async Task AddMedicalShiftAsync(int internshipId)
        {
            await SafeExecuteAsync(async () =>
            {
                var parameters = new Dictionary<string, object>
                {
                    { "internshipId", internshipId }
                };

                await Shell.Current.GoToAsync("addeditmedicalshifts", parameters);
            }, "Wystąpił problem podczas nawigacji do formularza dodawania dyżuru.");
        }

        private async Task EditShiftAsync(MedicalShift shift)
        {
            if (shift == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync($"AddEditMedicalShift?shiftId={shift.ShiftId}");
            }, "Wystąpił problem podczas nawigacji do formularza edycji dyżuru.");
        }

        private async Task DeleteShiftAsync(MedicalShift shift)
        {
            if (shift == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                if (!shift.CanBeDeleted)
                {
                    throw new BusinessRuleViolationException(
                        "Cannot delete approved shift",
                        "Nie można usunąć zatwierdzonego dyżuru.");
                }

                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć ten dyżur?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool success = await this.specializationService.DeleteMedicalShiftAsync(shift.ShiftId);
                    if (success)
                    {
                        this.Shifts.Remove(shift);
                        this.Summary = MedicalShiftsSummary.CalculateFromShifts(this.Shifts.ToList());
                        var currentModule = await this.specializationService.GetCurrentModuleAsync();
                        if (currentModule != null)
                        {
                            await this.specializationService.UpdateModuleProgressAsync(currentModule.ModuleId);
                        }
                        await this.LoadDataAsync();
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to delete medical shift",
                            "Nie udało się usunąć dyżuru. Spróbuj ponownie.");
                    }
                }
            }, "Wystąpił problem podczas usuwania dyżuru.");
        }
    }
}
```

## File: ViewModels/MedicalShifts/MedicalShiftsSelectorViewModel.cs
```csharp
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class MedicalShiftsSelectorViewModel : BaseViewModel
    {
        public MedicalShiftsSelectorViewModel(IExceptionHandlerService exceptionHandler = null) : base(exceptionHandler)
        {
            this.Title = "Dyżury medyczne";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        private async Task NavigateToOldSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("OldSMKMedicalShifts");
            }, "Wystąpił problem podczas nawigacji do dyżurów (Stary SMK).");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("NewSMKMedicalShifts");
            }, "Wystąpił problem podczas nawigacji do dyżurów (Nowy SMK).");
        }
    }
}
```

## File: ViewModels/MedicalShifts/NewSMKMedicalShiftsListViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class NewSMKMedicalShiftsListViewModel : BaseViewModel
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<InternshipRequirementViewModel> internshipRequirements;
        private bool isRefreshing;
        private string moduleTitle;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasTwoModules;
        private bool isLoading = false;

        public NewSMKMedicalShiftsListViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Dyżury medyczne (Nowy SMK)";
            this.InternshipRequirements = new ObservableCollection<InternshipRequirementViewModel>();
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<InternshipRequirementViewModel> InternshipRequirements
        {
            get => this.internshipRequirements;
            set => this.SetProperty(ref this.internshipRequirements, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                int? newModuleId = null;

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        newModuleId = basicModule.ModuleId;
                        await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Basic module not found",
                            "Nie znaleziono modułu podstawowego.");
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        newModuleId = specialisticModule.ModuleId;
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Specialistic module not found",
                            "Nie znaleziono modułu specjalistycznego.");
                    }
                }

                await this.LoadDataAsync();
            }, $"Wystąpił problem podczas przełączania modułu na {moduleType}.");
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy || isLoading)
            {
                return;
            }

            isLoading = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        throw new ResourceNotFoundException(
                            "Specialization not found",
                            "Nie znaleziono aktywnej specjalizacji.");
                    }

                    var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                    this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);
                    var currentModule = await this.specializationService.GetCurrentModuleAsync();

                    if (currentModule != null)
                    {
                        this.ModuleTitle = currentModule.Name;
                        this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                        this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Current module not found",
                            "Nie znaleziono aktualnego modułu.");
                    }

                    var requirements = await this.medicalShiftsService.GetAvailableInternshipRequirementsAsync();
                    var viewModels = new List<InternshipRequirementViewModel>();

                    // Get the exception handler to pass to the InternshipRequirementViewModel
                    var exceptionHandler = IPlatformApplication.Current.Services.GetService<IExceptionHandlerService>();

                    foreach (var requirement in requirements)
                    {
                        var summary = await this.medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: requirement.Id);
                        var shifts = await this.medicalShiftsService.GetNewSMKShiftsAsync(requirement.Id);
                        var viewModel = new InternshipRequirementViewModel(
                            requirement,
                            summary,
                            shifts,
                            this.medicalShiftsService,
                            this.dialogService,
                            exceptionHandler,
                            currentModule?.ModuleId);

                        viewModels.Add(viewModel);
                    }

                    this.InternshipRequirements.Clear();
                    foreach (var viewModel in viewModels)
                    {
                        this.InternshipRequirements.Add(viewModel);
                    }
                }, "Wystąpił problem podczas ładowania danych dyżurów.");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
                isLoading = false;
            }
        }
    }
}
```

## File: ViewModels/MedicalShifts/OldSMKMedicalShiftsListViewModel.cs
```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class OldSMKMedicalShiftsListViewModel : BaseViewModel
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<int> availableYears;
        private int selectedYear;
        private ObservableCollection<RealizedMedicalShiftOldSMK> shifts;
        private bool isRefreshing;
        private MedicalShiftsSummary summary;
        private string moduleTitle;

        public OldSMKMedicalShiftsListViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Dyżury medyczne (Stary SMK)";
            this.AvailableYears = new ObservableCollection<int>();
            this.Shifts = new ObservableCollection<RealizedMedicalShiftOldSMK>();
            this.Summary = new MedicalShiftsSummary();

            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectYearCommand = new AsyncRelayCommand<int>(this.SelectYearAsync);
            this.AddShiftCommand = new AsyncRelayCommand(this.AddShiftAsync);
            this.EditShiftCommand = new AsyncRelayCommand<RealizedMedicalShiftOldSMK>(this.EditShiftAsync);
            this.DeleteShiftCommand = new AsyncRelayCommand<RealizedMedicalShiftOldSMK>(this.DeleteShiftAsync);

            this.LoadYearsAsync().ConfigureAwait(false);
        }

        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
        }

        public int SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value))
                {
                    this.LoadDataAsyncForSelectedYear(value).ConfigureAwait(false);
                }
            }
        }

        private async Task LoadDataAsyncForSelectedYear(int year)
        {
            await this.LoadDataAsync();
        }

        public ObservableCollection<RealizedMedicalShiftOldSMK> Shifts
        {
            get => this.shifts;
            set => this.SetProperty(ref this.shifts, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public MedicalShiftsSummary Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectYearCommand { get; }
        public ICommand AddShiftCommand { get; }
        public ICommand EditShiftCommand { get; }
        public ICommand DeleteShiftCommand { get; }

        private async Task LoadYearsAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                var module = await this.specializationService.GetCurrentModuleAsync();
                if (module != null)
                {
                    this.ModuleTitle = module.Name;
                }
                else
                {
                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktualnego modułu.");
                }

                var years = await this.medicalShiftsService.GetAvailableYearsAsync();
                this.AvailableYears.Clear();

                foreach (var year in years)
                {
                    this.AvailableYears.Add(year);
                }

                this.OnPropertyChanged(nameof(AvailableYears));

                if (this.AvailableYears.Contains(1))
                {
                    this.selectedYear = 1;
                    this.OnPropertyChanged(nameof(SelectedYear));

                    var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(1);

                    await MainThread.InvokeOnMainThreadAsync(() => {
                        this.Shifts.Clear();
                        foreach (var shift in shifts)
                        {
                            this.Shifts.Add(shift);
                        }
                    });

                    this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: 1);
                    this.OnPropertyChanged(nameof(Summary));
                }
                else if (this.AvailableYears.Count > 0)
                {
                    int firstYear = this.AvailableYears[0];
                    this.selectedYear = firstYear;
                    this.OnPropertyChanged(nameof(SelectedYear));

                    var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(firstYear);

                    await MainThread.InvokeOnMainThreadAsync(() => {
                        this.Shifts.Clear();
                        foreach (var shift in shifts)
                        {
                            this.Shifts.Add(shift);
                        }
                    });

                    this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: firstYear);
                    this.OnPropertyChanged(nameof(Summary));
                }
                else
                {
                    // No years available, probably first time
                    this.AvailableYears.Add(1); // Add default year 1
                    this.selectedYear = 1;
                    this.OnPropertyChanged(nameof(SelectedYear));
                    this.Shifts.Clear();
                    this.Summary = new MedicalShiftsSummary();
                    this.OnPropertyChanged(nameof(Summary));
                }
            }, "Wystąpił problem podczas ładowania dostępnych lat specjalizacji.");

            this.IsBusy = false;
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            this.IsRefreshing = true;

            await SafeExecuteAsync(async () =>
            {
                var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(this.SelectedYear);

                this.Shifts.Clear();
                foreach (var shift in shifts)
                {
                    this.Shifts.Add(shift);
                }

                this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);
            }, $"Wystąpił problem podczas ładowania dyżurów dla roku {this.SelectedYear}.");

            this.IsBusy = false;
            this.IsRefreshing = false;
        }

        private async Task SelectYearAsync(int year)
        {
            if (this.IsBusy)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                this.SelectedYear = year;
                await this.LoadDataAsync();
            }, $"Wystąpił problem podczas wybierania roku {year}.");
        }

        private async Task AddShiftAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                if (this.SelectedYear <= 0 && this.AvailableYears.Count > 0)
                {
                    this.SelectedYear = this.AvailableYears[0];
                }

                var navigationParameter = new Dictionary<string, object>
                {
                    { "ShiftId", "-1" },
                    { "YearParam", this.SelectedYear.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", navigationParameter);
            }, "Wystąpił problem podczas nawigacji do formularza dodawania dyżuru.");
        }

        private async Task EditShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ShiftId", shift.ShiftId.ToString() },
                    { "YearParam", shift.Year.ToString() }
                };

                await Shell.Current.GoToAsync("//medicalshifts/AddEditOldSMKMedicalShift", navigationParameter);
            }, "Wystąpił problem podczas nawigacji do formularza edycji dyżuru.");
        }

        private async Task DeleteShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć ten dyżur?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);
                    if (success)
                    {
                        this.Shifts.Remove(shift);
                        this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to delete medical shift",
                            "Nie udało się usunąć dyżuru.");
                    }
                }
            }, "Wystąpił problem podczas usuwania dyżuru.");
        }
    }
}
```

## File: ViewModels/Procedures/AddEditNewSMKProcedureViewModel.cs
```csharp
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    [QueryProperty(nameof(ProcedureId), nameof(ProcedureId))]
    [QueryProperty(nameof(RequirementId), nameof(RequirementId))]
    public class AddEditNewSMKProcedureViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private string procedureId;
        private string requirementId;
        private string procedureName;
        private int countA;
        private int countB;
        private DateTime startDate;
        private DateTime endDate;
        private RealizedProcedureNewSMK procedure;
        private bool isEdit;

        public AddEditNewSMKProcedureViewModel(
            IProcedureService procedureService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.procedureService = procedureService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            this.startDate = DateTime.Now;
            this.endDate = DateTime.Now;

            this.Title = "Dodaj realizację procedury";
        }

        public string ProcedureId
        {
            set
            {
                this.procedureId = value;
                this.LoadProcedureAsync().ConfigureAwait(false);
            }
        }

        public string RequirementId
        {
            set
            {
                this.requirementId = value;
                this.LoadRequirementAsync().ConfigureAwait(false);
            }
        }

        public string ProcedureName
        {
            get => this.procedureName;
            set => this.SetProperty(ref this.procedureName, value);
        }

        public int CountA
        {
            get => this.countA;
            set
            {
                if (this.SetProperty(ref this.countA, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public int CountB
        {
            get => this.countB;
            set
            {
                if (this.SetProperty(ref this.countB, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set
            {
                if (this.SetProperty(ref this.startDate, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get => this.endDate;
            set
            {
                if (this.SetProperty(ref this.endDate, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadProcedureAsync()
        {
            if (this.IsBusy || string.IsNullOrEmpty(this.procedureId))
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                if (int.TryParse(this.procedureId, out int id))
                {
                    this.procedure = await this.procedureService.GetNewSMKProcedureAsync(id);
                    if (this.procedure != null)
                    {
                        this.isEdit = true;
                        this.Title = "Edytuj realizację procedury";

                        this.CountA = this.procedure.CountA;
                        this.CountB = this.procedure.CountB;
                        this.StartDate = this.procedure.StartDate;
                        this.EndDate = this.procedure.EndDate;
                        this.ProcedureName = this.procedure.ProcedureName;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            $"Procedure with ID {id} not found",
                            "Nie znaleziono realizacji procedury o podanym identyfikatorze.");
                    }
                }
            }, "Wystąpił problem podczas ładowania danych realizacji procedury.");

            this.IsBusy = false;
        }

        private async Task LoadRequirementAsync()
        {
            if (this.IsBusy || string.IsNullOrEmpty(this.requirementId))
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                if (int.TryParse(this.requirementId, out int id))
                {
                    var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync();
                    var requirement = requirements.FirstOrDefault(r => r.Id == id);

                    if (requirement != null)
                    {
                        this.ProcedureName = requirement.Name;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            $"Requirement with ID {id} not found",
                            "Nie znaleziono wymagania procedurowego o podanym identyfikatorze.");
                    }
                }
            }, "Wystąpił problem podczas ładowania danych wymagania procedury.");

            this.IsBusy = false;
        }

        private bool CanSave()
        {
            return (this.CountA > 0 || this.CountB > 0) &&
                   this.StartDate <= this.EndDate;
        }

        private async Task<bool> ValidateInputsAsync()
        {
            if (string.IsNullOrWhiteSpace(this.ProcedureName))
            {
                throw new InvalidInputException("Procedure name is required", "Nazwa procedury jest wymagana.");
            }

            if (this.CountA <= 0 && this.CountB <= 0)
            {
                throw new InvalidInputException(
                    "At least one procedure count must be greater than zero",
                    "Przynajmniej jedna z liczb wykonanych procedur musi być większa od zera.");
            }

            if (this.StartDate > this.EndDate)
            {
                throw new InvalidInputException(
                    "Start date must be before or equal to end date",
                    "Data rozpoczęcia musi być wcześniejsza lub równa dacie zakończenia.");
            }

            return true;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    await ValidateInputsAsync();

                    var currentModule = await this.specializationService.GetCurrentModuleAsync();
                    if (currentModule == null)
                    {
                        throw new ResourceNotFoundException(
                            "Current module not found",
                            "Nie można określić bieżącego modułu.");
                    }

                    var procedureToSave = this.procedure ?? new RealizedProcedureNewSMK();

                    procedureToSave.CountA = this.CountA;
                    procedureToSave.CountB = this.CountB;
                    procedureToSave.StartDate = this.StartDate;
                    procedureToSave.EndDate = this.EndDate;
                    procedureToSave.ProcedureName = this.ProcedureName;
                    procedureToSave.ModuleId = currentModule.ModuleId;

                    if (!this.isEdit && !string.IsNullOrEmpty(this.requirementId) &&
                        int.TryParse(this.requirementId, out int reqId))
                    {
                        procedureToSave.ProcedureRequirementId = reqId;
                    }

                    bool success = await this.procedureService.SaveNewSMKProcedureAsync(procedureToSave);

                    if (success)
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            this.isEdit ? "Realizacja procedury została zaktualizowana." : "Realizacja procedury została dodana.",
                            "OK");

                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to save procedure realization",
                            "Nie udało się zapisać realizacji procedury.");
                    }
                }, "Wystąpił problem podczas zapisywania realizacji procedury.");
            }
            catch (InvalidInputException ex)
            {
                await this.dialogService.DisplayAlertAsync("Błąd walidacji", ex.UserFriendlyMessage, "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Wystąpił problem podczas anulowania edycji.");
        }
    }
}
```

## File: ViewModels/Procedures/AddEditOldSMKProcedureViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    [QueryProperty(nameof(ProcedureId), nameof(ProcedureId))]
    [QueryProperty(nameof(IsEditString), "IsEdit")]
    [QueryProperty(nameof(DateString), "Date")]
    [QueryProperty(nameof(YearString), "Year")]
    [QueryProperty(nameof(CodeString), "Code")]
    [QueryProperty(nameof(PerformingPerson), "PerformingPerson")]
    [QueryProperty(nameof(Location), "Location")]
    [QueryProperty(nameof(PatientInitials), "PatientInitials")]
    [QueryProperty(nameof(PatientGenderString), "PatientGender")]
    [QueryProperty(nameof(AssistantData), "AssistantData")]
    [QueryProperty(nameof(ProcedureGroup), "ProcedureGroup")]
    [QueryProperty(nameof(InternshipIdString), "InternshipId")]
    [QueryProperty(nameof(InternshipNameString), "InternshipName")]
    [QueryProperty(nameof(RequirementId), nameof(RequirementId))]
    public class AddEditOldSMKProcedureViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private bool isEdit;
        private string procedureId;
        private string requirementId;
        private RealizedProcedureOldSMK procedure;
        private ObservableCollection<KeyValuePair<string, string>> codeOptions;
        private ObservableCollection<KeyValuePair<string, string>> yearOptions;
        private ObservableCollection<KeyValuePair<string, string>> genderOptions;
        private ObservableCollection<Internship> availableInternships;
        private Internship selectedInternship;
        private KeyValuePair<string, string> selectedCode;
        private KeyValuePair<string, string> selectedYear;
        private KeyValuePair<string, string> selectedGender;
        private User currentUser;
        private bool isInitialized;

        private string isEditString;
        private string dateString;
        private string yearString;
        private string codeString;
        private string patientGenderString;
        private string internshipIdString;
        private string internshipNameString;
        private string performingPerson;
        private string location;
        private string patientInitials;
        private string assistantData;
        private string procedureGroup;
        private bool isInternshipSelectionEnabled;
        private string internshipSelectionHint;

        public AddEditOldSMKProcedureViewModel(
            IProcedureService procedureService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.procedureService = procedureService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.CodeOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.YearOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.GenderOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.AvailableInternships = new ObservableCollection<Internship>();

            this.Procedure = new RealizedProcedureOldSMK
            {
                Date = DateTime.Now,
                Code = "A - operator",
                PatientGender = "K",
                Year = 1,
                SyncStatus = SyncStatus.NotSynced,
                PerformingPerson = string.Empty,
            };

            this.PerformingPerson = string.Empty;
            this.Location = string.Empty;
            this.PatientInitials = string.Empty;
            this.AssistantData = string.Empty;
            this.ProcedureGroup = string.Empty;
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);
            this.isInitialized = false;
        }

        #region Query Property Setters

        // Query property setters remain unchanged
        public string IsEditString
        {
            set
            {
                this.isEditString = value;
                if (bool.TryParse(value, out bool result))
                {
                    this.IsEdit = result;
                    this.Title = this.IsEdit ? "Edytuj procedurę" : "Dodaj procedurę";
                }
            }
        }

        public string DateString
        {
            set
            {
                this.dateString = value;
                if (DateTime.TryParse(value, out DateTime result))
                {
                    this.Procedure.Date = result;
                }
            }
        }

        public string YearString
        {
            set
            {
                this.yearString = value;
                if (int.TryParse(value, out int result))
                {
                    this.Procedure.Year = result;
                }
            }
        }

        public string CodeString
        {
            set
            {
                this.codeString = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.Procedure.Code = value;
                }
            }
        }

        public string PatientGenderString
        {
            set
            {
                this.patientGenderString = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.Procedure.PatientGender = value;
                }
            }
        }

        public string InternshipIdString
        {
            set
            {
                this.internshipIdString = value;
                if (int.TryParse(value, out int result))
                {
                    this.Procedure.InternshipId = result;
                }
            }
        }

        public string InternshipNameString
        {
            set
            {
                this.internshipNameString = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.Procedure.InternshipName = value;
                }
            }
        }

        #endregion

        #region Properties

        public string ProcedureId
        {
            set
            {
                this.procedureId = value;
            }
        }

        public string RequirementId
        {
            set
            {
                this.requirementId = value;
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedProcedureOldSMK Procedure
        {
            get => this.procedure;
            set
            {
                if (this.SetProperty(ref this.procedure, value))
                {
                    this.SyncPropertiesFromProcedure();
                }
            }
        }

        public string PerformingPerson
        {
            get => this.performingPerson;
            set
            {
                if (this.SetProperty(ref this.performingPerson, value))
                {
                    this.Procedure.PerformingPerson = value;
                }
            }
        }

        public string Location
        {
            get => this.location;
            set
            {
                if (this.SetProperty(ref this.location, value))
                {
                    this.Procedure.Location = value;
                }
            }
        }

        public string PatientInitials
        {
            get => this.patientInitials;
            set
            {
                if (this.SetProperty(ref this.patientInitials, value))
                {
                    this.Procedure.PatientInitials = value;
                }
            }
        }

        public string AssistantData
        {
            get => this.assistantData;
            set
            {
                if (this.SetProperty(ref this.assistantData, value))
                {
                    this.Procedure.AssistantData = value;
                }
            }
        }

        public string ProcedureGroup
        {
            get => this.procedureGroup;
            set
            {
                if (this.SetProperty(ref this.procedureGroup, value))
                {
                    this.Procedure.ProcedureGroup = value;
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> CodeOptions
        {
            get => this.codeOptions;
            set => this.SetProperty(ref this.codeOptions, value);
        }

        public ObservableCollection<KeyValuePair<string, string>> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        public ObservableCollection<KeyValuePair<string, string>> GenderOptions
        {
            get => this.genderOptions;
            set => this.SetProperty(ref this.genderOptions, value);
        }

        public ObservableCollection<Internship> AvailableInternships
        {
            get => this.availableInternships;
            set => this.SetProperty(ref this.availableInternships, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set
            {
                if (this.SetProperty(ref this.selectedInternship, value) && value != null)
                {
                    this.Procedure.InternshipId = value.InternshipId;
                    this.Procedure.InternshipName = value.InternshipName;
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedCode
        {
            get => this.selectedCode;
            set
            {
                if (this.SetProperty(ref this.selectedCode, value))
                {
                    if (!string.IsNullOrEmpty(value.Key))
                    {
                        this.Procedure.Code = value.Key;
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value))
                {
                    if (!string.IsNullOrEmpty(value.Key) && int.TryParse(value.Key, out int year))
                    {
                        this.Procedure.Year = year;
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedGender
        {
            get => this.selectedGender;
            set
            {
                if (this.SetProperty(ref this.selectedGender, value))
                {
                    if (!string.IsNullOrEmpty(value.Key))
                    {
                        this.Procedure.PatientGender = value.Key;
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public User CurrentUser
        {
            get => this.currentUser;
            set => this.SetProperty(ref this.currentUser, value);
        }

        public bool IsInternshipSelectionEnabled
        {
            get => this.isInternshipSelectionEnabled;
            set => this.SetProperty(ref this.isInternshipSelectionEnabled, value);
        }

        public string InternshipSelectionHint
        {
            get => this.internshipSelectionHint;
            set => this.SetProperty(ref this.internshipSelectionHint, value);
        }

        #endregion

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void SyncPropertiesFromProcedure()
        {
            if (this.procedure != null)
            {
                this.PerformingPerson = this.procedure.PerformingPerson ?? string.Empty;
                this.Location = this.procedure.Location ?? string.Empty;
                this.PatientInitials = this.procedure.PatientInitials ?? string.Empty;
                this.AssistantData = this.procedure.AssistantData ?? string.Empty;
                this.ProcedureGroup = this.procedure.ProcedureGroup ?? string.Empty;
            }
        }

        public async Task InitializeAsync()
        {
            if (this.IsBusy || this.isInitialized)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                this.CurrentUser = await this.authService.GetCurrentUserAsync();
                if (this.CurrentUser == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono aktywnego użytkownika.");
                }

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                if (this.IsEdit && this.Procedure.ProcedureId == 0)
                {
                    int procId = 0;
                    if (!string.IsNullOrEmpty(this.procedureId) && int.TryParse(this.procedureId, out procId) && procId > 0)
                    {
                        await this.LoadProcedureAsync(procId);
                    }
                }
                else if (!this.IsEdit)
                {
                    if (this.CurrentUser != null && string.IsNullOrEmpty(this.Procedure.PerformingPerson))
                    {
                        this.Procedure.PerformingPerson = this.CurrentUser.Name;
                        this.PerformingPerson = this.CurrentUser.Name;
                    }
                }

                this.LoadDropdownOptions(specialization?.DurationYears ?? 6);
                await this.LoadInternshipsAsync();
                this.SynchronizePickersWithProcedure();

                if (!this.IsEdit && !string.IsNullOrEmpty(this.requirementId) && int.TryParse(this.requirementId, out int reqId))
                {
                    await this.LoadRequirementDataAsync(reqId);
                }

                if (!this.IsEdit && string.IsNullOrEmpty(this.Procedure.Location))
                {
                    await this.LoadLastLocationAsync();
                }

                this.SyncPropertiesFromProcedure();
                ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                this.isInitialized = true;
            }, "Wystąpił problem podczas inicjalizacji formularza procedury.");

            this.IsBusy = false;
        }

        private void LoadDropdownOptions(int yearsFromSpecialization)
        {
            this.CodeOptions.Clear();
            this.CodeOptions.Add(new KeyValuePair<string, string>("A - operator", "A - operator"));
            this.CodeOptions.Add(new KeyValuePair<string, string>("B - asysta", "B - asysta"));

            this.GenderOptions.Clear();
            this.GenderOptions.Add(new KeyValuePair<string, string>("K", "K"));
            this.GenderOptions.Add(new KeyValuePair<string, string>("M", "M"));

            this.YearOptions.Clear();
            for (int i = 1; i <= yearsFromSpecialization; i++)
            {
                this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
            }
        }

        private void SynchronizePickersWithProcedure()
        {
            var codeItem = this.CodeOptions.FirstOrDefault(c => c.Key == this.Procedure.Code);
            if (codeItem.Key != null)
            {
                this.SelectedCode = codeItem;
            }
            else if (this.CodeOptions.Count > 0)
            {
                this.SelectedCode = this.CodeOptions.First();
            }

            var yearItem = this.YearOptions.FirstOrDefault(y => y.Key == this.Procedure.Year.ToString());
            if (yearItem.Key != null)
            {
                this.SelectedYear = yearItem;
            }
            else if (this.YearOptions.Count > 0)
            {
                this.SelectedYear = this.YearOptions.First();
            }

            var genderItem = this.GenderOptions.FirstOrDefault(g => g.Key == this.Procedure.PatientGender);
            if (genderItem.Key != null)
            {
                this.SelectedGender = genderItem;
            }
            else if (this.GenderOptions.Count > 0)
            {
                this.SelectedGender = this.GenderOptions.First();
            }

            if (this.Procedure.InternshipId > 0)
            {
                var internship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == this.Procedure.InternshipId);
                if (internship != null)
                {
                    this.SelectedInternship = internship;
                }
                else if (this.AvailableInternships.Count > 0)
                {
                    this.SelectedInternship = this.AvailableInternships.First();
                }
            }
            else if (this.AvailableInternships.Count > 0)
            {
                this.SelectedInternship = this.AvailableInternships.First();
            }
        }

        private async Task LoadInternshipsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule == null)
                {
                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktualnego modułu specjalizacji.");
                }

                var internships = await this.specializationService.GetInternshipsAsync(moduleId: currentModule?.ModuleId);

                if (this.CurrentUser.SmkVersion == SmkVersion.Old)
                {
                    bool isBasicModule = currentModule?.Type == ModuleType.Basic;
                    internships = internships.Where(i =>
                        (isBasicModule && i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)) ||
                        (!isBasicModule && !i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                this.AvailableInternships.Clear();
                foreach (var internship in internships)
                {
                    this.AvailableInternships.Add(internship);
                }

                if (this.AvailableInternships.Count == 0)
                {
                    throw new BusinessRuleViolationException(
                        "No internships available",
                        "Nie znaleziono dostępnych staży. Dodaj staż przed dodaniem procedury.");
                }
            }, "Wystąpił problem podczas wczytywania dostępnych staży.");
        }

        private async Task LoadRequirementDataAsync(int reqId)
        {
            await SafeExecuteAsync(async () =>
            {
                var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync();
                var requirement = requirements.FirstOrDefault(r => r.Id == reqId);

                if (requirement != null)
                {
                    if (this.CurrentUser.SmkVersion == SmkVersion.New)
                    {
                        if (requirement.InternshipId.HasValue)
                        {
                            var internship = this.AvailableInternships.FirstOrDefault(i =>
                                i.InternshipId == requirement.InternshipId.Value);
                            if (internship != null)
                            {
                                this.SelectedInternship = internship;
                                this.IsInternshipSelectionEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        bool isBasicInternship = requirement.Type.Contains("podstawowy", StringComparison.OrdinalIgnoreCase);
                        var matchingInternship = this.AvailableInternships.FirstOrDefault(i =>
                            (isBasicInternship && i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)) ||
                            (!isBasicInternship && i.InternshipName.Contains(requirement.Type, StringComparison.OrdinalIgnoreCase)));

                        if (matchingInternship != null)
                        {
                            this.SelectedInternship = matchingInternship;
                            this.IsInternshipSelectionEnabled = false;
                        }
                        else
                        {
                            var filteredInternships = this.AvailableInternships.Where(i =>
                                (isBasicInternship && i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)) ||
                                (!isBasicInternship && !i.InternshipName.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            this.AvailableInternships.Clear();
                            foreach (var internship in filteredInternships)
                            {
                                this.AvailableInternships.Add(internship);
                            }

                            this.IsInternshipSelectionEnabled = true;
                        }
                    }

                    this.InternshipSelectionHint = this.IsInternshipSelectionEnabled
                        ? "Wybierz staż z listy"
                        : "Staż jest przypisany automatycznie do tej procedury";
                }
                else
                {
                    throw new ResourceNotFoundException(
                        $"Requirement with ID {reqId} not found",
                        "Nie znaleziono wymagania procedurowego o podanym identyfikatorze.");
                }
            }, "Wystąpił problem podczas wczytywania danych wymagania procedurowego.");
        }

        private async Task LoadProcedureAsync(int procedureId)
        {
            await SafeExecuteAsync(async () =>
            {
                var loadedProcedure = await this.procedureService.GetOldSMKProcedureAsync(procedureId);
                if (loadedProcedure != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj procedurę";
                    this.Procedure = loadedProcedure;
                    this.PerformingPerson = loadedProcedure.PerformingPerson ?? string.Empty;
                    this.Location = loadedProcedure.Location ?? string.Empty;
                    this.PatientInitials = loadedProcedure.PatientInitials ?? string.Empty;
                    this.AssistantData = loadedProcedure.AssistantData ?? string.Empty;
                    this.ProcedureGroup = loadedProcedure.ProcedureGroup ?? string.Empty;
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj procedurę";
                    throw new ResourceNotFoundException(
                        $"Procedure with ID {procedureId} not found",
                        "Nie znaleziono procedury o podanym identyfikatorze.");
                }
            }, "Wystąpił problem podczas wczytywania procedury.");
        }

        private async Task LoadLastLocationAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono danych użytkownika.");
                }

                var lastProcedures = await this.procedureService.GetOldSMKProceduresAsync();

                if (lastProcedures.Count > 0)
                {
                    this.Procedure.Location = lastProcedures[0].Location;
                    this.Location = lastProcedures[0].Location;
                }
            }, "Wystąpił problem podczas wczytywania danych ostatniej lokalizacji.");
        }

        private async Task<bool> ValidateInputsAsync()
        {
            if (this.Procedure == null)
            {
                return false;
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(this.Procedure.Code))
            {
                throw new InvalidInputException("Code is required", "Kod procedury jest wymagany.");
            }

            if (string.IsNullOrWhiteSpace(this.Location))
            {
                throw new InvalidInputException("Location is required", "Lokalizacja jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.PerformingPerson))
            {
                throw new InvalidInputException("Performing person is required", "Osoba wykonująca jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.PatientInitials))
            {
                throw new InvalidInputException("Patient initials are required", "Inicjały pacjenta są wymagane.");
            }

            if (string.IsNullOrWhiteSpace(this.Procedure.PatientGender))
            {
                throw new InvalidInputException("Patient gender is required", "Płeć pacjenta jest wymagana.");
            }

            if (this.SelectedInternship == null)
            {
                throw new InvalidInputException("Internship is required", "Wybór stażu jest wymagany.");
            }

            return true;
        }

        public async Task OnSaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    await ValidateInputsAsync();

                    this.Procedure.PerformingPerson = this.PerformingPerson;
                    this.Procedure.Location = this.Location;
                    this.Procedure.PatientInitials = this.PatientInitials;
                    this.Procedure.AssistantData = this.AssistantData;
                    this.Procedure.ProcedureGroup = this.ProcedureGroup;

                    if (this.Procedure.SpecializationId <= 0 && this.CurrentUser != null)
                    {
                        this.Procedure.SpecializationId = this.CurrentUser.SpecializationId;
                    }

                    this.Procedure.InternshipId = this.SelectedInternship.InternshipId;
                    this.Procedure.InternshipName = this.SelectedInternship.InternshipName;

                    if (!string.IsNullOrEmpty(this.requirementId) && int.TryParse(this.requirementId, out int reqId))
                    {
                        this.Procedure.ProcedureRequirementId = reqId;
                    }

                    bool success = await this.procedureService.SaveOldSMKProcedureAsync(this.Procedure);

                    if (success)
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            this.IsEdit ? "Procedura została zaktualizowana." : "Procedura została dodana.",
                            "OK");

                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to save procedure",
                            "Nie udało się zapisać procedury. Sprawdź poprawność danych.");
                    }
                }, "Wystąpił problem podczas zapisywania procedury.");
            }
            catch (InvalidInputException ex)
            {
                await this.dialogService.DisplayAlertAsync("Błąd walidacji", ex.UserFriendlyMessage, "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Wystąpił problem podczas anulowania edycji procedury.");
        }
    }
}
```

## File: ViewModels/Procedures/NewSMKProceduresListViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class NewSMKProceduresListViewModel : BaseViewModel, IDisposable
    {
        private readonly IProcedureService procedureService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<ProcedureRequirementViewModel> procedureRequirements;
        private ProcedureSummary summary;
        private bool isRefreshing;
        private string moduleTitle;
        private bool hasTwoModules;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private int currentModuleId;
        private List<ProcedureRequirement> allRequirements;
        private bool isLoadingData;
        private int batchSize = 3;
        private bool isLoading;

        public NewSMKProceduresListViewModel(
            IProcedureService procedureService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.procedureService = procedureService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Procedury (Nowy SMK)";
            this.ProcedureRequirements = new ObservableCollection<ProcedureRequirementViewModel>();
            this.Summary = new ProcedureSummary();
            this.allRequirements = new List<ProcedureRequirement>();
            this.isLoadingData = false;

            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.LoadMoreCommand = new AsyncRelayCommand(this.LoadMoreItemsAsync);
            this.specializationService.CurrentModuleChanged += this.OnModuleChanged;

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<ProcedureRequirementViewModel> ProcedureRequirements
        {
            get => this.procedureRequirements;
            set => this.SetProperty(ref this.procedureRequirements, value);
        }

        public ProcedureSummary Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                if (this.SetProperty(ref this.currentModuleId, value))
                {
                    this.LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }
        public ICommand LoadMoreCommand { get; }

        private async void OnModuleChanged(object sender, int moduleId)
        {
            this.CurrentModuleId = moduleId;
            await this.LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);
                var currentModule = await this.specializationService.GetCurrentModuleAsync();

                if (currentModule == null)
                {
                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktualnego modułu.");
                }

                this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
                this.ModuleTitle = currentModule.Name;

                var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync(currentModule.ModuleId);
                this.ProcedureRequirements.Clear();

                for (int i = 0; i < requirements.Count; i++)
                {
                    var requirement = requirements[i];
                    var stats = await this.procedureService.GetProcedureSummaryAsync(currentModule.ModuleId, requirement.Id);

                    var viewModel = new ProcedureRequirementViewModel(
                        requirement,
                        stats,
                        new List<RealizedProcedureNewSMK>(),
                        i + 1,
                        currentModule.ModuleId,
                        this.procedureService,
                        this.dialogService,
                        this.ExceptionHandler);

                    this.ProcedureRequirements.Add(viewModel);
                }
            }, "Wystąpił problem podczas ładowania danych procedur.");

            this.IsBusy = false;
        }

        public async Task LoadMoreItemsAsync()
        {
            if (this.isLoadingData || this.allRequirements == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                this.isLoadingData = true;

                int currentCount = this.ProcedureRequirements.Count;

                if (currentCount >= this.allRequirements.Count)
                {
                    return;
                }

                int itemsToLoad = Math.Min(this.batchSize, this.allRequirements.Count - currentCount);
                var requirementsToLoad = this.allRequirements.Skip(currentCount).Take(itemsToLoad).ToList();

                for (int i = 0; i < requirementsToLoad.Count; i++)
                {
                    var requirement = requirementsToLoad[i];
                    var stats = await this.procedureService.GetProcedureSummaryAsync(
                        this.CurrentModuleId, requirement.Id);

                    var viewModel = new ProcedureRequirementViewModel(
                        requirement,
                        stats,
                        new List<RealizedProcedureNewSMK>(),
                        currentCount + i + 1,
                        this.CurrentModuleId,
                        this.procedureService,
                        this.dialogService,
                        this.ExceptionHandler);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.ProcedureRequirements.Add(viewModel);
                    });

                    await Task.Delay(50);
                }
            }, "Wystąpił problem podczas ładowania dodatkowych procedur.");

            this.isLoadingData = false;
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                        this.CurrentModuleId = basicModule.ModuleId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Basic module not found",
                            "Nie znaleziono modułu podstawowego.");
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.CurrentModuleId = specialisticModule.ModuleId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Specialistic module not found",
                            "Nie znaleziono modułu specjalistycznego.");
                    }
                }
            }, $"Wystąpił problem podczas przełączania modułu na {moduleType}.");
        }

        public void Dispose()
        {
            this.specializationService.CurrentModuleChanged -= this.OnModuleChanged;
        }
    }
}
```

## File: ViewModels/Procedures/OldSMKProceduresListViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class OldSMKProceduresListViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<ProcedureGroupViewModel> procedureGroups;
        private ProcedureSummary summary;
        private bool isRefreshing;
        private string moduleTitle;
        private bool hasTwoModules;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private int currentModuleId;
        private bool isInitialLoad = true;
        private bool isLoadingData = false;

        public OldSMKProceduresListViewModel(
            IProcedureService procedureService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.procedureService = procedureService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Procedury (Stary SMK)";
            this.ProcedureGroups = new ObservableCollection<ProcedureGroupViewModel>();
            this.Summary = new ProcedureSummary();

            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.AddProcedureCommand = new AsyncRelayCommand(this.AddProcedureAsync);
            this.specializationService.CurrentModuleChanged += this.OnModuleChanged;
        }

        public ObservableCollection<ProcedureGroupViewModel> ProcedureGroups
        {
            get => this.procedureGroups;
            set => this.SetProperty(ref this.procedureGroups, value);
        }

        public ProcedureSummary Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                if (this.SetProperty(ref this.currentModuleId, value))
                {
                    this.LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }
        public ICommand AddProcedureCommand { get; }

        private async void OnModuleChanged(object sender, int moduleId)
        {
            this.CurrentModuleId = moduleId;
            await this.LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy || this.isLoadingData)
            {
                return;
            }

            this.isLoadingData = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);

                if (this.CurrentModuleId == 0 && specialization.CurrentModuleId.HasValue)
                {
                    this.CurrentModuleId = specialization.CurrentModuleId.Value;
                }

                var currentModule = modules.FirstOrDefault(m => m.ModuleId == this.CurrentModuleId)
                                        ?? modules.FirstOrDefault();

                if (currentModule != null)
                {
                    this.ModuleTitle = currentModule.Name;
                    this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                    this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
                    this.Summary = await this.procedureService.GetProcedureSummaryAsync(currentModule.ModuleId);

                    var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync(currentModule.ModuleId);
                    var newGroups = new List<ProcedureGroupViewModel>();

                    foreach (var requirement in requirements)
                    {
                        var stats = await this.procedureService.GetProcedureSummaryAsync(currentModule.ModuleId, requirement.Id);
                        var groupViewModel = new ProcedureGroupViewModel(
                            requirement,
                            new List<RealizedProcedureOldSMK>(),
                            stats,
                            this.procedureService,
                            this.dialogService,
                            ExceptionHandler);

                        newGroups.Add(groupViewModel);
                    }

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.ProcedureGroups.Clear();

                        foreach (var group in newGroups)
                        {
                            this.ProcedureGroups.Add(group);
                        }
                    });
                }
                else
                {
                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktualnego modułu specjalizacji.");
                }
            }, "Wystąpił problem podczas ładowania danych procedur.");

            this.isLoadingData = false;
            this.IsBusy = false;
            this.IsRefreshing = false;
            this.isInitialLoad = false;
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                        this.CurrentModuleId = basicModule.ModuleId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Basic module not found",
                            "Nie znaleziono modułu podstawowego.");
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.CurrentModuleId = specialisticModule.ModuleId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Specialistic module not found",
                            "Nie znaleziono modułu specjalistycznego.");
                    }
                }
            }, $"Wystąpił problem podczas przełączania modułu na {moduleType}.");
        }

        private async Task AddProcedureAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("AddEditOldSMKProcedure");
            }, "Wystąpił problem podczas przejścia do dodawania procedury.");
        }

        public void Dispose()
        {
            this.specializationService.CurrentModuleChanged -= this.OnModuleChanged;
        }
    }
}
```

## File: ViewModels/Procedures/ProcedureGroupViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureGroupViewModel : ObservableObject
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;
        private readonly IExceptionHandlerService exceptionHandler;

        private ProcedureRequirement requirement;
        private ProcedureSummary statistics;
        private ObservableCollection<RealizedProcedureOldSMK> procedures;
        private RealizedProcedureOldSMK selectedProcedure;
        private bool isExpanded;
        private bool isLoading;
        private bool hasLoadedData;

        public ProcedureGroupViewModel(
            ProcedureRequirement requirement,
            List<RealizedProcedureOldSMK> procedures,
            ProcedureSummary statistics,
            IProcedureService procedureService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler = null)
        {
            this.requirement = requirement;
            this.statistics = statistics;
            this.procedures = new ObservableCollection<RealizedProcedureOldSMK>(procedures);
            this.procedureService = procedureService;
            this.dialogService = dialogService;
            this.exceptionHandler = exceptionHandler;
            this.hasLoadedData = false;
            this.isLoading = false;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.EditProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnEditProcedure);
            this.DeleteProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnDeleteProcedure);
            this.AddProcedureCommand = new AsyncRelayCommand(this.OnAddProcedure);
            this.SelectProcedureCommand = new RelayCommand<RealizedProcedureOldSMK>(this.OnSelectProcedure);
            this.procedures.CollectionChanged += this.Procedures_CollectionChanged;
        }

        public ProcedureRequirement Requirement => this.requirement;

        public string Title => this.requirement?.Name ?? "Nieznana procedura";

        public string StatsInfo => $"{this.statistics.CompletedCountA}/{this.statistics.RequiredCountA} (A), " +
                                   $"{this.statistics.CompletedCountB}/{this.statistics.RequiredCountB} (B)";

        public string ApprovedInfo => $"{this.statistics.ApprovedCountA}/{this.statistics.CompletedCountA} (A), " +
                                      $"{this.statistics.ApprovedCountB}/{this.statistics.CompletedCountB} (B)";

        public ObservableCollection<RealizedProcedureOldSMK> Procedures
        {
            get => this.procedures;
            set => this.SetProperty(ref this.procedures, value);
        }

        public RealizedProcedureOldSMK SelectedProcedure
        {
            get => this.selectedProcedure;
            set
            {
                if (this.SetProperty(ref this.selectedProcedure, value))
                {
                    this.OnPropertyChanged(nameof(this.SelectedProcedure));
                }
            }
        }

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand EditProcedureCommand { get; }
        public ICommand DeleteProcedureCommand { get; }
        public ICommand AddProcedureCommand { get; }
        public ICommand SelectProcedureCommand { get; }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && this.procedures.Count > 0 && this.selectedProcedure == null)
            {
                this.SelectedProcedure = this.procedures[0];
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && this.selectedProcedure != null)
            {
                if (!this.procedures.Contains(this.selectedProcedure))
                {
                    this.SelectedProcedure = this.procedures.Count > 0 ? this.procedures[0] : null;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.SelectedProcedure = this.procedures.Count > 0 ? this.procedures[0] : null;
            }
        }

        private void OnSelectProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure != null)
            {
                this.SelectedProcedure = procedure;
            }
        }

        private async Task OnToggleExpandAsync()
        {
            if (this.isExpanded)
            {
                this.IsExpanded = false;
                return;
            }

            if (!this.hasLoadedData && !this.isLoading)
            {
                this.isLoading = true;
                this.IsExpanded = true;

                await SafeExecuteAsync(async () =>
                {
                    var relatedProcedures = await this.procedureService.GetOldSMKProceduresAsync(
                        requirementId: this.requirement.Id);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.Procedures.Clear();
                        foreach (var procedure in relatedProcedures)
                        {
                            this.Procedures.Add(procedure);
                        }

                        if (this.Procedures.Count > 0 && this.SelectedProcedure == null)
                        {
                            this.SelectedProcedure = this.Procedures[0];
                        }

                        this.hasLoadedData = true;
                        this.isLoading = false;
                        this.OnPropertyChanged(nameof(this.IsLoading));
                    });
                }, "Wystąpił problem podczas ładowania procedur.");
            }
            else
            {
                this.IsExpanded = !this.isExpanded;
            }
        }

        private async Task OnEditProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", procedure.ProcedureId.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas przejścia do edycji procedury.");
        }

        public async Task OnDeleteProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę procedurę?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool result = await this.procedureService.DeleteOldSMKProcedureAsync(procedure.ProcedureId);

                    if (result)
                    {
                        if (this.SelectedProcedure == procedure)
                        {
                            int index = this.Procedures.IndexOf(procedure);
                            if (index >= 0 && this.Procedures.Count > 1)
                            {
                                int newIndex = Math.Min(index, this.Procedures.Count - 2);
                                this.SelectedProcedure = this.Procedures[newIndex];
                            }
                            else
                            {
                                this.SelectedProcedure = null;
                            }
                        }

                        this.Procedures.Remove(procedure);

                        if (procedure.Code == "A - operator")
                        {
                            this.statistics.CompletedCountA--;
                        }
                        else if (procedure.Code == "B - asysta")
                        {
                            this.statistics.CompletedCountB--;
                        }

                        this.OnPropertyChanged(nameof(this.StatsInfo));
                        this.OnPropertyChanged(nameof(this.ApprovedInfo));
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to delete procedure",
                            "Nie udało się usunąć procedury.");
                    }
                }
            }, "Wystąpił problem podczas usuwania procedury.");
        }

        private async Task OnAddProcedure()
        {
            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas przejścia do dodawania procedury.");
        }

        // Helper method for error handling
        private async Task SafeExecuteAsync(Func<Task> operation, string errorMessage)
        {
            if (this.exceptionHandler != null)
            {
                await this.exceptionHandler.ExecuteAsync(operation, null, errorMessage);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync("Błąd", errorMessage, "OK");
                }
            }
        }
    }
}
```

## File: ViewModels/Procedures/ProcedureRequirementViewModel.cs
```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureRequirementViewModel : ObservableObject
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;
        private readonly IExceptionHandlerService exceptionHandler;

        private ProcedureRequirement requirement;
        private ProcedureSummary statistics;
        private ObservableCollection<RealizedProcedureNewSMK> realizations;
        private int index;
        private int? moduleId;
        private bool isExpanded;
        private bool isAddingRealization;
        private string internshipName;
        private bool isLoading;
        private bool hasLoadedData;

        public ProcedureRequirementViewModel(
            ProcedureRequirement requirement,
            ProcedureSummary statistics,
            List<RealizedProcedureNewSMK> realizations,
            int index,
            int? moduleId,
            IProcedureService procedureService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler = null,
            string internshipName = "")
        {
            this.requirement = requirement;
            this.statistics = statistics;
            this.realizations = new ObservableCollection<RealizedProcedureNewSMK>(realizations);
            this.index = index;
            this.moduleId = moduleId;
            this.procedureService = procedureService;
            this.dialogService = dialogService;
            this.exceptionHandler = exceptionHandler;
            this.internshipName = internshipName;
            this.isLoading = false;
            this.hasLoadedData = false;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.ToggleAddRealizationCommand = new AsyncRelayCommand(this.OnToggleAddRealizationAsync);
            this.EditRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnEditRealization);
            this.DeleteRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnDeleteRealization);

            this.LoadRealizationsAsync().ConfigureAwait(false);
        }

        public ProcedureRequirement Requirement => this.requirement;

        public string Title => $"{this.index}. {this.requirement?.Name ?? "Nieznana procedura"}";

        public string InternshipName => this.internshipName;

        public ProcedureSummary Statistics => this.statistics;

        public ObservableCollection<RealizedProcedureNewSMK> Realizations
        {
            get => this.realizations;
            set => this.SetProperty(ref this.realizations, value);
        }

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public bool IsAddingRealization
        {
            get => this.isAddingRealization;
            set => this.SetProperty(ref this.isAddingRealization, value);
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public bool HasRealizations => this.Realizations != null && this.Realizations.Any();

        public ICommand ToggleExpandCommand { get; }
        public ICommand ToggleAddRealizationCommand { get; }
        public ICommand EditRealizationCommand { get; }
        public ICommand DeleteRealizationCommand { get; }

        private async Task LoadRealizationsAsync()
        {
            if (this.hasLoadedData || this.IsLoading)
            {
                return;
            }

            this.IsLoading = true;

            await SafeExecuteAsync(async () =>
            {
                var realizations = await this.procedureService.GetNewSMKProceduresAsync(
                    this.moduleId,
                    this.requirement.Id);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.Realizations.Clear();
                    foreach (var realization in realizations)
                    {
                        this.Realizations.Add(realization);
                    }
                    this.hasLoadedData = true;
                });

                this.OnPropertyChanged(nameof(this.Realizations));
                this.OnPropertyChanged(nameof(this.HasRealizations));
                this.OnPropertyChanged(nameof(this.Statistics));
            }, "Wystąpił problem podczas ładowania realizacji procedury.");

            this.IsLoading = false;
        }

        private async Task OnToggleExpandAsync()
        {
            if (this.isExpanded)
            {
                this.IsExpanded = false;
                return;
            }

            if (!this.hasLoadedData && !this.isLoading)
            {
                this.isLoading = true;
                this.IsExpanded = true;

                await SafeExecuteAsync(async () =>
                {
                    var realizations = await this.procedureService.GetNewSMKProceduresAsync(
                        this.moduleId,
                        this.requirement.Id);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.Realizations.Clear();
                        foreach (var realization in realizations)
                        {
                            this.Realizations.Add(realization);
                        }

                        this.OnPropertyChanged(nameof(this.Realizations));
                        this.OnPropertyChanged(nameof(this.HasRealizations));
                        this.OnPropertyChanged(nameof(this.Statistics));

                        this.hasLoadedData = true;
                        this.isLoading = false;
                        this.OnPropertyChanged(nameof(this.IsLoading));
                    });
                }, "Wystąpił problem podczas ładowania realizacji procedury.");
            }
            else
            {
                this.IsExpanded = !this.isExpanded;
            }
        }

        private async Task OnToggleAddRealizationAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditNewSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas nawigacji do formularza dodawania procedury.");
        }

        private async Task OnEditRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                if (realization.SyncStatus == SyncStatus.Synced)
                {
                    throw new BusinessRuleViolationException(
                        "Cannot edit synced realization",
                        "Nie można edytować zsynchronizowanej realizacji.");
                }

                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", realization.ProcedureId.ToString() },
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditNewSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas edycji realizacji procedury.");
        }

        private async Task OnDeleteRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                if (realization.SyncStatus == SyncStatus.Synced)
                {
                    throw new BusinessRuleViolationException(
                        "Cannot delete synced realization",
                        "Nie można usunąć zsynchronizowanej realizacji.");
                }

                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę realizację?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool success = await this.procedureService.DeleteNewSMKProcedureAsync(realization.ProcedureId);

                    if (success)
                    {
                        this.statistics.CompletedCountA -= realization.CountA;
                        this.statistics.CompletedCountB -= realization.CountB;
                        this.Realizations.Remove(realization);
                        this.OnPropertyChanged(nameof(this.HasRealizations));
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to delete realization",
                            "Nie udało się usunąć realizacji.");
                    }
                }
            }, "Wystąpił problem podczas usuwania realizacji procedury.");
        }

        // Helper method for error handling
        private async Task SafeExecuteAsync(Func<Task> operation, string errorMessage)
        {
            if (this.exceptionHandler != null)
            {
                await this.exceptionHandler.ExecuteAsync(operation, null, errorMessage);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync("Błąd", errorMessage, "OK");
                }
            }
        }
    }
}
```

## File: ViewModels/Procedures/ProcedureSelectorViewModel.cs
```csharp
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureSelectorViewModel : BaseViewModel
    {
        private readonly IAuthService authService;

        public ProcedureSelectorViewModel(
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.authService = authService;
            this.Title = "Procedury";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        public async Task InitializeAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();

                if (user != null)
                {
                    if (user.SmkVersion == SmkVersion.Old)
                    {
                        await Shell.Current.GoToAsync("OldSMKProcedures");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("NewSMKProcedures");
                    }
                }
            }, "Wystąpił problem podczas inicjalizacji widoku procedur.");
        }

        private async Task NavigateToOldSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("OldSMKProcedures");
            }, "Wystąpił problem podczas nawigacji do widoku procedur (Stary SMK).");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("NewSMKProcedures");
            }, "Wystąpił problem podczas nawigacji do widoku procedur (Nowy SMK).");
        }
    }
}
```

## File: Views/Authentication/LoginPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    x:Class="SledzSpecke.App.Views.Authentication.LoginPage"
    Title="Logowanie">

    <Grid RowDefinitions="Auto, *, Auto" Padding="20" VerticalOptions="FillAndExpand">
        <VerticalStackLayout Grid.Row="0" Spacing="10" Margin="0,20,0,40">
            <Image Source="app_logo.png" HeightRequest="120" WidthRequest="120" HorizontalOptions="Center"/>
            <Label Text="ŚledzSpecke" FontSize="24" FontAttributes="Bold" HorizontalOptions="Center"/>
            <Label Text="Śledź swoją specjalizację medyczną" FontSize="18" HorizontalOptions="Center"/>
        </VerticalStackLayout>
        <VerticalStackLayout Grid.Row="1" Spacing="20" VerticalOptions="Center">
            <Entry Placeholder="Nazwa użytkownika" Text="{Binding Username}" BackgroundColor="Transparent"/>
            <Entry Placeholder="Hasło" Text="{Binding Password}" IsPassword="True" BackgroundColor="Transparent"/>
            <HorizontalStackLayout Spacing="10">
                <CheckBox IsChecked="{Binding RememberMe}" />
                <Label Text="Zapamiętaj mnie" VerticalOptions="Center" />
            </HorizontalStackLayout>
            <Button Text="Zaloguj się" Command="{Binding LoginCommand}" 
                    Style="{StaticResource PrimaryButton}" Margin="0,20,0,0"/>
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Row="2" Spacing="10" Margin="0,20,0,0">
            <Label Text="Nie masz jeszcze konta?" HorizontalOptions="Center"/>
            <Button Text="Zarejestruj się" Command="{Binding GoToRegisterCommand}" 
                    Style="{StaticResource OutlineButton}"/>
            <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" 
                              HorizontalOptions="Center" Margin="0,10"/>
        </VerticalStackLayout>
    </Grid>
</ContentPage>
```

## File: Views/Authentication/LoginPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Authentication;

namespace SledzSpecke.App.Views.Authentication
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}
```

## File: Views/Authentication/RegisterPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    x:Class="SledzSpecke.App.Views.Authentication.RegisterPage"
    Title="Rejestracja">
    <ScrollView>
        <Grid RowDefinitions="Auto, *, Auto" Padding="20" VerticalOptions="FillAndExpand">
            <VerticalStackLayout Grid.Row="0" Spacing="10" Margin="0,20,0,20">
                <Image Source="app_logo.png" HeightRequest="80" WidthRequest="80" HorizontalOptions="Center"/>
                <Label Text="Rejestracja" FontSize="24" FontAttributes="Bold" HorizontalOptions="Center"/>
            </VerticalStackLayout>
            <VerticalStackLayout Grid.Row="1" Spacing="15" VerticalOptions="Center">
                <Label Text="Informacje osobiste" FontSize="18" FontAttributes="Bold"/>
                <Entry Placeholder="Nazwa użytkownika *" Text="{Binding Username}" BackgroundColor="Transparent"/>
                <Entry Placeholder="Email *" Text="{Binding Email}" Keyboard="Email" BackgroundColor="Transparent"/>
                <Entry Placeholder="Hasło *" Text="{Binding Password}" IsPassword="True" BackgroundColor="Transparent"/>
                <Entry Placeholder="Potwierdź hasło *" Text="{Binding ConfirmPassword}" IsPassword="True" BackgroundColor="Transparent"/>
                <Label Text="Hasła nie pasują do siebie! *" IsVisible="{Binding PasswordsNotMatch}" TextColor="Red"/>
                <Entry Placeholder="Imię i nazwisko (do wpisywania procedur) *" Text="{Binding Name}" BackgroundColor="Transparent"/>
                <Label Text="Wybierz wersję SMK" FontSize="18" FontAttributes="Bold" Margin="0,10,0,0"/>
                <HorizontalStackLayout Spacing="20">
                    <VerticalStackLayout Spacing="10">
                        <CheckBox IsChecked="{Binding IsNewSmkVersion}" />
                        <Label Text="Nowy SMK (CMKP 2023)" VerticalOptions="Center" />
                    </VerticalStackLayout>
                    <VerticalStackLayout Spacing="10">
                        <CheckBox IsChecked="{Binding IsOldSmkVersion}" />
                        <Label Text="Stary SMK (CMKP 2018)" VerticalOptions="Center" />
                    </VerticalStackLayout>
                </HorizontalStackLayout>
                <Label Text="Wybierz specjalizację" FontSize="18" FontAttributes="Bold" Margin="0,10,0,0"/>
                <Picker ItemsSource="{Binding AvailableSpecializations}" 
                        SelectedItem="{Binding SelectedSpecialization}"
                        ItemDisplayBinding="{Binding Name}" 
                        Title="Wybierz specjalizację"
                        BackgroundColor="Transparent"/>
                <Button Text="Zarejestruj się" Command="{Binding RegisterCommand}" 
                        Style="{StaticResource PrimaryButton}" Margin="0,20,0,0"/>
            </VerticalStackLayout>
            <VerticalStackLayout Grid.Row="2" Spacing="10" Margin="0,20,0,0">
                <Label Text="Masz już konto?" HorizontalOptions="Center"/>
                <Button Text="Zaloguj się" Command="{Binding GoToLoginCommand}" 
                        Style="{StaticResource OutlineButton}"/>
                <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" 
                                  HorizontalOptions="Center" Margin="0,10"/>
            </VerticalStackLayout>
        </Grid>
    </ScrollView>
</ContentPage>
```

## File: Views/Authentication/RegisterPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Authentication;

namespace SledzSpecke.App.Views.Authentication
{
    public partial class RegisterPage : ContentPage
    {
        private readonly RegisterViewModel viewModel;

        public RegisterPage(RegisterViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await this.viewModel.InitializeAsync();
        }
    }
}
```

## File: Views/Dashboard/DashboardPage.xaml
```
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="SledzSpecke.App.Views.Dashboard.DashboardPage"
             Title="SledzSpecke">
    <RefreshView IsRefreshing="{Binding IsBusy}" Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Padding="20" Spacing="20">
                <StackLayout IsVisible="{Binding HasTwoModules}" Orientation="Horizontal" 
                             HorizontalOptions="Center" Margin="0,10" Spacing="10">
                    <Label Text="Aktywny moduł:" VerticalOptions="Center" FontAttributes="Bold" />
                    <Frame Padding="0" CornerRadius="5" BorderColor="{StaticResource Primary}" 
                           HasShadow="False" IsClippedToBounds="True">
                        <Grid ColumnDefinitions="*,*">
                            <Button Text="Podstawowy" 
                                    BackgroundColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Basic" />
                            <Button Grid.Column="1" Text="Specjalistyczny" 
                                    BackgroundColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Specialistic" />
                        </Grid>
                    </Frame>
                </StackLayout>
                <Frame BorderColor="LightGray" HasShadow="True" CornerRadius="10" Padding="15">
                    <VerticalStackLayout>
                        <Label Text="{Binding ModuleTitle}" FontSize="24" FontAttributes="Bold" />
                        <Label Text="{Binding SpecializationInfo}" />
                        <Label Text="{Binding DateRangeInfo}" />
                        <ProgressBar Progress="{Binding OverallProgress}" Margin="0,10" />
                        <Label Text="{Binding ProgressText}" HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </Frame>
                <Grid ColumnDefinitions="*,*" RowDefinitions="*,*,*" RowSpacing="15" ColumnSpacing="15">
                    <Frame Grid.Row="1" Grid.Column="0" BorderColor="LightGray" HasShadow="True">
                        <VerticalStackLayout>
                            <Label Text="Procedury" FontAttributes="Bold" />
                            <Label Text="{Binding ProcedureCount, StringFormat='{0} z {1}'}" FontSize="24" HorizontalOptions="Center" />
                            <ProgressBar Progress="{Binding ProcedureProgress}" />
                            <Button Text="Przejdź" Command="{Binding NavigateToProceduresCommand}" />
                        </VerticalStackLayout>
                    </Frame>
                    <Frame Grid.Row="0" Grid.Column="1" BorderColor="LightGray" HasShadow="True">
                        <VerticalStackLayout>
                            <Label Text="Dyżury" FontAttributes="Bold" />
                            <Label Text="{Binding ShiftStats}" FontSize="24" HorizontalOptions="Center" />
                            <ProgressBar Progress="{Binding ShiftProgress}" />
                            <Button Text="Przejdź" Command="{Binding NavigateToShiftsCommand}" />
                        </VerticalStackLayout>
                    </Frame>
                    <Frame Grid.Row="0" Grid.Column="0" BorderColor="LightGray" HasShadow="True">
                        <VerticalStackLayout>
                            <Label Text="Staże" FontAttributes="Bold" />
                            <Label Text="{Binding InternshipCount, StringFormat='{0} z {1}'}" FontSize="24" HorizontalOptions="Center" />
                            <ProgressBar Progress="{Binding InternshipProgress}" />
                            <Button Text="Przejdź" Command="{Binding NavigateToInternshipsCommand}" />
                        </VerticalStackLayout>
                    </Frame>
                    <Frame Grid.Row="1" Grid.Column="1" BorderColor="LightGray" HasShadow="True">
                        <VerticalStackLayout>
                            <Label Text="Kursy" FontAttributes="Bold" />
                            <Label Text="{Binding CourseCount, StringFormat='{0} z {1}'}" FontSize="24" HorizontalOptions="Center" />
                            <ProgressBar Progress="{Binding CourseProgress}" />
                            <Button Text="Przejdź" Command="{Binding NavigateToCoursesCommand}" />
                        </VerticalStackLayout>
                    </Frame>
                    <Frame Grid.Row="2" Grid.Column="0" BorderColor="LightGray" HasShadow="True">
                        <VerticalStackLayout>
                            <Label Text="Samokształcenie" FontAttributes="Bold" />
                            <Label Text="{Binding SelfEducationCount}" FontSize="24" HorizontalOptions="Center" />
                            <Button Text="Przejdź" Command="{Binding NavigateToSelfEducationCommand}" />
                        </VerticalStackLayout>
                    </Frame>
                    <Frame Grid.Row="2" Grid.Column="1" BorderColor="LightGray" HasShadow="True">
                        <VerticalStackLayout>
                            <Label Text="Publikacje" FontAttributes="Bold" />
                            <Label Text="{Binding PublicationCount}" FontSize="24" HorizontalOptions="Center" />
                            <Button Text="Przejdź" Command="{Binding NavigateToPublicationsCommand}" />
                        </VerticalStackLayout>
                    </Frame>
                </Grid>
                <Grid ColumnDefinitions="*,*,*" ColumnSpacing="10">
                    <Button Grid.Column="0" Text="Nieobecności" Command="{Binding NavigateToAbsencesCommand}" />
                    <Button Grid.Column="1" Text="Eksport" Command="{Binding NavigateToExportCommand}" />
                </Grid>
                <Button IsVisible="{Binding SpecialisticModuleSelected}" 
                        Text="Uznania i skrócenia" 
                        Command="{Binding NavigateToRecognitionsCommand}" 
                        BackgroundColor="Transparent"
                        TextColor="{StaticResource Primary}" />
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
```

## File: Views/Dashboard/DashboardPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Dashboard;

namespace SledzSpecke.App.Views.Dashboard
{
    public partial class DashboardPage : ContentPage, IDisposable
    {
        private readonly DashboardViewModel viewModel;

        public DashboardPage()
        {
            this.InitializeComponent();
            this.viewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Dashboard.DashboardViewModel>();
            this.BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.BindingContext is SledzSpecke.App.ViewModels.Dashboard.DashboardViewModel viewModel)
            {
                viewModel.RefreshCommand?.Execute(null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public void Dispose()
        {
            (this.BindingContext as DashboardViewModel)?.Dispose();
        }
    }
}
```

## File: Views/Internships/AddEditRealizedInternshipPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Internships"
             x:DataType="vm:AddEditRealizedInternshipViewModel"
             x:Class="SledzSpecke.App.Views.Internships.AddEditRealizedInternshipPage"
             Title="{Binding Title}">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="15">
            <Grid IsVisible="{Binding IsNewSMK}">
                <VerticalStackLayout>
                    <Label Text="{Binding NewSMKInternship.InternshipName}" 
                           FontSize="18" 
                           FontAttributes="Bold" 
                           HorizontalOptions="Center" />

                    <Label Text="Placówka realizująca szkolenie" FontAttributes="Bold" />
                    <Entry Text="{Binding NewSMKInternship.InstitutionName}" Placeholder="Np. Katedra i Klinika Psychiatryczna" />

                    <Label Text="Nazwa komórki organizacyjnej (miejsce odbywania stażu)" FontAttributes="Bold" />
                    <Entry Text="{Binding NewSMKInternship.DepartmentName}" Placeholder="Np. Oddział psychiatryczny" />

                    <Label Text="Liczba dni" FontAttributes="Bold" />
                    <Entry Text="{Binding NewSMKInternship.DaysCount}" Keyboard="Numeric" />

                    <Label Text="Daty realizacji" FontAttributes="Bold" />
                    <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                        <VerticalStackLayout Grid.Column="0">
                            <Label Text="Od:" />
                            <DatePicker Date="{Binding NewSMKInternship.StartDate}" Format="dd.MM.yyyy" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Grid.Column="1">
                            <Label Text="Do:" />
                            <DatePicker Date="{Binding NewSMKInternship.EndDate}" Format="dd.MM.yyyy" />
                        </VerticalStackLayout>
                    </Grid>

                    <Label Text="Dane kierownika stażu" FontAttributes="Bold" />
                    <Entry Text="{Binding NewSMKInternship.SupervisorName}" Placeholder="Imię i nazwisko kierownika stażu" />

                    <Label Text="Status realizacji" FontAttributes="Bold" />
                    <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                        <VerticalStackLayout Grid.Column="0">
                            <Label Text="Ukończony:" />
                            <CheckBox IsChecked="{Binding NewSMKInternship.IsCompleted}" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Grid.Column="1">
                            <Label Text="Zatwierdzony:" />
                            <CheckBox IsChecked="{Binding NewSMKInternship.IsApproved}" />
                        </VerticalStackLayout>
                    </Grid>

                    <Label Text="Uznanie stażu" FontAttributes="Bold" />
                    <CheckBox IsChecked="{Binding NewSMKInternship.IsRecognition}" />
                    <VerticalStackLayout IsVisible="{Binding NewSMKInternship.IsRecognition}">
                        <Label Text="Powód uznania" />
                        <Editor Text="{Binding NewSMKInternship.RecognitionReason}" HeightRequest="100" />

                        <Label Text="Liczba dni uznanych" />
                        <Entry Text="{Binding NewSMKInternship.RecognitionDaysReduction}" Keyboard="Numeric" />
                    </VerticalStackLayout>
                </VerticalStackLayout>
            </Grid>

            <Grid IsVisible="{Binding IsNewSMK, Converter={StaticResource InvertedBoolConverter}}">
                <VerticalStackLayout>
                    <Label Text="{Binding OldSMKInternship.InternshipName}" 
                           FontSize="18" 
                           FontAttributes="Bold" 
                           HorizontalOptions="Center" />

                    <Label Text="Placówka realizująca szkolenie" FontAttributes="Bold" />
                    <Entry Text="{Binding OldSMKInternship.InstitutionName}" Placeholder="Np. Katedra i Klinika Psychiatryczna" />

                    <Label Text="Nazwa komórki organizacyjnej (miejsce odbywania stażu)" FontAttributes="Bold" />
                    <Entry Text="{Binding OldSMKInternship.DepartmentName}" Placeholder="Np. Oddział psychiatryczny" />

                    <Label Text="Liczba dni (wyliczane na podstawie dat)" FontAttributes="Bold" IsVisible="False" />
                    <Entry Text="{Binding OldSMKInternship.DaysCount}" Keyboard="Numeric" IsReadOnly="True" TextColor="Grey" IsVisible="False" />

                    <Label Text="Rok specjalizacji" FontAttributes="Bold" />
                    <Entry Text="{Binding OldSMKInternship.Year}" Keyboard="Numeric" />

                    <Label Text="Daty realizacji" FontAttributes="Bold" />
                    <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                        <VerticalStackLayout Grid.Column="0">
                            <Label Text="Od:" />
                            <DatePicker Date="{Binding OldSMKInternship.StartDate}" Format="dd.MM.yyyy" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Grid.Column="1">
                            <Label Text="Do:" />
                            <DatePicker Date="{Binding OldSMKInternship.EndDate}" Format="dd.MM.yyyy" />
                        </VerticalStackLayout>
                    </Grid>

                    <Label Text="Dane kierownika stażu" FontAttributes="Bold" />
                    <Entry Text="{Binding OldSMKInternship.SupervisorName}" Placeholder="Imię i nazwisko kierownika stażu" />

                    <Label Text="Status realizacji" FontAttributes="Bold" />
                    <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                        <VerticalStackLayout Grid.Column="0">
                            <Label Text="Ukończony:" />
                            <CheckBox IsChecked="{Binding OldSMKInternship.IsCompleted}" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Grid.Column="1">
                            <Label Text="Zatwierdzony:" />
                            <CheckBox IsChecked="{Binding OldSMKInternship.IsApproved}" />
                        </VerticalStackLayout>
                    </Grid>

                    <Label Text="Wymaga zatwierdzenia:" />
                    <CheckBox IsChecked="{Binding OldSMKInternship.RequiresApproval}" />
                </VerticalStackLayout>
            </Grid>

            <Grid ColumnDefinitions="*,*" ColumnSpacing="15" Margin="0,15,0,0">
                <Button Grid.Column="0" Text="Anuluj" Command="{Binding CancelCommand}" />
                <Button Grid.Column="1" Text="Zapisz" Command="{Binding SaveCommand}" 
                        BackgroundColor="{StaticResource Primary}" TextColor="White" />
            </Grid>
            <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" 
                             HorizontalOptions="Center" Margin="0,20,0,0" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

## File: Views/Internships/AddEditRealizedInternshipPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class AddEditRealizedInternshipPage : ContentPage
    {
        private readonly AddEditRealizedInternshipViewModel viewModel;

        public AddEditRealizedInternshipPage(AddEditRealizedInternshipViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.InitializeAsync().ConfigureAwait(false);
        }
    }
}
```

## File: Views/Internships/InternshipsSelectorPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Internships"
             x:DataType="vm:InternshipsSelectorViewModel"
             x:Class="SledzSpecke.App.Views.Internships.InternshipsSelectorPage"
             Title="{Binding Title}">
    <VerticalStackLayout Padding="20" Spacing="20" VerticalOptions="Center" HorizontalOptions="Center">
        <ActivityIndicator IsRunning="True" />
        <Label Text="Ładowanie staży..." HorizontalOptions="Center" />
    </VerticalStackLayout>
</ContentPage>
```

## File: Views/Internships/InternshipsSelectorPage.xaml.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;

namespace SledzSpecke.App.Views.Internships
{
    public partial class InternshipsSelectorPage : ContentPage
    {
        private readonly IAuthService authService;

        public InternshipsSelectorPage(IAuthService authService)
        {
            this.InitializeComponent();
            this.authService = authService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var user = await this.authService.GetCurrentUserAsync();

            if (user != null)
            {
                if (user.SmkVersion == SmkVersion.Old)
                {
                    await Shell.Current.GoToAsync("/OldSMKInternships");
                }
                else
                {
                    await Shell.Current.GoToAsync("/NewSMKInternships");
                }
            }
            else
            {
                await DisplayAlert("Błąd", "Nie można określić wersji SMK. Skontaktuj się z administratorem.", "OK");
            }
        }
    }
}
```

## File: Views/Internships/NewSMKInternshipsListPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Internships"
             x:DataType="vm:NewSMKInternshipsListViewModel"
             x:Class="SledzSpecke.App.Views.Internships.NewSMKInternshipsListPage"
             Title="{Binding Title}">
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="HeaderStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource Primary}" />
                <Setter Property="Margin" Value="0,10,0,5" />
            </Style>
            <Style x:Key="RequirementTitleStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource TextColor}" />
                <Setter Property="Margin" Value="0,5,0,0" />
            </Style>
            <Style x:Key="RequirementSummaryStyle" TargetType="Label">
                <Setter Property="FontSize" Value="14" />
                <Setter Property="TextColor" Value="{StaticResource TextMutedColor}" />
                <Setter Property="Margin" Value="0,5,0,0" />
            </Style>
            <Style x:Key="LightIconButton" TargetType="Button">
                <Setter Property="BackgroundColor" Value="Transparent" />
                <Setter Property="BorderColor" Value="Transparent" />
                <Setter Property="HeightRequest" Value="30" />
                <Setter Property="WidthRequest" Value="30" />
                <Setter Property="Padding" Value="0" />
                <Setter Property="CornerRadius" Value="15" />
                <Setter Property="BorderWidth" Value="0" />
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
    <RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Spacing="20" Padding="20">
                <Label Text="{Binding ModuleTitle}" FontSize="20" FontAttributes="Bold" />
                <StackLayout IsVisible="{Binding HasTwoModules}" Orientation="Horizontal" 
                            HorizontalOptions="Center" Margin="0,10" Spacing="10">
                    <Label Text="Aktywny moduł:" VerticalOptions="Center" FontAttributes="Bold" />
                    <Frame Padding="0" CornerRadius="5" BorderColor="{StaticResource Primary}" 
                          HasShadow="False" IsClippedToBounds="True">
                        <Grid ColumnDefinitions="*,*">
                            <Button Text="Podstawowy" 
                                    BackgroundColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Basic" />
                            <Button Grid.Column="1" Text="Specjalistyczny" 
                                    BackgroundColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Specialistic" />
                        </Grid>
                    </Frame>
                </StackLayout>
                <Label Text="Staże kierunkowe" Style="{StaticResource HeaderStyle}" />
                <Label Text="Brak danych" IsVisible="{Binding InternshipRequirements.Count, Converter={StaticResource StringMatchConverter}, ConverterParameter='0'}" 
                      HorizontalOptions="Center" Margin="0,20" />
                <CollectionView ItemsSource="{Binding InternshipRequirements}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:InternshipStageViewModel">
                            <Frame Margin="0,0,0,10" Padding="15" BorderColor="LightGray" HasShadow="True" CornerRadius="10">
                                <VerticalStackLayout>
                                    <Grid ColumnDefinitions="*,Auto">
                                        <Label Text="{Binding Name}" Style="{StaticResource RequirementTitleStyle}" />
                                        <Label Grid.Column="1" Text="{Binding FormattedStatistics}" Style="{StaticResource RequirementSummaryStyle}" />
                                    </Grid>
                                    <Button Text="Szczegóły" 
                                            Command="{Binding ToggleExpandCommand}" 
                                            Style="{StaticResource OutlineButton}" 
                                            Margin="0,10,0,0" />
                                    <VerticalStackLayout IsVisible="{Binding IsExpanded}" Margin="0,15,0,0" Spacing="10">
                                        <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto,Auto,Auto,Auto,Auto" RowSpacing="10" ColumnSpacing="10">
                                            <Label Grid.Row="0" Grid.Column="0" Text="Dni wymagane" FontAttributes="Bold" />
                                            <Label Grid.Row="0" Grid.Column="1" Text="{Binding RequiredDays}" />

                                            <Label Grid.Row="1" Grid.Column="0" Text="Dni wprowadzone" FontAttributes="Bold" />
                                            <Label Grid.Row="1" Grid.Column="1" Text="{Binding IntroducedDays}" />

                                            <Label Grid.Row="2" Grid.Column="0" Text="Dni uznane" FontAttributes="Bold" />
                                            <Label Grid.Row="2" Grid.Column="1" Text="{Binding RecognizedDays}" />

                                            <Label Grid.Row="3" Grid.Column="0" Text="Dni samokształcenia" FontAttributes="Bold" />
                                            <Label Grid.Row="3" Grid.Column="1" Text="{Binding SelfEducationDays}" />

                                            <Label Grid.Row="4" Grid.Column="0" Text="Pozostało do zrealizowania" FontAttributes="Bold" />
                                            <Label Grid.Row="4" Grid.Column="1" Text="{Binding RemainingDays}" />
                                        </Grid>
                                        <StackLayout IsVisible="{Binding NewSMKRealizationsCollection.Count, Converter={StaticResource NotNullConverter}}">
                                            <Label Text="Realizacje stażu" FontAttributes="Bold" Margin="0,10,0,5" />
                                            <CollectionView ItemsSource="{Binding NewSMKRealizationsCollection}"
                                                            HeightRequest="{Binding NewSMKRealizationsCollection.Count, Converter={StaticResource ItemCountToHeightConverter}, ConverterParameter='80'}">
                                                <CollectionView.ItemTemplate>
                                                    <DataTemplate>
                                                        <Frame Margin="2" Padding="10" BorderColor="LightGray" CornerRadius="5">
                                                            <Grid ColumnDefinitions="*,Auto,Auto">
                                                                <VerticalStackLayout Grid.Column="0">
                                                                    <Label Text="{Binding InstitutionName}" FontAttributes="Bold" />
                                                                    <Label Text="{Binding DateRange}" FontSize="12" />
                                                                    <Label Text="{Binding DaysCount, StringFormat='{0} dni'}" FontSize="12" />
                                                                </VerticalStackLayout>
                                                                <Label Grid.Column="1" Text="{Binding IsCompleted, Converter={StaticResource BoolToTextConverter}, ConverterParameter='Ukończony'}" 
                                                                       TextColor="{StaticResource SuccessColor}" VerticalOptions="Center"
                                                                       IsVisible="{Binding IsCompleted}" />
                                                            </Grid>
                                                        </Frame>
                                                    </DataTemplate>
                                                </CollectionView.ItemTemplate>
                                            </CollectionView>
                                        </StackLayout>
                                        <Button Text="Dodaj realizację" 
                                                Command="{Binding AddRealizationCommand}" 
                                                BackgroundColor="{StaticResource Primary}" 
                                                TextColor="White" 
                                                Margin="0,10,0,0" />
                                    </VerticalStackLayout>
                                </VerticalStackLayout>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
```

## File: Views/Internships/NewSMKInternshipsListPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class NewSMKInternshipsListPage : ContentPage
    {
        private readonly NewSMKInternshipsListViewModel viewModel;

        public NewSMKInternshipsListPage(NewSMKInternshipsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!this.viewModel.IsBusy)
            {
                this.viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
```

## File: Views/Internships/OldSMKInternshipsListPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Internships"
             xmlns:models="clr-namespace:SledzSpecke.App.Models"
             x:DataType="vm:OldSMKInternshipsListViewModel"
             x:Class="SledzSpecke.App.Views.Internships.OldSMKInternshipsListPage"
             Title="{Binding Title}">
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="HeaderStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource Primary}" />
                <Setter Property="Margin" Value="0,10,0,5" />
            </Style>
            <Style x:Key="RequirementTitleStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource TextColor}" />
                <Setter Property="Margin" Value="0,5,0,0" />
            </Style>
            <Style x:Key="RequirementSummaryStyle" TargetType="Label">
                <Setter Property="FontSize" Value="14" />
                <Setter Property="TextColor" Value="{StaticResource TextMutedColor}" />
                <Setter Property="Margin" Value="0,5,0,0" />
            </Style>
            <Style x:Key="LightIconButton" TargetType="Button">
                <Setter Property="BackgroundColor" Value="Transparent" />
                <Setter Property="BorderColor" Value="Transparent" />
                <Setter Property="HeightRequest" Value="30" />
                <Setter Property="WidthRequest" Value="30" />
                <Setter Property="Padding" Value="0" />
                <Setter Property="CornerRadius" Value="15" />
                <Setter Property="BorderWidth" Value="0" />
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
    <RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Spacing="20" Padding="20">
                <Label Text="{Binding ModuleTitle}" FontSize="20" FontAttributes="Bold" />
                <StackLayout IsVisible="{Binding HasTwoModules}" Orientation="Horizontal" 
                            HorizontalOptions="Center" Margin="0,10" Spacing="10">
                    <Label Text="Aktywny moduł:" VerticalOptions="Center" FontAttributes="Bold" />
                    <Frame Padding="0" CornerRadius="5" BorderColor="{StaticResource Primary}" 
                          HasShadow="False" IsClippedToBounds="True">
                        <Grid ColumnDefinitions="*,*">
                            <Button Text="Podstawowy" 
                                    BackgroundColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Basic" />
                            <Button Grid.Column="1" Text="Specjalistyczny" 
                                    BackgroundColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Specialistic" />
                        </Grid>
                    </Frame>
                </StackLayout>
                <Label Text="Staże kierunkowe" Style="{StaticResource HeaderStyle}" />
                <Label Text="Brak danych" IsVisible="{Binding InternshipRequirements.Count, Converter={StaticResource StringMatchConverter}, ConverterParameter='0'}" 
                      HorizontalOptions="Center" Margin="0,20" />
                <CollectionView ItemsSource="{Binding InternshipRequirements}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:InternshipStageViewModel">
                            <Frame Margin="0,0,0,10" Padding="15" BorderColor="LightGray" HasShadow="True" CornerRadius="10">
                                <VerticalStackLayout>
                                    <Grid ColumnDefinitions="*,Auto">
                                        <Label Text="{Binding Name}" Style="{StaticResource RequirementTitleStyle}" />
                                        <Label Grid.Column="1" Text="{Binding FormattedStatistics}" Style="{StaticResource RequirementSummaryStyle}" />
                                    </Grid>
                                    <Button Text="Szczegóły" 
                                            Command="{Binding ToggleExpandCommand}" 
                                            Style="{StaticResource OutlineButton}" 
                                            Margin="0,10,0,0" />
                                    <VerticalStackLayout IsVisible="{Binding IsExpanded}" Margin="0,15,0,0" Spacing="10">
                                        <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto,Auto,Auto,Auto,Auto" RowSpacing="10" ColumnSpacing="10">
                                            <Label Grid.Row="0" Grid.Column="0" Text="Dni wymagane" FontAttributes="Bold" />
                                            <Label Grid.Row="0" Grid.Column="1" Text="{Binding RequiredDays}" />

                                            <Label Grid.Row="1" Grid.Column="0" Text="Dni wprowadzone" FontAttributes="Bold" />
                                            <Label Grid.Row="1" Grid.Column="1" Text="{Binding IntroducedDays}" />

                                            <Label Grid.Row="2" Grid.Column="0" Text="Dni uznane" FontAttributes="Bold" />
                                            <Label Grid.Row="2" Grid.Column="1" Text="{Binding RecognizedDays}" />

                                            <Label Grid.Row="3" Grid.Column="0" Text="Dni samokształcenia" FontAttributes="Bold" />
                                            <Label Grid.Row="3" Grid.Column="1" Text="{Binding SelfEducationDays}" />

                                            <Label Grid.Row="4" Grid.Column="0" Text="Pozostało do zrealizowania" FontAttributes="Bold" />
                                            <Label Grid.Row="4" Grid.Column="1" Text="{Binding RemainingDays}" />
                                        </Grid>
                                        <StackLayout IsVisible="{Binding OldSMKRealizationsCollection.Count, Converter={StaticResource NotNullConverter}}">
                                            <Label Text="Realizacje stażu" FontAttributes="Bold" Margin="0,10,0,5" />
                                            <CollectionView ItemsSource="{Binding OldSMKRealizationsCollection}"
                                                            HeightRequest="{Binding OldSMKRealizationsCollection.Count, Converter={StaticResource ItemCountToHeightConverter}, ConverterParameter='80'}">
                                                <CollectionView.ItemTemplate>
                                                    <DataTemplate x:DataType="models:RealizedInternshipOldSMK">
                                                        <Frame Margin="2" Padding="10" BorderColor="LightGray" CornerRadius="5">
                                                            <Grid ColumnDefinitions="*,Auto,Auto,Auto">
                                                                <VerticalStackLayout Grid.Column="0">
                                                                    <Label Text="{Binding InstitutionName}" FontAttributes="Bold" />
                                                                    <Label Text="{Binding DateRange}" FontSize="12" />
                                                                    <Label Text="{Binding DaysCount, StringFormat='{0} dni'}" FontSize="12" />
                                                                </VerticalStackLayout>
                                                                <Label Grid.Column="1" Text="{Binding IsCompleted, Converter={StaticResource BoolToTextConverter}, ConverterParameter='Ukończony'}" 
                                                                       TextColor="{StaticResource SuccessColor}" VerticalOptions="Center"
                                                                       IsVisible="{Binding IsCompleted}" />
                                                                <Button Grid.Column="2" 
                                                                        Text="✏️" 
                                                                        Clicked="OnEditButtonClicked"
                                                                        BackgroundColor="LightGoldenrodYellow"
                                                                        TextColor="White"
                                                                        BorderColor="Orange"
                                                                        BorderWidth="1"
                                                                        Style="{StaticResource LightIconButton}" />
                                                                <Button Grid.Column="3" 
                                                                        Text="❌" 
                                                                        Clicked="OnDeleteButtonClicked"
                                                                        BackgroundColor="LightPink"
                                                                        TextColor="LightPink"
                                                                        Style="{StaticResource LightIconButton}" />
                                                            </Grid>
                                                        </Frame>
                                                    </DataTemplate>
                                                </CollectionView.ItemTemplate>
                                            </CollectionView>
                                        </StackLayout>
                                        <Button Text="Dodaj realizację" 
                                                Command="{Binding AddRealizationCommand}" 
                                                BackgroundColor="{StaticResource Primary}" 
                                                TextColor="White" 
                                                Margin="0,10,0,0" />
                                    </VerticalStackLayout>
                                </VerticalStackLayout>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
```

## File: Views/Internships/OldSMKInternshipsListPage.xaml.cs
```csharp
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class OldSMKInternshipsListPage : ContentPage
    {
        private readonly OldSMKInternshipsListViewModel viewModel;

        public OldSMKInternshipsListPage(OldSMKInternshipsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedInternshipOldSMK realization)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RealizedInternshipId", realization.RealizedInternshipId.ToString() },
                    { "Year", realization.Year.ToString() }
                };

                // Znajdź wymaganie stażowe o tej samej nazwie
                var specializationService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                var internships = await specializationService.GetInternshipsAsync(null);
                var requirement = internships.FirstOrDefault(i => i.InternshipName == realization.InternshipName);

                if (requirement != null)
                {
                    navigationParameter.Add("InternshipRequirementId", requirement.InternshipId.ToString());
                }
                else
                {
                    // Jeśli nie znaleziono, wyświetl komunikat
                    await this.DisplayAlert("Uwaga",
                        "Nie znaleziono wymagania stażowego dla tej realizacji. Edycja może być niekompletna.",
                        "OK");

                    // Użyj pierwszego dostępnego wymagania
                    if (internships.Count > 0)
                    {
                        navigationParameter.Add("InternshipRequirementId", internships[0].InternshipId.ToString());
                    }
                    else
                    {
                        await this.DisplayAlert("Błąd",
                            "Brak wymagań stażowych. Nie można edytować realizacji.",
                            "OK");
                        return;
                    }
                }

                await Shell.Current.GoToAsync("//AddEditRealizedInternship", navigationParameter);
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedInternshipOldSMK realization)
            {
                bool confirm = await this.DisplayAlert(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę realizację stażu?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    var specializationService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                    bool success = await specializationService.DeleteRealizedInternshipOldSMKAsync(realization.RealizedInternshipId);

                    if (success)
                    {
                        await this.DisplayAlert("Sukces", "Realizacja stażu została usunięta.", "OK");
                        this.viewModel.RefreshCommand.Execute(null);
                    }
                    else
                    {
                        await this.DisplayAlert("Błąd", "Nie udało się usunąć realizacji stażu.", "OK");
                    }
                }
            }
        }
    }
}
```

## File: Views/MedicalShifts/AddEditOldSMKMedicalShiftPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.MedicalShifts"
             x:DataType="vm:AddEditOldSMKMedicalShiftViewModel"
             x:Class="SledzSpecke.App.Views.MedicalShifts.AddEditOldSMKMedicalShiftPage"
             Title="{Binding Title}">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="15">
            <Label Text="{Binding Shift.Year, StringFormat='Rok: {0}'}" 
                   FontSize="16" 
                   FontAttributes="Bold" />
            <Label Text="Data rozpoczęcia" FontAttributes="Bold" />
            <DatePicker Date="{Binding Shift.StartDate}" Format="yyyy-MM-dd" />
            <Label Text="Czas trwania" FontAttributes="Bold" />
            <Grid ColumnDefinitions="*,Auto,*">
                <VerticalStackLayout Grid.Column="0">
                    <Label Text="Liczba godzin" />
                    <Entry Text="{Binding Shift.Hours}" Keyboard="Numeric" />
                </VerticalStackLayout>
                <Label Text=":" Grid.Column="1" VerticalOptions="End" Margin="0,0,0,10" FontSize="20" />
                <VerticalStackLayout Grid.Column="2">
                    <Label Text="Liczba minut" />
                    <Entry Text="{Binding Shift.Minutes}" Keyboard="Numeric" />
                </VerticalStackLayout>
            </Grid>
            <Label Text="Nazwa komórki organizacyjnej" FontAttributes="Bold" />
            <Entry Text="{Binding Shift.Location}" Placeholder="Np. Oddział Kardiologii" />
            <Grid ColumnDefinitions="*,*" ColumnSpacing="15" Margin="0,15,0,0">
                <Button Grid.Column="0" Text="Anuluj" Command="{Binding CancelCommand}" />
                <Button Grid.Column="1" Text="Zapisz" Command="{Binding SaveCommand}" 
                        BackgroundColor="{StaticResource Primary}" TextColor="White" />
            </Grid>
            <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" 
                              HorizontalOptions="Center" Margin="0,20,0,0" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

## File: Views/MedicalShifts/AddEditOldSMKMedicalShiftPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class AddEditOldSMKMedicalShiftPage : ContentPage
    {
        private readonly AddEditOldSMKMedicalShiftViewModel viewModel;

        public AddEditOldSMKMedicalShiftPage(AddEditOldSMKMedicalShiftViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.InitializeAsync().ConfigureAwait(false);
        }
    }
}
```

## File: Views/MedicalShifts/MedicalShiftsSelectorPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="SledzSpecke.App.Views.MedicalShifts.MedicalShiftsSelectorPage"
             Title="Dyżury medyczne">
    <VerticalStackLayout Padding="20" Spacing="20" VerticalOptions="Center" HorizontalOptions="Center">
        <ActivityIndicator IsRunning="True" />
        <Label Text="Ładowanie dyżurów..." HorizontalOptions="Center" />
    </VerticalStackLayout>
</ContentPage>
```

## File: Views/MedicalShifts/MedicalShiftsSelectorPage.xaml.cs
```csharp
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class MedicalShiftsSelectorPage : ContentPage
    {
        private readonly IAuthService authService;

        public MedicalShiftsSelectorPage(IAuthService authService)
        {
            this.InitializeComponent();
            this.authService = authService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var user = await this.authService.GetCurrentUserAsync();

            if (user != null)
            {
                if (user.SmkVersion == SmkVersion.Old)
                {
                    await Shell.Current.GoToAsync("OldSMKMedicalShifts");
                }
                else
                {
                    await Shell.Current.GoToAsync("NewSMKMedicalShifts");
                }
            }
            else
            {
                await DisplayAlert("Błąd", "Nie można określić wersji SMK. Skontaktuj się z administratorem.", "OK");
            }
        }
    }
}
```

## File: Views/MedicalShifts/NewSMKMedicalShiftsPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.MedicalShifts"
             xmlns:ivm="clr-namespace:SledzSpecke.App.ViewModels.Internships"
             x:DataType="vm:NewSMKMedicalShiftsListViewModel"
             x:Class="SledzSpecke.App.Views.MedicalShifts.NewSMKMedicalShiftsPage"
             Title="{Binding Title}">
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="HeaderStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource Primary}" />
                <Setter Property="Margin" Value="0,10,0,5" />
            </Style>
            <Style x:Key="RequirementTitleStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource TextColor}" />
                <Setter Property="Margin" Value="0,5,0,0" />
            </Style>
            <Style x:Key="RequirementSummaryStyle" TargetType="Label">
                <Setter Property="FontSize" Value="14" />
                <Setter Property="TextColor" Value="{StaticResource TextMutedColor}" />
                <Setter Property="Margin" Value="0,5,0,0" />
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
    <RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Spacing="20" Padding="20">
                <Label Text="{Binding ModuleTitle}" FontSize="20" FontAttributes="Bold" />
                <StackLayout IsVisible="{Binding HasTwoModules}" Orientation="Horizontal" 
                            HorizontalOptions="Center" Margin="0,10" Spacing="10">
                    <Label Text="Aktywny moduł:" VerticalOptions="Center" FontAttributes="Bold" />
                    <Frame Padding="0" CornerRadius="5" BorderColor="{StaticResource Primary}" 
                        HasShadow="False" IsClippedToBounds="True">
                        <Grid ColumnDefinitions="*,*">
                            <Button Text="Podstawowy" 
                                    BackgroundColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Basic" />
                            <Button Grid.Column="1" Text="Specjalistyczny" 
                                    BackgroundColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Specialistic" />
                        </Grid>
                    </Frame>
                </StackLayout>
                <Label Text="Dyżury medyczne" Style="{StaticResource HeaderStyle}" />
                <Label Text="Brak danych" IsVisible="{Binding InternshipRequirements.Count, Converter={StaticResource StringMatchConverter}, ConverterParameter='0'}" 
                       HorizontalOptions="Center" Margin="0,20" />
                <CollectionView ItemsSource="{Binding InternshipRequirements}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="ivm:InternshipRequirementViewModel">
                            <Frame Margin="0,0,0,10" Padding="15" BorderColor="LightGray" HasShadow="True" CornerRadius="10">
                                <VerticalStackLayout>
                                    <Grid ColumnDefinitions="*,Auto">
                                        <Label Text="{Binding Title}" Style="{StaticResource RequirementTitleStyle}" />
                                        <Label Grid.Column="1" Text="{Binding Summary}" Style="{StaticResource RequirementSummaryStyle}" />
                                    </Grid>
                                    <Button Text="Edytuj" 
                                            Command="{Binding ToggleExpandCommand}" 
                                            Style="{StaticResource OutlineButton}" 
                                            Margin="0,10,0,0" />
                                    <VerticalStackLayout IsVisible="{Binding IsEditing}" Margin="0,15,0,0" Spacing="10">
                                        <Grid ColumnDefinitions="*,Auto,*">
                                            <VerticalStackLayout Grid.Column="0">
                                                <Label Text="Liczba godzin" />
                                                <Entry Text="{Binding CurrentShift.Hours}" Keyboard="Numeric" />
                                            </VerticalStackLayout>
                                            <Label Text=":" Grid.Column="1" VerticalOptions="End" Margin="0,0,0,10" FontSize="20" />
                                            <VerticalStackLayout Grid.Column="2">
                                                <Label Text="Liczba minut" />
                                                <Entry Text="{Binding CurrentShift.Minutes}" Keyboard="Numeric" />
                                            </VerticalStackLayout>
                                        </Grid>
                                        <Label Text="Daty realizacji" Margin="0,10,0,0" />
                                        <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                                            <VerticalStackLayout Grid.Column="0">
                                                <Label Text="Od:" />
                                                <DatePicker Date="{Binding CurrentShift.StartDate}" Format="dd.MM.yyyy" />
                                            </VerticalStackLayout>
                                            <VerticalStackLayout Grid.Column="1">
                                                <Label Text="Do:" />
                                                <DatePicker Date="{Binding CurrentShift.EndDate}" Format="dd.MM.yyyy" />
                                            </VerticalStackLayout>
                                        </Grid>
                                        <Grid ColumnDefinitions="*,*" ColumnSpacing="10" Margin="0,10,0,0">
                                            <Button Grid.Column="0" Text="Anuluj" Command="{Binding CancelCommand}" />
                                            <Button Grid.Column="1" Text="Zapisz" Command="{Binding SaveCommand}" 
                                                    BackgroundColor="{StaticResource Primary}" TextColor="White" />
                                        </Grid>
                                    </VerticalStackLayout>
                                </VerticalStackLayout>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
```

## File: Views/MedicalShifts/NewSMKMedicalShiftsPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class NewSMKMedicalShiftsPage : ContentPage
    {
        private readonly NewSMKMedicalShiftsListViewModel viewModel;

        public NewSMKMedicalShiftsPage(NewSMKMedicalShiftsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!this.viewModel.IsBusy)
            {
                this.viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
```

## File: Views/MedicalShifts/OldSMKMedicalShiftsPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.MedicalShifts"
             xmlns:models="clr-namespace:SledzSpecke.App.Models"
             x:DataType="vm:OldSMKMedicalShiftsListViewModel"
             x:Class="SledzSpecke.App.Views.MedicalShifts.OldSMKMedicalShiftsPage"
             Title="{Binding Title}">
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="HeaderStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource Primary}" />
                <Setter Property="Margin" Value="0,10,0,5" />
            </Style>
            <Style x:Key="ColumnHeaderStyle" TargetType="Label">
                <Setter Property="FontSize" Value="12" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource TextMutedColor}" />
                <Setter Property="Margin" Value="0,0,5,0" />
            </Style>
            <Style x:Key="ColumnValueStyle" TargetType="Label">
                <Setter Property="FontSize" Value="14" />
                <Setter Property="TextColor" Value="{StaticResource TextColor}" />
                <Setter Property="LineBreakMode" Value="TailTruncation" />
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Dodaj" Clicked="OnAddButtonClicked" IconImageSource="add.png" />
    </ContentPage.ToolbarItems>
    <RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Spacing="20" Padding="20">
                <Label Text="{Binding ModuleTitle}" FontSize="20" FontAttributes="Bold" />
                <Frame BorderColor="LightGray" HasShadow="True" CornerRadius="10" Padding="15">
                    <VerticalStackLayout>
                        <Label Text="Wybierz rok specjalizacji, dla którego chcesz zobaczyć dyżury:" Style="{StaticResource HeaderStyle}" />
                        <VerticalStackLayout Spacing="5" HorizontalOptions="Center" Margin="0,10">
                            <Label Text="Wybierz rok:" HorizontalOptions="Center" FontAttributes="Bold" />
                            <FlexLayout x:Name="YearsContainer"
                                       Direction="Row"
                                       Wrap="Wrap"
                                       JustifyContent="Center"
                                       AlignItems="Center"
                                       AlignContent="Center"
                                       Margin="0,5,0,0">
                            </FlexLayout>
                        </VerticalStackLayout>
                    </VerticalStackLayout>
                </Frame>
                <Frame BorderColor="LightGray" HasShadow="True" CornerRadius="10" Padding="15">
                    <VerticalStackLayout>
                        <Label Text="Dyżury medyczne - podsumowanie" Style="{StaticResource HeaderStyle}" />
                        <Grid ColumnDefinitions="Auto,Auto,Auto" RowDefinitions="Auto,Auto">
                            <Label Grid.Row="0" Grid.Column="1" Text="Liczba godzin" Style="{StaticResource ColumnHeaderStyle}" HorizontalOptions="End"/>
                            <Label Grid.Row="0" Grid.Column="2" Text="Liczba minut" Style="{StaticResource ColumnHeaderStyle}" HorizontalOptions="End"/>
                            <Label Grid.Row="1" Grid.Column="0" Text="Dyżury zrealizowane" />
                            <Label Grid.Row="1" Grid.Column="1" Text="{Binding Summary.TotalHours}" FontAttributes="Bold" HorizontalOptions="End" />
                            <Label Grid.Row="1" Grid.Column="2" Text="{Binding Summary.TotalMinutes}" FontAttributes="Bold" HorizontalOptions="End" />
                        </Grid>
                    </VerticalStackLayout>
                </Frame>
                <Frame BorderColor="LightGray" HasShadow="True" CornerRadius="10" Padding="15">
                    <VerticalStackLayout>
                        <Label Text="Lista dyżurów medycznych" Style="{StaticResource HeaderStyle}" />
                        <Label Text="Brak danych" IsVisible="{Binding Shifts.Count, Converter={StaticResource StringMatchConverter}, ConverterParameter='0'}" 
                               HorizontalOptions="Center" Margin="0,20" />
                        <CollectionView x:Name="ShiftsCollectionView" ItemsSource="{Binding Shifts}" Margin="0,10">
                            <CollectionView.Header>
                                <Label Text="Swipe w lewo aby usunąć dyżur" 
                                       HorizontalOptions="Center" 
                                       TextColor="{StaticResource TextMutedColor}" 
                                       FontSize="12" 
                                       Margin="0,0,0,10" />
                            </CollectionView.Header>
                            <CollectionView.ItemTemplate>
                                <DataTemplate x:DataType="models:RealizedMedicalShiftOldSMK">
                                    <SwipeView SwipeEnded="OnSwipeEnded">
                                        <SwipeView.RightItems>
                                            <SwipeItems>
                                                <SwipeItem Text="Usuń" 
                                                           BackgroundColor="{StaticResource DangerColor}" 
                                                           Invoked="OnDeleteInvoked" />
                                            </SwipeItems>
                                        </SwipeView.RightItems>
                                        <Frame Margin="0,5,0,5" Padding="12" BorderColor="LightGray" HasShadow="False" CornerRadius="8">
                                            <Grid RowDefinitions="Auto,Auto" ColumnDefinitions="*,Auto">
                                                <VerticalStackLayout Grid.Row="0" Grid.Column="0" Spacing="3">
                                                    <Grid ColumnDefinitions="Auto,*">
                                                        <Label Text="Rok szkolenia: " FontAttributes="Bold" TextColor="{StaticResource TextColor}" />
                                                        <Label Grid.Column="1" Text="{Binding Year, StringFormat='Rok {0}'}" TextColor="{StaticResource TextColor}" />
                                                    </Grid>
                                                    <Grid ColumnDefinitions="Auto,*">
                                                        <Label Text="Liczba godzin: " FontAttributes="Bold" TextColor="{StaticResource TextColor}" />
                                                        <StackLayout Grid.Column="1" Orientation="Horizontal">
                                                            <Label Text="{Binding Hours}" TextColor="{StaticResource TextColor}" />
                                                        </StackLayout>
                                                    </Grid>
                                                    <Grid ColumnDefinitions="Auto,*">
                                                        <Label Text="Liczba minut: " FontAttributes="Bold" TextColor="{StaticResource TextColor}" />
                                                        <StackLayout Grid.Column="1" Orientation="Horizontal">
                                                            <Label Text="{Binding Minutes}" TextColor="{StaticResource TextColor}" />
                                                        </StackLayout>
                                                    </Grid>
                                                    <Grid ColumnDefinitions="Auto,*">
                                                        <Label Text="Data rozpoczęcia: " FontAttributes="Bold" TextColor="{StaticResource TextColor}" />
                                                        <Label Grid.Column="1" Text="{Binding StartDate, StringFormat='{0:yyyy-MM-dd}'}" TextColor="{StaticResource TextColor}" />
                                                    </Grid>
                                                    <Grid ColumnDefinitions="Auto,*">
                                                        <Label Text="Nazwa komórki organizacyjnej: " FontAttributes="Bold" TextColor="{StaticResource TextColor}" />
                                                    </Grid>
                                                    <Grid ColumnDefinitions="Auto,*">
                                                        <Label Grid.Column="1" Text="{Binding Location}" LineBreakMode="TailTruncation" TextColor="{StaticResource TextColor}" />
                                                    </Grid>
                                                </VerticalStackLayout>
                                                <Button Grid.Row="0" Grid.Column="1"
                                                        Text="Edytuj"
                                                        Clicked="OnEditButtonClicked"
                                                        Style="{StaticResource OutlineButton}"
                                                        HeightRequest="40"
                                                        WidthRequest="80"
                                                        VerticalOptions="Center"
                                                        Margin="0,0,0,0" />
                                            </Grid>
                                        </Frame>
                                    </SwipeView>
                                </DataTemplate>
                            </CollectionView.ItemTemplate>
                            <CollectionView.EmptyView>
                                <VerticalStackLayout HorizontalOptions="Center" VerticalOptions="Center" Padding="20">
                                    <Label Text="Brak dodanych dyżurów" FontSize="16" HorizontalOptions="Center" TextColor="{StaticResource TextMutedColor}" />
                                    <Label Text="Użyj przycisku + aby dodać nowy dyżur" FontSize="14" HorizontalOptions="Center" TextColor="{StaticResource TextMutedColor}" Margin="0,10,0,0" />
                                </VerticalStackLayout>
                            </CollectionView.EmptyView>
                        </CollectionView>
                    </VerticalStackLayout>
                </Frame>
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
```

## File: Views/MedicalShifts/OldSMKMedicalShiftsPage.xaml.cs
```csharp
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class OldSMKMedicalShiftsPage : ContentPage
    {
        private readonly OldSMKMedicalShiftsListViewModel viewModel;
        private readonly IMedicalShiftsService medicalShiftsService;

        public OldSMKMedicalShiftsPage(OldSMKMedicalShiftsListViewModel viewModel, IMedicalShiftsService medicalShiftsService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.medicalShiftsService = medicalShiftsService;
            this.BindingContext = this.viewModel;

            this.viewModel.PropertyChanged += this.ViewModel_PropertyChanged;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!isFirstLoad)
            {
                return;
            }

            isFirstLoad = false;

            await Task.Delay(100);

            var method = this.viewModel.GetType().GetMethod("LoadYearsAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                await (Task)method.Invoke(this.viewModel, null);
                this.CreateYearButtons();
            }
            else
            {
                this.viewModel.RefreshCommand.Execute(null);
            }
        }

        private bool isFirstLoad = true;

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.viewModel.AvailableYears) ||
                e.PropertyName == nameof(this.viewModel.SelectedYear))
            {
                this.CreateYearButtons();
            }
        }

        private void CreateYearButtons()
        {
            this.YearsContainer.Children.Clear();

            foreach (var year in this.viewModel.AvailableYears)
            {
                var button = new Button
                {
                    Text = $"Rok {year}",
                    HeightRequest = 40,
                    WidthRequest = 90,
                    Margin = new Thickness(5),
                    TextColor = Colors.White,
                    BackgroundColor = year == this.viewModel.SelectedYear ? Color.FromArgb("#0D759C") : Color.FromArgb("#547E9E")
                };

                button.Clicked += (s, e) =>
                {
                    this.viewModel.SelectYearCommand.Execute(year);
                };

                this.YearsContainer.Children.Add(button);
            }
        }

        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            this.viewModel.AddShiftCommand.Execute(null);
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedMedicalShiftOldSMK shift)
            {

                try
                {
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "ShiftId", shift.ShiftId.ToString() },
                        { "YearParam", shift.Year.ToString() }
                    };

                    await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", navigationParameter);
                }
                catch (Exception ex)
                {
                    try
                    {
                        await Shell.Current.GoToAsync($"//medicalshifts/AddEditOldSMKMedicalShift?ShiftId={shift.ShiftId}&YearParam={shift.Year}");
                    }
                    catch (Exception ex2)
                    {
                        var viewModel = IPlatformApplication.Current.Services.GetService<AddEditOldSMKMedicalShiftViewModel>();
                        var page = new AddEditOldSMKMedicalShiftPage(viewModel);
                        viewModel.ShiftId = shift.ShiftId.ToString();
                        viewModel.YearParam = shift.Year.ToString();
                        await Navigation.PushAsync(page);
                    }
                }
            }
        }

        private async void OnDeleteInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is RealizedMedicalShiftOldSMK shift)
            {
                bool confirm = await DisplayAlert(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć ten dyżur?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);

                    if (success)
                    {
                        this.viewModel.Shifts.Remove(shift);

                        if (this.viewModel.SelectedYear > 0)
                        {
                            var summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.viewModel.SelectedYear);
                            this.viewModel.Summary = summary;
                        }

                        await DisplayAlert("Sukces", "Dyżur został usunięty", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Błąd", "Nie udało się usunąć dyżuru. Spróbuj ponownie.", "OK");
                    }
                }
            }
        }

        private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
        }
    }
}
```

## File: Views/Procedures/AddEditNewSMKProcedurePage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Procedures"
             x:DataType="vm:AddEditNewSMKProcedureViewModel"
             x:Class="SledzSpecke.App.Views.Procedures.AddEditNewSMKProcedurePage"
             Title="{Binding Title}">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="15">
            <Label Text="{Binding ProcedureName}" 
                   FontSize="16" 
                   FontAttributes="Bold" />
            <Grid ColumnDefinitions="*,*" ColumnSpacing="15">
                <VerticalStackLayout Grid.Column="0">
                    <Label Text="Wykonane samodzielnie *" FontAttributes="Bold" />
                    <Entry Text="{Binding CountA}" 
                           Keyboard="Numeric" 
                           Placeholder="0" />
                </VerticalStackLayout>

                <VerticalStackLayout Grid.Column="1">
                    <Label Text="Wykonane jako asysta" FontAttributes="Bold" />
                    <Entry Text="{Binding CountB}" 
                           Keyboard="Numeric" 
                           Placeholder="0" />
                </VerticalStackLayout>
            </Grid>
            <Label Text="Daty realizacji *" FontAttributes="Bold" />
            <Grid ColumnDefinitions="*,*" ColumnSpacing="15">
                <VerticalStackLayout Grid.Column="0">
                    <Label Text="Od:" />
                    <DatePicker Date="{Binding StartDate}" 
                              Format="dd.MM.yyyy" />
                </VerticalStackLayout>

                <VerticalStackLayout Grid.Column="1">
                    <Label Text="Do:" />
                    <DatePicker Date="{Binding EndDate}" 
                              Format="dd.MM.yyyy" />
                </VerticalStackLayout>
            </Grid>
            <Grid ColumnDefinitions="*,*" 
                  ColumnSpacing="15" 
                  Margin="0,20,0,0">
                <Button Grid.Column="0" 
                        Text="Anuluj" 
                        Command="{Binding CancelCommand}" />
                <Button Grid.Column="1" 
                        Text="Zapisz" 
                        Command="{Binding SaveCommand}" 
                        BackgroundColor="{StaticResource Primary}"
                        TextColor="White" />
            </Grid>
            <ActivityIndicator IsRunning="{Binding IsBusy}" 
                             IsVisible="{Binding IsBusy}" 
                             HorizontalOptions="Center" 
                             Margin="0,20,0,0" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

## File: Views/Procedures/AddEditNewSMKProcedurePage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class AddEditNewSMKProcedurePage : ContentPage
    {
        private readonly AddEditNewSMKProcedureViewModel viewModel;

        public AddEditNewSMKProcedurePage(AddEditNewSMKProcedureViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("..");
                })
            });
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
    }
}
```

## File: Views/Procedures/AddEditOldSMKProcedurePage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Procedures"
             x:DataType="vm:AddEditOldSMKProcedureViewModel"
             x:Class="SledzSpecke.App.Views.Procedures.AddEditOldSMKProcedurePage"
             Title="{Binding Title}">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="15">
            <Label Text="Data wykonania *" FontAttributes="Bold" />
            <DatePicker Date="{Binding Procedure.Date}" Format="yyyy-MM-dd" />
            <Label Text="Rok specjalizacji *" FontAttributes="Bold" />
            <Picker ItemsSource="{Binding YearOptions}" 
                    SelectedItem="{Binding SelectedYear}" 
                    ItemDisplayBinding="{Binding Value}" />
            <Label Text="Kod zabiegu *" FontAttributes="Bold" />
            <Picker ItemsSource="{Binding CodeOptions}" 
                    SelectedItem="{Binding SelectedCode}" 
                    ItemDisplayBinding="{Binding Value}" />
            <Label Text="Osoba wykonująca *" FontAttributes="Bold" />
            <Entry Text="{Binding PerformingPerson}" Placeholder="Podaj imię i nazwisko osoby wykonującej" />
            <Label Text="Miejsce wykonania *" FontAttributes="Bold" />
            <Entry Text="{Binding Location}" Placeholder="Np. Oddział Kardiologii" />
            <StackLayout>
                <Label Text="Staż *" FontAttributes="Bold" />
                <Picker ItemsSource="{Binding AvailableInternships}" 
                        SelectedItem="{Binding SelectedInternship}" 
                        ItemDisplayBinding="{Binding InternshipName}"
                        IsEnabled="{Binding IsInternshipSelectionEnabled}" />
                <Label Text="{Binding InternshipSelectionHint}" 
                       TextColor="{StaticResource TextMutedColor}"
                       FontSize="12"
                       IsVisible="{Binding InternshipSelectionHint, Converter={StaticResource NotNullConverter}}" />
            </StackLayout>
            <Label Text="Inicjały pacjenta *" FontAttributes="Bold" />
            <Entry Text="{Binding PatientInitials}" Placeholder="Np. AB" MaxLength="10" />
            <Label Text="Płeć pacjenta *" FontAttributes="Bold" />
            <Picker ItemsSource="{Binding GenderOptions}" 
                    SelectedItem="{Binding SelectedGender}" 
                    ItemDisplayBinding="{Binding Value}" />
            <Label Text="Dane osoby wykonującej I i II asystę (opcjonalne)" FontAttributes="Bold" />
            <Entry Text="{Binding AssistantData}" Placeholder="Opcjonalne" />
            <Label Text="Procedura z grupy (opcjonalne)" FontAttributes="Bold" />
            <Entry Text="{Binding ProcedureGroup}" Placeholder="Opcjonalne" />
            <Grid ColumnDefinitions="*,*" ColumnSpacing="15" Margin="0,15,0,0">
                <Button Grid.Column="0" Text="Anuluj" Command="{Binding CancelCommand}" />
                <Button x:Name="BtnSave" Grid.Column="1" Text="Zapisz" Command="{Binding SaveCommand}" 
                        BackgroundColor="{StaticResource Primary}" TextColor="White" />
            </Grid>
            <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" 
                              HorizontalOptions="Center" Margin="0,20,0,0" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

## File: Views/Procedures/AddEditOldSMKProcedurePage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class AddEditOldSMKProcedurePage : ContentPage
    {
        private readonly AddEditOldSMKProcedureViewModel viewModel;

        public AddEditOldSMKProcedurePage(AddEditOldSMKProcedureViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
            this.BtnSave.Clicked += this.OnSaveClicked;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await this.viewModel.OnSaveAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await this.viewModel.InitializeAsync();
        }
    }
}
```

## File: Views/Procedures/NewSMKProceduresListPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Procedures"
             xmlns:models="clr-namespace:SledzSpecke.App.Models"
             x:DataType="vm:NewSMKProceduresListViewModel"
             x:Class="SledzSpecke.App.Views.Procedures.NewSMKProceduresListPage"
             Title="{Binding Title}">
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="HeaderText" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource Primary}" />
                <Setter Property="Margin" Value="0,10,0,5" />
            </Style>
            <Style x:Key="RequirementTitle" TargetType="Label">
                <Setter Property="FontSize" Value="14" />
                <Setter Property="LineBreakMode" Value="WordWrap" />
            </Style>
            <Style x:Key="TableHeader" TargetType="Label">
                <Setter Property="FontSize" Value="12" />
                <Setter Property="TextColor" Value="{StaticResource TextMutedColor}" />
                <Setter Property="HorizontalTextAlignment" Value="Center" />
            </Style>
            <Style x:Key="TableCell" TargetType="Label">
                <Setter Property="FontSize" Value="14" />
                <Setter Property="HorizontalTextAlignment" Value="Center" />
            </Style>
            <Style x:Key="LightIconButton" TargetType="Button">
                <Setter Property="BackgroundColor" Value="Transparent" />
                <Setter Property="BorderColor" Value="Transparent" />
                <Setter Property="HeightRequest" Value="30" />
                <Setter Property="WidthRequest" Value="30" />
                <Setter Property="Padding" Value="0" />
                <Setter Property="CornerRadius" Value="15" />
                <Setter Property="BorderWidth" Value="0" />
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
    <RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Padding="20" Spacing="15">
                <Label Text="{Binding ModuleTitle}" Style="{StaticResource HeaderText}" />
                <StackLayout IsVisible="{Binding HasTwoModules}" 
                           Orientation="Horizontal" 
                           HorizontalOptions="Center" 
                           Margin="0,10" 
                           Spacing="10">
                    <Label Text="Aktywny moduł:" 
                           VerticalOptions="Center" 
                           FontAttributes="Bold" />
                    <Frame Padding="0" 
                           CornerRadius="5" 
                           BorderColor="{StaticResource Primary}" 
                           HasShadow="False">
                        <Grid ColumnDefinitions="*,*">
                            <Button Text="Podstawowy" 
                                    BackgroundColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Basic" />
                            <Button Grid.Column="1" 
                                    Text="Specjalistyczny" 
                                    BackgroundColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Specialistic" />
                        </Grid>
                    </Frame>
                </StackLayout>
                <CollectionView ItemsSource="{Binding ProcedureRequirements}"
                              RemainingItemsThreshold="2"
                              RemainingItemsThresholdReachedCommand="{Binding LoadMoreCommand}">
                    <CollectionView.EmptyView>
                        <StackLayout HorizontalOptions="Center" VerticalOptions="Center">
                            <ActivityIndicator IsRunning="{Binding IsLoading}" 
                                             IsVisible="{Binding IsLoading}" />
                            <Label Text="Brak procedur do wyświetlenia" 
                                   IsVisible="{Binding IsLoading, Converter={StaticResource InvertedBoolConverter}}"
                                   TextColor="{StaticResource TextMutedColor}" />
                        </StackLayout>
                    </CollectionView.EmptyView>
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:ProcedureRequirementViewModel">
                            <Frame Style="{StaticResource CardFrame}" 
                                   Margin="0,5" 
                                   Padding="15">
                                <VerticalStackLayout Spacing="10">
                                    <Grid ColumnDefinitions="*,Auto">
                                        <Label Text="{Binding Title}" 
                                               Style="{StaticResource RequirementTitle}" />
                                        <Button Grid.Column="1" 
                                                Text="▼"
                                                Command="{Binding ToggleExpandCommand}"
                                                HeightRequest="30"
                                                WidthRequest="30"
                                                Padding="0"
                                                CornerRadius="15"
                                                BackgroundColor="{StaticResource Primary}"
                                                TextColor="White" />
                                    </Grid>
                                    <VerticalStackLayout IsVisible="{Binding IsExpanded}" 
                                                       Spacing="15">
                                        <VerticalStackLayout>
                                            <Label Text="Zabiegi/procedury wykonane samodzielnie" 
                                                   FontAttributes="Bold" />
                                            <Grid ColumnDefinitions="*,*,*">
                                                <VerticalStackLayout Grid.Column="0">
                                                    <Label Text="Wymagane" 
                                                           Style="{StaticResource TableHeader}" />
                                                    <Label Text="{Binding Statistics.RequiredCountA}" 
                                                           Style="{StaticResource TableCell}" />
                                                </VerticalStackLayout>
                                                <VerticalStackLayout Grid.Column="1">
                                                    <Label Text="Wprowadzone" 
                                                           Style="{StaticResource TableHeader}" />
                                                    <Label Text="{Binding Statistics.CompletedCountA}" 
                                                           Style="{StaticResource TableCell}" />
                                                </VerticalStackLayout>
                                                <VerticalStackLayout Grid.Column="2">
                                                    <Label Text="Pozostało do zrealizowania" 
                                                           Style="{StaticResource TableHeader}" />
                                                    <Label Text="{Binding Statistics.RemainingCountA}" 
                                                           Style="{StaticResource TableCell}" />
                                                </VerticalStackLayout>
                                            </Grid>
                                        </VerticalStackLayout>
                                        <VerticalStackLayout>
                                            <Label Text="Zabiegi/procedury wykonane jako pierwsza asysta" 
                                                   FontAttributes="Bold" />
                                            <Grid ColumnDefinitions="*,*,*">
                                                <VerticalStackLayout Grid.Column="0">
                                                    <Label Text="Wymagane" 
                                                           Style="{StaticResource TableHeader}" />
                                                    <Label Text="{Binding Statistics.RequiredCountB}" 
                                                           Style="{StaticResource TableCell}" />
                                                </VerticalStackLayout>
                                                <VerticalStackLayout Grid.Column="1">
                                                    <Label Text="Wprowadzone" 
                                                           Style="{StaticResource TableHeader}" />
                                                    <Label Text="{Binding Statistics.CompletedCountB}" 
                                                           Style="{StaticResource TableCell}" />
                                                </VerticalStackLayout>
                                                <VerticalStackLayout Grid.Column="2">
                                                    <Label Text="Pozostało do zrealizowania" 
                                                           Style="{StaticResource TableHeader}" />
                                                    <Label Text="{Binding Statistics.RemainingCountB}" 
                                                           Style="{StaticResource TableCell}" />
                                                </VerticalStackLayout>
                                            </Grid>
                                        </VerticalStackLayout>
                                        <Button Text="Dodaj realizację"
                                                Command="{Binding ToggleAddRealizationCommand}"
                                                IsVisible="{Binding IsAddingRealization, Converter={StaticResource InvertedBoolConverter}}"
                                                Style="{StaticResource PrimaryButton}"
                                                HorizontalOptions="Start" />
                                        <VerticalStackLayout IsVisible="{Binding IsExpanded}">
                                            <ActivityIndicator IsRunning="{Binding IsLoading}" 
                                                               IsVisible="{Binding IsLoading}" 
                                                               HorizontalOptions="Center" />
                                            <Label Text="Lista realizacji" 
                                                   FontAttributes="Bold" 
                                                   Margin="0,0,0,0"
                                                   IsVisible="{Binding HasRealizations}" />
                                            <Grid ColumnDefinitions="74,50,80,Auto" 
                                                    Padding="5"
                                                    IsVisible="{Binding HasRealizations}">
                                                <Label Text="Samodzielnie" 
                                                        Style="{StaticResource TableHeader}" 
                                                        Grid.Column="0" />
                                                <Label Text="Asysta" 
                                                        Style="{StaticResource TableHeader}" 
                                                        Grid.Column="1" />
                                                <Label Text="Daty realizacji" 
                                                       Style="{StaticResource TableHeader}" 
                                                       Grid.Column="2" />
                                            </Grid>
                                            <CollectionView ItemsSource="{Binding Realizations}"
                                                             IsVisible="{Binding HasRealizations}">
                                                <CollectionView.ItemTemplate>
                                                    <DataTemplate x:DataType="models:RealizedProcedureNewSMK">
                                                        <Grid ColumnDefinitions="74,50,80,Auto" 
                                                              Padding="5">
                                                            <Label Text="{Binding CountA}" 
                                                                    Grid.Column="0" 
                                                                    Style="{StaticResource TableCell}" />
                                                            <Label Text="{Binding CountB}" 
                                                                    Grid.Column="1" 
                                                                    Style="{StaticResource TableCell}" />
                                                            <Label Text="{Binding DateRange}" 
                                                                   Grid.Column="2" 
                                                                   Style="{StaticResource TableCell}" />
                                                            <StackLayout Grid.Column="3" 
                                                                        Orientation="Horizontal"
                                                                        Margin="10,0,0,0">
                                                                <Button Text="✏️" 
                                                                        Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=BindingContext.EditItemCommand}"
                                                                        CommandParameter="{Binding .}"
                                                                        BackgroundColor="LightGoldenrodYellow"
                                                                        TextColor="White"
                                                                        BorderColor="Orange"
                                                                        BorderWidth="1"
                                                                        Style="{StaticResource LightIconButton}" />
                                                                <Button Text="❌" 
                                                                        Command="{Binding Source={RelativeSource AncestorType={x:Type vm:ProcedureRequirementViewModel}}, Path=DeleteRealizationCommand}"
                                                                        CommandParameter="{Binding .}"
                                                                        BackgroundColor="LightPink"
                                                                        TextColor="LightPink"
                                                                        Style="{StaticResource LightIconButton}" />
                                                            </StackLayout>
                                                        </Grid>
                                                    </DataTemplate>
                                                </CollectionView.ItemTemplate>
                                                <CollectionView.EmptyView>
                                                    <Label Text="Brak realizacji" 
                                                           TextColor="{StaticResource TextMutedColor}"
                                                           HorizontalOptions="Center" />
                                                </CollectionView.EmptyView>
                                            </CollectionView>
                                        </VerticalStackLayout>
                                    </VerticalStackLayout>
                                </VerticalStackLayout>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
```

## File: Views/Procedures/NewSMKProceduresListPage.xaml.cs
```csharp
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class NewSMKProceduresListPage : ContentPage
    {
        private readonly NewSMKProceduresListViewModel viewModel;
        private readonly IProcedureService procedureService;

        public NewSMKProceduresListPage(NewSMKProceduresListViewModel viewModel, IProcedureService procedureService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.procedureService = procedureService;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (this.viewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
```

## File: Views/Procedures/OldSMKProceduresListPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Procedures"
             xmlns:models="clr-namespace:SledzSpecke.App.Models"
             x:DataType="vm:OldSMKProceduresListViewModel"
             x:Class="SledzSpecke.App.Views.Procedures.OldSMKProceduresListPage"
             Title="{Binding Title}">
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="HeaderStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource Primary}" />
                <Setter Property="Margin" Value="0,10,0,5" />
            </Style>
            <Style x:Key="ColumnHeaderStyle" TargetType="Label">
                <Setter Property="FontSize" Value="12" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource TextMutedColor}" />
                <Setter Property="Margin" Value="0,0,5,0" />
                <Setter Property="LineBreakMode" Value="WordWrap" />
            </Style>
            <Style x:Key="LightIconButton" TargetType="Button">
                <Setter Property="BackgroundColor" Value="Transparent" />
                <Setter Property="BorderColor" Value="Transparent" />
                <Setter Property="HeightRequest" Value="30" />
                <Setter Property="WidthRequest" Value="30" />
                <Setter Property="Padding" Value="0" />
                <Setter Property="CornerRadius" Value="15" />
                <Setter Property="BorderWidth" Value="0" />
            </Style>
            <Style x:Key="StandardButton" TargetType="Button">
                <Setter Property="Padding" Value="10,5" />
                <Setter Property="CornerRadius" Value="5" />
                <Setter Property="BorderWidth" Value="0" />
                <Setter Property="FontAttributes" Value="Bold" />
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Dodaj" Command="{Binding AddProcedureCommand}" IconImageSource="add.png" />
    </ContentPage.ToolbarItems>
    <RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Spacing="20" Padding="20">
                <Label Text="{Binding ModuleTitle}" FontSize="20" FontAttributes="Bold" />
                <StackLayout IsVisible="{Binding HasTwoModules}" Orientation="Horizontal" 
                             HorizontalOptions="Center" Margin="0,10" Spacing="10">
                    <Label Text="Aktywny moduł:" VerticalOptions="Center" FontAttributes="Bold" />
                    <Frame Padding="0" CornerRadius="5" BorderColor="{StaticResource Primary}" 
                           HasShadow="False" IsClippedToBounds="True">
                        <Grid ColumnDefinitions="*,*">
                            <Button Text="Podstawowy" 
                                    BackgroundColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding BasicModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Basic"
                                    Style="{StaticResource StandardButton}" />
                            <Button Grid.Column="1" Text="Specjalistyczny" 
                                    BackgroundColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToColorConverter}}"
                                    TextColor="{Binding SpecialisticModuleSelected, Converter={StaticResource BoolToTextColorConverter}}"
                                    Command="{Binding SelectModuleCommand}" 
                                    CommandParameter="Specialistic"
                                    Style="{StaticResource StandardButton}" />
                        </Grid>
                    </Frame>
                </StackLayout>
                <Frame BorderColor="LightGray" HasShadow="True" CornerRadius="10" Padding="15">
                    <StackLayout Spacing="10">
                        <Label Text="Podsumowanie procedur" FontSize="16" FontAttributes="Bold" 
                               TextColor="{StaticResource Primary}" Margin="0,0,0,10"/>
                        <Grid ColumnDefinitions="Auto" RowDefinitions="Auto,Auto,Auto" Margin="0,0,0,10">
                            <Label Grid.Row="0" Grid.Column="0" Text="A - operator" FontAttributes="Bold" />
                            <StackLayout Grid.Row="1" Grid.Column="0" Orientation="Horizontal">
                                <Label Text="Liczba procedur: " FontSize="12" />
                                <Label Text="{Binding Summary.RequiredCountA}" FontAttributes="Bold" FontSize="12"/>
                            </StackLayout>
                            <StackLayout Grid.Row="2" Grid.Column="0" Orientation="Horizontal">
                                <Label Text="Liczba wykonanych procedur: " FontSize="12" />
                                <Label Text="{Binding Summary.CompletedCountA}" FontAttributes="Bold" FontSize="12"/>
                            </StackLayout>
                        </Grid>
                        <Grid ColumnDefinitions="Auto" RowDefinitions="Auto,Auto,Auto" Margin="0,0,0,10">
                            <Label Grid.Row="0" Grid.Column="0" Text="B - asysta" FontAttributes="Bold" />
                            <StackLayout Grid.Row="1" Grid.Column="0" Orientation="Horizontal">
                                <Label Text="Liczba procedur: " FontSize="12" />
                                <Label Text="{Binding Summary.RequiredCountB}" FontAttributes="Bold" FontSize="12" />
                            </StackLayout>
                            <StackLayout Grid.Row="2" Grid.Column="0" Orientation="Horizontal">
                                <Label Text="Liczba wykonanych procedur: " FontSize="12" />
                                <Label Text="{Binding Summary.CompletedCountB}" FontAttributes="Bold" FontSize="12" />
                            </StackLayout>
                        </Grid>
                    </StackLayout>
                </Frame>
                <Label Text="Lista procedur" FontSize="16" FontAttributes="Bold" 
                       TextColor="{StaticResource Primary}" Margin="0,10,0,5"/>
                <CollectionView ItemsSource="{Binding ProcedureGroups}">
                    <CollectionView.EmptyView>
                        <Label Text="Brak procedur do wyświetlenia" 
                               HorizontalOptions="Center"
                               TextColor="{StaticResource TextMutedColor}" />
                    </CollectionView.EmptyView>
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:ProcedureGroupViewModel">
                            <Frame Margin="0,5" Padding="15" BorderColor="LightGray" HasShadow="True" CornerRadius="10">
                                <VerticalStackLayout>
                                    <Grid ColumnDefinitions="*,Auto">
                                        <VerticalStackLayout Grid.Column="0">
                                            <Label Text="{Binding Title}" FontSize="16" FontAttributes="Bold" />
                                            <Label Text="{Binding StatsInfo}" TextColor="{StaticResource TextMutedColor}" FontSize="12" />
                                        </VerticalStackLayout>
                                        <Button Grid.Column="1" 
                                                Command="{Binding ToggleExpandCommand}"
                                                Text="▼"
                                                WidthRequest="40"
                                                HeightRequest="40"
                                                Padding="0"
                                                CornerRadius="20"
                                                BackgroundColor="{StaticResource Primary}" 
                                                TextColor="White" />
                                    </Grid>
                                    <StackLayout IsVisible="{Binding IsExpanded}" Margin="0,10,0,0">
                                        <ScrollView Orientation="Horizontal" HorizontalScrollBarVisibility="Always">
                                            <Grid>
                                                <Grid.RowDefinitions>
                                                    <RowDefinition Height="Auto"/>
                                                    <RowDefinition Height="Auto"/>
                                                    <RowDefinition Height="*"/>
                                                </Grid.RowDefinitions>
                                                <Grid.ColumnDefinitions>
                                                    <ColumnDefinition Width="85"/>
                                                    <ColumnDefinition Width="26"/>
                                                    <ColumnDefinition Width="87"/>
                                                    <ColumnDefinition Width="90"/>
                                                    <ColumnDefinition Width="140"/>
                                                    <ColumnDefinition Width="70"/>
                                                    <ColumnDefinition Width="90"/>
                                                </Grid.ColumnDefinitions>
                                                <Label Grid.Row="0" Grid.Column="0" Text="Data" Style="{StaticResource ColumnHeaderStyle}" HorizontalTextAlignment="Center" />
                                                <Label Grid.Row="0" Grid.Column="1" Text="Rok" Style="{StaticResource ColumnHeaderStyle}" HorizontalTextAlignment="Center"/>
                                                <Label Grid.Row="0" Grid.Column="2" Text="Kod" Style="{StaticResource ColumnHeaderStyle}" HorizontalTextAlignment="Center"/>
                                                <Label Grid.Row="0" Grid.Column="3" Text="Miejsce" Style="{StaticResource ColumnHeaderStyle}" HorizontalTextAlignment="Center"/>
                                                <Label Grid.Row="0" Grid.Column="4" Text="Nazwa stażu" Style="{StaticResource ColumnHeaderStyle}" HorizontalTextAlignment="Center"/>
                                                <Label Grid.Row="0" Grid.Column="5" Text="Inicjały/Płeć" Style="{StaticResource ColumnHeaderStyle}" HorizontalTextAlignment="Center"/>
                                                <Label Grid.Row="0" Grid.Column="6" Text="Akcje" Style="{StaticResource ColumnHeaderStyle}" HorizontalTextAlignment="Center" />
                                                <ActivityIndicator Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="8" 
                                                                   IsRunning="{Binding IsLoading}" IsVisible="{Binding IsLoading}"
                                                                   HorizontalOptions="Center" Margin="0,10" />
                                                <CollectionView Grid.Row="2" Grid.Column="0" Grid.ColumnSpan="8" 
                                                                ItemsSource="{Binding Procedures}">
                                                    <CollectionView.EmptyView>
                                                        <Label Text="Brak procedur w tej grupie" Margin="0,10" TextColor="{StaticResource TextMutedColor}" />
                                                    </CollectionView.EmptyView>
                                                    <CollectionView.ItemTemplate>
                                                        <DataTemplate x:DataType="models:RealizedProcedureOldSMK">
                                                            <Grid>
                                                                <Grid.ColumnDefinitions>
                                                                    <ColumnDefinition Width="85"/>
                                                                    <ColumnDefinition Width="26"/>
                                                                    <ColumnDefinition Width="87"/>
                                                                    <ColumnDefinition Width="90"/>
                                                                    <ColumnDefinition Width="140"/>
                                                                    <ColumnDefinition Width="70"/>
                                                                    <ColumnDefinition Width="90"/>
                                                                </Grid.ColumnDefinitions>
                                                                <Label Grid.Column="0" LineBreakMode="WordWrap" Text="{Binding Date, StringFormat='{0:yyyy-MM-dd}'}" Margin="5,0" />
                                                                <Label Grid.Column="1" LineBreakMode="WordWrap" Text="{Binding Year}" Margin="5,0" />
                                                                <Label Grid.Column="2" LineBreakMode="WordWrap" Text="{Binding Code}" Margin="5,0" />
                                                                <Label Grid.Column="3" LineBreakMode="WordWrap" Text="{Binding Location}" Margin="5,0" />
                                                                <Label Grid.Column="4" LineBreakMode="WordWrap" Text="{Binding InternshipName}" Margin="5,0" />
                                                                <StackLayout Grid.Column="5" Orientation="Horizontal" Margin="5,0">
                                                                    <Label Text="{Binding PatientInitials}" />
                                                                    <Label Text="/" />
                                                                    <Label Text="{Binding PatientGender}" />
                                                                </StackLayout>
                                                                <HorizontalStackLayout Grid.Column="6" Spacing="0" Margin="0,0">
                                                                    <Button Text="✏️" 
                                                                            Clicked="OnEditButtonClicked"
                                                                            BackgroundColor="LightGoldenrodYellow"
                                                                            TextColor="White"
                                                                            BorderColor="Orange"
                                                                            BorderWidth="1"
                                                                            Style="{StaticResource LightIconButton}" />
                                                                    <Button Text="❌" 
                                                                            Clicked="OnDeleteButtonClicked"
                                                                            BackgroundColor="LightPink"
                                                                            TextColor="LightPink"
                                                                            Style="{StaticResource LightIconButton}" />
                                                                </HorizontalStackLayout>
                                                            </Grid>
                                                        </DataTemplate>
                                                    </CollectionView.ItemTemplate>
                                                </CollectionView>
                                            </Grid>
                                        </ScrollView>
                                        <Button Text="Dodaj procedurę" 
                                                Command="{Binding AddProcedureCommand}"
                                                Style="{StaticResource PrimaryButton}"
                                                HorizontalOptions="Start"
                                                Margin="0,10,0,0" />
                                    </StackLayout>
                                </VerticalStackLayout>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>
</ContentPage>
```

## File: Views/Procedures/OldSMKProceduresListPage.xaml.cs
```csharp
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class OldSMKProceduresListPage : ContentPage
    {
        private readonly OldSMKProceduresListViewModel viewModel;
        private readonly IProcedureService procedureService;

        public OldSMKProceduresListPage(OldSMKProceduresListViewModel viewModel, IProcedureService procedureService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.procedureService = procedureService;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (this.viewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedProcedureOldSMK procedure)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", procedure.ProcedureId.ToString() },
                    { "IsEdit", "true" },
                    { "Date", procedure.Date.ToString("o") },
                    { "Year", procedure.Year.ToString() },
                    { "Code", procedure.Code ?? string.Empty },
                    { "PerformingPerson", procedure.PerformingPerson ?? string.Empty },
                    { "Location", procedure.Location ?? string.Empty },
                    { "PatientInitials", procedure.PatientInitials ?? string.Empty },
                    { "PatientGender", procedure.PatientGender ?? string.Empty },
                    { "AssistantData", procedure.AssistantData ?? string.Empty },
                    { "ProcedureGroup", procedure.ProcedureGroup ?? string.Empty },
                    { "InternshipId", procedure.InternshipId.ToString() },
                    { "InternshipName", procedure.InternshipName ?? string.Empty }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is RealizedProcedureOldSMK procedure)
            {
                bool confirm = await DisplayAlert(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę procedurę?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    if (button.Parent?.Parent?.BindingContext is RealizedProcedureOldSMK &&
                        button.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.BindingContext is ProcedureGroupViewModel groupViewModel)
                    {
                        await groupViewModel.OnDeleteProcedure(procedure);
                    }
                    else
                    {
                        bool success = await this.procedureService.DeleteOldSMKProcedureAsync(procedure.ProcedureId);

                        if (success)
                        {
                            this.viewModel.RefreshCommand.Execute(null);
                            await DisplayAlert("Sukces", "Procedura została usunięta.", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Błąd", "Nie udało się usunąć procedury. Spróbuj ponownie.", "OK");
                        }
                    }
                }
            }
        }
    }
}
```

## File: Views/Procedures/ProcedureSelectorPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SledzSpecke.App.ViewModels.Procedures"
             x:DataType="vm:ProcedureSelectorViewModel"
             x:Class="SledzSpecke.App.Views.Procedures.ProcedureSelectorPage"
             Title="{Binding Title}">
    <VerticalStackLayout Padding="20" Spacing="20" VerticalOptions="Center" HorizontalOptions="Center">
        <ActivityIndicator IsRunning="True" />
        <Label Text="Ładowanie procedur..." HorizontalOptions="Center" />
    </VerticalStackLayout>
</ContentPage>
```

## File: Views/Procedures/ProcedureSelectorPage.xaml.cs
```csharp
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class ProcedureSelectorPage : ContentPage
    {
        private readonly ProcedureSelectorViewModel viewModel;

        public ProcedureSelectorPage(ProcedureSelectorViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await this.viewModel.InitializeAsync();
        }
    }
}
```

## File: App.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:SledzSpecke.App"
             xmlns:converters="clr-namespace:SledzSpecke.App.Converters"
             x:Class="SledzSpecke.App.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
                <ResourceDictionary Source="Resources/Styles/MaterialDesignStyles.xaml" />
            </ResourceDictionary.MergedDictionaries>
            <Color x:Key="PrimaryColor">#24C1DE</Color>
            <Color x:Key="SecondaryColor">#0D759C</Color>
            <Color x:Key="AccentColor">#30DDE8</Color>
            <Color x:Key="SuccessColor">#4AF5F7</Color>
            <Color x:Key="WarningColor">#F59E0B</Color>
            <Color x:Key="DangerColor">#EF4444</Color>
            <Color x:Key="BackgroundColor">#E4F0F5</Color>
            <Color x:Key="TextColor">#082044</Color>
            <Color x:Key="TextMutedColor">#547E9E</Color>
            <converters:BoolToColorConverter x:Key="BoolToColorConverter" />
            <converters:BoolToTextColorConverter x:Key="BoolToTextColorConverter" />
            <converters:CodeToColorConverter x:Key="CodeToColorConverter" />
            <converters:StringToBoolConverter x:Key="StringToBoolConverter" />
            <converters:NotNullConverter x:Key="NotNullConverter" />
            <converters:InvertedBoolConverter x:Key="InvertedBoolConverter" />
            <converters:BoolToYesNoConverter x:Key="BoolToYesNoConverter" />
            <converters:StringMatchConverter x:Key="StringMatchConverter" />
            <converters:StringMatchToBgColorConverter x:Key="StringMatchToBgColorConverter" />
            <converters:StringMatchToTextColorConverter x:Key="StringMatchToTextColorConverter" />
            <converters:BoolToBackgroundColorConverter x:Key="BoolToBackgroundColorConverter" />
            <converters:StatusToColorConverter x:Key="StatusToColorConverter" />
            <converters:StringToIntConverter x:Key="StringToIntConverter" />
            <converters:BoolToTextConverter x:Key="BoolToTextConverter" />
            <converters:DateRangeConverter x:Key="DateRangeConverter" />
            <converters:ItemCountToHeightConverter x:Key="ItemCountToHeightConverter" />
            <Style TargetType="Button" x:Key="PrimaryButton" BasedOn="{StaticResource MaterialButtonPrimary}">
                <Setter Property="BackgroundColor" Value="{StaticResource PrimaryColor}" />
                <Setter Property="TextColor" Value="White" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="CornerRadius" Value="8" />
                <Setter Property="Padding" Value="16,10" />
            </Style>
            <Style TargetType="Button" x:Key="SecondaryButton" BasedOn="{StaticResource MaterialButtonSecondary}">
                <Setter Property="BackgroundColor" Value="{StaticResource SecondaryColor}" />
                <Setter Property="TextColor" Value="White" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="CornerRadius" Value="8" />
                <Setter Property="Padding" Value="16,10" />
            </Style>
            <Style TargetType="Button" x:Key="OutlineButton" BasedOn="{StaticResource MaterialButtonOutlined}">
                <Setter Property="BackgroundColor" Value="Transparent" />
                <Setter Property="TextColor" Value="{StaticResource PrimaryColor}" />
                <Setter Property="BorderColor" Value="{StaticResource PrimaryColor}" />
                <Setter Property="BorderWidth" Value="1" />
                <Setter Property="CornerRadius" Value="8" />
                <Setter Property="Padding" Value="16,10" />
            </Style>
            <Style TargetType="Button" x:Key="DangerButton" BasedOn="{StaticResource MaterialButtonDanger}">
                <Setter Property="BackgroundColor" Value="{StaticResource DangerColor}" />
                <Setter Property="TextColor" Value="White" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="CornerRadius" Value="8" />
                <Setter Property="Padding" Value="16,10" />
            </Style>
            <Style TargetType="Frame" x:Key="CardFrame" BasedOn="{StaticResource MaterialCard}">
                <Setter Property="BackgroundColor" Value="White" />
                <Setter Property="CornerRadius" Value="10" />
                <Setter Property="BorderColor" Value="#95B8CE" />
                <Setter Property="Padding" Value="16" />
                <Setter Property="HasShadow" Value="True" />
                <Setter Property="Margin" Value="0,8" />
            </Style>
            <Style TargetType="Label" x:Key="HeaderLabel" BasedOn="{StaticResource MaterialLabelHeadline5}">
                <Setter Property="FontSize" Value="24" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource TextColor}" />
                <Setter Property="Margin" Value="0,16,0,8" />
            </Style>
            <Style TargetType="Label" x:Key="SubHeaderLabel" BasedOn="{StaticResource MaterialLabelHeadline6}">
                <Setter Property="FontSize" Value="18" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="TextColor" Value="{StaticResource TextColor}" />
                <Setter Property="Margin" Value="0,8,0,4" />
            </Style>
            <Style TargetType="ProgressBar" x:Key="CustomProgressBar">
                <Setter Property="ProgressColor" Value="{StaticResource SuccessColor}" />
                <Setter Property="HeightRequest" Value="10" />
                <Setter Property="VerticalOptions" Value="Center" />
            </Style>
            <Style TargetType="Button" x:Key="SMKReportButton" BasedOn="{StaticResource MaterialButtonPrimary}">
                <Setter Property="BackgroundColor" Value="#082044" />
                <Setter Property="TextColor" Value="White" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="CornerRadius" Value="8" />
                <Setter Property="Padding" Value="16,10" />
            </Style>
            <Style TargetType="Button" x:Key="SettingsButton" BasedOn="{StaticResource MaterialButtonSecondary}">
                <Setter Property="BackgroundColor" Value="#547E9E" />
                <Setter Property="TextColor" Value="White" />
                <Setter Property="FontAttributes" Value="None" />
                <Setter Property="CornerRadius" Value="8" />
                <Setter Property="Padding" Value="16,10" />
            </Style>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## File: App.xaml.cs
```csharp
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.App.Services.Exceptions;

namespace SledzSpecke.App
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            Helpers.Constants.SetFileSystemService(new FileSystemService());
            Helpers.SettingsHelper.SetSecureStorageService(new SecureStorageService());

            // Dodajemy obsługę nieobsłużonych wyjątków
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            // Obsługa platformowo-specyficznych wyjątków
#if ANDROID
            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#endif
#if IOS
            ObjCRuntime.Runtime.MarshalManagedException += OnIOSMarshalManagedException;
#endif
        }

        protected override async void OnStart()
        {
            base.OnStart();

            Task.Run(async () =>
            {
                var dbService = IPlatformApplication.Current.Services.GetRequiredService<IDatabaseService>();
                await dbService.MigrateShiftDataForModulesAsync();
                await dbService.MigrateInternshipDataAsync();
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var splashPage = IPlatformApplication.Current.Services.GetRequiredService<SplashPage>();
            return new Window(splashPage);
        }

        // Nowe metody do obsługi nieobsłużonych wyjątków
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            LogAndHandleException(exception, "Nieobsłużony wyjątek aplikacji");
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogAndHandleException(e.Exception, "Nieobsłużony wyjątek zadania");
            e.SetObserved(); // Oznacza wyjątek jako obsłużony
        }

#if ANDROID
        private void OnAndroidUnhandledException(object sender, Android.Runtime.RaiseThrowableEventArgs e)
        {
            LogAndHandleException(new Exception(e.Exception.ToString()), "Nieobsłużony wyjątek Android");
            e.Handled = true; // Oznacza wyjątek jako obsłużony
        }
#endif

#if IOS
        private void OnIOSMarshalManagedException(object sender, ObjCRuntime.MarshalManagedExceptionEventArgs e)
        {
            LogAndHandleException(e.Exception, "Nieobsłużony wyjątek iOS");
            e.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode; // Obsłuż wyjątek
        }
#endif

        private void LogAndHandleException(Exception exception, string source)
        {
            try
            {
                var exceptionHandler = IPlatformApplication.Current.Services.GetService<IExceptionHandlerService>();

                if (exceptionHandler != null)
                {
                    // Asynchronicznie obsługujemy wyjątek, ale nie możemy czekać (await) 
                    // w kontekście tych handlerów
                    Task.Run(() => exceptionHandler.HandleExceptionAsync(exception, source)).Wait();
                }
                else
                {
                    // Awaryjne logowanie, jeśli serwis nie jest dostępny
                    System.Diagnostics.Debug.WriteLine($"KRYTYCZNY BŁĄD ({source}): {exception.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {exception.StackTrace}");
                }
            }
            catch (Exception logEx)
            {
                // Ostateczne zabezpieczenie - w przypadku błędu podczas logowania
                System.Diagnostics.Debug.WriteLine($"BŁĄD PODCZAS LOGOWANIA WYJĄTKU: {logEx.Message}");
                System.Diagnostics.Debug.WriteLine($"ORYGINALNY WYJĄTEK: {exception?.Message}");
            }
        }
    }
}
```

## File: AppShell.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="SledzSpecke.App.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:SledzSpecke.App"
    xmlns:dashboard="clr-namespace:SledzSpecke.App.Views.Dashboard"
    xmlns:procedures="clr-namespace:SledzSpecke.App.Views.Procedures"
    xmlns:medicalShifts="clr-namespace:SledzSpecke.App.Views.MedicalShifts"
    xmlns:internships="clr-namespace:SledzSpecke.App.Views.Internships"
    Shell.FlyoutBehavior="Flyout"
    FlyoutHeaderBehavior="Fixed"
    FlyoutWidth="280"
    Title="ŚledzSpecke">
    <Shell.FlyoutHeader>
        <Grid HeightRequest="200" BackgroundColor="{StaticResource Primary}">
            <VerticalStackLayout Padding="20" VerticalOptions="Center">
                <Image Source="app_logo.png" HeightRequest="80" WidthRequest="80" HorizontalOptions="Center" />
                <Label x:Name="UserNameLabel" Text="Imię i Nazwisko" FontSize="18" TextColor="White" HorizontalOptions="Center" Margin="0,10,0,0" />
                <Label x:Name="SpecializationLabel" Text="Specjalizacja" FontSize="14" TextColor="White" HorizontalOptions="Center" />
            </VerticalStackLayout>
        </Grid>
    </Shell.FlyoutHeader>
    <FlyoutItem Title="Dashboard" Icon="dashboard.png" Route="dashboard">
        <ShellContent ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="dashboard" />
    </FlyoutItem>
    <FlyoutItem Title="Procedury" Icon="procedure.png" Route="ProcedureSelector">
        <ShellContent ContentTemplate="{DataTemplate procedures:ProcedureSelectorPage}" Route="procedures" />
    </FlyoutItem>
    <FlyoutItem Title="Dyżury" Icon="duty.png" Route="medicalshifts">
        <ShellContent ContentTemplate="{DataTemplate medicalShifts:MedicalShiftsSelectorPage}" Route="medicalshifts" />
    </FlyoutItem>
    <FlyoutItem Title="Staże" Icon="internship.png" Route="internships">
        <ShellContent ContentTemplate="{DataTemplate internships:InternshipsSelectorPage}" Route="internships" />
    </FlyoutItem>
    <FlyoutItem Title="Kursy" Icon="course.png" Route="courses">
        <ShellContent ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="internships" />
        <!-- to be implemented -->
    </FlyoutItem>
    <FlyoutItem Title="Samokształcenie" Icon="education.png" Route="selfeducation">
        <ShellContent ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="selfeducation" />
        <!-- to be implemented -->
    </FlyoutItem>
    <FlyoutItem Title="Publikacje" Icon="publication.png" Route="publications">
        <ShellContent ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="publications" />
        <!-- to be implemented -->
    </FlyoutItem>
    <FlyoutItem Title="Nieobecności" Icon="absence.png" Route="absences">
        <ShellContent ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="absences" />
        <!-- to be implemented -->
    </FlyoutItem>
    <FlyoutItem Title="Eksport" Icon="export.png" Route="export">
        <ShellContent ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="export" />
        <!-- to be implemented -->
    </FlyoutItem>
    </FlyoutItem>
    <FlyoutItem Title="Ustawienia" Icon="settings.png" Route="settings">
        <ShellContent ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="settings" />
        <!-- to be implemented -->
    </FlyoutItem>
    <Shell.FlyoutFooter>
        <Grid Padding="20" BackgroundColor="{StaticResource Primary}">
            <Button Text="Wyloguj" 
                    TextColor="White" 
                    BackgroundColor="Transparent" 
                    BorderColor="White" 
                    BorderWidth="1" 
                    CornerRadius="5"
                    Clicked="OnLogoutClicked" />
        </Grid>
    </Shell.FlyoutFooter>
</Shell>
```

## File: AppShell.xaml.cs
```csharp
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Views.Internships;
using SledzSpecke.App.Views.MedicalShifts;
using SledzSpecke.App.Views.Procedures;

namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService authService;

        public AppShell(IAuthService authService)
        {
            this.authService = authService;
            this.InitializeComponent();
            this.RegisterRoutes();
            this.InitializeUserInfoAsync();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("MedicalShiftsSelector", typeof(MedicalShiftsSelectorPage));
            Routing.RegisterRoute("OldSMKMedicalShifts", typeof(OldSMKMedicalShiftsPage));
            Routing.RegisterRoute("NewSMKMedicalShifts", typeof(NewSMKMedicalShiftsPage));
            Routing.RegisterRoute("AddEditOldSMKMedicalShift", typeof(AddEditOldSMKMedicalShiftPage));
            Routing.RegisterRoute("medicalshifts/AddEditOldSMKMedicalShift", typeof(AddEditOldSMKMedicalShiftPage));
            Routing.RegisterRoute("ProcedureSelector", typeof(ProcedureSelectorPage));
            Routing.RegisterRoute("OldSMKProcedures", typeof(OldSMKProceduresListPage));
            Routing.RegisterRoute("NewSMKProcedures", typeof(NewSMKProceduresListPage));
            Routing.RegisterRoute("AddEditOldSMKProcedure", typeof(AddEditOldSMKProcedurePage));
            Routing.RegisterRoute("AddEditNewSMKProcedure", typeof(AddEditNewSMKProcedurePage));
            Routing.RegisterRoute("internships", typeof(InternshipsSelectorPage));
            Routing.RegisterRoute("/OldSMKInternships", typeof(OldSMKInternshipsListPage));
            Routing.RegisterRoute("/NewSMKInternships", typeof(NewSMKInternshipsListPage));
            Routing.RegisterRoute("//AddEditRealizedInternship", typeof(AddEditRealizedInternshipPage));
        }

        private async void InitializeUserInfoAsync()
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user != null)
            {
                if (this.UserNameLabel != null)
                {
                    this.UserNameLabel.Text = user.Username;
                }

                var specializationService = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.Services.Specialization.ISpecializationService>();
                if (specializationService != null)
                {
                    var specialization = await specializationService.GetCurrentSpecializationAsync();
                    if (specialization != null && this.SpecializationLabel != null)
                    {
                        this.SpecializationLabel.Text = specialization.Name;
                    }
                }
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                bool confirm = await this.DisplayAlert(
                    "Wylogowanie",
                    "Czy na pewno chcesz się wylogować?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    await this.authService.LogoutAsync();

                    var loginViewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Authentication.LoginViewModel>();
                    var loginPage = new SledzSpecke.App.Views.Authentication.LoginPage(loginViewModel);

                    Application.Current.MainPage = new NavigationPage(loginPage);
                }
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Wystąpił błąd podczas wylogowywania.", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await this.LogoutAsync();
        }
    }
}
```

## File: MauiProgram.cs
```csharp
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Logging;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Services.Storage;
using SledzSpecke.App.ViewModels.Authentication;
using SledzSpecke.App.ViewModels.Dashboard;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.MedicalShifts;
using SledzSpecke.App.ViewModels.Procedures;
using SledzSpecke.App.Views.Authentication;
using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Internships;
using SledzSpecke.App.Views.MedicalShifts;
using SledzSpecke.App.Views.Procedures;

namespace SledzSpecke.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            RegisterServices(builder.Services);
            RegisterViewModels(builder.Services);
            RegisterViews(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggingService, LoggingService>();
            services.AddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
            services.AddSingleton<App>();
            services.AddSingleton<NavigationPage>();
            services.AddSingleton<AppShell>();
            services.AddSingleton<SplashPage>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<ISecureStorageService, SecureStorageService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<ISpecializationService, SpecializationService>();
            services.AddSingleton<ISecureStorageService, SecureStorageService>();
            services.AddSingleton<IMedicalShiftsService, MedicalShiftsService>();
            services.AddSingleton<IProcedureService, ProcedureService>();
            services.AddTransient<ModuleInitializer>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<MedicalShiftsSelectorViewModel>();
            services.AddTransient<OldSMKMedicalShiftsListViewModel>();
            services.AddTransient<NewSMKMedicalShiftsListViewModel>();
            services.AddTransient<AddEditOldSMKMedicalShiftViewModel>();
            services.AddTransient<AddEditOldSMKProcedureViewModel>();
            services.AddTransient<OldSMKProceduresListViewModel>();
            services.AddTransient<NewSMKProceduresListViewModel>();
            services.AddTransient<ProcedureSelectorViewModel>();
            services.AddTransient<ProcedureGroupViewModel>();
            services.AddTransient<ProcedureRequirementViewModel>();
            services.AddTransient<AddEditNewSMKProcedureViewModel>();
            services.AddTransient<InternshipsSelectorViewModel>();
            services.AddTransient<NewSMKInternshipsListViewModel>();
            services.AddTransient<OldSMKInternshipsListViewModel>();
            services.AddTransient<AddEditRealizedInternshipViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<DashboardPage>();
            services.AddTransient<MedicalShiftsSelectorPage>();
            services.AddTransient(sp => new OldSMKMedicalShiftsPage(
                sp.GetRequiredService<OldSMKMedicalShiftsListViewModel>(),
                sp.GetRequiredService<IMedicalShiftsService>()));
            services.AddTransient<NewSMKMedicalShiftsPage>();
            services.AddTransient<AddEditOldSMKMedicalShiftPage>();
            services.AddTransient(sp => new OldSMKProceduresListPage(
                sp.GetRequiredService<OldSMKProceduresListViewModel>(),
                sp.GetRequiredService<IProcedureService>()));
            services.AddTransient<AddEditOldSMKProcedurePage>();
            services.AddTransient<NewSMKProceduresListPage>();
            services.AddTransient<ProcedureSelectorPage>();
            services.AddTransient<AddEditNewSMKProcedurePage>();
            services.AddTransient<InternshipsSelectorPage>();
            services.AddTransient<NewSMKInternshipsListPage>();
            services.AddTransient<OldSMKInternshipsListPage>();
            services.AddTransient<AddEditRealizedInternshipPage>();
        }
    }
}
```

## File: SledzSpecke.csproj
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net9.0-ios;net9.0-maccatalyst;net9.0-android35.0</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net9.0-windows10.0.19041.0</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>SledzSpecke.App</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <ApplicationTitle>SledzSpecke.App</ApplicationTitle>
    <ApplicationId>com.companyname.sledzspecke.app</ApplicationId>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>
    <WindowsPackageType>None</WindowsPackageType>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">15.0</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
    <TargetPlatformMinVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</TargetPlatformMinVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'tizen'">6.5</SupportedOSPlatformVersion>
    <MtouchLink>SdkOnly</MtouchLink>
    <MauiEnableXamlCBindingWithSourceCompilation>true</MauiEnableXamlCBindingWithSourceCompilation>
  </PropertyGroup>
  <PropertyGroup>
    <RunAnalyzersDuringBuild>true</RunAnalyzersDuringBuild>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>
  <PropertyGroup Condition="'$(TargetFramework)' == 'net9.0-android35.0'">
    <RuntimeIdentifiers>android-arm64;android-arm;android-x64;android-x86</RuntimeIdentifiers>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Debug|net9.0-ios|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Debug|net9.0-maccatalyst|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Debug|net9.0-android35.0|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Debug|net9.0-windows10.0.19041.0|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net9.0-ios|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net9.0-maccatalyst|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net9.0-android35.0|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net9.0-windows10.0.19041.0|AnyCPU'">
    <NoWarn>1701;1702;CS8601;CS8604;CS8767;CS8618;CS8602;CS8600;CS8625;XC0022</NoWarn>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <MauiIcon Include="Resources\AppIcon\appicon.png" BaseSize="559,559" />
    <MauiSplashScreen Include="Resources\AppIcon\appicon.png" Color="#0d759c" BaseSize="559,559" />
    <MauiImage Include="Resources\Images\*" />
    <MauiImage Update="Resources\Images\appicon.png" Resize="True" BaseSize="559,559" />
    <MauiFont Include="Resources\Fonts\*" />
    <MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
  </ItemGroup>
  <ItemGroup>
    <Compile Remove="Resources\Icons\**" />
    <Compile Remove="Resources\Splash\**" />
    <EmbeddedResource Remove="Resources\Icons\**" />
    <EmbeddedResource Remove="Resources\Splash\**" />
    <MauiCss Remove="Resources\Icons\**" />
    <MauiCss Remove="Resources\Splash\**" />
    <MauiXaml Remove="Resources\Icons\**" />
    <MauiXaml Remove="Resources\Splash\**" />
    <None Remove="Resources\Icons\**" />
    <None Remove="Resources\Splash\**" />
  </ItemGroup>
  <ItemGroup>
    <MauiAsset Remove="Resources\Raw\SpecializationTemplates\cardiology_old.json" />
    <MauiAsset Remove="Resources\Raw\SpecializationTemplates\psychiatry_old.json" />
    <MauiAsset Remove="Resources\Raw\SpecializationTemplates\recommended_json_structure.json" />
  </ItemGroup>
  <ItemGroup>
    <MauiImage Remove="Resources\Images\appicon.png" />
  </ItemGroup>
  <ItemGroup>
    <None Remove="SledzSpecke.net9.0-android35.0.v3.ncrunchproject" />
    <None Remove="SledzSpecke.net9.0-ios.v3.ncrunchproject" />
    <None Remove="SledzSpecke.net9.0-maccatalyst.v3.ncrunchproject" />
  </ItemGroup>
  <ItemGroup>
    <EmbeddedResource Include="Resources\Raw\SpecializationTemplates\cardiology_old.json" />
    <EmbeddedResource Include="Resources\Raw\SpecializationTemplates\psychiatry_old.json" />
    <EmbeddedResource Include="Resources\Raw\SpecializationTemplates\cardiology_new.json" />
    <EmbeddedResource Include="Resources\Raw\SpecializationTemplates\psychiatry_new.json" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.0" />
    <PackageReference Include="EPPlus" Version="8.0.3" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.4" />
    <PackageReference Include="Microsoft.Maui.Controls" Version="9.0.60" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="9.0.4" />
    <PackageReference Include="Microsoft.Maui.Essentials" Version="9.0.60" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Plugin.LocalNotification" Version="12.0.1" />
    <PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
    <PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.11" />
    <PackageReference Include="System.Drawing.Common" Version="9.0.4" />
    <PackageReference Include="System.Formats.Nrbf" Version="9.0.4" />
  </ItemGroup>
  <ItemGroup>
    <MauiXaml Update="Resources\Styles\MaterialDesignStyles.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Internships\AddEditRealizedInternshipPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Internships\InternshipsSelectorPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Internships\NewSMKInternshipsListPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Internships\OldSMKInternshipsListPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\MedicalShifts\AddEditOldSMKMedicalShiftPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\MedicalShifts\MedicalShiftsSelectorPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\MedicalShifts\NewSMKMedicalShiftsPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\MedicalShifts\OldSMKMedicalShiftsPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Procedures\AddEditOldSMKProcedurePage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Procedures\NewSMKProceduresListPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Procedures\OldSMKProceduresListPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Procedures\ProcedureSelectorPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Specializations\InitializeSpecializationPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Authentication\LoginPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Authentication\RegisterPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
    <MauiXaml Update="Views\Dashboard\DashboardPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </MauiXaml>
  </ItemGroup>
  <ItemGroup>
    <None Update="Views\Internships\InternshipsListPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </None>
    <None Update="Views\MedicalShifts\MedicalShiftsListPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </None>
    <None Update="Views\Procedures\ProceduresListPage.xaml">
      <Generator>MSBuild:Compile</Generator>
    </None>
  </ItemGroup>
</Project>
```

## File: SledzSpecke.net9.0-android35.0.v3.ncrunchproject
```
<ProjectConfiguration>
  <Settings>
    <IgnoreThisComponentCompletely>True</IgnoreThisComponentCompletely>
  </Settings>
</ProjectConfiguration>
```

## File: SledzSpecke.net9.0-ios.v3.ncrunchproject
```
<ProjectConfiguration>
  <Settings>
    <IgnoreThisComponentCompletely>True</IgnoreThisComponentCompletely>
  </Settings>
</ProjectConfiguration>
```

## File: SledzSpecke.net9.0-maccatalyst.v3.ncrunchproject
```
<ProjectConfiguration>
  <Settings>
    <IgnoreThisComponentCompletely>True</IgnoreThisComponentCompletely>
  </Settings>
</ProjectConfiguration>
```

## File: SplashPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="SledzSpecke.App.SplashPage"
             Style="{StaticResource SplashScreenStyle}">
    <Grid RowDefinitions="*,Auto,*">
        <Image Grid.Row="1"
               Source="app_logo.png"
               HeightRequest="200"
               WidthRequest="200"
               HorizontalOptions="Center"/>
        <ActivityIndicator Grid.Row="2"
                           IsRunning="True"
                           HorizontalOptions="Center"
                           VerticalOptions="Start"
                           Margin="0,20,0,0"/>
    </Grid>
</ContentPage>
```

## File: SplashPage.xaml.cs
```csharp
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.ViewModels.Authentication;
using SledzSpecke.App.Views.Authentication;

namespace SledzSpecke.App
{
    public partial class SplashPage : ContentPage
    {
        private readonly IAuthService authService;

        public SplashPage(IAuthService authService)
        {
            this.authService = authService;
            this.InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            bool isAuthenticated = await this.authService.IsAuthenticatedAsync();

            Page mainPage;
            if (isAuthenticated)
            {
                mainPage = new AppShell(this.authService);
            }
            else
            {
                var viewModel = IPlatformApplication.Current.Services.GetService<LoginViewModel>();
                var loginPage = new LoginPage(viewModel);
                mainPage = new NavigationPage(loginPage);
            }

            var windows = Application.Current?.Windows;
            if (windows != null && windows.Count > 0)
            {
                var window = windows[0];
                window.Page = mainPage;
            }
        }
    }
}
```
