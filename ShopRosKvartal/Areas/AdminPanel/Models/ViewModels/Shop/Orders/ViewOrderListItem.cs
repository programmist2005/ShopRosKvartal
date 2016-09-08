using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Orders
{
    public class ViewOrderListItem
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateCreation { get; set; }
    }
}