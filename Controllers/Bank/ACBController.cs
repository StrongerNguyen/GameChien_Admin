using FT_Admin.Models;
using FT_Admin.Models.ACB;
using FT_Admin.Models.API;
using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [Authorize]
    public class ACBController : Controller
    {
        // GET: TPBankTransfer
        public async Task<ActionResult> Transfer(int bankAccountId)
        {
            List<BankNapas> bankList = new List<BankNapas>();
            tblBankAccount bankAccount = new tblBankAccount();
            try
            {
                using (var db = new BankAPIEntities())
                {
                    bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return RedirectToAction("Index", "Home");

                    ACBBankCodeModel aCBBankCode = await ACBAPI.GetBankCode(bankAccount.UserName, bankAccount.Password);
                    if (aCBBankCode != null && aCBBankCode.data != null)
                    {
                        bankList = aCBBankCode.data;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBController/Transfer", ex);
            }

            ViewBag.bankAccount = bankAccount;
            ViewBag.BankList = bankList;
            return View();
        }
        public async Task<JsonResult> GetOTP(int bankAccountId, string napasBankCode, string stkNhan, int money, string note)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    ACBTransferModel aCBTransfer;
                    //lấy thông tin người nhận
                    if (string.IsNullOrEmpty(napasBankCode))
                    {
                        //gửi trong bank
                        aCBTransfer = await ACBAPI.CreateTransferInBank(bankAccount.UserName, bankAccount.Password, bankAccount.AccountNumber, stkNhan, money, note);
                    }
                    else
                    {
                        aCBTransfer = await ACBAPI.CreateTransfer247(bankAccount.UserName, bankAccount.Password, bankAccount.AccountNumber, napasBankCode, stkNhan, money, note);
                    }
                    if (aCBTransfer == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (aCBTransfer.success)
                    {
                        return Json(new { success = true, data = aCBTransfer.data.uuid }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = aCBTransfer.message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBController/GetOTP", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> ConfirmOTP(int bankAccountId, string uuid, string otp)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    ACBConfirmOTPModel aCBConfirmOTP = await ACBAPI.ConfirmTransfer(bankAccount.UserName, bankAccount.Password, bankAccount.AccountNumber, uuid, otp);
                    if (aCBConfirmOTP == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (aCBConfirmOTP.success) return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    else return Json(new { success = false, message = aCBConfirmOTP.message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBController/ConfirmOTP", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}