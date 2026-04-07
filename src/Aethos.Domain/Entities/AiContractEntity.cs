using System;
using System.Threading;
using System.Threading.Tasks;
using Aethos.Domain.Interfaces;
using Aethos.Domain.ValueObjects;

namespace Aethos.Domain.Entities;

public class AiContractEntity : IAiContract
{
    public ContractAddress Address { get; }
    public string Name { get; set; } = string.Empty;
    public byte[] NeuralWeights { get; set; } = Array.Empty<byte>();
    public int LayerCount { get; set; } = 4;

    public AiContractEntity(ContractAddress address) => Address = address;

    public Task<ResultHash> ExecuteInferenceAsync(byte[] inputData, CancellationToken ct = default)
    {
        throw new NotImplementedException("Inferência Neural será acoplada na Fase 5.");
    }
}
