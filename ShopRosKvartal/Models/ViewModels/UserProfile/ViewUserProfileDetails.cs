using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopRosKvartal.Models.ViewModels.UserProfile
{
    public class ViewUserProfileDetails
    {
        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // пол
        [Display(Name = "Пол")]
        public string Gender { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // ф.и.о.
        public string FullName { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //----------------------------------------------------
        // адрес
        public string Adress { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // паспортные данные
        [Display(Name = "Номер и серия паспорта")]
        public string PassportNumberAndSeries { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Дата выдачи паспорта")]
        public DateTime PassportIssuingDate { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // фото пользователя 
        // (название файла с фото)
        public string Photo { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // телефоны 
        public string[] ContactPhoneNumber { get; set; }

        // скайп
        public string[] ContactSkype { get; set; }

        // соц сети
        public string[] ContactSocialNetworkLink { get; set; }
    }
}