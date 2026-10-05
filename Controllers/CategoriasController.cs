using Microsoft.AspNetCore.Mvc;
using DeskFlow.API.Services;

namespace DeskFlow.API.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaService _service;
    public CategoriasController(CategoriaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.ListarTodasAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _service.ObterPorIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string nome)
    {
        var nova = await _service.CriarAsync(nome);
        return CreatedAtAction(nameof(GetById), new { id = nova.Id }, nova);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] string nome)
    {
        await _service.AtualizarAsync(id, nome);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeletarAsync(id);
        return NoContent();
    }
}

