using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Livro> Livros { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Assunto> Assuntos { get; set; }
        public DbSet<LivroAutor> LivroAutores { get; set; }
        public DbSet<LivroAssunto> LivroAssuntos { get; set; }

        public DbSet<LivroRelatorio> LivrosRelatorio { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chaves compostas
            modelBuilder.Entity<LivroAutor>().HasKey(la => new { la.LivroCod, la.AutorCodAu });
            modelBuilder.Entity<LivroAssunto>().HasKey(la => new { la.LivroCod, la.AssuntoCodAs });

            // Chaves primárias das entidades principais
            modelBuilder.Entity<Autor>().HasKey(a => a.CodAu);
            modelBuilder.Entity<Assunto>().HasKey(a => a.CodAs);
            modelBuilder.Entity<Livro>().HasKey(l => l.Cod);

            modelBuilder.Entity<LivroRelatorio>().HasNoKey().ToView("vw_LivroRelatorio");
        }

        

    }
}
