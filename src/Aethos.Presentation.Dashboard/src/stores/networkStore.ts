import { defineStore } from 'pinia';

export const useNetworkStore = defineStore('network', {
  state: () => ({
    status: 'CONNECTING',
    divergenceRate: 0.0,
    connectedValidators: 0,
    lastUpdate: new Date(),
    transactions: [] as any[]
  }),
  actions: {
    updateHealth(data: any) {
      this.status = data.status;
      this.divergenceRate = data.divergenceRate;
      this.connectedValidators = data.connectedValidators;
      this.lastUpdate = new Date(data.timestamp);
    },
    addTransaction(tx: any) {
      this.transactions.unshift(tx);
      if (this.transactions.length > 50) this.transactions.pop();
    }
  }
});
