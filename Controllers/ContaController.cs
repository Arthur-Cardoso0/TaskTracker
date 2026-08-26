using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace TaskTracker.Controllers
{
    [Route("[controller]")]
    public class ContaController : Controller
   {
    [HttpGet]
    public IActionResult index() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (username == "admin" && password == "123456")
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Cria e salva o Cookie de autenticação no navegador
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
        return View();
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