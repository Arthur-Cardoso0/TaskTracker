using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Usuario
    {
        public int Id {get; set;}
        public string Nome {get; set;}
        [Required][EmailAddress]
        public string Email {get; set;}
        public string SenhaHash {get; set;}
        public ICollection<Tarefa> Tarefas {get; set;}

    }
}