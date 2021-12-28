using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Models.ViewModels
{
    public class UserVM
    {
        public ApplicationUser user { get; set; }

        public string Role { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }

    }
}
