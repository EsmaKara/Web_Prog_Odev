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
                    emergency.DepartmentID = ViewEmg.DepartmentID;
                    emergency.EmergencyDate = DateTime.Now;

                    db.Emergencies.Add(emergency);

                    int result = db.SaveChanges();

                    ControlViewBags(result, "added");
                    return RedirectToAction("EmergencyPage");
                }
            }
            return RedirectToAction("AddData");
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