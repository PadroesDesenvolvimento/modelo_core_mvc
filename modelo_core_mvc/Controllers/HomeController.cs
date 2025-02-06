using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using modelo_core_mvc.Errors;
using modelo_core_mvc.HttpClients;

namespace modelo_core_mvc.Controllers;

public class HomeController : BaseController
{
    private readonly IConfiguration configuration;
    private readonly FormularioApiClient api;

    public HomeController(IConfiguration Configuration, FormularioApiClient Api)
    {
        configuration = Configuration;
        api = Api;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Página incial";
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
    public IActionResult Acessibilidade()
    {
        ViewData["Title"] = "Acessibilidade";
        return View();
    }

    public async Task<ActionResult> Sobre()
    {
        ViewData["Title"] = "Sobre";
        ViewData["Message"] = "Sobre essa aplicação";
        ViewData["status"] = await api.GetStatusAsync();
        ViewData["conexao"] = await api.GetConexaoAsync();
        ViewData["EnderecoAPI"] = configuration["apiendereco:formulario"];

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
