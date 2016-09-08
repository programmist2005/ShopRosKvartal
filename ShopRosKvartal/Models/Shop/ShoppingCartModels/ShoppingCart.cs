using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop.ShoppingCartModels
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }

        public string CookieKey { get; set; }

        // навигационное свойство к "товарам корзины"
        public ICollection<ShoppingCartsProduct> ShoppingCartsProducts { get; set; }

        public ShoppingCart()
        {
            ShoppingCartsProducts = new List<ShoppingCartsProduct>();
        }
    }
}