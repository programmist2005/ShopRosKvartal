using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop
{
    public class ShopProductsBrand
    {
        // "брэнды"
        //
        // Связь с таблицей товаров. 1-брэнд -> М-товаров
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        // навигационное свойство к "товары"
        public ICollection<ShopProduct> ShopProducts { get; set; }

        public ShopProductsBrand()
        {
            ShopProducts = new List<ShopProduct>();
        }
    }
}