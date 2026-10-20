using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Auth;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Api.Controllers;

// AP-04: the token is created here, after the Service succeeded (GN-04-R1)
[Route("api/auth")]
public class AuthController(IAuthService auth, JwtTokenFactory tokens) : ApiControllerBase
{
    // FN-GU-01: 201, logged in right away
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await auth.RegisterAsync(request, ct);
        return result.IsSuccess
            ? Created($"/api/users/{result.Value!.Id}", ToAuthResponse(result.Value))
            : OkOrProblem(result);
    }

    // FN-GU-02
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await auth.LoginAsync(request, ct);
        return result.IsSuccess ? Ok(ToAuthResponse(result.Value!)) : OkOrProblem(result);
    }

    // DT-03, EN-01-R5
    private AuthResponse ToAuthResponse(UserResponse user) => new()
    {
        Token = tokens.Create(user),
        DisplayName = user.DisplayName,
        Role = user.Role,
    };
}
