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
        private DatabaseContext db = new DatabaseContext();

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
                ViewBag.Status = "Fail !";
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
        public ActionResult Add(Appointment Ap)
        {
            Appointment newAppointment = new Appointment();

            newAppointment.AvailableProfR = db.AvailableProfs.Where(x => x.AvailableProfID == Ap.AvailableProfR.AvailableProfID).FirstOrDefault();
            newAppointment.AssistantR = db.Assistants.Where(x => x.AssistantID == Ap.AssistantR.AssistantID).FirstOrDefault();
            newAppointment.AvailableProfR.IsAvailable = false;
            db.Appointments.Add(newAppointment);
            int result = db.SaveChanges();

            ControlViewBags(result, "scheduled");

            return View();
        }


        // Controller'dan View'a veriler gönderilir ki sayfada gösterilsin
        [HttpGet]
        public ActionResult GetDataToEdit(int apID)
        {
            Appointment Ap = null;
            Ap = db.Appointments.Where(x => x.AppointmentID == apID).FirstOrDefault();

            if (Ap != null)
            {             
                // Randevu bilgilerinden asistanınkilere ulaşılıp gösterilmesini sağlamak için
                ViewBag.Appo_AssistID = Ap.AssistantR.AssistantID;
                ViewBag.Appo_AssistName = Ap.AssistantR.AssistName;
                ViewBag.Appo_AssistSur = Ap.AssistantR.AssistSurname;
                ViewBag.Appo_AssistMail = Ap.AssistantR.AssistMail;

                // Bağlı olduğu Available_Prof'tan tarihin gönderilmesi
                ViewBag.Appo_DateStart = Ap.AvailableProfR.AvailableProfDateStart;
                ViewBag.Appo_DateEnd = Ap.AvailableProfR.AvailableProfDateEnd;
            }
            else
            {
                ViewBag.Appo_Error = "No appointment found.";
            }
            return View(Ap);
        }


        [HttpPost]
        public ActionResult Delete(int? apID)
        {
            Appointment Ap = db.Appointments.Where(x => x.AppointmentID == apID).FirstOrDefault();
            // Available_Prof yeniden true yapılarak randevu alınabilecek duruma getirilir
            Ap.AvailableProfR.IsAvailable = true;
            db.Appointments.Remove(Ap);
            ResetIdentity("Appointment");

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return View();
        }

    }
}