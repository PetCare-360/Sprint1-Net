using Microsoft.AspNetCore.Mvc;
using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;
using PetCare360.Services.Interfaces;

namespace PetCare360.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Registrar usuário</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        return Created(string.Empty, result);
    }

    /// <summary>Login</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var result = await authService.LoginAsync(request);
        return Ok(result);
    }
}