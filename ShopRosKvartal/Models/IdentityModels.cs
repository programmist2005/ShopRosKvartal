using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ShopRosKvartal.Models.UserData;
using ShopRosKvartal.Models.VerificationCodes;
using System.Collections.Generic;
using ShopRosKvartal.Models.Shop.ShoppingCartModels;
using ShopRosKvartal.Models.Shop.Order;

namespace ShopRosKvartal.Models
{
    public class ApplicationUser : IdentityUser
    {
        //=====================================================
        // навигационные свойства
        // пол пользователя
        public virtual UserGender UserGender { get; set; }

        // профиль пользователя
        public virtual UserProfile UserProfile { get; set; }

        //=====================================================
        // на таблицу кодов подтверждения email
        public virtual CodesVerificationEmail CodesVerificationEmail { get; set; }

        // на таблицу кодов сброса пароля
        public virtual CodesResetPassword CodesResetPassword { get; set; }
        //=====================================================
        // контакты пользователя
        // телефоны
        public ICollection<UserContactsPhone> UserContactsPhones { get; set; }

        // скайп
        public ICollection<UserContactsSkype> UserContactsSkypes { get; set; }

        // социальные сети
        public ICollection<UserContactsSocial> UserContactsSocials { get; set; }

        //=====================================================
        // заказы
        public ICollection<ShopOrder> ShopOrders { get; set; }

        // конструктор
        public ApplicationUser()
        {
            UserContactsPhones = new List<UserContactsPhone>();
            UserContactsSkypes = new List<UserContactsSkype>();
            UserContactsSocials = new List<UserContactsSocial>();
            ShopOrders = new List<ShopOrder>();
        }
        //=====================================================

        //=====================================================
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}