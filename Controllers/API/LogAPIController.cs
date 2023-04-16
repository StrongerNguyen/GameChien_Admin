using FT_Admin.Models.Data;
using FT_Admin.Models.Webhook;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FT_Admin.Controllers.API
{
    public class LogAPIController : ApiController
    {
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET api/<controller>
        public async Task<HttpResponseMessage> Post(LogOfMobileAppWebhookModel logOfMobileAppWebhook)
        {
            if (logOfMobileAppWebhook != null)
            {
                using (var db = new GameChienEntities())
                {
                    db.tblLogOfMobileApps.Add(new tblLogOfMobileApp()
                    {
                        Id = Guid.NewGuid(),
                        CreatedTime = DateTime.Now,
                        Data = logOfMobileAppWebhook.Data,
                        Source = logOfMobileAppWebhook.Source
                    });
                    await db.SaveChangesAsync();
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
