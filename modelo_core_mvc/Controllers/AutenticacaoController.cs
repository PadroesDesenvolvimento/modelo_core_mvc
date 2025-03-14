﻿using Microsoft.AspNetCore.Authentication;
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
using SefazLib;
using System.Xml;

namespace modelo_core_mvc.Controllers;

public class AutenticacaoController : BaseController
{
    private readonly IConfiguration configuration;
    private readonly ILogger<HomeController> _logger;

    public AutenticacaoController(IConfiguration configuration, ILogger<HomeController> logger)
    {
        this.configuration = configuration;
        _logger = logger;
    }

    [Authorize]
    public async Task<IActionResult> Entrar()
    {
        ViewData["Title"] = "Entrar";
        var token = "";
        if (User.Identity.IsAuthenticated)
        {
            ViewData["Title"] = "Identificação";
        }
        else
        {
            ViewData["Title"] = "Entrar";
        }
        ViewData["Nome"] = "";
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
        if (String.IsNullOrEmpty(token))
        {
            token = await AuthenticationAsync();
        }

        ViewData["token"] = token;

        IdentityConfig.jwtToken = token;

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
        ViewData["Authority"] = "";
        ViewData["Metadata"] = "";
        return View();
    }

    public async Task<string> AuthenticationAsync()
    {
        string accessToken = await HttpContext.GetTokenAsync("access_token");
        string idToken = await HttpContext.GetTokenAsync("id_token");
        string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
        string newAccessToken = "";
        switch (configuration["identity:type"])
        {
            case "loginsefaz":
                var currentUserName = "Não identificado";
                ClaimsPrincipal currentUser = this.User;
                if (currentUser != null)
                {
                    var claim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim != null)
                    {
                        currentUserName = claim.Value;
                        _logger.LogError(currentUserName);
                    }
                }

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
                break;

            case "sefazidentity":
                if (User.Identity.IsAuthenticated)
                {
                    // o token é SAML e precisa ser criado um jwt para ser repassado para o front-end
                    var claims = User.Claims;
                    foreach (var claim in User.Claims)
                    {
                        if (claim.Type.Contains("id_token")) { newAccessToken = claim.Value; }
                    }

                    if (newAccessToken == "")
                    {
                        var xml = new XmlDocument();
                        var bootstapContext = (string)User.Identities.First().BootstrapContext;
                        xml.LoadXml(bootstapContext);
                        var assertionNode = xml.SelectSingleNode("//*[local-name()='Assertion']");

                        if (assertionNode != null)
                        {
                            string samlToken = assertionNode.OuterXml;
                            var jwtToken = IdentityConfig.ConvertSamlToJwt(samlToken, configuration);
                            newAccessToken = jwtToken;
                        }
                    }
                }
                break;
        }

        return newAccessToken;
    }

}
