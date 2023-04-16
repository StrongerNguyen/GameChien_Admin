using FT_Admin.Models;
using FT_Admin.Models.API;
using FT_Admin.Models.Data;
using FT_Admin.Models.Vietcombank;
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
    public class VietcombankController : Controller
    {
        // GET: Vietcombank
        public async Task<ActionResult> Transfer(int bankAccountId)
        {
            List<VietcombankBankList> bankList = new List<VietcombankBankList>();
            tblBankAccount bankAccount = new tblBankAccount();
            try
            {
                using (var db = new BankAPIEntities())
                {
                    bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return RedirectToAction("Index", "Home");
                    VietcombankBankListModel vietcombankBankList = await VietcombankAPI.getBankList();
                    if (vietcombankBankList != null && vietcombankBankList.banks != null)
                    {
                        bankList = vietcombankBankList.banks;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankController/Transfer", ex);
            }

            ViewBag.bankAccount = bankAccount;
            ViewBag.BankList = bankList;
            return View();
        }
        public async Task<JsonResult> GetOTP(int bankAccountId, string bankcode, string stkNhan, string money, string note)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    VietcombankOTPModel vietcombankOTP = await VietcombankAPI.getOTP(bankAccount.UserName, bankAccount.Password, bankcode, stkNhan, money, note);
                    if (vietcombankOTP == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (vietcombankOTP.success)
                    {
                        return Json(new
                        {
                            success = true,
                            data = new
                            {
                                name = vietcombankOTP.transaction.creditAccountName,
                                tranId = vietcombankOTP.transaction.tranId
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else return Json(new { success = false, message = vietcombankOTP.message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankController/GetOTP", ex);

                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> ConfirmOTP(int bankAccountId, string tranId, string otp)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    VietcombankConfirmOTPModel vietcombankConfirmOTP = await VietcombankAPI.confirmOTP(bankAccount.UserName, bankAccount.Password, tranId, otp);
                    if (vietcombankConfirmOTP == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (vietcombankConfirmOTP.success) return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    else return Json(new { success = false, message = vietcombankConfirmOTP.message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankController/ConfirmOTP", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}