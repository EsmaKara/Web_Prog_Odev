using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models.Managers;
using Web_Prog_Odev.Models;

namespace Web_Prog_Odev.Controllers
{
    public class AppointmentController : Controller
    {
        DatabaseContext db = new DatabaseContext();

        // Gerekli Fonksiyon Tanımlamaları;
        private void ControlViewBags(int result, string state)
        {
            if (result == 0)
            {
                ViewBag.Result = "The Appointment has been " + state + ".";
                ViewBag.Success = true;
                ViewBag.Status = "Success :)";
            }
            else
            {
                ViewBag.Result = "The Appointment could not be " + state + ".";
                ViewBag.Success = false;
                ViewBag.Status = "Save failed!";
            }
        }

        // Identity tanımlı olduğunda veri silindiği durumda ID'ler resetlenmeli ki yeni eklemelerde sorun yaşanmasın
        public ActionResult ResetIdentity(string tableName)
        {
            string sql = $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)";

            using (var context = new DatabaseContext())
            {
                context.Database.ExecuteSqlCommand(sql);
            }

            return RedirectToAction("Index");
        }






        // View'dan Contrellar'a veri göndermek için post metodu tanımlanır
        [HttpPost]
        // Randevuları tutacak tabloya view'dan girilen verilerin eklenmesini sağlayacak metod
        public ActionResult Add(Appointment Ap, int personId, int avaiId)
        {
            Ap.Available_ProfR = db.AvailableProfs.Where(x => x.AvailableProfID == avaiId).FirstOrDefault();
            Ap.AssistantR = db.Assistants.Where(x => x.AssistantID == personId).FirstOrDefault();
            Ap.Available_ProfR.IsAvailable = false;
            db.Appointments.Add(Ap);
            int result = db.SaveChanges();

            ControlViewBags(result, "scheduled");

            return View();
        }


        // Controller'dan View'a veriler gönderilir ki sayfada gösterilsin
        [HttpGet]
        public ActionResult GetData(int apID)
        {
            Appointment Ap = null;
            Ap = db.Appointments.Where(x => x.AppointmentID == apID).FirstOrDefault();

            // Randevu bilgilerinden asistanınkilere ulaşılıp gösterilmesini sağlamak için
            ViewBag.Appo_PersonID = Ap.AssistantR.PersonID;
            ViewBag.Appo_PersonName = Ap.AssistantR.PersonName;
            ViewBag.Appo_PersonSur = Ap.AssistantR.PersonSurname;
            ViewBag.Appo_PersonMail = Ap.AssistantR.PersonMail;

            // Bağlı olduğu Available_Prof'tan tarihin gönderilmesi
            ViewBag.Appo_Date = Ap.Available_ProfR.AvailableProfDate;

            return View();
        }


        [HttpPost]
        public ActionResult Delete(int apID)
        {
            Appointment Ap = db.Appointments.Where(x => x.AppointmentID == apID).FirstOrDefault();
            // Available_Prof yeniden true yapılarak randevu alınabilecek duruma getirilir
            Ap.Available_ProfR.IsAvailable = true;
            db.Appointments.Remove(Ap);
            ResetIdentity("Appointment");

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return View();
        }

    }
}