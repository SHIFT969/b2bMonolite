using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models.ModelsForView
{
    public class PositionCatalogIndexView
    {
        public int Id { get; set; }
        [Display(Name = "Артикул")]
        public string Articul { get; set; }
        [Display(Name = "P/N")]
        public string PartNumber { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Стоимость")]
        public string Price { get; set; }

        private string category;
        [Display(Name = "Категория")]      
        public string Category
        {
            get { return category; }
            set
            {
                if (value != null)
                {
                    category = value;
                }
                else
                {
                    category = "";
                }
            }
        }
        [Display(Name = "Количество")]
        public string Quantity { get; set; }
        private string _distributor { get; set; }
        [Display(Name = "Дистрибьютер")]
        public string Distributor
        {
            get { return _distributor; }
            set
            {
                if (value != null)
                {
                    _distributor = value;
                }
                else
                {
                    _distributor = "";
                }
            }
        }
    }
}