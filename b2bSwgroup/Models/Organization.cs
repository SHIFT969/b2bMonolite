using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class Organization
    {
        public int Id { get; set; }
        [Display(Name="Наименование организации")]
        public string Name { get; set; }
        [Display(Name ="ИНН организации")]
        public string INN { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public Organization()
        {
            ApplicationUsers = new List<ApplicationUser>();
        }
    }
}