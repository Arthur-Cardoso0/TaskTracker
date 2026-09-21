using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error()
        {
            Response.StatusCode = 500;return View();
        }
        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AcessoNegado()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}