using Helpdesk.Application.DTOs;
using Helpdesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Api.Controllers;

/// <summary>
/// Manages ticket operations including creation, retrieval, assignment, and lifecycle updates.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Tickets")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketController> _logger;

    public TicketController(ITicketService ticketService, ILogger<TicketController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of tickets with optional filtering and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<TicketDto>), 200)]
    public async Task<ActionResult<PaginatedResponse<TicketDto>>> GetAll([FromQuery] TicketQueryRequest query)
    {
        _logger.LogInformation("[INFO] GET /api/tickets request received");
        var result = await _ticketService.SearchTicketsAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves full details of a specific ticket by its ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TicketDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TicketDto>> GetById(int id)
    {
        _logger.LogInformation("[INFO] GET /api/tickets/{Id} request received", id);
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null) 
        {
            _logger.LogWarning("[WARN] Ticket {Id} not found", id);
            return NotFound();
        }
        return Ok(ticket);
    }

    /// <summary>
    /// Creates a new support ticket.
    /// </summary>
    [Authorize(Roles = "User,Agent,Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<TicketDto>> Create(CreateTicketRequest request)
    {
        _logger.LogInformation("[INFO] POST /api/tickets request received for title: {Title}", request.Title);
        var ticket = await _ticketService.CreateTicketAsync(request);
        _logger.LogInformation("[INFO] Ticket created successfully with ID: {Id}", ticket.Id);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    /// <summary>
    /// Updates the status of an existing ticket.
    /// </summary>
    [Authorize(Roles = "Agent,Admin")]
    [HttpPatch("{id}/status")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        _logger.LogInformation("[INFO] PATCH /api/tickets/{Id}/status request received. New status: {Status}", id, status);
        await _ticketService.UpdateTicketStatusAsync(id, status);
        return NoContent();
    }

    /// <summary>
    /// Assigns a ticket to a specific agent.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AssignTicket(AssignTicketRequest request)
    {
        _logger.LogInformation("[INFO] POST /api/tickets/assign request received for Ticket: {TicketId}", request.TicketId);
        await _ticketService.AssignTicketAsync(request);
        return Ok(new { Message = "Ticket successfully assigned." });
    }

    /// <summary>
    /// Deletes a ticket from the system.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("[INFO] DELETE /api/tickets/{Id} request received", id);
        await _ticketService.DeleteTicketAsync(id);
        return NoContent();
    }
}
