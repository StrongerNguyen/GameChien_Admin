using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FT_Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Login(string UserName, string Password)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                ModelState.AddModelError("error", "Tài khoản không bỏ trống");
                return View();
            }
            if (string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("error", "Mật khẩu không bỏ trống");
                return View();
            }
            using (var db = new GameChienEntities())
            {
                var userFromDB = db.tblUsers.FirstOrDefault(u => u.UserName.Equals(UserName) && u.PassWord.Equals(Password));
                if (userFromDB != null)
                {
                    List<string> listPermissionByRole = db.tblRole_Permission.Where(t => t.RoleName.Equals(userFromDB.RoleName)).Select(t => t.tblPermission.Name).ToList();
                    List<string> listPermissionByUser = db.tblUser_Permission.Where(t => t.UserId.Equals(userFromDB.Id)).Select(t => t.tblPermission.Name).ToList();
                    List<string> totalPermission = listPermissionByRole.Union(listPermissionByUser).ToList();

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, UserName, DateTime.Now, DateTime.Now.AddDays(365), false, $"{string.Join("|", totalPermission)}");
                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    return RedirectToAction("Index", "Transaction");
                }
                else
                {
                    ModelState.AddModelError("error", "Tài khoản không chính xác");
                    return View();
                }
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}