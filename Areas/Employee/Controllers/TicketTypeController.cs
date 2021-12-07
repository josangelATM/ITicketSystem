using ITicketSystem.Data;
using ITicketSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize]
    public class TicketTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TicketTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            TicketTypeVM ticketTypeVM = new()
            {
                TicketTypes = _db.TicketTypes.ToList()
            };

            return View(ticketTypeVM);
        }
    }
}
