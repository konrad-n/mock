using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Security;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class ChangePasswordHandler : IResultCommandHandler<ChangePassword>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IPasswordManager _passwordManager;

    public ChangePasswordHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IPasswordManager passwordManager)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _passwordManager = passwordManager;
    }

    public async Task<Result> HandleAsync(ChangePassword command)
    {
        try
        {
            var currentUserId = _userContextService.GetUserId();
            var user = await _userRepository.GetByIdAsync(new UserId((int)currentUserId));
            
            if (user is null)
            {
                return Result.Failure($"User with ID {currentUserId} not found.");
            }

            // Verify current password
            if (!_passwordManager.Verify(command.CurrentPassword, user.Password.Value))
            {
                return Result.Failure("Current password is incorrect.");
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(command.NewPassword) || command.NewPassword.Length < 8)
            {
                return Result.Failure("Password must be at least 8 characters long.");
            }

            // Hash and update password
            var hashedPassword = _passwordManager.Secure(command.NewPassword);
            user.ChangePassword(new Password(hashedPassword));

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to change password: {ex.Message}");
        }
    }
}