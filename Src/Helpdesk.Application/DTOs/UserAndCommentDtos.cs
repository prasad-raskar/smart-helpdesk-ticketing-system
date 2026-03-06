using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Data required to create a new user.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Unique username for the user.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Valid email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Strong password.
    /// </summary>
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User role: User, Agent, or Admin.
    /// </summary>
    [Required]
    public string Role { get; set; } = "User";
}

public class CommentDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateCommentRequest
{
    public int TicketId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class AssignTicketRequest
{
    public int TicketId { get; set; }
    public int AssignedToUserId { get; set; }
    public int AssignedByUserId { get; set; }
}

/// <summary>
/// Login credentials.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Registered email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty; 
}

public class AuthResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
