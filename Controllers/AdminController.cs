using System.Web.Mvc;

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
            string adminUsername = "a";
            string adminPassword = "a";

            // Girilen isim ve şifre tanımlanan ile aynıysa
            if (username == adminUsername && password == adminPassword)
            {
                // Oturum bilgilerini sakla
                Session["IsAdmin"] = true;
                Session["AdminLogged"] = "Admin Panel is Activated.";
                return RedirectToAction("HomePage", "Home");
            }

            // Hatalı giriş mesajı
            ViewBag.ErrorMessage = "Invalid username or password.";
            return RedirectToAction("Login");
        }



        // giriş için admin kontrolü
        public ActionResult Dashboard()
        {
            // Admin oturumu kontrol et
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                return RedirectToAction("Login"); // Oturum yoksa login sayfasına yönlendir
            }

            return RedirectToAction("HomePage", "Home"); // anasayfaya döndür
        }



        // Admin çıkışı
        public ActionResult Logout()
        {
            Session["IsAdmin"] = false;
            Session["AdminLogged"] = "User Panel";

            // Session.Clear(); // Oturum bilgilerini temizle
            return RedirectToAction("HomePage", "Home"); // Login sayfasına dön
        }
    }
}
