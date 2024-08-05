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

namespace SefazLib.IdentityCfg
{
    public class IdentityConfig
    {
        private static IConfiguration configuration;
        public Action<WsFederationOptions> WSFederationOptions { get; private set; }
        public Action<CookieAuthenticationOptions> CookieAuthenticationOptions { get; private set; }
        public Action<Microsoft.AspNetCore.Authentication.AuthenticationOptions> AuthenticationOptions { get; private set; }
        public Action<OpenIdConnectOptions> OpenIdConnectOptions { get; private set; }
        public static Boolean Logoff { get; private set; }
        public HttpClient httpClient;
        public static string jwtToken;
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
                options.Wtrealm = configuration["identity:realm"];
                options.MetadataAddress = configuration["identity:metadataaddress"];

                if (Configuration["identity:type"] == "sefazidentity")
                {
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
                }
            };

            if (Configuration["identity:type"] == "loginsefaz")
            {
                OpenIdConnectOptions = options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = Configuration.GetSection("LoginSefaz")["ServerRealm"];
                    options.ClientId = Configuration.GetSection("LoginSefaz")["ClientId"];
                    options.ClientSecret = Configuration.GetSection("LoginSefaz")["ClientSecret"];
                    options.MetadataAddress = Configuration.GetSection("LoginSefaz")["Metadata"];
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
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                };
            }

            CookieAuthenticationOptions = options =>
            {
                options.ExpireTimeSpan = new TimeSpan(0, 0, int.Parse(configuration["identity:timeout"]), 0);
                options.SlidingExpiration = false;
            };
        }

        public AuthenticationHeaderValue AuthenticationHeader()
        {
            if (configuration["identity:type"] == "loginsefaz")
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
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
            TokenWSClient tokenWS = new TokenWSClient(TokenWSClient.EndpointConfiguration.TokenWS, configuration["identity:tokenws"]);
            try
            {
                if (await tokenWS.IsTokenValidAsync(arg.ProtocolMessage.GetToken(), configuration["identity:realm"], "00031C33"))
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
            throw new Exception($"Token recebido é inválido ou não foi emitido para '{configuration["identity:realm"]}'.");
        }

        public static Task OnRedirectToIdentityProvider(Microsoft.AspNetCore.Authentication.WsFederation.RedirectContext arg)
        {
            arg.ProtocolMessage.Wauth = configuration["identity:Wauth"];
            arg.ProtocolMessage.Wfresh = configuration["identity:timeout"];
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
}
