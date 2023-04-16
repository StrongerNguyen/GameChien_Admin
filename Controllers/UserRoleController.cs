using FT_Admin.Models;
using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{

    [CustomAuthorize("0", "userrole")]
    public class UserRoleController : Controller
    {
        // GET: UserRole
        public ActionResult Detail(string UserName)
        {
            using (var db = new BankAPIEntities())
            {
                ViewBag.UserRole = db.tblUser_Role.Where(t => t.UserName.Equals(UserName)).ToList();
                ViewBag.UserName = UserName;
                return PartialView("_Detail", db.tblRoles.OrderBy(t => t.Order).ToList());
            }
        }
        public async Task<JsonResult> Update(string UserName, string RoleName, bool Checked)
        {
            using (var db = new BankAPIEntities())
            {
                var ur = db.tblUser_Role.FirstOrDefault(t => t.UserName.Equals(UserName) && t.RoleName.Equals(RoleName));
                if (Checked && ur == null)
                {
                    tblUser_Role user_Role = new tblUser_Role()
                    {
                        Id = Guid.NewGuid(),
                        RoleName = RoleName,
                        UserName = UserName
                    };
                    db.tblUser_Role.Add(user_Role);
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("AddUserRole", user_Role, User.Identity.Name);
                }
                else if (!Checked && ur != null)
                {
                    db.tblUser_Role.Remove(ur);
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("RemoveUserRole", ur, User.Identity.Name);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}