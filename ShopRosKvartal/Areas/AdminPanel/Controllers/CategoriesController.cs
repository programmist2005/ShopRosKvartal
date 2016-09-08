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
using ShopRosKvartal.Areas.AdminPanel.Models.ViewModels.Shop.Categories;
using ShopRosKvartal.LibraryHelperClasses.Translit;

namespace ShopRosKvartal.Areas.AdminPanel.Controllers
{
    //==========================================================
    // Контроллер "категории товаров"
    // Доступен для администраторов и модераторов

    [Authorize(Roles = "Администратор, Модератор")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        // Вывод всех категорий в виде дерева ссылок
        public ActionResult Index()
        {
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
            return View(hierarchy);
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
        // инфо о категории
        // GET: AdminPanel/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopCategory shopCategory = db.ShopCategories.Find(id);
            if (shopCategory == null)
            {
                return HttpNotFound();
            }

            // заполнение модели представления
            ViewCategoryDetail category = new ViewCategoryDetail
            {
                Name = shopCategory.Name,
                Alias = shopCategory.Alias,
                Description = shopCategory.Description
            };
            if (shopCategory.ParentId != null)
            {
                category.Parent = db.ShopCategories.Find(shopCategory.ParentId).Name;
            }
            // подсчет подкатегорий
            category.CountChildCategories = ChildCategoriesCount(shopCategory.Id);
            // подсчет товаров
            category.CountProducts = 0;
            return View(category);
        }
        //==========================================================



        //==========================================================
        // вспомогательный метод
        // поиск дочерних категорий и сохранение их в лист для отображения в Index
        private int ChildCategoriesCount(int id)
        {
            // поиск дочерних
            List<ShopCategory> childCategories = db.ShopCategories
                .Where(c => c.ParentId == id).OrderBy(c => c.Name).ToList();
            int count = 0;
            foreach (ShopCategory childCategory in childCategories)
            {
                count++;
                count = count + ChildCategoriesCount(childCategory.Id);
            }
            return count;
        }
        //==========================================================



        //==========================================================
        // создание категории
        // GET: AdminPanel/Categories/Create
        public ActionResult Create()
        {
            ViewCategoryCreate category = new ViewCategoryCreate();
            // создания списка выбора родительской категории
            List<ShopCategory> categories = db.ShopCategories.OrderBy(c => c.Name).ToList();
            category.CategoriesList = new SelectList(categories, "Id", "Name");
            return View(category);
        }
        //==========================================================

        //==========================================================
        // создание категории
        // POST: AdminPanel/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewCategoryCreate shopCategory)
        {
            if (ModelState.IsValid)
            {
                // сохранение 
                ShopCategory category = new ShopCategory();
                category.Name = shopCategory.Name;
                if (shopCategory.Alias == null)
                {
                    category.Alias = Translit.TranslitString(shopCategory.Name);
                }
                else
                {
                    category.Alias = shopCategory.Alias;
                }
                category.Description = shopCategory.Description;
                category.ParentId = shopCategory.SelectedId;
                db.ShopCategories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // создание выпадающего списка родительских категорий
            List<ShopCategory> categories = db.ShopCategories.OrderBy(c => c.Name).ToList();
            shopCategory.CategoriesList = new SelectList(categories, "Id", "Name", shopCategory.SelectedId);
            return View(shopCategory);
        }
        //==========================================================



        //==========================================================
        // редактирование категории
        // GET: AdminPanel/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            // проверка входящего Id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // поиск модели в БД
            ShopCategory shopCategory = db.ShopCategories.Find(id);
            if (shopCategory == null)
            {
                return HttpNotFound();
            }

            // создание модели представления
            ViewCategoryEdit category = new ViewCategoryEdit
            {
                Id = shopCategory.Id,
                Name = shopCategory.Name,
                Alias = shopCategory.Alias,
                Description = shopCategory.Description,
                SelectedId = shopCategory.ParentId
            };
            // создание выпадающего списка родительских категорий с выбранным значением по умолчанию
            category.CategoriesList = new SelectList(ParentListForCurrentCategory(category.Id), "Id", "Name", category.SelectedId);
            return View(category);
        }
        //==========================================================

        //==========================================================
        // редактирование категории
        // POST: AdminPanel/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewCategoryEdit shopCategory)
        {
            if (ModelState.IsValid)
            {
                // сохранение 
                ShopCategory category = db.ShopCategories.Find(shopCategory.Id); ;
                bool changeName = false;
                if (category.Name != shopCategory.Name)
                {
                    category.Name = shopCategory.Name;
                    changeName = true;
                }

                if (shopCategory.Alias == null || changeName)
                {
                    category.Alias = Translit.TranslitString(shopCategory.Name);
                }
                else
                {
                    category.Alias = shopCategory.Alias;
                }
                category.Description = shopCategory.Description;
                category.ParentId = shopCategory.SelectedId;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // создание выпадающего списка родительских категорий
            shopCategory.CategoriesList = new SelectList(ParentListForCurrentCategory(shopCategory.Id), "Id", "Name", shopCategory.SelectedId);
            return View(shopCategory);
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // создание списка возможных родительских категорий 
        // без выбранной категори и ее дочерних
        private List<ShopCategory> ParentListForCurrentCategory(int id)
        {
            List<ShopCategory> categories = db.ShopCategories.OrderBy(c => c.Name).ToList();
            categories = RemoveChileFromParentList(id, categories);
            return categories;
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // создание списка возможных родительских категорий 
        // без выбранной категори и ее дочерних - удаление дочерних из списка
        private List<ShopCategory> RemoveChileFromParentList(int id, List<ShopCategory> categories)
        {
            List<ShopCategory> childCategories = db.ShopCategories
                .Where(c => c.ParentId == id).OrderBy(c => c.Name).ToList();
            foreach (ShopCategory childCategory in childCategories)
            {
                categories = RemoveChileFromParentList(childCategory.Id, categories);
                categories.Remove(childCategory);
            }
            return categories;
        }
        //==========================================================



        //==========================================================
        // GET: AdminPanel/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopCategory shopCategory = db.ShopCategories.Find(id);
            if (shopCategory == null)
            {
                return HttpNotFound();
            }

            // создание модели представления
            ViewCategoryDelete category = new ViewCategoryDelete
            {
                Id = shopCategory.Id,
                Name = shopCategory.Name,
                Alias = shopCategory.Alias,
                Description = shopCategory.Description,
                DeleteAll = false
            };

            if (shopCategory.ParentId != null)
            {
                category.Parent = db.ShopCategories.Find(shopCategory.ParentId).Name;
            }
            // подсчет подкатегорий
            category.CountChildCategories = ChildCategoriesCount(shopCategory.Id);
            // подсчет товаров
            category.CountProducts = 0;
            return View(category);
        }
        //==========================================================

        //==========================================================
        // POST: AdminPanel/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ViewCategoryDelete model)
        {
            ShopCategory shopCategory = db.ShopCategories.Find(model.Id);
            if (model.DeleteAll)
            {
                DeleteChileCategories(shopCategory);
                db.ShopCategories.Remove(shopCategory);
                db.SaveChanges();
            }
            else
            {
                ClearParent(shopCategory.Id);
                db.ShopCategories.Remove(shopCategory);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // поиск дочерних категорий 
        // и изменение родительской категории на null
        private void ClearParent(int id)
        {
            List<ShopCategory> childCategories = db.ShopCategories
                .Where(c => c.ParentId == id).ToList();
            foreach (ShopCategory childCategory in childCategories)
            {
                childCategory.ParentId = null;
                db.Entry(childCategory).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // поиск дочерних категорий 
        // и удаление их 
        private void DeleteChileCategories(ShopCategory category)
        {
            List<ShopCategory> childCategories = db.ShopCategories
                .Where(c => c.ParentId == category.Id).OrderBy(c => c.Name).ToList();
            foreach (ShopCategory childCategory in childCategories)
            {
                DeleteChileCategories(childCategory);
                db.ShopCategories.Remove(childCategory);
                db.SaveChanges();
            }
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
