using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.UserData
{
    public class UserContactsPhone
    {
        public int Id { get; set; }
        public string CountryPhoneCode { get; set; }
        public string PhoneNumber { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}