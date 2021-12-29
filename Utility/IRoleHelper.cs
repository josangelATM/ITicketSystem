using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITicketSystem.Utility
{
    public interface IRoleHelper
    {
        public Task<string> GetRoleName(string Id);
    }
}
