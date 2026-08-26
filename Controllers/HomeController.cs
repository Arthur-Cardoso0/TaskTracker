using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}