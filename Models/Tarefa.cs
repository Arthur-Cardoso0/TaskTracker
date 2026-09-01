using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
public enum StatusTarefa{Pendente, [Display(Name = "Em Andamento")]EmAndamento, Concluida}

    public class Tarefa
    {
        public int Id {get; set;}
        public string Titulo {get; set;}
        [Display(Name = "Descrição")]
        public string Descricao {get; set;}
        public StatusTarefa Status {get; set;} = StatusTarefa.Pendente;
        public DateTime Prazo {get; set;}
        public DateTime CriadaEm {get; set;} = DateTime.UtcNow;
        public Prioridade Prioridade{get;set;} = Prioridade.Media;
        public int? CategoriaId{get;set;}
        public Categoria? Categoria{get;set;}

        public int UsuarioId {get; set;}
        public Usuario Usuario {get; set;}

    }
}