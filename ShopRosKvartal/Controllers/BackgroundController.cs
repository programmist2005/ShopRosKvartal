using ShopRosKvartal.Models;
using ShopRosKvartal.Models.Shop;
using ShopRosKvartal.Models.Shop.ShoppingCartModels;
using ShopRosKvartal.Models.ViewModels.Shop.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopRosKvartal.Controllers
{
    public class BackgroundController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        



        //==========================================================
        public ActionResult CleanCookie()
        {
            DeleteCookie();
            return RedirectToAction("Index", "Home", null);
        }
        //==========================================================

        private void DeleteCookie()
        {
            if (HttpContext.Request.Cookies["ShoppingCartKeyId"] != null)
            {
                HttpContext.Response.Cookies["ShoppingCartKeyId"].Expires = DateTime.Now.AddDays(-1);
            }
        }
    }
}