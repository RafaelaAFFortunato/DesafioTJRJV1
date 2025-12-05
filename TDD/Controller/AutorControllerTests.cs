using Xunit;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Tests
{
    public class AutorControllerTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(options);

            if (!context.Autores.Any())
            {
                context.Autores.AddRange(
                    new Autor { CodAu = 1, Nome = "Autor 1" },
                    new Autor { CodAu = 2, Nome = "Autor 2" }
                );
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task GetAutores_RetornaTodosAutores()
        {
            var context = GetDbContext("GetAutoresDb");
            var controller = new AutorController(context);

            var result = await controller.GetAutores();

            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAutor_AutorExistente_RetornaSucesso()
        {
            var context = GetDbContext("GetAutorOkDb");
            var controller = new AutorController(context);

            var result = await controller.GetAutor(1);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Autor encontrado.", result.Value.Mensagem);
        }

        [Fact]
        public async Task GetAutor_AutorNaoExistente_RetornaErro()
        {
            var context = GetDbContext("GetAutorNotFoundDb");
            var controller = new AutorController(context);

            var result = await controller.GetAutor(999);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Autor não encontrado.", result.Value.Mensagem);
        }

        [Fact]
        public async Task PostAutor_NovoAutor_RetornaSucesso()
        {
            var context = GetDbContext("PostAutorDb");
            var controller = new AutorController(context);

            var dto = new AutorDTO { Nome = "Novo Autor" };
            var result = await controller.PostAutor(dto);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Autor criado com sucesso!", result.Value.Mensagem);
            Assert.Equal(3, context.Autores.Count());
        }

        [Fact]
        public async Task PostAutor_AutorDuplicado_RetornaErro()
        {
            var context = GetDbContext("PostAutorDuplicadoDb");
            var controller = new AutorController(context);

            var dto = new AutorDTO { Nome = "Autor 1" };
            var result = await controller.PostAutor(dto);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Já existe um autor com esta descrição.", result.Value.Mensagem);
        }

        [Fact]
        public async Task PutAutor_AtualizaAutor_RetornaSucesso()
        {
            var context = GetDbContext("PutAutorDb");
            var controller = new AutorController(context);

            var dto = new AutorDTO { Nome = "Autor Atualizado" };
            var result = await controller.PutAutor(1, dto);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Autor atualizado com sucesso.", result.Value.Mensagem);

            var autorAtualizado = await context.Autores.FindAsync(1);
            Assert.Equal("Autor Atualizado", autorAtualizado.Nome);
        }

        [Fact]
        public async Task PutAutor_IdInvalido_RetornaErro()
        {
            var context = GetDbContext("PutAutorIdInvalidoDb");
            var controller = new AutorController(context);

            var dto = new AutorDTO { CodAu = 2, Nome = "Novo Nome" };
            var result = await controller.PutAutor(1, dto);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("ID inválido.", result.Value.Mensagem);
        }

        [Fact]
        public async Task PutAutor_NomeDuplicado_RetornaErro()
        {
            var context = GetDbContext("PutAutorNomeDuplicadoDb");
            var controller = new AutorController(context);

            var dto = new AutorDTO { Nome = "Autor 2" };
            var result = await controller.PutAutor(1, dto);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Já existe outro autor com este nome.", result.Value.Mensagem);
        }

        [Fact]
        public async Task DeleteAutor_Existente_RetornaSucesso()
        {
            var context = GetDbContext("DeleteAutorDb");
            var controller = new AutorController(context);

            var result = await controller.DeleteAutor(1);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Autor excluído com sucesso.", result.Value.Mensagem);
            Assert.Equal(1, context.Autores.Count());
        }

        [Fact]
        public async Task DeleteAutor_NaoExistente_RetornaErro()
        {
            var context = GetDbContext("DeleteAutorNaoExistenteDb");
            var controller = new AutorController(context);

            var result = await controller.DeleteAutor(999);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Autor não encontrado.", result.Value.Mensagem);
        }
    }
}
