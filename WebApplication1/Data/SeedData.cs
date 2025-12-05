using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (!context.Autores.Any())
            {
                var autor1 = new Autor { CodAu = 1, Nome = "Autor A" };
                var autor2 = new Autor { CodAu = 2, Nome = "Autor B" };
                context.Autores.AddRange(autor1, autor2);
            }

            if (!context.Assuntos.Any())
            {
                var assunto1 = new Assunto { CodAs = 1, Descricao = "Ficção" };
                var assunto2 = new Assunto { CodAs = 2, Descricao = "Tecnologia" };
                context.Assuntos.AddRange(assunto1, assunto2);
            }

            if (!context.Livros.Any())
            {
                var livro1 = new Livro { Cod = 1, Titulo = "Livro 1", Editora = "Editora X", Edicao = 1, AnoPublicacao = "2023", Valor = 88 };
                
                context.Livros.Add(livro1);

                context.LivroAutores.Add(new LivroAutor { Livro = livro1, AutorCodAu = 1 });
                context.LivroAssuntos.Add(new LivroAssunto { Livro = livro1, AssuntoCodAs = 1 });
            }

            context.SaveChanges();
        }
    }
}
