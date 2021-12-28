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

namespace ITicketSystem.Areas.IT.Controllers
{   
    [Area("IT")]
    [Authorize(Roles = Const.Role_IT + "," + Const.Role_Admin)]
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

        #region API
        public IActionResult GetAll()
        {
            var ticketsType = _db.TicketTypes.ToList();
            var jsonData = new
            {
                data = ticketsType
            };


            return Ok(jsonData);
        }

        
        #endregion

    }
}
