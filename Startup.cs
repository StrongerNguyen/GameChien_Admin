using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using FT_Admin.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(FT_Admin.Startup))]

namespace FT_Admin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            WithdrawSuccessQueue.Queues = new System.Collections.Generic.HashSet<Guid>();

            var check = bool.Parse(ConfigurationManager.AppSettings["isRelease"]);
            if (check)
            {
                Scheduler sc = new Scheduler();
                sc.Start();
            }
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration { };
                hubConfiguration.EnableDetailedErrors = true;
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
