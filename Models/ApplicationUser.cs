using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public int EmployeeNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public string Position { get; set; }

        public int TicketAssignedCount { get; set; }

    }
}
