using Helpdesk.Domain.Entities;

namespace Helpdesk.Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
