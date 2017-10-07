using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace b2bSwgroup.Models
{
    public class UserAdmin
    {
        public ApplicationUser User { get; set; }
        public List<ApplicationRole> Roles { get; set; }
      
       
        public UserAdmin()
        {
            Roles = new List<ApplicationRole>();
        }
    }
}