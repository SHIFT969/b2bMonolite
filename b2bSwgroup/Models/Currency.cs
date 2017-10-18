using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class Currency
    {
        public int Id { get; set; }
        [Display(Name="Валюта")]
        public string IsoCode { get; set; }
        [Display(Name = "Локаль")]
        public string СultureInfo { get; set; }
        [Display(Name = "Наименование валюты")]
        public string Name { get; set; }     
    }
}