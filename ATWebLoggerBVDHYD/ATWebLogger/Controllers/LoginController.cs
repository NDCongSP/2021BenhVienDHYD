using System.Web.Mvc;

namespace ATWebLogger.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                if (userAccount.Username?.ToLower() != "admin")
                {
                    ModelState.AddModelError("", "Wrong username!");
                }
                else
                {
                    if (userAccount.Password?.ToLower() != DataController.WebLogger.Password)
                    {
                        ModelState.AddModelError("", "Wrong password!");

                    }
                    else
                    {
                        SessionHelper.SetSession(new UserSession() { User = userAccount.Username });
                        return RedirectToAction("Index", "Home");
                    }
                }
                //FormsAuth.SetAuthCookie(userAccount.Username, userAccount.RememberMe); 
            }
            return View();
        }
    }
}