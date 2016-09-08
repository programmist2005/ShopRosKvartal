using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.UserData
{
    public class UserGender
    {
        public int Id { get; set; }
        public string Gender { get; set; }

        // навишационное свойство
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public UserGender()
        {
            ApplicationUsers = new List<ApplicationUser>();
        }
    }
}