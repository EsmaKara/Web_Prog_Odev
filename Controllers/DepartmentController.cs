using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models.Managers;
using Web_Prog_Odev.Models;
using System.Data.Entity;

namespace Web_Prog_Odev.Controllers
{
    public class DepartmentController : Controller
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







        // Bölümlerin tanıtıldığı sayfa tasarımı
        public ActionResult DepartmentPage()
        {
            List<Department> departments = db.Departments.ToList();
            return View(departments);

            // farklı bir yöntem
            //var departments = db.Departments.Select(d => new
            //{
            //    d.DepartmentName,
            //    d.Dep_NumberOfPatients,
            //    d.Dep_NumberOfBedridden,
            //    d.Dep_NumberOfEmptyBed
            //}).ToList();
        }



        public ActionResult EditData(int? depId)
        {
            // alternatif yöntem
            // Department department = db.Departments.Find(id);

            if (depId != null)
            {
                Department department = db.Departments.Where(dep => dep.DepartmentID == depId).FirstOrDefault();
                return View(department);
            }
            return View();
        }

        // Vİew'dan Controller'a gelen veriler kaydolacak
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Department ViewDepartment, int? depId)
        {
            int capasity = 100;
            int bedCapacity = 50;

            if (ModelState.IsValid)
            {
                if (depId != null)
                {
                    // alternatif;
                    // db.Entry(department).State = EntityState.Modified; // Güncelleme işlemi

                    Department department = db.Departments.Where(dep => dep.DepartmentID == depId).FirstOrDefault();
                    department.DepartmentName = ViewDepartment.DepartmentName;
                    department.Dep_Description = ViewDepartment.Dep_Description;
                    // kapasiteden fazla bir değer ya da negatif bir değer girilemez
                    if(department.Dep_NumberOfPatients >= 0 && department.Dep_NumberOfPatients <= capasity)
                        department.Dep_NumberOfPatients = ViewDepartment.Dep_NumberOfPatients;
                    if(ViewDepartment.Dep_NumberOfBed >= 0 && ViewDepartment.Dep_NumberOfBed <= bedCapacity)
                    {
                        // yatışı yapılan hasta sayısı yatak sayısından fazla olamaz
                        if (ViewDepartment.Dep_NumberOfBedridden >= 0 && ViewDepartment.Dep_NumberOfBed >= ViewDepartment.Dep_NumberOfBedridden)
                        {
                            department.Dep_NumberOfBed = ViewDepartment.Dep_NumberOfBed;
                            department.Dep_NumberOfBedridden = ViewDepartment.Dep_NumberOfBedridden;
                            department.Dep_NumberOfEmptyBed = ViewDepartment.Dep_NumberOfBed - ViewDepartment.Dep_NumberOfBedridden;
                        }                     
                    }
                    else
                    {
                        TempData["Result"] = "Make sure that the values you entered are Valid.";
                        return View();
                    }


                    int result = db.SaveChanges(); // Değişiklikleri veritabanına kaydet
                    ControlViewBags(result, "updated");
                    return RedirectToAction("DepartmentPage");
                    
                }
            }
            return View();
        }


        public ActionResult Delete(int? depId)
        {
            if (depId != null)
            {
                Department department = db.Departments.Where(dep => dep.DepartmentID == depId).FirstOrDefault();
                db.Departments.Remove(department);
                ResetIdentity("Department");

                int result = db.SaveChanges();

                ControlViewBags(result, "deleted");
                return RedirectToAction("DepartmentPage");
            }
            return View();
        }

    }
}