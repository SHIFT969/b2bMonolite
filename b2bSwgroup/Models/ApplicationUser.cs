using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
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