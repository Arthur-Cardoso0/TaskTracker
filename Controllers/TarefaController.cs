using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Data;
using TaskTracker.Models;

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

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Tarefa tarefa)
        {
            ModelState.Remove(nameof(Tarefa.UsuarioId));
             if (!ModelState.IsValid)
            {
                return View(tarefa);
            } 
            tarefa.Prazo = DateTime.SpecifyKind(tarefa.Prazo, DateTimeKind.Utc);

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            tarefa.UsuarioId = usuarioId;
            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

                       
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if(tarefa == null)
            {
                return NotFound();
            }
            
            return View(tarefa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Tarefa tarefaAtualizada)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if(tarefa == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(tarefa);
            }

            tarefa.Titulo = tarefaAtualizada.Titulo;
            tarefa.Descricao = tarefaAtualizada.Descricao;
            tarefa.Status = tarefaAtualizada.Status;
            tarefa.Prazo = DateTime.SpecifyKind(tarefaAtualizada.Prazo, DateTimeKind.Utc);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if(tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        [HttpGet]
        
        public async Task<IActionResult> Excluir(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if(tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConfirmado(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if(tarefa == null)
            {
                return NotFound();
            }
            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}