using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Categories
{
    public class ViewCategoryIndex
    {
        public int Id { get; set; }

        [Display(Name = "Название категории")]
        public string Name { get; set; }

        [Display(Name = "Алиас категории")]
        public string Alias { get; set; }

        [Display(Name = "Родительская категория")]
        public string Parent { get; set; }
    }
}