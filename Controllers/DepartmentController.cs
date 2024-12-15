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



        // GET: Department
        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public ActionResult Details(int id)
        {
            var departments = db.Departments.Select(d => new
            {
                d.DepartmentName,
                d.Dep_Description,
                d.Dep_NumberOfPatients,
                d.Dep_NumberOfBedridden,
                d.Dep_NumberOfEmptyBed
            }).ToList();

            return View(departments);
        }


    }
}