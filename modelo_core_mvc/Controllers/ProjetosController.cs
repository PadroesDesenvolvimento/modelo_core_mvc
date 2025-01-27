﻿using modelo_core_mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using modelo_core_mvc.ProjetosApi;
using System;

namespace modelo_core_mvc.Controllers;

[Authorize]
public class ProjetosController : BaseController
{
    private readonly ProjetosApiClient api;

    public ProjetosController(ProjetosApiClient Api)
    {
        api = Api;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        try
        {
            return View(await api.GetProjetosAsync());
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
        ViewData["Message"] = "";
        ViewData["url"] = Url.Action("Alterar", "Projetos", new { id });
        return View(await api.GetProjetoAsync(id));
    }

    [HttpGet]
    public ActionResult Adicionar()
    {
        ViewData["Message"] = "Incluir novo projeto";
        return View(new ProjetosModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Adicionar(ProjetosModel model)
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
                ViewData["Title"] = "Novo Projeto";
                ViewData["Message"] = "Incluir novo projeto";
                ViewData["Erro"] = "Falha ao criar o registro: " + ex.Message;              

                return View(model);
            }
        }
        return BadRequest();
    }

    [HttpGet]
    public async Task<ActionResult> Alterar(long id)
    {
        ViewData["Message"] = "Editar informações do projeto";
        var model = await api.GetProjetoAsync(id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Alterar(ProjetosModel model)
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
        ViewData["Message"] = "Exclusão do projeto";
        var model = await api.GetProjetoAsync(id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Excluir(ProjetosModel model)
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
            ViewData["Message"] = "Exclusão do projeto"; 
            model = await api.GetProjetoAsync(model.id);
            ViewData["Erro"] = "Falha ao excluir o registro: " + ex.Message;
            return View(model);
        }
    }
}
