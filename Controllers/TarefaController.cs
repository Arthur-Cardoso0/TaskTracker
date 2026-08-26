using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.Controllers
{
    public class TarefaController : Controller
    {
        private readonly ILogger<TarefaController> _logger;

        public TarefaController(ILogger<TarefaController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}