using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.UserData
{
    public class UserProfile
    {
        [Required]
        [Key, ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        // ф.и.о.
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }

        // адрес
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public int House { get; set; }
        public int? Apartment { get; set; }

        // паспортные данные
        public string PassportNumberAndSeries { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime PassportIssuingDate { get; set; }

        // фото пользователя (название файла с фото)
        public string Photo { get; set; }
    }
}