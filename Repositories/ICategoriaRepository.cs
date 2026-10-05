using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Repositories;

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> ListarTodasAsync();
    Task<Categoria?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Categoria categoria);
    Task AtualizarAsync(Categoria categoria);
    Task DeletarAsync(Categoria categoria);
}
