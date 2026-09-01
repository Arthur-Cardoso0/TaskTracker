namespace TaskTracker.Models
{
    public class Categoria
    {
        public int Id{get;set;}
        public string Nome {get;set;} = string.Empty;
        public int UsuarioId {get;set;}
        public Usuario? Usuario{get;set;}
        public ICollection<Tarefa> Tarefas{get;set;} = new List<Tarefa>();
    }
}