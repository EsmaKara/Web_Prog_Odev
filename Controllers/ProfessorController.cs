using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    public class ProfessorController : Controller
    {
        DatabaseContext db = new DatabaseContext();


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



    }
}