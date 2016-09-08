using ShopRosKvartal.LibraryHelperClasses.DataChecking;
using ShopRosKvartal.LibraryHelperClasses.Images;
using ShopRosKvartal.Models;
using ShopRosKvartal.Models.UserData;
using ShopRosKvartal.Models.ViewModels.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ShopRosKvartal.Controllers
{
    //==========================================================
    //============= Контроллер Профиль пользователя ============
    //============== Доступен для авторизированных =============
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================

        public ActionResult Index()
        {
            string currentUserId = HttpContext.User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users
                .Include(p => p.UserContactsPhones)
                .Include(s => s.UserContactsSkypes)
                .Include(sn => sn.UserContactsSocials)
                .Where(u => u.Id == currentUserId).SingleOrDefault();
                
            if (currentUser.UserProfile == null)
            {
                return RedirectToAction("Create", "Profile", null);
            }

            ViewUserProfileDetails profile = new ViewUserProfileDetails
            {
                Gender = currentUser.UserGender.Gender,
                FullName = currentUser.UserProfile.Surname + " " + currentUser.UserProfile.Name + " " + currentUser.UserProfile.Patronymic,
                Photo = currentUser.UserProfile.Photo,
                // паспортные данные
                PassportNumberAndSeries = currentUser.UserProfile.PassportNumberAndSeries,
                DateOfBirth = currentUser.UserProfile.DateOfBirth,
                PassportIssuingDate = currentUser.UserProfile.PassportIssuingDate
            };

            //контакты
            profile.ContactPhoneNumber = FromBDToArrayPhone(currentUser.UserContactsPhones);
            profile.ContactSkype = FromBDToArraySkypes(currentUser.UserContactsSkypes);
            profile.ContactSocialNetworkLink = FromBDToArraySocial(currentUser.UserContactsSocials);

            //адрес
            profile.Adress = FromBDToAdressString(
                currentUser.UserProfile.Country,
                currentUser.UserProfile.City,
                currentUser.UserProfile.ZipCode, 
                currentUser.UserProfile.Street, 
                currentUser.UserProfile.House, 
                currentUser.UserProfile.Apartment);

            if (!currentUser.EmailConfirmed)
            {
                string error = "Подтвердите ваш email";
                ViewBag.Error = error;
            }
            return View(profile);
        }
        //==========================================================



        //==========================================================
        // Создание профиля
        // Метод Get
        public ActionResult Create(string returnUrl)
        {
            ViewUserProfileCreate profile = new ViewUserProfileCreate();
            // создания списка выбора пола
            profile.GenderList = new SelectList(db.UserGenders.OrderBy(u => u.Id).ToList(), "Id", "Gender");
            ViewBag.ReturnUrl = returnUrl;
            return View(profile);
        }
        //==========================================================

        //==========================================================
        // Создание профиля
        // Метод Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewUserProfileCreate model, string returnUrl)
        {
            bool correct = true;

            if (ModelState.IsValid)
            {
                // проверка номера телефона
                if (model.ContactPhoneNumber != null)
                {
                    correct = DataChecking.Phone(model.ContactPhoneNumber, model.ContactPhoneCountryCode);
                    if (!correct)
                    {
                        ViewBag.ErrorCorrectPhones = "Один или несколько телефонов указаны неверно";
                    }
                }

                // проверка ссылок соц сетей
                if (model.ContactSocialNetworkLink != null)
                {
                    correct = DataChecking.SosialNetworkLink(model.ContactSocialNetworkLink);
                    if (!correct)
                    {
                        ViewBag.ErrorCorrectSocialNetworkLink = "Одна или несколько ссылок указаны неверно";
                    }
                }

                // если ошибок нет - сохранение профиля пользователя
                if (correct)
                {
                    string currentUserId = HttpContext.User.Identity.GetUserId();
                    ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                    ApplicationUser currentUser = userManager.FindById(currentUserId);

                    currentUser.UserContactsPhones = ConvertPhones(currentUser, model.ContactPhoneNumber, model.ContactPhoneCountryCode);
                    currentUser.UserContactsSkypes = ConvertSkypes(currentUser, model.ContactSkype);
                    currentUser.UserContactsSocials = ConvertSocials(currentUser, model.ContactSocialNetworkLink);

                    //сохранение фото пользователя
                    //получение имя файла
                    string userPhoto = "";
                    if (model.FilePhoto != null)
                    {
                        string dirPath = HttpContext.Server.MapPath("~/Content/Images/UserPhoto");
                        userPhoto = Image.Save(
                            model.FilePhoto,
                            dirPath,
                            null,
                            currentUser.Id);
                    }

                    //пол пользователя
                    if (model.SelectedId != null)
                    {
                        UserGender gender = db.UserGenders.Find(model.SelectedId);
                        currentUser.UserGender = gender;
                        if (userPhoto == "")
                        {
                            if (gender.Gender == "Мужской")
                            {
                                userPhoto = "male200.jpg";
                            }
                            if (gender.Gender == "Женский") 
                            {
                                userPhoto = "female200.jpg";
                            }
                            
                        }
                    }
                    else
                    {
                        if (userPhoto == "")
                        {
                            userPhoto = "unknown200.jpg";
                        }
                    }

                    currentUser.UserProfile = new UserProfile
                    {
                        //фио
                        Name = model.Name,
                        Patronymic = model.Patronymic,
                        Surname = model.Surname,
                        //фото
                        Photo = userPhoto,
                        //адрес проживания
                        Country = model.Country,
                        City = model.City,
                        ZipCode = model.ZipCode,
                        Street = model.Street,
                        House = model.House,
                        Apartment = model.Apartment,
                        //паспортные данные
                        PassportNumberAndSeries = model.PassportNumberAndSeries,
                        DateOfBirth = model.DateOfBirth,
                        PassportIssuingDate = model.PassportIssuingDate
                    };
                    userManager.Update(currentUser);
                    //db.SaveChanges();
                    if (returnUrl != null)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    return RedirectToAction("Index", "Profile", null);
                }
            }
            // создания списка выбора пола
            model.GenderList = new SelectList(db.UserGenders.OrderBy(u => u.Id).ToList(), "Id", "Gender", model.SelectedId);

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        //==========================================================



        //==========================================================
        // вспомогательный метод
        // сохренение телефонов в модель
        private List<UserContactsPhone> ConvertPhones(ApplicationUser currentUser, string[] phoneNumber, string[] countryCode)
        {
            if (phoneNumber != null)
            {
                List<UserContactsPhone> convertPhones = new List<UserContactsPhone>();
                for (int i = 0; i < phoneNumber.Length; i++)
                {
                    UserContactsPhone phone = new UserContactsPhone
                    {
                        PhoneNumber = phoneNumber[i],
                        CountryPhoneCode = countryCode[i],
                        ApplicationUser = currentUser
                    };
                    convertPhones.Add(phone);
                }
                return convertPhones;
            }
            return null;
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // сохренение skype в модель
        private List<UserContactsSkype> ConvertSkypes(ApplicationUser currentUser, string[] skypes)
        {
            if (skypes != null)
            {
                List<UserContactsSkype> convertSkypes = new List<UserContactsSkype>();
                for (int i = 0; i < skypes.Length; i++)
                {
                    UserContactsSkype skype = new UserContactsSkype
                    {
                        Skype = skypes[i],
                        ApplicationUser = currentUser
                    };
                    convertSkypes.Add(skype);
                }
                return convertSkypes;
            }
            return null;
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // сохренение ссылки на соц сеть в модель
        private List<UserContactsSocial> ConvertSocials(ApplicationUser currentUser, string[] socials)
        {
            if (socials != null)
            {
                List<UserContactsSocial> convertSocials = new List<UserContactsSocial>();
                for (int i = 0; i < socials.Length; i++)
                {
                    UserContactsSocial social = new UserContactsSocial
                    {
                        SocialNetworkLink = socials[i],
                        ApplicationUser = currentUser
                    };
                    convertSocials.Add(social);
                }
                return convertSocials;
            }
            return null;
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // тедефоны из Бд в массив
        private string[] FromBDToArrayPhone(ICollection<UserContactsPhone> phones)
        {
            List<string> listPhones=new List<string>();
            foreach (UserContactsPhone phone in phones)
            {
                string phoneToList = string.Format("{0}-({1})-{2}-{3}",
                    phone.CountryPhoneCode,
                    phone.PhoneNumber.Substring(0, 3),
                    phone.PhoneNumber.Substring(3, 3),
                    phone.PhoneNumber.Substring(6));
                listPhones.Add(phoneToList);
            }
            return listPhones.ToArray();
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // skype из Бд в массив
        private string[] FromBDToArraySkypes(ICollection<UserContactsSkype> skypes)
        {
            List<string> listSkype = new List<string>();
            foreach (UserContactsSkype skype in skypes)
            {
                listSkype.Add(skype.Skype);
            }
            return listSkype.ToArray();
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // соцсети из Бд в массив
        private string[] FromBDToArraySocial(ICollection<UserContactsSocial> socials)
        {
            List<string> listSocial = new List<string>();
            foreach (UserContactsSocial social in socials)
            {
                listSocial.Add(social.SocialNetworkLink);
            }
            return listSocial.ToArray();
        }
        //==========================================================

        //==========================================================
        // вспомогательный метод
        // адрес из Бд в строку
        private string FromBDToAdressString(string country, string city, string zipCode, string street, int house, int? apartment)
        {
            string adress = string.Format("{0}, г.{1}, {2}, ул.{3}, дом №{4}",
                country, city, zipCode, street, house);
            if (apartment != null)
            {
                adress = string.Format("{0}, кварира №{1}", adress, apartment);
            }
            return adress;
        }
        //==========================================================



        //==========================================================
        // вспомогательный метод
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        //==========================================================



        //==========================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}