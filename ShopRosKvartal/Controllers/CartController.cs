using ShopRosKvartal.Models;
using ShopRosKvartal.Models.Shop;
using ShopRosKvartal.Models.Shop.ShoppingCartModels;
using ShopRosKvartal.Models.ViewModels.Shop.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ShopRosKvartal.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        public ActionResult Index()
        {
            ShoppingCart cart = ShoppingCartCreateOrLoad();
            ViewShoppingCart modelCart = new ViewShoppingCart();
            if (cart != null)
            {

                modelCart.Id = cart.Id;
                modelCart.UserId = cart.UserId;
                modelCart.CookieKey = cart.CookieKey;
                modelCart.CountProducts = 0;

                if (cart.ShoppingCartsProducts.Count > 0)
                {
                    modelCart.Products = new List<ViewShoppingCartsProducts>();

                    foreach (ShoppingCartsProduct cartProduct in cart.ShoppingCartsProducts)
                    {
                        ViewShoppingCartsProducts modelProduct = new ViewShoppingCartsProducts
                        {
                            Id = cartProduct.Id,
                            Alias = cartProduct.ShopProduct.Alias,
                            Name = cartProduct.ShopProduct.Name,
                            PhotoName = cartProduct.ShopProduct.PhotoName,
                            Quantity = cartProduct.Quantity,
                            DateAdded = cartProduct.DateAdded
                        };

                        modelCart.CountProducts = modelCart.CountProducts + cartProduct.Quantity;

                        ShopProductsPrice price = db.ShopProductsPrices.Where(p => p.ShopProduct.Id == cartProduct.ShopProduct.Id && p.CurrentPrice).SingleOrDefault();
                        if (price != null)
                        {
                            modelProduct.PriceOne = price.Price;
                            modelProduct.PriceTotal = Decimal.Multiply(modelProduct.Quantity, modelProduct.PriceOne);
                        }

                        modelCart.Products.Add(modelProduct);
                    }

                    modelCart.Products = modelCart.Products.OrderByDescending(m => m.DateAdded).ToList();
                }
            }
            return View(modelCart);
        }
        //==========================================================



        //==========================================================
        public ActionResult AddToCard(int? id, int? quant)
        {
            if (id != null && quant != null && quant > 0) 
            {
                ShoppingCart cart = ShoppingCartCreateOrLoad();
                ShopProduct product = db.ShopProducts.Find(id);
                if (product != null)
                {
                    // проверка корзины - нет ли в ней этого товара добавленного ранее
                    ShoppingCartsProduct existProductInCart = cart.ShoppingCartsProducts.Where(p => p.ShopProduct.Id == product.Id).SingleOrDefault();
                    if (existProductInCart == null)
                    {
                        // если продукта в корзине нет - добавляем
                        ShoppingCartsProduct addingProduct = new ShoppingCartsProduct
                        {
                            ShopProduct = product,
                            Quantity = quant.Value,
                            ShoppingCart = cart,
                            DateAdded = DateTime.Now
                        };

                        cart.ShoppingCartsProducts.Add(addingProduct);
                        db.Entry(cart).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        // если продукта в корзине есть - редактируем
                        existProductInCart.Quantity = existProductInCart.Quantity + quant.Value;
                        existProductInCart.DateAdded = DateTime.Now;

                        db.Entry(existProductInCart).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    
                }
            }
            
            return RedirectToAction("_ShoppingCart");
        }
        //==========================================================



        //==========================================================
        public ActionResult EditProductInCart(int? id, int? quant)
        {
            if (id != null && quant != null && quant > 0)
            {
                ShoppingCart cart = ShoppingCartCreateOrLoad();
                    // проверка корзины есть ли в ней товар
                    ShoppingCartsProduct existProductInCart = cart.ShoppingCartsProducts.Where(p => p.Id == id).SingleOrDefault();
                    if (existProductInCart != null)
                    {
                        // если продукт в корзине есть - редактируем
                        existProductInCart.Quantity = quant.Value;
                        existProductInCart.DateAdded = DateTime.Now;

                        db.Entry(existProductInCart).State = EntityState.Modified;
                        db.SaveChanges();
                    }

            }
            //return new EmptyResult();
            return RedirectToAction("Index");
        }
        //==========================================================


        //==========================================================
        public ActionResult CleanCard() 
        {
            ShoppingCart cart = ShoppingCartCreateOrLoad();
            if (cart != null && cart.ShoppingCartsProducts.Count > 0) 
            {
                db.ShoppingCartsProducts.RemoveRange(cart.ShoppingCartsProducts);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //==========================================================



        //==========================================================
        public ActionResult RemoveProductFromCart(int id)
        {
            ShoppingCart cart = ShoppingCartCreateOrLoad();
            if (cart != null && cart.ShoppingCartsProducts.Count > 0)
            {
                ShoppingCartsProduct removeProduct = db.ShoppingCartsProducts.Find(id);
                db.ShoppingCartsProducts.Remove(removeProduct);
                db.SaveChanges();
            }
            return new EmptyResult();
        }
        //==========================================================


        //==========================================================
        public ActionResult _ShoppingCart()
        {
            ShoppingCart cart = ShoppingCartCreateOrLoad();
            int countProducts = 0;
            if (cart != null && cart.ShoppingCartsProducts.Count > 0)
            {
                foreach (ShoppingCartsProduct prodCart in cart.ShoppingCartsProducts)
                {
                    countProducts = countProducts + prodCart.Quantity;
                }
            }
            return PartialView(countProducts);
        }
        //==========================================================



        //==========================================================
        private ShoppingCart ShoppingCartCreateOrLoad()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string currentUserId = HttpContext.User.Identity.GetUserId();
                ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = userManager.FindById(currentUserId);

                ShoppingCart cardUser = ShoppingCartFindByUserId(currentUser.Id);
                if (cardUser == null)
                {
                    // создаем корзину для пользователя
                    cardUser = ShoppingCartCreateForUser(currentUser.Id);
                    cardUser.ShoppingCartsProducts = new List<ShoppingCartsProduct>();
                    // проверяем наличие анонимной корзины
                    if (HttpContext.Request.Cookies["ShoppingCartKeyId"] != null)
                    {
                        ShoppingCart cardAnonym = ShoppingCartFind(HttpContext.Request.Cookies["ShoppingCartKeyId"].Value);
                        if (cardAnonym != null && cardAnonym.ShoppingCartsProducts != null && cardAnonym.ShoppingCartsProducts.Count > 0)
                        {
                            // переносим товары из анонимной корзины в корзину пользователя
                            foreach (var item in cardAnonym.ShoppingCartsProducts)
                            {
                                cardUser.ShoppingCartsProducts.Add(item);
                            }
                            db.Entry(cardUser).State = EntityState.Modified;
                            // удаляем анонимную корзину
                            db.ShoppingCartsProducts.RemoveRange(cardAnonym.ShoppingCartsProducts);
                            db.ShoppingCarts.Remove(cardAnonym);

                            DeleteCookie();

                            db.SaveChanges();
                        }
                    }
                    return cardUser;
                }
                else
                {
                    // проверяем наличие анонимной корзины
                    if (HttpContext.Request.Cookies["ShoppingCartKeyId"] != null)
                    {
                        ShoppingCart cardAnonym = ShoppingCartFind(HttpContext.Request.Cookies["ShoppingCartKeyId"].Value);
                        if (cardAnonym != null && cardAnonym.ShoppingCartsProducts != null && cardAnonym.ShoppingCartsProducts.Count > 0)
                        {
                            // если пользовательская корзина пуста а анонимная имеет товары, тогда
                            if (cardUser.ShoppingCartsProducts == null || cardUser.ShoppingCartsProducts.Count == 0)
                            {
                                // переносим товары из анонимной корзины в корзину пользователя
                                cardUser.ShoppingCartsProducts = new List<ShoppingCartsProduct>();
                                foreach (var item in cardAnonym.ShoppingCartsProducts)
                                {
                                    cardUser.ShoppingCartsProducts.Add(item);
                                }
                                db.Entry(cardUser).State = EntityState.Modified;
                                // удаляем анонимную корзину
                                db.ShoppingCartsProducts.RemoveRange(cardAnonym.ShoppingCartsProducts);
                                db.ShoppingCarts.Remove(cardAnonym);
                                db.SaveChanges();

                                DeleteCookie();

                                return cardUser;
                            }
                            else
                            {
                                // если обе корзины не пустые
                                foreach (var item in cardAnonym.ShoppingCartsProducts)
                                {
                                    ShoppingCartsProduct exist = cardUser.ShoppingCartsProducts
                                        .Where(p => p.ShopProduct.Id == item.ShopProduct.Id)
                                        .SingleOrDefault();

                                    if (exist != null)
                                    {
                                        exist.Quantity = exist.Quantity + item.Quantity;
                                        exist.DateAdded = DateTime.Now;
                                    }
                                    else
                                    {
                                        cardUser.ShoppingCartsProducts.Add(item);
                                    }
                                }
                                db.Entry(cardUser).State = EntityState.Modified;
                                // удаляем анонимную корзину
                                db.ShoppingCartsProducts.RemoveRange(cardAnonym.ShoppingCartsProducts);
                                db.ShoppingCarts.Remove(cardAnonym);
                                db.SaveChanges();

                                DeleteCookie();

                                return cardUser;
                            }
                        }
                    }
                }
                return cardUser;
            }
            else
            {
                if (HttpContext.Request.Cookies["ShoppingCartKeyId"] != null)
                {
                    return ShoppingCartFind(HttpContext.Request.Cookies["ShoppingCartKeyId"].Value);
                }
                else
                {
                    return ShoppingCartCreate();
                }
            }
            //return null;
        }
        //==========================================================

        //==========================================================
        private ShoppingCart ShoppingCartCreate()
        {
            string keyId = Guid.NewGuid().ToString();
            HttpContext.Response.Cookies["ShoppingCartKeyId"].Value = keyId;
            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.CookieKey = keyId;
            db.ShoppingCarts.Add(shoppingCart);
            db.SaveChanges();
            return shoppingCart;
        }
        //==========================================================



        //==========================================================
        private ShoppingCart ShoppingCartCreateForUser(string id)
        {
            ShoppingCart shoppingCart = new ShoppingCart { UserId = id };

            db.ShoppingCarts.Add(shoppingCart);
            db.SaveChanges();
            return shoppingCart;
        }
        //==========================================================



        //==========================================================
        private ShoppingCart ShoppingCartFind(string keyId)
        {
            ShoppingCart shoppingCart = db.ShoppingCarts
                .Include(s=>s.ShoppingCartsProducts)
                .Where(s => s.CookieKey == keyId)
                .SingleOrDefault();
            return shoppingCart;
        }
        //==========================================================



        //==========================================================
        private ShoppingCart ShoppingCartFindByUserId(string id)
        {
            ShoppingCart shoppingCart = db.ShoppingCarts
                .Include(s => s.ShoppingCartsProducts)
                .Where(s => s.UserId == id)
                .SingleOrDefault();
            return shoppingCart;
        }
        //==========================================================



        //==========================================================
        private void DeleteCookie()
        {
            if (HttpContext.Request.Cookies["ShoppingCartKeyId"] != null)
            {
                HttpContext.Response.Cookies["ShoppingCartKeyId"].Expires = DateTime.Now.AddDays(-1);
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
    }
}