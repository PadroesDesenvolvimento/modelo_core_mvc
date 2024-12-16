using modelo_core_mvc.Models;
using modelo_core_mvc.projetos;
using Newtonsoft.Json;

namespace modelo_core_mvc.test.Models;
public class ProjetosModelTest
{
    [Fact]
    public async Task ToJson_ReturnsStringContent()
    {
        // Arrange
        var projeto = new ProjetosModel(1, "Projeto Teste", "Descrição do projeto");

        // Act
        var result = projeto.ToJson();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        var jsonString = await result.ReadAsStringAsync();
        var expectedJson = JsonConvert.SerializeObject(projeto);
        Assert.Equal(expectedJson, jsonString);
    }

    [Fact]
    public void ToModel_ReturnsProjetosModel()
    {
        // Arrange
        var projetoJson = "{\"id\":1,\"nome\":\"Projeto Teste\",\"descricao\":\"Descrição do projeto\"}";

        // Act
        var result = new ProjetosModel().ToModel(projetoJson);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ProjetosModel>(result);
        Assert.Equal(1, result.id);
        Assert.Equal("Projeto Teste", result.nome);
        Assert.Equal("Descrição do projeto", result.descricao);
    }

    [Fact]
    public void ToList_ReturnsListOfProjetosModel()
    {
        // Arrange
        var projetosJson = "[{\"id\":1,\"nome\":\"Projeto Teste 1\",\"descricao\":\"Descrição do projeto 1\"}, {\"id\":2,\"nome\":\"Projeto Teste 2\",\"descricao\":\"Descrição do projeto 2\"}]";

        // Act
        var result = new ProjetosModel().ToList(projetosJson);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<ProjetosModel>>(result);
        var projetosList = new List<ProjetosModel>(result);
        Assert.Equal(2, projetosList.Count);
        Assert.Equal(1, projetosList[0].id);
        Assert.Equal("Projeto Teste 1", projetosList[0].nome);
        Assert.Equal("Descrição do projeto 1", projetosList[0].descricao);
        Assert.Equal(2, projetosList[1].id);
        Assert.Equal("Projeto Teste 2", projetosList[1].nome);
        Assert.Equal("Descrição do projeto 2", projetosList[1].descricao);
    }
}
