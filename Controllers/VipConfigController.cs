using FT_Admin.Models.Data;
using FT_Admin.Models.Dto;
using FT_Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT_Admin.Models.RequestModel;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("VipConfigManage")]
    public class VipConfigController : Controller
    {
        // GET: VipConfig
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetViewByPage(string searchText, int? curPage = 1)
        {
            using (var db = new BankAPIEntities())
            {
                string query = @"   SELECT *, ROW_NUMBER() over (order by FromAmount asc) as r
                                FROM tblVipConfig ";
                string where = "WHERE 1 = 1 ";

                if (!string.IsNullOrEmpty(searchText))
                {
                    where += $" AND ( Name like N'%{searchText}%' ) ";
                }
                string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by FromAmount asc";
                string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblVipConfig
                                            {1}
                                        ", PageSize, where);
                List<VipConfigDto> dataVipConfig = db.Database.SqlQuery<VipConfigDto>(query).ToList();
                var maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                ViewBag.CurPage = curPage;
                ViewBag.MaxPage = maxPage;

                return PartialView("_VipConfigTable", dataVipConfig);
            }
        }
        public ActionResult GetViewById(Guid Id, string searchText)
        {
            using (var db = new BankAPIEntities())
            {
                var vipConfigFromDB = db.tblVipConfigs.FirstOrDefault(x => x.Id == Id);
                if (vipConfigFromDB != null
                    && (
                    string.IsNullOrEmpty(searchText)
                    || (vipConfigFromDB.Name.Contains(searchText))
                    )
                    )
                {
                    VipConfigDto vipConfigDto = new VipConfigDto()
                    {
                        Id = vipConfigFromDB.Id,
                        Name = vipConfigFromDB.Name,
                        FromAmount = vipConfigFromDB.FromAmount,
                        ToAmount = vipConfigFromDB.ToAmount,
                        isActive = vipConfigFromDB.isActive
                    };
                    return PartialView("_VipConfigRow", vipConfigDto);
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult Detail(Guid? Id)
        {
            tblVipConfig vipConfigFromDB = null;
            if (Id != null)
            {
                using (var db = new BankAPIEntities())
                {
                    vipConfigFromDB = db.tblVipConfigs.Where(c => c.Id == Id).FirstOrDefault();
                }
            }
            if (vipConfigFromDB == null)
            {
                vipConfigFromDB = new tblVipConfig();
                vipConfigFromDB.isActive = true;
            }
            return PartialView("_Detail", new VipConfigDto()
            {
                Id = vipConfigFromDB.Id,
                FromAmount = vipConfigFromDB.FromAmount,
                Name = vipConfigFromDB.Name,
                ToAmount = vipConfigFromDB.ToAmount,
                isActive = vipConfigFromDB.isActive
            });
        }
        [HttpPost]
        public JsonResult Update(VipConfigRequestModel vipConfigRequest)
        {
            try
            {
                if (vipConfigRequest.Id.Equals(Guid.Empty))
                {
                    //Thêm mới
                    if (string.IsNullOrEmpty(vipConfigRequest.Name)) return Json(new { success = false, message = "Vui lòng điền Tên." }, JsonRequestBehavior.AllowGet);
                    using (var db = new BankAPIEntities())
                    {
                        tblVipConfig vipConfig = new tblVipConfig()
                        {
                            Id = Guid.NewGuid(),
                            Name = vipConfigRequest.Name,
                            ToAmount = vipConfigRequest.ToAmount,
                            FromAmount = vipConfigRequest.FromAmount,
                            isActive = true
                        };
                        db.tblVipConfigs.Add(vipConfig);
                        db.SaveChanges();
                        return Json(new { success = true, message = "Thêm mới thành công.", Id = vipConfig.Id }, JsonRequestBehavior.AllowGet);
                    };
                }
                else
                {
                    //Cập nhật
                    using (var db = new BankAPIEntities())
                    {
                        tblVipConfig vipConfigFromDB = db.tblVipConfigs.FirstOrDefault(t => t.Id == vipConfigRequest.Id);
                        if (vipConfigFromDB == null) return Json(new { success = false, message = "Không tồn tại VipConfig." }, JsonRequestBehavior.AllowGet);
                        //cập nhật case
                        vipConfigFromDB.Name = vipConfigRequest.Name;
                        vipConfigFromDB.FromAmount = vipConfigRequest.FromAmount;
                        vipConfigFromDB.ToAmount = vipConfigRequest.ToAmount;
                        vipConfigFromDB.isActive = vipConfigRequest.isActive;

                        db.SaveChanges();
                        return Json(new { success = true, message = "Cập nhật VipConfig thành công.", Id = vipConfigFromDB.Id }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogToDB("VipConfig/Update", ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}