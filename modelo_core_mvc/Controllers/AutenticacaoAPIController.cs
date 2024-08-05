using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SefazLib.IdentityCfg;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace exemplo_autenticador_bff.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AutenticacaoAPIController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<AutenticacaoAPIController> logger;
        private readonly IdentityConfig identityConfig;

        public AutenticacaoAPIController(IConfiguration Configuration, ILogger<AutenticacaoAPIController> Logger, IdentityConfig IdentityConfig)
        {
            configuration = Configuration;
            logger = Logger;
            identityConfig = IdentityConfig;
        }

        [Authorize]
        [HttpGet("token")]
        public IActionResult ObterToken(string returnUrl)
        {
            var token = "";

            if (User.Identity.IsAuthenticated)
            {
                switch (configuration["identity:type"])
                {
                    case "sefazidentity":
                        // o token é SAML e precisa ser criado um jwt para ser repassado para o front-end
                        var claims = User.Claims;
                        foreach (var claim in User.Claims)
                        {
                            if (claim.Type.Contains("id_token")) { token = claim.Value; }
                        }

                        if (token == "")
                        {
                            var xml = new XmlDocument();
                            var bootstapContext = (string)User.Identities.First().BootstrapContext;
                            xml.LoadXml(bootstapContext);
                            var assertionNode = xml.SelectSingleNode("//*[local-name()='Assertion']");

                            if (assertionNode != null)
                            {
                                string samlToken = assertionNode.OuterXml;
                                var jwtToken = ConvertSamlToJwt(samlToken, configuration);
                                token = jwtToken;
                            }
                        }
                        break;
                }
            }
            else { return Unauthorized(); }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                var uriBuilder = new UriBuilder(returnUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = token;
                uriBuilder.Query = query.ToString();
                return Redirect(uriBuilder.ToString());
            }

            var chavePublica = ConvertXmlToJwk(configuration["identity:PublicKey"]);
            return Ok(new { token });
        }

        private string ConvertSamlToJwt(string samlToken, IConfiguration configuration)
        {
            var issuer = configuration["identity:metadataaddress"];
            var audience = configuration["identity:realm"];
            var privateKeyXml = configuration["identity:PrivateKey"];
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKeyXml);
            var key = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            var samlTokenXml = new XmlDocument();
            samlTokenXml.LoadXml(samlToken);

            var claims = new ClaimsIdentity();
            var samlAssertionNode = samlTokenXml.SelectSingleNode("//*[local-name()='Assertion']");
            var samlAttributeNodes = samlAssertionNode.SelectNodes("//*[local-name()='Attribute']");

            foreach (XmlNode attributeNode in samlAttributeNodes)
            {
                var attributeName = attributeNode.Attributes["Name"].Value;
                var attributeValue = attributeNode.FirstChild.InnerText;
                claims.AddClaim(new Claim(attributeName, attributeValue));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Issuer = configuration["identity:issuer"],
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = credentials
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(securityToken);

            return jwtToken;
        }

        private bool IsValidToken(string jwtToken)
        {
            try
            {
                var xmlPublicKey = configuration["identity:PublicKey"];
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPublicKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("Erro ao validar o token: {Message}", ex.Message);
                return false;
            }
        }

        [HttpGet("validar")]
        public IActionResult ValidarToken()
        {
            // Obter o token do cabeçalho Authorization
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return BadRequest("Token JWT não encontrado no cabeçalho.");
            }

            // O cabeçalho Authorization tem o formato "Bearer {token}"
            string[] tokenParts = authorizationHeader.Split(' ');

            if (tokenParts.Length != 2 || !tokenParts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Formato inválido do cabeçalho Authorization. Use 'Bearer {token}'.");
            }

            string jwtToken = tokenParts[1];

            try
            {
                if (IsValidToken(jwtToken))
                {
                    return Ok("Token JWT válido.");
                }
                else
                {
                    return Unauthorized("Token JWT inválido.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao validar o token JWT: {ex.Message}");
            }
        }

        [HttpGet(".well-known/openid-configuration")]
        public IActionResult GetOidcConfiguration()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/AutenticacaoAPI";
            var oidcConfig = new
            {
                issuer = baseUrl,
                token_endpoint = $"{baseUrl}/token",
                verificacao_endpoint = $"{baseUrl}/validar",
                id_token_signing_alg_values_supported = new[] { "RS256" },
                chave_publica = ConvertXmlToJwk(configuration["identity:PublicKey"]),
            };

            return Ok(oidcConfig);
        }

        private object ConvertXmlToJwk(string xmlKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlKey);
            var parameters = rsa.ExportParameters(false);

            var modulus = Convert.ToBase64String(parameters.Modulus)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            var exponent = Convert.ToBase64String(parameters.Exponent)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            var jwk = new
            {
                kty = "RSA",
                n = modulus,
                e = exponent
            };

            return jwk;
        }
    }
}
