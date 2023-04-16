using FT_Admin.Hubs;
using FT_Admin.Models;
using FT_Admin.Models.Data;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using FT_Admin.Models.ViewModel;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("TransactionManage")]
    public class TransactionController : Controller
    {
        // GET: Home
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll(string FromFilter, bool? StatusFilter, string MoneyFilter, string TimeFilter, DateTime? DateFilter, int? curPage = 1)
        {
            try
            {
                //new RealtimeHub().updateCountNotify();
                //new RealtimeHub().updateCountTicket();
                using (var db = new GameChienEntities())
                {
                    string query = @"   SELECT t.*, u.UserName, d.Id as DepositId, ROW_NUMBER() over (order by t.CreatedTime desc) as r
                                        FROM tblTransaction t
                                        LEFT JOIN tblUser u on t.LastUpdateBy = u.Id
                                        LEFT JOIN tblDeposit d on t.Id = d.TransactionId
                            ";
                    string where = "WHERE 1 = 1 ";
                    if (!string.IsNullOrEmpty(FromFilter))
                    {
                        where += $" AND [GetBy] like '%{FromFilter}%' ";
                    }
                    if (StatusFilter != null)
                    {
                        where += $" AND [Status] = {StatusFilter} ";
                    }

                    if (!string.IsNullOrEmpty(MoneyFilter))
                    {
                        where += $" AND Amount like '%{MoneyFilter}%' ";
                    }
                    if (!string.IsNullOrEmpty(TimeFilter))
                    {
                        where += $" AND CONVERT(VARCHAR(5),t.CreatedTime,108) like N'%{TimeFilter}%' ";
                    }
                    if (DateFilter != null)
                    {
                        where += $" AND (TRY_CONVERT(date,t.CreatedTime,103) = Convert(date,'{DateFilter.Value:yyyy-MM-dd}')) ";
                    }
                    string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

                    query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by CreatedTime desc";
                    string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblTransaction t
                                            {1}
                                        ", PageSize, where);

                    List<TransactionViewModel> dataTransaction = db.Database.SqlQuery<TransactionViewModel>(query).ToList();
                    ViewBag.maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                    ViewBag.curPage = curPage;
                    return PartialView("_TransactionTable", dataTransaction);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetById(Guid Id, string FromFilter, bool? StatusFilter, string MoneyFilter, string TimeFilter, DateTime? DateFilter)
        {
            using (var db = new GameChienEntities())
            {
                var transactionFromDB = db.tblTransactions.FirstOrDefault(x => x.Id == Id);

                if (
                    transactionFromDB != null
                    && (DateFilter == null || transactionFromDB.CreatedTime.ToString("dd/MM/yyyy").Equals(DateFilter.Value.ToString("dd/MM/yyyy")))
                    && (string.IsNullOrEmpty(TimeFilter) || transactionFromDB.CreatedTime.ToString("hh:mm:ss").Contains(TimeFilter))
                    && (string.IsNullOrEmpty(MoneyFilter) || transactionFromDB.Amount.ToString().Contains(MoneyFilter))
                    && (string.IsNullOrEmpty(FromFilter) || transactionFromDB.GetBy.Equals(FromFilter))
                )
                {
                    return PartialView("_TransactionRow", new TransactionViewModel()
                    {
                        Id = transactionFromDB.Id,
                        GetBy = transactionFromDB.GetBy,
                        Device = transactionFromDB.Device,
                        BankName = transactionFromDB.BankName,
                        TransactionDate = transactionFromDB.TransactionDate,
                        TransactionTime = transactionFromDB.TransactionTime,
                        CD = transactionFromDB.CD,
                        Amount = transactionFromDB.Amount,
                        Description = transactionFromDB.Description,
                        PhoneNumber = transactionFromDB.PhoneNumber,
                        CreatedTime = transactionFromDB.CreatedTime,
                        Status = transactionFromDB.Status,
                        LastUpdateBy = transactionFromDB.LastUpdateBy,
                        LastUpdateByUserName = transactionFromDB.tblUser?.UserName ?? "",
                        LastUpdateTime = transactionFromDB.LastUpdateTime,
                        DepositId = transactionFromDB.tblDeposits.FirstOrDefault(t => t.TransactionId == transactionFromDB.Id)?.Id ?? null
                    });
                }
                else return null;
            }
        }
        //[CustomAuthorize("0", "actionTransaction")]
        //public async Task<JsonResult> UpdateStatus(int? Id, string Status)
        //{
        //    new RealtimeHub().updateCountNotify();
        //    try
        //    {
        //        if (Id == null) return Json(new { success = false, message = "ID is not null" }, JsonRequestBehavior.AllowGet);
        //        else
        //        {
        //            using (var db = new BankAPIEntities())
        //            {
        //                tblTransaction transactionFromDB = db.tblTransactions.FirstOrDefault(t => t.Id == Id);
        //                if (transactionFromDB == null)
        //                {
        //                    return Json(new { success = false, message = "Message is not exist!" }, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    transactionFromDB.Status = Status;
        //                    transactionFromDB.UpdateBy = User.Identity.Name;
        //                    db.SaveChanges();
        //                    await Logging.LogChangeAsync("UpdateStatusTransaction", new
        //                    {
        //                        Id = transactionFromDB.Id,
        //                        Amount = transactionFromDB.Amount,
        //                        BankName = transactionFromDB.BankName,
        //                        CD = transactionFromDB.CD,
        //                        DateTime = transactionFromDB.DateTime,
        //                        Description = transactionFromDB.Description,
        //                        From = transactionFromDB.From,
        //                        PCTime = transactionFromDB.PCTime,
        //                        PhoneNumber = transactionFromDB.PhoneNumber,
        //                        Reference = transactionFromDB.Reference,
        //                        Status = transactionFromDB.Status,
        //                        TransactionDate = transactionFromDB.TransactionDate,
        //                        UpdateBy = transactionFromDB.UpdateBy
        //                    }, User.Identity.Name);
        //                    new RealtimeHub().updateTransaction(transactionFromDB.Id);
        //                    return Json(new { success = true, message = "Update status success!" }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public JsonResult LogoutAccount(int bankAccountId)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblBankAccount bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId);
        //            if (bankAccount != null)
        //            {
        //                if (bankAccount.isActive)
        //                {
        //                    bankAccount.isActive = false;
        //                    db.SaveChanges();
        //                    return Json(new { success = true, message = "Đăng xuất thành công." }, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    db.tblBankAccounts.Remove(bankAccount);
        //                    db.SaveChanges();
        //                    return Json(new { success = true, message = "Xóa tài khoản thành công." }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = "Không tìm thấy tài khoản này. Vui lòng thử lại" }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Log.Error(ex);
        //        return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpGet]
        //public ActionResult LoginBankAccountAPI(int? bankAccountId)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblBankAccount bankAccount = null;
        //            if (bankAccountId != null)
        //            {
        //                bankAccount = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId);
        //            }
        //            ViewBag.bankListConfig = db.tblBankConfigs.Where(t => t.isActive && t.isUseAPI).Select(t => new BankConfigModel()
        //            {
        //                BankName = t.BankName,
        //                Id = t.Id,
        //                isActive = t.isActive,
        //                isUseAccountNumberInLogin = t.isUseAccountNumberInLogin,
        //                isUseOTPInLogin = t.isUseOTPInLogin,
        //                Logo = t.Logo
        //            }).ToList();
        //            return PartialView("_AddBankAccount", bankAccount);
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Log.Error(ex);
        //        return Json(new { success = false, message = "Lỗi: Không lấy được danh sách ngân hàng" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpGet]
        //public async Task<JsonResult> GetOTPLogin(int bankConfigId, string userName, string passWord, string accountNumber)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblBankConfig bankConfig = db.tblBankConfigs.FirstOrDefault(t => t.Id == bankConfigId);
        //            if (bankConfig == null)
        //            {
        //                return Json(new { success = false, message = "Lỗi: Không tồn tại Loại tài khoản này" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                //check xem tài khoản này đã tồn tại chưa
        //                if (db.tblBankAccounts.Count(t => t.UserName.Equals(userName) && t.BankConfigId == bankConfigId && t.isActive) > 0) return Json(new { success = false, message = "Tài khoản này đang hoạt động. Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
        //                if (bankConfig.BankName.Equals("MoMo"))
        //                {
        //                    MoMoGetOTPModel moMoGetOTP = await MoMoAPI.getOTP(userName, passWord);
        //                    if (moMoGetOTP == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
        //                    else
        //                    {
        //                        if (moMoGetOTP.result)
        //                        {
        //                            return Json(new { success = true, message = "Yêu cầu OTP thành công!", data = new JavaScriptSerializer().Serialize(moMoGetOTP.ext) }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        else return Json(new { success = false, message = moMoGetOTP.errorDesc }, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                else
        //                {
        //                    return Json(new { success = false, message = "Lỗi: Chưa xử lý Lấy OTP cho loại tài khoản này!" }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Log.Error(ex);
        //        return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpPost]
        //public async Task<JsonResult> LoginAccount(int? bankAccountId, int bankConfigId, string userName, string passWord, string accountNumber, string otp, string ext)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblBankConfig bankConfig = db.tblBankConfigs.FirstOrDefault(t => t.Id == bankConfigId);
        //            if (bankConfig == null)
        //            {
        //                return Json(new { success = false, message = "Lỗi: Không tồn tại Loại tài khoản này" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                //check username
        //                if (db.tblBankAccounts.Count(t => t.UserName.Equals(userName) && t.BankConfigId == bankConfigId && t.isActive) > 0) return Json(new { success = false, message = "Tài khoản này đang hoạt động. Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
        //                if (bankConfig.BankName.Equals("Vietcombank"))
        //                {
        //                    VCBTransactionResultModel vCBTransactionResult = await VietcombankAPI.getHistoryTransactions(userName, passWord, DateTime.Now, DateTime.Now);
        //                    if (vCBTransactionResult == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);

        //                    if (vCBTransactionResult.success)
        //                    {
        //                        if (bankAccountId == null)
        //                        {
        //                            //lưu lại trong db
        //                            db.tblBankAccounts.Add(new tblBankAccount()
        //                            {
        //                                BankName = "Vietcombank",
        //                                UserName = userName,
        //                                Password = passWord,
        //                                isActive = true,
        //                                AccountNumber = "",
        //                                LoginTime = DateTime.Now,
        //                                BankConfigId = bankConfigId
        //                            });
        //                            db.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId.Value).isActive = true;
        //                            db.SaveChanges();
        //                        }
        //                        return Json(new { success = true, message = "Đăng nhập thành công!" }, JsonRequestBehavior.AllowGet);
        //                    }
        //                    else return Json(new { success = false, message = vCBTransactionResult.message }, JsonRequestBehavior.AllowGet);
        //                }
        //                else if (bankConfig.BankName.Equals("Techcombank"))
        //                {
        //                    TechcombankLoginModel techcombankLogin = await TechcombankAPI.Login(userName, passWord);
        //                    if (techcombankLogin == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
        //                    if (techcombankLogin.success)
        //                    {
        //                        //get Wallet
        //                        TechcombankWalletsModel techcombankWallets = await TechcombankAPI.getWallet(userName);
        //                        if (techcombankWallets == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
        //                        if (bankAccountId == null)
        //                        {
        //                            //lưu lại trong db

        //                            db.tblBankAccounts.Add(new tblBankAccount()
        //                            {
        //                                BankName = "Techcombank",
        //                                UserName = userName,
        //                                Password = passWord,
        //                                isActive = true,
        //                                AccountNumber = techcombankWallets.walletEntries[0].bankAccount.accountNumber,
        //                                LoginTime = DateTime.Now,
        //                                BankConfigId = bankConfigId
        //                            });
        //                            db.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId.Value).isActive = true;
        //                            db.SaveChanges();
        //                        }
        //                        return Json(new { success = true, message = "Đăng nhập thành công!" }, JsonRequestBehavior.AllowGet);
        //                    }
        //                    else
        //                    {
        //                        return Json(new { success = false, message = techcombankLogin.message }, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                else if (bankConfig.BankName.Equals("TPBank"))
        //                {
        //                    TPBankLoginModel tPBankLogin = await TPBankAPI.Login(userName, passWord);
        //                    if (tPBankLogin == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
        //                    if (tPBankLogin.error != null) return Json(new { success = false, message = "Bạn đã đăng nhập sai nhiều lần. Vui lòng thử lại sau." }, JsonRequestBehavior.AllowGet);
        //                    //check detail
        //                    TPBankDetailModel tPBankDetail = await TPBankAPI.getDetail(tPBankLogin.access_token, accountNumber);
        //                    if (tPBankDetail == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
        //                    if (tPBankDetail.errorMessage == null)
        //                    {
        //                        if (bankAccountId == null)
        //                        {
        //                            //lưu lại trong db
        //                            db.tblBankAccounts.Add(new tblBankAccount()
        //                            {
        //                                BankName = "TPBank",
        //                                UserName = userName,
        //                                Password = passWord,
        //                                isActive = true,
        //                                Token = tPBankLogin.access_token.ToString(),
        //                                AccountNumber = accountNumber,
        //                                LoginTime = DateTime.Now,
        //                                BankConfigId = bankConfigId
        //                            });
        //                            db.SaveChanges();
        //                        }
        //                        else
        //                        {
        //                            db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId.Value).isActive = true;
        //                            db.SaveChanges();
        //                        }
        //                        return Json(new { success = true, message = "Đăng nhập thành công!" }, JsonRequestBehavior.AllowGet);
        //                    }
        //                    else return Json(new { success = false, message = tPBankDetail.errorMessage.messages.vn }, JsonRequestBehavior.AllowGet);
        //                }
        //                else if (bankConfig.BankName.Equals("ACB"))
        //                {
        //                    ACBTransactionsModel aCBTransactions = await ACBAPI.GetHistoryTransactions(userName, passWord, accountNumber, DateTime.Now.ToString("dd/MM/yyyy 00:00:00"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        //                    if (aCBTransactions == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
        //                    if (aCBTransactions.success)
        //                    {
        //                        //thanh cong
        //                        if (bankAccountId == null)
        //                        {
        //                            //lưu lại trong db
        //                            db.tblBankAccounts.Add(new tblBankAccount()
        //                            {
        //                                BankName = "ACB",
        //                                UserName = userName,
        //                                Password = passWord,
        //                                isActive = true,
        //                                AccountNumber = accountNumber,
        //                                LoginTime = DateTime.Now,
        //                                BankConfigId = bankConfigId
        //                            });
        //                        }
        //                        else
        //                        {
        //                            db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId.Value).isActive = true;
        //                        }
        //                        //cập nhật lịch sử giao dịch
        //                        foreach (var item in aCBTransactions.transactions)
        //                        {
        //                            if (db.tblTransactions.Count(t => t.BankName.Equals("ACB") && t.Reference == item.transactionNumber.ToString()) == 0)
        //                            {
        //                                var time = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(item.activeDatetime.ToString()));
        //                                db.tblTransactions.Add(new tblTransaction()
        //                                {
        //                                    TransactionDate = time.ToString("dd/MM/yyyy"),
        //                                    Reference = item.transactionNumber.ToString(),
        //                                    CD = item.type == "OUT" ? "-" : "+",
        //                                    Amount = item.amount,
        //                                    BankName = "ACB",
        //                                    Description = item.description,
        //                                    PCTime = time.ToString("HH:mm"),
        //                                    DateTime = time,
        //                                    Status = "null"
        //                                });
        //                            }
        //                        };
        //                        db.SaveChanges();
        //                        return Json(new { success = true, message = "Đăng nhập thành công." }, JsonRequestBehavior.AllowGet);
        //                    }
        //                    else
        //                    {
        //                        //that bai
        //                        return Json(new { success = false, message = aCBTransactions.message }, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                else if (bankConfig.BankName.Equals("MoMo"))
        //                {
        //                    if (string.IsNullOrEmpty(ext)) return Json(new { success = false, message = "OTP đã được tạo không đúng cách." }, JsonRequestBehavior.AllowGet);
        //                    JObject j = JObject.Parse(ext);
        //                    MoMoLoginModel moMoLogin = await MoMoAPI.ConfirmOTPasLogin(userName, passWord, otp, j["rkey"].ToString(), j["onesignal"].ToString(), j["imei"].ToString());
        //                    if (moMoLogin.result)
        //                    {
        //                        if (bankAccountId == null)
        //                        {
        //                            //lưu lại trong db
        //                            db.tblBankAccounts.Add(new tblBankAccount()
        //                            {
        //                                BankName = "MoMo",
        //                                UserName = userName,
        //                                Password = passWord,
        //                                isActive = true,
        //                                AccountNumber = accountNumber,
        //                                LoginTime = DateTime.Now,
        //                                OTP = otp,
        //                                BankConfigId = bankConfigId,
        //                                rkey = moMoLogin.ext.rkey,
        //                                onesignal = moMoLogin.ext.onesignal,
        //                                imei = moMoLogin.ext.imei,
        //                                ohash = moMoLogin.ext.ohash,
        //                                setupkey = moMoLogin.ext.setupkey,
        //                                requestkey = moMoLogin.ext.requestkey,
        //                                auth_token = moMoLogin.ext.auth_token
        //                            });
        //                        }
        //                        else
        //                        {
        //                            db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId.Value).isActive = true;
        //                        }
        //                        db.SaveChanges();
        //                        return Json(new { success = true, message = "Đăng nhập MoMo thành công." }, JsonRequestBehavior.AllowGet);
        //                    }
        //                    else
        //                    {
        //                        return Json(new { success = false, message = moMoLogin.errorDesc }, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                else if (bankConfig.BankName.Equals("Vietinbank"))
        //                {
        //                    VietinbankLoginModel vietinbankLogin = await VietinbankAPI.Login(userName, passWord);
        //                    if (vietinbankLogin == null) return Json(new { success = false, message = MessageEndUser.APIError }, JsonRequestBehavior.AllowGet);
        //                    if (!vietinbankLogin.error)
        //                    {
        //                        //lấy accountnumber/bsb
        //                        VietinbankEntitiesAndAccountModel vietinbankEntitiesAndAccount = await VietinbankAPI.getEntitiesAndAccounts(vietinbankLogin.sessionId);
        //                        if (!vietinbankEntitiesAndAccount.error)
        //                        {
        //                            if (bankAccountId == null)
        //                            {
        //                                //lưu lại trong db
        //                                db.tblBankAccounts.Add(new tblBankAccount()
        //                                {
        //                                    BankName = "Vietinbank",
        //                                    UserName = userName,
        //                                    Password = passWord,
        //                                    isActive = true,
        //                                    AccountNumber = vietinbankEntitiesAndAccount.accounts[0].number,
        //                                    LoginTime = DateTime.Now,
        //                                    BankConfigId = bankConfigId,
        //                                    sessionId = vietinbankLogin.sessionId,
        //                                    bsb = vietinbankEntitiesAndAccount.accounts[0].bsb,
        //                                    accountType = vietinbankEntitiesAndAccount.accounts[0].type,
        //                                    currencyCode = vietinbankEntitiesAndAccount.accounts[0].currencyCode
        //                                });
        //                            }
        //                            else
        //                            {
        //                                db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccountId.Value).isActive = true;
        //                            }
        //                            db.SaveChanges();
        //                            //Nên có thông báo đồng bộ giao dịch trong ngày -> lấy và lưu các giao dịch trong ngày
        //                            return Json(new { success = true, message = "Đăng nhập thành công!" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        else
        //                        {
        //                            return Json(new { success = false, message = "Đăng nhập thất bại!" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    else return Json(new { success = false, message = vietinbankLogin.errorMessage }, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    return Json(new { success = false, message = "Lỗi: Chưa xử lý đăng nhập cho Loại tài khoản này." }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Log.Error(ex);
        //        return Json(new
        //        {
        //            success = false,
        //            message = MessageEndUser.CodeException
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpPost]
        //public JsonResult UpdateBank(Guid? Id, string Title, string BankName, string BankAccountNumber, string BankFullName)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            if (string.IsNullOrEmpty(BankName) || string.IsNullOrEmpty(BankAccountNumber) || string.IsNullOrEmpty(BankFullName))
        //            {
        //                return Json(new { success = false, message = "Vui lòng điền đẩy đủ th" }, JsonRequestBehavior.AllowGet);
        //            }
        //            var bankNote = db.tblBankAccountNotes.FirstOrDefault(t => t.Id == (Id ?? Guid.Empty));
        //            if (bankNote == null)
        //            {
        //                db.tblBankAccountNotes.Add(new tblBankAccountNote()
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Title = Title,
        //                    BankAccountNumber = BankAccountNumber,
        //                    BankName = BankName,
        //                    BankFullName = BankFullName,
        //                    isActive = true,
        //                    CreatedTime = DateTime.Now
        //                });
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                bankNote.Title = Title;
        //                bankNote.BankName = BankName;
        //                bankNote.BankAccountNumber = BankAccountNumber;
        //                bankNote.BankFullName = BankFullName;
        //                db.SaveChanges();
        //            }
        //            return Json(new { success = true, message = "Cập nhật tài khoản ngân hàng thành công" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.LogToDB("Home/UpdateBank", ex);
        //        return Json(new { success = false, message = "Cập nhật tài khoản ngân hàng thất bại" }, JsonRequestBehavior.AllowGet);
        //    }

        //}
        //[HttpDelete]
        //public JsonResult DeleteBankNote(Guid id)
        //{
        //    using (var db = new BankAPIEntities())
        //    {
        //        tblBankAccountNote accountNote = db.tblBankAccountNotes.FirstOrDefault(t => t.Id == id);
        //        if (accountNote != null)
        //        {
        //            db.tblBankAccountNotes.Remove(accountNote);
        //            db.SaveChanges();
        //            return Json(new { success = true, message = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json(new { success = false, message = "Không tồn tại" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("yesnoTransactionWithoutDeposit")]
        //[HttpPost]
        //public JsonResult UpdateStatusWithoutDeposit(List<int> transactionIds, string status)
        //{
        //    try
        //    {
        //        Logging.LogChange("UpdateStatusWithoutDeposit", new
        //        {
        //            transactionIds,
        //            status
        //        }, User.Identity.Name);
        //        using (var db = new BankAPIEntities())
        //        {
        //            foreach (var tranId in transactionIds)
        //            {
        //                var transaction = db.tblTransactions.FirstOrDefault(t => t.Id == tranId);
        //                if (transaction != null)
        //                {
        //                    transaction.Status = status;
        //                    db.SaveChanges();
        //                    new RealtimeHub().updateTransaction(tranId);
        //                }
        //            }
        //        }
        //        return Json(new { success = true, message = "Cập nhật thành công." }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.LogToDB("Home/UpdateStatusWithoutDeposit", ex);
        //        return Json(new { success = false, message = "Cập nhật thất bại." }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("exportExcel")]
        //public JsonResult ExportExcel(string FromFilter, string StatusFilter, string MoneyFilter, string TimeFilter, DateTime? DateFilter, bool isTruTien)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            string query = @"   SELECT *
        //                        FROM tblTransaction
        //                    ";
        //            string where = "WHERE 1 = 1 ";
        //            if (isTruTien)
        //            {
        //                where += $" AND CD = '-'";
        //            }
        //            if (!string.IsNullOrEmpty(StatusFilter) && !StatusFilter.Equals("all"))
        //            {
        //                where += $" AND [Status] = '{StatusFilter}'";
        //            }
        //            if (!string.IsNullOrEmpty(FromFilter))
        //            {
        //                where += $" AND [From] like '%{FromFilter}%'";
        //            }
        //            if (!string.IsNullOrEmpty(MoneyFilter))
        //            {
        //                where += $" AND Amount like '%{MoneyFilter}%'";
        //            }
        //            if (!string.IsNullOrEmpty(TimeFilter))
        //            {
        //                where += $" AND CONVERT(VARCHAR(5),DateTime,108) like N'%{TimeFilter}%'";
        //            }
        //            if (DateFilter != null)
        //            {
        //                where += $" AND (TRY_CONVERT(date,DateTime,103) = Convert(date,'{DateFilter.Value:yyyy-MM-dd}'))";
        //            }

        //            query = $"{query + where} order by DateTime desc";
        //            var dataTransaction = db.Database.SqlQuery<tblTransaction>(query).Select(t => new TransactionExcelModel()
        //            {
        //                From = t.From,
        //                BankName = t.BankName,
        //                TransactionDate = t.TransactionDate,
        //                PCTime = t.PCTime,
        //                DateTime = t.DateTime.ToString("dd/MM/yyyy HH:mm:ss"),
        //                Description = t.Description,
        //                PhoneNumber = t.PhoneNumber,
        //                CD = t.CD,
        //                Amount = t.Amount,
        //                Status = t.Status,
        //                UpdateBy = t.UpdateBy
        //            }).ToList();
        //            using (var excelPackage = new ExcelPackage())
        //            {
        //                // Tạo author cho file Excel
        //                excelPackage.Workbook.Worksheets.Add("Transaction");
        //                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
        //                var workSheet = excelPackage.Workbook.Worksheets[0];
        //                // Đổ data vào Excel file
        //                workSheet.Cells[1, 1].LoadFromCollection(dataTransaction, true, TableStyles.Dark9);
        //                // BindingFormatForExcel(workSheet, list);
        //                excelPackage.Save();
        //                var folder = Server.MapPath("/Download");
        //                if (!Directory.Exists(folder))
        //                {
        //                    Directory.CreateDirectory(folder);
        //                }
        //                string filePath = $"/Download/Transaction_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
        //                var file = Server.MapPath(filePath);
        //                excelPackage.SaveAs(file);
        //                return Json(new { success = true, url = filePath }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}