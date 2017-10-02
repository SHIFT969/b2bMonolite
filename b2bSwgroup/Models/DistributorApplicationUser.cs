using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace b2bSwgroup.Models
{
    public class DistributorApplicationUser : ApplicationUser
    {
        ICollection<PositionCatalog> PositionsCatalog { get; set; }
      
        public DistributorApplicationUser()
        { }
    }
}