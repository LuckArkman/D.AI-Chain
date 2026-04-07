using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aethos.Core.Governance;

/// <summary>
/// Sprint 38: Governance Voting (Votação On-Chain Ponderada).
/// Sistema de PoS (Proof of Stake) para atualização de parâmetros da rede Aethos L1/L2.
/// </summary>
public class GovernanceVoting
{
    private readonly ILogger<GovernanceVoting> _logger;
    private readonly ConcurrentDictionary<string, decimal> _proposalVotes;

    public GovernanceVoting(ILogger<GovernanceVoting> logger)
    {
        _logger = logger;
        _proposalVotes = new ConcurrentDictionary<string, decimal>();
    }

    /// <summary>
    /// Registra um voto ponderado pelo Stake de $AETH do validador.
    /// </summary>
    public Task CastVoteAsync(string proposalId, string validatorAddress, decimal aethStake, bool approved)
    {
        if (!approved) return Task.CompletedTask;

        // Adiciona o peso do Stake à proposta desejada.
        _proposalVotes.AddOrUpdate(proposalId, aethStake, (id, currentWeight) => currentWeight + aethStake);
        
        _logger.LogInformation($"[GOVERNANCE] Voto computado do validador {validatorAddress}. Stake atribuído: {aethStake} $AETH.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica se a maioria percentual foi atingida para atualizar parâmetros (ex: Base Fee).
    /// </summary>
    public bool IsProposalApproved(string proposalId, decimal requiredMajorityStake)
    {
        if (_proposalVotes.TryGetValue(proposalId, out var totalStakedVotes))
        {
            bool approved = totalStakedVotes >= requiredMajorityStake;
            if (approved)
            {
                _logger.LogCritical($"[GOVERNANCE] PROPOSTA {proposalId} APROVADA PELO CONSENSO $AETH! Atualizando parâmetros da rede.");
            }
            return approved;
        }
        return false;
    }
}
