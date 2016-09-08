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
            //������������� �����
            if (!context.Roles.Any())
            {
                // �������� ����
                var role = new IdentityRole { Name = "�������������" };
                // ���������� ���� � ��
                roleManager.Create(role);

                role = new IdentityRole { Name = "���������" };
                roleManager.Create(role);

                role = new IdentityRole { Name = "����������" };
                roleManager.Create(role);

            }
            //============================================================

            //============================================================
            //�������� ������ ��� ������ ��
            var admin = userManager.FindByName("Admin");
            if (admin == null || !context.Users.Any())
            {
                // �������� ������
                admin = new ApplicationUser { Email = "test@gmail.com", UserName = "Admin" };
                string password = "P@$$w0rd";
                var result = userManager.Create(admin, password);

                // ���� �������� ������������ ������ �������
                if (result.Succeeded)
                {
                    var role = roleManager.FindByName("�������������");
                    if (role != null)
                    {
                        // ��������� ��� ������������ ����
                        userManager.AddToRole(admin.Id, role.Name);
                    }
                    else
                    {
                        // �������� ����
                        role = new IdentityRole { Name = "�������������" };
                        // ���������� ���� � ��
                        roleManager.Create(role);
                        // ��������� ��� ������������ ����
                        userManager.AddToRole(admin.Id, role.Name);
                    }
                }
            }
            //============================================================

            //============================================================
            //������������� SMTP �������
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
            //������������� ������� ����� �������������
            if (!context.UserGenders.Any())
            {
                UserGender gender = new UserGender();
                gender.Gender = "�������";
                context.UserGenders.Add(gender);
                context.SaveChanges();

                gender.Gender = "�������";
                context.UserGenders.Add(gender);
                context.SaveChanges();
            }
            //============================================================

            //============================================================
            //������������� ������� ���������
            if (!context.ShopCategories.Any())
            {
                ShopCategory parent = new ShopCategory();
                parent.Name = "���������� �������";
                parent.Alias = Translit.TranslitString(parent.Name);
                context.ShopCategories.Add(parent);
                context.SaveChanges();

                //-----------------------------------------------
                ShopCategory child = new ShopCategory {
                    Name = "��������",
                    ParentId=parent.Id
                };
                child.Alias = Translit.TranslitString(child.Name);
                context.ShopCategories.Add(child);
                context.SaveChanges();
                //-----------------------------------------------
                child = new ShopCategory
                {
                    Name = "�������",
                    ParentId = parent.Id
                };
                child.Alias = Translit.TranslitString(child.Name);
                context.ShopCategories.Add(child);
                context.SaveChanges();
            }
            //============================================================

            //============================================================
            //������������� ������� ���� ������
            if (!context.ShopProductsTastes.Any())
            {
                string[] tastes = {"������", "��������", "�������", "��������-������", "������-�������", "�������-�������", 
                                      "������", "�����", "�����", "�������", "������", "��������"};
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
            //������������� ������� ������
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
