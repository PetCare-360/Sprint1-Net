using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;
using PetCare360.Exceptions;
using PetCare360.Models;
using PetCare360.Repositories.Interfaces;
using PetCare360.Services.Interfaces;

namespace PetCare360.Services;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration,
    PetMapper mapper) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();

        if (await userRepository.ExistsByEmailAsync(email))
            throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");

        var user = new AppUser
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await userRepository.SaveAsync(user);
        return new AuthResponse(GenerateToken(user.Email), "Bearer", mapper.ToUserResponse(user));
    }

    public async Task<AuthResponse> LoginAsync(AuthRequest request)
    {
        var email = request.Email.Trim().ToLower();
        var user = await userRepository.GetByEmailAsync(email)
            ?? throw new UnauthorizedException("Credenciais inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Credenciais inválidas.");

        return new AuthResponse(GenerateToken(user.Email), "Bearer", mapper.ToUserResponse(user));
    }

    private string GenerateToken(string email)
    {
        var secret = configuration["Jwt:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}