using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop
{
    public class ShopCategory
    {
        // "категории товаров"
        //
        // категории товаров могут быть вложенными
        // выборка родительской категории происходит по ParentId == null
        //
        // Одна "категория товаров" может иметь несколько товаров, 
        // один товар может быть в нескольких категориях. 
        // Связь моделей "категории товаров" и "товарами" - "М-М"

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }

        // навигационное свойство к "товары"
        public ICollection<ShopProduct> ShopProducts { get; set; }

        public ShopCategory()
        {
            ShopProducts = new List<ShopProduct>();
        }
    }
}