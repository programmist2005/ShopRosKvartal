using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ShopRosKvartal.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using ShopRosKvartal.Models.SiteTools;
using ShopRosKvartal.LibraryHelperClasses.SMTPServer;
using ShopRosKvartal.Models.VerificationCodes;

namespace ShopRosKvartal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        //============================================================
        // используется в проекте
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        //============================================================



        //============================================================
        // используется в проекте
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        //============================================================



        //============================================================
        // используется в проекте
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //============================================================

        //============================================================
        // используется в проекте
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        //============================================================



        //============================================================
        // используется в проекте
        // регистрация пользователей
        // Метод GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //============================================================

        //============================================================
        // используется в проекте
        // регистрация пользователей
        // Метод POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // выбираем роль
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                    var role = roleManager.FindByName("Покупатель");
                    
                    if (role != null)
                    {
                        // добавляем для пользователя роль
                        UserManager.AddToRole(user.Id, role.Name);
                    }

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // отправка письма с активацие почтового ящика
                    bool sendResult = GenerateEmailConfirmation(user.Id);

                    // продолжение заполнения профиля пользователя
                    return RedirectToAction("Create", "Profile", new { returnUrl });
                }
                AddErrors(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        //============================================================



        //============================================================
        // используется в проекте
        // подтверждение почтового ящика пользователя
        // отправка письма
        public bool GenerateEmailConfirmation(string id)
        {
            ApplicationUser user = UserManager.FindById(id);

            // загрузка настроек smtp
            ToolsSMTPSetting smtpSetting = db.ToolsSMTPSettings.FirstOrDefault();
            // генерация кода
            Guid codeGUID = Guid.NewGuid();
            string code = codeGUID.ToString();
            user.CodesVerificationEmail = new CodesVerificationEmail();
            user.CodesVerificationEmail.Code = code;
            UserManager.Update(user);
            string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

            // отправка письма
            string body = "Подтвердите ваш e-mail адрес перейдя <a href=\"" + callbackUrl + "\">по этой ссылке</a>";
            string title = "Подтверждение E-mail адреса";
            bool sendResult = MailSender.SendRmail(smtpSetting, code, title, body, user.Email);

            return sendResult;
        }
        //============================================================

        //============================================================
        // используется в проекте
        // подтверждение почтового ящика пользователя
        // проверка кода из ссылки письма
        [AllowAnonymous]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("ConfirmationEmailError");
            }

            // поиск пользователя
            ApplicationUser user = UserManager.FindById(userId);
            if (user == null)
            {
                // пользователь не найден
                return RedirectToAction("Error404", "Errors");
            }

            // проверка подтверждения почты
            if (!user.EmailConfirmed)
            {
                if (user.CodesVerificationEmail.Code != code)
                {
                    return View("ConfirmationEmailFailed");
                }
                // установка подтверждения почты
                user.EmailConfirmed = true;
                user.CodesVerificationEmail = null;
                UserManager.Update(user);
                return View("ConfirmationEmailSuccess");
            }
            return View("ConfirmedEmail");
        }
        //============================================================



        //============================================================
        // используется в проекте
        // восстановления доступа - сброс пароля
        // Метод GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        //============================================================

        //============================================================
        // используется в проекте
        // восстановления доступа - сброс пароля
        // Метод POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.UserName);

                // check email address
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id))
                    || (await UserManager.GetEmailAsync(user.Id)) != model.Email)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // отправка письма с активацие почтового ящика
                bool sendResult = GeneratePasswordReset(user.Id);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //============================================================



        //============================================================
        // используется в проекте
        // сброс пароля - отправка письма с кодом
        // отправка письма
        public bool GeneratePasswordReset(string id)
        {
            ApplicationUser user = UserManager.FindById(id);

            // загрузка настроек smtp
            ToolsSMTPSetting smtpSetting = db.ToolsSMTPSettings.FirstOrDefault();

            // генерация кода
            Guid codeGUID = Guid.NewGuid();
            string code = codeGUID.ToString();

            user.CodesResetPassword = new CodesResetPassword();
            user.CodesResetPassword.Code = code;
            UserManager.Update(user);

            string callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

            // отправка письма
            string body = "Для восстановления доступа к вашему аккаунту перейдите <a href=\"" + callbackUrl + "\">по этой ссылке</a>";
            string title = "Восстановление доступа";
            bool sendResult = MailSender.SendRmail(smtpSetting, code, title, body, user.Email);

            return sendResult;
        }
        //============================================================



        //============================================================
        // используется в проекте
        // сброс пароля - информация об отправленном письме
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        //============================================================



        //============================================================
        // используется в проекте
        // сброс пароля
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {

                return View("ResetPasswordError");
            }

            // поиск пользователя
            ApplicationUser user = UserManager.FindById(userId);
            if (user == null)
            {
                // пользователь не найден
                return RedirectToAction("Error404", "Errors");
            }

            // проверка подтверждения почты
            if (user.CodesResetPassword.Code != code)
            {

                return View("ResetPasswordFailed");
            }
            // установка подтверждения почты
            user.CodesVerificationEmail = null;
            UserManager.Update(user);
            return View();
        }
        //============================================================

        //============================================================
        // используется в проекте
        // сброс пароля
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null || (await UserManager.GetEmailAsync(user.Id)) != model.Email || user.CodesResetPassword.Code != model.Code)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            UserManager.RemovePassword(user.Id);
            var result = await UserManager.AddPasswordAsync(user.Id, model.Password);
            if (result.Succeeded)
            {
                user.CodesResetPassword = null;
                UserManager.Update(user);
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }
        //============================================================



        //============================================================
        // используется в проекте
        // сброс пароля - информация об успешном сбросе старого и установке нового
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        //============================================================



        //======================================================
        // Используется в проекте 
        // выход из сессии
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        //==========================================================



        //============================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}