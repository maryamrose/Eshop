﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using GSD.Globalization;
using System.Threading;
using System.Web.Hosting;
using DataLayer;


namespace MyEshop
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            HttpContext.Current.Application["Online"]= 0;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var persianCulture = new PersianCulture();
            Thread.CurrentThread.CurrentCulture = persianCulture;
            Thread.CurrentThread.CurrentUICulture = persianCulture;
        }

        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

        protected void Session_Start()
        {
            int online =int.Parse(HttpContext.Current.Application["Online"].ToString());
            online += 1;
            HttpContext.Current.Application["Online"] = online;
            string ip = Request.UserHostAddress;
            DateTime dtNow = DateTime.Now.Date;
            using (EshopUnitOfWork db = new EshopUnitOfWork()) {
                if (!db.VisitSiteRepository.GetAll(v => v.IP == ip && v.Date == dtNow).Any()) { 
               
                VisitSite visitsite = new VisitSite()
                {
                    IP = ip,
                    Date = dtNow
                };
                db.VisitSiteRepository.Insert(visitsite);
                db.Save();
            }
            }
        }

        protected void Session_End() {
            int online = int.Parse(HttpContext.Current.Application["Online"].ToString());
            online -= 1;
            HttpContext.Current.Application["Online"] = online;
        }
    }
}