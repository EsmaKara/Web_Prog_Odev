using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models.Managers;
using Web_Prog_Odev.Models;

namespace Web_Prog_Odev.Controllers
{
    public class DepartmentController : Controller
    {
        DatabaseContext db = new DatabaseContext();



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

        // Vİew'dan Controller'a gelen veriler kaydolacak
        [HttpPost]
        public ActionResult AddData()
        {
            return View();
        }

        // boş yatak sayısını gelen verilerle hesaplayıp atamasını yap arka planda EDİT'TE DE ADD'DE DE

    }
}