using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using SefazLib.IdentityCfg;
using modelo_core_mvc.ProjetosApi;
using modelo_core_mvc.Errors;

namespace modelo_core_mvc.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly ProjetosApiClient api;

        #region Insercao de vulnerabilidades para teste de análise de código
        string username = "teste";
        string password = "123@teste";
        private readonly string[] whiteList = { "https://ads.intra.fazenda.sp.gov.br/tfs" };

        public IActionResult RedirectMe(string url)
        {
            return Redirect(url);
        }

        #endregion //Fim do teste

        public HomeController(IConfiguration Configuration, ProjetosApiClient Api)
        {
            configuration = Configuration;
            api = Api;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
