using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopRosKvartal.Models.ViewModels.Shop.Filters
{
    public class ShopFilter
    {
        //----------------------------------------------------
        // категории
        public int? CategorySelectedId { get; set; }

        [Display(Name = "Категория")]
        public SelectList CategoriesList { get; set; }
        //----------------------------------------------------

        //----------------------------------------------------
        // брэнды
        public int? BrandSelectedId { get; set; }

        [Display(Name = "Производитель")]
        public SelectList BrandList { get; set; }
        //----------------------------------------------------

        //----------------------------------------------------
        // вкус
        public int? TasteSelectedId { get; set; }

        [Display(Name = "Вкус")]
        public SelectList TasteList { get; set; }
        //----------------------------------------------------

        //----------------------------------------------------
        // сортировка
        public int? SortingSelectedId { get; set; }

        [Display(Name = "Сортировка")]
        public SelectList SortingList { get; set; }
        //----------------------------------------------------

        //----------------------------------------------------
        // цена
        [Display(Name = "Цена от")]
        public string PriceFrom { get; set; }

        [Display(Name = "Цена до")]
        public string PriceTo { get; set; }
        //----------------------------------------------------

    }
}