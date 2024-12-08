using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class MainController : Controller
    {
        // Anasayfa tasarımı
        public ActionResult HomePage()
        {
            return View();
        }

        // Asistan ve nöbet bilgilerinin takvim yapısında gösterileceği sayfa tasarımı
        public ActionResult SchedulePage()
        {
            return View();
        }

        // Asistanların tanıtıldığı sayfa tasarımı
        public ActionResult AssistantPage()
        {
            return View();
        }

        // Profesörlerin tanıtıldığı sayfa tasarımı
        public ActionResult ProfessorPage()
        {
            return View();
        }

        // Bölümlerin tanıtıldığı sayfa tasarımı
        public ActionResult DepartmentPage()
        {
            return View();
        }

        // Ramndevu Alım sayfası tasarımı
        public ActionResult AppointmentPage()
        {
            return View();
        }

        // Acil durumların girileceği sayfa tasrımı
        public ActionResult EmergencyPage()
        {
            return View();
        }

        public ActionResult ShiftsPage()
        {
            DatabaseContext dbContext = new DatabaseContext();

            List<Shift> model = dbContext.Shifts.ToList();

            return View(model);
        }
    }
}