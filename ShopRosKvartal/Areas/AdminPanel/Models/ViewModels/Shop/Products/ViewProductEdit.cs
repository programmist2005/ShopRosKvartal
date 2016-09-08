using ShopRosKvartal.Models.Shop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Products
{
    public class ViewProductEdit
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Введите название")]
        public string Name { get; set; }        // название

        [Display(Name = "Описание товара")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } // описание

        [Display(Name = "Артикул")]
        [Required(ErrorMessage = "Введите артикул")]
        public string VendorCode { get; set; }  // артикул

        [Display(Name = "Вес упаковки")]
        [Required(ErrorMessage = "Введите вес упаковки")]
        [UIHint("string")]
        public int? Weight { get; set; }         // вес

        //-----------------------------------------------------
        // БЖУ
        [Display(Name = "Белки")]
        [Required(ErrorMessage = "Введите соотношение белков")]
        public double? Proteins { get; set; }

        [Display(Name = "Жиры")]
        [Required(ErrorMessage = "Введите соотношение жиров")]
        public double? Fats { get; set; }

        [Display(Name = "Углеводы")]
        [Required(ErrorMessage = "Введите соотношение углеводов")]
        public double? Carbohydrates { get; set; }
        //-----------------------------------------------------

        //-----------------------------------------------------
        // Ккал
        [Display(Name = "Энергетическая ценность")]
        [Required(ErrorMessage = "Укажите количество ккал")]
        [UIHint("string")]
        public int? Kcal { get; set; }
        //----------------------------------------------------

        //----------------------------------------------------
        // вес порции
        [Display(Name = "Вес порции")]
        [UIHint("string")]
        public int? PortionsWeight { get; set; }

        // количество порций
        [Display(Name = "Количество порций")]
        [UIHint("string")]
        public int? PortionsCount { get; set; }
        //----------------------------------------------------

        //----------------------------------------------------
        // название фото
        [Display(Name = "Фото")]
        public string PhotoName { get; set; }
        public HttpPostedFileBase PhotoFile { get; set; }
        //----------------------------------------------------

        //-----------------------------------------------------
        // "вкус"
        public int? SelectedTasteId { get; set; }

        [Display(Name = "Вкус")]
        public SelectList TasteList { get; set; }
        //-----------------------------------------------------

        //-----------------------------------------------------
        // "брэнды"
        public int? SelectedBrandId { get; set; }

        [Display(Name = "Производитель")]
        public SelectList BrandList { get; set; }
        //-----------------------------------------------------


        //-----------------------------------------------------
        public int[] SelectedCategoriesId { get; set; }

        [Display(Name = "Категории")]
        public ICollection<ShopCategory> CategoriesAll { get; set; }
        public ICollection<ShopCategory> CategoriesSelected { get; set; }

        public ViewProductEdit()
        {
            CategoriesAll = new List<ShopCategory>();
            CategoriesSelected = new List<ShopCategory>();
        }
    }
}