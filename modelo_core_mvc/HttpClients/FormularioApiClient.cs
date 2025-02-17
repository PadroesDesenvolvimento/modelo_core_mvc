using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using modelo_core_mvc.Models;
using SefazLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace modelo_core_mvc.HttpClients;

public class FormularioApiClient
{
    private readonly IConfiguration configuration;
    private readonly IdentityConfig identityConfig;

    [ExcludeFromCodeCoverage]
    public HttpClient httpClient { get; set; }

    [ExcludeFromCodeCoverage]
    public FormularioApiClient(HttpClient HttpClient, IConfiguration Configuration, IdentityConfig IdentityConfig)
    {
        configuration = Configuration;
        identityConfig = IdentityConfig;
        httpClient = HttpClient;
        httpClient.DefaultRequestHeaders.Authorization = identityConfig.AuthenticationHeader();
        httpClient.BaseAddress = new Uri(configuration["apiendereco:formulario"]);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    //Consultar
    public async Task<FormularioModel> GetProjetoAsync(long id)
    {
        var resposta = await httpClient.GetAsync($"Projetos/{id}");
        resposta.EnsureSuccessStatusCode();
        return new FormularioModel().ToModel(await resposta.Content.ReadAsStringAsync());
    }

    //Listar 
    public async Task<IEnumerable<FormularioModel>> GetFormularioAsync(int? numReg, int? pagNum, string colName)
    {
        var query = $"Projetos?numReg={numReg}&pagNum={pagNum}&colName={colName}";
        var resposta = await httpClient.GetAsync(query);
        resposta.EnsureSuccessStatusCode();
        return new FormularioModel().ToList(await resposta.Content.ReadAsStringAsync());
    }

    //Total de registros
    public async Task<int> GetTotalRegistrosAsync()
    {
        var resposta = await httpClient.GetAsync("Projetos/numreg");
        resposta.EnsureSuccessStatusCode();
        return int.Parse(await resposta.Content.ReadAsStringAsync());
    }

    //Verificar api
    public async Task<string> GetStatusAsync()
    {
        try
        {
            var resposta = await httpClient.GetAsync($"projetos/status");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    //Verificar conexão
    public async Task<string> GetConexaoAsync()
    {
        try
        {
            var resposta = await httpClient.GetAsync($"projetos/conexao");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task DeleteProjetoAsync(long id)
    {
        if (id != 0)
        {
            var resposta = await httpClient.DeleteAsync($"Projetos/{id}");
            resposta.EnsureSuccessStatusCode();
        }
    }

    //Incluir
    public async Task PostProjetoAsync(FormularioModel projeto)
    {
        var resposta = await httpClient.PostAsync("Projetos", projeto.ToJson());
        resposta.EnsureSuccessStatusCode();
    }

    //Alterar
    public async Task PutProjetoAsync(FormularioModel projeto)
    {
        var id = projeto.id;
        var resposta = await httpClient.PutAsync($"Projetos/{id}", projeto.ToJson());
        resposta.EnsureSuccessStatusCode();
    }

    public async Task<byte[]> GetAnexoAsync(long id)
    {
        var resposta = await httpClient.GetAsync($"Projetos/{id}/anexo");
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadAsByteArrayAsync();
    }
}
