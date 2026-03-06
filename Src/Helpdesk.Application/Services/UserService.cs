using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Helpdesk.Domain.Entities;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;

namespace Helpdesk.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        _logger.LogInformation("[INFO] Retrieving user details for ID: {UserId}", id);
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) 
        {
            _logger.LogWarning("[WARN] User with ID: {UserId} not found", id);
            return null;
        }

        return MapToDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("[INFO] Retrieving user details for Email: {Email}", email);
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) 
        {
            _logger.LogWarning("[WARN] User with Email: {Email} not found", email);
            return null;
        }

        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("[INFO] Retrieving all users");
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogInformation("[INFO] Attempting to create new user: {Username}", request.Username);
        
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogError("[ERROR] User creation failed. Email {Email} already exists.", request.Email);
            throw new Exception($"User with email {request.Email} already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Role = request.Role,
            PasswordHash = BC.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("[INFO] User {Username} created successfully with ID: {UserId}", user.Username, user.Id);
        return MapToDto(user);
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        Role = user.Role
    };
}
