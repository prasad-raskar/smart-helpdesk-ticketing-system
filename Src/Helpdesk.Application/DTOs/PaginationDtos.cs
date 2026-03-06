using Helpdesk.Domain.Enums;

namespace Helpdesk.Application.DTOs;

public class TicketQueryRequest
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }
    public string? SortBy { get; set; } // e.g., "CreatedAt", "Priority", "Status"
    public string SortDirection { get; set; } = "desc"; // "asc" or "desc"
}

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public PaginatedResponse(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}
