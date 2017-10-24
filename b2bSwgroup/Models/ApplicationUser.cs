using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;

namespace b2bSwgroup.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        [Display(Name ="Дата регистрации")]
        public DateTime DateRegistration { get; set; }
        [Display(Name = "Дата Последнего входа")]
        public DateTime DateLastLogin { get; set; }
        [Display(Name ="Логин")]
        [Required]
        
        
        public override string UserName
        {
            get
            {
                return base.UserName;
            }

            set
            {
                base.UserName = value;
            }
        }
        public ApplicationUser()
        {            
                 
        }
    }
}