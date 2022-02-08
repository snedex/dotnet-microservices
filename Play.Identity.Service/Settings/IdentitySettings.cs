using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Identity.Service.Settings
{
    public class IdentitySettings
    {
        public string AdminUserName { get; init; }

        public string AdminUserPassword { get; init; }   

        public int StartingGil { get; init; }
    }
}