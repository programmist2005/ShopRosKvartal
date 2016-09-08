using ShopRosKvartal.Models.Shop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Brands
{
    public class ViewBrandDelete
    {
        public int Id { get; set; }

        [Display(Name = "Производитель")]
        public string Name { get; set; }

        [Display(Name = "Всего товаров")]
        public int ProductsCount { get; set; }

        [Display(Name = "Удалить производителя и все его товары")]
        public bool DeleteAll { get; set; }

        // навигационное свойство к "товары"
        [Display(Name = "Все товары")]
        public ICollection<ShopProduct> Products { get; set; }

        public ViewBrandDelete()
        {
            Products = new List<ShopProduct>();
        }
    }
}