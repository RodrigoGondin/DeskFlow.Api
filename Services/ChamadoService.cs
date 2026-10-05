using DeskFlow.API.Models.Entities;
using DeskFlow.API.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskFlow.API.Services;

public class ChamadoService
{
    private readonly IChamadoRepository _chamadoRepo;
    private readonly ICategoriaRepository _categoriaRepo;

    public ChamadoService(IChamadoRepository chamadoRepo, ICategoriaRepository categoriaRepo)
    {
        _chamadoRepo = chamadoRepo;
        _categoriaRepo = categoriaRepo;
    }

    public async Task<Chamado> AbrirChamadoAsync(Chamado chamado)
    {
        var categoriaExists = await _categoriaRepo.ObterPorIdAsync(chamado.CategoriaId);
        if (categoriaExists == null) 
            throw new ArgumentException("Categoria informada é inválida.");

        // RF06 - Atribui automaticamente status Aberto e DataAbertura atual
        chamado.Status = Status.Aberto;
        chamado.DataAbertura = DateTime.UtcNow;
        chamado.DataFechamento = null;
        chamado.Solucao = null;

        await _chamadoRepo.AdicionarAsync(chamado);
        return chamado;
    }

    public async Task IniciarAtendimentoAsync(int id)
    {
        var chamado = await _chamadoRepo.ObterPorIdCompletoAsync(id) 
            ?? throw new KeyNotFoundException("Chamado não encontrado.");
            
        // Validação do Ciclo de Vida
        if (chamado.Status != Status.Aberto) 
            throw new InvalidOperationException("Apenas chamados com status 'Aberto' podem ser iniciados.");

        chamado.Status = Status.EmAndamento;
        await _chamadoRepo.AtualizarAsync(chamado);
    }

    public async Task EncerrarChamadoAsync(int id, string solucao)
    {
        // RF08 - Deve exigir a informação do texto de Solucao
        if (string.IsNullOrWhiteSpace(solucao)) 
            throw new ArgumentException("É obrigatório informar o texto de solução para encerrar o chamado.");
        
        var chamado = await _chamadoRepo.ObterPorIdCompletoAsync(id) 
            ?? throw new KeyNotFoundException("Chamado não encontrado.");
            
        if (chamado.Status == Status.Fechado) 
            throw new InvalidOperationException("Este chamado já se encontra encerrado.");

        // RF08 - Grava DataFechamento, altera status para Fechado e salva Solucao
        chamado.Status = Status.Fechado;
        chamado.Solucao = solucao;
        chamado.DataFechamento = DateTime.UtcNow;

        await _chamadoRepo.AtualizarAsync(chamado);
    }

    public async Task AdicionarInteracaoAsync(int chamadoId, string autor, string mensagem)
    {
        var chamado = await _chamadoRepo.ObterPorIdCompletoAsync(chamadoId) 
            ?? throw new KeyNotFoundException("Chamado não encontrado.");
            
        // RF10 - Permite adicionar comentários apenas se o chamado NÃO estiver Fechado
        if (chamado.Status == Status.Fechado) 
            throw new InvalidOperationException("Não é permitido adicionar interações a um chamado fechado.");

        var interacao = new Interacao
        {
            ChamadoId = chamadoId,
            Autor = autor,
            Mensagem = mensagem,
            DataRegistro = DateTime.UtcNow
        };

        await _chamadoRepo.AdicionarInteracaoAsync(interacao);
    }

    public async Task<Chamado> ObterDetalhesAsync(int id) => 
        await _chamadoRepo.ObterPorIdCompletoAsync(id) ?? throw new KeyNotFoundException("Chamado não encontrado.");

    public async Task<IEnumerable<Chamado>> ListarComFiltrosAsync(Status? status, Prioridade? prioridade, int? catId) => 
        await _chamadoRepo.ListarComFiltrosAsync(status, prioridade, catId);
}
