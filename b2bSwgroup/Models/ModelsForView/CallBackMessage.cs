using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models.ModelsForView
{
    public class CallBackMessage
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email для связи")]
        public string Email { get; set; }
        [Required]
        [Display(Name ="Заголовок")]
        public string Subject { get; set; }
        [Required]
        [Display(Name ="Описание")]
        public string Body { get; set; }
    }
}