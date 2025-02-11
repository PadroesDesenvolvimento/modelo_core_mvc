using modelo_core_mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using modelo_core_mvc.HttpClients;

namespace modelo_core_mvc.Controllers;

[Authorize]
public class FormularioController : BaseController
{
    private readonly FormularioApiClient api;

    public FormularioController(FormularioApiClient Api)
    {
        api = Api;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        ViewData["Title"] = "Formulário – Manutenção de Dados";
        try
        {
            return View(await api.GetFormularioAsync());
        }
        catch (Exception ex)
        {
            ViewData["Erro"] = "Falha ao listar os registros: " + ex.Message;
            return View();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> Detalhes(long id)
    {
        ViewData["Title"] = "Detalhes";
        ViewData["Message"] = "";
        ViewData["url"] = Url.Action("Alterar", "Formulario", new { id });
        return View(await api.GetProjetoAsync(id));
    }

    [HttpGet]
    public ActionResult Adicionar()
    {
        ViewData["Title"] = "Adicionar";
        ViewData["Message"] = "Incluir novo registro";
        return View(new FormularioModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Adicionar(FormularioModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await api.PostProjetoAsync(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["Title"] = "Novo registro";
                ViewData["Message"] = "Incluir novo registro";
                ViewData["Erro"] = "Falha ao criar o registro: " + ex.Message;              

                return View(model);
            }
        }
        return BadRequest();
    }

    [HttpGet]
    public async Task<ActionResult> Alterar(long id)
    {
        ViewData["Title"] = "Alterar";
        ViewData["Message"] = "Editar informações do projeto";
        var model = await api.GetProjetoAsync(id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Alterar(FormularioModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await api.PutProjetoAsync(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = "Falha ao alterar o registro: " + ex.Message;
                return View(model);
            }
        }
        return BadRequest();
    }

    [HttpGet]
    public async Task<ActionResult> Excluir(long id)
    {
        var model = await api.GetProjetoAsync(id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Excluir(FormularioModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await api.DeleteProjetoAsync(model.id);
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            model = await api.GetProjetoAsync(model.id);
            ViewData["Erro"] = "Falha ao excluir o registro: " + ex.Message;
            return View(model);
        }
    }
}
