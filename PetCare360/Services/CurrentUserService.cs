using PetCare360.Exceptions;
using PetCare360.Models;
using PetCare360.Repositories.Interfaces;

namespace PetCare360.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
{
    public string Email()
    {
        var email = httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            throw new UnauthorizedException("Usuário não autenticado.");
        return email;
    }

    public async Task<AppUser> UserAsync()
    {
        var email = Email();
        return await userRepository.GetByEmailAsync(email)
            ?? throw new UnauthorizedException("Usuário não autenticado.");
    }
}