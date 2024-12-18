using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();


        // GET: Home
        public ActionResult HomePage()
        {
            List<Emergency> emergencies = db.Emergencies
                    .OrderBy(e => e.EmergencyDate) // Tarihe göre sıralama (en yakın olanlar önce)
                    .Take(3).ToList();            // İlk 3 kaydı al

            return View(emergencies);
        }
    }
}