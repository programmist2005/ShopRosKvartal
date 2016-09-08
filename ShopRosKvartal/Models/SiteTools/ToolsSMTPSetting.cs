using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.Models.SiteTools
{
    public class ToolsSMTPSetting
    {
        public int Id { get; set; }

        [Display(Name = "E-mail отправителя")]
        [Required(ErrorMessage = "Введите E-Mail адрес отправителя")]
        [EmailAddress]
        public string EmailFrom { get; set; }

        [Display(Name = "Логин отправителя")]
        //[Required(ErrorMessage = "Введите логин отправителя")]
        public string UserName { get; set; }

        [Display(Name = "Пароль отправителя")]
        //[Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }

        [Display(Name = "Хост-адрес")]
        [Required(ErrorMessage = "Введите хост-адрес")]
        public string Host { get; set; }

        [Display(Name = "Номера порта")]
        [Required(ErrorMessage = "Введите номера порта")]
        public int Port { get; set; }

        [Display(Name = "Поддержка SSL")]
        public bool EnableSsl { get; set; }
    }
}