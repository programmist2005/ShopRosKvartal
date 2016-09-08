using Microsoft.AspNet.Identity.EntityFramework;
using ShopRosKvartal.Models.Shop;
using ShopRosKvartal.Models.Shop.Order;
using ShopRosKvartal.Models.Shop.ShoppingCartModels;
using ShopRosKvartal.Models.SiteTools;
using ShopRosKvartal.Models.UserData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //======================================================
        //-----------------------------------------------------
        // НАСТРОЙКА САЙТА
        // Настройки почтового сервиса
        public DbSet<ToolsSMTPSetting> ToolsSMTPSettings { get; set; }
        //-----------------------------------------------------

        //======================================================
        // НАСТРОЙКА МАГАЗИНА
        // Категории товаров
        public DbSet<ShopCategory> ShopCategories { get; set; }
        // товары
        public DbSet<ShopProduct> ShopProducts { get; set; }
        // брэнды
        public DbSet<ShopProductsBrand> ShopProductsBrands { get; set; }
        // вкус
        public DbSet<ShopProductsTaste> ShopProductsTastes { get; set; }
        // цены
        public DbSet<ShopProductsPrice> ShopProductsPrices { get; set; }

        //======================================================
        // ИНФОРМАЦИЯ О ПОЛЬЗОВАТЕЛЕ
        // Пол человека
        public DbSet<UserGender> UserGenders { get; set; }
        //-----------------------------------------------------

        //======================================================
        // КОРЗИНА ПОКУПОК
        // Корзина
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        // Товары корзины
        public DbSet<ShoppingCartsProduct> ShoppingCartsProducts { get; set; }
        //-----------------------------------------------------

        //======================================================
        // ОФОРМЛЕНИЕ ЗАКАЗ
        // заказ
        public DbSet<ShopOrder> ShopOrders { get; set; }

        // Товары заказа
        public DbSet<ShopOrderProduct> ShopOrderProducts { get; set; }
        //-----------------------------------------------------
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}