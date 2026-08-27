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
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Login() => View("Index");

    [HttpPost]
    public async Task<IActionResult> Login(string Username, string Password)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == Username);
        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            return View("Index");
        }
        //compara a senha digitada salva a do banco de dados   
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            return View("Index");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        };
        //cria e salva o cookie no navegador
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
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