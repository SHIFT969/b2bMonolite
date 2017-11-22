using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using b2bSwgroup.Models.ModelsForView;

namespace b2bSwgroup.Models
{
    public class PageInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItem { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItem/PageSize); }
        }
    }

    public class IndexViewModel
    {
        public IEnumerable<PositionCatalogIndexView> Positions { get; set; }
        public PageInfo PageInfo { get; set; }
        public string KeyWord { get; set; }
    }

    public class MyPositionsViewModel
    {
        public IEnumerable<PositionCatalog> Positions { get; set; }
        public PageInfo PageInfo { get; set; }
        public string KeyWord { get; set; }
    }
}