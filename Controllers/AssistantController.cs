using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class AssistantController : Controller
    {
        DatabaseContext db = new DatabaseContext();


        // Asistanların tanıtıldığı sayfa tasarımı
        public ActionResult AssistantPage()
        {
            List<Assistant> assistants = db.Assistants.ToList();
            return View(assistants);
        }
    }
}