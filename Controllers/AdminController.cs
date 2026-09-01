using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string senha)
        {
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Username == username);
            if (admin == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario invalido");
                return View();
            }

            var hasher = new PasswordHasher<Admin>();
            var resultado = hasher.VerifyHashedPassword(admin, admin.SenhaHash, senha);

            if(resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty,"senha invalida");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity =  new ClaimsIdentity(claims, "AdminScheme");
            await HttpContext.SignInAsync("AdminScheme", new ClaimsPrincipal(claimsIdentity));
            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.Include(u => u.Tarefas).ToListAsync();
            return View(usuarios);  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminScheme");
            return RedirectToAction("login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> TarefasUsuario(int id, string buscar, StatusTarefa? statusfiltro, Prioridade? prioridadefiltro)
        {
            var usuario = await _context.Usuarios.Include(u => u.Tarefas).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)return NotFound();

            var query = _context.Tarefas.Where(t => t.UsuarioId == id);
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(t => EF.Functions.ILike(t.Titulo, $"%{buscar}%") || EF.Functions.ILike(t.Descricao, $"%{buscar}%"));
            }
            if (statusfiltro.HasValue)
            {
                query = query.Where(t => t.Status == statusfiltro.Value);
            }
             if (prioridadefiltro.HasValue)
            {
                query = query.Where(t => t.Prioridade == prioridadefiltro.Value);
            }
            usuario.Tarefas = await query.ToListAsync();
            ViewBag.FiltroBusca = buscar;
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarTarefaAdmin(int usuarioId, Tarefa tarefa)
        {
            tarefa.UsuarioId = usuarioId;
            tarefa.Prazo = DateTime.SpecifyKind(tarefa.Prazo, DateTimeKind.Utc);
            ModelState.Remove(nameof(Tarefa.Usuario));

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TarefasUsuario), new{id = usuarioId});
        }

        [HttpGet]
        public async Task<IActionResult> EditarTarefaAdmin(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if(tarefa == null)
            {
                return NotFound();
            }
            return View(tarefa);   
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarTarefaAdmin(int id, Tarefa tarefaAtualizada)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if(tarefa == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Tarefa.Usuario));

            if (!ModelState.IsValid)
            {
                return View(tarefaAtualizada);
            }

            tarefa.Titulo = tarefaAtualizada.Titulo;
            tarefa.Descricao = tarefaAtualizada.Descricao;
            tarefa.Status = tarefaAtualizada.Status;
            tarefa.Prioridade = tarefaAtualizada.Prioridade;
            tarefa.CategoriaId = tarefaAtualizada.CategoriaId;
            tarefa.Prazo = DateTime.SpecifyKind(tarefaAtualizada.Prazo, DateTimeKind.Utc);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TarefasUsuario), new{id = tarefa.UsuarioId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTarefaAdmin(int id, int usuarioId)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if(tarefa != null)
            {
                _context.Tarefas.Remove(tarefa);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(TarefasUsuario), new{id = usuarioId});
        }

        [HttpGet]
        public async Task<IActionResult> EditarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if(usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(int id, Usuario dadosAtualizados)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if(usuario == null)
            {
                return NotFound();
            }
            
            usuario.Nome = dadosAtualizados.Nome;
            usuario.Email = dadosAtualizados.Email;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

         [HttpGet]
        public IActionResult CriarAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarAdmin(string username, string password, string confirmarSenha)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Preencha usuário e senha.");
                return View();
            }
            
            if (password != confirmarSenha)
            {
                ModelState.AddModelError(string.Empty, "As senhas não coincidem.");
                return View();
            }

            var jaExiste = await _context.Admin.AnyAsync(a => a.Username == username);
            if (jaExiste)
            {
                ModelState.AddModelError(string.Empty, "Já existe um administrador com esse nome de usuário.");
                return View();
            }

            var hasher = new PasswordHasher<Admin>();
            var novoAdmin = new Admin { Username = username };
            novoAdmin.SenhaHash = hasher.HashPassword(novoAdmin, password);

            _context.Admin.Add(novoAdmin);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Administrador \"{username}\" criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GerenciarCategoria()
        {
            var categorias = await _context.Categorias.Include(c => c.Usuario).ToListAsync();
            return View(categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
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
            return Ok();
        }
    }
}