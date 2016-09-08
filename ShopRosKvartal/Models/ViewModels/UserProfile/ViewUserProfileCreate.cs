using ShopRosKvartal.LibraryHelperClasses.Validators;
using ShopRosKvartal.Models.UserData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopRosKvartal.Models.ViewModels.UserProfile
{
    public class ViewUserProfileCreate
    {
        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // выбор пола
        public int? SelectedId { get; set; }

        [Display(Name = "Выберите ваш пол")]
        public SelectList GenderList { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // ф.и.о.
        [Required(ErrorMessage = "Введите ваше имя")]
        [Display(Name = "Имя")]
        [RegularExpression(@"^[а-яА-ЯёЁ]+$", ErrorMessage = "Имя может содержать только из букв кириллицы а-я, А-Я")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите вашу фамилию")]
        [Display(Name = "Фамилия")]
        [RegularExpression(@"^[а-яА-ЯёЁ]+$", ErrorMessage = "Фамилия может содержать только из букв кириллицы а-я, А-Я")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Введите ваше отчество")]
        [Display(Name = "Отчество")]
        [RegularExpression(@"^[а-яА-ЯёЁ]+$", ErrorMessage = "Отчество может содержать только из букв кириллицы а-я, А-Я")]
        public string Patronymic { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //----------------------------------------------------
        // адрес
        [Required(ErrorMessage = "Введите вашу страну проживания")]
        [Display(Name = "Страна")]
        [RegularExpression(@"^[а-яА-ЯёЁ]+$", ErrorMessage = "Название страны может содержать только из букв кириллицы а-я, А-Я")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Введите ваш город проживания")]
        [Display(Name = "Город")]
        [RegularExpression(@"^[а-яА-ЯёЁ]+$", ErrorMessage = "Название города может содержать только из букв кириллицы а-я, А-Я")]
        public string City { get; set; }

        [Required(ErrorMessage = "Введите ваш почтовый индекс")]
        [Display(Name = "Почтовый индекс")]
        [RegularExpression(@"\d+", ErrorMessage = "Только числовое значение")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Введите вашу улицу")]
        [Display(Name = "Улица")]
        [RegularExpression(@"^[а-яА-ЯёЁ]+$", ErrorMessage = "Название улицы может содержать только из букв кириллицы а-я, А-Я")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Введите № вашего дома")]
        [Display(Name = "№ дома")]
        [RegularExpression(@"\d+", ErrorMessage = "Только числовое значение")]
        public int House { get; set; }

        [Display(Name = "№ квартиры")]
        [RegularExpression(@"\d+", ErrorMessage = "Только числовое значение")]
        public int? Apartment { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // паспортные данные
        [Required(ErrorMessage = "Укажите номер и серия паспорта")]
        [Display(Name = "Номер и серия паспорта")]
        [RegularExpression(@"^[0-9а-яА-ЯЁё]+$", ErrorMessage = "Номер и серия паспорта может состоять только из букв кириллицы а-я, А-Я и цифры 0-9")]
        public string PassportNumberAndSeries { get; set; }

        [Required(ErrorMessage = "Укажите вашу дату рождения")]
        [Display(Name = "Дата рождения")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Укажите дату выдачи паспорта")]
        [Display(Name = "Дата выдачи паспорта")]
        [CheckDatePassportIssuing("DateOfBirth", ErrorMessage = "Укажите вашу дату рождения")]
        public DateTime PassportIssuingDate { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // фото пользователя 
        // (название файла с фото)
        public string Photo { get; set; }

        // (загружаемый файл фото)
        public HttpPostedFileBase FilePhoto { get; set; }

        //-----------------------------------------------------
        //-----------------------------------------------------
        //-----------------------------------------------------
        // телефоны 
        public string[] ContactPhoneCountryCode { get; set; }
        public string[] ContactPhoneNumber { get; set; }

        // скайп
        public string[] ContactSkype { get; set; }

        // соц сети
        public string[] ContactSocialNetworkLink { get; set; }

    }
}