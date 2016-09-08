using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.ViewModels.Shop.ShoppingCart
{
    public class ViewShoppingCartsProducts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string PhotoName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceOne { get; set; }
        public decimal PriceTotal { get; set; }
        public DateTime DateAdded { get; set; }

    }
}