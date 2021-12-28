using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using ITicketSystem.Data;
using ITicketSystem.Models;
using ITicketSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly INotyfService _notyf;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            INotyfService notyf,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserController> logger)
        {
            _logger = logger;
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
            _notyf = notyf;
        }

        // GET: UserController
        public ActionResult Index()
        {

            return View();
        }

      

        // GET: UserController/Edit/5
        public async Task<ActionResult> EditAsync(string id)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userFromDb == null)
            {
                Response.StatusCode = 404;
                ViewBag.Message = "Requested User not found, sorry.";
                return View("~/Areas/Employee/Views/Home/Error.cshtml");
            }

            var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == userFromDb.Id);
            var roleName = await _roleManager.FindByIdAsync(userRole.RoleId);
            UserVM appUser = new UserVM
            {
                user = userFromDb,
                RoleList = _roleManager.Roles.Select(r => r.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                Role = roleName.Name
            };

            return View(appUser);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(UserVM userEdit)
        {
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ApplicationUser, ApplicationUser>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)));
            var mapper = new Mapper(config);

            try
            {                
                var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userEdit.user.Id);
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == userFromDb.Id);
                var roleName = await _roleManager.FindByIdAsync(userRole.RoleId);
                if(roleName.Name != userEdit.Role)
                {
                    await _userManager.RemoveFromRoleAsync(userFromDb, roleName.Name);
                    await _userManager.AddToRoleAsync(userFromDb, userEdit.Role);
                }

                mapper.Map(userEdit.user, userFromDb);
                _db.SaveChanges();
                _notyf.Success("User Updated");
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                _notyf.Error("Error while tried updating");
                return View(userEdit);
            }
        }

        #region API
        public IActionResult GetAll()
        {
            var users = _db.ApplicationUsers.Select(x => new
            {
               user = x,
               role = _userManager.GetRolesAsync(x).Result[0]
            });
            var jsonData = new
            {
                data = users
            };


            return Ok(jsonData);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(user);

                return Json( new { 
                success = true,
                message = "User deleted"
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = "Error in server"
                });
            }
        }
        #endregion
    }
}
