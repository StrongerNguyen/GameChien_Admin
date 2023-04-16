using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using FT_Admin.Models.RquestToPlayer;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace FT_Admin.Models
{
    public class PlayerAPI
    {
        private static Dictionary<string, string> apis = new Dictionary<string, string>()
        {
            {"notify",ConfigurationManager.AppSettings["PlayerServer"]+"/api/notifyWebhook" }
        };
        public static async void pushNotifyToPlayer(NotifyToPlayerModel notifyToPlayer)
        {
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.DefaultRequestHeaders.Add("apikey", ConfigurationManager.AppSettings["keyPlayerAPI"]);
                await client.PostAsJsonAsync(apis["notify"], notifyToPlayer);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/pushNotifyToPlayer", ex + "\n" + JsonConvert.SerializeObject(notifyToPlayer));
            }
        }
    }
}