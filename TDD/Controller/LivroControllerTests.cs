using Xunit;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Tests
{
    public class LivroControllerTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(options);

            if (!context.Livros.Any())
            {
                var autor1 = new Autor { CodAu = 1, Nome = "Autor 1" };
                var autor2 = new Autor { CodAu = 2, Nome = "Autor 2" };

                var assunto1 = new Assunto { CodAs = 1, Descricao = "Assunto 1" };
                var assunto2 = new Assunto { CodAs = 2, Descricao = "Assunto 2" };

                context.Autores.AddRange(autor1, autor2);
                context.Assuntos.AddRange(assunto1, assunto2);

                var livro = new Livro
                {
                    Cod = 1,
                    Titulo = "Livro 1",
                    Editora = "Editora 1",
                    Edicao = 1,
                    AnoPublicacao = "2020",
                    Valor = 100,
                    LivroAutores = new List<LivroAutor>
                    {
                        new LivroAutor { AutorCodAu = 1 },
                        new LivroAutor { AutorCodAu = 2 }
                    },
                    LivroAssuntos = new List<LivroAssunto>
                    {
                        new LivroAssunto { AssuntoCodAs = 1 },
                        new LivroAssunto { AssuntoCodAs = 2 }
                    },
                    LivroPrecos = new List<LivroPreco>
                    {
                        new LivroPreco { FormaCompra = "Compra 1", Valor = 90 },
                        new LivroPreco { FormaCompra = "Compra 2", Valor = 80 }
                    }
                };

                context.Livros.Add(livro);
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task GetLivros_RetornaTodosLivrosComRelacionamentos()
        {
            var context = GetDbContext("GetLivrosDb");
            var controller = new LivroController(context);

            var result = await controller.GetLivros();

            Assert.NotNull(result.Value);
            var livros = result.Value.ToList();
            Assert.Single(livros);

            var livro = livros[0];
            Assert.Equal("Livro 1", livro.Titulo);
            Assert.Equal(2, livro.LivroAutores.Count);
            Assert.Equal(2, livro.LivroAssuntos.Count);
            Assert.Equal(2, livro.LivroPrecos.Count);
        }

        [Fact]
        public async Task GetLivro_Existente_RetornaLivroComRelacionamentos()
        {
            var context = GetDbContext("GetLivroExistenteDb");
            var controller = new LivroController(context);

            var actionResult = await controller.GetLivro(1);
            Assert.IsType<OkObjectResult>(actionResult.Result);

            var livro = actionResult.Value;
            Assert.NotNull(livro);
            Assert.Equal(1, livro.Cod);
            Assert.Equal("Livro 1", livro.Titulo);
            Assert.Equal(2, livro.LivroAutores.Count);
            Assert.Equal(2, livro.LivroAssuntos.Count);
            Assert.Equal(2, livro.LivroPrecos.Count);
        }

        [Fact]
        public async Task GetLivro_NaoExistente_RetornaNotFound()
        {
            var context = GetDbContext("GetLivroNaoExistenteDb");
            var controller = new LivroController(context);

            var actionResult = await controller.GetLivro(999);

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostLivro_NovoLivro_RetornaSucesso()
        {
            var context = GetDbContext("PostLivroDb");
            var controller = new LivroController(context);

            var dto = new LivroDTO
            {
                Titulo = "Novo Livro",
                Editora = "Nova Editora",
                Edicao = 1,
                AnoPublicacao = "2022",
                Valor = 150,
                AutorIds = new List<int> { 1 },
                AssuntoIds = new List<int> { 2 },
                Precos = new List<LivroPrecoDTO>
                {
                    new LivroPrecoDTO { FormaCompra = "Compra Online", Valor = 140 }
                }
            };

            var result = await controller.PostLivro(dto);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Livro criado com sucesso.", result.Value.Mensagem);

            Assert.Equal(2, context.Livros.Count());
        }

        [Fact]
        public async Task PostLivro_Duplicado_RetornaErro()
        {
            var context = GetDbContext("PostLivroDuplicadoDb");
            var controller = new LivroController(context);

            var dto = new LivroDTO
            {
                Titulo = "Livro 1",
                Editora = "Editora 1"
            };

            var result = await controller.PostLivro(dto);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Já existe um livro com este título e editora.", result.Value.Mensagem);
        }

        [Fact]
        public async Task PutLivro_AtualizaLivro_RetornaSucesso()
        {
            var context = GetDbContext("PutLivroDb");
            var controller = new LivroController(context);

            var dto = new LivroDTO
            {
                Cod = 1,
                Titulo = "Livro Atualizado",
                Editora = "Editora Atualizada",
                Edicao = 2,
                AnoPublicacao = "2023",
                Valor = 200,
                AutorIds = new List<int> { 2 },
                AssuntoIds = new List<int> { 1 },
                Precos = new List<LivroPrecoDTO>
                {
                    new LivroPrecoDTO { FormaCompra = "Compra Física", Valor = 190 }
                }
            };

            var result = await controller.PutLivro(1, dto);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Livro atualizado com sucesso.", result.Value.Mensagem);

            var livroAtualizado = await context.Livros
                .Include(l => l.LivroAutores)
                .Include(l => l.LivroAssuntos)
                .Include(l => l.LivroPrecos)
                .FirstOrDefaultAsync(l => l.Cod == 1);

            Assert.Equal("Livro Atualizado", livroAtualizado.Titulo);
            Assert.Single(livroAtualizado.LivroAutores);
            Assert.Single(livroAtualizado.LivroAssuntos);
            Assert.Single(livroAtualizado.LivroPrecos);
        }

        [Fact]
        public async Task PutLivro_IdInvalido_RetornaBadRequest()
        {
            var context = GetDbContext("PutLivroIdInvalidoDb");
            var controller = new LivroController(context);

            var dto = new LivroDTO
            {
                Cod = 2,
                Titulo = "Outro Livro",
                Editora = "Editora X"
            };

            var result = await controller.PutLivro(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Livro>>(badRequest.Value);

            Assert.False(response.Sucesso);
            Assert.Equal("ID inválido.", response.Mensagem);
        }

        [Fact]
        public async Task PutLivro_LivroNaoEncontrado_RetornaNotFound()
        {
            var context = GetDbContext("PutLivroNaoEncontradoDb");
            var controller = new LivroController(context);

            var dto = new LivroDTO
            {
                Cod = 999,
                Titulo = "Livro Desconhecido",
                Editora = "Editora X"
            };

            var result = await controller.PutLivro(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Livro>>(notFound.Value);

            Assert.False(response.Sucesso);
            Assert.Equal("Livro não encontrado.", response.Mensagem);
        }

        [Fact]
        public async Task PutLivro_Duplicado_RetornaBadRequest()
        {
            var context = GetDbContext("PutLivroDuplicadoDb");
            var controller = new LivroController(context);

            var dto = new LivroDTO
            {
                Cod = 1,
                Titulo = "Livro 1",
                Editora = "Editora 1"
            };

            var result = await controller.PutLivro(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<Livro>>(badRequest.Value);

            Assert.False(response.Sucesso);
            Assert.Equal("Já existe outro livro com este título e editora.", response.Mensagem);
        }

        [Fact]
        public async Task DeleteLivro_Existente_RetornaSucesso()
        {
            var context = GetDbContext("DeleteLivroDb");
            var controller = new LivroController(context);

            var result = await controller.DeleteLivro(1);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Livro excluído com sucesso.", result.Value.Mensagem);
            Assert.Empty(context.Livros);
        }

        [Fact]
        public async Task DeleteLivro_NaoExistente_RetornaErro()
        {
            var context = GetDbContext("DeleteLivroNaoExistenteDb");
            var controller = new LivroController(context);

            var result = await controller.DeleteLivro(999);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Livro não encontrado.", result.Value.Mensagem);
        }
    }
}
