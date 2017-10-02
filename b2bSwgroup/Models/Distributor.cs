﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace b2bSwgroup.Models
{
    public class Distributor : Organization
    {
        [Display(Name="Позиции каталога")]
        public ICollection<PositionCatalog> PositionsCatalog { get; set; }
        public Distributor()
        {
            PositionsCatalog = new List<PositionCatalog>();
        }
    }
}