using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models.Managers;
using Web_Prog_Odev.Models;
using System.Diagnostics.Eventing.Reader;
using System.EnterpriseServices;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;

namespace Web_Prog_Odev.Controllers
{
    public class ShiftController : Controller
    {
        private DatabaseContext db = new DatabaseContext();


        // Gerekli Fonksiyon Tanımlamaları;
        private void ControlViewBags(int result, string state)
        {
            if (result != 0)
            {
                ViewBag.Result = "The shift has been " + state + ".";
                ViewBag.Success = true;
                ViewBag.Status = "Success :)";
            }
            else
            {
                ViewBag.Result = "The shift could not be " + state + ".";
                ViewBag.Success = false;
                ViewBag.Status = "Fail !";
            }
        }

        // Departman için de nöbet kontrolü yapabilmek için bunun bir fonksiyon olarak tanımlanması gerekliydi
        // Bir asistanın atanacak tarihler arasında randevusu veya nöbeti var mı kontrolü sağlama
        private void AssistantControl(Shift newShift, Shift ViewSt)
        {
            // ıd ile eşleşen assistant'ı getir ve geçici değişkene ata
            Assistant assisTemp = db.Assistants.Where(x => x.AssistantID == ViewSt.AssistantID).FirstOrDefault();
            List<Shift> shiftLassist = assisTemp.ShiftList.ToList();
            List<Appointment> appoLcond = assisTemp.AppointmentList.ToList();

            if (shiftLassist.Count > 0)
            {
                foreach (Shift s in shiftLassist)
                {

                    // yeni nöbet ya var olan bir nöbetin bitişinden sonra başlayabilir ya da var olan nöbetin başlangıcından önce bitebilir
                    if ((DateTime)ViewSt.ShiftStart > s.ShiftEnd || (DateTime)ViewSt.ShiftEnd < s.ShiftStart)
                    {
                        // bugünün tarihinden daha eski veya 15 gün sonrasından daha ileri bir tarih atanamaz
                        if ((DateTime)ViewSt.ShiftStart >= DateTime.Now && (DateTime)ViewSt.ShiftStart <= DateTime.Now.AddHours(360))
                        {
                            // başlangıç ve bitiş arasındaki fark 24 saat ise
                            TimeSpan difference = (DateTime)ViewSt.ShiftEnd - (DateTime)ViewSt.ShiftStart;
                            if (difference.TotalHours == 24)
                            {
                                if (appoLcond.Count > 0)
                                {
                                    // randevusu != null kontrolü çıkartılabilir sonradan bir KONTROL ET
                                    foreach (Appointment appo in appoLcond)
                                    {
                                        // ancak asistan randevusu olduğu durumlarda da kontroller gerekli
                                        if (appoLcond != null)
                                        {
                                            // nöbet ya asistanın randevusundan sonra başlamalı ya da randevunun başlangıcından önce bitmeli
                                            if ((DateTime)ViewSt.ShiftStart > appo.AvailableProfR.AvailableProfDateEnd || (DateTime)ViewSt.ShiftEnd < appo.AvailableProfR.AvailableProfDateStart)
                                            {
                                                newShift.AssistantR = assisTemp;
                                                newShift.ShiftStart = (DateTime)ViewSt.ShiftStart;
                                                newShift.ShiftEnd = (DateTime)ViewSt.ShiftStart.AddHours(24);
                                            }
                                            else
                                            {
                                                ViewBag.Result = "The selected assistant is not available for the entered date range, has appointment and shift.";
                                            }
                                        }
                                        // randevusu yoksa da atama yapılabilir 
                                        else if (appo == null && ((DateTime)ViewSt.ShiftStart > appo.AvailableProfR.AvailableProfDateEnd || (DateTime)ViewSt.ShiftEnd < appo.AvailableProfR.AvailableProfDateStart))
                                        {
                                            newShift.AssistantID = assisTemp.AssistantID;
                                            newShift.ShiftStart = (DateTime)ViewSt.ShiftStart;
                                            newShift.ShiftEnd = (DateTime)ViewSt.ShiftStart.AddHours(24);
                                        }
                                        // hiçbir şekilde atama yapılamadıysa doğru bir tarih girilmemiştir
                                        else
                                        {
                                            ViewBag.Result = "The selected assistant is not available for the entered date range, has shift.";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
            else
            {
                if (appoLcond.Count > 0)
                {
                    // randevusu != null kontrolü çıkartılabilir sonradan bir KONTROL ET
                    foreach (Appointment appo in appoLcond)
                    {
                        // ancak asistan randevusu olduğu durumlarda da kontroller gerekli
                        if (appoLcond != null)
                        {
                            // nöbet ya asistanın randevusundan sonra başlamalı ya da randevunun başlangıcından önce bitmeli
                            if ((DateTime)ViewSt.ShiftStart > appo.AvailableProfR.AvailableProfDateEnd || (DateTime)ViewSt.ShiftEnd < appo.AvailableProfR.AvailableProfDateStart)
                            {
                                newShift.AssistantR = assisTemp;
                                newShift.ShiftStart = (DateTime)ViewSt.ShiftStart;
                                newShift.ShiftEnd = (DateTime)ViewSt.ShiftStart.AddHours(24);
                            }
                            else
                            {
                                ViewBag.Result = "The selected assistant is not available for the entered date range, has appointment and shift.";
                            }
                        }
                    }
                }
                else
                {
                    newShift.AssistantR = assisTemp;
                    newShift.ShiftStart = (DateTime)ViewSt.ShiftStart;
                    newShift.ShiftEnd = (DateTime)ViewSt.ShiftStart.AddHours(24);
                }
            }

        }


        private void AssignControl(Shift newShift, Shift ViewSt)
        {

            // bir departman için girilen zaman diliminde sadece bir nöbet ataması yapılmış olması gerekir
            Department tempDepart = db.Departments.Where(dep => dep.DepartmentID == ViewSt.DepartmentID).ToList().FirstOrDefault();
            List<Shift> shiftLdep = tempDepart.ShiftList.ToList();
            if (shiftLdep.Count > 0)
            {
                foreach (Shift shifTemp in shiftLdep)
                {
                    // eklenecek nöbetin ya bitişi departmana ait nöbetlerin başlangıç zamanından önce olmalı ya da başlangıcı departmana ait nöbetlerin bitiş tarihinden sonra olmalı
                    if (shifTemp.ShiftStart > (DateTime)ViewSt.ShiftStart.AddHours(24) || shifTemp.ShiftEnd < (DateTime)ViewSt.ShiftStart)
                    {
                        newShift.DepartmentID = ViewSt.DepartmentID;
                        AssistantControl(newShift, ViewSt);
                    }
                    else
                    {
                        ViewBag.Result = "An shift is already scheduled for the department in the assigned time slot.";
                    }
                }
            }
            else
            {
                newShift.DepartmentID = ViewSt.DepartmentID;
                AssistantControl(newShift, ViewSt);
            }
        }













        // Asistan ve nöbet bilgilerinin takvim yapısında gösterileceği sayfa tasarımı  +randevular da eklenebilir
        public JsonResult SchedulePage(DateTime? start, DateTime? end)
        {

            if (!start.HasValue)
            {
                start = DateTime.Now.AddMonths(-1); // Varsayılan başlangıç tarihi (bir ay önce)
            }

            if (!end.HasValue)
            {
                end = DateTime.Now.AddMonths(1); // Varsayılan bitiş tarihi (bir ay sonra)
            }

            var shifts = db.Shifts
                .Where(s => s.ShiftStart >= start && s.ShiftEnd <= end)
                .ToList()
                .AsEnumerable()
                .Select(s => new
                {
                    title = $"Shift For: Asst. {s.AssistantR.AssistName} {s.AssistantR.AssistSurname} on {s.DepartmentR.DepartmentName} Department",
                    start = s.ShiftStart.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = s.ShiftEnd.ToString("yyyy-MM-ddTHH:mm:ss"), // Örnek bitiş süresi
                    color = "#FFD700" // Sarı renk
                }).ToList();

            var appointments = db.Appointments
                .Where(a => a.AvailableProfR.AvailableProfDateStart >= start && a.AvailableProfR.AvailableProfDateEnd <= end)
                .ToList()
                .AsEnumerable()
                .Select(a => new
                {
                    title = $"Appointment For: Prof. {a.AvailableProfR.ProfessorR.ProfName} {a.AvailableProfR.ProfessorR.ProfSurname} " +
                    $"and Asst. {a.AssistantR.AssistName} {a.AssistantR.AssistSurname}",
                    start = a.AvailableProfR.AvailableProfDateStart.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = a.AvailableProfR.AvailableProfDateStart.AddMinutes(90).ToString("yyyy-MM-ddTHH:mm:ss"), // Örnek bitiş süresi
                    color = "#fff5cc" // Daha açık sarı renk
                }).ToList();

            var events = shifts.Concat(appointments);
            return Json(events, JsonRequestBehavior.AllowGet);
        }







        public ActionResult ShiftPage(int? assistId)
        {
            if (assistId != null)
            {
                List<Shift> shifts = db.Shifts.Where(s => s.AssistantID == assistId).ToList();
                return View(shifts);
            }
            else
            {
                List<Shift> shifts = db.Shifts.ToList();
                return View(shifts);
            }
        }






        public ActionResult AddData()
        {
            List<Assistant> assistants = db.Assistants.ToList();
            ViewBag.Assistants = assistants;
            List<Department> departments = db.Departments.ToList();
            ViewBag.DepData = departments;

            return View();
        }

        // View'dan Contrellar'a veri göndermek için post metodu tanımlanır
        [HttpPost]
        // Nöbetleri tutacak tabloya view'dan girilen verilerin eklenmesini sağlayacak metod
        public ActionResult Add(Shift ViewSt)
        {
            Shift newShift = new Shift();
        
            if(ViewSt.AssistantID != 0 &&  ViewSt.DepartmentID != 0)
            {
                AssignControl(newShift, ViewSt);

                if (newShift.ShiftStart == null)
                {
                    return RedirectToAction("AddData");
                }
                else
                {
                    db.Shifts.Add(newShift);
                    int result = db.SaveChanges();

                    ControlViewBags(result, "scheduled");

                    return RedirectToAction("ShiftPage");
                }
            }
            else
            {
                ViewBag.Result = "Make sure that you enter all the values.";
                return RedirectToAction("AddData");
            }
            
        }






        // Controller'dan View'a veriler gönderilir ki sayfada gösterilsin
        public ActionResult EditData(int? shiftId)
        {
            Shift shifData = db.Shifts.Where(x => x.ShiftID == shiftId).FirstOrDefault();
            List<Assistant> assistants = db.Assistants.ToList();

            ViewBag.Assistants = assistants;
            List<Department> departments = db.Departments.ToList();
            ViewBag.DepData = departments;

            if (shifData != null)
            {
                // sayfada nöbet ile eşleşen asistanların bilgisinin de görülmesi isteniyor, ViewBag ile bunlar gönderilir
                ViewBag.Assistants = assistants;
            }
            else
            {
                ViewBag.Appo_Error = "No shift found.";
            }

            return View(shifData);
        }


        [HttpPost]
        public ActionResult Edit(Shift ViewSt, int? shiftId)
        {
            if (ModelState.IsValid)
            {
                if (shiftId != null && ViewSt.AssistantID != 0 && ViewSt.DepartmentID != 0)
                {
                    // viewdan gelen ıd ile eşleşen shift için değişikliklerin yapılması sağlanacak
                    Shift newShift = db.Shifts.Where(x => x.ShiftID == shiftId).FirstOrDefault();

                    AssignControl(newShift, ViewSt);

                    int result = db.SaveChanges();

                    ControlViewBags(result, "edited");

                    return RedirectToAction("ShiftPage");
                }
            }
            return RedirectToAction("EditData", new { shiftId });

        }





        public ActionResult Delete(int? shiftId)
        {
            Shift shiftDel = db.Shifts.Where(x => x.ShiftID == shiftId).FirstOrDefault();

            db.Shifts.Remove(shiftDel);

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return RedirectToAction("ShiftPage");
        }
    }
}