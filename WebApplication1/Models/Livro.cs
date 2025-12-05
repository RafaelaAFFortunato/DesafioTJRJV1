namespace WebApplication1.Models
{
    public class Livro
    {
        public int Cod { get; set; }
        public string Titulo { get; set; }
        public string Editora { get; set; }
        public int Edicao { get; set; }
        public string AnoPublicacao { get; set; }

        public decimal Valor { get; set; } // R$

        public ICollection<LivroAutor> LivroAutores { get; set; }
        public ICollection<LivroAssunto> LivroAssuntos { get; set; }

        // Nova relação: valores por forma de compra
        public ICollection<LivroPreco> LivroPrecos { get; set; } = new List<LivroPreco>();
    }

    public class LivroPreco
    {
        public int Id { get; set; }
        public int LivroCod { get; set; } // FK para Livro
        public Livro Livro { get; set; }

        public string FormaCompra { get; set; } // Ex: Balcão, Internet, Evento, Self-service
        public decimal Valor { get; set; }      // Valor para essa forma de compra
    }
}

