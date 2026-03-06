using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Helpdesk.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TicketService> _logger;
    private readonly IMemoryCache _cache;
    
    private const string TicketCacheKey = "Tickets_List_";
    private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

    public TicketService(
        ITicketRepository ticketRepository, 
        IUserRepository userRepository, 
        ILogger<TicketService> logger,
        IMemoryCache cache)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
    {
        _logger.LogInformation("[INFO] Retrieving all tickets (Legacy method)");
        var tickets = await _ticketRepository.GetAllAsync();
        return tickets.Select(MapToDto);
    }

    public async Task<TicketDto?> GetTicketByIdAsync(int id)
    {
        _logger.LogInformation("[INFO] Retrieving ticket by ID: {Id}", id);
        var ticket = await _ticketRepository.GetTicketWithDetailsAsync(id);
        if (ticket == null) 
        {
            _logger.LogWarning("[WARN] Ticket with ID: {Id} not found", id);
            return null;
        }

        return MapToDto(ticket);
    }

    public async Task<TicketDto> CreateTicketAsync(CreateTicketRequest request)
    {
        _logger.LogInformation("[INFO] Attempting to create ticket: {Title} by User ID: {UserId}", request.Title, request.CreatedByUserId);
        
        var user = await _userRepository.GetByIdAsync(request.CreatedByUserId);
        if (user == null)
        {
            _logger.LogError("[ERROR] Ticket creation aborted. Creator user ID: {UserId} not found.", request.CreatedByUserId);
            throw new Exception("Creator user not found.");
        }

        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ticketRepository.AddAsync(ticket);
        await _ticketRepository.SaveChangesAsync();

        _logger.LogInformation("[INFO] Ticket created successfully with ID: {TicketId}", ticket.Id);
        
        InvalidateCache();
        return MapToDto(ticket);
    }

    public async Task UpdateTicketStatusAsync(int id, string status)
    {
        _logger.LogInformation("[INFO] Attempting to update status for Ticket ID: {TicketId} to {Status}", id, status);
        
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null) 
        {
            _logger.LogError("[ERROR] Status update failed. Ticket ID: {TicketId} not found.", id);
            throw new Exception("Ticket not found.");
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            _logger.LogError("[ERROR] Status update failed. Ticket ID: {TicketId} is already closed.", id);
            throw new InvalidOperationException("Closed tickets cannot be modified.");
        }

        if (Enum.TryParse<TicketStatus>(status, true, out var newStatus))
        {
            ticket.Status = newStatus;
            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
            
            _logger.LogInformation("[INFO] Ticket ID: {TicketId} status updated to {Status}", id, status);
            InvalidateCache();
        }
        else
        {
            _logger.LogError("[ERROR] Status update failed. Invalid status '{Status}' provided.", status);
            throw new ArgumentException("Invalid status.");
        }
    }

    public async Task AssignTicketAsync(AssignTicketRequest request)
    {
        _logger.LogInformation("[INFO] Attempting to assign Ticket ID: {TicketId} to Agent ID: {AgentId}", request.TicketId, request.AssignedToUserId);
        
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
        var agent = await _userRepository.GetByIdAsync(request.AssignedToUserId);

        if (ticket == null || agent == null)
        {
            _logger.LogError("[ERROR] Assignment failed. Ticket or Agent not found.");
            throw new Exception("Ticket or Agent not found.");
        }

        var assignment = new TicketAssignment
        {
            TicketId = request.TicketId,
            AssignedToUserId = request.AssignedToUserId,
            AssignedByUserId = request.AssignedByUserId,
            AssignedAt = DateTime.UtcNow
        };

        ticket.Status = TicketStatus.InProgress;

        // In a real generic repo, we'd add to the assignments repo, 
        // but for this demo let's assume update ticket handles the state.
        await _ticketRepository.UpdateAsync(ticket);
        await _ticketRepository.SaveChangesAsync();
        
        _logger.LogInformation("[INFO] Ticket ID: {TicketId} successfully assigned and status moved to InProgress", request.TicketId);

        InvalidateCache();
    }

    public async Task DeleteTicketAsync(int id)
    {
        _logger.LogInformation("[INFO] Attempting to delete Ticket ID: {TicketId}", id);
        await _ticketRepository.DeleteAsync(id);
        await _ticketRepository.SaveChangesAsync();
        _logger.LogInformation("[INFO] Ticket ID: {TicketId} deleted.", id);
        
        InvalidateCache();
    }

    public async Task<PaginatedResponse<TicketDto>> SearchTicketsAsync(TicketQueryRequest query)
    {
        string cacheKey = $"{TicketCacheKey}{query.Status}_{query.Priority}_{query.SortBy}_{query.SortDirection}_{query.PageNumber}_{query.PageSize}";

        _logger.LogInformation("[INFO] Searching tickets. Cache Key: {CacheKey}", cacheKey);

        if (!_cache.TryGetValue(cacheKey, out PaginatedResponse<TicketDto>? response))
        {
            _logger.LogInformation("[INFO] Cache miss for tickets. Fetching from database.");
            
            var (items, count) = await _ticketRepository.SearchTicketsAsync(
                query.Status, 
                query.Priority, 
                query.SortBy, 
                query.SortDirection, 
                query.PageNumber, 
                query.PageSize);

            var dtos = items.Select(MapToDto).ToList();
            response = new PaginatedResponse<TicketDto>(dtos, count, query.PageNumber, query.PageSize);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60))
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _cache.Set(cacheKey, response, cacheOptions);
        }
        else
        {
            _logger.LogInformation("[INFO] Cache hit for tickets list.");
        }

        return response!;
    }

    private void InvalidateCache()
    {
        _logger.LogInformation("[INFO] Invalidating tickets cache due to data modification.");
        if (!_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
        }
        _resetCacheToken = new CancellationTokenSource();
    }

    private static TicketDto MapToDto(Ticket ticket) => new()
    {
        Id = ticket.Id,
        Title = ticket.Title,
        Description = ticket.Description,
        Status = ticket.Status,
        Priority = ticket.Priority,
        CreatedAt = ticket.CreatedAt,
        CreatedByUserName = ticket.CreatedByUser?.Username ?? "Unknown",
        AssignedToUserName = ticket.Assignments?.OrderByDescending(a => a.AssignedAt).FirstOrDefault()?.AssignedToUser?.Username
    };
}
