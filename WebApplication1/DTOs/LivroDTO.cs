namespace WebApplication1.DTOs
{
    public class LivroDTO
    {
        public int? Cod { get; set; }
        public string Titulo { get; set; } = null!;
        public string Editora { get; set; } = null!;
        public int Edicao { get; set; }
        public string AnoPublicacao { get; set; } = null!;
        public decimal Valor { get; set; }

        public List<int>? AutorIds { get; set; } = new List<int>();
        public List<int>? AssuntoIds { get; set; } = new List<int>();

        // Nova propriedade para valores por forma de compra
        public List<LivroPrecoDTO>? Precos { get; set; }
    }

    
}
