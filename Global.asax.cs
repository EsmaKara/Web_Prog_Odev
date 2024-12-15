using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Database.SetInitializer(new DatabaseCreator()); // Do�ru Initializer kullan�m�   / migrations kullan�m�na ge�tim
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
