using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Api.Controllers;

/// <summary>
/// Facilitates communication on specific tickets through comments.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new comment or reply to an existing ticket.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CommentDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CommentDto>> AddComment(CreateCommentRequest request)
    {
        _logger.LogInformation("[INFO] POST /api/comment request received for Ticket: {TicketId}", request.TicketId);
        var comment = await _commentService.AddCommentAsync(request);
        _logger.LogInformation("[INFO] Comment added successfully to Ticket: {TicketId}", request.TicketId);
        return CreatedAtAction(nameof(AddComment), comment);
    }

    /// <summary>
    /// Retrieves all comments associated with a specific ticket.
    /// </summary>
    [HttpGet("ticket/{ticketId}")]
    [ProducesResponseType(typeof(IEnumerable<CommentDto>), 200)]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByTicket(int ticketId)
    {
        _logger.LogInformation("[INFO] GET /api/comment/ticket/{TicketId} request received", ticketId);
        var comments = await _commentService.GetCommentsByTicketIdAsync(ticketId);
        return Ok(comments);
    }
}
