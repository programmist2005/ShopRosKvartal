using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop.ShoppingCartModels
{
    public class ShoppingCartsProduct
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        // навигационное свойство к "товары"
        public virtual ShoppingCart ShoppingCart { get; set; }

        // навигационное свойство к "товары"
        public virtual ShopProduct ShopProduct { get; set; }

        public DateTime DateAdded { get; set; }

    }
}