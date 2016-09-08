using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.ViewModels.Shop.ShoppingCart
{
    public class ViewShoppingCart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CookieKey { get; set; }
        public ICollection<ViewShoppingCartsProducts> Products { get; set; }

        public int CountProducts { get; set; }
    }
}