using FT_Admin.Models;
using FT_Admin.Models.Data;
using FT_Admin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("UserManage")]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll(int? curPage = 1)
        {
            using (var db = new GameChienEntities())
            {
                string query = @"   SELECT *, ROW_NUMBER() over (order by CreatedTime desc) as r
                                FROM tblUser ";
                string where = "WHERE 1 = 1 ";

                string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by CreatedTime desc";
                string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblUser
                                            {1}
                                        ", PageSize, where);
                List<UserViewModel> data = db.Database.SqlQuery<UserViewModel>(query).ToList();
                var maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                ViewBag.CurPage = curPage;
                ViewBag.MaxPage = maxPage;
                return PartialView("_UserTable", data);
            }
        }
        public ActionResult GetById(Guid Id)
        {
            using (var db = new GameChienEntities())
            {
                var userFromDB = db.tblUsers.FirstOrDefault(x => x.Id == Id);
                if (userFromDB != null)
                {
                    UserViewModel userView = new UserViewModel()
                    {
                        Id = userFromDB.Id,
                        UserName = userFromDB.UserName,
                        FullName = userFromDB.FullName,
                        PassWord = userFromDB.PassWord,
                        RoleName = userFromDB.RoleName,
                        isActive = userFromDB.isActive
                    };
                    return PartialView("_UserRow", userView);
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult Detail(Guid? Id)
        {
            tblUser user = null;
            using (var db = new GameChienEntities())
            {
                if (Id != null)
                {
                    user = db.tblUsers.FirstOrDefault(t => t.Id.Equals(Id.Value));
                }
                else user = new tblUser();
                ViewBag.ListRole = db.tblRoles.Select(t => new RoleViewModel()
                {
                    Name = t.Name,
                    Level = t.Level,
                    Description = t.Description
                }).ToList();
                return PartialView("_Detail", user);
            }
        }
        public async Task<JsonResult> Update(UserViewModel user)
        {
            using (var db = new GameChienEntities())
            {
                var userFromDb = db.tblUsers.FirstOrDefault(t => t.Id.Equals(user.Id));
                if (userFromDb != null)
                {
                    //check username xem đã có ai dùng chưa
                    if (db.tblUsers.Count(t => t.UserName.Equals(user.UserName) && t.Id != user.Id) > 0)
                    {
                        return Json(new { success = false, message = "UserName đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    }
                    userFromDb.UserName = user.UserName;
                    userFromDb.PassWord = user.PassWord;
                    userFromDb.FullName = user.FullName;
                    userFromDb.RoleName = user.RoleName;
                    userFromDb.isActive = user.isActive;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật User thành công.", Id = userFromDb.Id }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //check username xem đã có ai dùng chưa
                    if (db.tblUsers.Count(t => t.UserName.Equals(user.UserName)) > 0)
                    {
                        return Json(new { success = false, message = "UserName đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    }
                    userFromDb = new tblUser()
                    {
                        Id = Guid.NewGuid(),
                        UserName = user.UserName,
                        PassWord = user.PassWord,
                        FullName = user.FullName,
                        RoleName = user.RoleName,
                        isActive = user.isActive,
                        CreatedTime = DateTime.Now
                    };
                    db.tblUsers.Add(userFromDb);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Thêm User thành công.", Id = userFromDb.Id }, JsonRequestBehavior.AllowGet);

                }
            }
        }
        public ActionResult Permission(Guid UserId)
        {
            using (var db = new GameChienEntities())
            {
                tblUser user = db.tblUsers.FirstOrDefault(t => t.Id == UserId);

                ViewBag.UserPermission = db.tblUser_Permission.Where(t => t.UserId.Equals(UserId)).Select(t => new UserPermissionViewModel()
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    PermissionName = t.PermissionName
                }).ToList();
                ViewBag.User = new UserViewModel()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    isActive = user.isActive,
                    PassWord = user.PassWord,
                    RoleName = user.RoleName,
                    UserName = user.UserName
                };
                return PartialView("_UserPermission", db.tblPermissions.OrderBy(t => t.Name).Select(t => new PermissionViewModel()
                {
                    Name = t.Name,
                    Description = t.Description
                }).ToList());
            }
        }
        public async Task<JsonResult> UpdateUserPermission(Guid UserId, string PermissionName, bool Checked)
        {
            using (var db = new GameChienEntities())
            {
                var up = db.tblUser_Permission.FirstOrDefault(t => t.UserId.Equals(UserId) && t.PermissionName.Equals(PermissionName));
                if (Checked && up == null)
                {
                    tblUser_Permission user_Permission = new tblUser_Permission()
                    {
                        Id = Guid.NewGuid(),
                        UserId = UserId,
                        PermissionName = PermissionName
                    };
                    db.tblUser_Permission.Add(user_Permission);
                    db.SaveChanges();
                    //await Logging.LogChangeAsync("AddUserRole", user_Role, User.Identity.Name);
                    return Json(new { success = true, message = "Thêm quyền thành công" }, JsonRequestBehavior.AllowGet);

                }
                else if (!Checked && up != null)
                {
                    db.tblUser_Permission.Remove(up);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Gỡ bỏ quyền thành công" }, JsonRequestBehavior.AllowGet);
                    //await Logging.LogChangeAsync("RemoveUserRole", up, User.Identity.Name);
                }
                return Json(new { success = false, message = "Phân quyền không đúng" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}