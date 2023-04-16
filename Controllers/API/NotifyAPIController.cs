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
    public class NotifyAPIController : ApiController
    {
        //public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET api/<controller>
        public async Task<HttpResponseMessage> Post(NotificationWebhookModel notificationWebhook)
        {
            if (notificationWebhook != null)
            {
                notificationWebhook.Data = notificationWebhook.Data.Trim();
                notificationWebhook.Data2 = notificationWebhook.Data2.Trim();

                //check xem có notify nào giống hệt trong 10s không
                var timeCheck = DateTime.Now.AddSeconds(-10);
                using (var db = new GameChienEntities())
                {
                    if ((!string.IsNullOrEmpty(notificationWebhook.Data) || !string.IsNullOrEmpty(notificationWebhook.Data2)) && db.tblNotifications.Count(t => (t.Data.Equals(notificationWebhook.Data) || t.Data2.Equals(notificationWebhook.Data)) && t.CreatedTime >= timeCheck) == 0)
                    {
                        tblNotification notification = new tblNotification()
                        {
                            Id = Guid.NewGuid(),
                            Type = notificationWebhook.Type,
                            DeviceName = notificationWebhook.DeviceName,
                            From = notificationWebhook.From,
                            Data = notificationWebhook.Data,
                            Data2 = notificationWebhook.Data2,
                            Executed = false,
                            CreatedTime = DateTime.Now
                        };
                        db.tblNotifications.Add(notification);
                        await db.SaveChangesAsync();
                    }
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
