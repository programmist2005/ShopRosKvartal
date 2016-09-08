using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.ViewModels.Shop.Filters
{
    public class ShopFilterResult
    {
        public int? CategorySelectedId { get; set; }
        public int? BrandSelectedId { get; set; }
        public int? TasteSelectedId { get; set; }
        public int? SortingSelectedId { get; set; }
        public string PriceFrom { get; set; }
        public string PriceTo { get; set; }
    }
}