namespace WebApplication1.DTOs
{
    public class AutorDTO
    {
        public int? CodAu { get; set; }   // null para POST, preenchido para PUT
        public string Nome { get; set; } = null!;
    }
}
