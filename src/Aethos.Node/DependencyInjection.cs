using Microsoft.Extensions.DependencyInjection;
using Aethos.Domain.Interfaces;
using Aethos.Core.Persistence.RocksDB;
using Aethos.Core.EVM;
using Aethos.Core.Consensus;

using Aethos.Application.Common.Behaviors;
using MediatR;
using System.Reflection;

namespace Aethos.Node;

/// <summary>
/// Sprint 29: Orchestrator que congrega todos os micro-sistemas no Nó raiz.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAethosNode(this IServiceCollection services, string dataDirectory)
    {
        // 1. Armazenamento L2 (Merkle + Rocks)
        services.AddSingleton<IStateDb>(sp => new RocksDbStore(dataDirectory));
        
        // 2. Transações e Smart Contracts EVM
        services.AddScoped<EvmTransactionProcessor>();
        
        // 3. Comunicação P2P nativa com validadores
        services.AddGrpc();
        
        // 4. O Coração: Background Worker Engine L2
        services.AddHostedService<Sequencer>();

        // 5. Sprint 30: Auditor Matemático contra Traições (Drift P2P)
        services.AddSingleton<DivergenceGuard>();

        // 6. Sprint 31: Serviço Flagelador de Infraestrutura Incorreta
        services.AddSingleton<SlashingService>();

        // 7. Sprint 32: Registro de contas L2 (Mapeador de EOAs e AIWallets)
        services.AddSingleton<Aethos.Core.SmartContracts.AccountRegistry>();

        // 8. Sprint 33: Account Abstraction (EIP-4337 Base Handler)
        services.AddSingleton<Aethos.Core.SmartContracts.ERC4337.UserOperationHandler>();

        // 9. Sprint 34: Modelagem Autônoma Neural Lógica (Limiares de certeza LSTM)
        services.AddSingleton<Aethos.Core.SmartContracts.ERC4337.AiWalletLogic>();

        // 10. Sprint 35: Botão do Pânico (Retenção e revogação soberana de autonomais L2)
        services.AddSingleton<Aethos.Core.SmartContracts.ERC4337.PanicButtonExecute>();

        // 11. Sprint 36: Governança Máster / Admin L2 State (Suspension Logic)
        services.AddSingleton<Aethos.Core.Governance.GovernanceService>();

        // 12. Sprint 37: Shadow Mode (Testes A/B neurais sem escrita no State Root)
        services.AddSingleton<Aethos.Core.Governance.ShadowModeManager>();

        // 13. Sprint 38: Votação On-Chain $AETH (PoS Ponderado)
        services.AddSingleton<Aethos.Core.Governance.GovernanceVoting>();

        // Sprint 45 e 47: Bridge L1/L2 Relayer (Ethereum Finality)
        services.AddSingleton(sp => new Aethos.Infrastructure.Bridge.L1BridgeClient(
            "https://eth-sepolia.g.alchemy.com/v2/YOUR-API-KEY", // RPC Sepolia
            "0x-YOUR-SEQUENCER-PRIVATE-KEY", // Private Key (EM PRODUCAO USAR KES/AZURE KEY VAULT)
            "0x-AETHOS-BRIDGE-CONTRACT-ADDRESS", // Endereço L1
            sp.GetRequiredService<ILogger<Aethos.Infrastructure.Bridge.L1BridgeClient>>()
        ));

        services.AddHostedService<Aethos.Infrastructure.Bridge.StateRootPublisher>();

        // Sprint 52: Monitoramento Proativo Master (Push para Dashboard SignalR)
        services.AddHostedService<Aethos.Presentation.RPC.Services.NetworkHealthBroadcaster>();

        // Sprint 39 a 41: Application Layer (MediatR + FluentValidation + Polly)
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Aethos.Application.Transactions.Commands.ProcessTransactionCommand).Assembly);
            
            // Ordem de execução (Behaviors): Log -> Guardrail -> Retry -> Validation -> Handler
            cfg.AddOpenBehavior(typeof(AiContractGuardBehavior<,>));
            cfg.AddOpenBehavior(typeof(RetryBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
