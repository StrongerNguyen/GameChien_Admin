using FT_Admin.Models;
using FT_Admin.Models.Data;
using FT_Admin.Models.Dto;
using FT_Admin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("RoleManage")]
    public class RoleController : Controller
    {
        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll(int? curPage = 1)
        {
            using (var db = new GameChienEntities())
            {
                string query = @"   SELECT *, ROW_NUMBER() over (order by Level asc) as r
                                FROM tblRole ";
                string where = "WHERE 1 = 1 ";

                string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by Level asc";
                string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblRole
                                            {1}
                                        ", PageSize, where);
                List<RoleViewModel> data = db.Database.SqlQuery<RoleViewModel>(query).ToList();
                var maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                ViewBag.CurPage = curPage;
                ViewBag.MaxPage = maxPage;
                return PartialView("_RoleTable", data);
            }
        }
        public ActionResult GetByName(string Name)
        {
            using (var db = new GameChienEntities())
            {
                var roleFromDB = db.tblRoles.FirstOrDefault(x => x.Name == Name);
                if (roleFromDB != null)
                {
                    RoleViewModel roleView = new RoleViewModel()
                    {
                        Name = roleFromDB.Name,
                        Description = roleFromDB.Description,
                        Level = roleFromDB.Level
                    };
                    return PartialView("_RoleRow", roleView);
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult Detail(string Name)
        {
            tblRole role;
            using (var db = new GameChienEntities())
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    role = db.tblRoles.FirstOrDefault(t => t.Name.Equals(Name));
                }
                else
                {
                    role = new tblRole();
                    role.Level = db.tblRoles.Max(t => t.Level) + 1;
                }
                return PartialView("_Detail", new RoleViewModel()
                {
                    Name = role.Name,
                    Level = role.Level,
                    Description = role.Description
                });
            }
        }
        public async Task<JsonResult> Update(RoleViewModel roleView)
        {
            using (var db = new GameChienEntities())
            {
                var roleFromDB = db.tblRoles.FirstOrDefault(t => t.Name.Equals(roleView.Name));
                if (roleFromDB == null)
                {
                    roleFromDB = new tblRole()
                    {
                        Name = roleView.Name,
                        Level = roleView.Level,
                        Description = roleView.Description
                    };
                    db.tblRoles.Add(roleFromDB);
                    db.SaveChanges();

                    //await Logging.LogChangeAsync("AddRole", roleDto, User.Identity.Name);
                    return Json(new { success = true, message = "Thêm mới thành công.", Name = roleFromDB.Name }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    roleFromDB.Description = roleView.Description;
                    roleFromDB.Level = roleView.Level;
                    db.SaveChanges();

                    //await Logging.LogChangeAsync("UpdateRole", roleDto, User.Identity.Name);
                    return Json(new { success = true, message = "Cập nhật thành công.", Name = roleFromDB.Name }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult Permission(string RoleName)
        {
            using (var db = new GameChienEntities())
            {
                tblRole roleFromDB = db.tblRoles.FirstOrDefault(t => t.Name == RoleName);

                ViewBag.RolePermission = db.tblRole_Permission.Where(t => t.RoleName.Equals(RoleName)).Select(t => new RolePermissionViewModel()
                {
                    Id = t.Id,
                    RoleName = t.RoleName,
                    PermissionName = t.PermissionName
                }).ToList();
                ViewBag.Role = new RoleViewModel()
                {
                    Name = roleFromDB.Name,
                    Level = roleFromDB.Level,
                    Description = roleFromDB.Description
                };
                return PartialView("_RolePermission", db.tblPermissions.OrderBy(t => t.Name).Select(t => new PermissionViewModel()
                {
                    Name = t.Name,
                    Description = t.Description
                }).ToList());
            }
        }
        public async Task<JsonResult> UpdateRolePermission(string RoleName, string PermissionName, bool Checked)
        {
            using (var db = new GameChienEntities())
            {
                var rp = db.tblRole_Permission.FirstOrDefault(t => t.RoleName.Equals(RoleName) && t.PermissionName.Equals(PermissionName));
                if (Checked && rp == null)
                {
                    tblRole_Permission role_Permission = new tblRole_Permission()
                    {
                        Id = Guid.NewGuid(),
                        RoleName = RoleName,
                        PermissionName = PermissionName
                    };
                    db.tblRole_Permission.Add(role_Permission);
                    db.SaveChanges();
                    //await Logging.LogChangeAsync("AddUserRole", user_Role, User.Identity.Name);
                    return Json(new { success = true, message = "Thêm quyền thành công" }, JsonRequestBehavior.AllowGet);

                }
                else if (!Checked && rp != null)
                {
                    db.tblRole_Permission.Remove(rp);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Gỡ bỏ quyền thành công" }, JsonRequestBehavior.AllowGet);
                    //await Logging.LogChangeAsync("RemoveUserRole", up, User.Identity.Name);
                }
                return Json(new { success = false, message = "Phân quyền không đúng" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}