using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Prices
{
    public class ViewProductPriceEdit
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Укажите цену")]
        public string Price { get; set; }

        public bool CurrentPrice { get; set; }
    }
}