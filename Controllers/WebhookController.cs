using FT_Admin.Hubs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace FT_Admin.Controllers
{
    public class WebhookController : ApiController
    {
        // POST api/<controller>
        [HttpPost]
        public void NewCustomerRequest([FromBody] Guid customerRequestId)
        {
            new RealtimeHub().updateCustomerRequest(customerRequestId, true);
        }
    }
}