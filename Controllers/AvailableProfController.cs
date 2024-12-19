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
            if (result != 0)
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




        public ActionResult AddData()
        {
            List<Professor> professors = db.Professors.ToList();
            ViewBag.Professors = professors;

            return View();
        }


        // View'dan Contrellar'a veri göndermek için post metodu tanımlanır
        [HttpPost]
        // Profesörlerin müsait olduğu zaman dilimlerini tutacak tabloya view'dan girilen verilerin eklenmesini sağlayacak metod
        public ActionResult Add(Available_Prof ViewAvaiProf)
        {
            Available_Prof newAP = new Available_Prof();

            // bugünün tarihinden daha eski veya 15 gün sonrasından daha ileri bir tarih atanamaz, eğer şartlar sağlanırsa atama gerçekleşir
            if (ViewAvaiProf.AvailableProfDateStart >= DateTime.Now && ViewAvaiProf.AvailableProfDateStart <= DateTime.Now.AddHours(360))
            {
                newAP.AvailableProfDateStart = ViewAvaiProf.AvailableProfDateStart;
                newAP.AvailableProfDateEnd = ViewAvaiProf.AvailableProfDateStart.AddMinutes(90);
                newAP.IsAvailable = true;
                // ıd ile eşleşen profesör atanır
                newAP.ProfessorID = ViewAvaiProf.ProfessorID;

                db.AvailableProfs.Add(newAP);
                int result = db.SaveChanges();

                ControlViewBags(result, "saved");

                return RedirectToAction("AppointmentPage", "Appointment");
            }
            else
            {
                ViewBag.Avai_DateError = "An incorrect date/time entry has been detected.";
                return RedirectToAction("AddData");
            }
        }


        // Controller'dan View'a veriler gönderilir ki sayfada gösterilsin
        [HttpGet]
        public ActionResult EditData(int? apID)
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
                return RedirectToAction("AppointmentPage", "Appointment");
            }
            return RedirectToAction("EditData");
        }


        public ActionResult Delete(int? avaiId)
        {
            Available_Prof AP = db.AvailableProfs.Where(x => x.AvailableProfID == avaiId).FirstOrDefault();

            db.AvailableProfs.Remove(AP);

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return RedirectToAction("AppointmentPage", "Appointment");
        }
    }
}