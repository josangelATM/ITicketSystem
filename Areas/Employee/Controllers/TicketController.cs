using AspNetCoreHero.ToastNotification.Abstractions;
using ITicketSystem.Data;
using ITicketSystem.Models;
using ITicketSystem.Models.ViewModels;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TicketController> _logger;
        private readonly IAssignationManager _assignationManager;
        private readonly INotyfService _notyf;

        public TicketController(ApplicationDbContext db,
            ILogger<TicketController> logger, 
            IAssignationManager assignationManager,
            INotyfService notyf)
        {
            _logger = logger;
            _db = db;
            _assignationManager = assignationManager;
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
            var ticketInDb = _db.Tickets.FirstOrDefault(t => t.Id == id);
            TicketVM ticketVM = new TicketVM()
            {
                ticket = ticketInDb,
                ticketType = _db.TicketTypes.FirstOrDefault(t => t.Id == ticketInDb.TicketTypeId)
            };
            return View(ticketVM);
        }

        public IActionResult Create(int id)
        {

            TicketVM ticketVM = new TicketVM()
            {
                ticketType = _db.TicketTypes.FirstOrDefault(t => t.Id == id)
            };

            _logger.LogInformation("Here");
            return View(ticketVM); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TicketVM ticketVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var requesterId = _db.ApplicationUsers.FirstOrDefault(a => a.Id == claim.Value).Id;
            _logger.LogInformation($"RequesterId = {requesterId}");
            Ticket ticket = new Ticket()
            {
                RequesterId = requesterId,
                AssignedToId = _assignationManager.GetNextIT(),
                TicketTypeId = ticketVM.ticketType.Id,
                Comments = ticketVM.ticket.Comments,
                Status = Const.Status_Assigned
            };

            _db.Tickets.Add(ticket);

            _db.SaveChanges();

            _notyf.Success("Ticket created");

            return RedirectToAction("Index","TicketType");
        }

        #region API 
        //Employee/Ticket/GetTickets
        public IActionResult GetTickets(string Id)
        {

            _logger.LogInformation(Id);
            var tickets = _db.Tickets.Where(a => a.RequesterId == Id).Select(x => new
            {
                ticket = x,
                assignedTo = new 
                { 
                    id = x.AssignedTo.Id,
                    username = x.AssignedTo.UserName,
                    
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
