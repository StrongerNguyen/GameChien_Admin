using FT_Admin.Hubs;
using FT_Admin.Models;
using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FT_Admin.Controllers
{
    public class AccountAPIController : ApiController
    {
        [HttpPost]
        [API]
        public async Task<HttpResponseMessage> Post([FromBody] string GameIds)
        {
            await Logging.LogToDBAsync("AccountAPIController/Post", null, GameIds);
            try
            {
                //check GameId
                if (string.IsNullOrEmpty(GameIds))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                using (var db = new BankAPIEntities())
                {
                    List<string> listGameId = GameIds.Split(',').ToList();
                    foreach (var gameId in listGameId)
                    {
                        var customerFromDB = db.tblCustomers.FirstOrDefault(t => t.GameId.Equals(gameId));
                        if (customerFromDB != null)
                        {
                            customerFromDB.isReset = true;
                            customerFromDB.isActive = false;
                            customerFromDB.PhoneNumber = null;
                            customerFromDB.BankName = null;
                            customerFromDB.BankAccountNumber = null;
                            customerFromDB.BankFullName = null;
                            customerFromDB.GameAccountName = null;
                        }
                        else
                        {
                            customerFromDB = new tblCustomer()
                            {
                                Id = Guid.NewGuid(),
                                CreatedTime = DateTime.Now,
                                GameId = gameId,
                                isActive = false,
                                isReset = false
                            };
                            db.tblCustomers.Add(customerFromDB);
                        }
                        await db.SaveChangesAsync();
                        new RealtimeHub().updateCustomer(customerFromDB.Id);
                    }
                    await Logging.LogChangeAsync("JobResetGameId", GameIds, "JOB");
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("AccountAPIController/Reset", ex, GameIds);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
