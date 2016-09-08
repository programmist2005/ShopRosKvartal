using ShopRosKvartal.Models;
using ShopRosKvartal.Models.SiteTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ShopRosKvartal.Areas.AdminPanel.Controllers
{
    //==========================================================
    //=============== Контроллер Настройки сайта ===============
    //=============== Доступен для администраторов =============

    [Authorize(Roles = "Администратор")]
    public class SiteToolsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //==========================================================



        //==========================================================
        //========= Вывод настроек почтового сервиса (SMTP) ========
        public ActionResult SMTPSettings()
        {
            ToolsSMTPSetting smtp = db.ToolsSMTPSettings.FirstOrDefault();
            return View(smtp);
        }
        //==========================================================



        //==========================================================
        //==== Редактирование настроек почтового сервиса (SMTP) ====
        //======================= Метод GET ========================
        public ActionResult SMTPSettingsEdit()
        {
            ToolsSMTPSetting smtp = db.ToolsSMTPSettings.FirstOrDefault();
            return View(smtp);
        }
        //==========================================================
        
        //==========================================================
        //==== Редактирование настроек почтового сервиса (SMTP) ====
        //======================= Метод POST =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMTPSettingsEdit(ToolsSMTPSetting model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SMTPSettings", "SiteTools", null);
            }
            return View(model);
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
        //==========================================================
    }
}