using FT_Admin.Models;
using FT_Admin.Models.Data;
using FT_Admin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("PermissionManage")]
    public class PermissionController : Controller
    {
        // GET: Permission
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll(int? curPage = 1)
        {
            using (var db = new GameChienEntities())
            {
                string query = @"   SELECT *, ROW_NUMBER() over (order by Name asc) as r
                                FROM tblPermission ";
                string where = "WHERE 1 = 1 ";

                string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by Name asc";
                string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblPermission
                                            {1}
                                        ", PageSize, where);
                List<PermissionViewModel> data = db.Database.SqlQuery<PermissionViewModel>(query).ToList();
                var maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                ViewBag.CurPage = curPage;
                ViewBag.MaxPage = maxPage;
                return PartialView("_PermissionTable", data);
            }
        }
        public ActionResult GetByName(string Name)
        {
            using (var db = new GameChienEntities())
            {
                var permissionFromDB = db.tblPermissions.FirstOrDefault(x => x.Name == Name);
                if (permissionFromDB != null)
                {
                    PermissionViewModel permission = new PermissionViewModel()
                    {
                        Name = permissionFromDB.Name,
                        Description = permissionFromDB.Description
                    };
                    return PartialView("_PermissionRow", permission);
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult Detail(string Name)
        {
            tblPermission permission;
            using (var db = new GameChienEntities())
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    permission = db.tblPermissions.FirstOrDefault(t => t.Name.Equals(Name));
                }
                else
                {
                    permission = new tblPermission();
                }
                return PartialView("_Detail", new PermissionViewModel()
                {
                    Name = permission.Name,
                    Description = permission.Description
                });
            }
        }
        public async Task<JsonResult> Update(PermissionViewModel permission)
        {
            using (var db = new GameChienEntities())
            {
                var permissionFromDB = db.tblPermissions.FirstOrDefault(t => t.Name.Equals(permission.Name));
                if (permissionFromDB == null)
                {
                    permissionFromDB = new tblPermission()
                    {
                        Name = permission.Name,
                        Description = permission.Description
                    };
                    db.tblPermissions.Add(permissionFromDB);
                    db.SaveChanges();

                    //await Logging.LogChangeAsync("AddRole", roleDto, User.Identity.Name);
                    return Json(new { success = true, message = "Thêm mới thành công.", Name = permissionFromDB.Name }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    permissionFromDB.Description = permission.Description;
                    db.SaveChanges();

                    //await Logging.LogChangeAsync("UpdateRole", roleDto, User.Identity.Name);
                    return Json(new { success = true, message = "Cập nhật thành công.", Name = permissionFromDB.Name }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}