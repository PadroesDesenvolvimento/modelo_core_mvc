using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using SefazLib.usuarios;
using SefazLib.IdentityCfg;

namespace SefazLib.AzureUtils
{
    public class AzureUtil
    {
        public HttpClient httpClient;
        public string erro;
        public string[] scopes;
        public Dictionary<string, string> tokenInfo;
        private readonly IConfiguration configuration;
        private readonly IdentityConfig identityConfig;
        public string jwtToken; 

        public AzureUtil(IConfiguration Configuration, IdentityConfig IdentityConfig)
        {
            httpClient = new HttpClient();
            configuration = Configuration;
            identityConfig = IdentityConfig;
            jwtToken = IdentityConfig.jwtToken;
        }

        public async Task<Usuario> GetUserAsync()
        {
            if (configuration["identity:type"] == "azuread")
            {
                string fotoUsuario = null;
                Microsoft.Graph.User userAzure;
                try
                {
                    GraphServiceClient graphClientDelegated = ObterGraphClient("");
                    userAzure = await graphClientDelegated.Me
                        .Request()
                        .GetAsync();

                    try
                    {
                        // Get user photo
                        using (var photoStream = await graphClientDelegated.Me.Photo.Content.Request().GetAsync())
                        {
                            byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                            fotoUsuario = Convert.ToBase64String(photoByte);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        fotoUsuario = null;
                        erro = ex.Message;
                    }
                    return new Usuario(userAzure.Id, userAzure.GivenName, userAzure.DisplayName, userAzure.JobTitle, userAzure.Mail, fotoUsuario);
                }
                catch (System.Exception ex)
                {
                    erro = ex.Message;
                    return new Usuario();
                }

            }
            else
            {
                return new Usuario();
            }

        }

        public GraphServiceClient ObterGraphClient(string tipoClient)
        {
            switch (tipoClient)
            {
                case "Application":
                    return new GraphServiceClient(new ClientSecretCredential(configuration["AzureAd:ClientId"], configuration["AzureAd:TenantId"], configuration["AzureAd:ClientSecret"]));

                default:
                    identityConfig.SetScope("MSGraph");
                    var authProvider = new DelegateAuthenticationProvider(async (request) =>
                    {
                        request.Headers.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await identityConfig.obterAccessToken(null));
                    });
                    return new GraphServiceClient(authProvider);
            }
        }
    }
}
