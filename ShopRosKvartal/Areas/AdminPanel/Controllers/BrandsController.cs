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
using ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Brands;
using ShopRosKvartal.LibraryHelperClasses.Translit;

namespace ShopRosKvartal.Areas.AdminPanel.Controllers
{
    //==========================================================
    // Контроллер "брэнды товаров"
    // Доступен для администраторов и модераторов

    [Authorize(Roles = "Администратор, Модератор")]

    public class BrandsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        public ActionResult Index()
        {
            return View(db.ShopProductsBrands.ToList());
        }
        //==========================================================



        //==========================================================
        public ActionResult Details(int? id)
        {
            ShopProductsBrand brand = db.ShopProductsBrands
                .Include(b => b.ShopProducts)
                .Where(b => b.Id == id)
                .SingleOrDefault();
            if (brand == null)
            {
                return HttpNotFound();
            }

            ViewBrandDetails model = new ViewBrandDetails
            {
                Name = brand.Name,
                Products = brand.ShopProducts,
                ProductsCount = brand.ShopProducts.Count,
            };
            return View(model);
        }
        //==========================================================



        //==========================================================
        public ActionResult Create()
        {
            return View();
        }
        //==========================================================
        
        //==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewBrandCreate model)
        {
            if (ModelState.IsValid)
            {
                ShopProductsBrand brand = new ShopProductsBrand
                {
                    Name = model.Name,
                    Alias = Translit.TranslitString(model.Name)
                };
                db.ShopProductsBrands.Add(brand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        //==========================================================



        //==========================================================
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProductsBrand brand = db.ShopProductsBrands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }

            ViewBrandEdit model = new ViewBrandEdit { Id = brand.Id, Name = brand.Name };
            return View(model);
        }
        //==========================================================
        
        //==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewBrandEdit model)
        {
            if (ModelState.IsValid)
            {
                ShopProductsBrand brand = db.ShopProductsBrands.Find(model.Id);
                brand.Name = model.Name;
                brand.Alias = Translit.TranslitString(model.Name);
                db.Entry(brand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        //==========================================================



        //==========================================================
        public ActionResult Delete(int? id)
        {
            ShopProductsBrand brand = db.ShopProductsBrands
                .Include(b => b.ShopProducts)
                .Where(b => b.Id == id)
                .SingleOrDefault();
            if (brand == null)
            {
                return HttpNotFound();
            }

            ViewBrandDelete model = new ViewBrandDelete
            {
                Id = brand.Id,
                Name = brand.Name,
                Products = brand.ShopProducts,
                ProductsCount = brand.ShopProducts.Count,
                DeleteAll = false
            };
            return View(model);
        }
        //==========================================================
        
        //==========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ViewBrandDelete model)
        {
            ShopProductsBrand brand = db.ShopProductsBrands
               .Include(b => b.ShopProducts)
               .Where(b => b.Id == model.Id)
               .SingleOrDefault();

            if (model.DeleteAll)
            {
                if (brand.ShopProducts != null)
                {
                    foreach (ShopProduct product in brand.ShopProducts)
                    {
                        List<ShopProductsPrice> removePrices = db.ShopProductsPrices.Where(p => p.ShopProduct.Id == product.Id).ToList();
                        db.ShopProductsPrices.RemoveRange(removePrices);
                        db.SaveChanges();

                        string dirPath = HttpContext.Server.MapPath("~/Content/Images/Shop/Products");
                        product.PhotoName = Image.Delete(dirPath, product.PhotoName);
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    db.ShopProducts.RemoveRange(brand.ShopProducts);
                    db.SaveChanges();
                }
            }

            db.ShopProductsBrands.Remove(brand);
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