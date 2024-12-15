using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class AvailableProfController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // Gerekli Fonksiyon Tanımlamaları;
        private void ControlViewBags (int result, string state)
        {
            if (result == 0)
            {
                ViewBag.Result = "The available time slot for professors has been " + state + ".";
                ViewBag.Success = true;
                ViewBag.Status = "Success :)";
            }
            else
            {
                ViewBag.Result = "The available time slot for professors could not be " + state + ".";
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
        // Profesörlerin müsait olduğu zaman dilimlerini tutacak tabloya view'dan girilen verilerin eklenmesini sağlayacak metod
        public ActionResult Add(Available_Prof ViewAp)
        {
            Available_Prof newAP = new Available_Prof();

            // bugünün tarihinden daha eski veya 15 gün sonrasından daha ileri bir tarih atanamaz, eğer şartlar sağlanırsa atama gerçekleşir
            if (ViewAp.AvailableProfDateStart >= DateTime.Now && ViewAp.AvailableProfDateStart <= DateTime.Now.AddHours(360))
            {
                newAP.AvailableProfDateStart = ViewAp.AvailableProfDateStart;
                newAP.AvailableProfDateEnd = ViewAp.AvailableProfDateEnd;
            }
            else
            {
                ViewBag.Avai_DateError = "An incorrect date/time entry has been detected.";
            }

            // ıd ile eşleşen profesörler appointmente atanır
            newAP.ProfessorR = db.Professors.Where(x => x.ProfessorID == ViewAp.ProfessorR.ProfessorID).FirstOrDefault();
            
            // randevu alınabilirliği false ise bir randevuya bağlı olması gerektiği anlamına gelir, bu yüzden eşleşen randevu ataması gerçekleştirilir
            if(newAP.IsAvailable == false)
            {
                newAP.AppointmentR = db.Appointments.Where(x => x.AppointmentID == ViewAp.AppointmentR.AppointmentID).FirstOrDefault();
            }

            db.AvailableProfs.Add(newAP);
            int result = db.SaveChanges();

            ControlViewBags(result, "saved");

            return View();
        }


        // Controller'dan View'a veriler gönderilir ki sayfada gösterilsin
        [HttpGet]
        public ActionResult GetDataToEdit(int apID)
        {
            Available_Prof AP = null;
            AP = db.AvailableProfs.Where(x => x.AvailableProfID == apID).FirstOrDefault();
            if (AP != null) {
                // sayfada müsait olunan zaman dilimi ile eşleşen profesörlerin bilgisinin de görülmesi isteniyor, ViewBag ile bunlar gönderilir
                ViewBag.Avai_ProfID = AP.ProfessorR.ProfessorID;
                ViewBag.Avai_ProfName = AP.ProfessorR.ProfName;
                ViewBag.Avai_ProfSur = AP.ProfessorR.ProfSurname;
                ViewBag.Avai_ProfMail = AP.ProfessorR.ProfMail;

                // sayfada tarih de görülmeli mantıken
                ViewBag.AvailableDateStart = AP.AvailableProfDateStart;
                ViewBag.AvailableDateEnd = AP.AvailableProfDateEnd;
                // bu alınabilir mi durumu butonlarda kullanılacak 
                ViewBag.AvailableState = AP.IsAvailable;
            }
            else
            {
                ViewBag.Appo_Error = "No professor is available.";
            }
            return View(AP);
        }


        [HttpPost]
        public ActionResult Edit(Available_Prof ViewAp)
        {
            Available_Prof tempAP = db.AvailableProfs.Where(x => x.AvailableProfID == ViewAp.AvailableProfID).FirstOrDefault();

            // böyle bir veri satırı varsa değiştirilen tarih verisi atanır, kontrol sağlanarak
            if(tempAP != null)
            {
                if (ViewAp.AvailableProfDateStart >= DateTime.Now && ViewAp.AvailableProfDateStart <= DateTime.Now.AddHours(360))
                {
                    tempAP.AvailableProfDateStart = ViewAp.AvailableProfDateStart;
                    tempAP.AvailableProfDateEnd = ViewAp.AvailableProfDateEnd;
                }
                int result = db.SaveChanges();

                ControlViewBags(result, "edited");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Delete(int? apID)
        {
            Available_Prof AP = db.AvailableProfs.Where(x => x.AvailableProfID == apID).FirstOrDefault();

            // bağlı olduğu appointment boş değilse ve bu müsaitlik durumu silinirse müsaitliğe bağlı randevu da silinmeli
            if (AP.AppointmentR != null)
            {
                db.Appointments.Remove(AP.AppointmentR);
                ResetIdentity("Appointment");
            }
            db.AvailableProfs.Remove(AP);
            ResetIdentity("Available_Prof");

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return View();
        }
    }
}