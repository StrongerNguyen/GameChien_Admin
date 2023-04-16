using FT_Admin.Hubs;
using FT_Admin.Models;
using FT_Admin.Models.Data;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FT_Admin.Models.Excel;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System.IO;
using System.Data.Entity;
using FT_Admin.Models.ViewModel;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("WithdrawManage")]
    public class WithdrawController : Controller
    {
        // GET: Withdraw
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll(string searchText, bool? statusFilter, DateTime? dateFilter, int? curPage = 1)
        {
            //new RealtimeHub().updateCountNotify();
            //new RealtimeHub().updateCountTicket();
            using (var db = new GameChienEntities())
            {
                string query = @"   SELECT	w.Id, w.Amount, w.Status,w.CreatedTime, u.UserName as UpdatedByUserName, w.UpdatedTime, 
		                                    p.AccountName, p.PhoneNumber, 
		                                    b.BankName, b.ShortName, b.Bin, 
		                                    bab.BankAccountNumber, bab.BankFullName,
		                                    ROW_NUMBER() over (order by w.CreatedTime desc) as r
                                    FROM tblWithdraw w
                                    JOIN tblBankAccountByPlayer bab on w.BankAccountByPlayerId = bab.Id
                                    JOIN tblPlayer p on bab.PlayerId = p.Id
                                    JOIN tblBank b on bab.BankId = b.Id
                                    LEFT JOIN tblUser u on w.UpdatedBy = u.Id
                                    ";
                string where = " Where 1 = 1";
                if (!string.IsNullOrEmpty(searchText))
                {
                    where += $" AND ( p.AccountName like N'%{searchText}%' or p.PhoneNumber like N'%{searchText}%' or bab.BankAccountNumber like N'%{searchText}%' or bab.BankFullName like N'%{searchText}%' ) ";
                }
                if (statusFilter != null)
                {
                    where += $" AND (cr.Status = {statusFilter} ) ";
                }
                if (dateFilter != null)
                {
                    where += $" AND (TRY_CONVERT(date,w.CreatedTime,103) = Convert(date,'{dateFilter.Value:yyyy-MM-dd}'))";
                }
                string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by CreatedTime desc";
                string queryMaxPage = string.Format(@"  SELECT  CASE 
                                                                    When COUNT(*)%{0} = 0 
                                                                        then count(*)/{0}
                                                                    else ((count(*)/{0}) + 1) 
                                                                end
                                                        FROM tblWithdraw w
                                                        JOIN tblBankAccountByPlayer bab on w.BankAccountByPlayerId = bab.Id
                                                        JOIN tblPlayer p on bab.PlayerId = p.Id
                                                        JOIN tblBank b on bab.BankId = b.Id
                                                        LEFT JOIN tblUser u on w.UpdatedBy = u.Id
                                            {1}", PageSize, where);

                List<WithdrawViewModel> dataCustomerRequest = db.Database.SqlQuery<WithdrawViewModel>(query).ToList();
                ViewBag.maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                ViewBag.curPage = curPage;
                return PartialView("_WithdrawTable", dataCustomerRequest);
            }
        }
        public ActionResult GetById(Guid Id, string searchText, DateTime? dateFilter, bool? statusFilter)
        {
            try
            {
                using (var db = new GameChienEntities())
                {
                    tblWithdraw withdrawFromDB = db.tblWithdraws.FirstOrDefault(t => t.Id.Equals(Id));
                    if (withdrawFromDB != null
                        && (string.IsNullOrEmpty(searchText)
                        || withdrawFromDB.tblBankAccountByPlayer.tblPlayer.AccountName.Contains(searchText)
                        || withdrawFromDB.tblBankAccountByPlayer.tblPlayer.PhoneNumber.Contains(searchText)
                        || withdrawFromDB.tblBankAccountByPlayer.BankAccountNumber.Contains(searchText)
                        || withdrawFromDB.tblBankAccountByPlayer.BankFullName.Contains(searchText)
                        )
                        && ((statusFilter == null && withdrawFromDB.Status == null) || (statusFilter != null && withdrawFromDB.Status == statusFilter))
                        && (dateFilter == null || withdrawFromDB.CreatedTime.Date == dateFilter.Value.Date)
                    )
                    {
                        return PartialView("_WithdrawRow", new WithdrawViewModel()
                        {
                            Id = withdrawFromDB.Id,
                            Amount = withdrawFromDB.Amount,
                            Status = withdrawFromDB.Status,
                            CreatedTime = withdrawFromDB.CreatedTime,
                            UpdatedByUserName = withdrawFromDB.tblUser?.UserName ?? "",
                            UpdatedTime = withdrawFromDB.UpdatedTime,
                            AccountName = withdrawFromDB.tblBankAccountByPlayer.tblPlayer.AccountName,
                            PhoneNumber = withdrawFromDB.tblBankAccountByPlayer.tblPlayer.PhoneNumber,
                            BankName = withdrawFromDB.tblBankAccountByPlayer.tblBank.BankName,
                            ShortName = withdrawFromDB.tblBankAccountByPlayer.tblBank.ShortName,
                            Bin = withdrawFromDB.tblBankAccountByPlayer.tblBank.Bin,
                            BankAccountNumber = withdrawFromDB.tblBankAccountByPlayer.BankAccountNumber,
                            BankFullName = withdrawFromDB.tblBankAccountByPlayer.BankFullName
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogToDB("Withdraw/RenderRow", ex);
            }
            return null;
        }
        //public ActionResult Detail(Guid customerRequestId)
        //{
        //    using (var db = new BankAPIEntities())
        //    {
        //        tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(customerRequestId));
        //        if (customerRequestFromDB == null) return Json(new { success = false, message = "Lệnh không tồn tại." }, JsonRequestBehavior.AllowGet);
        //        customerRequestFromDB.UpdatingBy = User.Identity.Name;
        //        db.SaveChanges();
        //        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        //        {
        //            Id = customerRequestFromDB.Id,
        //            GameId = customerRequestFromDB.tblCustomer.GameId,
        //            GameAccountName = customerRequestFromDB.tblCustomer.GameAccountName,
        //            PhoneNumber = customerRequestFromDB.tblCustomer.PhoneNumber,
        //            BankName = customerRequestFromDB.BankName,
        //            BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //            BankFullName = customerRequestFromDB.BankFullName,
        //            Type = customerRequestFromDB.Type,
        //            Point = customerRequestFromDB.Point.ToString("N2"),
        //            MoneyOfPoint = (customerRequestFromDB.MoneyOfPoint == null ? "" : ((customerRequestFromDB.MoneyOfPoint.Value / 1000).ToString("N0"))),
        //            Total = customerRequestFromDB.Total,
        //            CreatedTime = customerRequestFromDB.CreatedTime,
        //            CustomerId = customerRequestFromDB.CustomerId,
        //            Status = customerRequestFromDB.Status,
        //            isActive = customerRequestFromDB.isActive,
        //            UpdateBy = customerRequestFromDB.UpdateBy,
        //            SubtractPointBy = customerRequestFromDB.SubtractPointBy,
        //            SendFromBank = customerRequestFromDB.SendFromBank,
        //            AttachFile = customerRequestFromDB.AttachFile,
        //            Note = customerRequestFromDB.Note,
        //            ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //            BankBin = db.tblBankLists.FirstOrDefault(a => a.ShortName.Equals(customerRequestFromDB.BankName))?.Bin ?? ""
        //        };
        //        ViewBag.listNoteRecommend = db.tblContentRecommends.Where(t => t.Name.Equals("withdraw_no")).Select(t => t.Content).ToList();
        //        ViewBag.listBankRecommend = listBankRecommend;
        //        tblConfig config = db.tblConfigs.FirstOrDefault();
        //        if (config != null)
        //        {
        //            ViewBag.QRCodeTemplate = config.QRCodeTemplate;
        //        }
        //        return PartialView("_Detail", customerRequestDto);
        //    }
        //}
        //[CustomAuthorize("0", "actionWithdraw")]
        //public async Task<ActionResult> SubtractPoint(Guid customerRequestid)
        //{
        //    tblCustomerRequest customerRequestFromDB = null;
        //    using (var db = new BankAPIEntities())
        //    {
        //        try
        //        {
        //            customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(customerRequestid));
        //            if (customerRequestFromDB == null) return Json(new { success = false, message = "Lệnh rút không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            if (customerRequestFromDB.isProcessing) return Json(new { success = false, message = "Lệnh rút này đang được xử lý. Vui lòng đợi" }, JsonRequestBehavior.AllowGet);

        //            //Update đang xử lý
        //            customerRequestFromDB.UpdatingBy = User.Identity.Name;
        //            db.SaveChanges();
        //            //push cho user biết
        //            new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            tblCustomer customerFromDB = customerRequestFromDB.tblCustomer;
        //            //trừ điểm
        //            SubPointAccountModel subPointAccount = await GameAccountAPI.SubPointAccountAsync(customerFromDB.GameId, customerRequestFromDB.Point, "", User.Identity.Name);
        //            if (subPointAccount == null || !subPointAccount.status)
        //            {
        //                //Thất bại
        //                customerRequestFromDB.UpdatingBy = null;
        //                customerRequestFromDB.ReportErrorMessage = (subPointAccount == null ? "Call API Subtract Point thất bại" : subPointAccount.message);
        //                await db.SaveChangesAsync();
        //                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                return Json(new { success = false, message = "Trừ điểm thất bại.\n Lỗi: " + (subPointAccount?.message ?? "Call API Subtract Point thất bại.") }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                //Trừ điểm thành công
        //                //Cập nhật request
        //                customerRequestFromDB.UpdatingBy = null;
        //                customerRequestFromDB.SubtractPointBy = User.Identity.Name;
        //                //Cập nhật customer
        //                customerFromDB.GameAccountName = subPointAccount.loginName;
        //                customerFromDB.Balance = null;


        //                await db.SaveChangesAsync();
        //                await Logging.LogChangeAsync("Subtract Point Success", new
        //                {
        //                    Id = customerRequestFromDB.Id,
        //                    GameId = customerFromDB.GameId,
        //                    GameAccountName = customerFromDB.GameAccountName,
        //                    MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                    Point = customerRequestFromDB.Point,
        //                    Total = customerRequestFromDB.Total,
        //                    CreatedTime = customerRequestFromDB.CreatedTime
        //                }, User.Identity.Name);

        //                //Push update request
        //                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);


        //                return Json(new { success = true, message = "Trừ điểm thành công." }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await Logging.LogToDBAsync("WithdrawController/SubtractPoint", ex, $"id={customerRequestid}");
        //            //Thất bại
        //            if (customerRequestFromDB != null)
        //            {
        //                customerRequestFromDB.UpdatingBy = null;
        //                customerRequestFromDB.ReportErrorMessage = "Lỗi hệ thống. Xem log WithdrawController/SubtractPoint";
        //                await db.SaveChangesAsync();
        //            }
        //            return Json(new { success = false, message = MessageEndUser.CodeException, }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}
        ///// <summary>
        ///// ajax call
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="status"></param>
        ///// <param name="note"></param>
        ///// <returns></returns>
        //[CustomAuthorize("0", "actionWithdraw")]
        //public async Task<ActionResult> UpdateStatus(Guid id, string status, string note)
        //{
        //    try
        //    {
        //        //check input
        //        if (string.IsNullOrEmpty(status)) return Json(new { success = false, message = "Trạng thái không bỏ trống." }, JsonRequestBehavior.AllowGet);
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(id));

        //            if (customerRequestFromDB == null) return Json(new { success = false, message = "Lệnh rút không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            if (customerRequestFromDB.isProcessing && status.Equals("yes")) return Json(new { success = false, message = "Lệnh rút này đang được xử lý. Vui lòng đợi" }, JsonRequestBehavior.AllowGet);
        //            if (!string.IsNullOrEmpty(customerRequestFromDB.Status) && !status.Equals("reporterror")) return Json(new { success = false, message = "Lệnh rút này đã được xử lý." }, JsonRequestBehavior.AllowGet);
        //            tblCustomer customerFromDB = db.tblCustomers.FirstOrDefault(t => t.Id == customerRequestFromDB.CustomerId);
        //            if (customerFromDB == null) return Json(new { success = false, message = "Khách hàng này không tồn tại." }, JsonRequestBehavior.AllowGet);

        //            //Update đang xử lý
        //            customerRequestFromDB.isProcessing = true;
        //            customerRequestFromDB.UpdateBy = User.Identity.Name;
        //            await db.SaveChangesAsync();

        //            //push cho user biết
        //            new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);

        //            if (status.Equals("yes"))
        //            {
        //                if (string.IsNullOrEmpty(customerRequestFromDB.ReportErrorMessage))
        //                {

        //                    //subtract point
        //                    SubPointAccountModel subPointAccount = await GameAccountAPI.SubPointAccountAsync(customerFromDB.GameId, customerRequestFromDB.Point, note);
        //                    if (subPointAccount == null || !subPointAccount.status)
        //                    {
        //                        //Thất bại
        //                        customerRequestFromDB.isProcessing = false;
        //                        customerRequestFromDB.UpdateBy = null;
        //                        customerRequestFromDB.isCallAPIError = (subPointAccount == null ? true : false);
        //                        customerFromDB.Balance = null;
        //                        await db.SaveChangesAsync();

        //                        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                        return Json(new { success = false, message = "Cập nhật trạng thái " + status.ToUpper() + " thất bại.\n Lỗi: " + (subPointAccount?.message ?? "API Withdraw") }, JsonRequestBehavior.AllowGet);
        //                    }
        //                    else
        //                    {
        //                        //Thành công
        //                        //Cập nhật request
        //                        customerRequestFromDB.isProcessing = false;
        //                        customerRequestFromDB.Status = status;
        //                        customerRequestFromDB.Note = note;
        //                        //Cập nhật customer
        //                        customerFromDB.GameAccountName = subPointAccount.loginName;
        //                        customerFromDB.Balance = null;


        //                        await db.SaveChangesAsync();
        //                        await Logging.LogChangeAsync("Withdraw", new
        //                        {
        //                            Id = customerRequestFromDB.Id,
        //                            BankName = customerRequestFromDB.BankName,
        //                            BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //                            BankFullName = customerRequestFromDB.BankFullName,
        //                            MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                            Point = customerRequestFromDB.Point,
        //                            Total = customerRequestFromDB.Total,
        //                            Note = customerRequestFromDB.Note,
        //                            CreatedTime = customerRequestFromDB.CreatedTime,
        //                            ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //                            Status = customerRequestFromDB.Note,
        //                            Type = customerRequestFromDB.Type
        //                        }, User.Identity.Name);

        //                        //Push update request
        //                        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                        //Push update customer
        //                        new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, false);
        //                        //Push update totalSum
        //                        var date = DateTime.Now.Date;
        //                        var totalUpdateFromDB = db.tblTotalUpdates.Where(t => t.Date == date && !t.Type).OrderByDescending(t => t.Id).FirstOrDefault();
        //                        if (totalUpdateFromDB != null)
        //                        {
        //                            totalUpdateFromDB.Total += customerRequestFromDB.Total ?? 0;
        //                            await db.SaveChangesAsync();
        //                        }
        //                        new RealtimeHub().updateTotalWithdraw();
        //                    }
        //                }
        //                else
        //                {
        //                    customerRequestFromDB.isProcessing = false;
        //                    customerRequestFromDB.Status = status;
        //                    customerRequestFromDB.ReportErrorMessage = null;
        //                    customerFromDB.Balance = null;
        //                    await db.SaveChangesAsync();
        //                    new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                }
        //            }
        //            else if (status.Equals("no"))
        //            {
        //                customerRequestFromDB.isProcessing = false;
        //                customerRequestFromDB.Status = status;
        //                customerRequestFromDB.Note = note;
        //                customerFromDB.Balance = null;
        //                await db.SaveChangesAsync();
        //                await Logging.LogChangeAsync("Withdraw", new
        //                {
        //                    Id = customerRequestFromDB.Id,
        //                    BankName = customerRequestFromDB.BankName,
        //                    BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //                    BankFullName = customerRequestFromDB.BankFullName,
        //                    MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                    Point = customerRequestFromDB.Point,
        //                    Total = customerRequestFromDB.Total,
        //                    Note = customerRequestFromDB.Note,
        //                    CreatedTime = customerRequestFromDB.CreatedTime,
        //                    ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //                    Status = customerRequestFromDB.Note,
        //                    Type = customerRequestFromDB.Type
        //                }, User.Identity.Name);
        //                new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, false);
        //                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            }
        //            else if (status.Equals("reporterror"))
        //            {
        //                tblCashOutSuccessWithBank cashOutSuccessWithBank = db.tblCashOutSuccessWithBanks.FirstOrDefault(t => t.CustomerRequestId == customerRequestFromDB.Id);
        //                if (cashOutSuccessWithBank != null)
        //                {
        //                    cashOutSuccessWithBank.isActive = false;
        //                }
        //                customerRequestFromDB.isProcessing = false;
        //                customerRequestFromDB.Status = null;
        //                customerRequestFromDB.ReportErrorMessage = note;
        //                customerFromDB.Balance = null;
        //                await db.SaveChangesAsync();
        //                await Logging.LogChangeAsync("Withdraw", new
        //                {
        //                    Id = customerRequestFromDB.Id,
        //                    BankName = customerRequestFromDB.BankName,
        //                    BankAccountNumber = customerRequestFromDB.BankAccountNumber,
        //                    BankFullName = customerRequestFromDB.BankFullName,
        //                    MoneyOfPoint = customerRequestFromDB.MoneyOfPoint,
        //                    Point = customerRequestFromDB.Point,
        //                    Total = customerRequestFromDB.Total,
        //                    Note = customerRequestFromDB.Note,
        //                    CreatedTime = customerRequestFromDB.CreatedTime,
        //                    ReportErrorMessage = customerRequestFromDB.ReportErrorMessage,
        //                    Status = customerRequestFromDB.Note,
        //                    Type = customerRequestFromDB.Type
        //                }, User.Identity.Name);
        //                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            }
        //            return Json(new { success = true, message = "Cập nhật trạng thái " + status.ToUpper() + " thành công" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Logging.LogToDBAsync("/WithdrawController/UpdateStatus", ex, $"id={id}&status={status}");
        //        return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("0", "actionWithdraw")]
        //public async Task<ActionResult> UpdateBankSend(Guid customerRequestId, string bankcashout)
        //{
        //    try
        //    {
        //        bool check = WithdrawSuccessQueue.Queues.Add(customerRequestId);

        //        if (!check) return Json(new { success = false, message = "Bạn đang ấn quá nhiều lần, vui lòng đợi." }, JsonRequestBehavior.AllowGet);
        //        //check input
        //        if (!string.IsNullOrEmpty(bankcashout) && listBankRecommend.Count(t => t.Equals(bankcashout)) == 0) return Json(new { success = false, message = "Ngân hàng chuyển tiền không hợp lệ." }, JsonRequestBehavior.AllowGet);
        //        if (ConfigurationManager.AppSettings["Website"].Equals("Index SVB") && string.IsNullOrEmpty(bankcashout)) return Json(new { success = false, message = "Vui lòng chọn ngân hàng chuyển tiền" }, JsonRequestBehavior.AllowGet);
        //        if (ConfigurationManager.AppSettings["Website"].Equals("Index SVB") && listBankRecommend.Count(t => t == bankcashout) == 0) return Json(new { success = false, message = "Ngân hàng chuyển tiền không hợp lệ." }, JsonRequestBehavior.AllowGet);
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(customerRequestId));
        //            if (customerRequestFromDB == null) return Json(new { success = false, message = "Lệnh rút không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            customerRequestFromDB.SendFromBank = bankcashout;
        //            customerRequestFromDB.Status = "yes";
        //            customerRequestFromDB.UpdateBy = User.Identity.Name;
        //            customerRequestFromDB.isCallAPIError = false;
        //            customerRequestFromDB.ReportErrorMessage = null;
        //            customerRequestFromDB.tblCustomer.Balance = null;
        //            customerRequestFromDB.Note = customerRequestFromDB.CreatedTime.ToString("ddMMHHmm");
        //            if (!string.IsNullOrEmpty(bankcashout))
        //            {
        //                db.tblCashOutSuccessWithBanks.Add(new tblCashOutSuccessWithBank()
        //                {
        //                    Id = Guid.NewGuid(),
        //                    BankName = bankcashout,
        //                    CreatedBy = User.Identity.Name,
        //                    CreatedTime = DateTime.Now,
        //                    CustomerRequestId = customerRequestFromDB.Id,
        //                    isActive = true,
        //                    TotalMoney = (int)(customerRequestFromDB.Total ?? 0)
        //                });
        //            }
        //            db.SaveChanges();
        //            //push cho user biết
        //            new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            //Push update customer
        //            new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, false, customerRequestFromDB.Note);
        //            //Push update totalSum
        //            var date = DateTime.Now.Date;
        //            var totalUpdateFromDB = db.tblTotalUpdates.Where(t => t.Date == date && !t.Type).OrderByDescending(t => t.Id).FirstOrDefault();
        //            if (totalUpdateFromDB != null)
        //            {
        //                totalUpdateFromDB.Total += customerRequestFromDB.Total ?? 0;
        //                await db.SaveChangesAsync();
        //            }
        //            new RealtimeHub().updateTotalWithdraw();

        //            return Json(new { success = true, message = "Thành công." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Logging.LogToDBAsync("/WithdrawController/UpdateBankSend", ex, $"id={customerRequestId}&bankcashout={bankcashout}");
        //        return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("0", "actionWithdraw")]
        //public async Task<ActionResult> ReportCustomerRequest(Guid customerRequestId, string note)
        //{
        //    try
        //    {
        //        //check input
        //        if (string.IsNullOrEmpty(note)) return Json(new { success = false, message = "Vui lòng điền ghi chú." }, JsonRequestBehavior.AllowGet);
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(customerRequestId));
        //            if (customerRequestFromDB == null) return Json(new { success = false, message = "Lệnh rút không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            customerRequestFromDB.ReportErrorMessage = note;
        //            customerRequestFromDB.UpdateBy = User.Identity.Name;
        //            db.SaveChanges();
        //            //push cho user biết
        //            new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            return Json(new { success = true, message = "Cập nhật thành công." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Logging.LogToDBAsync("/WithdrawController/ReportCustomerRequest", ex, $"id={customerRequestId}&note={note}");
        //        return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("0", "actionWithdraw")]
        //public async Task<ActionResult> UpdateNo(Guid customerRequestId, string note)
        //{
        //    try
        //    {
        //        //check input
        //        if (string.IsNullOrEmpty(note)) return Json(new { success = false, message = "Vui lòng điền ghi chú." }, JsonRequestBehavior.AllowGet);
        //        using (var db = new BankAPIEntities())
        //        {
        //            tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.FirstOrDefault(t => t.Id.Equals(customerRequestId));
        //            if (customerRequestFromDB == null) return Json(new { success = false, message = "Lệnh rút không tồn tại." }, JsonRequestBehavior.AllowGet);
        //            customerRequestFromDB.Status = "no";
        //            customerRequestFromDB.UpdateBy = User.Identity.Name;
        //            customerRequestFromDB.Note = note;
        //            customerRequestFromDB.isCallAPIError = null;
        //            customerRequestFromDB.ReportErrorMessage = null;
        //            db.SaveChanges();
        //            //push cho user biết
        //            new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //            //Push update customer
        //            new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, false, customerRequestFromDB.Note);
        //            return Json(new { success = true, message = "Cập nhật thành công." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Logging.LogToDBAsync("/WithdrawController/UpdateNo", ex, $"id={customerRequestId}&note={note}");
        //        return Json(new { success = false, message = MessageEndUser.CodeException }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[CustomAuthorize("0", "editTotalWithdraw")]
        //public async Task<JsonResult> ChangeTotal(string exp, double val)
        //{
        //    using (var db = new BankAPIEntities())
        //    {
        //        var date = DateTime.Now.Date;
        //        // lấy cập nhật mới nhất
        //        var totalUpdateFromDB = db.tblTotalUpdates.Where(t => t.Date == date && !t.Type).OrderByDescending(t => t.Id).FirstOrDefault();
        //        double total = totalUpdateFromDB == null ? (db.Database.SqlQuery<double?>("select sum(total) from tblCustomerRequest where [Type] = 0 and [Status] = 'yes' and convert(date,CreatedTime) = convert(date,getdate())").FirstOrDefault() ?? 0) : totalUpdateFromDB.Total;
        //        tblTotalUpdate totalUpdate = new tblTotalUpdate()
        //        {
        //            Date = date,
        //            TotalOld = total,
        //            Amount = (exp.Equals("+") ? val : -val),
        //            Total = exp.Equals("+") ? (total + val) : (total - val),
        //            Type = false
        //        };
        //        db.tblTotalUpdates.Add(totalUpdate);
        //        await db.SaveChangesAsync();
        //        await Logging.LogChangeAsync("Withdraw", totalUpdate, User.Identity.Name);
        //        return Json(new { success = true, total = (exp.Equals("+") ? (total + val) : (total - val)).ToString("N0") }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[CustomAuthorize("exportExcel")]
        //public JsonResult ExportExcel(string searchtext, string statusFilter, bool chbBaoLoi, DateTime? dateFilter)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            string query = @"   SELECT c.GameId, c.GameAccountName, c.PhoneNumber, c.BankName, c.BankAccountNumber, c.BankFullName,
        //                                        cr.Point, cr.MoneyOfPoint, cr.Total, cr.Status, 
        //                                        cr.UpdateBy, cr.AttachFile, cr.Note,
        //                                        cr.isCallAPIError,  cr.ReportErrorMessage, cr.CreatedTime, cr.SendFromBank
        //                            FROM tblCustomerRequest cr
        //                            join tblCustomer c on cr.CustomerId = c.Id
        //                            ";
        //            string where = " Where [Type] = 0";
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
        //            query = $"{query + where} order by CreatedTime desc";
        //            var dataTransaction = db.Database.SqlQuery<WithdrawExcelModel>(query).ToList();
        //            using (var excelPackage = new ExcelPackage())
        //            {
        //                // Tạo author cho file Excel
        //                excelPackage.Workbook.Worksheets.Add("Withdraw");
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
        //                string filePath = $"/Download/Withdraw_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
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
        //[CustomAuthorize("viewTotalCashoutByBank")]
        //public JsonResult ExportExcelCashoutSuccessByBank(DateTime? dateFilter)
        //{
        //    try
        //    {
        //        using (var db = new BankAPIEntities())
        //        {
        //            var dateNow = DateTime.Now.Date;
        //            var dataTransaction = db.tblCashOutSuccessWithBanks.Where(t => DbFunctions.TruncateTime(t.CreatedTime) == dateNow).ToList().Select(t => new SummaryCashoutWithBankExcelModel()
        //            {
        //                CreatedTime = t.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss"),
        //                BankName = t.BankName,
        //                CreatedBy = t.CreatedBy,
        //                SendTo_BankName = t.tblCustomerRequest.BankName,
        //                SendTo_BankAccountNumber = t.tblCustomerRequest.BankAccountNumber,
        //                SendTo_BankFullName = t.tblCustomerRequest.BankFullName,
        //                TotalMoney = t.TotalMoney,
        //                isActive = t.isActive
        //            });
        //            using (var excelPackage = new ExcelPackage())
        //            {
        //                // Tạo author cho file Excel
        //                excelPackage.Workbook.Worksheets.Add("CashoutWithBank");
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
        //                string filePath = $"/Download/CashoutWithBank_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
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