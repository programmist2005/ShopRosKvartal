using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.ViewModels.Shop.Product
{
    public class ViewProductCatalog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string PhotoName { get; set; }
        public string Taste { get; set; }
        public decimal Price { get; set; }
        public DateTime  DateCreation { get; set; }
        public int Quantity { get; set; }

    }
}