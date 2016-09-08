using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.UserData
{
    public class UserContactsSkype
    {
        public int Id { get; set; }
        public string Skype { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}