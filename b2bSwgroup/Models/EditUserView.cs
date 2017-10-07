using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace b2bSwgroup.Models
{
    public class EditUserView
    {
        public IdentityUser  User { get; set; }
        
        public List<string> RoleId { get; set; }
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public EditUserView()
        {

        }
    }
}