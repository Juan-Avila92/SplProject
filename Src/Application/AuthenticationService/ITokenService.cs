using Domain.Entities;

namespace Application.Authentication;

public interface ITokenService
{
    Task<string> CreateAccessTokenAsync(ApplicationUser user);
}
