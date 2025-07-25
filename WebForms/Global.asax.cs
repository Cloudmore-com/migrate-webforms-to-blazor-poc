using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebForms
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SystemWebAdapterConfiguration.AddSystemWebAdapters(this)
                .AddJsonSessionSerializer(options => {
                    options.RegisterKey<bool>("UserInitialized");
                    options.RegisterKey<string>("SessionID");
                    options.RegisterKey<DateTime>("VisitTime");
                    options.RegisterKey<string>("BlazorString");
                })
                .AddRemoteAppServer(options =>
                {
                    options.ApiKey = ConfigurationManager.AppSettings["RemoteAppApiKey"];
                })
                .AddSessionServer();
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Set your session variables here
            Session["UserInitialized"] = true;
            Session["SessionID"] = Session.SessionID;
            Session["VisitTime"] = DateTime.Now;
            // Add more session variables as needed
        }
    }
}