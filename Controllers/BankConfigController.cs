using FT_Admin.Models;
using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("BankManage")]
    public class BankConfigController : Controller
    {
        // GET: BankConfig
        public ActionResult Index()
        {
            using (var db = new BankAPIEntities())
            {
                return View(db.tblBankConfigs.ToList());
            }
        }
        public ActionResult Detail(int? id)
        {
            tblBankConfig bankConfig = null;
            if (id != null)
            {
                using (var db = new BankAPIEntities())
                {
                    bankConfig = db.tblBankConfigs.FirstOrDefault(t => t.Id == id);
                }
            }
            return PartialView("_Detail", bankConfig);
        }
        public async Task<ActionResult> Update(tblBankConfig bankConfig)
        {
            using (var db = new BankAPIEntities())
            {
                if (bankConfig.Id == 0)
                {
                    db.tblBankConfigs.Add(bankConfig);
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("AddBankConfig", bankConfig, User.Identity.Name);
                }
                else
                {
                    var bankConfigFromDB = db.tblBankConfigs.FirstOrDefault(t => t.Id == bankConfig.Id);
                    bankConfigFromDB.BankName = bankConfig.BankName;
                    bankConfigFromDB.isUseOTPInLogin = bankConfig.isUseOTPInLogin;
                    bankConfigFromDB.isUseAccountNumberInLogin = bankConfig.isUseAccountNumberInLogin;
                    bankConfigFromDB.isUseSMS = bankConfig.isUseSMS;
                    bankConfigFromDB.isUseAPI = bankConfig.isUseAPI;
                    bankConfigFromDB.isUseNotify = bankConfig.isUseNotify;
                    bankConfigFromDB.isActive = bankConfig.isActive;
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("UpdateBankConfig", bankConfigFromDB, User.Identity.Name);
                }
                return RedirectToAction("Index", "BankConfig");
            }
        }
    }
}