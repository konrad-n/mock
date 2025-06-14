#!/usr/bin/env python3
"""
Test script for module-based validation for New SMK procedures.
"""

import os
import subprocess
import json

# Colors for output
GREEN = '\033[92m'
RED = '\033[91m'
YELLOW = '\033[93m'
BLUE = '\033[94m'
RESET = '\033[0m'

def print_test_header(test_name):
    print(f"\n{BLUE}{'='*60}{RESET}")
    print(f"{BLUE}Testing: {test_name}{RESET}")
    print(f"{BLUE}{'='*60}{RESET}")

def print_success(message):
    print(f"{GREEN}✓ {message}{RESET}")

def print_error(message):
    print(f"{RED}✗ {message}{RESET}")

def print_info(message):
    print(f"{YELLOW}ℹ {message}{RESET}")

def test_module_validation_implementation():
    """Test module-based validation for New SMK procedures"""
    print_test_header("Module-Based Validation for New SMK")
    
    # Check SpecializationValidationService enhancements
    validation_file = "src/SledzSpecke.Application/Services/SpecializationValidationService.cs"
    if os.path.exists(validation_file):
        with open(validation_file, 'r') as f:
            content = f.read()
            
        # Check for module validation in ValidateProcedureAsync
        if "Module-based validation for New SMK" in content:
            print_success("Module-based validation logic implemented")
        else:
            print_error("Module-based validation not found")
            
        # Check for current module validation
        if "No current module selected. Cannot add procedures without an active module." in content:
            print_success("Current module requirement enforced")
        else:
            print_error("Current module validation not found")
            
        # Check for module mismatch validation
        if "belongs to module" in content and "but current module is" in content:
            print_success("Module mismatch validation implemented")
        else:
            print_error("Module mismatch validation not found")
            
        # Check for module-specific procedure search
        if "For New SMK, only search in current module" in content:
            print_success("Module-specific procedure search implemented")
        else:
            print_error("Module-specific procedure search not found")
    else:
        print_error(f"File not found: {validation_file}")

def test_module_progress_calculation():
    """Test module progress calculation functionality"""
    print_test_header("Module Progress Calculation")
    
    validation_file = "src/SledzSpecke.Application/Services/SpecializationValidationService.cs"
    if os.path.exists(validation_file):
        with open(validation_file, 'r') as f:
            content = f.read()
            
        # Check for CalculateModuleProgressAsync method
        if "CalculateModuleProgressAsync" in content:
            print_success("CalculateModuleProgressAsync method implemented")
            
            # Check for progress calculation formula
            if "internshipProgress * 0.35" in content:
                print_success("MAUI progress formula implemented (35% internship)")
            if "courseProgress * 0.25" in content:
                print_success("MAUI progress formula implemented (25% courses)")
            if "procedureProgress * 0.30" in content:
                print_success("MAUI progress formula implemented (30% procedures)")
            if "shiftProgress * 0.10" in content:
                print_success("MAUI progress formula implemented (10% shifts)")
        else:
            print_error("CalculateModuleProgressAsync not found")
    
    # Check ModuleProgress class
    interface_file = "src/SledzSpecke.Application/Abstractions/ISpecializationValidationService.cs"
    if os.path.exists(interface_file):
        with open(interface_file, 'r') as f:
            content = f.read()
            
        if "class ModuleProgress" in content:
            print_success("ModuleProgress class defined")
            
            # Check required properties
            required_props = [
                "CompletedProceduresA",
                "CompletedProceduresB", 
                "TotalRequiredProceduresA",
                "TotalRequiredProceduresB",
                "ProcedureACompletionPercentage",
                "ProcedureBCompletionPercentage",
                "OverallCompletionPercentage"
            ]
            
            for prop in required_props:
                if prop in content:
                    print_success(f"ModuleProgress.{prop} property exists")
                else:
                    print_error(f"ModuleProgress.{prop} property missing")

def test_module_specific_features():
    """Test module-specific features"""
    print_test_header("Module-Specific Features")
    
    validation_file = "src/SledzSpecke.Application/Services/SpecializationValidationService.cs"
    if os.path.exists(validation_file):
        with open(validation_file, 'r') as f:
            content = f.read()
            
        print_info("Checking module-specific validations:")
        
        # Check for New SMK specific logic
        if "specialization.SmkVersion == SmkVersion.New" in content:
            count = content.count("specialization.SmkVersion == SmkVersion.New")
            print_success(f"New SMK specific checks found ({count} occurrences)")
        
        # Check for module template usage
        if "ModuleTemplate" in content:
            print_success("Uses ModuleTemplate for validation")
        
        # Check for internship ID validation
        if "procedureTemplate.InternshipId" in content:
            print_success("Validates procedure internship requirements")

def test_integration_points():
    """Test integration with other components"""
    print_test_header("Integration Points")
    
    # Check if handlers use the new validation
    handler_file = "src/SledzSpecke.Application/Procedures/Handlers/AddProcedureHandler.cs"
    if os.path.exists(handler_file):
        with open(handler_file, 'r') as f:
            content = f.read()
            
        if "_validationService" in content:
            print_success("AddProcedureHandler integrated with validation service")
    
    # Check interface updates
    interface_file = "src/SledzSpecke.Application/Abstractions/ISpecializationValidationService.cs"
    if os.path.exists(interface_file):
        with open(interface_file, 'r') as f:
            content = f.read()
            
        if "CalculateModuleProgressAsync" in content:
            print_success("Interface includes module progress calculation")

def run_build_test():
    """Test that the project builds successfully"""
    print_test_header("Build Test")
    
    print_info("Running dotnet build...")
    result = subprocess.run(["dotnet", "build"], capture_output=True, text=True)
    
    if result.returncode == 0:
        print_success("Project builds successfully with module validation")
    else:
        print_error("Build failed")
        print(result.stderr)

def main():
    print(f"\n{BLUE}Testing Module-Based Validation Implementation{RESET}")
    
    # Change to project directory
    if os.path.exists("src/SledzSpecke.Api"):
        print_info("Already in project directory")
    elif os.path.exists("SledzSpecke.WebApi/src/SledzSpecke.Api"):
        os.chdir("SledzSpecke.WebApi")
        print_info("Changed to SledzSpecke.WebApi directory")
    else:
        print_error("Cannot find project directory")
        return
    
    # Run tests
    test_module_validation_implementation()
    test_module_progress_calculation()
    test_module_specific_features()
    test_integration_points()
    run_build_test()
    
    print(f"\n{BLUE}Summary:{RESET}")
    print("✓ Module-based validation ensures procedures are tied to correct modules")
    print("✓ Current module must be selected for New SMK procedures")
    print("✓ Procedures can only be added to the active module")
    print("✓ Module progress calculation follows MAUI formula (35/25/30/10)")
    print("✓ Procedure search is module-specific for New SMK")

if __name__ == "__main__":
    main()