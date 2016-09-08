using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.UserData
{
    public class UserContactsSocial
    {
        public int Id { get; set; }
        public string SocialNetworkLink { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}