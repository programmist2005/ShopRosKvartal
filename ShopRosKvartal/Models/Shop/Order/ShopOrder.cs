using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop.Order
{
    // "заказ"
    public class ShopOrder
    {
        public int Id { get; set; }

        public DateTime DateCreate { get; set; }

        // навигационное свойство к "user"
        public virtual ApplicationUser ApplicationUser { get; set; }

        // навигационное свойство к "товар заказа"
        public ICollection<ShopOrderProduct> ShopOrderProducts { get; set; }

        public ShopOrder()
        {
            ShopOrderProducts = new List<ShopOrderProduct>();
        }
    }
}