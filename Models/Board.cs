namespace TaskTracker.Models
{
    public class BoardList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();
    }

    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int OwnerId { get; set; }
        public Usuario Owner { get; set; } = null!;
        public ICollection<BoardList> Lists { get; set; } = new List<BoardList>();
    }
}