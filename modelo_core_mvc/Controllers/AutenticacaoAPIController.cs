using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SefazLib.IdentityCfg;
using SefazLib.SamlToJwt;
using System;
using System.IdentityModel.Tokens.Jwt;
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
        public async Task<IActionResult> Obter_Token(string returnUrl)
        {
            var token = "";

            if (User.Identity.IsAuthenticated)
            {
                if (configuration["identity:type"] == "azuread")
                {
                    try
                    {
                        token = await identityConfig.obterAccessToken(null);
                    }
                    catch { }
                }
                else
                {
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
                            var jwtToken = SamlToJwt.ConvertSamlToJwt(samlToken, configuration);
                            token = jwtToken;
                        }
                    }
                }
            }
            else
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                var uriBuilder = new UriBuilder(returnUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = token;
                uriBuilder.Query = query.ToString();
                return Redirect(uriBuilder.ToString());
            }

            return Ok(new { token });
        }

        [HttpGet("validar-token")]
        public IActionResult ValidarToken()
        {
            // Obter o token do cabeçalho Authorization
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return BadRequest("Token JWT não encontrado no cabeçalho Authorization.");
            }

            // O cabeçalho Authorization tem o formato "Bearer {token}", onde {token}" é o token JWT
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

        private string SignToken(string token)
        {
            var privateKey = configuration["identity:PrivateKey"];
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new RsaSecurityKey(rsa);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("jwt", token) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            };

            var signedToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(signedToken);
        }

        private bool IsValidToken(string jwtToken)
        {
            var publicKey = configuration["identity:PublicKey"];
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

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

            try
            {
                tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("Erro ao validar o token: {Message}", ex.Message);
                return false;
            }
        }
    }
}
