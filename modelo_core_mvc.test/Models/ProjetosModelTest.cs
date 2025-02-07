using modelo_core_mvc.Models;
using Newtonsoft.Json;

namespace modelo_core_mvc.test.Models;
public class FormularioModelTest
{
    [Fact]
    public async Task ToJson_ReturnsStringContent()
    {
        // Arrange
        var projeto = new FormularioModel(1, "Projeto Teste", "Descrição do projeto");

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
    public void ToModel_ReturnsFormularioModel()
    {
        // Arrange
        var projetoJson = "{\"id\":1,\"nome\":\"Projeto Teste\",\"descricao\":\"Descrição do projeto\"}";

        // Act
        var result = new FormularioModel().ToModel(projetoJson);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FormularioModel>(result);
        Assert.Equal(1, result.id);
        Assert.Equal("Projeto Teste", result.nome);
        Assert.Equal("Descrição do projeto", result.descricao);
    }

    [Fact]
    public void ToList_ReturnsListOfRegistrosModel()
    {
        // Arrange
        var registrosJson = "[{\"id\":1,\"nome\":\"Projeto Teste 1\",\"descricao\":\"Descrição do projeto 1\"}, {\"id\":2,\"nome\":\"Projeto Teste 2\",\"descricao\":\"Descrição do projeto 2\"}]";

        // Act
        var result = new FormularioModel().ToList(registrosJson);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<FormularioModel>>(result);
        var registrosList = new List<FormularioModel>(result);
        Assert.Equal(2, registrosList.Count);
        Assert.Equal(1, registrosList[0].id);
        Assert.Equal("Projeto Teste 1", registrosList[0].nome);
        Assert.Equal("Descrição do projeto 1", registrosList[0].descricao);
        Assert.Equal(2, registrosList[1].id);
        Assert.Equal("Projeto Teste 2", registrosList[1].nome);
        Assert.Equal("Descrição do projeto 2", registrosList[1].descricao);
    }
}
