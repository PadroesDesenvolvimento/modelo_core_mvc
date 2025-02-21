using modelo_core_mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using modelo_core_mvc.HttpClients;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;

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
    public async Task<ActionResult> Index(int? numReg, int? pagNum, string colName, string sortOrder)
    {
        ViewData["Title"] = "Formulário – Manutenção de Dados";

        numReg = numReg ?? int.Parse(Request.Cookies["numReg"] ?? "5");
        pagNum = pagNum ?? int.Parse(Request.Cookies["pagNum"] ?? "1");
        colName = colName ?? Request.Cookies["colName"];
        sortOrder = sortOrder ?? Request.Cookies["sortOrder"];

        // Salvar os valores atuais nos cookies
        Response.Cookies.Append("numReg", numReg.ToString(), new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
        Response.Cookies.Append("pagNum", pagNum.ToString(), new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
        Response.Cookies.Append("colName", colName ?? "", new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
        Response.Cookies.Append("sortOrder", sortOrder ?? "", new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });

        try
        {
            var totalRegistros = await api.GetTotalRegistrosAsync();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / (double)numReg);
            if (pagNum > totalPaginas)
            {
                pagNum = totalPaginas > 0 ? totalPaginas : 1;
                Response.Cookies.Append("pagNum", pagNum.ToString(), new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
            }
            var projetos = await api.GetFormularioAsync(numReg, pagNum, colName, sortOrder);

            ViewData["numReg"] = numReg;
            ViewData["pagNum"] = pagNum;
            ViewData["colName"] = colName;
            ViewData["sortOrder"] = sortOrder;
            ViewData["totalRegistros"] = totalRegistros;

            return View(projetos);
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
