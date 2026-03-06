using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Helpdesk.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Application.Services;

public class CommentService : ICommentService
{
    private readonly IGenericRepository<TicketComment> _commentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<CommentService> _logger;

    public CommentService(
        IGenericRepository<TicketComment> commentRepository, 
        ITicketRepository ticketRepository,
        ILogger<CommentService> logger)
    {
        _commentRepository = commentRepository;
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentRequest request)
    {
        _logger.LogInformation("[INFO] Attempting to add comment to Ticket ID: {TicketId} by User ID: {UserId}", request.TicketId, request.UserId);
        
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
        if (ticket == null) 
        {
            _logger.LogError("[ERROR] Failed to add comment. Ticket ID: {TicketId} not found.", request.TicketId);
            throw new Exception("Ticket not found.");
        }

        var comment = new TicketComment
        {
            TicketId = request.TicketId,
            UserId = request.UserId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        _logger.LogInformation("[INFO] Comment added successfully to Ticket ID: {TicketId}", request.TicketId);

        return new CommentDto
        {
            Id = comment.Id,
            TicketId = comment.TicketId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UserName = "User" 
        };
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByTicketIdAsync(int ticketId)
    {
        _logger.LogInformation("[INFO] Fetching comments for Ticket ID: {TicketId}", ticketId);
        var comments = await _commentRepository.FindAsync(c => c.TicketId == ticketId);
        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            TicketId = c.TicketId,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            UserName = "User"
        });
    }
}
