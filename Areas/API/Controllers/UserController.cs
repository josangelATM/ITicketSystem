using ITicketSystem.Data;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("api/Users")]
        [Authorize(Roles = Const.Role_Admin)]
        public IActionResult Get()
        {
            var users = _db.ApplicationUsers.Select(x => new
            {
                user = x
                //role = _roleHelper.GetRoleName(x.Id).Result //Need to check, working on dev but not in production
                //Workaround was change with Position in DT.
            });

            return Json(new { data = users });
        }
    }
}
//FEATURE COMING SOON 