using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using TokenWS;
using System.Security.Claims;
using System.Threading;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Xml;
using System.Net.Security;

namespace SefazLib;

public class IdentityConfig
{
    private static IConfiguration configuration;
    public Action<WsFederationOptions> WSFederationOptions { get; private set; }
    public Action<CookieAuthenticationOptions> CookieAuthenticationOptions { get; private set; }
    public Action<Microsoft.AspNetCore.Authentication.AuthenticationOptions> AuthenticationOptions { get; private set; }
    public Action<OpenIdConnectOptions> OpenIdConnectOptions { get; private set; }
    public static Boolean Logoff { get; private set; }
    public HttpClient httpClient;
    public static string jwtToken { get; set; }
    public string erro;
    public string[] scopes;
    public Dictionary<string, string> tokenInfo;

    public IdentityConfig(string accessToken)
    {
        jwtToken = accessToken;
    }
    public IdentityConfig(IConfiguration Configuration)
    {
        httpClient = new HttpClient();
        configuration = Configuration;
        Logoff = false;

        AuthenticationOptions = options =>
        {
            switch (Configuration["identity:type"])
            {
                case ("loginsefaz"):
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    break;
                default:
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    break;
            }
        };

        WSFederationOptions = options =>
        {
            switch (Configuration["identity:type"])
            {
                case ("loginsefaz"):
                    options.Wtrealm = configuration["loginsefaz:ServerRealm"];
                    options.MetadataAddress = configuration["loginsefaz:Metadata"];
                    break;

                case ("sefazidentity"):
                    if (configuration["sefazidentity:realm"] is not null)
                    {
                        options.Wtrealm = configuration["sefazidentity:realm"];
                    }
                    else
                    {
                        throw new Exception("Realm não configurado. Verifique se existe sefazidentity:realm no appSettings.json");
                    }
                    options.MetadataAddress = configuration["sefazidentity:metadataaddress"];
                    if (configuration["sefazidentity:reply"] is not null)
                    {
                        options.Wreply = configuration["sefazidentity:reply"];
                    }
                    options.Wreply = configuration["identity:reply"];
                    options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;
                    options.Events.OnSecurityTokenReceived = OnSecurityTokenReceived;
                    options.TokenValidationParameters = new TokenValidationParameters { SaveSigninToken = true };
                    // Remove todos os TokenHandlers registrados, que por padrão são 3:
                    options.TokenHandlers.Clear();
                    // Acrescenta os TokenHandlers customizados para tokens SAML2 e SAML
                    options.TokenHandlers.Add(new CustomSaml2SecurityTokenHandler());
                    options.TokenHandlers.Add(new CustomSamlSecurityTokenHandler());
                    // Acrescenta novamente o TokenHandler para tokens JWT
                    options.TokenHandlers.Add(new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler());
                    break;
            }
        };

        if (Configuration["identity:type"] == "loginsefaz")
        {
            OpenIdConnectOptions = options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = configuration["loginsefaz:ServerRealm"];
                options.MetadataAddress = configuration["loginsefaz:Metadata"];
                options.ClientId = configuration["loginsefaz:ClientId"];
                options.ClientSecret = configuration["loginsefaz:ClientSecret"];
                options.RequireHttpsMetadata = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.SaveTokens = true;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.NonceCookie.SameSite = SameSiteMode.Unspecified;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = ClaimTypes.Role,
                    ValidateIssuer = true
                };

                options.Events.OnSignedOutCallbackRedirect += context =>
                {
                    context.Response.Redirect(context.Options.SignedOutRedirectUri);
                    context.HandleResponse();

                    return Task.CompletedTask;
                };

                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        // Implementar a lógica de validação personalizada aqui
                        return errors == SslPolicyErrors.None;
                    }
                };
            };
        }

        CookieAuthenticationOptions = options =>
        {
            options.ExpireTimeSpan = new TimeSpan(0, 0, int.Parse(configuration["sefazidentity:timeout"]), 0);
            options.SlidingExpiration = false;
        };
    }

    public static string ConvertSamlToJwt(string samlToken, IConfiguration configuration)
    {
        var issuer = configuration["jwt:issuer"]; // endpoint dessa aplicacao
        var audience = configuration["jwt:audience"];  // endpoint da aplicacao cliente que vai usar esse servico
        var privateKeyXml = configuration["identity:PrivateKey"];
        if (string.IsNullOrEmpty(privateKeyXml))
        {
            throw new Exception("Chave privada não configurada.");
        }

        var rsa = RSA.Create(2048);
        rsa.FromXmlString(privateKeyXml);
        var key = new RsaSecurityKey(rsa);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var samlTokenXml = new XmlDocument();
        samlTokenXml.LoadXml(samlToken);

        var claims = new ClaimsIdentity();
        var samlAssertionNode = samlTokenXml.SelectSingleNode("//*[local-name()='Assertion']");
        if (samlAssertionNode == null)
        {
            return "Token SAML inválido.";
        }
        var samlAttributeNodes = samlAssertionNode.SelectNodes("//*[local-name()='Attribute']");
        if (samlAttributeNodes == null)
        {
            return "Token SAML não possui claims.";
        }
        foreach (XmlNode attributeNode in samlAttributeNodes)
        {
            var attributeValue = attributeNode.FirstChild?.InnerText;
            var attributeName = attributeNode.Attributes?["Name"]?.Value;
            if (attributeName is not null && attributeValue is not null)
            {
                claims.AddClaim(new Claim(attributeName, attributeValue));
            }
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Issuer = issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = credentials
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(securityToken);

        return jwtToken;
    }

    public AuthenticationHeaderValue AuthenticationHeader()
    {
        if (jwtToken != "" && jwtToken is not null)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }
        }

        return httpClient.DefaultRequestHeaders.Authorization;
    }

    public class CustomSaml2SecurityTokenHandler : Microsoft.IdentityModel.Tokens.Saml2.Saml2SecurityTokenHandler
    {
        public override async Task<TokenValidationResult> ValidateTokenAsync(string token,
        TokenValidationParameters tokenValidationParameters)
        {
            if ((string.IsNullOrEmpty(tokenValidationParameters.ValidIssuer) &&
            (tokenValidationParameters.ValidIssuers?.Any() != true)) ||
            (tokenValidationParameters.IssuerSigningKeys?.Any() != true))
            {
                var baseConfiguration = await
                tokenValidationParameters.ConfigurationManager.GetBaseConfigurationAsync(
                CancellationToken.None);
                tokenValidationParameters.ValidIssuer = baseConfiguration.Issuer;
                tokenValidationParameters.IssuerSigningKeys = baseConfiguration.SigningKeys;
            }
            return await base.ValidateTokenAsync(token, tokenValidationParameters);
        }
    }
    public class CustomSamlSecurityTokenHandler :
     Microsoft.IdentityModel.Tokens.Saml.SamlSecurityTokenHandler
    {
        public override async Task<TokenValidationResult> ValidateTokenAsync(string token,
        TokenValidationParameters tokenValidationParameters)
        {
            if ((string.IsNullOrEmpty(tokenValidationParameters.ValidIssuer) &&
            (tokenValidationParameters.ValidIssuers?.Any() != true)) ||
            (tokenValidationParameters.IssuerSigningKeys?.Any() != true))
            {
                var baseConfiguration = await
                tokenValidationParameters.ConfigurationManager.GetBaseConfigurationAsync(
                CancellationToken.None);
                tokenValidationParameters.ValidIssuer = baseConfiguration.Issuer;
                tokenValidationParameters.IssuerSigningKeys = baseConfiguration.SigningKeys;
            }
            return await base.ValidateTokenAsync(token, tokenValidationParameters);
        }
    }

    public static async Task Logout(HttpContext httpContext, IConfiguration Configuration)
    {
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        switch (Configuration["identity:type"])
        {
            case ("sefazidentity"):
                await httpContext.SignOutAsync(WsFederationDefaults.AuthenticationScheme);
                break;
            default:
                await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                break;
        }
        Logoff = true;
    }

    private static async Task<Task<int>> OnSecurityTokenReceived(SecurityTokenReceivedContext arg)
    {
        TokenWSClient tokenWS = new TokenWSClient(TokenWSClient.EndpointConfiguration.TokenWS, configuration["sefazidentity:tokenws"]);
        try
        {
            if (await tokenWS.IsTokenValidAsync(arg.ProtocolMessage.GetToken(), configuration["sefazidentity:realm"], "00031C33"))
            {
                return Task.FromResult(0);
            }
        }
        finally
        {
            #region Close_or_Abort
            if (tokenWS != null)
            {
                try
                {
                    await tokenWS.CloseAsync();
                }
                catch (Exception)
                {
                    tokenWS.Abort();
                }
            }
            #endregion
        }
        throw new Exception($"Token recebido é inválido ou não foi emitido para '{configuration["sefazidentity:realm"]}'.");
    }

    public static Task OnRedirectToIdentityProvider(Microsoft.AspNetCore.Authentication.WsFederation.RedirectContext arg)
    {
        arg.ProtocolMessage.Wauth = configuration["sefazidentity:Wauth"];
        arg.ProtocolMessage.Wfresh = configuration["sefazidentity:timeout"];
        arg.ProtocolMessage.Parameters.Add("ClaimSets", "80000000");
        arg.ProtocolMessage.Parameters.Add("TipoLogin", "00031C33");
        arg.ProtocolMessage.Parameters.Add("AutoLogin", "0");
        arg.ProtocolMessage.Parameters.Add("Layout", "2");
        return Task.FromResult(0);
    }

    public static Task OnAuthenticationFailed(RemoteFailureContext context)
    {
        context.HandleResponse();
        context.Response.Redirect("/?errormessage=" + context.Failure.Message);
        return Task.FromResult(0);
    }

    #region

    #endregion
}
