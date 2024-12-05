using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Moq;

namespace SefazLib.Tests;

public class IdentityConfigTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly IdentityConfig _identityConfig;

    public IdentityConfigTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.SetupGet(x => x["identity:type"]).Returns("loginsefaz");
        _mockConfiguration.SetupGet(x => x["loginsefaz:ServerRealm"]).Returns("https://example.com");
        _mockConfiguration.SetupGet(x => x["loginsefaz:Metadata"]).Returns("https://example.com/metadata");
        _mockConfiguration.SetupGet(x => x["loginsefaz:ClientId"]).Returns("client-id");
        _mockConfiguration.SetupGet(x => x["loginsefaz:ClientSecret"]).Returns("client-secret");
        _mockConfiguration.SetupGet(x => x["jwt:issuer"]).Returns("issuer");
        _mockConfiguration.SetupGet(x => x["jwt:audience"]).Returns("audience");
        _mockConfiguration.SetupGet(x => x["identity:PrivateKey"]).Returns("<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent><P>...</P><Q>...</Q><DP>...</DP><DQ>...</DQ><InverseQ>...</InverseQ><D>...</D></RSAKeyValue>");

        _identityConfig = new IdentityConfig(_mockConfiguration.Object);
    }

    [Fact]
    public void AuthenticationOptions_ConfiguresCorrectly()
    {
        // Arrange
        var options = new Microsoft.AspNetCore.Authentication.AuthenticationOptions();

        // Act
        _identityConfig.AuthenticationOptions(options);

        // Assert
        Assert.Equal(CookieAuthenticationDefaults.AuthenticationScheme, options.DefaultAuthenticateScheme);
        Assert.Equal(CookieAuthenticationDefaults.AuthenticationScheme, options.DefaultSignInScheme);
        Assert.Equal(OpenIdConnectDefaults.AuthenticationScheme, options.DefaultChallengeScheme);
    }
}
