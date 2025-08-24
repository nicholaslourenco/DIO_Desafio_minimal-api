using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Dominio.ModelViews;
using Test.Helpers;
using MinimalApi.Dominio.Interfaces;
using Test.Mocks;

namespace Test.Requests;

[TestClass]
public class VeiculoRequestTest
{
    // Variáveis para os clientes HTTP e o token.
    private static HttpClient _client = default!;
    private static string _token = string.Empty;

    [ClassInitialize]
    public static async Task ClassInit(TestContext testContext)
    {
        // 1. Cria um cliente para a rota de Veículo, injetando o mock de Veículo.
        _client = Setup.CreateClientFor<IVeiculoServico, VeiculoServicoMock>();

        // 2. Para o login, cria-se um cliente temporário para a rota de Administrador.
        var loginClient = Setup.CreateClientFor<IAdministradorServico, AdministradorServicoMock>();
        
        // 3. Obtém o token.
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };
        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");
        var response = await loginClient.PostAsync("/administradores/login", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            _token = admLogado?.Token ?? string.Empty;
        }

        // 4. Adiciona o token obtido ao cliente de Veículo que será usado nos testes.
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }
    
    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client.Dispose();
    }
    
    [TestMethod]
    public async Task TestarInclusaoDeVeiculo()
    {
        // Arrange
        var novoVeiculo = new Veiculo
        {
            Nome = "Fusca",
            Marca = "Volkswagen",
            Ano = 1970
        };
        var content = new StringContent(JsonSerializer.Serialize(novoVeiculo), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/veiculos", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculoCriado);
        Assert.AreEqual(novoVeiculo.Nome, veiculoCriado.Nome);
    }
    
    [TestMethod]
    public async Task TestarBuscaDeTodosVeiculos()
    {
        // Act
        var response = await _client.GetAsync("/veiculos");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculos = JsonSerializer.Deserialize<List<Veiculo>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculos);
        Assert.AreEqual(3, veiculos.Count);
    }

    [TestMethod]
    public async Task TestarBuscaDeVeiculoPorId()
    {
        // Act
        var response = await _client.GetAsync("/veiculos/1");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculo = JsonSerializer.Deserialize<Veiculo>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculo);
        Assert.AreEqual(1, veiculo.Id);
    }

    [TestMethod]
    public async Task TestarAtualizacaoDeVeiculo()
    {
        // Arrange
        var veiculoAtualizado = new Veiculo
        {
            Id = 1,
            Nome = "Golf",
            Marca = "Volkswagen",
            Ano = 2020
        };
        var content = new StringContent(JsonSerializer.Serialize(veiculoAtualizado), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/veiculos/1", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculoNoBanco = JsonSerializer.Deserialize<Veiculo>(await _client.GetStringAsync("/veiculos/1"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.AreEqual("Golf", veiculoNoBanco?.Nome);
    }

    [TestMethod]
    public async Task TestarApagarVeiculo()
    {
        // Act
        var response = await _client.DeleteAsync("/veiculos/1");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        var veiculos = JsonSerializer.Deserialize<List<Veiculo>>(await _client.GetStringAsync("/veiculos"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.AreEqual(2, veiculos.Count);
    }
}



/*using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Dominio.ModelViews;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class VeiculoRequestTest
{
    // Variável para armazenar o token JWT.
    private static string _token = string.Empty;

    [ClassInitialize]
    public static async Task ClassInit(TestContext testContext)
    {
        // 1. Configura o cliente HTTP e injeta o mock do serviço de veículo.
        Setup.ClassInitVeiculo(testContext);

        // 2. Cria os dados de login para obter o token.
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };
        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // 3. Envia a requisição de login.
        var response = await Setup.client.PostAsync("/administradores/login", content);

        // 4. Se a requisição foi bem-sucedida, armazena o token.
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            _token = admLogado?.Token ?? string.Empty;
        }
    }
    
    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }
    
    // Teste para o método Incluir (POST /veiculos)
    [TestMethod]
    public async Task TestarInclusaoDeVeiculo()
    {
        // Arrange
        var novoVeiculo = new Veiculo
        {
            Nome = "Fusca",
            Marca = "Volkswagen",
            Ano = 1970
        };
        var content = new StringContent(JsonSerializer.Serialize(novoVeiculo), Encoding.UTF8, "application/json");

        // Adiciona o cabeçalho de autorização antes de fazer a requisição.
        Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        // Act
        var response = await Setup.client.PostAsync("/veiculos", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculoCriado);
        Assert.AreEqual(novoVeiculo.Nome, veiculoCriado.Nome);
    }
    
    // Teste para o método Todos (GET /veiculos)
    [TestMethod]
    public async Task TestarBuscaDeTodosVeiculos()
    {
        // Adiciona o cabeçalho de autorização.
        Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        // Act
        var response = await Setup.client.GetAsync("/veiculos");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculos = JsonSerializer.Deserialize<List<Veiculo>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculos);
        Assert.AreEqual(3, veiculos.Count);
    }

    // Teste para o método BuscaPorId (GET /veiculos/{id})
    [TestMethod]
    public async Task TestarBuscaDeVeiculoPorId()
    {
        // Adiciona o cabeçalho de autorização.
        Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        // Act
        var response = await Setup.client.GetAsync("/veiculos/1");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculo = JsonSerializer.Deserialize<Veiculo>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculo);
        Assert.AreEqual(1, veiculo.Id);
    }

    // Teste para o método Atualizar (PUT /veiculos/{id})
    [TestMethod]
    public async Task TestarAtualizacaoDeVeiculo()
    {
        // Adiciona o cabeçalho de autorização.
        Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        // Arrange
        var veiculoAtualizado = new Veiculo
        {
            Id = 1,
            Nome = "Golf",
            Marca = "Volkswagen",
            Ano = 2020
        };
        var content = new StringContent(JsonSerializer.Serialize(veiculoAtualizado), Encoding.UTF8, "application/json");

        // Act
        var response = await Setup.client.PutAsync("/veiculos/1", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculoNoBanco = JsonSerializer.Deserialize<Veiculo>(await Setup.client.GetStringAsync("/veiculos/1"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.AreEqual("Golf", veiculoNoBanco?.Nome);
    }

    // Teste para o método Apagar (DELETE /veiculos/{id})
    [TestMethod]
    public async Task TestarApagarVeiculo()
    {
        // Adiciona o cabeçalho de autorização.
        Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        // Act
        var response = await Setup.client.DeleteAsync("/veiculos/1");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculos = JsonSerializer.Deserialize<List<Veiculo>>(await Setup.client.GetStringAsync("/veiculos"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.AreEqual(2, veiculos.Count);
    }
}*/