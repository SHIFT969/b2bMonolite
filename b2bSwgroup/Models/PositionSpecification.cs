using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class PositionSpecification
    {
        public int Id { get; set; }
        [Display(Name = "Артикул")]
        public string Articul { get; set; }
        [Display(Name = "P/N")]
        public string PartNumber { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Стоимость")]
        public double Price { get; set; }
        [Display(Name = "Валюта")]
        public int? CurrencyId { get; set; }
        public Currency Currency { get; set; }
        [Display(Name = "Категория")]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        [Display(Name = "Дистрибьютер")]
        public int? DistributorId { get; set; }
        public Distributor Distributor { get; set; }
        public string DistributorApplicationUserId { get; set; }
        public DistributorApplicationUser DistributorApplicationUser { get; set; }
        public int? SpecificationId { get; set; }
        public Specification Specification { get; set; }

        //public PositionSpecification()
        //{
        //    Specifications = new List<Specification>();
        //}
    }
}