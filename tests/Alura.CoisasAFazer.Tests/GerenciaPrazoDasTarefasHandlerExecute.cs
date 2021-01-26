using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Alura.CoisasAFazer.Tests
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {
        [Fact]
        public void QuandoTarefasAtrasadasDeveMudarStatus()
        {
            //arrange
            var casaCateg = new Categoria(2, "Casa");
            var trabalhoCateg = new Categoria(3, "trabalho");
            var saudeCateg = new Categoria(4, "saude");
            var higieneCateg = new Categoria(5, "higiene");
            var compraCateg = new Categoria(6, "Compras");

            var tarefas = new List<Tarefa>
            {
                new Tarefa(2,"Fazer almoço",casaCateg,new DateTime(2021,01,30),null,StatusTarefa.Criada),
                new Tarefa(3,"Cortar cabelo",higieneCateg,new DateTime(2021,01,24),null,StatusTarefa.Criada),
                new Tarefa(4,"Comprar carne",compraCateg,new DateTime(2021,02,01),null,StatusTarefa.Criada),
                new Tarefa(5,"Fazer teste de integração",trabalhoCateg,new DateTime(2021,01,25),null,StatusTarefa.Criada),
                new Tarefa(6,"Tirar Lixo",casaCateg,new DateTime(2021,01,26),null,StatusTarefa.Criada),
            };

            var option = new DbContextOptionsBuilder<DbTarefasContext>()
               .UseInMemoryDatabase("DbTarefas")
               .Options;
            var contexto = new DbTarefasContext(option);
            var repo = new RepositorioTarefa(contexto);

            repo.IncluirTarefas(tarefas.ToArray());

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2021, 01, 25));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            var tarefasEmAtraso = repo.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(2, tarefasEmAtraso.Count());

        }

        [Fact]
        public void QuandoInvocadeDeveChamarAtualizarListaDeTarefasApenasUmaVez()
        {
            //arrange
            var categ = new Categoria("dummy");
            var tarefas = new List<Tarefa>
            {
                new Tarefa(20,"Fazer almoço",categ,new DateTime(2021,01,30),null,StatusTarefa.Criada),
                new Tarefa(30,"Cortar cabelo",categ,new DateTime(2021,01,24),null,StatusTarefa.Criada),
                new Tarefa(40,"Comprar carne",categ,new DateTime(2021,02,01),null,StatusTarefa.Criada),
            };

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>()))
                .Returns(tarefas);

            var repo = mock.Object;

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2021, 01, 25));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act 
            handler.Execute(comando);

            mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());
        }
    }
}
