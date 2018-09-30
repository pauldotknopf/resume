using Microsoft.AspNetCore.Mvc;

namespace Resume.Controllers
{
    public class ResumeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Template()
        {
            return View();
        }
    }
}