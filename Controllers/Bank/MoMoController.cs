using FT_Admin.Models;
using FT_Admin.Models.API;
using FT_Admin.Models.Data;
using FT_Admin.Models.MoMo;
using FT_Admin.Models.MoMo.BankList;
using FT_Admin.Models.MoMo.ConfirmOTP;
using FT_Admin.Models.MoMo.ConfirmTransferToMoMo;
using FT_Admin.Models.MoMo.CreateTransferToBank;
using FT_Admin.Models.MoMo.MoMoConfirmTransferToBank;
using FT_Admin.Models.MoMo.TransferToMoMo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [Authorize]
    public class MoMoController : Controller
    {
        public ActionResult Transfer(int bankAccountId)
        {
            tblBankAccount bankAccount = new tblBankAccount();
            using (var db = new BankAPIEntities())
            {
                bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                if (bankAccount == null) return RedirectToAction("Index", "Home");
            }
            ViewBag.bankAccount = bankAccount;
            return View();
        }
        public async Task<ActionResult> getBankList(int bankAccountId)
        {
            List<NapasBank> napasBanks = new List<NapasBank>();
            using (var db = new BankAPIEntities())
            {
                tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                if (bankAccount == null) return RedirectToAction("Index", "Home");

                MoMoBankListModel tPBankBankList = await MoMoAPI.getBankList(bankAccount);
                if (tPBankBankList != null && tPBankBankList.napasBanks != null)
                {
                    napasBanks = tPBankBankList.napasBanks.OrderBy(t => t.shortBankName).ToList();
                }
                else
                {
                    return Json(new { success = false, message = "Không lấy được danh sách ngân hàng!" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, data = napasBanks }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CreateTransferToMoMo(int bankAccountId, string phone, int amount, string note)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);

                    TransferToMoMoModel transferToMoMo = await MoMoAPI.createTransferToMoMo(bankAccount, phone, amount, note);
                    if (transferToMoMo == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                    if (transferToMoMo.result)
                    {
                        return Json(new { success = true, data = new { name = transferToMoMo.momoMsg.replyMsgs[0].tranHisMsg.partnerName, id = transferToMoMo.momoMsg.replyMsgs[0].ID } }, JsonRequestBehavior.AllowGet);
                    }
                    else return Json(new { success = false, message = transferToMoMo.errorDesc }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoController/CreateTransferToMoMo", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> ConfirmTransferToMoMo(int bankAccountId, string id)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);
                    else
                    {
                        ConfirmTransferToMoMoModel confirmTransferToMoMo = await MoMoAPI.confirmTransferToMoMo(bankAccount, id);
                        if (confirmTransferToMoMo == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                        if (confirmTransferToMoMo.result)
                        {
                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }
                        else return Json(new { success = false, message = confirmTransferToMoMo.errorDesc }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoController/ConfirmTransferToMoMo", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> CreateTransferToBank(int bankAccountId, string bankObject, string cardNumber, int amount, string note)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);
                    else
                    {
                        MoMoCreateTransferToBank transferToBank = await MoMoAPI.createTransferToBank(bankAccount, bankObject, cardNumber, "", amount, "", note);
                        if (transferToBank == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                        if (transferToBank.result)
                        {
                            return Json(new { success = true, data = new { name = transferToBank.momoMsg.tranHisMsg.ownerName, id = transferToBank.momoMsg.tranHisMsg.ID, extras = transferToBank.momoMsg.tranHisMsg.extras } }, JsonRequestBehavior.AllowGet);
                        }
                        else return Json(new { success = false, message = transferToBank.errorDesc }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoController/CreateTransferToBank", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> ConfirmTransferToBank(int bankAccountId, string id, string bankObject, string cardNumber, int amount, string note, string extras)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId && t.isActive == true);
                    if (bankAccount == null) return Json(new { success = false, message = "Tài khoản này đang không hoạt động. vui lòng đăng nhập lại" }, JsonRequestBehavior.AllowGet);
                    else
                    {
                        MoMoConfirmTransferToBankModel moMoConfirmTransferToBank = await MoMoAPI.confirmTransferToBank(bankAccount, id, bankObject, cardNumber, "", amount, "", note, 0, extras);
                        if (moMoConfirmTransferToBank == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
                        if (moMoConfirmTransferToBank.result)
                        {
                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }
                        else return Json(new { success = false, message = moMoConfirmTransferToBank.errorDesc }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoController/ConfirmTransferToBank", ex);
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}