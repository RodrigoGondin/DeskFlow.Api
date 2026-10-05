using Microsoft.AspNetCore.Mvc;
using DeskFlow.API.Models.Entities;
using DeskFlow.API.Services;

namespace DeskFlow.API.Controllers;

[ApiController]
[Route("api/chamados")]
public class ChamadosController : ControllerBase
{
    private readonly ChamadoService _service;
    public ChamadosController(ChamadoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Status? status, [FromQuery] Prioridade? prioridade, [FromQuery] int? categoriaId)
    {
        return Ok(await _service.ListarComFiltrosAsync(status, prioridade, categoriaId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _service.ObterDetalhesAsync(id));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Chamado chamado)
    {
        var novo = await _service.AbrirChamadoAsync(chamado);
        return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
    }

    [HttpPatch("{id}/iniciar")]
    public async Task<IActionResult> IniciarAtendimento(int id)
    {
        await _service.IniciarAtendimentoAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/encerrar")]
    public async Task<IActionResult> EncerrarChamado(int id, [FromBody] string solucao)
    {
        await _service.EncerrarChamadoAsync(id, solucao);
        return NoContent();
    }

    [HttpPost("{id}/interacoes")]
    public async Task<IActionResult> AdicionarInteracao(int id, [FromBody] InteracaoInput input)
    {
        await _service.AdicionarInteracaoAsync(id, input.Autor, input.Mensagem);
        return Ok(new { message = "Interação incluída com sucesso." });
    }
}

public record InteracaoInput(string Autor, string Mensagem);
