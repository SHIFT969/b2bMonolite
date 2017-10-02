using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace b2bSwgroup.Models
{
    public class ApplicationContext :  IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("b2bDb1") { }
        public DbSet<PositionCatalog> Positionscatalog { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        //public DbSet<DistributorApplicationUser> DistributorApplicationUsers { get; }
        //public DbSet<CustomerApplUser> CustomerApplUsers { get; }

        //public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }

  
}