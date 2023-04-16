using FT_Admin.Models;
using FT_Admin.Models.Data;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FT_Admin.Models.ViewModel;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("DepositManage")]
    public class DepositController : Controller
    {
        // GET: Deposit
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Ajax GetAll
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="statusFilter"></param>
        /// <param name="dateFilter"></param>
        /// <param name="curPage"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetAll(string searchtext, string statusFilter, bool chbBaoLoi, DateTime? dateFilter, int? curPage = 1)
        {
            try
            {
                //new RealtimeHub().updateCountNotify();
                //new RealtimeHub().updateCountTicket();
                using (var db = new GameChienEntities())
                {
                    string query = @"   SELECT d.Id, d.PlayerId, p.AccountName as PlayerAccountName,
                                                d.Amount, d.AttachFile, d.TransactionId, d.CreatedTime, d.Status, d.UpdatedBy,
                                                u.UserName as UpdatedByUserName, d.UpdatedTime,
                                                ROW_NUMBER() over (order by d.CreatedTime desc) as r
                                        FROM tblDeposit d
                                        join tblPlayer p on d.PlayerId = p.Id
                                        left join tblUser u on d.UpdatedBy = u.Id
                                    ";
                    string where = " Where 1 = 1";
                    if (!string.IsNullOrEmpty(searchtext))
                    {
                        where += $" AND ( p.AccountName like N'%{searchtext}%' or p.FullName like N'%{searchtext}%' or p.PhoneNumber like N'%{searchtext}%' or p.GameAccount like N'%{searchtext}%' or p.Email like N'%{searchtext}%' ) ";
                    }
                    if (!statusFilter.Equals("all"))
                    {
                        where += $" AND (ISNULL(d.Status,'') = N'{statusFilter}') ";
                    }

                    //if (dateFilter != null)
                    //{
                    //    where += $" AND (TRY_CONVERT(date,cr.CreatedTime,103) = Convert(date,'{dateFilter.Value:yyyy-MM-dd}'))";
                    //}
                    string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                    query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by CreatedTime desc";
                    string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblDeposit d
                                            join tblPlayer p on d.PlayerId = p.Id
                                            {1}", PageSize, where);
                    List<DepositViewModel> dataDepositView = db.Database.SqlQuery<DepositViewModel>(query).ToList();
                    ViewBag.maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                    ViewBag.curPage = curPage;
                    return PartialView("_DepositTable", dataDepositView);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("/Deposit/GetAll", ex, $"\n searchtext={searchtext},statusFilter={statusFilter}, dateFilter={dateFilter?.ToString("dd/MM/yyyy") ?? ""},curPage={curPage}");
                return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Ajax RenderRow
        /// </summary>
        /// <param name="cusRequestId"></param>
        /// <param name="searchtext"></param>
        /// <param name="statusFilter"></param>
        /// <param name="dateFilter"></param>
        /// <returns></returns>
        //public async Task<ActionResult> RenderRow(Guid cusRequestId, string searchtext, string statusFilter, bool chbBaoLoi, DateTime? dateFilter)
        //{
        //    try
        //    {
        //        using (var db = new GameChienEntities())
        //        {
        //            var cusRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(cusRequestId) && t.Type);
        //            if (cusRequestFromDB != null
        //                && (string.IsNullOrEmpty(searchtext)
        //                || (cusRequestFromDB.tblCustomer.GameId?.Contains(searchtext) ?? false)
        //                || (cusRequestFromDB.tblCustomer.GameAccountName?.Contains(searchtext) ?? false)
        //                || (cusRequestFromDB.tblCustomer.PhoneNumber?.Contains(searchtext) ?? false)
        //                || (cusRequestFromDB.tblCustomer.BankName?.Contains(searchtext) ?? false)
        //                || (cusRequestFromDB.tblCustomer.BankAccountNumber?.Contains(searchtext) ?? false)
        //                || (cusRequestFromDB.tblCustomer.BankFullName?.Contains(searchtext) ?? false)
        //                || (cusRequestFromDB.Total?.ToString().Contains(searchtext) ?? false)
        //                || cusRequestFromDB.Point.ToString().Contains(searchtext)
        //                || (cusRequestFromDB.Note?.Contains(searchtext) ?? false)
        //                )
        //                && ((string.IsNullOrEmpty(statusFilter) && cusRequestFromDB.Status == null) || statusFilter.Equals("all") || (cusRequestFromDB.Status != null && cusRequestFromDB.Status.Equals(statusFilter)))
        //                && (dateFilter == null || cusRequestFromDB.CreatedTime.Date == dateFilter.Value.Date)
        //                && (!chbBaoLoi || !string.IsNullOrEmpty(cusRequestFromDB.ReportErrorMessage))
        //                )
        //            {
        //                return PartialView("_DepositRow", new CustomerRequestDto()
        //                {
        //                    Id = cusRequestFromDB.Id,
        //                    GameId = cusRequestFromDB.tblCustomer.GameId,
        //                    GameAccountName = cusRequestFromDB.tblCustomer.GameAccountName,
        //                    PhoneNumber = cusRequestFromDB.tblCustomer.PhoneNumber,
        //                    BankName = cusRequestFromDB.tblCustomer.BankName,
        //                    BankAccountNumber = cusRequestFromDB.tblCustomer.BankAccountNumber,
        //                    BankFullName = cusRequestFromDB.tblCustomer.BankFullName,
        //                    Type = cusRequestFromDB.Type,
        //                    Point = cusRequestFromDB.Point.ToString("N2"),
        //                    MoneyOfPoint = (cusRequestFromDB.MoneyOfPoint == null ? "" : ((cusRequestFromDB.MoneyOfPoint.Value / 1000).ToString("N0"))),
        //                    Total = cusRequestFromDB.Total,
        //                    CreatedTime = cusRequestFromDB.CreatedTime,
        //                    CustomerId = cusRequestFromDB.CustomerId,
        //                    Status = cusRequestFromDB.Status,
        //                    isActive = cusRequestFromDB.isActive,
        //                    UpdateBy = cusRequestFromDB.UpdateBy,
        //                    UpdatingBy = cusRequestFromDB.UpdatingBy,
        //                    AttachFile = cusRequestFromDB.AttachFile,
        //                    Note = cusRequestFromDB.Note,
        //                    isCallAPIError = cusRequestFromDB.isCallAPIError,
        //                    ReportErrorMessage = cusRequestFromDB.ReportErrorMessage
        //                });
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Logging.LogToDBAsync("Deposit/RenderRow", ex, $"\n cusRequestId={cusRequestId}, searchtext={searchtext}, statusFilter={statusFilter}, dateFilter={dateFilter?.ToString("dd/MM/yyyy") ?? ""}");
        //        return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        ///// <summary>
        ///// Ajax Detail
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="status"></param>
        ///// <returns></returns>
        //public ActionResult Detail(Guid id, string status)
        //{
        //    using (var db = new BankAPIEntities())
        //    {
        //        tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(id));
        //        if (customerRequestFromDB == null) return Redirect("/Home");
        //        customerRequestFromDB.UpdateBy = User.Identity.Name;
        //        db.SaveChanges();
        //        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        //        {
        //            Id = customerRequestFromDB.Id,
        //            GameId = customerRequestFromDB.tblCustomer.GameId,
        //            GameAccountName = customerRequestFromDB.tblCustomer.GameAccountName,
        //            PhoneNumber = customerRequestFromDB.tblCustomer.PhoneNumber,
        //            BankName = customerRequestFromDB.tblCustomer.BankName,
        //            BankAccountNumber = customerRequestFromDB.tblCustomer.BankAccountNumber,
        //            BankFullName = customerRequestFromDB.tblCustomer.BankFullName,
        //            Type = customerRequestFromDB.Type,
        //            Point = customerRequestFromDB.Point.ToString("N2"),
        //            MoneyOfPoint = (customerRequestFromDB.MoneyOfPoint == null ? "" : ((customerRequestFromDB.MoneyOfPoint.Value / 1000).ToString("N0"))),
        //            Total = customerRequestFromDB.Total,
        //            CreatedTime = customerRequestFromDB.CreatedTime,
        //            CustomerId = customerRequestFromDB.CustomerId,
        //            Status = customerRequestFromDB.Status,
        //            isActive = customerRequestFromDB.isActive,
        //            UpdateBy = customerRequestFromDB.UpdateBy,
        //            AttachFile = customerRequestFromDB.AttachFile,
        //            Note = customerRequestFromDB.Note
        //        };

        //        ViewBag.status = status;
        //        if (status.Equals("no"))
        //        {
        //            ViewBag.listNoteRecommend = db.tblContentRecommends.Where(t => t.Name.Equals("deposit_no")).Select(t => t.Content).ToList();
        //        }
        //        else if (status.Equals("reporterror"))
        //        {
        //            ViewBag.listNoteRecommend = db.tblContentRecommends.Where(t => t.Name.Equals("deposit_reporterror")).Select(t => t.Content).ToList();
        //        }
        //        return PartialView("_Detail", customerRequestDto);
        //    }
        //}
        //[CustomAuthorize("0", "actionDeposit")]
        //public async Task<ActionResult> UpdateStatus(Guid id, string status, string note)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(id));
        //            if (customerRequestFromDB == null) return RedirectToAction("Index", "Home");
        //            if (customerRequestFromDB.isProcessing && status.Equals("yes"))
        //            {
        //                return Json(new { success = false, message = "Hệ thống đang xử lý yêu cầu này. Vui lòng đợi" }, JsonRequestBehavior.AllowGet);
        //            }
        //            if (!string.IsNullOrEmpty(customerRequestFromDB.Status) && !status.Equals("reporterror"))
        //            {
        //                return Json(new { success = false, message = "Yêu cầu này đã được xử lý." }, JsonRequestBehavior.AllowGet);
        //            }
        //            if (status.Equals("no") && string.IsNullOrEmpty(note))
        //            {
        //                return Json(new { success = false, message = "Vui lòng chọn lý do." }, JsonRequestBehavior.AllowGet);
        //            }
        //            tblCustomer customerFromDB = db.tblCustomers.FirstOrDefault(t => t.Id == customerRequestFromDB.CustomerId);
        //            if (customerFromDB == null) return Json(new { success = false, message = "Khách hàng này không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            //Update đang xử lý -> push cho user biết
        //            customerRequestFromDB.UpdatingBy = User.Identity.Name;
        //            db.SaveChanges();
        //            new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            if (status.Equals("yes"))
        //            {
        //                //add point
        //                bool isActiveAccount = false;
        //                //Check xem lệnh này phải lệnh active không
        //                if (string.IsNullOrEmpty(customerFromDB.GameId) || !customerFromDB.isActive)
        //                {
        //                    isActiveAccount = true;
        //                    //tài khoản này chưa được cấp gameID
        //                    //Lấy gameID Avaliable để gán
        //                    tblCustomer customerAvaliableGameId = db.tblCustomers.Where(t => !string.IsNullOrEmpty(t.GameId) && string.IsNullOrEmpty(t.PhoneNumber) && string.IsNullOrEmpty(t.GameAccountName)).OrderBy(t => t.CreatedTime).FirstOrDefault();
        //                    if (customerAvaliableGameId != null)
        //                    {
        //                        customerFromDB.GameId = customerAvaliableGameId.GameId;
        //                        customerFromDB.Password = ConfigurationManager.AppSettings["passWord"];
        //                        customerFromDB.isActive = true;

        //                        customerAvaliableGameId.GameId += "_" + customerAvaliableGameId.CreatedTime.ToString("ddMMyyyyHHmmss");
        //                        customerAvaliableGameId.GameAccountName = customerAvaliableGameId.GameId + "_" + customerAvaliableGameId.CreatedTime.ToString("ddMMyyyyHHmmss");
        //                        customerAvaliableGameId.isActive = false;
        //                    }
        //                    else
        //                    {
        //                        //đang không có sẵn => call api để tạo
        //                        CreateAccountAPIModel createAccountAPI = await GameAccountAPI.CreateAccountAsync(1);
        //                        if (createAccountAPI != null && createAccountAPI.status)
        //                        {
        //                            customerFromDB.GameId = createAccountAPI.data.FirstOrDefault();
        //                            customerFromDB.Password = ConfigurationManager.AppSettings["passWord"];
        //                            customerFromDB.isActive = true;
        //                        }
        //                        else
        //                        {
        //                            customerRequestFromDB.UpdatingBy = null;
        //                            db.SaveChanges();
        //                            return Json(new { success = false, message = createAccountAPI?.message ?? "GameId đang không có sẵn & không thể tạo mới." }, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                }
        //                AddPointAccountModel addPointAccount = await GameAccountAPI.AddPointAccountAsync(customerFromDB.GameId, customerRequestFromDB.Point, customerRequestFromDB.MoneyOfPoint.Value / 1000, note, User.Identity.Name);
        //                if (addPointAccount != null)
        //                {
        //                    if (addPointAccount.status)
        //                    {
        //                        //Add điểm thành công
        //                        //Cập nhật request
        //                        customerRequestFromDB.UpdatingBy = null;
        //                        customerRequestFromDB.UpdateBy = User.Identity.Name;
        //                        customerRequestFromDB.Status = "yes";
        //                        customerRequestFromDB.Note = note;
        //                        customerRequestFromDB.ReportErrorMessage = null;
        //                        //Cập nhật customer
        //                        customerFromDB.GameAccountName = addPointAccount.loginName;
        //                        customerFromDB.MoneyOfPoint = customerRequestFromDB.MoneyOfPoint;
        //                        db.SaveChanges();

        //                        //tính tổng nạp
        //                        double totalDeposit = db.Database.SqlQuery<double>($"select isnull(sum(Total),0) from tblCustomerRequest where [Type] = 1 and CustomerId = '{customerFromDB.Id}' and [Status] = 'yes'").FirstOrDefault();

        //                        //Check tổng nạp để cập nhật vip
        //                        if (totalDeposit > 0)
        //                        {
        //                            //cập nhật tổng nạp
        //                            customerFromDB.TotalDeposit = (long)totalDeposit;
        //                            tblVipConfig vipConfig = db.tblVipConfigs.Where(t => t.FromAmount <= totalDeposit).OrderByDescending(t => t.FromAmount).FirstOrDefault();
        //                            if (vipConfig != null)
        //                            {
        //                                customerFromDB.VipConfigId = vipConfig.Id;
        //                            }
        //                            db.SaveChanges();
        //                        }
        //                        //Push update request
        //                        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                        //Push update customer
        //                        new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, isActiveAccount, null);
        //                        //Push update totalSum
        //                        var date = DateTime.Now.Date;
        //                        var totalUpdateFromDB = db.tblTotalUpdates.Where(t => t.Date == date && t.Type).OrderByDescending(t => t.Id).FirstOrDefault();
        //                        if (totalUpdateFromDB != null)
        //                        {
        //                            totalUpdateFromDB.Total += customerRequestFromDB.Total ?? 0;
        //                        }
        //                        db.SaveChanges();
        //                        await Logging.LogChangeAsync("Deposit", new
        //                        {
        //                            UpdateBy = customerRequestFromDB.UpdateBy,
        //                            Status = customerRequestFromDB.Status,
        //                            AttachFile = customerRequestFromDB.AttachFile,
        //                            AutoBankingStatus = customerRequestFromDB.AutoBankingStatus,
        //                            BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //                            BankFullName = customerRequestFromDB.BankFullName,
        //                            BankName = customerRequestFromDB.BankName,
        //                            CreatedTime = customerRequestFromDB.CreatedTime,
        //                            CustomerId = customerRequestFromDB.CustomerId,
        //                            Id = customerRequestFromDB.Id,
        //                            MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                            Note = customerRequestFromDB.Note,
        //                            Point = customerRequestFromDB.Point,
        //                            ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //                            Total = customerRequestFromDB.Total,
        //                            Type = customerRequestFromDB.Type
        //                        }, User.Identity.Name);
        //                        new RealtimeHub().updateTotalDeposit();
        //                    }
        //                    else
        //                    {
        //                        customerRequestFromDB.UpdatingBy = null;
        //                        db.SaveChanges();
        //                        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                        return Json(new { success = false, message = "Cập nhật trạng thái " + status.ToUpper() + " thất bại.\b " + addPointAccount.message }, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                else
        //                {
        //                    customerRequestFromDB.UpdatingBy = null;
        //                    customerRequestFromDB.isCallAPIError = true;
        //                    customerRequestFromDB.Note = "API AddPointAccount return Null";
        //                    db.SaveChanges();

        //                    new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                    return Json(new { success = false, message = "Cập nhật trạng thái " + status.ToUpper() + " thất bại. Lỗi API Deposit." }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else if (status.Equals("no"))
        //            {
        //                customerRequestFromDB.Status = "no";
        //                customerRequestFromDB.UpdatingBy = null;
        //                customerRequestFromDB.UpdateBy = User.Identity.Name;
        //                customerRequestFromDB.Note = note;
        //                customerRequestFromDB.ReportErrorMessage = null;
        //                customerRequestFromDB.isCallAPIError = null;
        //                db.SaveChanges();

        //                await Logging.LogChangeAsync("Deposit", new
        //                {
        //                    UpdateBy = customerRequestFromDB.UpdateBy,
        //                    Status = customerRequestFromDB.Status,
        //                    AttachFile = customerRequestFromDB.AttachFile,
        //                    AutoBankingStatus = customerRequestFromDB.AutoBankingStatus,
        //                    BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //                    BankFullName = customerRequestFromDB.BankFullName,
        //                    BankName = customerRequestFromDB.BankName,
        //                    CreatedTime = customerRequestFromDB.CreatedTime,
        //                    CustomerId = customerRequestFromDB.CustomerId,
        //                    Id = customerRequestFromDB.Id,
        //                    isActive = customerRequestFromDB.isActive,
        //                    isApproved = customerRequestFromDB.isApproved,
        //                    isCallAPIError = customerRequestFromDB.isCallAPIError,
        //                    isProcessing = customerRequestFromDB.isProcessing,
        //                    MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                    Note = customerRequestFromDB.Note,
        //                    Point = customerRequestFromDB.Point,
        //                    ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //                    Total = customerRequestFromDB.Total,
        //                    Type = customerRequestFromDB.Type
        //                }, User.Identity.Name);
        //                //Push update customer
        //                new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, false, customerRequestFromDB.Note);
        //                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            }
        //            else if (status.Equals("reporterror"))
        //            {
        //                customerRequestFromDB.Status = null;
        //                customerRequestFromDB.ReportErrorMessage = note;
        //                customerRequestFromDB.UpdatingBy = null;
        //                customerRequestFromDB.UpdateBy = User.Identity.Name;
        //                db.SaveChanges();
        //                await Logging.LogChangeAsync("Deposit", new
        //                {
        //                    UpdateBy = customerRequestFromDB.UpdateBy,
        //                    Status = customerRequestFromDB.Status,
        //                    AttachFile = customerRequestFromDB.AttachFile,
        //                    AutoBankingStatus = customerRequestFromDB.AutoBankingStatus,
        //                    BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //                    BankFullName = customerRequestFromDB.BankFullName,
        //                    BankName = customerRequestFromDB.BankName,
        //                    CreatedTime = customerRequestFromDB.CreatedTime,
        //                    CustomerId = customerRequestFromDB.CustomerId,
        //                    Id = customerRequestFromDB.Id,
        //                    isActive = customerRequestFromDB.isActive,
        //                    isApproved = customerRequestFromDB.isApproved,
        //                    isCallAPIError = customerRequestFromDB.isCallAPIError,
        //                    isProcessing = customerRequestFromDB.isProcessing,
        //                    MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                    Note = customerRequestFromDB.Note,
        //                    Point = customerRequestFromDB.Point,
        //                    ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //                    Total = customerRequestFromDB.Total,
        //                    Type = customerRequestFromDB.Type
        //                }, User.Identity.Name);
        //                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            }
        //            return Json(new { success = true, message = "Cập nhật trạng thái " + status.ToUpper() + " thành công" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Logging.LogToDBAsync("/DepositController/UpdateStatus", ex, $"id={id}&status={status}");
        //        return Json(new { success = false, message = MessageEndUser.CodeException, }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("0", "editTotalDeposit")]
        //public async Task<JsonResult> ChangeTotal(string exp, double val)
        //{
        //    using (var db = new BankAPIEntities())
        //    {
        //        var date = DateTime.Now.Date;
        //        // lấy cập nhật mới nhất
        //        var totalUpdateFromDB = db.tblTotalUpdates.Where(t => t.Date == date && t.Type).OrderByDescending(t => t.Id).FirstOrDefault();
        //        double total = totalUpdateFromDB == null ? (db.Database.SqlQuery<double?>("select sum(total) from tblCustomerRequest where [Type] = 1 and [Status] = 'yes' and convert(date,CreatedTime) = convert(date,getdate())").FirstOrDefault() ?? 0) : totalUpdateFromDB.Total;
        //        tblTotalUpdate totalUpdate = new tblTotalUpdate()
        //        {
        //            Date = date,
        //            TotalOld = total,
        //            Amount = (exp.Equals("+") ? val : -val),
        //            Total = exp.Equals("+") ? (total + val) : (total - val),
        //            Type = true
        //        };
        //        db.tblTotalUpdates.Add(totalUpdate);
        //        await db.SaveChangesAsync();
        //        await Logging.LogChangeAsync("ChangeTotal", totalUpdate, User.Identity.Name);
        //        return Json(new { success = true, total = (exp.Equals("+") ? (total + val) : (total - val)).ToString("N0") }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult SearchTransaction(Guid customerRequestId)
        //{
        //    using (var db = new BankAPIEntities())
        //    {
        //        tblCustomerRequest customerRequest = db.tblCustomerRequests.FirstOrDefault(t => t.Id == customerRequestId);
        //        if (customerRequest == null) return Json(new { success = false, message = "Lệnh nạp không tồn tại." }, JsonRequestBehavior.AllowGet);
        //        else
        //        {
        //            customerRequest.UpdatingBy = User.Identity.Name;
        //            db.SaveChanges();
        //            new RealtimeHub().updateCustomerRequest(customerRequestId);

        //            string checkBy = ConfigurationManager.AppSettings["CheckAddPointBy"]?.ToString() ?? "App";
        //            string amountCheck = customerRequest.Total.Value.ToString("N0", CultureInfo.InstalledUICulture);
        //            var listTransaction = db.tblTransactions.Where(t => DbFunctions.TruncateTime(t.DateTime) == customerRequest.CreatedTime.Date
        //            && t.From.Contains(checkBy)
        //            && t.CD.Equals("+")
        //            && t.Status.Equals("null")
        //            && t.Amount != null
        //            && t.Amount == customerRequest.Total).ToList();
        //            if (!User.IsInRole("0"))
        //            {
        //                var listBankSMS = db.tblUsers.FirstOrDefault(t => t.UserName.Equals(User.Identity.Name))?.BankSMS ?? null;
        //                var listBankAPI = db.tblUsers.FirstOrDefault(t => t.UserName.Equals(User.Identity.Name))?.BankAPI ?? null;
        //                var listBankNotify = db.tblUsers.FirstOrDefault(t => t.UserName.Equals(User.Identity.Name))?.BankNotify ?? null;
        //                if (!string.IsNullOrEmpty(listBankSMS))
        //                {
        //                    listTransaction = listTransaction.Where(t => listBankSMS.Contains(t.BankName)).ToList();
        //                }
        //                if (!string.IsNullOrEmpty(listBankAPI))
        //                {
        //                    listTransaction = listTransaction.Where(t => listBankAPI.Contains(t.BankName)).ToList();
        //                }
        //                if (!string.IsNullOrEmpty(listBankNotify))
        //                {
        //                    listTransaction = listTransaction.Where(t => listBankNotify.Contains(t.BankName)).ToList();
        //                }
        //            }
        //            if (listTransaction.Count == 0)
        //            {
        //                return Json(new { success = false, message = "Chưa có giao dịch nào khớp lệnh nạp." }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                ViewBag.Transactions = listTransaction;
        //                return PartialView("_TransactionSearch", new CustomerRequestDto()
        //                {
        //                    Id = customerRequest.Id,
        //                    CreatedTime = customerRequest.CreatedTime,
        //                    GameId = customerRequest.tblCustomer.GameId,
        //                    GameAccountName = customerRequest.tblCustomer.GameAccountName,
        //                    PhoneNumber = customerRequest.tblCustomer.PhoneNumber,
        //                    Total = customerRequest.Total
        //                });
        //            }
        //        }
        //    }
        //}
        //public async Task<ActionResult> ChooseTransaction(int transactionId, Guid customerRequestId)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(customerRequestId));
        //            if (customerRequestFromDB == null) return Json(new { success = false, message = "Lệnh nạp không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            if (!string.IsNullOrEmpty(customerRequestFromDB.Status)) return Json(new { success = false, message = "Lệnh nạp này đã được xử lý." }, JsonRequestBehavior.AllowGet);

        //            tblTransaction transactionFromDB = db.tblTransactions.FirstOrDefault(t => t.Id.Equals(transactionId));
        //            if (transactionFromDB == null) return Json(new { success = false, message = "Giao dịch không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            if (!transactionFromDB.Status.Equals("null")) return Json(new { success = false, message = "Giao dịch này đã được chọn." }, JsonRequestBehavior.AllowGet);

        //            tblCustomer customerFromDB = customerRequestFromDB.tblCustomer;
        //            if (customerFromDB == null) return Json(new { success = false, message = "Khách hàng không tồn tại." }, JsonRequestBehavior.AllowGet);

        //            bool isActiveAccount = false;

        //            //add point
        //            if (string.IsNullOrEmpty(customerFromDB.GameId) || !customerFromDB.isActive)
        //            {
        //                isActiveAccount = true;
        //                //tài khoản này chưa được cấp gameID
        //                //Lấy gameID Avaliable để gán
        //                tblCustomer customerAvaliableGameId = db.tblCustomers.Where(t => !string.IsNullOrEmpty(t.GameId) && string.IsNullOrEmpty(t.PhoneNumber) && string.IsNullOrEmpty(t.GameAccountName)).OrderBy(t => t.CreatedTime).FirstOrDefault();
        //                if (customerAvaliableGameId != null)
        //                {
        //                    customerFromDB.GameId = customerAvaliableGameId.GameId;
        //                    customerFromDB.Password = ConfigurationManager.AppSettings["passWord"];
        //                    customerFromDB.isActive = true;

        //                    customerAvaliableGameId.GameId += "_" + customerAvaliableGameId.CreatedTime.ToString("ddMMyyyyHHmmss");
        //                    customerAvaliableGameId.GameAccountName = customerAvaliableGameId.GameId + "_" + customerAvaliableGameId.CreatedTime.ToString("ddMMyyyyHHmmss");
        //                    customerAvaliableGameId.isActive = false;
        //                }
        //                else
        //                {
        //                    //đang không có sẵn => call api để tạo 
        //                    CreateAccountAPIModel createAccountAPI = await GameAccountAPI.CreateAccountAsync(1);
        //                    if (createAccountAPI != null && createAccountAPI.status)
        //                    {
        //                        customerFromDB.GameId = createAccountAPI.data.FirstOrDefault();
        //                        customerFromDB.Password = ConfigurationManager.AppSettings["passWord"];
        //                        customerFromDB.isActive = true;
        //                    }
        //                    else
        //                    {
        //                        customerRequestFromDB.UpdatingBy = null;
        //                        db.SaveChanges();
        //                        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                        return Json(new { success = false, message = "GameId đang không có sẵn & không thể tạo mới." }, JsonRequestBehavior.AllowGet);

        //                    }
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(customerFromDB.GameId) && customerFromDB.isActive)
        //            {
        //                string note = $"{transactionFromDB.TransactionDate.Substring(0, 5)} {transactionFromDB.PCTime} {transactionFromDB.Amount?.ToString("N0") ?? ""}";
        //                AddPointAccountModel addPointAccount = await GameAccountAPI.AddPointAccountAsync(customerFromDB.GameId, customerRequestFromDB.Point, customerRequestFromDB.MoneyOfPoint.Value / 1000, note, User.Identity.Name);
        //                if (addPointAccount != null && addPointAccount.status)
        //                {
        //                    //Add điểm thành công
        //                    //Cập nhật request
        //                    customerRequestFromDB.Status = "yes";
        //                    customerRequestFromDB.Note = note;
        //                    customerRequestFromDB.UpdatingBy = null;
        //                    customerRequestFromDB.UpdateBy = User.Identity.Name;

        //                    //cập nhật transaction
        //                    transactionFromDB.Status = "yes";
        //                    transactionFromDB.UpdateBy = User.Identity.Name;
        //                    //Cập nhật customer
        //                    customerFromDB.GameAccountName = addPointAccount.loginName;
        //                    customerFromDB.MoneyOfPoint = customerRequestFromDB.MoneyOfPoint;
        //                    db.SaveChanges();

        //                    //tính tổng nạp
        //                    double totalDeposit = db.Database.SqlQuery<double>($"select isnull(sum(Total),0) from tblCustomerRequest where [Type] = 1 and CustomerId = '{customerFromDB.Id}' and [Status] = 'yes'").FirstOrDefault();

        //                    //Check tổng nạp để cập nhật vip
        //                    if (totalDeposit > 0)
        //                    {
        //                        //cập nhật tổng nạp
        //                        customerFromDB.TotalDeposit = (long)totalDeposit;
        //                        tblVipConfig vipConfig = db.tblVipConfigs.Where(t => t.FromAmount <= totalDeposit).OrderByDescending(t => t.FromAmount).FirstOrDefault();
        //                        if (vipConfig != null)
        //                        {
        //                            customerFromDB.VipConfigId = vipConfig.Id;
        //                        }
        //                        db.SaveChanges();
        //                    }
        //                    //Push Update transaction
        //                    new RealtimeHub().updateTransaction(transactionFromDB.Id);
        //                    //Push update request
        //                    new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                    //Push update customer
        //                    new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, isActiveAccount, customerRequestFromDB.Note);
        //                    //Push update totalSum
        //                    var date = DateTime.Now.Date;
        //                    var totalUpdateFromDB = db.tblTotalUpdates.Where(t => t.Date == date && t.Type).OrderByDescending(t => t.Id).FirstOrDefault();
        //                    if (totalUpdateFromDB != null)
        //                    {
        //                        totalUpdateFromDB.Total += customerRequestFromDB.Total ?? 0;
        //                    }
        //                    db.SaveChanges();
        //                    await Logging.LogChangeAsync("ChooseTransaction", new
        //                    {
        //                        UpdateBy = customerRequestFromDB.UpdateBy,
        //                        Status = customerRequestFromDB.Status,
        //                        AttachFile = customerRequestFromDB.AttachFile,
        //                        AutoBankingStatus = customerRequestFromDB.AutoBankingStatus,
        //                        BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //                        BankFullName = customerRequestFromDB.BankFullName,
        //                        BankName = customerRequestFromDB.BankName,
        //                        CreatedTime = customerRequestFromDB.CreatedTime,
        //                        CustomerId = customerRequestFromDB.CustomerId,
        //                        Id = customerRequestFromDB.Id,
        //                        MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                        Note = customerRequestFromDB.Note,
        //                        Point = customerRequestFromDB.Point,
        //                        ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //                        Total = customerRequestFromDB.Total,
        //                        Type = customerRequestFromDB.Type
        //                    }, User.Identity.Name);
        //                    new RealtimeHub().updateTotalDeposit();
        //                    return Json(new { success = true, message = "Cập nhật trạng thái thành công" }, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    customerRequestFromDB.UpdatingBy = null;
        //                    db.SaveChanges();
        //                    new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                    return Json(new { success = false, message = (addPointAccount?.message ?? "Lỗi: Call API thất bại.") }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                customerRequestFromDB.UpdatingBy = null;

        //                db.SaveChanges();
        //                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                return Json(new { success = false, message = "Không có sẵn GameID" }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Logging.LogToDBAsync("/DepositController/ChooseTransaction", ex, $"transactionid={transactionId}&customerrequestid={customerRequestId}");
        //        return Json(new { success = false, message = MessageEndUser.CodeException, }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("exportExcel")]
        //public JsonResult ExportExcel(string searchtext, string statusFilter, bool chbBaoLoi, DateTime? dateFilter)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            string query = @"   SELECT c.GameId, c.GameAccountName, c.PhoneNumber, 
        //                                        cr.Point, cr.MoneyOfPoint, cr.Total, cr.Status, 
        //                                        cr.UpdateBy, cr.AttachFile, cr.Note,
        //                                        cr.isCallAPIError,  cr.ReportErrorMessage, cr.CreatedTime
        //                            FROM tblCustomerRequest cr
        //                            join tblCustomer c on cr.CustomerId = c.Id
        //                            ";
        //            string where = " Where [Type] = 1";
        //            if (!string.IsNullOrEmpty(searchtext))
        //            {
        //                where += $" AND ( c.GameId like N'%{searchtext}%' or c.GameAccountName like N'%{searchtext}%' or c.PhoneNumber like N'%{searchtext}%' or c.BankName like N'%{searchtext}%' or c.BankAccountNumber like N'%{searchtext}%' or c.BankFullName like N'%{searchtext}%' or cr.ToTal like N'%{searchtext}%' or cr.Point like N'%{searchtext}%') ";
        //            }
        //            if (!statusFilter.Equals("all"))
        //            {
        //                where += $" AND (ISNULL(cr.Status,'') = N'{statusFilter}') ";
        //            }
        //            if (chbBaoLoi)
        //            {
        //                where += $" AND (ISNULL(cr.ReportErrorMessage,'') != '') ";
        //            }
        //            if (dateFilter != null)
        //            {
        //                where += $" AND (TRY_CONVERT(date,cr.CreatedTime,103) = Convert(date,'{dateFilter.Value:yyyy-MM-dd}'))";
        //            }
        //            string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
        //            query = $"{query + where} order by CreatedTime desc";
        //            var dataTransaction = db.Database.SqlQuery<DepositExcelModel>(query).ToList();
        //            using (var excelPackage = new ExcelPackage())
        //            {
        //                // Tạo author cho file Excel
        //                excelPackage.Workbook.Worksheets.Add("Deposit");
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
        //                string filePath = $"/Download/Deposit_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
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
        //[HttpGet]
        //public JsonResult getTotalYes(DateTime? dateFilter)
        //{
        //    try
        //    {
        //        double? total = null;
        //        using (var db = new BankAPIEntities())
        //        {
        //            if (dateFilter != null)
        //            {
        //                var date = dateFilter.Value.Date;
        //                total = db.tblTotalUpdates.Where(t => t.Date == date && t.Type).OrderByDescending(t => t.Id).FirstOrDefault()?.Total ?? null;
        //            }
        //            if (total == null)
        //            {
        //                string query = $"select sum(cast(isnull(total,0) as bigint)) from tblCustomerRequest where [Type] = 1 and [Status] = 'yes' and {(dateFilter == null ? "1 = 1" : "convert(date,CreatedTime) = convert(date,'" + dateFilter.Value.ToString("yyyy-MM-dd") + "')")}";
        //                total = db.Database.SqlQuery<long?>(query).FirstOrDefault() ?? 0;
        //            }
        //            return Json(new { success = true, total = total.Value.ToString("N0") }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.LogToDB("Deposit/getTotalYes", ex);
        //        return Json(new { success = false, total = "Lỗi" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}