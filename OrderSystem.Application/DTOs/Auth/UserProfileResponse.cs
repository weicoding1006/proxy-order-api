namespace OrderSystem.Application.DTOs.Auth;

public record UserProfileResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles);
