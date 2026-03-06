using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace Helpdesk.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IUserService userService, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("[INFO] Authentication attempt for email: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null || !BC.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("[WARN] Authentication failed for email: {Email}", request.Email);
            throw new Exception("Invalid email or password.");
        }

        var userDto = new UserDto 
        { 
            Id = user.Id, 
            Username = user.Username, 
            Email = user.Email, 
            Role = user.Role 
        };

        var token = GenerateJwtToken(userDto);

        _logger.LogInformation("[INFO] User authenticated successfully. ID: {UserId}, Role: {Role}", user.Id, user.Role);

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = token
        };
    }

    public async Task<AuthResponse> RegisterAsync(CreateUserRequest request)
    {
        _logger.LogInformation("[INFO] Registering new user: {Username}, Email: {Email}", request.Username, request.Email);

        var userDto = await _userService.CreateUserAsync(request);
        var token = GenerateJwtToken(userDto);
        
        _logger.LogInformation("[INFO] User registered successfully. ID: {UserId}", userDto.Id);

        return new AuthResponse
        {
            UserId = userDto.Id,
            Username = userDto.Username,
            Role = userDto.Role,
            Token = token
        };
    }

    private string GenerateJwtToken(UserDto user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
