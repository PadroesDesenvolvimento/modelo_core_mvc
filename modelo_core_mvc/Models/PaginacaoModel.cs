using System;
using System.Collections.Generic;

public class PaginacaoModel
{
    public int ItensPorPagina { get; set; }
    public int PaginaAtual { get; set; }
    public string ColunaOrdenacao { get; set; }
    public int TotalRegistros { get; set; }
    public string Ordenacao { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / ItensPorPagina);
    public List<string> ColunasOrdenacao { get; set; } = new List<string> { "", "cd_projeto", "nm_projeto", "ds_projeto" };
    public Dictionary<string, string> ColunasDisplayNames { get; set; } = new Dictionary<string, string>
    {
        { "", "Sem ordenação" },
        { "cd_projeto", "Ordenar por Cód" },
        { "nm_projeto", "Ordenar por Nome" },
        { "ds_projeto", "Ordenar por Descrição" }
    };

    public PaginacaoModel(int itensPorPagina, int paginaAtual, string colunaOrdenacao, string ordenacao, int totalRegistros)
    {
        ItensPorPagina = itensPorPagina;
        PaginaAtual = paginaAtual;
        ColunaOrdenacao = colunaOrdenacao;
        Ordenacao = ordenacao;
        TotalRegistros = totalRegistros;
    }
}