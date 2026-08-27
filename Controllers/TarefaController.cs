using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Data;

namespace TaskTracker.Controllers
{
    [Authorize]
    public class TarefaController : Controller
    {
        private readonly ILogger<TarefaController> _logger;
        private readonly AppDbContext _context;

        public TarefaController(ILogger<TarefaController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefas = await _context.Tarefas.Where(t => t.UsuarioId == usuarioId).ToListAsync();
            return View(tarefas);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}