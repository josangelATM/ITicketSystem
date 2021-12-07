using ITicketSystem.Models;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Data
{
    public class DbSeeder : IDbSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbSeeder(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(Const.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Const.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Const.Role_IT)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Const.Role_Employee)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin",
                    EmployeeNumber = 1,
                    Email = "admin@gmail.com",
                    FirstName = "Admin",
                    Lastname = "Admin",
                    Position = "IT Admin",
                },"Abcd1234*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");

                _userManager.AddToRoleAsync(user, Const.Role_Admin).GetAwaiter().GetResult();

            }
            if (!_db.TicketTypes.Any())
            {
                TicketType ticketTypePwdReset = new TicketType
                {
                    Title = "Password Reset",
                    Description = "Password reset for any account in the comments section please type the username and the new password"
                };

                TicketType ticketTypeUserDisabled = new TicketType
                {
                    Title = "User Disabled",
                    Description = "To enable your username, please in the comments section please type ther username to be enabled"
                };

                TicketType ticketTypeHardwareRepair = new TicketType
                {
                    Title = "Hardware Repair",
                    Description = "In the comments sections please give us a explanation about your issue and where are you located."
                };

                _db.TicketTypes.Add(ticketTypePwdReset);
                _db.TicketTypes.Add(ticketTypeUserDisabled);
                _db.TicketTypes.Add(ticketTypeHardwareRepair);

                _db.SaveChanges();

            }
            return;
        }
    }
}
