using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act
        veiculo.Id = 1;
        veiculo.Nome = "nomeTeste";
        veiculo.Marca = "marcaTeste";
        veiculo.Ano = 2007;

        // Assert
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("nomeTeste", veiculo.Nome);
        Assert.AreEqual("marcaTeste", veiculo.Marca);
        Assert.AreEqual(2007, veiculo.Ano);
    }
}