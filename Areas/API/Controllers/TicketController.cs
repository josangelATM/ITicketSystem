using ITicketSystem.Areas.API.Models;
using ITicketSystem.Data;
using ITicketSystem.Models;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TicketController> _logger;
        private readonly IAssignationManager _assignationManager;

        public TicketController(ApplicationDbContext db,
             ILogger<TicketController> logger,
             IAssignationManager assignationManager)
        {
            _db = db;
            _logger = logger;
            _assignationManager = assignationManager;
        }

        [HttpGet]
        [Route("api/tickets")]
        public IActionResult Get()
        {
            IQueryable tickets;

            if (User.IsInRole(Const.Role_Admin))
            {
                tickets = _db.Tickets.Select(x => new
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
            }
            else if (User.IsInRole(Const.Role_IT))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = _db.ApplicationUsers.FirstOrDefault(a => a.UserName == claimsIdentity.Name).Id;
                tickets = _db.Tickets.Where(a => a.AssignedToId == userId).Select(x => new
                {
                    ticket = x,
                    requester = new
                    {
                        id = x.Requester.Id,
                        userName = x.Requester.UserName
                    },
                    ticketTypeTitle = x.TicketType.Title
                });
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = _db.ApplicationUsers.FirstOrDefault(a => a.UserName == claimsIdentity.Name).Id;
                tickets = _db.Tickets.Where(a => a.RequesterId == userId).Select(x => new
                {
                    ticket = x,
                    assignedTo = new
                    {
                        id = x.AssignedTo.Id,
                        username = x.AssignedTo.UserName,

                    },
                    ticketTypeTitle = x.TicketType.Title
                });
            }

            var jsonData = new
            {
                data = tickets
            };

            return Ok(jsonData);
        }

        [HttpDelete]
        [Authorize(Roles = Const.Role_Admin)]
        [Route("api/tickets/{id}")]
        public IActionResult Delete(int id)
        {
            var ticketFromDb = _db.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticketFromDb == null)
            {
                Response.StatusCode = 404;
                return Json(new Response
                {
                    Status = "Not Found",
                    Message = "Ticket Type not found"
                });
            }

            _db.Tickets.Remove(ticketFromDb);
            _db.SaveChanges();

            return Json(new Response
            {
                Status = "Success",
                Message = "Ticket #" + id + " deleted."
            });
        }

        [HttpPost]
        [Route("api/tickets")]
        public IActionResult Create([FromBody] TicketModel ticketModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); Fix this one later.
            var userId = _db.ApplicationUsers.FirstOrDefault(a => a.UserName == claimsIdentity.Name).Id;

            Ticket ticket = new Ticket()
            {
                RequesterId = userId,
                AssignedToId = _assignationManager.GetNextIT(),
                TicketTypeId = ticketModel.TicketTypeId,
                Comments = ticketModel.Comments,
                Status = Const.Status_Assigned
            };
            _db.Tickets.Add(ticket);

            _db.SaveChanges();

            return Json(new Response
            {
                Status = "Success",
                Message = "Ticket created."
            });
        }

        [HttpGet]
        [Route("api/tickets/{id}")]
        public IActionResult Get(int id)
        {
            var ticketInDb = _db.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticketInDb == null)
            {
                Response.StatusCode = 404;
                return Json(new Response
                {
                    Status = "Not Found",
                    Message = "Ticket not found"
                });
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); Fix this one later.
            var userId = _db.ApplicationUsers.FirstOrDefault(a => a.UserName == claimsIdentity.Name).Id;

            if (ticketInDb.AssignedToId != userId && ticketInDb.RequesterId != userId && !User.IsInRole(Const.Role_Admin))
            {
                Response.StatusCode = 403;
                return Json(new Response
                {
                    Status = "Forbidden",
                    Message = "You do not have permission to access this resource"
                });
            }

            return Json(new
            {
                data = ticketInDb
            });

        }

        [HttpPatch]
        [Authorize(Roles = Const.Role_IT + "," + Const.Role_Admin)]
        [Route("api/tickets/{id}")]
        public IActionResult Update(int id, [FromBody] JsonPatchDocument <TicketModel> patchTicket)
        {
            var ticketInDb = _db.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticketInDb == null)
            {
                Response.StatusCode = 404;
                return Json(new Response
                {
                    Status = "Not Found",
                    Message = "Ticket not found"
                });
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); Fix this one later.
            var userId = _db.ApplicationUsers.FirstOrDefault(a => a.UserName == claimsIdentity.Name).Id;

            if (ticketInDb.AssignedToId != userId && !User.IsInRole(Const.Role_Admin))
            {
                Response.StatusCode = 403;
                return Json(new Response
                {
                    Status = "Forbidden",
                    Message = "You do not have permission to access this resource"
                });
            }

            TicketModel ticketUpdate = new TicketModel{ };
            patchTicket.ApplyTo(ticketUpdate);

            ticketInDb.Status = ticketUpdate.Status;
            ticketInDb.Response = ticketUpdate.Response;


            _db.SaveChanges();


            return Json(new Response
            {
                Status = "Success",
                Message = "Ticket Updated"
            });

        }
    }
}
