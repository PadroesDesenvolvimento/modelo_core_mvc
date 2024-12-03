﻿using modelo_core_mvc.ProjetosApi;
using modelo_core_mvc.projetos;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using SefazLib;

namespace modelo_core_mvc.test.HttpClients
{
    public class ProjetosApiClientTest
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly Mock<IdentityConfig> _mockIdentityConfig;
        private readonly ProjetosApiClient _apiClient;

        public ProjetosApiClientTest()
        {
            // Carregar a configuração da aplicação
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = configurationBuilder.Build();

            // Configurar o mock de HttpMessageHandler
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            // Configurar o mock de IdentityConfig
            _mockIdentityConfig = new Mock<IdentityConfig>(_configuration);

            // Configurar o ProjetosApiClient
            _apiClient = new ProjetosApiClient(_httpClient, _configuration, _mockIdentityConfig.Object);
        }

        [Fact]
        public async Task GetProjetoAsync_ReturnsProjeto()
        {
            // Arrange
            var projetoId = 1;
            var projetoJson = "{\"id\":1,\"nome\":\"Projeto Teste\",\"descricao\":\"Descrição do projeto\"}";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri(_httpClient.BaseAddress, $"Projetos/{projetoId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(projetoJson)
                });

            // Act
            var result = await _apiClient.GetProjetoAsync(projetoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projetoId, result.id);
            Assert.Equal("Projeto Teste", result.nome);
            Assert.Equal("Descrição do projeto", result.descricao);
        }

        [Fact]
        public async Task GetProjetosAsync_ReturnsListOfProjetos()
        {
            // Arrange
            var projetosJson = "[{\"id\":1,\"nome\":\"Projeto Teste 1\",\"descricao\":\"Descrição do projeto 1\"}, {\"id\":2,\"nome\":\"Projeto Teste 2\",\"descricao\":\"Descrição do projeto 2\"}]";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri(_httpClient.BaseAddress, "Projetos")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(projetosJson)
                });

            // Act
            var result = await _apiClient.GetProjetosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task PostProjetoAsync_CreatesProjeto()
        {
            // Arrange
            var projeto = new ProjetosModel { id = 1, nome = "Novo Projeto", descricao = "Descrição do novo projeto" };
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri == new Uri(_httpClient.BaseAddress, "Projetos")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created
                });

            // Act
            await _apiClient.PostProjetoAsync(projeto);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri == new Uri(_httpClient.BaseAddress, "Projetos")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task PutProjetoAsync_UpdatesProjeto()
        {
            // Arrange
            var projeto = new ProjetosModel { id = 1, nome = "Projeto Atualizado", descricao = "Descrição atualizada do projeto" };
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && req.RequestUri == new Uri(_httpClient.BaseAddress, $"Projetos/{projeto.id}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            await _apiClient.PutProjetoAsync(projeto);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && req.RequestUri == new Uri(_httpClient.BaseAddress, $"Projetos/{projeto.id}")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task DeleteProjetoAsync_DeletesProjeto()
        {
            // Arrange
            var projetoId = 1;
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete && req.RequestUri == new Uri(_httpClient.BaseAddress, $"Projetos/{projetoId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            await _apiClient.DeleteProjetoAsync(projetoId);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete && req.RequestUri == new Uri(_httpClient.BaseAddress, $"Projetos/{projetoId}")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetStatusAsync_ReturnsStatus()
        {
            // Arrange
            var status = "API is running";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri(_httpClient.BaseAddress, "projetos/status")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(status)
                });

            // Act
            var result = await _apiClient.GetStatusAsync();

            // Assert
            Assert.Equal(status, result);
        }

        [Fact]
        public async Task GetConexaoAsync_ReturnsConexao()
        {
            // Arrange
            var conexao = "Connection successful";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri(_httpClient.BaseAddress, "projetos/conexao")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(conexao)
                });

            // Act
            var result = await _apiClient.GetConexaoAsync();

            // Assert
            Assert.Equal(conexao, result);
        }

        [Fact]
        public async Task GetAnexoAsync_ReturnsAnexo()
        {
            // Arrange
            var projetoId = 1;
            var anexoContent = new byte[] { 1, 2, 3, 4, 5 };
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri(_httpClient.BaseAddress, $"Projetos/{projetoId}/anexo")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent(anexoContent)
                });

            // Act
            var result = await _apiClient.GetAnexoAsync(projetoId);

            // Assert
            Assert.Equal(anexoContent, result);
        }
    }
}
