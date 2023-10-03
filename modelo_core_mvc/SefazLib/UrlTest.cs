using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace SefazLib.UrlTest;

public class UrlTest

{
    public async Task TestUrl(string nomeTeste, string url)

    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                Console.WriteLine($"Testando {nomeTeste} - {url}");
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"URL verificada com sucesso.");
                    Console.WriteLine($"URL Content:\n{content}");
                }
                else
                {
                    Console.WriteLine("URL com falha de acesso.");
                    Console.WriteLine($"HTTP Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao acessar a URL:");
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void Verificar_URL(string nomeTeste, string url)
    {
        var urlTest = new UrlTest();
        Task.Run(async () =>
            {
                await urlTest.TestUrl(nomeTeste, url);
            }).Wait();
    }
}
