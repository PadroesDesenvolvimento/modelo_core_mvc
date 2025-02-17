using System;
using System.Collections.Generic;

public class PaginacaoModel
{
    public int ItensPorPagina { get; set; }
    public int PaginaAtual { get; set; }
    public string ColunaOrdenacao { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / ItensPorPagina);
    public string Ordenacao { get; set; }
    public List<string> ColunasOrdenacao { get; set; } = new List<string> { "cd_projeto", "nm_projeto", "ds_projeto" };
    public Dictionary<string, string> ColunasDisplayNames { get; set; } = new Dictionary<string, string>
    {
        { "cd_projeto", "Cód" },
        { "nm_projeto", "Nome" },
        { "ds_projeto", "Descrição" }
    };

    public PaginacaoModel(int itensPorPagina, int paginaAtual, string colunaOrdenacao, int totalRegistros)
    {
        ItensPorPagina = itensPorPagina;
        PaginaAtual = paginaAtual;
        ColunaOrdenacao = colunaOrdenacao;
        TotalRegistros = totalRegistros;
    }
}