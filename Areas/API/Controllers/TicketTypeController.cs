using ITicketSystem.Areas.API.Models;
using ITicketSystem.Data;
using ITicketSystem.Models;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class TicketTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TicketTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        [Route("api/TicketTypes")]
        public IActionResult Get()
        {
            var jsonData = new
            {
                data = _db.TicketTypes.ToList()
            };

            return Ok(jsonData);
        }

        [HttpGet]
        [Authorize(Roles = Const.Role_Admin)]
        [Route("api/TicketTypes/{id}")]
        public IActionResult Get(int id)
        {
            var ticketType = _db.TicketTypes.FirstOrDefault(t => t.Id == id);

            if (ticketType == null)
            {
                Response.StatusCode = 404;
                return Json(new Response
                {
                    Status = "Not Found",
                    Message = "Ticket Type not found"
                });
            }
            return Json(new
            {
                data = ticketType
            });
        }

        [HttpPost]
        [Authorize(Roles = Const.Role_Admin)]
        [Route("api/TicketTypes/")]
        public IActionResult Create([FromBody] TicketType ticketType)
        {
            _db.TicketTypes.Add(ticketType);

            _db.SaveChanges();

            return Json(new Response
            {
                Status = "Success",
                Message = "Ticket Type Created"
            });

        }

        [HttpDelete]
        [Authorize(Roles = Const.Role_Admin)]
        [Route("api/TicketTypes/{id}")]
        public IActionResult Delete(int id)
        {
            var ticketType = _db.TicketTypes.FirstOrDefault(t => t.Id == id);

            if (ticketType == null)
            {
                Response.StatusCode = 404;
                return Json(new Response
                {
                    Status = "Not Found",
                    Message = "Ticket Type not found"
                });
            }

            _db.TicketTypes.Remove(ticketType);
            _db.SaveChanges();

            return Json(new Response
            {
                Status = "Success",
                Message = "Ticket Type #" + id + " deleted."
            });
        }

        [HttpPatch]
        [Authorize(Roles = Const.Role_Admin)]
        [Route("api/TicketTypes/{id}")]
        public IActionResult Update(int id, [FromBody] JsonPatchDocument<TicketType> patchTicketType)
        {
            var ticketTypeFromDb = _db.TicketTypes.FirstOrDefault(t => t.Id == id);
            if (ticketTypeFromDb == null)
            {
                Response.StatusCode = 404;
                return Json(new Response
                {
                    Status = "Not Found",
                    Message = "Ticket Type not found"
                });
            }

            patchTicketType.ApplyTo(ticketTypeFromDb);
           
            _db.SaveChanges();


            return Json(new Response
            {
                Status = "Success",
                Message = "Ticket Type Updated"
            });

        }
    }
}

