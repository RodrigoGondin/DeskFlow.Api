using DeskFlow.API.Models.Entities;
using DeskFlow.API.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskFlow.API.Services;

public class CategoriaService
{
    private readonly ICategoriaRepository _repo;

    public CategoriaService(ICategoriaRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Categoria>> ListarTodasAsync() => 
        await _repo.ListarTodasAsync();

    public async Task<Categoria> ObterPorIdAsync(int id) => 
        await _repo.ObterPorIdAsync(id) ?? throw new KeyNotFoundException("Categoria não encontrada.");

    public async Task<Categoria> CriarAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) 
            throw new ArgumentException("O nome da categoria é obrigatório.");
            
        var nova = new Categoria { Nome = nome };
        await _repo.AdicionarAsync(nova);
        return nova;
    }

    public async Task AtualizarAsync(int id, string nome)
    {
        var categoria = await ObterPorIdAsync(id);
        if (string.IsNullOrWhiteSpace(nome)) 
            throw new ArgumentException("O nome da categoria não pode ser vazio.");
            
        categoria.Nome = nome;
        await _repo.AtualizarAsync(categoria);
    }

    public async Task DeletarAsync(int id)
    {
        var categoria = await ObterPorIdAsync(id);
        
        // RF04 - Valida se ela possui chamados associados antes de deletar
        if (await _repo.PossuiChamadosVinculadosAsync(id)) 
            throw new InvalidOperationException("Não é possível excluir uma categoria que possui chamados vinculados.");
        
        await _repo.DeletarAsync(categoria);
    }
}
