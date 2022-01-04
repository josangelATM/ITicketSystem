using ITicketSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.API.Models
{
    public class RegisterModel : ApplicationUser
    {
        public string Password { get; set; }
    }
}
