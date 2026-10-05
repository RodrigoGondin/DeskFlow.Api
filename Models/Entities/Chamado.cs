using System.ComponentModel.DataAnnotations;

namespace DeskFlow.API.Models.Entities;

public enum Prioridade { Baixa, Media, Alta }
public enum Status { Aberto, EmAndamento, Fechado }

public class Chamado
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public Prioridade Prioridade { get; set; }

    [Required]
    public Status Status { get; set; }

    [Required]
    [StringLength(100)]
    public string SolicitanteNome { get; set; } = string.Empty;

    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public string? Solucao { get; set; }

    // Relacionamento com Categoria
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    // Relacionamento 1:N com Interacoes
    public ICollection<Interacao> Interacoes { get; set; } = new List<Interacao>();
}
