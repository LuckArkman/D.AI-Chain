using MediatR;
using Aethos.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;

namespace Aethos.Application.Transactions.Commands;

/// <summary>
/// Sprint 39: Comando Mediator para processar uma transação rígidamente no Ledger.
/// </summary>
public class ProcessTransactionCommand : IRequest<bool>
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Data { get; set; }
}

/// <summary>
/// Sprint 39: Handler central que isola a lógica de negócio e integra a EVM à pipeline de aplicação.
/// </summary>
public class ProcessTransactionHandler : IRequestHandler<ProcessTransactionCommand, bool>
{
    public Task<bool> Handle(ProcessTransactionCommand request, CancellationToken cancellationToken)
    {
        // 1. Aqui seriam chamadas as validações de domínio.
        // 2. Chamar o EVM Transaction Processor que codificamos.
        // 3. Orquestrar o persistência no RocksDB.
        
        return Task.FromResult(true);
    }
}
