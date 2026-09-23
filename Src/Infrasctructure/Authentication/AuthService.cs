using Application.Authentication;
using Application.Authentication.DTOs;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _db = db;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
            throw new InvalidOperationException("Unable to create account.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(x => x.Description)));
        }

        await _userManager.AddToRoleAsync(user, "User");

        return await CreateAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return null;

        var valid = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!valid)
            return null;

        return await CreateAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var hash = RefreshTokenHelper.Hash(refreshToken);

        var storedToken = await _db.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == hash);

        if (storedToken == null || !storedToken.IsActive)
            return null;

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        return await CreateAuthResponseAsync(storedToken.User);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var hash = RefreshTokenHelper.Hash(refreshToken);

        var storedToken = await _db.RefreshTokens
            .SingleOrDefaultAsync(x => x.TokenHash == hash);

        if (storedToken != null && storedToken.IsActive)
        {
            storedToken.RevokedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(
        ApplicationUser user)
    {
        var accessToken =
            await _tokenService.CreateAccessTokenAsync(user);

        var rawRefreshToken =
            RefreshTokenHelper.Generate();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = RefreshTokenHelper.Hash(rawRefreshToken),
            UserId = user.Id,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(
                _jwtOptions.AccessTokenExpirationMinutes)
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return new AuthResponse(
            user.Id,
            accessToken,
            rawRefreshToken,
            DateTime.UtcNow.AddMinutes(
                _jwtOptions.AccessTokenExpirationMinutes));
    }
}
