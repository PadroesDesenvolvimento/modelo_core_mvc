using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace SefazLib.UrlTest;

public class UrlTest

{
    public async Task<string> TestUrl(string nomeTeste, string url)
    {
        string mensagem = "";
        using (var httpClient = new HttpClient())
        {
            try
            {
                Console.WriteLine($"Testando {nomeTeste} - {url}");
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    mensagem += $"URL verificada com sucesso.";
                    //mensagem += $"URL Content:\n{content}";
                }
                else
                {
                    mensagem += "URL com falha de acesso.";
                    mensagem += $"HTTP Status Code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                mensagem += "Erro ao acessar a URL:";
                mensagem += ex.Message;
            }
        }
        return mensagem;
    }

    public string Verificar_URL(string nomeTeste, string url)
    {
        var urlTest = new UrlTest();
        var mensagem = "";
        Task.Run(async () =>
            {
                mensagem = await urlTest.TestUrl(nomeTeste, url);
            }).Wait();
        return mensagem;
    }
}
