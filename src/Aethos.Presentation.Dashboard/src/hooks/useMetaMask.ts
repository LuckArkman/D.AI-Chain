import { ref } from 'vue';
import { ethers } from 'ethers';

export function useMetaMask() {
  const account = ref<string | null>(null);
  const chainId = ref<string | null>(null);
  const isConnected = ref(false);

  const connect = async () => {
    if ((window as any).ethereum) {
      try {
        const provider = new ethers.BrowserProvider((window as any).ethereum);
        const accounts = await provider.send("eth_requestAccounts", []);
        account.value = accounts[0];
        isConnected.value = true;
        
        const network = await provider.getNetwork();
        chainId.value = network.chainId.toString();

        // Sprint 56: Forçar a rede Aethos L2 (Custom Chain 0xAE7H05)
        if (chainId.value !== '7146373') { // 0xAE7H05 em decimal
          await switchNetwork();
        }
      } catch (error) {
        console.error("Falha ao conectar MetaMask:", error);
      }
    } else {
      alert("Por favor, instale a MetaMask para interagir com a Aethos Ledger!");
    }
  };

  const switchNetwork = async () => {
    try {
      await (window as any).ethereum.request({
        method: 'wallet_addEthereumChain',
        params: [{
          chainId: '0xAE7H05',
          chainName: 'Aethos Ledger L2',
          nativeCurrency: { name: 'Aethos Token', symbol: 'AETH', decimals: 18 },
          rpcUrls: ['http://localhost:5000'], // Nosso Nó em C#
          blockExplorerUrls: null
        }]
      });
    } catch (error) {
      console.error("Erro ao trocar para rede Aethos:", error);
    }
  };

  return { account, chainId, isConnected, connect };
}
