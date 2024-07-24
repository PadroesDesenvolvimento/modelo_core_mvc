using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SefazLib.IdentityCfg;
using System.Linq;
using System.Xml;
using SefazLib.SamlToJwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Web;

namespace exemplo_autenticador_bff.Controllers;

[Route("[controller]")]
[ApiController]
public class AutenticacaoAPIController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly ILogger<AutenticacaoAPIController> logger;
    private readonly IdentityConfig identityConfig;

    public AutenticacaoAPIController(IConfiguration Configuration,
                                  ILogger<AutenticacaoAPIController> Logger,
                                  IdentityConfig IdentityConfig)
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
                    var innerXml = xml.InnerXml;
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

    [HttpGet("teste")]
    public async Task<IActionResult> Teste_chamada()
    {
        var token = "Ok";
        return Ok(new { token });
    }
    
    [HttpGet("validar-token")]
    [Authorize]
    public IActionResult ValidarToken()
    {
        // Obter o token do cabeçalho Authorization
        string authorizationHeader = HttpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return BadRequest("Token JWT não encontrado no cabeçalho Authorization.");
        }

        // O cabeçalho Authorization tem o formato "Bearer {token}", onde {token} é o token JWT
        string[] tokenParts = authorizationHeader.Split(' ');

        if (tokenParts.Length != 2 || !tokenParts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Formato inválido do cabeçalho Authorization. Use 'Bearer {token}'.");
        }

        string jwtToken = tokenParts[1];

        try
        {
            // Aqui você pode adicionar a lógica para validar o token JWT
            // Pode ser uma verificação de assinatura, expiração, reivindicações, etc.
            // Se o token for válido, retorne OK, caso contrário, retorne Unauthorized

            // Exemplo de validação simples
            if (IsValidToken(jwtToken, ""))
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

    private bool IsValidToken(string jwtToken, string publicKey)
    {
        try
        {
            // Configurar o validador de tokens JWT
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false, // Configure de acordo com suas necessidades
                ValidateAudience = false, // Configure de acordo com suas necessidades
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(publicKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            // Tente validar o token JWT
            var claimsPrincipal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);

            // Se a validação não lançou exceções, o token é válido
            return true;
        }
        catch (Exception ex)
        {
            // Trate exceções de validação aqui, se necessário
            // Por exemplo, TokenExpiredException, SecurityTokenValidationException, etc.
            Console.WriteLine(ex.Message);
            return false;
        }
    }

}
