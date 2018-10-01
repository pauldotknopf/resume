using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Resume.Controllers
{
    public class ResumeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsPDF = false;
            return View();
        }

        public ActionResult Template()
        {
            ViewBag.IsPDF = true;
            return View();
        }

        public IActionResult Pdf()
        {
            return new PrinceActionResult();
        }
        
        class PrinceActionResult : IActionResult
        {
            public Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.ContentType = "application/pdf";
                var prince = new Prince.Prince("/usr/local/bin/prince");
                var url = new UrlHelper(context);
                prince.Convert(
                    url.ServerBaseUrl() + "/template",
                    context.HttpContext.Response.Body);
                return Task.CompletedTask;
            }
        }
    }
}