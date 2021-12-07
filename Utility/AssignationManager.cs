using ITicketSystem.Data;
using ITicketSystem.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Utility
{
    
    public class AssignationManager : IAssignationManager
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public AssignationManager(ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public string GetNextIT()
        {
            IEnumerable<string> rolesID = _roleManager.Roles.Where(r => r.Name == Const.Role_IT || r.Name == Const.Role_Admin).Select(r => r.Id);

            IEnumerable<string> ITUsersId = _db.UserRoles.Where(u => rolesID.Contains(u.RoleId)).Select(u => u.UserId);

            IEnumerable<ApplicationUser> ITAvailables = _db.ApplicationUsers.Where(u => ITUsersId.Contains(u.Id));

            Random rnd = new Random();
            int minTicketsAssigned = ITAvailables.Min(a => a.TicketAssignedCount);
            ITAvailables = ITAvailables.Where(a => a.TicketAssignedCount == minTicketsAssigned).ToList();
            var NextIT = ITAvailables.ElementAt(rnd.Next(0, ITAvailables.Count()));
            NextIT.TicketAssignedCount += 1;
            _db.SaveChanges();
            return (NextIT.Id);
        }

        public void IncreaseCountTicket(string id)
        {
            ApplicationUser IT = _db.ApplicationUsers.FirstOrDefault(a => a.Id == id);
            IT.TicketAssignedCount += 1;
            _db.SaveChanges();
        }

        public void ReduceCountTicket(string id)
        {
            ApplicationUser IT = _db.ApplicationUsers.FirstOrDefault(a => a.Id == id);
            IT.TicketAssignedCount -= 1;
            _db.SaveChanges();
        }



    }
}
