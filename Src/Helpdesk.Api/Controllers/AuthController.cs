using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Api.Controllers;

/// <summary>
/// Handles user onboarding and authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user into the system.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AuthResponse>> Register(CreateUserRequest request)
    {
        _logger.LogInformation("[INFO] User registration request received for Email: {Email}", request.Email);
        var response = await _authService.RegisterAsync(request);
        _logger.LogInformation("[INFO] User registered successfully: {Email}", request.Email);
        return CreatedAtAction(nameof(Register), response);
    }

    /// <summary>
    /// Authenticates a user and provides a JWT access token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        _logger.LogInformation("[INFO] Login attempt received for Email: {Email}", request.Email);
        var response = await _authService.LoginAsync(request);
        _logger.LogInformation("[INFO] Login successful for Email: {Email}", request.Email);
        return Ok(response);
    }
}
