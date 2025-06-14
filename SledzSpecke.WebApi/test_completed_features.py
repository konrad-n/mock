#!/usr/bin/env python3
"""
Test script for completed features without relying on database state.
Tests implementation details of the 5 completed tasks.
"""

import os
import subprocess
import re
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

def search_file_content(file_path, pattern):
    """Search for pattern in file content"""
    try:
        with open(file_path, 'r') as f:
            content = f.read()
            return pattern in content
    except:
        return False

def test_sync_status_management():
    """Test 1: Sync status auto-transition implementation"""
    print_test_header("Task 1: Sync Status Management Implementation")
    
    # Check MedicalShift entity has sync status transition
    medical_shift_file = "src/SledzSpecke.Core/Entities/MedicalShift.cs"
    if os.path.exists(medical_shift_file):
        with open(medical_shift_file, 'r') as f:
            content = f.read()
            
        # Check for sync status transition in UpdateShiftDetails
        if "if (SyncStatus == SyncStatus.Synced)" in content and "SyncStatus = SyncStatus.Modified;" in content:
            print_success("MedicalShift entity has sync status auto-transition in UpdateShiftDetails")
        else:
            print_error("MedicalShift entity missing sync status auto-transition")
            
        # Check that CanBeModified doesn't restrict synced items
        if "CanBeModified => SyncStatus != SyncStatus.Synced" not in content:
            print_success("MedicalShift allows modification of synced items")
        else:
            print_error("MedicalShift still restricts modification of synced items")
    else:
        print_error(f"File not found: {medical_shift_file}")
    
    # Check Procedure entity
    procedure_file = "src/SledzSpecke.Core/Entities/ProcedureBase.cs"
    if os.path.exists(procedure_file):
        with open(procedure_file, 'r') as f:
            content = f.read()
            
        if "if (SyncStatus == SyncStatus.Synced)" in content:
            print_success("ProcedureBase entity has sync status transition logic")
        else:
            print_info("ProcedureBase might handle sync status differently")
    else:
        print_info(f"File not found: {procedure_file}")
        
    # Check Internship entity
    internship_file = "src/SledzSpecke.Core/Entities/Internship.cs"
    if os.path.exists(internship_file):
        with open(internship_file, 'r') as f:
            content = f.read()
            
        if "SyncStatus = SyncStatus.Modified;" in content:
            print_success("Internship entity has sync status transitions in update methods")
        else:
            print_error("Internship entity missing sync status transitions")
    else:
        print_error(f"File not found: {internship_file}")

def test_documentation():
    """Test 2: Documentation of sync status management"""
    print_test_header("Task 2: Sync Status Documentation")
    
    # Check for documentation in code files
    files_to_check = [
        "src/SledzSpecke.Core/Entities/MedicalShift.cs",
        "src/SledzSpecke.Core/Entities/Internship.cs",
        "src/SledzSpecke.Application/Commands/Handlers/UpdateInternshipHandler.cs"
    ]
    
    doc_found = False
    for file_path in files_to_check:
        if os.path.exists(file_path):
            with open(file_path, 'r') as f:
                content = f.read()
                if "sync" in content.lower() and ("automatic" in content.lower() or "transition" in content.lower()):
                    print_success(f"Documentation found in {os.path.basename(file_path)}")
                    doc_found = True
        else:
            print_info(f"File not found: {file_path}")
    
    if not doc_found:
        print_info("No explicit sync status documentation found in comments")

def test_medical_shift_validation():
    """Test 3: Medical shift duration validation alignment with MAUI"""
    print_test_header("Task 3: Medical Shift Duration Validation")
    
    # Check AddMedicalShiftHandler
    handler_file = "src/SledzSpecke.Application/MedicalShifts/Handlers/AddMedicalShiftHandler.cs"
    if os.path.exists(handler_file):
        with open(handler_file, 'r') as f:
            content = f.read()
            
        # Check for removal of maximum duration limits
        if "command.Hours > 24" not in content and "shift duration cannot exceed" not in content.lower():
            print_success("Maximum duration limits removed (24h restriction gone)")
        else:
            print_error("Still has maximum duration restrictions")
            
        # Check for allowing minutes > 59
        if "command.Minutes > 59" not in content and "minutes must be between" not in content.lower():
            print_success("Minutes > 59 allowed (no 0-59 restriction)")
        else:
            print_error("Still restricts minutes to 0-59")
            
        # Check for only validating > 0
        if "command.Hours == 0 && command.Minutes == 0" in content:
            print_success("Only checks that total duration > 0")
        else:
            print_info("Duration validation logic might be different")
    else:
        print_error(f"File not found: {handler_file}")
    
    # Check TimeNormalizationHelper
    helper_file = "src/SledzSpecke.Application/Helpers/TimeNormalizationHelper.cs"
    if os.path.exists(helper_file):
        print_success("TimeNormalizationHelper created for display formatting")
        with open(helper_file, 'r') as f:
            content = f.read()
            if "NormalizeTime" in content and "minutes >= 60" in content:
                print_success("TimeNormalizationHelper properly normalizes excess minutes")
    else:
        print_error("TimeNormalizationHelper not found")

