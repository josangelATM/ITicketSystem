using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Models.ViewModels
{
    public class TicketVM
    {
        public TicketType ticketType { get; set; }

        public Ticket ticket { get; set; }

        public IEnumerable<SelectListItem> ticketTypesSelect { get; set; }
    }
}
