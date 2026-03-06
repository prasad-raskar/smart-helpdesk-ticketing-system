using System.ComponentModel.DataAnnotations;
using Helpdesk.Domain.Common;

namespace Helpdesk.Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "User"; // Admin, Agent, User

    // Navigation Properties
    public virtual ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
    public virtual ICollection<TicketAssignment> AssignmentsTo { get; set; } = new List<TicketAssignment>();
    public virtual ICollection<TicketAssignment> AssignmentsBy { get; set; } = new List<TicketAssignment>();
    public virtual ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}
