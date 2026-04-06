using System;
using System.Collections.Generic;
using System.Linq;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Aethos.Infrastructure.Bridge;

/// <summary>
/// Prova de Merkle simplificada para conformidade com EscapeHatch.sol.
/// </summary>
public class MerkleProofGenerator
{
    private static byte[] Keccak256(byte[] data) => Sha3Keccack.Current.CalculateHash(data);

    public string[] GenerateUserProof(List<string> addresses, List<decimal> amounts, string targetAddress)
    {
        var leaves = new List<byte[]>();
        for (int i = 0; i < addresses.Count; i++)
        {
            // O formato das folhas deve bater com o abi.encodePacked do Solidity (EscapeHatch.sol:38)
            leaves.Add(Keccak256(System.Text.Encoding.UTF8.GetBytes(addresses[i] + amounts[i].ToString())));
        }

        int targetIdx = addresses.IndexOf(targetAddress);
        if (targetIdx == -1) return Array.Empty<string>();

        // Lógica de árvore binária manual para gerar a prova (Path)
        var proof = new List<string>();
        var currentLevel = leaves;

        while (currentLevel.Count > 1)
        {
            var nextLevel = new List<byte[]>();
            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                if (i + 1 < currentLevel.Count)
                {
                    // Se o vizinho for o par do nosso alvo, guardamos ele na prova
                    if (i == targetIdx) proof.Add(currentLevel[i + 1].ToHex(true));
                    else if (i + 1 == targetIdx) proof.Add(currentLevel[i].ToHex(true));

                    // Sobe o hash combinado para o próximo nível
                    nextLevel.Add(Keccak256(currentLevel[i].Concat(currentLevel[i + 1]).ToArray()));
                }
                else
                {
                    // Se estivermos no fim da fila sem par, o noh sobe sozinho (conforme padrao Merkle)
                    if (i == targetIdx) { /* No brother to add to proof at this level */ }
                    nextLevel.Add(currentLevel[i]);
                }
            }
            currentLevel = nextLevel;
            targetIdx /= 2;
        }

        return proof.ToArray();
    }
}
