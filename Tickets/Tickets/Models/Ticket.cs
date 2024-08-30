using System;
using System.ComponentModel.DataAnnotations;


namespace Tickets.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TicketNo { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string TicketBody { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    
        public string Priority { get; set; } // Add this field

        public StatusEnum Status { get; set; } // Use the enum here

        public DateTime? Deadline { get; set; } // New field for deadline

        public int? AssignedUserId { get; set; }


    }
}
