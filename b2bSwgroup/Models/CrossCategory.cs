using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class CrossCategory
    {
        public int Id { get; set; }
        [Display(AutoGenerateField =false)]
        public int? DistributorId { get; set; }
        public Distributor Distributor { get; set; }
        [Display(Name ="Внутренний идентификатор категории")]
        public string IdentifyCategory { get; set; }
        [Display(Name = "Категория")]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}