using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserProfileController : BaseController
{
    private readonly IQueryHandler<GetUserProfile, UserProfileDto> _getUserProfileHandler;
    private readonly ICommandHandler<UpdateUserProfile> _updateUserProfileHandler;
    private readonly ICommandHandler<ChangePassword> _changePasswordHandler;
    private readonly ICommandHandler<UpdateUserPreferences> _updateUserPreferencesHandler;

    public UserProfileController(
        IQueryHandler<GetUserProfile, UserProfileDto> getUserProfileHandler,
        ICommandHandler<UpdateUserProfile> updateUserProfileHandler,
        ICommandHandler<ChangePassword> changePasswordHandler,
        ICommandHandler<UpdateUserPreferences> updateUserPreferencesHandler)
    {
        _getUserProfileHandler = getUserProfileHandler;
        _updateUserProfileHandler = updateUserProfileHandler;
        _changePasswordHandler = changePasswordHandler;
        _updateUserPreferencesHandler = updateUserPreferencesHandler;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var query = new GetUserProfile();
        var profile = await _getUserProfileHandler.HandleAsync(query);
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var command = new UpdateUserProfile(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.CorrespondenceAddress);

        await _updateUserProfileHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePassword(
            request.CurrentPassword,
            request.NewPassword);

        await _changePasswordHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdateUserPreferencesRequest request)
    {
        var command = new UpdateUserPreferences(
            request.NotificationsEnabled,
            request.EmailNotificationsEnabled);

        await _updateUserPreferencesHandler.HandleAsync(command);
        return NoContent();
    }
}

public record UpdateUserProfileRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    SledzSpecke.Application.Commands.AddressDto CorrespondenceAddress);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);

public record UpdateUserPreferencesRequest(
    bool NotificationsEnabled,
    bool EmailNotificationsEnabled);