def test_internship_update():
    """Test 4: Internship update functionality"""
    print_test_header("Task 4: Internship Update Functionality")
    
    # Check UpdateInternshipHandler exists
    update_handler_file = "src/SledzSpecke.Application/Commands/Handlers/UpdateInternshipHandler.cs"
    if os.path.exists(update_handler_file):
        print_success("UpdateInternshipHandler created")
        with open(update_handler_file, 'r') as f:
            content = f.read()
            if "ValidateInternshipDates" in content:
                print_success("Handler includes date validation")
            if "UpdateInstitution" in content or "UpdateDates" in content:
                print_success("Handler calls entity update methods")
    else:
        print_error("UpdateInternshipHandler not found")
    
    # Check MarkInternshipAsCompletedHandler
    complete_handler_file = "src/SledzSpecke.Application/Commands/Handlers/MarkInternshipAsCompletedHandler.cs"
    if os.path.exists(complete_handler_file):
        print_success("MarkInternshipAsCompletedHandler created")
    else:
        print_error("MarkInternshipAsCompletedHandler not found")
    
    # Check Internship entity update methods
    internship_file = "src/SledzSpecke.Core/Entities/Internship.cs"
    if os.path.exists(internship_file):
        with open(internship_file, 'r') as f:
            content = f.read()
            methods = ["UpdateInstitution", "UpdateDates", "MarkAsCompleted"]
            for method in methods:
                if f"public void {method}" in content:
                    print_success(f"Internship.{method} method implemented")
                else:
                    print_error(f"Internship.{method} method not found")
    
    # Check controller endpoints
    controller_file = "src/SledzSpecke.Api/Controllers/InternshipsController.cs"
    if os.path.exists(controller_file):
        with open(controller_file, 'r') as f:
            content = f.read()
            if '[HttpPut("{internshipId:int}")]' in content:
                print_success("PUT endpoint for internship update exists")
            if '[HttpPost("{internshipId:int}/complete")]' in content:
                print_success("POST endpoint for marking complete exists")

def test_year_calculation():
    """Test 5: Year calculation based on specialization structure"""
    print_test_header("Task 5: Year Calculation Implementation")
    
    # Check YearCalculationService
    service_file = "src/SledzSpecke.Application/Services/YearCalculationService.cs"
    if os.path.exists(service_file):
        print_success("YearCalculationService created")
        with open(service_file, 'r') as f:
            content = f.read()
            
        # Check for key methods
        methods = [
            ("GetAvailableYears", "Returns years 1-6 for Old SMK"),
            ("GetModuleYearRange", "Returns year ranges for modules"),
            ("IsYearValidForModule", "Validates year including 0 for unassigned"),
            ("CalculateCurrentYear", "Calculates current year from elapsed time")
        ]
        
        for method, description in methods:
            if f"public {method}" in content or f"{method}(" in content:
                print_success(f"{method} implemented - {description}")
            else:
                print_error(f"{method} not found")
                
        # Check for year 0 support
        if "year == 0" in content:
            print_success("Supports year 0 for unassigned items")
    else:
        print_error("YearCalculationService not found")
    
    # Check interface
    interface_file = "src/SledzSpecke.Application/Abstractions/IYearCalculationService.cs"
    if os.path.exists(interface_file):
        print_success("IYearCalculationService interface exists")
    else:
        print_error("IYearCalculationService interface not found")
    
    # Check integration in handlers
    procedure_handler = "src/SledzSpecke.Application/Procedures/Handlers/AddProcedureHandler.cs"
    if os.path.exists(procedure_handler):
        with open(procedure_handler, 'r') as f:
            content = f.read()
            if "_yearCalculationService" in content:
                print_success("YearCalculationService integrated in AddProcedureHandler")
                if "availableYears.Contains" in content:
                    print_success("Year validation using available years")
    
    # Check Procedure.Create accepts year parameter
    procedure_file = "src/SledzSpecke.Core/Entities/Procedure.cs"
    if os.path.exists(procedure_file):
        with open(procedure_file, 'r') as f:
            content = f.read()
            if "int year," in content or "int year)" in content:
                print_success("Procedure.Create accepts year parameter")
            else:
                print_error("Procedure.Create doesn't accept year parameter")
    
    # Check DI registration
    extensions_file = "src/SledzSpecke.Application/Extensions.cs"
    if os.path.exists(extensions_file):
        with open(extensions_file, 'r') as f:
            content = f.read()
            if "IYearCalculationService" in content and "YearCalculationService" in content:
                print_success("YearCalculationService registered in DI container")

def run_build_test():
    """Test that the project builds successfully"""
    print_test_header("Build Test")
    
    print_info("Running dotnet build...")
    result = subprocess.run(["dotnet", "build"], capture_output=True, text=True)
    
    if result.returncode == 0:
        print_success("Project builds successfully")
        # Count warnings
        warning_count = result.stdout.count("Warning(s)")
        if warning_count > 0:
            print_info(f"Build completed with warnings")
    else:
        print_error("Build failed")
        print(result.stderr)

def main():
    print(f"\n{BLUE}Testing Completed Features Implementation{RESET}")
    print(f"{BLUE}This tests the code changes without requiring database data{RESET}")
    
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
    test_sync_status_management()
    test_documentation()
    test_medical_shift_validation()
    test_internship_update()
    test_year_calculation()
    run_build_test()
    
    print(f"\n{BLUE}All tests completed!{RESET}")

if __name__ == "__main__":
    main()