using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace b2bSwgroup.Models
{
    public class CustomerApplUser : ApplicationUser
    {
        public ICollection<Specification> Specifications { get; set; }
        public CustomerApplUser()
        {

        }
    }
}