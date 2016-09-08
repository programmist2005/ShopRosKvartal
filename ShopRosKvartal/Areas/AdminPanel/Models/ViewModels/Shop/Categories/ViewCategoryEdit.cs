using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Categories
{
    public class ViewCategoryEdit
    {
        public int Id { get; set; }
        //-----------------------------------------------------
        // выбор родительской категории
        public int? SelectedId { get; set; }

        [Display(Name = "Родительская категория")]
        public SelectList CategoriesList { get; set; }

        //-----------------------------------------------------

        [Display(Name = "Название категории")]
        [Required(ErrorMessage = "Введите название категории")]
        public string Name { get; set; }

        [Display(Name = "Алиас категории")]
        public string Alias { get; set; }

        [Display(Name = "Описание категории")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}