using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helpdesk.Domain.Common;

namespace Helpdesk.Domain.Entities;

public class TicketAssignment : BaseEntity
{

    [Required]
    public int TicketId { get; set; }

    [ForeignKey(nameof(TicketId))]
    public virtual Ticket Ticket { get; set; } = null!;

    [Required]
    public int AssignedToUserId { get; set; }

    [ForeignKey(nameof(AssignedToUserId))]
    public virtual User AssignedToUser { get; set; } = null!;

    [Required]
    public int AssignedByUserId { get; set; }

    [ForeignKey(nameof(AssignedByUserId))]
    public virtual User AssignedByUser { get; set; } = null!;

    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
