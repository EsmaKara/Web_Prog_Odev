using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class EmergencyController : Controller
    {
        DatabaseContext db = new DatabaseContext();

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






        // Acil durumların gösterileceği sayfa tasarımı
        public ActionResult EmergencyPage()
        {
            List<Emergency> emgList = db.Emergencies.ToList();

            return View(emgList);
        }


        public ActionResult AddData()
        {
            List<Department> departments = db.Departments.ToList();
            ViewBag.DepData = departments;

            return View();
        }

        [HttpPost]
        public ActionResult Add(Emergency ViewEmg)
        {
            Emergency emergency = new Emergency();

            if (ModelState.IsValid)
            {
                if (ViewEmg.DepartmentID != 0)
                {
                    emergency.EmergencyName = ViewEmg.EmergencyName.ToString();
                    emergency.EmergencyDescription = ViewEmg.EmergencyDescription.ToString();
                    emergency.EmergencyDate = DateTime.Now;

                    emergency.DepartmentID = ViewEmg.DepartmentID;

                    db.Emergencies.Add(emergency);

                    int result = db.SaveChanges();

                    ControlViewBags(result, "added");
                    return RedirectToAction("EmergencyPage");
                }
                else
                {
                    TempData["Result"] = "Make sure that the values you entered are Valid.";
                    return RedirectToAction("AddData");
                }
            }
            return RedirectToAction("AddData");
        }




        public ActionResult EditData(int? emgId)
        {
            if (emgId != null)
            {
                Emergency emergency = db.Emergencies.Where(emg => emg.EmergencyID == emgId).FirstOrDefault();
                List<Department> departments = db.Departments.ToList();
                ViewBag.DepData = departments;
                ViewBag.DepName = emergency.DepartmentR.DepartmentName;

                return View(emergency);
            }
            return View();
        }

        // Vİew'dan Controller'a gelen veriler kaydolacak
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Emergency ViewEmergency, int? emgId)
        {
            if (ModelState.IsValid)
            {
                if (emgId != null)
                {
                    Emergency emergency = db.Emergencies.Where(emg => emg.EmergencyID == emgId).FirstOrDefault();
                    emergency.EmergencyName = ViewEmergency.EmergencyName;
                    emergency.EmergencyDescription = ViewEmergency.EmergencyDescription;
                    emergency.EmergencyDate = DateTime.Now;

                    emergency.DepartmentID = ViewEmergency.DepartmentID;

                    int result = db.SaveChanges(); // Değişiklikleri veritabanına kaydet
                    ControlViewBags(result, "updated");
                    return RedirectToAction("EmergencyPage");

                }
                else
                {
                    TempData["Result"] = "Make sure that the values you entered are Valid.";
                    return RedirectToAction("EditData", new { emgId });
                }
            }
            return RedirectToAction("EditData", new { emgId });
        }





        public ActionResult Delete(int? emgId)
        {
            if (emgId != null)
            {
                Emergency emergency = db.Emergencies.Where(emg => emg.EmergencyID == emgId).FirstOrDefault();
                db.Emergencies.Remove(emergency);

                int result = db.SaveChanges();

                ControlViewBags(result, "deleted");
                return RedirectToAction("EmergencyPage");
            }
            return RedirectToAction("EmergencyPage");
        }



    }
}