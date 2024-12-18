using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class ProfessorController : Controller
    {
        DatabaseContext db = new DatabaseContext();


        // Gerekli Fonksiyon Tanımlamaları;
        private void ControlViewBags(int result, string state)
        {
            if (result == 0)
            {
                TempData["Result"] = "The shift has been " + state + ".";
                TempData["Success"] = true;
                TempData["Status"] = "Success :)";
            }
            else
            {
                TempData["Result"] = "The shift could not be " + state + ".";
                TempData["Success"] = false;
                TempData["Status"] = "Fail !";
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

        // E-posta doğrulama yöntemi
        bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Regex ile e-posta doğrulama deseni
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, emailPattern);
        }











        // Profesörlerin tanıtıldığı sayfa tasarımı
        public ActionResult ProfessorPage(int? id)
        {
            if (id == null)
            {
                List<Professor> professor = db.Professors.ToList();
                return View(professor);
            }
            else
            {
                List<Professor> professors = db.Professors.Where(p => p.DepartmentR.DepartmentID == id).ToList();
                return View(professors);
            }
        }





        public  ActionResult AddData()
        {
            List<Department> departments = db.Departments.ToList();
            ViewBag.DepData = departments;

            return View();
        }

        [HttpPost]
        public ActionResult Add(Professor ViewProfessor)
        {
            Professor professor = new Professor();

            if (IsValidEmail(ViewProfessor.ProfMail) && ViewProfessor.ProfTel.Length == 12 && ViewProfessor.DepartmentID != 0)
            {
                professor.ProfName = ViewProfessor.ProfName;
                professor.ProfSurname = ViewProfessor.ProfSurname;
                professor.ProfTel = ViewProfessor.ProfTel;
                professor.ProfMail = ViewProfessor.ProfMail;

                professor.DepartmentID = ViewProfessor.DepartmentID;

                db.Professors.Add(professor);

                int result = db.SaveChanges();

                ControlViewBags(result, "added");
                return RedirectToAction("ProfessorPage");
            }
            else
            {
                TempData["Result"] = "Make sure that the values you entered are Valid.";
                return RedirectToAction("AddData");
            }
        }


        public ActionResult EditData(int? profId)
        {
            if (profId != null)
            {
                Professor professor = db.Professors.Where(prof => prof.ProfessorID == profId).FirstOrDefault();
                return View(professor);
            }
            return View();
        }

        // Vİew'dan Controller'a gelen veriler kaydolacak
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Professor ViewProfessor, int? profId)
        {
            if (ModelState.IsValid)
            {
                if (profId != null)
                {
                    Professor professor = db.Professors.Where(prof => prof.ProfessorID == profId).FirstOrDefault();
                    professor.ProfName = ViewProfessor.ProfName;
                    professor.ProfSurname = ViewProfessor.ProfSurname;
                    professor.ProfTel = ViewProfessor.ProfTel;
                    professor.ProfMail = ViewProfessor.ProfMail;

                    int result = db.SaveChanges(); // Değişiklikleri veritabanına kaydet
                    ControlViewBags(result, "updated");
                    return RedirectToAction("ProfessorPage");

                }
            }
            return RedirectToAction("EditData");
        }


        public ActionResult Delete(int? profId)
        {
            if (profId != null)
            {
                Professor professor = db.Professors.Where(prof => prof.ProfessorID == profId).FirstOrDefault();
                db.Professors.Remove(professor);
                ResetIdentity("Professor");

                int result = db.SaveChanges();

                ControlViewBags(result, "deleted");
                return RedirectToAction("ProfessorPage");
            }
            return RedirectToAction("ProfessorPage");
        }




    }
}