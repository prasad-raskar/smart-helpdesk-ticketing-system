using Helpdesk.Application.DTOs;

namespace Helpdesk.Application.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
    Task<TicketDto?> GetTicketByIdAsync(int id);
    Task<TicketDto> CreateTicketAsync(CreateTicketRequest request);
    Task UpdateTicketStatusAsync(int id, string status);
    Task AssignTicketAsync(AssignTicketRequest request);
    Task<PaginatedResponse<TicketDto>> SearchTicketsAsync(TicketQueryRequest query);
    Task DeleteTicketAsync(int id);
}
