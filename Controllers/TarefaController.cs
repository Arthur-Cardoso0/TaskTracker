using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Data;
using TaskTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace TaskTracker.Controllers
{
    [Authorize]
    public class TarefaController : Controller
    {
        private async Task ValidarCategoriasAsync(int? categoriaId, int usuarioId)
        {
            if (!categoriaId.HasValue)
            {
                return;
            }
            var categoriaValida = await _context.Categorias.AnyAsync(c => c.Id == categoriaId.Value && c.UsuarioId == usuarioId);
            if (!categoriaValida)
            {
                ModelState.AddModelError(nameof(Tarefa.CategoriaId), "selecione uma categoria valida");
            }
        }
        private readonly ILogger<TarefaController> _logger;
        private readonly AppDbContext _context;

        public TarefaController(ILogger<TarefaController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string buscar, StatusTarefa? statusfiltro, Prioridade? prioridadefiltro, string ordenarPor)
        {
            var usuarioIdstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdstring))
                return RedirectToAction("login", "Conta");
            int usuarioId = int.Parse(usuarioIdstring);

            var query = _context.Tarefas.Include(t => t.Categoria).Where(t => t.UsuarioId == usuarioId).AsQueryable();
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(t => t.Titulo.Contains(buscar) || t.Descricao.Contains(buscar));

            }
            if (statusfiltro.HasValue)
            {
                query = query.Where(t => t.Status == statusfiltro.Value);
            }
            if (prioridadefiltro.HasValue)
            {
                query = query.Where(t => t.Prioridade == prioridadefiltro.Value);
            }

            query = ordenarPor switch
            {
                "nome_asc" => query.OrderBy(t => t.Titulo),
                "nome_desc" => query.OrderByDescending(t => t.Titulo),
                "status_asc" => query.OrderBy(t => t.Status),
                "status_desc" => query.OrderByDescending(t => t.Status),
                "prazo_asc" => query.OrderBy(t => t.Prazo),
                "prazo_desc" => query.OrderByDescending(t => t.Prazo),
                "prioridade_desc" => query.OrderByDescending(t => t.Prioridade),
                "prioridade_asc" => query.OrderBy(t => t.Prioridade),
                "data_desc" => query.OrderByDescending(t => t.Id),
                "data_asc" => query.OrderBy(t => t.Id),
                _ => query.OrderBy(t => t.Titulo)
            };

            ViewBag.FiltroBusca = buscar;
            ViewBag.FiltroStatus = statusfiltro;
            ViewBag.FiltroPrioridade = prioridadefiltro;
            ViewBag.OrdenarPor = ordenarPor;
            ViewBag.NomeSortParam = string.IsNullOrEmpty(ordenarPor) || ordenarPor == "nome_asc" ? "nome_desc" : "nome_asc";
            ViewBag.PrazoSortParam = ordenarPor == "prazo_asc" ? "prazo_desc" : "prazo_asc";
            ViewBag.PrioridadeSortParam = ordenarPor == "prioridade_desc" ? "prioridade_asc" : "prioridade_desc";
            ViewBag.StatusSortParam = ordenarPor == "status_asc" ? "status_desc" : "status_asc";

            var tarefas = await query.ToListAsync();
            return View(tarefas);
        }

        [HttpGet]
        public async Task<IActionResult> Criar(int? boardListId)
        {
            var usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var categorias = await _context.Categorias
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            ViewBag.Categorias = new SelectList(
                categorias,
                "Id",
                "Nome"
            );

            var tarefa = new Tarefa();

            if (boardListId.HasValue)
            {
                var lista = await _context.BoardLists
                    .Include(l => l.Board)
                    .FirstOrDefaultAsync(l =>
                        l.Id == boardListId.Value &&
                        l.Board.OwnerId == usuarioId
                    );

                if (lista == null)
                {
                    return NotFound();
                }

                tarefa.BoardListId = lista.Id;

                ViewBag.NomeLista = lista.Name;
                ViewBag.BoardId = lista.BoardId;
            }

            return View(tarefa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Tarefa tarefa)
        {
            var usuarioId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            tarefa.UsuarioId = usuarioId;
            await ValidarCategoriasAsync(tarefa.CategoriaId, usuarioId);

            var lista = await _context.BoardLists
                .Include(l => l.Board)
                .FirstOrDefaultAsync(l =>
                    l.Id == tarefa.BoardListId &&
                    l.Board.OwnerId == usuarioId
                );

            if (lista == null)
            {
                ModelState.AddModelError(
                    nameof(Tarefa.BoardListId),
                    "A lista selecionada não é válida."
                );
            }

            ModelState.Remove(nameof(Tarefa.Usuario));
            ModelState.Remove(nameof(Tarefa.Categoria));
            ModelState.Remove(nameof(Tarefa.BoardList));

            if (!ModelState.IsValid)
            {
                var categorias = await _context.Categorias
                    .Where(c => c.UsuarioId == usuarioId)
                    .ToListAsync();

                ViewBag.Categorias = new SelectList(
                    categorias,
                    "Id",
                    "Nome",
                    tarefa.CategoriaId
                );

                if (lista != null)
                {
                    ViewBag.NomeLista = lista.Name;
                    ViewBag.BoardId = lista.BoardId;
                }

                return View(tarefa);
            }

            tarefa.Prazo = DateTime.SpecifyKind(
                tarefa.Prazo,
                DateTimeKind.Utc
            );

            tarefa.CriadaEm = DateTime.UtcNow;

            _context.Tarefas.Add(tarefa);

            await _context.SaveChangesAsync();

            return RedirectToAction("Detalhes", "Board", new { id = lista!.BoardId });
        }
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.Include(t => t.BoardList).FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
            {
                return NotFound();
            }

            var categorias = await _context.Categorias.Where(c => c.UsuarioId == usuarioId).ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nome", tarefa.CategoriaId);

            ViewBag.BoardId = tarefa.BoardList.BoardId;

            return View(tarefa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Tarefa tarefaAtualizada)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.Include(t => t.BoardList).FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
            {
                return NotFound();
            }
            ViewBag.BoardId = tarefa.BoardList.BoardId;

            ModelState.Remove(nameof(Tarefa.Usuario));
            ModelState.Remove(nameof(Tarefa.Categoria));

            await ValidarCategoriasAsync(tarefaAtualizada.CategoriaId, usuarioId);

            if (!ModelState.IsValid)
            {
                var categorias = await _context.Categorias.Where(c => c.UsuarioId == usuarioId).ToListAsync();
                ViewBag.Categorias = new SelectList(categorias, "Id", "Nome", tarefaAtualizada.CategoriaId);
                return View(tarefaAtualizada);
            }

            tarefa.Titulo = tarefaAtualizada.Titulo;
            tarefa.Descricao = tarefaAtualizada.Descricao;
            tarefa.Status = tarefaAtualizada.Status;
            tarefa.Prioridade = tarefaAtualizada.Prioridade;
            tarefa.CategoriaId = tarefaAtualizada.CategoriaId;
            tarefa.Prazo = DateTime.SpecifyKind(tarefaAtualizada.Prazo, DateTimeKind.Utc);

            await _context.SaveChangesAsync();
            return RedirectToAction("Detalhes", "Board", new { id = tarefa.BoardList.BoardId });

        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.Include(t => t.Categoria).Include(t => t.BoardList).FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
            {
                return NotFound();
            }
            ViewBag.BoardId = tarefa.BoardList.BoardId;
            return View(tarefa);
        }

        [HttpGet]

        public async Task<IActionResult> Excluir(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.Include(t => t.BoardList).FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
            {
                return NotFound();
            }

            ViewBag.BoardId = tarefa.BoardList.BoardId;
            return View(tarefa);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConfirmado(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tarefa = await _context.Tarefas.Include(t => t.BoardList).FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
            {
                return NotFound();
            }
            var BoardId = tarefa.BoardList.BoardId;

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detalhes", "Board", new { id = BoardId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            Response.StatusCode = 500;
            return View("Error");
        }
    }
}
