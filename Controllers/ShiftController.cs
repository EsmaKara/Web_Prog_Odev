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

        // Departman için de nöbet kontrolü yapabilmek için bunun bir fonksiyon olarak tanımlanması gerekliydi
        // Bir asistanın atanacak tarihler arasında randevusu veya nöbeti var mı kontrolü sağlama
        private void AssistantControl(Shift newShift, Shift ViewSt)
        {
            // ıd ile eşleşen assistant'ı getir ve geçici değişkene ata
            Assistant assisTemp = db.Assistants.Where(x => x.AssistantID == ViewSt.AssistantR.AssistantID).FirstOrDefault();
            List<Shift> shiftLassist = assisTemp.ShiftList.ToList();
            List<Appointment> appoLcond = assisTemp.AppointmentList.ToList();

            foreach (Shift s in shiftLassist)
            {
                if (s != null)
                {
                    // yeni nöbet ya var olan bir nöbetin bitişinden sonra başlayabilir ya da var olan nöbetin başlangıcından önce bitebilir
                    if (ViewSt.ShiftStart > s.ShiftEnd || ViewSt.ShiftEnd < s.ShiftStart)
                    {
                        // bugünün tarihinden daha eski veya 15 gün sonrasından daha ileri bir tarih atanamaz
                        if (ViewSt.ShiftStart >= DateTime.Now && ViewSt.ShiftStart <= DateTime.Now.AddHours(360))
                        {
                            // başlangıç ve bitiş arasındaki fark 24 saat ise
                            TimeSpan difference = ViewSt.ShiftEnd - ViewSt.ShiftStart;
                            if (difference.TotalHours == 24)
                            {
                                // randevusu != null kontrolü çıkartılabilir sonradan bir KONTROL ET
                                foreach (Appointment appo in appoLcond)
                                {
                                    // ancak asistan randevusu olduğu durumlarda da kontroller gerekli
                                    if (appoLcond != null)
                                    {
                                        // nöbet ya asistanın randevusundan sonra başlamalı ya da randevunun başlangıcından önce bitmeli
                                        if (ViewSt.ShiftStart > appo.AvailableProfR.AvailableProfDateEnd || ViewSt.ShiftEnd < appo.AvailableProfR.AvailableProfDateStart)
                                        {
                                            newShift.AssistantR = assisTemp;
                                            newShift.ShiftStart = ViewSt.ShiftStart;
                                            newShift.ShiftEnd = ViewSt.ShiftEnd;
                                        }
                                        else
                                        {
                                            ViewBag.DateError = "The selected assistant is not available for the entered date range, has appointment and shift.";
                                        }
                                    }
                                    // randevusu yoksa da atama yapılabilir 
                                    else if (appo == null)
                                    {
                                        newShift.AssistantR = assisTemp;
                                        newShift.ShiftStart = ViewSt.ShiftStart;
                                        newShift.ShiftEnd = ViewSt.ShiftEnd;
                                    }
                                    // hiçbir şekilde atama yapılamadıysa doğru bir tarih girilmemiştir
                                    else
                                    {
                                        ViewBag.DateError = "The selected assistant is not available for the entered date range, has shift.";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private void AssignControl(Shift newShift, Shift ViewSt)
        {
            // bir departman için girilen zaman diliminde sadece bir nöbet ataması yapılmış olması gerekir
            Department tempDepart = ViewSt.DepartmentR;
            List<Shift> shiftLdep = tempDepart.ShiftList.ToList();
            foreach (Shift shifTemp in shiftLdep)
            {
                if (shiftLdep != null)
                {
                    // eklenecek nöbetin ya bitişi departmana ait nöbetlerin başlangıç zamanından önce olmalı ya da başlangıcı departmana ait nöbetlerin bitiş tarihinden sonra olmalı
                    if (shifTemp.ShiftStart > ViewSt.ShiftEnd || shifTemp.ShiftEnd < ViewSt.ShiftStart)
                    {
                        newShift.DepartmentR = ViewSt.DepartmentR;
                        AssistantControl(newShift, ViewSt);
                    }
                    else
                    {
                        ViewBag.ShiftDepError = "An shift is already scheduled for the department in the assigned time slot.";
                    }
                }
            }
        }











        // Asistan ve nöbet bilgilerinin takvim yapısında gösterileceği sayfa tasarımı  +randevular da eklenebilir
        public ActionResult SchedulePage()
        {


            return View();
        }





        // View'dan Contrellar'a veri göndermek için post metodu tanımlanır
        [HttpPost]
        // Nöbetleri tutacak tabloya view'dan girilen verilerin eklenmesini sağlayacak metod
        public ActionResult Add(Shift ViewSt)
        {
            Shift newShift = new Shift();
        
            AssignControl(newShift, ViewSt);
            
            db.Shifts.Add(newShift);
            int result = db.SaveChanges();

            ControlViewBags(result, "scheduled");

            return View();
        }


        // Controller'dan View'a veriler gönderilir ki sayfada gösterilsin
        [HttpGet]
        public ActionResult GetDataToEdit(int? stID)
        {
            Shift shifData = null;
            shifData = db.Shifts.Where(x => x.ShiftID == stID).FirstOrDefault();

            if (shifData != null)
            {
                // sayfada nöbet ile eşleşen asistanların bilgisinin de görülmesi isteniyor, ViewBag ile bunlar gönderilir
                ViewBag.Shift_AssistID = shifData.AssistantR.AssistantID;
                ViewBag.Shift_AssistName = shifData.AssistantR.AssistName;
                ViewBag.Shift_AssistSur = shifData.AssistantR.AssistSurname;
                ViewBag.Shift_AssistMail = shifData.AssistantR.AssistMail;

                // sayfada tarih ve departman adı de görünmeli mantıken
                ViewBag.Shift_DateStart = shifData.ShiftStart;
                ViewBag.Shift_DateEnd = shifData.ShiftEnd;
                ViewBag.Shift_DepName = shifData.DepartmentR.DepartmentName;
            }
            else
            {
                ViewBag.Appo_Error = "No shift found.";
            }

            return View(shifData);
        }


        [HttpPost]
        public ActionResult Edit(Shift ViewSt)
        {
            // viewdan gelen ıd ile eşleşen shift için değişikliklerin yapılması sağlanacak
            Shift newShift = db.Shifts.Where(x => x.ShiftID == ViewSt.ShiftID).FirstOrDefault();

            AssignControl(newShift, ViewSt);

            int result = db.SaveChanges();

            ControlViewBags(result, "edited");

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Delete(int? stID)
        {
            Shift shiftDel = db.Shifts.Where(x => x.ShiftID == stID).FirstOrDefault();

            db.Shifts.Remove(shiftDel);
            ResetIdentity("Shift");

            int result = db.SaveChanges();

            ControlViewBags(result, "deleted");

            return View();
        }
    }
}