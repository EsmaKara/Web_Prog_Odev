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
        private void ControlViewBags(int result, string state)
        {
            // İşlemin gerçekleştiği durumda döndürülen değer sıfırdan farklı olur
            if (result != 0)
            {
                TempData["Result"] = "The Professor has been " + state + ".";
                TempData["Status"] = "Success :)";
            }
            else
            {
                TempData["Result"] = "The Professor could not be " + state + ".";
                TempData["Status"] = "Fail !";
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
                TempData["Result"] = "An incorrect date/time entry has been detected.";
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
                TempData["Result"] = "No professor is available.";
                return RedirectToAction("EditData", new { avaiId });
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
                else
                {
                    TempData["Result"] = "Make sure that the values you entered are Valid.";
                    return RedirectToAction("EditData", new { avaiId });
                }
            }
            return RedirectToAction("EditData", new { avaiId });

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