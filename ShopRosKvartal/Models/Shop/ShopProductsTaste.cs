using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop
{
    public class ShopProductsTaste
    {
        // вкус
        // сквязь с товарами 1 к М
        public int Id { get; set; }

        [Display(Name = "Вкус")]
        [Required(ErrorMessage = "Введите вкус")]
        public string Name { get; set; }

        // навигационное свойство к "товары"
        public ICollection<ShopProduct> ShopProducts { get; set; }

        public ShopProductsTaste()
        {
            ShopProducts = new List<ShopProduct>();
        }
    }
}