using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.Configuration;
using modelo_core_mvc.Helpers;
using SefazLib.AzureUtils;
using SefazLib.usuarios;
using SefazLib.IdentityCfg;
using SefazLib.UrlTest;

namespace modelo_core_mvc.Controllers;

public class AutenticacaoController : BaseController
{
    private readonly IConfiguration configuration;
    private readonly ILogger<HomeController> _logger;
    private readonly AzureUtil azureUtil;

    public AutenticacaoController(IConfiguration configuration, ILogger<HomeController> logger, AzureUtil azureUtil)
    {
        this.configuration = configuration;
        _logger = logger;
        this.azureUtil = azureUtil;
    }

    [Authorize]
    public IActionResult Entrar()
    {
        return View();
    }

    public async Task<IActionResult> Identificacao()
    {
        var token = "";
        if (User.Identity.IsAuthenticated)
        {
            ViewData["Title"] = "Identificação";
            ViewData["Nome"] = "";
            if (configuration["identity:type"] == "azuread")
            {
                Usuario usuario = await azureUtil.GetUserAsync();
                ViewData["html"] = usuario.GetAdaptiveCard().Html;
                ViewData["id"] = usuario.id;
            }
            else
            {
                var claims = User.Claims;
                foreach (var claim in User.Claims)
                {
                    if (claim.Type.Contains("upn")) { ViewData["Login"] = claim.Value; }
                    else if (claim.Type.Contains("givenname")) { ViewData["Nome"] = claim.Value; }
                    else if (claim.Type.Contains("preferred_username")) { ViewData["Cpf"] = claim.Value; }
                    else if (claim.Type.Contains("name") && string.IsNullOrEmpty(ViewData["Nome"].ToString())) { ViewData["Nome"] = claim.Value; }
                    else if (claim.Type.Contains("id_token")) { token = claim.Value; }
                    else if (claim.Type.Contains("CPF")) { ViewData["Cpf"] = claim.Value; }
                    else if (claim.Type.Contains("dateofbirth")) { ViewData["Nascimento"] = claim.Value; }
                }
            }
            if (String.IsNullOrEmpty(token))
            {
                ViewData["token"] = await AuthenticationAsync();
            }

            ViewData["token"] = token;

        }
        else
        {
            ViewData["Title"] = "Não autenticado";
        }

        return View();
    }

    [Authorize]
    public async Task<IActionResult> SairAsync()
    {
        ViewData["Title"] = "Sair";
        ViewData["Message"] = "Encerrar a sessão";
        await IdentityConfig.Logout(HttpContext, configuration);

        return View();
    }

    public IActionResult TesteIdentity()
    {
        ViewData["Title"] = "Teste do Identity";
        ViewData["jwt"] = azureUtil.jwtToken;
        ViewData["Authority"] = "";
        ViewData["Metadata"] = "";

        if (configuration["identity:type"] == "loginsefaz")
        {
            var testeURL = new UrlTest();
            ViewData["Authority"] = testeURL.Verificar_URL("Authority", configuration["LoginSefaz:ServerRealm"]);
            ViewData["Metadata"] = testeURL.Verificar_URL("Metadata", configuration["LoginSefaz:Metadata"]);
        }

        return View();
    }

    public async Task<string> AuthenticationAsync()
    {
        ClaimsPrincipal currentUser = this.User;
        var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        _logger.LogError(currentUserName);

        string accessToken = await HttpContext.GetTokenAsync("access_token");
        string idToken = await HttpContext.GetTokenAsync("id_token");
        string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
        string newAccessToken = "";
        if (configuration["identity:type"] == "loginsefaz")
        {
            TokenExchangeHelper exchange = new TokenExchangeHelper(configuration);
            newAccessToken = await exchange.GetRefreshTokenAsync(refreshToken);
            var serviceAccessToken = await exchange.GetTokenExchangeAsync(newAccessToken);
            IEnumerable<Claim> roleClaims = User.FindAll(ClaimTypes.Role);
            IEnumerable<string> roles = roleClaims.Select(r => r.Value);
            foreach (var role in roles)
            {
                _logger.LogError(role);
            }
            var currentClaims = currentUser.FindAll(ClaimTypes.Role).ToList();
            foreach (var claim in currentClaims)
            {
                _logger.LogError(claim.ToString());
            }
        }

        return newAccessToken;
    }

}
