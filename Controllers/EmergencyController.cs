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
            }
            return RedirectToAction("EditData");
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