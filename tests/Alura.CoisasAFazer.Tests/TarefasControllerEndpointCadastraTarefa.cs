using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Alura.CoisasAFazer.Tests
{
    public class TarefasControllerEndpointCadastraTarefa
    {
        [Fact]
        public void DadoUmaTarefaComInformacoesValidasDeveRetornar200()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);

            contexto.Categorias.Add(new Categoria(20, "Estudar"));
            contexto.SaveChanges();

            var controlador = new TarefasController(repo, mockLogger.Object);
            var model = new CadastraTarefaVM
            {
                IdCategoria = 20,
                Titulo = "Estudar XUnit",
                Prazo = new DateTime(2021, 01, 26)
            };

            //act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<OkResult>(retorno); //200
        }

        [Fact]
        public void QuandoExececaoForLancadoDeveRetornar500()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();


            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemCategoriaPorId(20))
                .Returns(new Categoria(20, "Estudos"));

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro"));

            var repo = mock.Object;

            var controlador = new TarefasController(repo, mockLogger.Object);
            var model = new CadastraTarefaVM
            {
                IdCategoria = 20,
                Titulo = "Estudar XUnit",
                Prazo = new DateTime(2021, 01, 26)
            };

            //act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<StatusCodeResult>(retorno);
            var statusCodeRetornado = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCodeRetornado);
        }
    }
}
