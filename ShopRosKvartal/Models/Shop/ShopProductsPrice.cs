using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop
{
    public class ShopProductsPrice
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime DateSet { get; set; }
        public bool CurrentPrice { get; set; }

        // навигационное свойство к "товары"
        public virtual ShopProduct ShopProduct { get; set; }

    }
}