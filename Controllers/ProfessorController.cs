
// Gerekli tanımlamalar
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Web_Prog_Odev.Models;
using Web_Prog_Odev.Models.Managers;

namespace Web_Prog_Odev.Controllers
{
    // Controller sınıfından kalıtım almalı 
    public class ProfessorController : Controller
    {
        // Modellerin veri tabanında karşılık gelen tabloları için değişkenleri barındıran bir yönetim sınıfı üretiliyor
        DatabaseContext db = new DatabaseContext();


        // Gerekli Fonksiyon Tanımlamaları;

        /// <summary>
        /// Veri tabanında gerekli değişikliğin yapılıp yapılmadığını kontrol edip hata veya başarı mesajı üretmeyi sağlar
        /// ve bu verileri TempData yapısı aracılığı ile ilgili sayfaya gönderir
        /// Başka bir sayfaya yönlendirme söz konusu olduğu için redirect senaryolarda çalışan TempData yapısı kullanılmıştır
        /// </summary>
        /// <param name="result">SaveChanges() metodunun döndürdüğü değişiklik sayısı</param>
        /// <param name="state">Yapılan işlem için verilen isim</param>
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

        /// <summary>
        /// Professor modelinde email olarak belirlenen değişken için girilecek verinin E-posta olup olmadığını doğrulama yöntemi
        /// </summary>
        /// <param name="email">View'dan gelen modelin barındırdığı email bilgisi</param>
        /// <returns>boolean değer</returns>
        bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Regex ile e-posta doğrulama deseni
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, emailPattern);
        }











        // Profesörlerin tanıtıldığı sayfa tasarımı
        // Vİew'a Controller'dan veri gönderiminde kullanılan GET metodu
        [HttpGet]
        public ActionResult ProfessorPage(int? depId)
        {
            // Department sayfasında da bu action kullanılıyor, eğer o sayfadan gelinmediyse id boştur ve
            // bütün profesörlerin listelenmesi istenir
            if (depId == null)
            {
                // Veri tabanındaki bütün profesörler gönderiliyor
                List<Professor> professor = db.Professors.ToList();
                return View(professor);
            }
            else
            {
                // Eğer ki department sayfasından gelindiyse parametre olarak gelen departmanın ID bilgisine göre
                // sadece o departmana ait olan profesörlerin View'a gönderilmesi
                List<Professor> professors = db.Professors.Where(p => p.DepartmentR.DepartmentID == depId).ToList();
                return View(professors);
            }
        }




        // GET metodu: View'a gönderilmek istenen veriler için tasarlanan metot
        // Veri ekleme aşamasında View'da kullanılacak departman bilgileri ViewBag ile gönderilir
        // daha sonra bu veriler DropDownList için kullanılacak
        public ActionResult AddData()
        {
            List<Department> departments = db.Departments.ToList();
            // Controller'dan View'a veri gönderilirken kullanılan yapılardan biri: ViewBag
            ViewBag.DepData = departments;

            return View();
        }

        /// <summary>
        /// View'dan Controller'a Form yapısı sayesinde gönderilen verilerin alınmasını sağlayan POST metodu
        /// Gönderilen verilerin veritabanına eklenmesini sağlayacak metot
        /// </summary>
        /// <param name="ViewProfessor">View'da içi doldurulan model yapısı</param>
        /// <returns>ekleme gerçekleştiyse profesör sayfasına, gerçekleşmediyse yeniden AddData sayfasına yönlendirir</returns>
        [HttpPost]
        public ActionResult Add(Professor ViewProfessor)
        {
            // Yeni profesör nesnesi oluşturma / bu veri tabanında bir satırı temsil eder
            Professor professor = new Professor();

            // View'dan gelen 
            // Email verisi daha önceden tanımlanan fonksiyon ile kontrol edilir, uygun bir şekilde mi girilmiş
            // ProfTel verisi belirtilen/istenilen uzunluğa sahip mi
            // Department verisi boş olmamalı
            if (IsValidEmail(ViewProfessor.ProfMail) && ViewProfessor.ProfTel.Length == 12 && ViewProfessor.DepartmentID != 0)
            {
                // Bütün şartlar sağlanıyorsa gerekli atamalar yeni nesneye atanır
                professor.ProfName = ViewProfessor.ProfName;
                professor.ProfSurname = ViewProfessor.ProfSurname;
                professor.ProfTel = ViewProfessor.ProfTel;
                professor.ProfMail = ViewProfessor.ProfMail;

                professor.DepartmentID = ViewProfessor.DepartmentID;

                // Oluşturulan ve değerleri atanan nesne veri tabanındaki Professor tablosuna eklenir
                db.Professors.Add(professor);

                // Değişiklikler kaydedilir ve SaveChanges() metodunun döndürdüğü değişiklik sayısı verisi bir değişkene atanır
                int result = db.SaveChanges();

                // Daha önceden tanımlanmış fonksiyon yardımıyla kaydedilme sonucuna göre ViewBag verisi oluşturulur
                ControlViewBags(result, "added");
                return RedirectToAction("ProfessorPage");
            }
            else
            {
                // şartlar sağlanmazsa durum belirten bir TempData oluşturulur ve yeniden AddData sayfasına yönlendirilir
                TempData["Result"] = "Make sure that the values you entered are Valid.";
                return RedirectToAction("AddData");
            }
        }



        // Düzenlenmek istenen modelin verilerinin gösterilmesini sağlayan metod
        public ActionResult EditData(int? profId)
        {
            // ProfessorPage'te seçilen profesörün gönderilir, boş/null olmamalı 
            if (profId != null)
            {
                // Gelen ID'ye göre tablodan bu profesörü içeren veri çekilir
                Professor professor = db.Professors.Where(prof => prof.ProfessorID == profId).FirstOrDefault();

                // DropDownList için kullanılacak department verileri ViewBag aracılığıyla gönderilir
                List<Department> departments = db.Departments.ToList();
                ViewBag.DepData = departments;
                ViewBag.DepName = professor.DepartmentR.DepartmentName;

                // Veri tabanından çekilen profesörün bilgileri View'a gönderilir
                return View(professor);
            }
            return View();
        }

        // View'dan Controller'a gelen veriler düzenlenip kaydolacak
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Professor ViewProfessor, int? profId)
        {
            // View'dan gelen verilerin modelin gereksinimlerine uygunluğunun kontrolü
            if (ModelState.IsValid)
            {
                // View'dan gelen
                // seçilen profesörün gönderilen ID'si boş değilse
                // geçerli bir email adresi girildiyse 
                // Telefon verisi istenilen uzunluğa sahip mi
                // şartlar sağlanıyorsa gerekli atamaları yap ve veritabanına kaydet
                if (profId != null && IsValidEmail(ViewProfessor.ProfMail) && ViewProfessor.ProfTel.Length == 12)
                {
                    Professor professor = db.Professors.Where(prof => prof.ProfessorID == profId).FirstOrDefault();
                    professor.ProfName = ViewProfessor.ProfName;
                    professor.ProfSurname = ViewProfessor.ProfSurname;
                    professor.ProfTel = ViewProfessor.ProfTel;
                    professor.ProfMail = ViewProfessor.ProfMail;

                    professor.DepartmentID = ViewProfessor.DepartmentID;

                    // Değişiklikleri veritabanına kaydet
                    int result = db.SaveChanges();
                    ControlViewBags(result, "updated");
                    return RedirectToAction("ProfessorPage");

                }
                else
                {
                    TempData["Result"] = "Make sure that the values you entered are Valid.";
                    return RedirectToAction("EditData", new { profId });
                }
            }
            return RedirectToAction("EditData", new { profId });
        }



        // View'dan ID'si gönderilen profesör için silme metodu
        public ActionResult Delete(int? profId)
        {
            // Gönderilen ID boş değilse yani bir profesör silinmek için seçildiyse
            if (profId != null)
            {
                // Veri tabanından o profesörü çekeerek bir değişkene ata
                Professor professor = db.Professors.Where(prof => prof.ProfessorID == profId).FirstOrDefault();

                // Veri tabanındaki tablodan bu profesörü sil/kaldır
                db.Professors.Remove(professor);

                int result = db.SaveChanges();

                ControlViewBags(result, "deleted");
                return RedirectToAction("ProfessorPage");
            }
            return RedirectToAction("ProfessorPage");
        }




    }
}