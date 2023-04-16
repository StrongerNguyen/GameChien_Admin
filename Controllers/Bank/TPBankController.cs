using FT_Admin.Models;
using FT_Admin.Models.API;
using FT_Admin.Models.Data;
using FT_Admin.Models.TPBank;
using FT_Admin.Models.TPBankBankList;
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
    public class TPBankController : Controller
    {
        public async Task<ActionResult> Transfer(int bankAccountId)
        {
            List<Banksnapas> bankList = new List<Banksnapas>();
            tblBankAccount bankAccount = new tblBankAccount();
            try
            {
                using (var db = new BankAPIEntities())
                {
                    bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return RedirectToAction("Index", "Home");

                    TPBankBankListModel tPBankBankList = await TPBankAPI.getBankList(bankAccount.Token);
                    if (tPBankBankList != null && tPBankBankList.banksnapas != null)
                    {
                        bankList = tPBankBankList.banksnapas;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankController/Transfer", ex);
            }
            ViewBag.bankAccount = bankAccount;
            ViewBag.BankList = bankList;
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
                    //lấy thông tin người nhận
                    TPBankAccountBankInfoModel tPBankAccountBankInfo = await TPBankAPI.getAccountBankInfo(bankAccount.Token, bankAccount.AccountNumber, stkNhan, bankid);
                    if (tPBankAccountBankInfo == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (tPBankAccountBankInfo.status != 0) return Json(new { success = false, message = tPBankAccountBankInfo.message }, JsonRequestBehavior.AllowGet);
                    if (tPBankAccountBankInfo.errorMessage != null) return Json(new { success = false, message = tPBankAccountBankInfo.errorMessage.messages.vn }, JsonRequestBehavior.AllowGet);
                    //lấy otp
                    TPBankGetOTPModel tPBankGetOTP = await TPBankAPI.getOTP(bankAccount.Token, bankAccount.AccountNumber, stkNhan, bankid, money, note, new JavaScriptSerializer().Serialize(tPBankAccountBankInfo.creditorInfo));
                    if (tPBankGetOTP == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            name = tPBankAccountBankInfo.creditorInfo.name,
                            id = tPBankGetOTP.id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankController/GetOTP", ex);

                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> ConfirmOTP(int bankAccountId, string id, string otp)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    TPBankConfirmOTPModel tPBankConfirmOTP = await TPBankAPI.confirmOTP(bankAccount.Token, bankAccount.AccountNumber, id, otp);
                    if (tPBankConfirmOTP == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (tPBankConfirmOTP.errorMessage != null) return Json(new { success = false, message = tPBankConfirmOTP.errorMessage.messages.vn }, JsonRequestBehavior.AllowGet);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankController/ConfirmOTP", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}