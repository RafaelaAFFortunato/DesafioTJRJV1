namespace WebApplication1.DTOs
{
    public class AssuntoDTO
    {
        public int? CodAs { get; set; }   // null para POST, preenchido para PUT
        public string Descricao { get; set; } = null!;
    }
}
