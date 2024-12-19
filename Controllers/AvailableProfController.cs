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
            if ((DateTime)ViewAvaiProf.AvailableProfDateStart >= DateTime.Now && (DateTime)ViewAvaiProf.AvailableProfDateStart <= DateTime.Now.AddHours(360))
            {
                newAP.AvailableProfDateStart = (DateTime) ViewAvaiProf.AvailableProfDateStart;
                newAP.AvailableProfDateEnd = (DateTime) ViewAvaiProf.AvailableProfDateStart.AddMinutes(90);
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
        public ActionResult EditData(int? avaiId)
        {
            Available_Prof AP = db.AvailableProfs.Where(x => x.AvailableProfID == avaiId).FirstOrDefault();
            List<Professor> professors = db.Professors.ToList();

            if (AP != null) {
                // sayfada müsait olunan zaman dilimi ile eşleşen profesörlerin bilgisinin de görülmesi isteniyor, ViewBag ile profesör gönderilir
                ViewBag.ProfName = AP.ProfessorR.ProfName;
                ViewBag.ProfSurname = AP.ProfessorR.ProfSurname;

                ViewBag.Professors = professors;
                return View(AP);
            }
            else
            {
                ViewBag.Result = "No professor is available.";
                return RedirectToAction("EditData");
            }
        }


        [HttpPost]
        public ActionResult Edit(Available_Prof ViewAp, int? avaiId)
        {
            Available_Prof tempAP = db.AvailableProfs.Where(x => x.AvailableProfID == avaiId).FirstOrDefault();

            // böyle bir veri satırı varsa değiştirilen tarih verisi atanır, kontrol sağlanarak
            if(tempAP != null)
            {
                if (ViewAp.AvailableProfDateStart >= DateTime.Now && ViewAp.AvailableProfDateStart <= DateTime.Now.AddHours(360))
                {
                    tempAP.AvailableProfDateStart = (DateTime) ViewAp.AvailableProfDateStart;
                    tempAP.AvailableProfDateEnd = (DateTime) ViewAp.AvailableProfDateStart.AddMinutes(90);

                    tempAP.ProfessorID = ViewAp.ProfessorID;

                    int result = db.SaveChanges();

                    ControlViewBags(result, "edited");
                    return RedirectToAction("AppointmentPage", "Appointment");
                }             
            }
            return RedirectToAction("EditData", new { avaiId = ViewAp.AvailableProfID });

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