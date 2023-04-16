using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //private BankAPIEntities db = new BankAPIEntities();
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            foreach (var role in allowedroles)
            {
                if (httpContext.User.IsInRole(role))
                {
                    authorize = true;
                }
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
            {
                controller = "Account",
                action = "Logout"
            }));
        }
    }
}