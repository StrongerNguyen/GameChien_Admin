using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FT_Admin.Models
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class APIAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.Request.Headers.TryGetValues("SecretKey", out var secretKey))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                return;
            }
            if (!ConfigurationManager.AppSettings["SecretKey"].ToString().Equals(secretKey.FirstOrDefault()))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                return ;
            }
        }
    }
}