using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.LibraryHelperClasses.Validators
{
    // проверка выбранной даты выдачи паспорта в профиле пользователя
    // дата выдачи паспорта не может быть раньше чем через 16лет после рождения
    public class CheckDatePassportIssuingAttribute : ValidationAttribute
    {
        public CheckDatePassportIssuingAttribute(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

       private string DateToCompareToFieldName { get; set; }

       protected override ValidationResult IsValid(object value, ValidationContext validationContext)
       {
           if (value != null)
           {
               if (validationContext != null)
               {
                   DateTime passportIssuingDate = (DateTime)value;
                   DateTime dateOfBirth = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);


                   TimeSpan subtraction = passportIssuingDate.Subtract(dateOfBirth);
                   DateTime subtractionDate = new DateTime(subtraction.Ticks);
                   if (subtractionDate.Year >= 16)
                   {
                       return ValidationResult.Success;
                   }
                   return new ValidationResult("Дата выдачи паспорта не может быть до достижения 16ти летнего возраста");
               }
               return new ValidationResult("Укажите вашу дату рождения");
           }
           return new ValidationResult("Укажите дату выдачи паспорта");
       }
    }
}