using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class AssistantController : Controller
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


        // E-posta doğrulama yöntemi
        bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Regex ile e-posta doğrulama deseni
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, emailPattern);
        }









        // Asistanların tanıtıldığı sayfa tasarımı
        public ActionResult AssistantPage()
        {
            List<Assistant> assistants = db.Assistants.ToList();
            return View(assistants);
        }




        public ActionResult AddData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Assistant ViewAssistant)
        {
            Assistant assistant = new Assistant();

            if (IsValidEmail(ViewAssistant.AssistMail) && ViewAssistant.AssistTel.Length == 12)
            {
                assistant.AssistName = ViewAssistant.AssistName;
                assistant.AssistSurname = ViewAssistant.AssistSurname;
                assistant.AssistTel = ViewAssistant.AssistTel;
                assistant.AssistMail = ViewAssistant.AssistMail;

                db.Assistants.Add(assistant);

                int result = db.SaveChanges();

                ControlViewBags(result, "added");
                return RedirectToAction("AssistantPage");
            }
            else
            {
                TempData["Result"] = "Make sure that the values you entered are Valid.";
                return RedirectToAction("AddData");
            }
        }




        public ActionResult EditData(int? assistId)
        {
            if (assistId != null)
            {
                Assistant assistant = db.Assistants.Where(assist => assist.AssistantID == assistId).FirstOrDefault();
                return View(assistant);
            }
            return View();
        }

        // Vİew'dan Controller'a gelen veriler kaydolacak
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Assistant ViewAssistant, int? assistId)
        {
            if (ModelState.IsValid && IsValidEmail(ViewAssistant.AssistMail) && ViewAssistant.AssistTel.Length == 12)
            {
                if (assistId != null)
                {
                    Assistant assistant = db.Assistants.Where(assist => assist.AssistantID == assistId).FirstOrDefault();
                    assistant.AssistName = ViewAssistant.AssistName;
                    assistant.AssistSurname = ViewAssistant.AssistSurname;
                    assistant.AssistTel = ViewAssistant.AssistTel;
                    assistant.AssistMail = ViewAssistant.AssistMail;

                    int result = db.SaveChanges(); // Değişiklikleri veritabanına kaydet
                    ControlViewBags(result, "updated");
                    return RedirectToAction("AssistantPage");

                }
                else
                {
                    TempData["Result"] = "Make sure that the values you entered are Valid.";
                    return RedirectToAction("EditData", new { assistId });
                }
            }
            return RedirectToAction("EditData", new { assistId });
        }




        public ActionResult Delete(int? assistId)
        {
            if (assistId != null)
            {
                Assistant assistant = db.Assistants.Where(assist => assist.AssistantID == assistId).FirstOrDefault();
                db.Assistants.Remove(assistant);

                int result = db.SaveChanges();

                ControlViewBags(result, "deleted");
                return RedirectToAction("AssistantPage");
            }
            return RedirectToAction("AssistantPage");
        }

    }
}