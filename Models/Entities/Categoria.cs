using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DeskFlow.API.Models;

namespace DeskFlow.API.Models.Entities;

public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    // Relacionamento 1:N com Chamados
    public ICollection<Chamado> Chamados { get; set; } = new List<Chamado>();
}
