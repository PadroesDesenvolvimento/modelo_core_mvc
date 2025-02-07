using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;

namespace modelo_core_mvc.Models;

[ExcludeFromCodeCoverage]

public class FormularioModel
{
    [Display(Name = "Cód")]
    public long id { get; set; }
    [Display(Name = "Nome")]
    public string nome { get; set; }
    [Display(Name = "Descrição")]
    public string descricao { get; set; }
    public FormularioModel(long Id, string Nome, string Descricao)
    {
        id = Id;
        nome = Nome;
        descricao = Descricao;
    }

    public FormularioModel()
    {

    }

    public StringContent ToJson()
    {
        return new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
    }

    public FormularioModel ToModel(string ProjetoJson)
    {
        return JsonConvert.DeserializeObject<FormularioModel>(ProjetoJson);
    }

    public IEnumerable<FormularioModel> ToList(string ProjetoJson)
    {
        return JsonConvert.DeserializeObject<IEnumerable<FormularioModel>>(ProjetoJson);
    }
}
