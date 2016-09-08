using ShopRosKvartal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ShopRosKvartal.Models.Shop.ShoppingCartModels;
using ShopRosKvartal.Models.Shop.Order;
using ShopRosKvartal.Models.Shop;

namespace ShopRosKvartal.Controllers
{
    //==========================================================
    //=============        Контроллер Заказ         ============
    //============== Доступен для авторизированных =============
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        // GET: Order
        public ActionResult Index()
        {
            string currentUserId = HttpContext.User.Identity.GetUserId();
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = userManager.FindById(currentUserId);
            if (!currentUser.EmailConfirmed)
            {
                return RedirectToAction("Index", "Profile");
            }

            ShoppingCart cart = db.ShoppingCarts
                .Include(c=>c.ShoppingCartsProducts)
                .Where(c => c.UserId == currentUser.Id).SingleOrDefault();
            if (cart != null && cart.ShoppingCartsProducts != null && cart.ShoppingCartsProducts.Count > 0) 
            {
                ShopOrder order = new ShopOrder
                {
                    DateCreate = DateTime.Now,
                    ApplicationUser = currentUser
                };

                foreach (ShoppingCartsProduct cartProduct in cart.ShoppingCartsProducts)
                {
                    ShopOrderProduct orderProduct = new ShopOrderProduct
                    {
                        Quantity = cartProduct.Quantity,
                        ShopProduct = cartProduct.ShopProduct
                    };

                    ShopProductsPrice price = db.ShopProductsPrices.Where(p => p.ShopProduct.Id == cartProduct.ShopProduct.Id && p.CurrentPrice).SingleOrDefault();
                        //cartProduct.ShopProduct.ShopProductsPrices.Where(p => p.CurrentPrice).SingleOrDefault();
                    if (price != null)
                    {
                        orderProduct.Price = price.Price;
                    }

                    order.ShopOrderProducts.Add(orderProduct);
                }

                db.ShopOrders.Add(order);
                db.ShoppingCartsProducts.RemoveRange(cart.ShoppingCartsProducts);
                db.SaveChanges();

                return View("Index");
            }
            return View("Error");
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
    }
}