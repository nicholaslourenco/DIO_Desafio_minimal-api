using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using MinimalApi; // Certifique-se de que este 'using' aponte para o namespace onde sua classe Startup está.

namespace Test.Helpers;

public static class Setup
{
    public const string PORT = "5001";

    /// <summary>
    /// Cria um cliente HTTP para o servidor de teste, injetando um serviço mock.
    /// </summary>
    /// <typeparam name="TService">A interface do serviço (ex: IVeiculoServico).</typeparam>
    /// <typeparam name="TMock">A classe mock que implementa a interface (ex: VeiculoServicoMock).</typeparam>
    /// <returns>Um HttpClient configurado para o servidor de teste.</returns>
    public static HttpClient CreateClientFor<TService, TMock>()
        where TService : class
        where TMock : class, TService
    {
        // A chave aqui é usar a classe Startup, que é o seu ponto de entrada.
        var factory = new WebApplicationFactory<Startup>();

        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", PORT).UseEnvironment("Testing");

            builder.ConfigureTestServices(services =>
            {
                // Este método remove a injeção do serviço de produção e adiciona a sua versão mock.
                // É a forma mais segura de substituir serviços em testes.
                services.AddScoped<TService, TMock>();
            });
        }).CreateClient();

        return client;
    }
}