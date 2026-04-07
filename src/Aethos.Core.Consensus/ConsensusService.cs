using System.Threading.Tasks;
using Grpc.Core;
using Aethos.Core.Consensus.Grpc;
using System;

namespace Aethos.Core.Consensus;

/// <summary>
/// Sprint 27: Envio de BlockProposal e recebimento de BlockVote via P2P.
/// A ponte que interliga as máquinas virtuais independentes para selarem os estados em consenso.
/// </summary>
public class ConsensusService : ConsensusP2P.ConsensusP2PBase
{
    public override Task<BlockVote> ProposeBlock(BlockProposal request, ServerCallContext context)
    {
        Console.WriteLine($"[P2P] Bloco {request.BlockNumber} Proposto pelo Sequencer. PoR Hash: {request.PorHash}");

        // Neste estagio de homologação, o Node espelho aprova imediatamente sob o preceito do Determinismo Math L2
        var vote = new BlockVote 
        {
            BlockHash = request.BlockHash,
            ValidatorAddress = "0xLightValidator1",
            Approved = true,
            Signature = "0xFakeSignatureApproval"
        };
        
        return Task.FromResult(vote);
    }

    public override Task<Ack> BroadcastVote(BlockVote request, ServerCallContext context)
    {
        Console.WriteLine($"[P2P] Voto recebido do Validador {request.ValidatorAddress} para o Bloco {request.BlockHash}. Aprovado: {request.Approved}");
        
        // Retorna sucesso na comunicação inter-cluster
        return Task.FromResult(new Ack { Success = true });
    }
}
