using FT_Admin.Hubs;
using FT_Admin.Models;
using FT_Admin.Models.Data;
using FT_Admin.Models.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using static Common.Logging.Configuration.ArgUtils;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("viewCase")]
    public class CaseController : Controller
    {
        // GET: Case
        public static Dictionary<int, string> CaseStatusMap = new Dictionary<int, string>()
        {
            {-1,"Hủy" },
            {0,"Khởi tạo" },
            {1,"Đã xử lý"}
        };
        public ActionResult Index()
        {
            ViewBag.Status = CaseStatusMap;
            return View();
        }
        public ActionResult GetViewByPage(int? status, string searchText, int? curPage = 1)
        {
            new RealtimeHub().updateCountNotify();
            new RealtimeHub().updateCountTicket();
            using (var db = new BankAPIEntities())
            {
                string query = @"   SELECT *, ROW_NUMBER() over (order by CreatedTime desc) as r
                                FROM tblCase ";
                string where = "WHERE 1 = 1 ";
                if (status != null)
                {
                    where += $" AND Status = {status} ";
                }
                if (!string.IsNullOrEmpty(searchText))
                {
                    where += $" AND ( Title like N'%{searchText}%' or Detail like N'%{searchText}%' or Comment like N'%{searchText}%') ";
                }
                string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by CreatedTime desc";
                string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblCase
                                            {1}
                                        ", PageSize, where);
                List<CaseDto> dataCase = db.Database.SqlQuery<CaseDto>(query).ToList();
                var maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                ViewBag.CurPage = curPage;
                ViewBag.MaxPage = maxPage;
                ViewBag.Status = CaseStatusMap;
                return PartialView("_CaseTable", dataCase);
            }
        }
        public ActionResult GetViewById(Guid Id, int? status, string searchText)
        {
            using (var db = new BankAPIEntities())
            {
                var caseFromDB = db.tblCases.FirstOrDefault(x => x.Id == Id);
                if (caseFromDB != null
                    && (
                    string.IsNullOrEmpty(searchText)
                    || (caseFromDB.Title?.Contains(searchText) ?? false)
                    || (caseFromDB.Detail?.Contains(searchText) ?? false)
                    || (caseFromDB.Comment?.Contains(searchText) ?? false)
                    )
                    && (status == null || caseFromDB.Status == status)
                    )
                {
                    CaseDto caseDto = new CaseDto()
                    {
                        Id = caseFromDB.Id,
                        Title = caseFromDB.Title,
                        Detail = caseFromDB.Detail,
                        Comment = caseFromDB.Comment,
                        AttachFile = caseFromDB.AttachFile,
                        CreatedBy = caseFromDB.CreatedBy,
                        CreatedTime = caseFromDB.CreatedTime,
                        LastUpdateBy = caseFromDB.LastUpdateBy,
                        LastUpdateTime = caseFromDB.LastUpdateTime,
                        Status = caseFromDB.Status
                    };
                    ViewBag.Status = CaseStatusMap;
                    return PartialView("_CaseRow", caseDto);
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult Detail(Guid? Id)
        {
            tblCase caseFromDB = null;
            if (Id != null)
            {
                using (var db = new BankAPIEntities())
                {
                    caseFromDB = db.tblCases.Where(c => c.Id == Id).FirstOrDefault();
                }
            }
            if (caseFromDB == null) caseFromDB = new tblCase();
            ViewBag.Status = CaseStatusMap;
            return PartialView("_Detail", caseFromDB);
        }
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                using (var db = new BankAPIEntities())
                {
                    //Check File
                    if (file == null) return Json(new { success = false, message = "File is null." }, JsonRequestBehavior.AllowGet);
                    if (!file.ContentType.Contains("image/")) return Json(new { success = false, message = "Vui lòng chọn đúng định dạng hình ảnh." }, JsonRequestBehavior.AllowGet);
                    if (file.ContentLength > 10485760) return Json(new { success = false, message = "Kích thước ảnh quá lớn." }, JsonRequestBehavior.AllowGet);

                    string attachFile = Function.UploadImage("", file, User.Identity.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmm"));
                    return Json(new { success = true, attachFile = attachFile }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error(ex);
                return Json(new { success = false, message = "Upload file không thành công. Vui lòng liên hệ Admin để được hỗ trợ." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Update(CaseRequestModel casePost)
        {
            try
            {
                if (casePost.Id.Equals(Guid.Empty))
                {
                    //Thêm mới
                    if (string.IsNullOrEmpty(casePost.Title)) return Json(new { success = false, message = "Vui lòng điền tiêu đề." }, JsonRequestBehavior.AllowGet);
                    if (string.IsNullOrEmpty(casePost.Detail)) return Json(new { success = false, message = "Vui lòng điền chi tiết." }, JsonRequestBehavior.AllowGet);
                    using (var db = new BankAPIEntities())
                    {
                        tblCase caseNew = new tblCase()
                        {
                            Id = Guid.NewGuid(),
                            Title = casePost.Title,
                            Detail = casePost.Detail,
                            AttachFile = casePost.AttachFile,
                            CreatedTime = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            Status = 0
                        };
                        db.tblCases.Add(caseNew);
                        db.SaveChanges();
                        new RealtimeHub().updateCountTicket();
                        return Json(new { success = true, message = "Thêm mới thành công.", caseId = caseNew.Id }, JsonRequestBehavior.AllowGet);
                    };
                }
                else
                {
                    //Cập nhật
                    using (var db = new BankAPIEntities())
                    {
                        tblCase caseFromDB = db.tblCases.FirstOrDefault(t => t.Id == casePost.Id);
                        if (caseFromDB == null) return Json(new { success = false, message = "Không tồn tại Case." }, JsonRequestBehavior.AllowGet);
                        //cập nhật case
                        caseFromDB.Status = casePost.Status;
                        caseFromDB.Comment = (caseFromDB.Comment == null ? "" : (caseFromDB.Comment + "\n")) + User.Identity.Name + ": " + casePost.Comment;
                        caseFromDB.LastUpdateTime = DateTime.Now;
                        caseFromDB.LastUpdateBy = User.Identity.Name;
                        //thêm case history
                        db.tblCaseHistories.Add(new tblCaseHistory()
                        {
                            Id = Guid.NewGuid(),
                            CaseId = caseFromDB.Id,
                            Comment = casePost.Comment,
                            Status = CaseStatusMap[casePost.Status],
                            CreatedBy = User.Identity.Name,
                            CreatedTime = DateTime.Now
                        });
                        db.SaveChanges();
                        new RealtimeHub().updateCountTicket();
                        return Json(new { success = true, message = "Cập nhật Case thành công.", caseId = caseFromDB.Id }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogToDB("Case/Update", ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}