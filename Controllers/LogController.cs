using FT_Admin.Models;
using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("LogManage")]
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index()
        {
            using (var db = new BankAPIEntities())
            {
                ViewBag.Names = db.tblLogChanges.GroupBy(t => t.Name).Select(t => t.Key).OrderByDescending(t => t).ToList();
                return View();
            }
        }
        public ActionResult GetData(string Name, string Data, DateTime? FromDate, DateTime? ToDate)
        {
            if (string.IsNullOrEmpty(Name)) return Json(new { success = false, message = "Vui lòng chọn Name" }, JsonRequestBehavior.AllowGet);
            if (FromDate == null) return Json(new { success = false, message = "Vui lòng chọn Từ ngày" }, JsonRequestBehavior.AllowGet);
            if (ToDate == null) return Json(new { success = false, message = "Vui lòng chọn Đến ngày" }, JsonRequestBehavior.AllowGet);
            using (var db = new BankAPIEntities())
            {
                var fromDate = FromDate.Value.Date;
                var toDate = ToDate.Value.Date;
                var data = db.tblLogChanges.Where(t => t.Name.Equals(Name)
                && DbFunctions.TruncateTime(t.CreatedTime) >= fromDate
                && DbFunctions.TruncateTime(t.CreatedTime) <= toDate
                && (string.IsNullOrEmpty(Data) || t.Data.Contains(Data))).OrderByDescending(t => t.CreatedTime).ToList();
                return PartialView("_LogTable", data);
            }
        }
    }
}