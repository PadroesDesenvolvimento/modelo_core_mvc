using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using modelo_core_mvc.projetos;
using SefazLib;
using System;

namespace modelo_core_mvc.ProjetosApi;

public class ProjetosApiClient
{
    private readonly IConfiguration configuration;
    private readonly IdentityConfig identityConfig;

    public HttpClient httpClient { get; set; }

    public ProjetosApiClient(HttpClient HttpClient, IConfiguration Configuration, IdentityConfig IdentityConfig)
    {
        configuration = Configuration;
        identityConfig = IdentityConfig;
        httpClient = HttpClient;
        httpClient.DefaultRequestHeaders.Authorization = identityConfig.AuthenticationHeader();
        httpClient.BaseAddress = new System.Uri(configuration["apiendereco:projetos"]);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    //Consultar
    public async Task<ProjetosModel> GetProjetoAsync(long id)
    {
        var resposta = await httpClient.GetAsync($"Projetos/{id}");
        resposta.EnsureSuccessStatusCode();
        return new ProjetosModel().ToModel(await resposta.Content.ReadAsStringAsync());
    }

    //Listar todos
    public async Task<IEnumerable<ProjetosModel>> GetProjetosAsync()
    {
        var resposta = await httpClient.GetAsync($"Projetos");
        resposta.EnsureSuccessStatusCode();
        return new ProjetosModel().ToList(await resposta.Content.ReadAsStringAsync());
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
    public async Task PostProjetoAsync(ProjetosModel projeto)
    {
        var resposta = await httpClient.PostAsync("Projetos", projeto.ToJson());
        resposta.EnsureSuccessStatusCode();
    }

    //Alterar
    public async Task PutProjetoAsync(ProjetosModel projeto)
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
