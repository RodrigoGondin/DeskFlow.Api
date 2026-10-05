using Microsoft.EntityFrameworkCore;
using DeskFlow.API.Data;
using DeskFlow.API.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskFlow.API.Repositories;

// A interface que estava faltando foi declarada aqui dentro!
public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> ListarTodasAsync();
    Task<Categoria?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Categoria categoria);
    Task AtualizarAsync(Categoria categoria);
    Task DeletarAsync(Categoria categoria);
    Task<bool> PossuiChamadosVinculadosAsync(int categoriaId);
}

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;
    public CategoriaRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Categoria>> ListarTodasAsync() => 
        await _context.Categorias.ToListAsync();

    public async Task<Categoria?> ObterPorIdAsync(int id) => 
        await _context.Categorias.FindAsync(id);

    public async Task AdicionarAsync(Categoria categoria)
    {
        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task DeletarAsync(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> PossuiChamadosVinculadosAsync(int categoriaId) => 
        await _context.Chamados.AnyAsync(c => c.CategoriaId == categoriaId);
}
