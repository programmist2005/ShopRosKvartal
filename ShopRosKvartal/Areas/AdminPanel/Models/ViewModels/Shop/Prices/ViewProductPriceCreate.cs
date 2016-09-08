using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Prices
{
    public class ViewProductPriceCreate
    {
        public int Id { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Укажите цену")]
        public string Price { get; set; }

        public string URL { get; set; }

    }
}