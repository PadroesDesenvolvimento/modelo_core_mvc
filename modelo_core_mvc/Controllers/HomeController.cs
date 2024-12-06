using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using modelo_core_mvc.ProjetosApi;
using modelo_core_mvc.Errors;

namespace modelo_core_mvc.Controllers;

public class HomeController : BaseController
{
    private readonly IConfiguration configuration;
    private readonly ProjetosApiClient api;

    public HomeController(IConfiguration Configuration, ProjetosApiClient Api)
    {
        configuration = Configuration;
        api = Api;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Sefaz-SP";
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
