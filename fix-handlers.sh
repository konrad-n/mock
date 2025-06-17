#!/bin/bash

# Fix all handlers that are missing the CancellationToken parameter

echo "Fixing DeleteProcedureHandler..."
sed -i 's/public async Task<Result> HandleAsync(DeleteProcedure command)/public async Task<Result> HandleAsync(DeleteProcedure command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Procedures/Handlers/DeleteProcedureHandler.cs

echo "Fixing CreateCourseHandler..."
sed -i 's/public async Task<Result<int>> HandleAsync(CreateCourse command)/public async Task<Result<int>> HandleAsync(CreateCourse command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/CreateCourseHandler.cs

echo "Fixing CreateEducationalActivityHandler..."
sed -i 's/public async Task<Result<int>> HandleAsync(CreateEducationalActivity command)/public async Task<Result<int>> HandleAsync(CreateEducationalActivity command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/CreateEducationalActivityHandler.cs

echo "Fixing CreatePublicationHandler..."
sed -i 's/public async Task<Result> HandleAsync(CreatePublication command)/public async Task<Result> HandleAsync(CreatePublication command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/CreatePublicationHandler.cs

echo "Fixing DeleteCourseHandler..."
sed -i 's/public async Task<Result> HandleAsync(DeleteCourse command)/public async Task<Result> HandleAsync(DeleteCourse command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/DeleteCourseHandler.cs

echo "Fixing DeleteEducationalActivityHandler..."
sed -i 's/public async Task<Result> HandleAsync(DeleteEducationalActivity command)/public async Task<Result> HandleAsync(DeleteEducationalActivity command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/DeleteEducationalActivityHandler.cs

echo "Fixing DeleteFileHandler..."
sed -i 's/public async Task<Result> HandleAsync(DeleteFile command)/public async Task<Result> HandleAsync(DeleteFile command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/DeleteFileHandler.cs

echo "Fixing DeleteInternshipHandler..."
sed -i 's/public async Task<Result> HandleAsync(DeleteInternship command)/public async Task<Result> HandleAsync(DeleteInternship command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/DeleteInternshipHandler.cs

echo "Fixing DeletePublicationHandler..."
sed -i 's/public async Task<Result> HandleAsync(DeletePublication command)/public async Task<Result> HandleAsync(DeletePublication command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/DeletePublicationHandler.cs

echo "Fixing MarkInternshipAsCompletedHandler..."
sed -i 's/public async Task<Result> HandleAsync(MarkInternshipAsCompleted command)/public async Task<Result> HandleAsync(MarkInternshipAsCompleted command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/MarkInternshipAsCompletedHandler.cs

echo "Fixing SignInHandler..."
sed -i 's/public async Task<Result<JwtDto>> HandleAsync(SignIn command)/public async Task<Result<JwtDto>> HandleAsync(SignIn command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/SignInHandler.cs

echo "Fixing SignUpHandler..."
sed -i 's/public async Task<Result> HandleAsync(SignUp command)/public async Task<Result> HandleAsync(SignUp command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/SignUpHandler.cs

echo "Fixing ApproveAbsenceHandler..."
sed -i 's/public async Task<Result> HandleAsync(ApproveAbsence command)/public async Task<Result> HandleAsync(ApproveAbsence command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/ApproveAbsenceHandler.cs

echo "Fixing ApproveInternshipHandler..."
sed -i 's/public async Task<Result> HandleAsync(ApproveInternship command)/public async Task<Result> HandleAsync(ApproveInternship command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/ApproveInternshipHandler.cs

echo "Fixing UpdateCourseHandler..."
sed -i 's/public async Task<Result> HandleAsync(UpdateCourse command)/public async Task<Result> HandleAsync(UpdateCourse command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/UpdateCourseHandler.cs

echo "Fixing UpdateEducationalActivityHandler..."
sed -i 's/public async Task<Result> HandleAsync(UpdateEducationalActivity command)/public async Task<Result> HandleAsync(UpdateEducationalActivity command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/UpdateEducationalActivityHandler.cs

echo "Fixing UpdateInternshipHandler..."
sed -i 's/public async Task<Result> HandleAsync(UpdateInternship command)/public async Task<Result> HandleAsync(UpdateInternship command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/UpdateInternshipHandler.cs

echo "Fixing UploadFileHandler..."
sed -i 's/public async Task<Result<FileMetadataDto>> HandleAsync(UploadFile command)/public async Task<Result<FileMetadataDto>> HandleAsync(UploadFile command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/UploadFileHandler.cs

echo "Fixing UpdatePublicationHandler..."
sed -i 's/public async Task<Result> HandleAsync(UpdatePublication command)/public async Task<Result> HandleAsync(UpdatePublication command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/UpdatePublicationHandler.cs

echo "Fixing CreateAbsenceHandler..."
sed -i 's/public async Task<Result> HandleAsync(CreateAbsence command)/public async Task<Result> HandleAsync(CreateAbsence command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/CreateAbsenceHandler.cs

echo "Fixing ChangePasswordHandler..."
sed -i 's/public async Task<Result> HandleAsync(ChangePassword command)/public async Task<Result> HandleAsync(ChangePassword command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/ChangePasswordHandler.cs

echo "Fixing UpdateUserProfileHandler..."
sed -i 's/public async Task<Result> HandleAsync(UpdateUserProfile command)/public async Task<Result> HandleAsync(UpdateUserProfile command, CancellationToken cancellationToken = default)/' /home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application/Commands/Handlers/UpdateUserProfileHandler.cs

echo "Done fixing handlers!"