using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace b2bSwgroup.Models
{
    public class Shema
    {
        public int Id { get; set; }
        [Display(Name = "Дистрибьютер")]
        public int? DistributorId { get; set; }
        public Distributor Distributor { get; set; }

        [Required]
        [Display(Name = "Лист №")]
        public int Sheet { get; set; }
        [Required]
        [Display(Name = "Первая строка")]
        public int FistRow { get; set; }
        [Required]
        [Display(Name = "Ключевая колонка")]
        public int KeyColumn { get; set; }
        
        [Display(Name = "Игнорировать значение")]
        public string IgnoreValue { get; set; }
        [Display(Name = "По колонке")]
        public int IgnoreColumn { get; set; }

        public List<ShemaMember> ShemaMembers { get; set; }

        public Shema()
        {
            ShemaMembers = new List<ShemaMember>();
        }

        public Shema(ApplicationUser currentUser) : this()
        {
            DistributorId = currentUser.OrganizationId;
            Distributor = currentUser.Organization as Distributor;
        }
    }

    public class ShemaMember
    {
        public ShemaMember()
        {
            Column = -1;
            DefaultValue = string.Empty;
            ParentLevel = -1;
        }
        public ShemaMember(string name) : this()
        {
            Name = name;
        }

        public int Id { get; set; }
        public Shema Shema { get; set; }
        public int? ShemaId { get; set; }

        [Required]
        [Display(Name = "Колонка")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Номер колонки")]
        public int Column { get; set; }
        [Display(Name = "Значение по умолчанию")]
        public string DefaultValue { get; set; }
        [Required]
        [Display(Name = "Значение из родителя (указать уровень)")]
        public int ParentLevel { get; set; }
    }

}