using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using modelo_core_mvc.Models;
using modelo_core_mvc.ProjetosApi;
using SefazLib.AzureUtils;
using SefazLib.IdentityCfg;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

var services = builder.Services;

services.AddApplicationInsightsTelemetry();

#region Tipos de Autenticacao
IdentityConfig identityConfig = new(Configuration);
var opcoesAutenticacao = identityConfig.AuthenticationOptions;
// Sugestao: exclua os tipos de autenticacao que não forem usados para simplificar o codigo 
// Esse switch e so para exemplificar os diversos tipos possiveis na Sefaz
switch (Configuration["identity:type"])
{
    case "azuread":
        services.AddControllersWithViews().AddMicrosoftIdentityUI();
        string[] initialScopes = Configuration.GetValue<string>("CallApi:ScopeForAccessToken")?.Split(' ').ToArray();
        services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddInMemoryTokenCaches();
        services.AddTransient<ListModel>();
        break;

    case ("wsfed"):
        services.AddControllersWithViews();
        services.AddAuthentication(opcoesAutenticacao)
                .AddWsFederation(identityConfig.WSFederationOptions)
                .AddCookie();
        break;

    case ("loginsefaz"):
        services.AddControllersWithViews();
        services.AddAuthentication(opcoesAutenticacao)
                .AddCookie(cookie =>
                {
                    cookie.Cookie.Name = "keycloak.cookie";
                    cookie.Cookie.MaxAge = TimeSpan.FromMinutes(60);
                    cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    cookie.SlidingExpiration = true;
                })
                .AddOpenIdConnect(identityConfig.OpenIdConnectOptions);
        break;

    case ("openid"):
        services.AddControllersWithViews();
        services.AddAuthentication(opcoesAutenticacao)
                .AddOpenIdConnect(identityConfig.OpenIdConnectOptions)
                .AddCookie();
        break;

    default:
        services.AddControllersWithViews();
        services.AddAuthentication(opcoesAutenticacao)
                .AddWsFederation(identityConfig.WSFederationOptions)
                .AddCookie("Cookies", identityConfig.CookieAuthenticationOptions);
        break;
}
#endregion

services.AddSingleton(async provider =>
{
    var authenticationResult = await provider.GetRequiredService<IHttpContextAccessor>().HttpContext.AuthenticateAsync();
    var accessToken = authenticationResult.Properties.Items.FirstOrDefault(prop => prop.Key == "Token.access_token").Value;

    return accessToken;
});
services.AddTransient<IdentityConfig>();
services.AddHttpClient<ProjetosApiClient>();
services.AddTransient<AzureUtil>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();