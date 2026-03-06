using Helpdesk.Application.Interfaces;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Infrastructure.Persistence.Repositories;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context, ILogger<TicketRepository> logger) : base(context, logger)
    {
    }

    public async Task<Ticket?> GetTicketWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(t => t.CreatedByUser)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .Include(t => t.Assignments)
                .ThenInclude(a => a.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Ticket>> GetRecentTicketsAsync(int count)
    {
        return await _dbSet
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Ticket> Items, int TotalCount)> SearchTicketsAsync(
        TicketStatus? status, 
        TicketPriority? priority, 
        string? sortBy, 
        string sortDirection, 
        int pageNumber, 
        int pageSize)
    {
        var query = _dbSet
            .Include(t => t.CreatedByUser)
            .Include(t => t.Assignments)
                .ThenInclude(a => a.AssignedToUser)
            .AsQueryable();

        // Filtering
        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        // Sorting
        bool isDescending = sortDirection.ToLower() == "desc";
        query = sortBy?.ToLower() switch
        {
            "priority" => isDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            "status" => isDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            "title" => isDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            _ => isDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
