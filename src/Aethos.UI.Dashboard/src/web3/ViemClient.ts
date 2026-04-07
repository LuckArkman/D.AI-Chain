import { createPublicClient, http } from 'viem'
import { mainnet } from 'viem/chains'

// Camada Resistência à Censura: Com viem a Dashboard fala direto com RPC Ethereum
export const l1PublicClient = createPublicClient({
  chain: mainnet,
  transport: http()
})

// Direcionador do Local RPC Endpoint (Subido pela Camada Aethos.Node C#)
export const aethosL2Client = createPublicClient({
  chain: {
      id: 0xAE7, // Arbitrário ChainID (como mock, 2791 Hexadecimal)
      name: 'Aethos Ledger L2',
      nativeCurrency: { name: 'Aethos ETH', symbol: 'ETH', decimals: 18 },
      rpcUrls: { default: { http: ['http://localhost:5000/rpc'] } }
  },
  transport: http()
})
