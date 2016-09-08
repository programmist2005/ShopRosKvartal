using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Products
{
    public class ViewProducts
    {
        public int Id { get; set; }
        public string Name { get; set; }        // название
        public string PhotoName { get; set; }   // название фото
        public string Brand { get; set; }       // брэнд
        public ICollection<string> ShopCategories { get; set; } // категории
        public decimal Price { get; set; }      // цена

        public ViewProducts()
        {
            ShopCategories = new List<string>();
        }
    }
}