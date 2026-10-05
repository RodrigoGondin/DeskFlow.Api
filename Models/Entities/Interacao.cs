using System.ComponentModel.DataAnnotations;

namespace DeskFlow.API.Models.Entities;

public class Interacao
{
    [Key]
    public int Id { get; set; }

    public int ChamadoId { get; set; }
    public Chamado? Chamado { get; set; }

    [Required]
    [StringLength(100)]
    public string Autor { get; set; } = string.Empty;

    [Required]
    public string Mensagem { get; set; } = string.Empty;

    public DateTime DataRegistro { get; set; }
}
