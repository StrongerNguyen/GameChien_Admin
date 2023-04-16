using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FT_Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapRoute(
            //    name: "CustomerRequest",
            //    url: "{controller}/{action}/{type}",
            //    defaults: new { controller = "CustomerRequest", action = "Index", type = "" }
            //);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Transaction", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
