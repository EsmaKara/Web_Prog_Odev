using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_Prog_Odev.Controllers
{
    public class EmergencyController : Controller
    {


        // Acil durumların girileceği sayfa tasarımı
        public ActionResult EmergencyPage()
        {
            return View();
        }
    }
}