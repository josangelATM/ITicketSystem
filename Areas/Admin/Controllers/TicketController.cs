using AspNetCoreHero.ToastNotification.Abstractions;
using ITicketSystem.Data;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Const.Role_Admin)]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TicketController> _logger;
        private readonly INotyfService _notyf;
        public TicketController(ApplicationDbContext db,
           ILogger<TicketController> logger,
           INotyfService notyf)
        {
            _db = db;
            _logger = logger;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
           
            return View();
        }

        #region API
        [HttpGet] 
        public IActionResult GetAll()
        {

            var tickets = _db.Tickets.Select(x => new
            {
                ticket = x,
                requester = new
                {
                    id = x.Requester.Id,
                    userName = x.Requester.UserName
                },
                ticketTypeTitle = x.TicketType.Title,
                assignedTo = new
                {
                    id = x.AssignedTo.Id,
                    userName = x.AssignedTo.UserName
                }
            });
            var jsonData = new
            {
                data = tickets
            };


            return Ok(jsonData);
        }

  
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var ticketFromDb = _db.Tickets.FirstOrDefault(t => t.Id == id);
            _db.Tickets.Remove(ticketFromDb);
            _db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Delete Succesful"
            });
        }


        #endregion
    }
}
