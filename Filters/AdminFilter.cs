using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_Prog_Odev.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["IsAdmin"] == null || !(bool)filterContext.HttpContext.Session["IsAdmin"])
            {
                filterContext.Result = new RedirectResult("/Admin/Login"); // Login sayfasına yönlendir
            }
        }
    }
}