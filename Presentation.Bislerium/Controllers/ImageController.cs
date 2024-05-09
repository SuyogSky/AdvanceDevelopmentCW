using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    public class ImageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
