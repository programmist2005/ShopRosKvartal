using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Orders
{
    public class ViewOrder
    {
        public int Id { get; set; }
        public DateTime DateCreation { get; set; }
        public decimal PriceTotal { get; set; }
        
        public string UserFullName { get; set; }
        public string Adress { get; set; }
        public string Passport { get; set; }

        public ICollection<ViewOrderProduct> Products { get; set; }
        

    }
}