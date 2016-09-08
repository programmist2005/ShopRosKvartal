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
using ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Tastes;
using ShopRosKvartal.LibraryHelperClasses.Images;

namespace ShopRosKvartal.Areas.AdminPanel.Controllers
{
    //==========================================================
    // Контроллер "вкусы товаров"
    // Доступен для администраторов и модераторов

    [Authorize(Roles = "Администратор, Модератор")]
    public class TastesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Tastes
        public ActionResult Index()
        {
            return View(db.ShopProductsTastes.ToList());
        }
        //==========================================================
        


        //==========================================================
        // GET: AdminPanel/Tastes/Create
        public ActionResult Create()
        {
            return View();
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Tastes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopProductsTaste shopProductsTaste)
        {
            if (ModelState.IsValid)
            {
                db.ShopProductsTastes.Add(shopProductsTaste);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shopProductsTaste);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Tastes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProductsTaste shopProductsTaste = db.ShopProductsTastes.Find(id);
            if (shopProductsTaste == null)
            {
                return HttpNotFound();
            }
            return View(shopProductsTaste);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Tastes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShopProductsTaste shopProductsTaste)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shopProductsTaste).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shopProductsTaste);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Tastes/Delete/5
        public ActionResult Delete(int? id)
        {
            ShopProductsTaste taste = db.ShopProductsTastes
                .Include(b => b.ShopProducts)
                .Where(b => b.Id == id)
                .SingleOrDefault();
            if (taste == null)
            {
                return HttpNotFound();
            }

            ViewTasteDelete model = new ViewTasteDelete
            {
                Id = taste.Id,
                Name = taste.Name,
                Products = taste.ShopProducts,
                ProductsCount = taste.ShopProducts.Count,
                DeleteAll = false
            };
            return View(model);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Tastes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ViewTasteDelete model)
        {
            ShopProductsTaste taste = db.ShopProductsTastes
               .Include(b => b.ShopProducts)
               .Where(b => b.Id == model.Id)
               .SingleOrDefault();

            if (model.DeleteAll)
            {
                if (taste.ShopProducts != null)
                {
                    foreach (ShopProduct product in taste.ShopProducts)
                    {
                        List<ShopProductsPrice> removePrices = db.ShopProductsPrices.Where(p => p.ShopProduct.Id == product.Id).ToList();
                        db.ShopProductsPrices.RemoveRange(removePrices);
                        db.SaveChanges();

                        string dirPath = HttpContext.Server.MapPath("~/Content/Images/Shop/Products");
                        product.PhotoName = Image.Delete(dirPath, product.PhotoName);
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    db.ShopProducts.RemoveRange(taste.ShopProducts);
                    db.SaveChanges();
                }
            }

            db.ShopProductsTastes.Remove(taste);
            db.SaveChanges();
            return RedirectToAction("Index");
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
