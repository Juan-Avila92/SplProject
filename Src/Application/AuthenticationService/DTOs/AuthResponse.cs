namespace Application.Authentication.DTOs;

public record AuthResponse(
    string UserId,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc);
