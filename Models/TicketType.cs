using System.ComponentModel.DataAnnotations;

namespace ITicketSystem.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

    }
}
