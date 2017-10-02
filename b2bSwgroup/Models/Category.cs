using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class Category
    {                
        public int Id { get; set; }
        [Display(Name ="Наименование категории")]
        public string Name { get; set; }
        [Display(Name="Позиции каталога")]
        public ICollection<PositionCatalog> PositionsCatalog { get; set; }
        public Category()
        {
            PositionsCatalog = new List<PositionCatalog>();
        }

    }
}