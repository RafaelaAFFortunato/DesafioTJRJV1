namespace WebApplication1.Models
{
    public class LivroAssunto
    {
        public int LivroCod { get; set; }
        public Livro Livro { get; set; }

        public int AssuntoCodAs { get; set; }
        public Assunto Assunto { get; set; }
    }
}
