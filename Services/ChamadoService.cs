using DeskFlow.API.Models.Entities;
using DeskFlow.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private static async Task<Chamado> ObterChamadoPorIdAsync(IChamadoRepository chamadoRepo, int id)
    {
        var metodo = chamadoRepo.GetType()
            .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
            .FirstOrDefault(m =>
                m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(int)
                && m.ReturnType.IsGenericType
                && m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
                && m.ReturnType.GetGenericArguments()[0] == typeof(Chamado));

        if (metodo == null)
            throw new InvalidOperationException("O repositório de chamados não expõe um método compatível para buscar por ID.");

        var tarefa = (Task<Chamado>)metodo.Invoke(chamadoRepo, new object[] { id })!;
        return await tarefa;
    }

    private static async Task AdicionarChamadoAsync(IChamadoRepository chamadoRepo, Chamado chamado)
    {
        var metodo = typeof(IChamadoRepository)
            .GetMethods()
            .FirstOrDefault(m =>
                m.Name is "AdicionarAsync" or "CriarAsync" or "AdicionarChamadoAsync" or "CriarChamadoAsync" or "SalvarAsync" or "InserirAsync"
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(Chamado));

        if (metodo == null)
            throw new InvalidOperationException("O repositório de chamados não expõe um método de criação compatível.");

        var tarefa = (Task)metodo.Invoke(chamadoRepo, new object[] { chamado })!;
        await tarefa;
    }

    private static async Task AtualizarChamadoAsync(IChamadoRepository chamadoRepo, Chamado chamado)
    {
        var metodo = chamadoRepo.GetType()
            .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
            .FirstOrDefault(m =>
                m.Name.EndsWith("AtualizarAsync", StringComparison.Ordinal)
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(Chamado)
                && typeof(Task).IsAssignableFrom(m.ReturnType));

        if (metodo == null)
            throw new InvalidOperationException("O repositório de chamados não expõe um método compatível para atualizar chamados.");

        var tarefa = (Task)metodo.Invoke(chamadoRepo, new object[] { chamado })!;
        await tarefa;
    }

    private static async Task AdicionarInteracaoAsync(IChamadoRepository chamadoRepo, Interacao interacao)
    {
        var metodo = chamadoRepo.GetType()
            .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
            .FirstOrDefault(m =>
                m.Name.EndsWith("AdicionarInteracaoAsync", StringComparison.Ordinal)
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(Interacao));

        if (metodo == null)
            throw new InvalidOperationException("O repositório de chamados não expõe um método compatível para adicionar interações.");

        var tarefa = (Task)metodo.Invoke(chamadoRepo, new object[] { interacao })!;
        await tarefa;
    }

    private static async Task<IEnumerable<Chamado>> ListarComFiltrosAsync(
        IChamadoRepository chamadoRepo, Status? status, Prioridade? prioridade, int? catId)
    {
        var metodo = chamadoRepo.GetType()
            .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
            .FirstOrDefault(m =>
                m.Name == "ListarComFiltrosAsync"
                && m.GetParameters().Length == 3
                && m.GetParameters()[0].ParameterType == typeof(Status?)
                && m.GetParameters()[1].ParameterType == typeof(Prioridade?)
                && m.GetParameters()[2].ParameterType == typeof(int?));

        if (metodo == null)
            throw new InvalidOperationException("O repositório de chamados não expõe um método compatível para listar com filtros.");

        var tarefa = metodo.Invoke(chamadoRepo, new object?[] { status, prioridade, catId }) as Task
            ?? throw new InvalidOperationException("O método de listagem do repositório não retornou uma tarefa válida.");
        await tarefa;

        return tarefa.GetType().GetProperty("Result")?.GetValue(tarefa) as IEnumerable<Chamado>
            ?? Enumerable.Empty<Chamado>();
    }

    public async Task<Chamado> AbrirChamadoAsync(Chamado chamado)
    {
        var categoriaExists = await _categoriaRepo.ObterPorIdAsync(chamado.CategoriaId);
        if (categoriaExists == null) 
            throw new ArgumentException("Categoria informada é inválida.");

        // RF06 - Atribuição automatizada de Status e Data de Abertura
        chamado.Status = Status.Aberto;
        chamado.DataAbertura = DateTime.UtcNow;
        chamado.DataFechamento = null;
        chamado.Solucao = null;

        await AdicionarChamadoAsync(_chamadoRepo, chamado);
        return chamado;
    }

    public async Task IniciarAtendimentoAsync(int id)
    {
        var chamado = await ObterChamadoPorIdAsync(_chamadoRepo, id) 
            ?? throw new KeyNotFoundException("Chamado não encontrado.");
            
        // RF07 - Validação de status para início do atendimento
        if (chamado.Status != Status.Aberto) 
            throw new InvalidOperationException("Apenas chamados com status 'Aberto' podem ser iniciados.");

        chamado.Status = Status.EmAndamento;
        await AtualizarChamadoAsync(_chamadoRepo, chamado);
    }

    public async Task EncerrarChamadoAsync(int id, string solucao)
    {
        // RF08 - Validação de Solução Obrigatória
        if (string.IsNullOrWhiteSpace(solucao)) 
            throw new ArgumentException("O texto de solução é obrigatório para encerrar o chamado.");
        
        var chamado = await ObterChamadoPorIdAsync(_chamadoRepo, id) 
            ?? throw new KeyNotFoundException("Chamado não encontrado.");
            
        if (chamado.Status == Status.Fechado) 
            throw new InvalidOperationException("Este chamado já se encontra encerrado.");

        // RF08 - Encerramento com registro de data e solução técnica
        chamado.Status = Status.Fechado;
        chamado.Solucao = solucao;
        chamado.DataFechamento = DateTime.UtcNow;

        await AtualizarChamadoAsync(_chamadoRepo, chamado);
    }

    public async Task AdicionarInteracaoAsync(int chamadoId, string autor, string mensagem)
    {
        var chamado = await ObterChamadoPorIdAsync(_chamadoRepo, chamadoId) 
            ?? throw new KeyNotFoundException("Chamado não encontrado.");
            
        // RF10 - Bloqueio de novas interações em chamados fechados
        if (chamado.Status == Status.Fechado) 
            throw new InvalidOperationException("Não é permitido adicionar interações a um chamado fechado.");

        var interacao = new Interacao
        {
            ChamadoId = chamadoId,
            Autor = autor,
            Mensagem = mensagem,
            DataRegistro = DateTime.UtcNow
        };

        await AdicionarInteracaoAsync(_chamadoRepo, interacao);
    }

    public async Task<Chamado> ObterDetalhesAsync(int id) => 
        await ObterChamadoPorIdAsync(_chamadoRepo, id) ?? throw new KeyNotFoundException("Chamado não encontrado.");

    public async Task<IEnumerable<Chamado>> ListarComFiltrosAsync(Status? status, Prioridade? prioridade, int? catId) => 
        await ListarComFiltrosAsync(_chamadoRepo, status, prioridade, catId);
}
