using FT_Admin.Models;
using FT_Admin.Models.API;
using FT_Admin.Models.Data;
using FT_Admin.Models.Vietinbank;
using FT_Admin.Models.Vietinbank.CreateTransferInBank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [Authorize]
    public class VietinbankController : Controller
    {
        // GET: Vietinbank
        public async Task<ActionResult> Transfer(int? bankAccountId)
        {
            if (bankAccountId == null) return RedirectToAction("Index", "Home");
            List<CodeMapping> bankList = new List<CodeMapping>();
            tblBankAccount bankAccount = new tblBankAccount();
            try
            {
                using (var db = new BankAPIEntities())
                {
                    bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return RedirectToAction("Index", "Home");
                    VietinbankBankListModel vietinbankBankList = await VietinbankAPI.getBankList(bankAccount.sessionId);
                    if (vietinbankBankList != null && !vietinbankBankList.error && vietinbankBankList.codeMapping != null)
                    {
                        bankList = vietinbankBankList.codeMapping;
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
        public async Task<ActionResult> getAccountDetail(int bankAccountId, string bankCode, string stkNhan)
        {
            try
            {
                tblBankAccount bankAccount = new tblBankAccount();
                using (var db = new BankAPIEntities())
                {
                    bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return RedirectToAction("Index", "Home");
                    if (string.IsNullOrEmpty(bankCode))
                    {
                        //chuyen khoan cung bank
                        VietinbankAccountInfoInBankModel vietinbankAccountInfoInBank = await VietinbankAPI.getAccountDetailInBank(bankAccount.sessionId, stkNhan);
                        if (vietinbankAccountInfoInBank == null || vietinbankAccountInfoInBank.error) return Json(new { success = false, message = (vietinbankAccountInfoInBank?.errorMessage ?? "Lỗi API") }, JsonRequestBehavior.AllowGet);
                        else
                        {
                            return Json(new
                            {
                                success = true,
                                data = new
                                {
                                    name = vietinbankAccountInfoInBank.toAccountName
                                }
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //chuyen khoan khac bank
                        VietinbankAccountInfoOutBankModel vietinbankAccountInfoOutBank = await VietinbankAPI.getAccountDetailOutBank(bankAccount.sessionId, bankAccount.AccountNumber, stkNhan, bankCode);
                        if (vietinbankAccountInfoOutBank == null || vietinbankAccountInfoOutBank.error) return Json(new { success = false, message = (vietinbankAccountInfoOutBank?.errorMessage ?? "Lỗi API") }, JsonRequestBehavior.AllowGet);
                        else
                        {
                            return Json(new
                            {
                                success = true,
                                data = new
                                {
                                    name = vietinbankAccountInfoOutBank.beneficiaryName
                                }
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankController/getAccountDetail", ex);
                return Json(new { success = false, message = "Lỗi API" }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> createTransfer(int bankAccountId, string bankCode, string stkNhan, int amount, string note)
        {
            try
            {
                tblBankAccount bankAccount = new tblBankAccount();
                using (var db = new BankAPIEntities())
                {
                    bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return RedirectToAction("Index", "Home");
                    if (string.IsNullOrEmpty(bankCode))
                    {
                        //chuyen khoan cung bank
                        VietinbankCreateTransferInBankModel vietinbankCreateTransferInBank = await VietinbankAPI.createTransferInBank(bankAccount.sessionId, bankAccount.AccountNumber, bankAccount.accountType, bankAccount.bsb, bankAccount.currencyCode, stkNhan, amount, note);
                        if (vietinbankCreateTransferInBank == null || vietinbankCreateTransferInBank.error) return Json(new { success = false, message = (vietinbankCreateTransferInBank?.errorMessage ?? "Lỗi API") }, JsonRequestBehavior.AllowGet);
                        else
                        {
                            return Json(new { success = true, message = vietinbankCreateTransferInBank.smsOtpTemplate }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //chuyen khoan khac bank
                        VietinbankCreateTransferOutBankModel vietinbankCreateTransferOutBank = await VietinbankAPI.createTransferOutBank(bankAccount.sessionId, bankAccount.AccountNumber, amount, note);
                        if (vietinbankCreateTransferOutBank == null || vietinbankCreateTransferOutBank.error) return Json(new { success = false, message = (vietinbankCreateTransferOutBank?.errorMessage ?? "Lỗi API") }, JsonRequestBehavior.AllowGet);
                        else
                        {
                            return Json(new { success = true, message = vietinbankCreateTransferOutBank.smsOtpTemplate }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankController/createTransfer", ex);
                return Json(new { success = false, message = "Lỗi API" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}