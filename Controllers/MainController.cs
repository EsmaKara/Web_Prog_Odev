using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        public ActionResult ProfessorPage(int? id)
        {
            if (id == null)
            {
                List<Professor> professor = db.Professors.ToList();
                return View(professor);
            }
            else
            {
                List<Professor> professors = db.Professors.Where(p => p.DepartmentR.DepartmentID == id).ToList();
                return View(professors);
            }
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

        // Randevu Alım sayfası tasarımı
        public ActionResult AppointmentPage()
        {
            return View();
        }

        // Acil durumların girileceği sayfa tasarımı
        public ActionResult EmergencyPage()
        {
            return View();
        }


        // Randevuların takvim şeklinde gösterileceği sayfa tasarımı
        public ActionResult ShiftsPage()
        {
            return View();
        }
    }
}