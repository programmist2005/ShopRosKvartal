namespace ShopRosKvartal.Migrations
{
    using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ShopRosKvartal.LibraryHelperClasses.Translit;
using ShopRosKvartal.Models;
using ShopRosKvartal.Models.Shop;
using ShopRosKvartal.Models.SiteTools;
using ShopRosKvartal.Models.UserData;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ShopRosKvartal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            // true - false
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ShopRosKvartal.Models.ApplicationDbContext";
        }

        protected override void Seed(ShopRosKvartal.Models.ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //============================================================
            //инициализация ролей
            if (!context.Roles.Any())
            {
                // создание роли
                var role = new IdentityRole { Name = "Администратор" };
                // добавление роли в бд
                roleManager.Create(role);

                role = new IdentityRole { Name = "Модератор" };
                roleManager.Create(role);

                role = new IdentityRole { Name = "Покупатель" };
                roleManager.Create(role);

            }
            //============================================================

            //============================================================
            //создание админа для пустой БД
            var admin = userManager.FindByName("Admin");
            if (admin == null || !context.Users.Any())
            {
                // создание админа
                admin = new ApplicationUser { Email = "test@gmail.com", UserName = "Admin" };
                string password = "P@$$w0rd";
                var result = userManager.Create(admin, password);

                // если создание пользователя прошло успешно
                if (result.Succeeded)
                {
                    var role = roleManager.FindByName("Администратор");
                    if (role != null)
                    {
                        // добавляем для пользователя роль
                        userManager.AddToRole(admin.Id, role.Name);
                    }
                    else
                    {
                        // создание роли
                        role = new IdentityRole { Name = "Администратор" };
                        // добавление роли в бд
                        roleManager.Create(role);
                        // добавляем для пользователя роль
                        userManager.AddToRole(admin.Id, role.Name);
                    }
                }
            }
            //============================================================

            //============================================================
            //инициализация SMTP сервера
            if (!context.ToolsSMTPSettings.Any())
            {
                ToolsSMTPSetting smtp = new ToolsSMTPSetting();
                smtp.EmailFrom = "mailtester2005@gmail.com";
                smtp.UserName = "mailtester2005";
                smtp.Password = "@dm1nP@$$word";
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                context.ToolsSMTPSettings.Add(smtp);
                context.SaveChanges();
            }
            //============================================================

            //============================================================
            //инициализация таблицы полов пользователей
            if (!context.UserGenders.Any())
            {
                UserGender gender = new UserGender();
                gender.Gender = "Мужской";
                context.UserGenders.Add(gender);
                context.SaveChanges();

                gender.Gender = "Женский";
                context.UserGenders.Add(gender);
                context.SaveChanges();
            }
            //============================================================

            //============================================================
            //инициализация таблицы категории
            if (!context.ShopCategories.Any())
            {
                ShopCategory parent = new ShopCategory();
                parent.Name = "Спортивное питание";
                parent.Alias = Translit.TranslitString(parent.Name);
                context.ShopCategories.Add(parent);
                context.SaveChanges();

                //-----------------------------------------------
                ShopCategory child = new ShopCategory {
                    Name = "Протеины",
                    ParentId=parent.Id
                };
                child.Alias = Translit.TranslitString(child.Name);
                context.ShopCategories.Add(child);
                context.SaveChanges();
                //-----------------------------------------------
                child = new ShopCategory
                {
                    Name = "Гейнеры",
                    ParentId = parent.Id
                };
                child.Alias = Translit.TranslitString(child.Name);
                context.ShopCategories.Add(child);
                context.SaveChanges();
            }
            //============================================================

            //============================================================
            //инициализация таблицы вкус товара
            if (!context.ShopProductsTastes.Any())
            {
                string[] tastes = {"Ваниль", "Клубника", "Шоколад", "Карамель-ваниль", "Малина-шоколад", "Миндаль-шоколад", 
                                      "Малина", "Банан", "Вишня", "Абрикос", "Персик", "Апельсин"};
                for (int i = 0; i < tastes.Length; i++)
                {
                    ShopProductsTaste taste = new ShopProductsTaste
                    {
                        Name = tastes[i]
                    };
                    context.ShopProductsTastes.Add(taste);
                    context.SaveChanges();
                }
            }
            //============================================================

            //============================================================
            //инициализация таблицы брэнды
            if (!context.ShopProductsBrands.Any())
            {
                string[] brands = {"Optimum Nutrition", 
                                      "Multipower", 
                                      "BSN", 
                                      "Dymatize", 
                                      "MuscleTech", 
                                      "Weider", 
                                      "Sponser", 
                                      "Twinlab", 
                                      "Gaspari Nutrition", 
                                      "Universal Nutrition"};
                for (int i = 0; i < brands.Length; i++)
                {
                    ShopProductsBrand brand = new ShopProductsBrand
                    {
                        Name = brands[i],
                        Alias = brands[i].Replace(" ", "-")
                    };
                    context.ShopProductsBrands.Add(brand);
                    context.SaveChanges();
                }
            }
            //============================================================
        }
    }
}
