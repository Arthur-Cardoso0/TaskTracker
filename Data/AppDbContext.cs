using TaskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Tarefa> Tarefas {get; set;}
        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Admin> Admin {get; set;}
    }
}