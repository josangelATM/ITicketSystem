using AspNetCoreHero.ToastNotification.Abstractions;
using ITicketSystem.Data;
using ITicketSystem.Models.ViewModels;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.IT.Controllers
{
    [Area("IT")]
    [Authorize(Roles = Const.Role_IT+","+Const.Role_Admin)]
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = _db.ApplicationUsers.FirstOrDefault(a => a.Id == claim.Value).Id;
            ViewBag.userId = userId;
            return View();  
        }

        public IActionResult Details(int id)
        {
            _logger.LogInformation("WHYYY?");
            var ticketInDb = _db.Tickets.FirstOrDefault(t => t.Id == id);
            TicketVM ticketVM = new TicketVM()
            {
                ticket = ticketInDb,
                ticketType = _db.TicketTypes.FirstOrDefault(t => t.Id == ticketInDb.TicketTypeId)
            };

            var statusOptions = new List<SelectListItem>();
            statusOptions.Add(new SelectListItem()
            {
                Text = Const.Status_Assigned,
                Value = Const.Status_Assigned
            });
            statusOptions.Add(new SelectListItem()
            {
                Text = Const.Status_Completed,
                Value = Const.Status_Completed
            });
            statusOptions.Add(new SelectListItem()
            {
                Text = Const.Status_Failed,
                Value = Const.Status_Failed
            });
            statusOptions.Add(new SelectListItem()
            {
                Text = Const.Status_Working,
                Value = Const.Status_Working
            });

            ViewBag.statusOptions = statusOptions;
            return View(ticketVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(TicketVM ticketVM)
        {
            var ticketFromDb = _db.Tickets.FirstOrDefault(t => t.Id == ticketVM.ticket.Id);

            ticketFromDb.Response = ticketVM.ticket.Response;
            ticketFromDb.Status = ticketVM.ticket.Status;

            _db.SaveChanges();

            _notyf.Success("Ticket updated.");
            return RedirectToAction("Index");
            
        }

        #region API 
        //IT/Ticket/GetTickets
        public IActionResult GetTickets(string Id)
        {

            var tickets = _db.Tickets.Where(a => a.AssignedToId == Id).Select(x => new
            {
                ticket = x,
                requester =  new
                {
                    id = x.Requester.Id,
                    userName = x.Requester.UserName
                },
                ticketTypeTitle = x.TicketType.Title
            });
            var jsonData = new
            {
                data = tickets
            };


            return Ok(jsonData);
        }


        #endregion
    }
}
