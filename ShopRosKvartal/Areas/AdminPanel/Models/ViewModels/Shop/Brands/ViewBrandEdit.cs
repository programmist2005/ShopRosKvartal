using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Brands
{
    public class ViewBrandEdit
    {
        public int Id { get; set; }

        [Display(Name = "Производитель")]
        [Required(ErrorMessage = "Введите название производителя")]
        public string Name { get; set; }
    }
}