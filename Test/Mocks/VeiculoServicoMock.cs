using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using System.Linq;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{
    // A lista estática é usada para simular o banco de dados em memória.
    private static List<Veiculo> veiculos = new List<Veiculo>()
    {
        new Veiculo { Id = 1, Nome = "Fox", Marca = "Volkswagen", Ano = 2007 },
        new Veiculo { Id = 2, Nome = "Camaro", Marca = "Chevrolet", Ano = 2022 }
    };

    public static void Reset()
    {
        veiculos = new List<Veiculo>()
        {
            new Veiculo { Id = 1, Nome = "Fox", Marca = "Volkswagen", Ano = 2007 },
            new Veiculo { Id = 2, Nome = "Camaro", Marca = "Chevrolet", Ano = 2022 }
        };
    }

    public void Apagar(Veiculo veiculo)
    {
        veiculos.Remove(veiculo);
    }

    public void Atualizar(Veiculo veiculo)
    {
        var veiculoOriginal = veiculos.FirstOrDefault(v => v.Id == veiculo.Id);
        if (veiculoOriginal != null)
        {
            veiculoOriginal.Nome = veiculo.Nome;
            veiculoOriginal.Marca = veiculo.Marca;
            veiculoOriginal.Ano = veiculo.Ano;
        }
    }

    public Veiculo? BuscaPorId(int id)
    {
        return veiculos.FirstOrDefault(v => v.Id == id);
    }

    // O método Incluir deve ser void para seguir a interface.
    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = veiculos.Count + 1;
        veiculos.Add(veiculo);
    }

    public List<Veiculo> Todos(int? pagina, string? nome = null, string? marca = null)
    {
        return veiculos;
    }
}