using ITicketSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Utility
{
    public interface IAssignationManager
    {
        public string GetNextIT();
        public void IncreaseCountTicket(string id);
        public void ReduceCountTicket(string id);


    }
}
