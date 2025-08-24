using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }


    [TestMethod]
    public void TestandoSalvarVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Fox";
        veiculo.Marca = "Wolksvagen";
        veiculo.Ano = 2007;

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Incluir(veiculo);

        // Assert
        Assert.AreEqual(1, veiculoServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Fox";
        veiculo.Marca = "Wolksvagen";
        veiculo.Ano = 2007;

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        Assert.AreEqual(1, veiculoDoBanco?.Id);
    }

    [TestMethod]
    public void TestandoTodos()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Fox";
        veiculo.Marca = "Wolksvagen";
        veiculo.Ano = 2007;

        var veiculoServico = new VeiculoServico(context);

        // Act
        veiculoServico.Incluir(veiculo);
        var veiculosDoBanco = veiculoServico.Todos(1);

        // Assert
        Assert.AreEqual(1, veiculosDoBanco?.Count());
    }

    [TestMethod]
    public void TestandoUpdate()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Fox";
        veiculo.Marca = "Wolksvagen";
        veiculo.Ano = 2007;

        var veiculoServico = new VeiculoServico(context);
        veiculoServico.Incluir(veiculo);
        var veiculoAntigo = veiculoServico.BuscaPorId(veiculo.Id);

        veiculo.Nome = "Camaro";
        veiculo.Marca = "Chevrolet";
        veiculo.Ano = 2022;

        // Act

        veiculoServico.Atualizar(veiculo);
        var veiculoAtualizado = veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        Assert.AreEqual("Camaro", veiculoAtualizado?.Nome);
    }
    
    [TestMethod]
    public void TestandoApagar()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Fox";
        veiculo.Marca = "Wolksvagen";
        veiculo.Ano = 2007;

        var veiculoServico = new VeiculoServico(context);
        veiculoServico.Incluir(veiculo);
        veiculoServico.Apagar(veiculo);
        var veiculoApagado = veiculoServico.BuscaPorId(veiculo.Id);

        // Act

        // Assert
        Assert.AreEqual(null, veiculoApagado);
    }
}