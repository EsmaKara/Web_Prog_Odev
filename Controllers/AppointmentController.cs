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

        int appoSayac = 2;

        // Gerekli Fonksiyon Tanımlamaları;
        private void ControlViewBags(int result, string state)
        {
            if (result != 0)
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











        // Randevu Alım sayfası tasarımı
        public ActionResult AppointmentPage(int? profId) 
        {
            if (profId == null)
            {
                List<Available_Prof> avaiList = db.AvailableProfs.ToList();
                List<Available_Prof> falseAvaiList = avaiList.Where(x => x.IsAvailable == false).ToList();
                ViewBag.falseAvaiList = falseAvaiList;
                return View(avaiList);
            }
            else
            {
                Professor professor = db.Professors.Where(prof => prof.ProfessorID == profId).ToList().FirstOrDefault();
                List<Available_Prof> avaiProfList = professor.AvailableProfList;

                List<Available_Prof> falseAvaiList = avaiProfList.Where(x => x.IsAvailable == false).ToList();
                ViewBag.falseAvaiList = falseAvaiList;
                return View(avaiProfList);
            }
        }




        public ActionResult AddData(int? avaiId)
        {
            Available_Prof avaiProf = db.AvailableProfs.Where(x => x.AvailableProfID == avaiId).FirstOrDefault();
            ViewBag.AvaiProf = avaiProf;

            List<Assistant> assistants = db.Assistants.ToList();
            ViewBag.AssistData = assistants;

            return View();
        }


        // View'dan Contrellar'a veri göndermek için post metodu tanımlanır
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Randevuları tutacak tabloya view'dan girilen verilerin eklenmesini sağlayacak metod
        public ActionResult Add(Appointment ViewAp, int? avaiId)
        {
            Available_Prof avaiProf = db.AvailableProfs.Where(x => x.AvailableProfID == avaiId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                Appointment newAppointment = new Appointment();

                Assistant assistant = db.Assistants.Where(x => x.AssistantID == ViewAp.AssistantID).FirstOrDefault();
                // eğer seçilen asistanın bir nöbeti varsa getir
                List<Shift> shifts = assistant.ShiftList.ToList();
                if (shifts.Count > 0)
                {
                    foreach (Shift s in shifts)
                    {
                        // nöbetin başlangıç saati randevu bitiş saatinden sonra olmalı ya da randevunun başlangıç saati nöbetin bitiş saatinden sonra olmalı
                        if (s.ShiftStart > avaiProf.AvailableProfDateEnd && s.ShiftEnd < avaiProf.AvailableProfDateStart && ViewAp.AssistantID != 0)
                        {
                            newAppointment.AppointmentID = appoSayac;
                            newAppointment.AvailableProfR = avaiProf;
                            newAppointment.AssistantR = assistant;
                            newAppointment.AvailableProfR.IsAvailable = false;

                            db.Appointments.Add(newAppointment);
                            appoSayac += 1;
                            int result = db.SaveChanges();

                            ControlViewBags(result, "scheduled");
                            return RedirectToAction("AppointmentPage");
                        }
                        else
                        {
                            ViewBag.Result = "Make sure that the assistant you selected has no shift on the exact time.";
                            ViewBag.AvaiProf = avaiProf;
                            return RedirectToAction("AddData", new { avaiId = avaiProf.AvailableProfID });
                        }
                    }
                }
                else
                {
                    newAppointment.AppointmentID = appoSayac;
                    newAppointment.AvailableProfR = db.AvailableProfs.Where(x => x.AvailableProfID == avaiId).FirstOrDefault();
                    newAppointment.AssistantR = assistant;
                    newAppointment.AvailableProfR.IsAvailable = false;

                    db.Appointments.Add(newAppointment);
                    appoSayac += 1;
                    int result = db.SaveChanges();

                    ControlViewBags(result, "scheduled");
                    return RedirectToAction("AppointmentPage");
                }
            }
            ViewBag.Result = "Make sure that t time.";

            ViewBag.AvaiProf = avaiProf;
            return RedirectToAction("AddData", new { avaiId = avaiProf.AvailableProfID });
        }



        public ActionResult Delete(int? appoId)
        {
            Appointment Ap = db.Appointments.Where(x => x.AppointmentID == appoId).FirstOrDefault();
            // Available_Prof yeniden true yapılarak randevu alınabilecek duruma getirilir
            Ap.AvailableProfR.IsAvailable = true;
            db.Appointments.Remove(Ap);

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return RedirectToAction("AppointmentPage");
        }

    }
}