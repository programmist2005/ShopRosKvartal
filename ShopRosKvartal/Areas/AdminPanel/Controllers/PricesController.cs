using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopRosKvartal.Models;
using ShopRosKvartal.Models.Shop;
using ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Prices;

namespace ShopRosKvartal.Areas.AdminPanel.Controllers
{
    //==========================================================
    // Контроллер "цены товаров"
    // Доступен для администраторов и модераторов

    [Authorize(Roles = "Администратор, Модератор")]
    public class PricesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Prices/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProduct product = db.ShopProducts.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewProductPriceCreate model = new ViewProductPriceCreate
            {
                Id = id.Value,
                URL = HttpContext.Request.UrlReferrer.ToString()
            };
            return View(model);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Prices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewProductPriceCreate model)
        {
            if (ModelState.IsValid)
            {
                ShopProduct product = db.ShopProducts.Include(p=>p.ShopProductsPrices).Where(p=>p.Id==model.Id).SingleOrDefault();
                ShopProductsPrice price = db.ShopProductsPrices.Where(p => p.ShopProduct.Id == product.Id && p.CurrentPrice).SingleOrDefault();
                if (price != null)
                {
                    price.CurrentPrice = false;
                    db.Entry(price).State = EntityState.Modified;
                    db.SaveChanges();
                }

                price = new ShopProductsPrice
                {
                    Price = decimal.Parse(model.Price),
                    CurrentPrice = true,
                    DateSet = DateTime.Now,
                    ShopProduct = product
                };
                db.ShopProductsPrices.Add(price);

                db.SaveChanges();
                return Redirect(model.URL);
                //return RedirectToAction("Details", "Products", new { id = product.Id });
            }

            return View(model);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Prices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProductsPrice price = db.ShopProductsPrices.Find(id);
            if (price == null)
            {
                return HttpNotFound();
            }

            ViewProductPriceEdit model = new ViewProductPriceEdit
            {
                Id = price.Id,
                Price = price.Price.ToString(),
                ProductId = price.ShopProduct.Id,
                CurrentPrice = price.CurrentPrice
            };
            return View(model);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Prices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewProductPriceEdit model)
        {
            if (ModelState.IsValid)
            {
                ShopProductsPrice price = db.ShopProductsPrices.Find(model.Id);
                if (model.CurrentPrice != price.CurrentPrice)
                {
                    price.CurrentPrice = model.CurrentPrice;
                    ShopProductsPrice ollCurrentPrice = db.ShopProductsPrices.Where(p => p.ShopProduct.Id == model.ProductId && p.CurrentPrice==model.CurrentPrice).SingleOrDefault();
                    if (ollCurrentPrice != null)
                    {
                        ollCurrentPrice.CurrentPrice = !model.CurrentPrice;
                        db.Entry(ollCurrentPrice).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                price.Price = decimal.Parse(model.Price);
                db.Entry(price).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Products", new { id = model.ProductId });
            }
            return View(model);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Prices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProductsPrice price = db.ShopProductsPrices.Find(id);
            if (price == null)
            {
                return HttpNotFound();
            }

            ViewProductPriceDelete model = new ViewProductPriceDelete
            {
                Id = price.Id,
                ProductId = price.ShopProduct.Id,
                Price = price.Price.ToString(),
                DateSet = price.DateSet,
                CurrentPrice = price.CurrentPrice
            };
            return View(model);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Prices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ViewProductPriceDelete model)
        {
            ShopProductsPrice price = db.ShopProductsPrices.Find(model.Id);
            if (price.CurrentPrice)
            {
                ShopProductsPrice currentPriceNew = db.ShopProductsPrices.Where(p => p.Id != price.Id).OrderByDescending(p => p.DateSet).FirstOrDefault();
                if (currentPriceNew != null)
                {
                    currentPriceNew.CurrentPrice = true;
                    db.Entry(currentPriceNew).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            RemovePrice(price);
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }
        //==========================================================

        //==========================================================
        public void RemovePrice(ShopProductsPrice price)
        {
            db.ShopProductsPrices.Remove(price);
            db.SaveChanges();
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
