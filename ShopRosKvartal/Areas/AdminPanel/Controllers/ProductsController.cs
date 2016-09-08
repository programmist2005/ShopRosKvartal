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
using ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Products;
using ShopRosKvartal.LibraryHelperClasses.Translit;
using ShopRosKvartal.LibraryHelperClasses.Images;

namespace ShopRosKvartal.Areas.AdminPanel.Controllers
{
    //==========================================================
    // Контроллер "товаров"
    // Доступен для администраторов и модераторов

    [Authorize(Roles = "Администратор, Модератор")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        // вывод всех товаров с доп данными
        public ActionResult Index()
        {
            // все товары
            List<ShopProduct> products = db.ShopProducts
                .Include(p=>p.ShopProductsBrand)
                .Include(p=>p.ShopCategories)
                .Include(p=>p.ShopProductsPrices)
                .ToList();

            // созадем список отображения 
            List<ViewProducts> models = new List<ViewProducts>();
            foreach (ShopProduct product in products)
            {
                ViewProducts model = new ViewProducts
                {
                    Id = product.Id,
                    Name = product.Name,
                    PhotoName = product.PhotoName,
                    
                };
                if (product.ShopProductsBrand != null)
                {
                    model.Brand = product.ShopProductsBrand.Name;
                }

                if (product.ShopProductsPrices.Count > 0)
                {
                    ShopProductsPrice price = product.ShopProductsPrices.Where(p => p.CurrentPrice).SingleOrDefault();
                    if (price != null)
                    {
                        model.Price = price.Price;
                    }
                }
                
                model.ShopCategories = new List<string>();
                foreach (ShopCategory category in product.ShopCategories)
                {
                    model.ShopCategories.Add(category.Name);
                }

                models.Add(model);
            }
            return View(models);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProduct product = db.ShopProducts
                .Include(p => p.ShopProductsBrand)
                .Include(p => p.ShopProductsPrices)
                .Include(p => p.ShopProductsTaste)
                .Include(p => p.ShopCategories)
                .Where(p => p.Id == id).SingleOrDefault();
            product.ShopProductsPrices = product.ShopProductsPrices.OrderByDescending(p=>p.DateSet).ToList();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Products/Create
        public ActionResult Create()
        {
            ViewProductCreate model = new ViewProductCreate();
            // все категории
            model.CategoriesAll = db.ShopCategories.OrderBy(c=>c.Name).ToList();

            // список вкусов
            List<ShopProductsTaste> tastes = db.ShopProductsTastes.OrderBy(c => c.Name).ToList();
            model.TasteList = new SelectList(tastes, "Id", "Name");

            // список брэндов
            List<ShopProductsBrand> brands = db.ShopProductsBrands.OrderBy(c => c.Name).ToList();
            model.BrandList = new SelectList(brands, "Id", "Name");

            return View(model);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewProductCreate model)
        {
            for (int i = 0; i < model.SelectedCategoriesId.Length; i++)
            {
                ShopCategory category = db.ShopCategories.Find(model.SelectedCategoriesId[i]);
                model.CategoriesSelected.Add(category);
            }

            if (ModelState.IsValid)
            {
                ShopProduct product = new ShopProduct
                {
                    Name = model.Name,
                    Alias = Translit.TranslitString(model.Name),
                    Description = model.Description,
                    VendorCode = model.VendorCode,
                    Weight = model.Weight.Value,
                    Proteins = model.Proteins.Value,
                    Fats = model.Fats.Value,
                    Carbohydrates = model.Carbohydrates.Value,
                    Kcal=model.Kcal.Value
                };

                //брэнд
                if (model.SelectedBrandId.HasValue)
                {
                    product.ShopProductsBrand = db.ShopProductsBrands.Find(model.SelectedBrandId);
                }

                // вкус
                if (model.SelectedTasteId.HasValue)
                {
                    product.ShopProductsTaste = db.ShopProductsTastes.Find(model.SelectedTasteId);
                }

                // катгории
                product.ShopCategories = model.CategoriesSelected;

                if (model.PortionsWeight.HasValue)
                {
                    product.PortionsWeight = model.PortionsWeight.Value;
                }

                if (model.PortionsCount.HasValue)
                {
                    product.PortionsCount = model.PortionsCount.Value;
                }

                product.DateCreation = DateTime.Now;

                if (model.Price!=null)
                {
                    ShopProductsPrice price = new ShopProductsPrice
                    {
                        Price = decimal.Parse(model.Price),
                        CurrentPrice = true,
                        DateSet = product.DateCreation,
                    };
                    product.ShopProductsPrices.Add(price);
                }

                //сохранение фото товра
                if (model.PhotoFile != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string dirPath = HttpContext.Server.MapPath("~/Content/Images/Shop/Products");
                    product.PhotoName = Image.Save(
                        model.PhotoFile,
                        dirPath,
                        null,
                        fileName);
                }

                db.ShopProducts.Add(product);
                db.SaveChanges();

                
                return RedirectToAction("Index");
            }

            // все категории
            model.CategoriesAll = db.ShopCategories.OrderBy(c=>c.Name).ToList();

            // список вкусов
            List<ShopProductsTaste> tastes = db.ShopProductsTastes.OrderBy(c => c.Name).ToList();
            model.TasteList = new SelectList(tastes, "Id", "Name");

            // список брэндов
            List<ShopProductsBrand> brands = db.ShopProductsBrands.OrderBy(c => c.Name).ToList();
            model.BrandList = new SelectList(brands, "Id", "Name");

            return View(model);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProduct product = db.ShopProducts
                .Include(p => p.ShopProductsBrand)
                .Include(p => p.ShopProductsTaste)
                .Include(p => p.ShopCategories)
                .Where(p => p.Id == id).SingleOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewProductEdit model = new ViewProductEdit
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                VendorCode = product.VendorCode,
                Weight = product.Weight,
                Proteins = product.Proteins,
                Fats = product.Fats,
                Carbohydrates = product.Carbohydrates,
                Kcal = product.Kcal,
                PortionsWeight = product.PortionsWeight,
                PortionsCount = product.PortionsCount,
                PhotoName = product.PhotoName
            };

            if (product.ShopProductsTaste!=null)
            {
                model.SelectedTasteId = product.ShopProductsTaste.Id;
            }
            if  (product.ShopProductsBrand!=null)
            {
                model.SelectedBrandId = product.ShopProductsBrand.Id;
            }
            if (product.ShopCategories!=null)
            {
                model.CategoriesSelected = product.ShopCategories;
            }
                

            // все категории
            model.CategoriesAll = db.ShopCategories.OrderBy(c => c.Name).ToList();

            // список вкусов
            List<ShopProductsTaste> tastes = db.ShopProductsTastes.OrderBy(c => c.Name).ToList();
            model.TasteList = new SelectList(tastes, "Id", "Name");

            // список брэндов
            List<ShopProductsBrand> brands = db.ShopProductsBrands.OrderBy(c => c.Name).ToList();
            model.BrandList = new SelectList(brands, "Id", "Name");

            return View(model);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewProductEdit model)
        {
            if (model.SelectedCategoriesId != null)
            {
                for (int i = 0; i < model.SelectedCategoriesId.Length; i++)
                {
                    ShopCategory category = db.ShopCategories.Find(model.SelectedCategoriesId[i]);
                    model.CategoriesSelected.Add(category);
                }
            }

            if (ModelState.IsValid)
            {
                ShopProduct product = db.ShopProducts.Include(p=>p.ShopCategories).Include(p=>p.ShopProductsBrand).Include(p=>p.ShopProductsTaste).Where(p=>p.Id==model.Id).SingleOrDefault();

                product.Name = model.Name;
                product.Alias = Translit.TranslitString(model.Name);
                product.Description = model.Description;
                product.VendorCode = model.VendorCode;
                product.Weight = model.Weight.Value;
                product.Proteins = model.Proteins.Value;
                product.Fats = model.Fats.Value ;
                product.Carbohydrates = model.Carbohydrates.Value;
                product.Kcal = model.Kcal.Value;

                //брэнд
                if (model.SelectedBrandId.HasValue)
                {
                    product.ShopProductsBrand = db.ShopProductsBrands.Find(model.SelectedBrandId);
                }
                else
                {
                    product.ShopProductsBrand = null;
                }

                // вкус
                if (model.SelectedTasteId.HasValue)
                {
                    product.ShopProductsTaste = db.ShopProductsTastes.Find(model.SelectedTasteId);
                }
                else
                {
                    product.ShopProductsTaste = null;
                }

                // катгории
                product.ShopCategories = model.CategoriesSelected;

                if (model.PortionsWeight.HasValue)
                {
                    product.PortionsWeight = model.PortionsWeight.Value;
                }
                else
                {
                    product.PortionsWeight = 0;
                }

                if (model.PortionsCount.HasValue)
                {
                    product.PortionsCount = model.PortionsCount.Value;
                }
                else
                {
                    product.PortionsCount = 0;
                }

                product.DateCreation = DateTime.Now;

                //сохранение фото товра
                if (model.PhotoFile != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string dirPath = HttpContext.Server.MapPath("~/Content/Images/Shop/Products");
                    product.PhotoName = Image.Save(
                        model.PhotoFile,
                        dirPath,
                        null,
                        fileName);
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // все категории
            model.CategoriesAll = db.ShopCategories.OrderBy(c => c.Name).ToList();

            // список вкусов
            List<ShopProductsTaste> tastes = db.ShopProductsTastes.OrderBy(c => c.Name).ToList();
            model.TasteList = new SelectList(tastes, "Id", "Name");

            // список брэндов
            List<ShopProductsBrand> brands = db.ShopProductsBrands.OrderBy(c => c.Name).ToList();
            model.BrandList = new SelectList(brands, "Id", "Name");
            return View(model);
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopProduct product = db.ShopProducts
                .Include(p => p.ShopProductsBrand)
                .Include(p => p.ShopProductsPrices)
                .Include(p => p.ShopProductsTaste)
                .Include(p => p.ShopCategories)
                .Where(p => p.Id == id).SingleOrDefault();
            product.ShopProductsPrices = product.ShopProductsPrices.OrderByDescending(p => p.DateSet).ToList();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        //==========================================================
        
        //==========================================================
        // POST: AdminPanel/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ShopProduct model)
        {

            ShopProduct product = db.ShopProducts
                .Include(p => p.ShopProductsPrices)
                .Where(p => p.Id == model.Id).SingleOrDefault();

            RemoveProduct(product);
            return RedirectToAction("Index");
        }
        //==========================================================

        //==========================================================
        public void RemoveProduct(ShopProduct product)
        {
            db.ShopProductsPrices.RemoveRange(product.ShopProductsPrices);
            db.SaveChanges();

            string dirPath = HttpContext.Server.MapPath("~/Content/Images/Shop/Products");
            product.PhotoName = Image.Delete(dirPath, product.PhotoName);

            db.ShopProducts.Remove(product);
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
