using ITicketSystem.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Utility
{
    public class RoleHelper : IRoleHelper
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public RoleHelper(ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }


        public async Task<string> GetRoleName(string Id)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == Id);
            var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == userFromDb.Id);
            var roleName = await _roleManager.FindByIdAsync(userRole.RoleId);
            return roleName.Name;
        }
    }
}
