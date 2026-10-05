using DeskFlow.API.Data;
using DeskFlow.API.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeskFlow.API.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;
    public CategoriaRepository(AppDbContext context) => _context = context;

    public Task<IEnumerable<Categoria>> ListarTodasAsync() =>
        Task.FromResult<IEnumerable<Categoria>>(_context.Categorias.ToList());

    public Task<Categoria?> ObterPorIdAsync(int id) =>
        Task.FromResult(_context.Categorias.Find(id));

    public Task AdicionarAsync(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        return _context.SaveChangesAsync();
    }

    public Task AtualizarAsync(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        return _context.SaveChangesAsync();
    }

    public Task DeletarAsync(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
        return _context.SaveChangesAsync();
    }

    public Task<bool> PossuiChamadosVinculadosAsync(int categoriaId) =>
        Task.FromResult(_context.Chamados.Any(c => c.CategoriaId == categoriaId));
}
