using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helpdesk.Domain.Common;
using Helpdesk.Domain.Enums;

namespace Helpdesk.Domain.Entities;

public class Ticket : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [Required]
    public int CreatedByUserId { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public virtual User CreatedByUser { get; set; } = null!;

    // Navigation properties for history and comments
    public virtual ICollection<TicketAssignment> Assignments { get; set; } = new List<TicketAssignment>();
    public virtual ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}
