using ShopRosKvartal.Models.Shop.Order;
using ShopRosKvartal.Models.Shop.ShoppingCartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.Shop
{
    public class ShopProduct
    {
        // "товары"
        //
        // Один товар может быть в нескольких категориях. 
        // Одна "категория товаров" может иметь несколько товаров, 
        // Связь моделей "товарами" и "категории товаров" - "М-М"
        //
        // Связь с таблицей брэндов. М-товаров -> 1-брэнд

        public int Id { get; set; }
        public string Name { get; set; }        // название
        public string Alias { get; set; }       // название лаитиницей для url
        public string Description { get; set; } // описание
        public string VendorCode { get; set; }  // артикул
        public int Weight { get; set; }         // вес

        // БЖУ
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }

        // Ккал
        public int Kcal { get; set; }

        // вес порции
        public int PortionsWeight { get; set; }

        // количество порций
        public int PortionsCount { get; set; }

        // дата добавления
        public DateTime DateCreation { get; set; }

        // название фото
        public string PhotoName { get; set; }

        // навигационное свойство к "вкус"
        public virtual ShopProductsTaste ShopProductsTaste { get; set; }

        // навигационное свойство к "брэнды"
        public virtual ShopProductsBrand ShopProductsBrand { get; set; }

        // навигационное свойство к "товары"
        public ICollection<ShopCategory> ShopCategories { get; set; }

        // навигационное свойство к "цены"
        public ICollection<ShopProductsPrice> ShopProductsPrices { get; set; }

        // навигационное свойство к "товарам корзины"
        public ICollection<ShoppingCartsProduct> ShoppingCartsProducts { get; set; }

        // навигационное свойство к "товарам заказ"
        public ICollection<ShopOrderProduct> ShopOrderProducts { get; set; }

        public ShopProduct()
        {
            ShopCategories = new List<ShopCategory>();
            ShopProductsPrices = new List<ShopProductsPrice>();
            ShoppingCartsProducts = new List<ShoppingCartsProduct>();
            ShopOrderProducts = new List<ShopOrderProduct>();
        }
    }
}