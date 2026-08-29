using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers

{   public class ContaController : Controller
   {
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public ContaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Registrar()
        {
            return View();
        }

    [HttpPost]
    public async Task<IActionResult> Registrar(string username, string password, string confirmPassword)
        {
            if(string.IsNullOrEmpty(username) | string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Preencha todos os campos");
                return View();
            }
            //verifica se existe
            var usuarioExiste = _context.Usuarios.Any(u => u.Nome == username);
            if (usuarioExiste)
            {
                ModelState.AddModelError(string.Empty, "Este nome de usuario já esta em uso");
                return View();
            }
            if(password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "As senhas não coicidem.");
                return View();
            }
            var novoUsuario = new Usuario{Nome=username};
            novoUsuario.SenhaHash = _passwordHasher.HashPassword(novoUsuario, password);
            //salva no banco de dados
            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Login() => View("Index");

    [HttpPost]
    public async Task<IActionResult> Login(string Username, string Password)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nome == Username);
        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Usuário inválido");
            return View("Index");
        }
        //compara a senha digitada salva a do banco de dados   
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "senha inválida.");
            return View("Index");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        };
        //cria e salva o cookie no navegador
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Remove o Cookie do navegador
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
}