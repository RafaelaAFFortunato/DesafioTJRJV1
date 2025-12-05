using Xunit;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApplication1.Tests
{
    public class AssuntoControllerTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(options);

            if (!context.Assuntos.Any())
            {
                context.Assuntos.AddRange(
                    new Assunto { CodAs = 1, Descricao = "Assunto 1" },
                    new Assunto { CodAs = 2, Descricao = "Assunto 2" }
                );
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task GetAssuntos_RetornaTodosAssuntos()
        {
            var context = GetDbContext("GetAssuntosDb");
            var controller = new AssuntoController(context);

            var result = await controller.GetAssuntos();

            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAssunto_Existente_RetornaAssunto()
        {
            var context = GetDbContext("GetAssuntoExistenteDb");
            var controller = new AssuntoController(context);

            var actionResult = await controller.GetAssunto(1);
            Assert.IsType<OkObjectResult>(actionResult.Result);

            var assunto = actionResult.Value;
            Assert.NotNull(assunto);
            Assert.Equal(1, assunto.CodAs);
            Assert.Equal("Assunto 1", assunto.Descricao);
        }

        [Fact]
        public async Task GetAssunto_NaoExistente_RetornaNotFound()
        {
            var context = GetDbContext("GetAssuntoNaoExistenteDb");
            var controller = new AssuntoController(context);

            var actionResult = await controller.GetAssunto(999);

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostAssunto_NovoAssunto_RetornaSucesso()
        {
            var context = GetDbContext("PostAssuntoDb");
            var controller = new AssuntoController(context);

            var dto = new AssuntoDTO { Descricao = "Novo Assunto" };
            var result = await controller.PostAssunto(dto);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Assunto criado com sucesso.", result.Value.Mensagem);
            Assert.Equal(3, context.Assuntos.Count());
        }

        [Fact]
        public async Task PostAssunto_DescricaoDuplicada_RetornaErro()
        {
            var context = GetDbContext("PostAssuntoDuplicadoDb");
            var controller = new AssuntoController(context);

            var dto = new AssuntoDTO { Descricao = "Assunto 1" };
            var result = await controller.PostAssunto(dto);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Já existe um assunto com esta descrição.", result.Value.Mensagem);
        }

        [Fact]
        public async Task PutAssunto_AtualizaAssunto_RetornaSucesso()
        {
            var context = GetDbContext("PutAssuntoDb");
            var controller = new AssuntoController(context);

            var dto = new AssuntoDTO { Descricao = "Assunto Atualizado" };
            var result = await controller.PutAssunto(1, dto);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Assunto atualizado com sucesso.", result.Value.Mensagem);

            var assuntoAtualizado = await context.Assuntos.FindAsync(1);
            Assert.Equal("Assunto Atualizado", assuntoAtualizado.Descricao);
        }

        [Fact]
        public async Task PutAssunto_NaoExistente_RetornaErro()
        {
            var context = GetDbContext("PutAssuntoNaoExistenteDb");
            var controller = new AssuntoController(context);

            var dto = new AssuntoDTO { Descricao = "Qualquer Descricao" };
            var result = await controller.PutAssunto(999, dto);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Assunto não encontrado.", result.Value.Mensagem);
        }

        [Fact]
        public async Task PutAssunto_DescricaoDuplicada_RetornaErro()
        {
            var context = GetDbContext("PutAssuntoDuplicadoDb");
            var controller = new AssuntoController(context);

            var dto = new AssuntoDTO { Descricao = "Assunto 2" };
            var result = await controller.PutAssunto(1, dto);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Já existe outro assunto com esta descrição.", result.Value.Mensagem);
        }

        [Fact]
        public async Task DeleteAssunto_Existente_RetornaSucesso()
        {
            var context = GetDbContext("DeleteAssuntoDb");
            var controller = new AssuntoController(context);

            var result = await controller.DeleteAssunto(1);

            Assert.True(result.Value.Sucesso);
            Assert.Equal("Assunto excluído com sucesso.", result.Value.Mensagem);
            Assert.Equal(1, context.Assuntos.Count());
        }

        [Fact]
        public async Task DeleteAssunto_NaoExistente_RetornaErro()
        {
            var context = GetDbContext("DeleteAssuntoNaoExistenteDb");
            var controller = new AssuntoController(context);

            var result = await controller.DeleteAssunto(999);

            Assert.False(result.Value.Sucesso);
            Assert.Equal("Assunto não encontrado.", result.Value.Mensagem);
        }
    }
}
