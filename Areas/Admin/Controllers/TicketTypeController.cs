using AspNetCoreHero.ToastNotification.Abstractions;
using ITicketSystem.Data;
using ITicketSystem.Models;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = Const.Role_Admin)]
    public class TicketTypeController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _db;

        public TicketTypeController(ApplicationDbContext db,
            INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TicketType ticketType)
        {
            _db.TicketTypes.Add(ticketType);

            _db.SaveChanges();

            _notyf.Success("Ticket Type created.");

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {


            var ticketType = _db.TicketTypes.FirstOrDefault(t => t.Id == id);

            if (ticketType == null)
            {
                Response.StatusCode = 404;
                ViewBag.Message = "Requested ticket type not found, sorry.";
                return View("~/Areas/Employee/Views/Home/Error.cshtml");
            }
            return View(ticketType);
        }

        [HttpPost]
        public IActionResult Edit(TicketType ticketType)
        {
            var ticketTypeFromDb = _db.TicketTypes.FirstOrDefault(t => t.Id == ticketType.Id);
            if (ticketTypeFromDb == null)
            {
                Response.StatusCode = 404;
                ViewBag.Message = "Requested ticket type not found, sorry.";
                return View("~/Areas/Employee/Views/Home/Error.cshtml");
            }

            ticketTypeFromDb.Title = ticketType.Title;
            ticketTypeFromDb.Description = ticketType.Description;

            _db.SaveChanges();
            _notyf.Success("Ticket updated.");
            return RedirectToAction("Index");
        }

        #region API
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var ticketTypeFromDb = _db.TicketTypes.FirstOrDefault(t => t.Id == id);
            _db.TicketTypes.Remove(ticketTypeFromDb);
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
