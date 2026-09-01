using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;
using System.Security.Claims;

namespace TaskTracker.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUsuarioId()
        {
           return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = GetUsuarioId();
            var categorias = await _context.Categorias.Where(c => c.UsuarioId == usuarioId).ToListAsync();
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Categoria categoria)
        {
            categoria.UsuarioId = GetUsuarioId();
            ModelState.Remove(nameof(categoria.Usuario));
            ModelState.Remove(nameof(categoria.Tarefas));
            ModelState.Remove(nameof(categoria.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarRapido(string nome)
        {
           if(string.IsNullOrWhiteSpace(nome))
           {
                return BadRequest("O nome da categoria não pode ser vazio.");
           }

           var categoria = new Categoria
           {
                Nome = nome,
                UsuarioId = GetUsuarioId()
           };

           _context.Categorias.Add(categoria);
           await _context.SaveChangesAsync();
           return Json(new { id = categoria.Id, nome = categoria.Nome });
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = GetUsuarioId();
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
            if (categoria == null)return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Categoria dadosAtualizados)
        {
            var usuarioId = GetUsuarioId();
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
            if (categoria == null) return NotFound();

            ModelState.Remove(nameof(categoria.Usuario));
            ModelState.Remove(nameof(categoria.Tarefas));
            ModelState.Remove(nameof(categoria.UsuarioId));

            if (!ModelState.IsValid)
            {
               return View(dadosAtualizados);
            }

            categoria.Nome = dadosAtualizados.Nome;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConfirmado(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
            if (categoria != null)
            {
                var tarefasAssociadas = await _context.Tarefas.Where(t => t.CategoriaId == id).ToListAsync();
                foreach (var tarefa in tarefasAssociadas)
                {
                    tarefa.CategoriaId = null;
                }
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}