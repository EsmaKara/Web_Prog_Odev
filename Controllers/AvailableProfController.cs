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
        DatabaseContext db = new DatabaseContext();

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
        // Profesörlerin müsait olduğu zaman dilimlerini tutacak tabloya view'dan girilen verilerin eklenmesini sağlayacak metod
        public ActionResult Add(Available_Prof ViewAp, int personId, int? appoId)
        {
            Available_Prof Ap = new Available_Prof();

            // bugünün tarihinden daha eski veya 15 gün sonrasından daha ileri bir tarih atanamaz, eğer şartlar sağlanırsa atama gerçekleşir
            if (ViewAp.AvailableProfDate >= DateTime.Now && ViewAp.AvailableProfDate <= DateTime.Now.AddHours(360))
            {
                Ap.AvailableProfDate = ViewAp.AvailableProfDate;
            }

            // ıd ile eşleşen profesörler appointmente atanır
            Ap.ProfessorR = db.Professors.Where(x => x.PersonID == personId).FirstOrDefault();
            
            // randevu alınabilirliği false ise bir randevuya bağlı olması gerektiği anlamına gelir, bu yüzden eşleşen randevu ataması gerçekleştirilir
            if(Ap.IsAvailable == false)
            {
                Ap.AppointmentR = db.Appointments.Where(x => x.AppointmentID == appoId).FirstOrDefault();
            }

            db.AvailableProfs.Add(Ap);
            int result = db.SaveChanges();

            ControlViewBags(result, "saved");

            return View();
        }


        // Controller'dan View'a veriler gönderilir ki sayfada gösterilsin
        [HttpGet]
        public ActionResult GetData(int apID)
        {
            Available_Prof Ap = null;
            Ap = db.AvailableProfs.Where(x => x.AvailableProfID == apID).FirstOrDefault();

            // sayfada müsait olunan zaman dilimi ile eşleşen profesörlerin bilgisinin de görülmesi isteniyor, ViewBag ile bunlar gönderilir
            ViewBag.Avai_PersonID = Ap.ProfessorR.PersonID;
            ViewBag.Avai_PersonName = Ap.ProfessorR.PersonName;
            ViewBag.Avai_PersonSur = Ap.ProfessorR.PersonSurname;
            ViewBag.Avai_PersonMail = Ap.ProfessorR.PersonMail;

            // sayfada tarih de görülmeli mantıken
            ViewBag.AvailableDate = Ap.AvailableProfDate;
            // bu alınabilir mi durumu butonlarda kullanılacak 
            ViewBag.AvailableState = Ap.IsAvailable;

            return View();
        }


        [HttpPost]
        public ActionResult Edit(Available_Prof ViewAp, int apID)
        {
            Available_Prof Ap = db.AvailableProfs.Where(x => x.AvailableProfID == apID).FirstOrDefault();

            // böyle bir veri satırı varsa değiştirilen tarih verisi atanır, kontrol sağlanarak
            if(Ap != null)
            {
                if (ViewAp.AvailableProfDate >= DateTime.Now && ViewAp.AvailableProfDate <= DateTime.Now.AddHours(360))
                {
                    Ap.AvailableProfDate = ViewAp.AvailableProfDate;
                }
                int result = db.SaveChanges();

                ControlViewBags(result, "edited");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Delete(int apID)
        {
            Available_Prof Ap = db.AvailableProfs.Where(x => x.AvailableProfID == apID).FirstOrDefault();

            // bağlı olduğu appointment boş değilse ve bu müsaitlik durumu silinirse müsaitliğe bağlı randevu da silinmeli
            if (Ap.AppointmentR != null)
            {
                db.Appointments.Remove(Ap.AppointmentR);
                ResetIdentity("Appointment");
            }
            db.AvailableProfs.Remove(Ap);
            ResetIdentity("Available_Prof");

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return View();
        }
    }
}