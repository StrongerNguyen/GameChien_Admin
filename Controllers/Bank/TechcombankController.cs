using FT_Admin.Models;
using FT_Admin.Models.API;
using FT_Admin.Models.Data;
using FT_Admin.Models.Techcombank;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FT_Admin.Controllers
{
    [Authorize]
    public class TechcombankController : Controller
    {
        // GET: TechcombankTransfer

        public async Task<ActionResult> Transfer(int bankAccountId)
        {
            List<BankList> bankLists = new List<BankList>();
            tblBankAccount bankAccount = new tblBankAccount();
            try
            {
                using (var db = new BankAPIEntities())
                {
                    bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return RedirectToAction("Index", "Home");
                    TechcombankBankListModel techcombankBankList = await TechcombankAPI.getBankList(bankAccount.UserName);
                    if (techcombankBankList != null && techcombankBankList.success)
                    {
                        bankLists = techcombankBankList.BankList;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankController/Transfer", ex);
            }

            ViewBag.bankAccount = bankAccount;
            ViewBag.BankList = bankLists;
            return View();
        }
        public async Task<JsonResult> GetOTP(int bankAccountId, string bankid, string stkNhan, string money, string note)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    TechcombankOTPModel techcombankOTP = await TechcombankAPI.getOTP(bankAccount.UserName, bankAccount.AccountNumber, bankid, stkNhan, int.Parse(money), note);
                    if (techcombankOTP == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);

                    if (techcombankOTP.success)
                    {
                        return Json(new { success = true, data = techcombankOTP }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = techcombankOTP.message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankController/GetOTP", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> ConfirmOTP(int bankAccountId, string systemid, string otp)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    TeckcombankConfirmOTPModel teckcombankConfirmOTP = await TechcombankAPI.confirmOTP(bankAccount.UserName, otp, systemid);
                    if (teckcombankConfirmOTP == null || teckcombankConfirmOTP.Status == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (teckcombankConfirmOTP.Status.code == 0) return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    else return Json(new { success = false, message = teckcombankConfirmOTP.Status.value }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankController/ConfirmOTP", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}