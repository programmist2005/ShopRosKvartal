using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop.Order
{
    // "товар заказа"
    public class ShopOrderProduct
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        // навигационное свойство к "заказ"
        public virtual ShopOrder ShopOrder { get; set; }

        // навигационное свойство к "товары"
        public virtual ShopProduct ShopProduct { get; set; }
    }
}