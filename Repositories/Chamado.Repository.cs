using Microsoft.EntityFrameworkCore;
using DeskFlow.API.Data;
using DeskFlow.API.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskFlow.API.Repositories;

public class ChamadoRepository : IChamadoRepository
{
    private readonly AppDbContext _context;
    public ChamadoRepository(AppDbContext context) => _context = context;

    public async Task<Chamado?> ObterPorIdCompletoAsync(int id)
    {
        return await _context.Chamados
            .Include(c => c.Categoria)
            .Include(c => c.Interacoes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Chamado>> ListarComFiltrosAsync(Status? status, Prioridade? prioridade, int? categoriaId)
    {
        var query = _context.Chamados.Include(c => c.Categoria).AsQueryable();

        if (status.HasValue) query = query.Where(c => c.Status == status);
        if (prioridade.HasValue) query = query.Where(c => c.Prioridade == prioridade);
        if (categoriaId.HasValue) query = query.Where(c => c.CategoriaId == categoriaId);

        return await query.ToListAsync();
    }

    public async Task AdicionarAsync(Chamado chamado) { await _context.Chamados.AddAsync(chamado); await _context.SaveChangesAsync(); }
    public async Task AtualizarAsync(Chamado chamado) { _context.Chamados.Update(chamado); await _context.SaveChangesAsync(); }
    public async Task AdicionarInteracaoAsync(Interacao interacao) { await _context.Interacoes.AddAsync(interacao); await _context.SaveChangesAsync(); }
}
