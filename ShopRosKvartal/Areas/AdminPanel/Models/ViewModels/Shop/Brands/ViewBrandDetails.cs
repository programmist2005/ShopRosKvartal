using ShopRosKvartal.Models.Shop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Brands
{
    public class ViewBrandDetails
    {
        [Display(Name = "Производитель")]
        public string Name { get; set; }

        [Display(Name = "Всего товаров")]
        public int ProductsCount { get; set; }

        // навигационное свойство к "товары"
        [Display(Name = "Все товары")]
        public ICollection<ShopProduct> Products { get; set; }

        public ViewBrandDetails()
        {
            Products = new List<ShopProduct>();
        }
    }
}