using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    [Keyless]
    public class LivroRelatorio
    {
        public int LivroId { get; set; }
        public string Titulo { get; set; }
        public string Editora { get; set; }
        public int Edicao { get; set; }
        public string AnoPublicacao { get; set; }
        public decimal ValorBase { get; set; }
        public int? AutorId { get; set; }
        public string AutorNome { get; set; }
        public int? AssuntoId { get; set; }
        public string AssuntoNome { get; set; }
        public string FormaCompra { get; set; }
        public decimal? ValorFormaCompra { get; set; }
    }

}
