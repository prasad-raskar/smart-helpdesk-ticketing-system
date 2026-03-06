using Helpdesk.Application.DTOs;

namespace Helpdesk.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
}

public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(CreateCommentRequest request);
    Task<IEnumerable<CommentDto>> GetCommentsByTicketIdAsync(int ticketId);
}
