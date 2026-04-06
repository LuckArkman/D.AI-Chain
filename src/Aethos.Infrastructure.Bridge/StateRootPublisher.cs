using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aethos.Infrastructure.Bridge;

/// <summary>
/// Sprint 47: State Root Publisher (L2 -> L1 Relayer).
/// Job assíncrono encarregado de enviar periodicamente a raiz de estado consolidada
/// para o Smart Contract na Camada 1 (Ethereum/Sepolia).
/// </summary>
public class StateRootPublisher : BackgroundService
{
    private readonly ILogger<StateRootPublisher> _logger;
    private readonly L1BridgeClient _bridgeClient;
    private readonly Random _mockDataGenerator = new Random();

    public StateRootPublisher(ILogger<StateRootPublisher> logger, L1BridgeClient bridgeClient)
    {
        _logger = logger;
        _bridgeClient = bridgeClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[RELAYER-L2] Iniciando o Job de Publicação de Estado L1...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. Em produção, buscaríamos a última StateRoot e o PoRAggregate do RocksDB/Sequencer
                long currentL2Block = 420; // Bloco simulado
                string mockStateRoot = "0x" + Guid.NewGuid().ToString("N");
                string mockPoRHash = "0x" + Guid.NewGuid().ToString("N");

                _logger.LogInformation($"[RELAYER-L2] Sincronizando checkpoint L2 no Ethereum (Bloco #{currentL2Block})...");

                // 2. Dispara o commit via Nethereum para o contrato L1
                await _bridgeClient.CommitL2BlockToL1Async(currentL2Block, mockStateRoot, mockPoRHash);

                _logger.LogInformation("[RELAYER-L2] Checkpoint L1 estabelecido com SUCESSO. Próximo ciclo em 1 hora.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[RELAYER-L2] ERRO CRÍTICO no Relayer: {ex.Message}. Re-tentando no próximo ciclo.");
            }

            // Intervalo de checkpoint (1 hora conforme Sprint 47)
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
