using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace modelo_core_mvc.Helpers;

[ExcludeFromCodeCoverage]
public class TokenExchangeHelper
{
    static readonly HttpClient client = new HttpClient();
    private readonly IConfiguration configuration;

    public TokenExchangeHelper(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<string> GetRefreshTokenAsync(string refreshToken)
    {
        if (configuration["identity:type"] == "loginsefaz")
        {
            /*
             * Get refresh token
             * Uses the settings injected from startup to read the configuration
             */
            try
            {
                string grant_type = "refresh_token";
                string url = "";
                string client_id = "";
                string client_secret = "";
                string token = refreshToken;

                url = configuration["loginsefaz:TokenExchange"];
                client_id = configuration["loginsefaz:ClientId"];
                client_secret = configuration["loginsefaz:ClientSecret"];

                var form = new Dictionary<string, string>
            {
                {"grant_type", grant_type},
                {"client_id", client_id},
                {"client_secret", client_secret},
                {"refresh_token", token }
            };

                HttpResponseMessage tokenResponse = await client.PostAsync(url, new FormUrlEncodedContent(form));
                var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
                Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);
                return tok.AccessToken;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        else 
        { 
            return null; 
        }
    }

    public async Task<string> GetTokenExchangeAsync(string accessToken)
    {
        /*
         * Get exchange token
         * ses the settings injected from startup to read the configuration
         */
        try
        {
            string url = configuration["Keycloak:TokenExchange"];
            //Important, the grant types for token exchange, must be set to this!
            string grant_type = "urn:ietf:params:oauth:grant-type:token-exchange";
            string client_id = configuration["Keycloak:ClientId"];
            string client_secret = configuration["Keycloak:ClientSecret"];
            string audience = configuration["Keycloak:Audience"];
            string token = accessToken;

            var form = new Dictionary<string, string>
            {
                {"grant_type", grant_type},
                {"client_id", client_id},
                {"client_secret", client_secret},
                {"audience", audience},
                {"subject_token", token }
            };

            HttpResponseMessage tokenResponse = await client.PostAsync(url, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);
            return tok.AccessToken;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    internal class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
