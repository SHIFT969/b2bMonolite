using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace b2bSwgroup.Models
{
    public class Customer : Organization
    {
        public ICollection<Specification> Specifications { get; set; }
        public Customer()
        {
            Specifications = new List<Specification>();
        }
    }
}