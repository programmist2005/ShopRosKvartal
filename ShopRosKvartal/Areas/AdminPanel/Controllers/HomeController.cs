using ShopRosKvartal.Models;
using ShopRosKvartal.Models.Shop.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Orders;

namespace ShopRosKvartal.Areas.AdminPanel.Controllers
{
    [Authorize(Roles = "Администратор, Модератор")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================




        //==========================================================
        // вывод всех заказов
        public ActionResult Index()
        {
            List<ShopOrder> orders = db.ShopOrders
                .Include(o => o.ApplicationUser)
                .Include(o => o.ShopOrderProducts)
                .ToList();

            List<ViewOrderListItem> ordersViewList = new List<ViewOrderListItem>();

            foreach (ShopOrder order in orders)
            {
                ViewOrderListItem orderViewItem = new ViewOrderListItem
                {
                    Id = order.Id,
                    DateCreation = order.DateCreate,
                    UserFullName = order.ApplicationUser.UserProfile.Surname + " "
                        + order.ApplicationUser.UserProfile.Name + " " 
                        + order.ApplicationUser.UserProfile.Patronymic
                };

                foreach (ShopOrderProduct product in order.ShopOrderProducts)
                {
                    orderViewItem.TotalPrice = orderViewItem.TotalPrice + product.Price;
                }
                
                ordersViewList.Add(orderViewItem);
            }
            return View(ordersViewList.OrderByDescending(o=>o.DateCreation));
        }
        //==========================================================



        //==========================================================
        public ActionResult Details(int? id)
        {
            ShopOrder order = db.ShopOrders
                .Include(o => o.ApplicationUser)
                .Include(o => o.ShopOrderProducts)
                .Where(b => b.Id == id)
                .SingleOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewOrder orderView = new ViewOrder
            {
                Id = order.Id,
                DateCreation = order.DateCreate,
                UserFullName = order.ApplicationUser.UserProfile.Surname + " "
                        + order.ApplicationUser.UserProfile.Name + " "
                        + order.ApplicationUser.UserProfile.Patronymic,

                Passport = order.ApplicationUser.UserProfile.PassportNumberAndSeries 
                        + ", выдан " + order.ApplicationUser.UserProfile.PassportIssuingDate.ToShortDateString()
            };

            //адрес
            orderView.Adress = FromBDToAdressString(
                order.ApplicationUser.UserProfile.Country,
                order.ApplicationUser.UserProfile.City,
                order.ApplicationUser.UserProfile.ZipCode,
                order.ApplicationUser.UserProfile.Street,
                order.ApplicationUser.UserProfile.House,
                order.ApplicationUser.UserProfile.Apartment);
            
            orderView.Products = new List<ViewOrderProduct>();
            foreach (ShopOrderProduct product in order.ShopOrderProducts)
            {
                orderView.PriceTotal = orderView.PriceTotal + product.Price;

                ViewOrderProduct productView = new ViewOrderProduct
                {
                    Name = product.ShopProduct.Name,
                    Brand = product.ShopProduct.ShopProductsBrand.Name,
                    VendorCode = product.ShopProduct.VendorCode,
                    Quantity = product.Quantity,
                    PriceOne = product.Price,
                    PriceAll = product.Quantity * product.Price
                };

                orderView.Products.Add(productView);
            }

            return View(orderView);
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // адрес из Бд в строку
        private string FromBDToAdressString(string country, string city, string zipCode, string street, int house, int? apartment)
        {
            string adress = string.Format("{0}, г.{1}, {2}, ул.{3}, дом №{4}",
                country, city, zipCode, street, house);
            if (apartment != null)
            {
                adress = string.Format("{0}, кварира №{1}", adress, apartment);
            }
            return adress;
        }
        //==========================================================



        //==========================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //==========================================================
    }
}