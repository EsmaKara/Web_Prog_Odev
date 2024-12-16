using System.Web.Mvc;
using Web_Prog_Odev.Filters;

namespace Web_Prog_Odev.Controllers
{
    public class AdminController : Controller
    {
        // Admin Login Sayfası
        public ActionResult Login()
        {
            return View();
        }

        // Login işlemi için Post
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Admin bilgilerini tanımlama
            string adminUsername = "esma";
            string adminPassword = "1234";

            // Girilen isim ve şifre tanımlanan ile aynıysa
            if (username == adminUsername && password == adminPassword)
            {
                // Oturum bilgilerini sakla
                Session["IsAdmin"] = true;
                return RedirectToAction("Dashboard", "Admin"); // Admin Dashboard'a yönlendirme
            }

            // Hatalı giriş mesajı
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        // giriş için admin kontrolü
        [AdminAuthorize]
        public ActionResult Dashboard()
        {
            // Admin oturumu kontrol et
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Login"); // Oturum yoksa login sayfasına yönlendir
            }

            return View(); // Admin panelini göster
        }

        // Admin çıkışı
        public ActionResult Logout()
        {
            Session.Clear(); // Oturum bilgilerini temizle
            return RedirectToAction("Login"); // Login sayfasına dön
        }
    }
}
