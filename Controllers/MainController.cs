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
        DatabaseContext db = new DatabaseContext();


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
            List<Assistant> assistants = db.Assistants.ToList();
            return View(assistants);
        }

        // Profesörlerin tanıtıldığı sayfa tasarımı
        public ActionResult ProfessorPage()
        {
            return View();
        }

        // Bölümlerin tanıtıldığı sayfa tasarımı
        public ActionResult DepartmentPage()
        {
            // farklı bir yöntem
            //var departments = db.Departments.Select(d => new
            //{
            //    d.DepartmentName,
            //    d.Dep_NumberOfPatients,
            //    d.Dep_NumberOfBedridden,
            //    d.Dep_NumberOfEmptyBed
            //}).ToList();


            List<Department> departments = db.Departments.ToList();


            return View(departments);
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
            return View();
        }
    }
}