using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using SefazLib.IdentityCfg;
using SefazLib.AzureUtils;
using SefazLib.usuarios;
using modelo_core_mvc.ProjetosApi;
using modelo_core_mvc.Errors;
using Antlr4.Runtime;
using Microsoft.IdentityModel.Tokens;

namespace modelo_core_mvc.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly ProjetosApiClient api;
        private readonly AzureUtil azureUtil;

        //Insercao de vulnerabilidades para teste de análise de código
        string username = "teste";
        string password = "123@teste";
        private readonly string[] whiteList = { "https://ads.intra.fazenda.sp.gov.br/tfs" };

        public IActionResult RedirectMe(string url)
        {
            return Redirect(url);
        }
        //Fim do teste

        public HomeController(IConfiguration Configuration, ProjetosApiClient Api, AzureUtil AzureUtil)
        {
            configuration = Configuration;
            api = Api;
            azureUtil = AzureUtil;
        }

        [Authorize]
        public async Task<IActionResult> Entrar()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["Title"] = "Identificação";
            }
            else
            {
                ViewData["Title"] = "Entrar";
            }
            ViewData["Nome"] = "";
            ViewData["token"] = "";
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
                    if (claim.Type.Contains("upn"))              { ViewData["Login"]      = claim.Value; }
                    else if (claim.Type.Contains("givenname"))   { ViewData["Nome"]       = claim.Value; }
                    else if (claim.Type.Contains("preferred_username")) { ViewData["Cpf"] = claim.Value; }
                    else if (claim.Type.Contains("name") && string.IsNullOrEmpty(ViewData["Nome"].ToString()))   { ViewData["Nome"]       = claim.Value; }
                    else if (claim.Type.Contains("id_token"))    { ViewData["token"]      = claim.Value; }
                    else if (claim.Type.Contains("CPF"))         { ViewData["Cpf"]        = claim.Value; }
                    else if (claim.Type.Contains("dateofbirth")) { ViewData["Nascimento"] = claim.Value; }
                }
            }

            return View();
        }
        public IActionResult TesteIdentity()
        {
            ViewData["Title"] = "Teste do Identity";
            ViewData["jwt"] = azureUtil.jwtToken;
            return View();
        }

        public IActionResult Index()
        {
            @ViewData["Title"] = "Sefaz-SP";
            return View();
        }

        public IActionResult Privacidade()
        {
            ViewData["Title"] = "Privacidade";
            return View();
        }

        public IActionResult Contato()
        {
            ViewData["Title"] = "Contato";
            return View();
        }

        public async Task<ActionResult> Sobre()
        {
            ViewData["Title"] = "Sobre";
            ViewData["Message"] = "Sobre essa aplicação";
            ViewData["status"] = await api.GetStatusAsync();
            ViewData["conexao"] = await api.GetConexaoAsync();
            ViewData["EnderecoAPI"] = configuration["apiendereco:projetos"];

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
