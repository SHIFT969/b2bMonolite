using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace b2bSwgroup.Models.ModelsForView
{
    public class RequestDistributorView
    {
        [Required]
        [Display(Name ="Имя")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Компания")]
        public string CompanyName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        public override string ToString()
        {
            return string.Format("<p>Имя: {0}</p><p>Фамилия: {1}</p><p>Компания: {2}</p><p>Email: {3}</p><p>Телефон: {4}</p>", FirstName,LastName,CompanyName,
                Email,Phone);
            //return base.ToString();
        }
    }
}