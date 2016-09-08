using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Orders
{
    public class ViewOrderProduct
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string VendorCode { get; set; }
        public int Quantity { get; set; }
        public decimal PriceOne { get; set; }
        public decimal PriceAll { get; set; }
    }
}