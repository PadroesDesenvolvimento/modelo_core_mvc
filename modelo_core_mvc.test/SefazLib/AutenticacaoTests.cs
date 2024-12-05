using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Moq;

namespace SefazLib.Tests;

public class AutenticacaoTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Autenticacao _autenticacao;
    private readonly HttpClient _httpClient; // Add this field

    public AutenticacaoTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.SetupGet(x => x["jwt:issuer"]).Returns("https://example.com");
        _mockConfiguration.SetupGet(x => x["jwt:audience"]).Returns("example-audience");
        _mockConfiguration.SetupGet(x => x["identity:PublicKey"]).Returns("https://example.com/publickey");

        _autenticacao = new Autenticacao(_mockConfiguration.Object);
        _httpClient = new HttpClient(); // Initialize the HttpClient
    }

    [Fact]
    public void ConfigureJwtBearerOptions_ConfiguresCorrectly()
    {
        // Arrange
        var options = new JwtBearerOptions();

        // Act
        _autenticacao.ConfigureJwtBearerOptions(options);

        // Assert
        Assert.True(options.TokenValidationParameters.ValidateIssuer);
        Assert.True(options.TokenValidationParameters.ValidateAudience);
        Assert.True(options.TokenValidationParameters.ValidateLifetime);
        Assert.True(options.TokenValidationParameters.ValidateIssuerSigningKey);
        Assert.Equal("https://example.com", options.TokenValidationParameters.ValidIssuer);
        Assert.Equal("example-audience", options.TokenValidationParameters.ValidAudience);
    }
}

