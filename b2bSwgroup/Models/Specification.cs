using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class Specification
    {
        [Display(AutoGenerateField = false)]
        public int Id { get; set; }
        [Display(Name="Наименование")]
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<PositionSpecification> PositionsSpecification { get; set; }
        //public int? CustomerApplUserId { get; set; }
        //public CustomerApplUser CustomerApplUser { get; set; }
        [Display(Name="Комментарий")]
        public string Comment { get; set; }
        [Display(Name ="Заказчик")]
        public string Zakazchik { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime DateCreate { get; set; }
        [Display(Name = "Дата последнего изменения")]
        public DateTime DateEdit { get; set; }

        public Specification()
        {
            PositionsSpecification = new List<PositionSpecification>();
        }
    }
}