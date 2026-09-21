using Microsoft.AspNetCore.Authentication.Cookies;
using TaskTracker.Data;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddControllersWithViews(); 

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
        options.AccessDeniedPath = "/Home/AcessoNegado";
    })
    .AddCookie("AdminScheme", options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Home/AcessoNegado";
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    if (!context.Admin.Any())
    {
        var username = builder.Configuration["AdminSeed:Username"];
        var password = builder.Configuration["AdminSeed:Password"];
        if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Configure a senha do administrador e a senha antes de iniciar a aplicaçao");
        }

        var hasher = new PasswordHasher<Admin>();
        var admin = new Admin
        {
            Username = username,
            SenhaHash = hasher.HashPassword(null!, password)
        };

        context.Admin.Add(admin);
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();