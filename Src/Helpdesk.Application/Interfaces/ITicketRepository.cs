using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Enums;

namespace Helpdesk.Application.Interfaces;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<Ticket?> GetTicketWithDetailsAsync(int id);
    Task<IEnumerable<Ticket>> GetRecentTicketsAsync(int count);
    Task<(IEnumerable<Ticket> Items, int TotalCount)> SearchTicketsAsync(
        TicketStatus? status, 
        TicketPriority? priority, 
        string? sortBy, 
        string sortDirection, 
        int pageNumber, 
        int pageSize);
}
