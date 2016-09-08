using ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Categories;
using ShopRosKvartal.Models;
using ShopRosKvartal.Models.Shop;
using ShopRosKvartal.Models.Shop.ShoppingCartModels;
using ShopRosKvartal.Models.ViewModels.Shop.Filters;
using ShopRosKvartal.Models.ViewModels.Shop.Product;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShopRosKvartal.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        public ActionResult Index()
        {
            ShopFilter filter = new ShopFilter();
            // список вкусов
            List<ShopProductsTaste> tastes = db.ShopProductsTastes.OrderBy(c => c.Name).ToList();
            filter.TasteList = new SelectList(tastes, "Id", "Name");

            // список брэндов
            List<ShopProductsBrand> brands = db.ShopProductsBrands.OrderBy(c => c.Name).ToList();
            filter.BrandList = new SelectList(brands, "Id", "Name");

            // категории
            List<ShopCategory> parentCategories = db.ShopCategories.Where(p => p.ParentId == null).OrderBy(p => p.Name).ToList();
            List<ViewCategoryIndex> hierarchy = new List<ViewCategoryIndex>();
            foreach (ShopCategory category in parentCategories)
            {
                ViewCategoryIndex categoryToList = new ViewCategoryIndex
                {
                    Id = category.Id,
                    Name = category.Name,
                    Alias = category.Alias
                };
                hierarchy.Add(categoryToList);
                hierarchy.AddRange(FindChildCategories(category, ""));
            }
            filter.CategoriesList = new SelectList(hierarchy, "Id", "Name");

            // сортировка
            List<ShopSorting> sortingList = new List<ShopSorting>();
            ShopSorting sort = new ShopSorting { Id = 1, Name = "Возростание цены" };
            sortingList.Add(sort);
            sort = new ShopSorting { Id = 2, Name = "Убывание цены" };
            sortingList.Add(sort);
            sort = new ShopSorting { Id = 3, Name = "По алфавиту А-Я" };
            sortingList.Add(sort);
            sort = new ShopSorting { Id = 4, Name = "По алфавиту Я-А" };
            sortingList.Add(sort);
            sort = new ShopSorting { Id = 5, Name = "Новые вначале" };
            sortingList.Add(sort);
            sort = new ShopSorting { Id = 6, Name = "Новые в конце" };
            sortingList.Add(sort);
            filter.SortingList = new SelectList(sortingList, "Id", "Name");

            return View(filter);
        }
        //==========================================================



        //==========================================================
        // вспомогательный метод
        // поиск дочерних категорий и сохранение их в лист для отображения в Index
        private List<ViewCategoryIndex> FindChildCategories(ShopCategory category, string childtab)
        {
            childtab = childtab + "-";
            List<ShopCategory> childCategories = db.ShopCategories
                .Where(c => c.ParentId == category.Id).OrderBy(c => c.Name).ToList();
            List<ViewCategoryIndex> childList = new List<ViewCategoryIndex>();
            foreach (ShopCategory childCategory in childCategories)
            {
                ViewCategoryIndex categoryToLost = new ViewCategoryIndex
                {
                    Id = childCategory.Id,
                    Name = childtab + " " + childCategory.Name,
                    Alias = childCategory.Alias,
                    Parent = category.Name
                };
                childList.Add(categoryToLost);
                childList.AddRange(FindChildCategories(childCategory, childtab));
            }
            return childList;
        }
        //==========================================================



        //==========================================================
        // вспомогательный метод
        // перегрузка метода
        private List<ShopCategory> FindChildCategories(ShopCategory category)
        {
            List<ShopCategory> childCategories = db.ShopCategories
                .Where(c => c.ParentId == category.Id).OrderBy(c => c.Name).ToList();

            List<ShopCategory> childList = new List<ShopCategory>();
            childList.Add(category);

            foreach (ShopCategory childCategory in childCategories)
            {
                childList.AddRange(FindChildCategories(childCategory));
            }
            return childList;
        }
        //==========================================================



        //==========================================================
        public ActionResult _ProductsList(ShopFilterResult filterResult)
        {
            List<ShopProduct> products = new List<ShopProduct>();

            // категории
            if (filterResult.CategorySelectedId != null)
            {
                // поиск категории и дочерних категорий
                ShopCategory category = db.ShopCategories.Find(filterResult.CategorySelectedId);
                List<ShopCategory> categories = new List<ShopCategory>();
                categories.AddRange(FindChildCategories(category));

                foreach (ShopCategory item in categories)
                {
                    List<ShopProduct> productsByCategory = db.ShopProducts
                        .Include(p => p.ShopProductsBrand)
                        .Include(p => p.ShopCategories)
                        .Include(p => p.ShopProductsPrices)
                        .ToList();

                    productsByCategory = productsByCategory.Where(p => p.ShopCategories.Contains(item)).ToList();
                    products.AddRange(productsByCategory);
                }
                
            }
            else
            {
                products = db.ShopProducts
                .Include(p => p.ShopProductsBrand)
                .Include(p => p.ShopCategories)
                .Include(p => p.ShopProductsPrices)
                .ToList();
            }

            // брэнды
            if (filterResult.BrandSelectedId != null)
            {
                products = products.Where(p => p.ShopProductsBrand != null && p.ShopProductsBrand.Id == filterResult.BrandSelectedId).ToList();
            }
            // вкусы
            if (filterResult.TasteSelectedId != null)
            {
                products = products.Where(p => p.ShopProductsTaste != null && p.ShopProductsTaste.Id == filterResult.TasteSelectedId).ToList();
            }

            List<ViewProductCatalog> models = new List<ViewProductCatalog>();

            foreach (ShopProduct prod in products)
            {
                ViewProductCatalog model = new ViewProductCatalog
                {
                    Id = prod.Id,
                    Alias = prod.Alias,
                    Name = prod.Name,
                    PhotoName = prod.PhotoName,
                    DateCreation = prod.DateCreation,
                    Quantity = 1
                };

                ShopProductsPrice price = db.ShopProductsPrices.Where(p => p.ShopProduct.Id == prod.Id && p.CurrentPrice).SingleOrDefault();
                if (price != null)
                {
                    model.Price = price.Price;
                }

                if (prod.ShopProductsTaste!=null)
                {
                    model.Taste = prod.ShopProductsTaste.Name;
                }
                models.Add(model);
            }

            // сортировка
            if (filterResult.SortingSelectedId != null)
            {
                switch (filterResult.SortingSelectedId)
                {
                    case 1:
                        models = models.OrderBy(m => m.Price).ToList();
                        break;
                    case 2:
                        models = models.OrderByDescending(m => m.Price).ToList();
                        break;
                    case 3:
                        models = models.OrderBy(m => m.Name).ToList();
                        break;
                    case 4:
                        models = models.OrderByDescending(m => m.Name).ToList();
                        break;
                    case 5:
                        models = models.OrderByDescending(m => m.DateCreation).ToList();
                        break;
                    case 6:
                        models = models.OrderBy(m => m.DateCreation).ToList();
                        break;
                    default:
                        models = models.OrderByDescending(m => m.DateCreation).ToList();
                        break;
                }
            }
            else
            {
                models = models.OrderByDescending(m => m.DateCreation).ToList();
            }

            // цена от
            if (filterResult.PriceFrom != null)
            {
                decimal priceFrom = decimal.Parse(filterResult.PriceFrom);
                models = models.Where(m => m.Price != 0 && m.Price > priceFrom).ToList();
            }

            // цена до
            if (filterResult.PriceTo != null)
            {
                decimal priceTo = decimal.Parse(filterResult.PriceTo);
                models = models.Where(m => m.Price != 0 && m.Price < priceTo).ToList();
            }

            return PartialView(models);
        }
        //==========================================================



        //==========================================================
        // 
        public ActionResult Details(string alias)
        {
            if (alias == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ShopProduct product = db.ShopProducts
                .Include(p => p.ShopProductsBrand)
                .Include(p => p.ShopProductsPrices)
                .Include(p => p.ShopProductsTaste)
                .Include(p => p.ShopCategories)
                .Where(p => p.Alias == alias).SingleOrDefault();

            if (product == null)
            {
                return HttpNotFound();
            }

            product.ShopProductsPrices = product.ShopProductsPrices.OrderByDescending(p => p.DateSet).ToList();
            
            return View(product);
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