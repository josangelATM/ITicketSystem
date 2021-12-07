using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public string RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public ApplicationUser Requester { get; set; }

        public string AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public ApplicationUser AssignedTo { get; set; }

        [Required]
        public int TicketTypeId { get; set; }

        [ForeignKey("TicketTypeId")]
        public TicketType TicketType { get; set; }

        [Required]
        public string Comments { get; set; }

        [Required]
        public string Status { get; set; }

        public string Response { get; set; }

    }
}
