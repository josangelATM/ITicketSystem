using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.API.Models
{
    public class TicketModel
    {
        public int TicketTypeId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }

    }
}
