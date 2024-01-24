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
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using Azure.Identity;
using Azure.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using TokenWS;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens.Saml;
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
        private readonly ITokenAcquisition tokenAcquisition;

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
                    case ("azuread"):
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                        break;
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
                options.TokenHandlers.Clear();
                options.TokenHandlers.Add(new CustomSamlSecurityTokenHandler());
                options.Wtrealm = configuration["identity:realm"];
                options.MetadataAddress = configuration["identity:metadataaddress"];

                if (Configuration["identity:type"] == "sefazidentity")
                {
                    options.Wreply = configuration["identity:reply"];
                    options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;
                    options.Events.OnSecurityTokenReceived = OnSecurityTokenReceived;
                    options.TokenValidationParameters = new TokenValidationParameters { SaveSigninToken = true };
                    options.CorrelationCookie = new CookieBuilder
                    {
                        Name = ".Correlation.",
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None,
                        SecurePolicy = CookieSecurePolicy.Always,
                        Expiration = new TimeSpan(0, 0, 15, 0),
                        MaxAge = new TimeSpan(0, 0, 15, 0)
                    };
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
                options.Cookie = new CookieBuilder
                {
                    Name = "FedAuth",
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    SecurePolicy = CookieSecurePolicy.Always
                };
                options.ExpireTimeSpan = new TimeSpan(0, 0, int.Parse(configuration["identity:timeout"]), 0);
                options.SlidingExpiration = false;
            };
        }

        public async Task<AuthenticationHeaderValue> AuthenticationHeader()
        {
            if (configuration["identity:type"] == "azuread")
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await obterAccessToken(null));
            }
            if (configuration["identity:type"] == "loginsefaz")
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }

            return httpClient.DefaultRequestHeaders.Authorization;
        }

        public class CustomSamlSecurityTokenHandler : SamlSecurityTokenHandler
        {
            public override async Task<TokenValidationResult> ValidateTokenAsync(string token, TokenValidationParameters _validationParameters)
            {
                var validationParameters = _validationParameters.Clone();
                var configuration = await validationParameters.ConfigurationManager.GetBaseConfigurationAsync(CancellationToken.None).ConfigureAwait(false);
                var issuers = new[] { configuration.Issuer };
                validationParameters.ValidIssuers = (validationParameters.ValidIssuers == null ? issuers : validationParameters.ValidIssuers.Concat(issuers));
                validationParameters.IssuerSigningKeys = (validationParameters.IssuerSigningKeys == null ? configuration.SigningKeys : validationParameters.IssuerSigningKeys.Concat(configuration.SigningKeys));

                return await base.ValidateTokenAsync(token, validationParameters);
            }
        }

        #region Azure AD
        public IdentityConfig(IConfiguration Configuration, ITokenAcquisition TokenAcquisition)
        {
            tokenAcquisition = TokenAcquisition;
            httpClient = new HttpClient();
            configuration = Configuration;
            Logoff = false;

            AuthenticationOptions = options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            };

            WSFederationOptions = options =>
            {
                options.Wtrealm = configuration["identity:realm"];
                options.MetadataAddress = configuration["identity:metadataaddress"];
            };

        }

        public void SetScope(string callApi)
        {
            scopes = configuration.GetValue<string>("CallApi:" + callApi)?.Split(' ').ToArray();
        }

        public async Task<string> obterAccessToken(ClientSecretCredential clientSecretCredential)
        {
            try
            {
                if (clientSecretCredential is null)
                {
                    if (scopes is null)
                    {
                        SetScope("ScopeForAccessToken");
                    }
                    jwtToken = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);

                }
                else
                {
                    jwtToken = clientSecretCredential!.GetTokenAsync(new TokenRequestContext(scopes)).Result.Token;
                }
                tokenInfo = GetTokenInfo(jwtToken);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }
            return jwtToken;
        }
        protected Dictionary<string, string> GetTokenInfo(string token)
        {
            var TokenInfo = new Dictionary<string, string>();

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var claims = jwtSecurityToken.Claims.ToList();

            foreach (var claim in claims)
            {
                if (!TokenInfo.ContainsKey(claim.Type))
                {
                    TokenInfo.Add(claim.Type, claim.Value);
                }
            }

            return TokenInfo;
        }
        #endregion

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
