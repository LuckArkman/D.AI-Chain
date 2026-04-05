<template>
  <div class="health-monitor">
    <h2>Network Telemetry do Mainnet & Shadow Mode</h2>
    
    <div class="stats-grid">
      <div class="stat-card">
        <h3>Último Bloco Hash (EVM State)</h3>
        <p class="mono">{{ latestBlock || '0x0000000000000...000' }}</p>
      </div>
      <div class="stat-card alert">
        <h3>Proof of Reasoning (LSTMs Hash Trace)</h3>
        <p class="mono">{{ latestPoR || 'Aguardando inference de rede neural via SignalR...' }}</p>
      </div>
    </div>

    <!-- O contêiner de SVG puro usado pela biblioteca D3.js para animar o rastro do modelo IA -->
    <div class="graph-container" id="d3-por-canvas"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import * as signalR from '@microsoft/signalr'
import { renderPoRGraph } from '../graphs/PoRGraphRenderer'

const latestBlock = ref('')
const latestPoR = ref('')

onMounted(() => {
  // Bind nativo das Websockets construídas na Fase 6 na Sprint 14
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/hubs/admin")
    .withAutomaticReconnect()
    .build();

  connection.on("OnBlockFinalized", (data) => {
    latestBlock.value = data.blockHash
  })

  connection.on("OnAiContractExecuted", (data) => {
    latestPoR.value = data.porHash
    // Engatilha D3.js Renderer no DOM Virtual
    renderPoRGraph('#d3-por-canvas', data.porHash)
  })

  connection.start()
    .then(() => connection.invoke("SubscribeToNetworkHealth"))
    .catch(err => console.error('Falha Catastrófica de Hub Connection:', err))
})
</script>

<style scoped>
.health-monitor { display: flex; flex-direction: column; gap: 2rem; }
h2 { color: #f3f4f6; border-bottom: 1px solid #374151; padding-bottom: 0.5rem; }
.stats-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.stat-card { background: #1f2937; padding: 1.5rem; border-radius: 8px; border-left: 4px solid #3b82f6; }
.stat-card.alert { border-left-color: #f59e0b; }
h3 { margin-top: 0; font-size: 0.875rem; color: #9ca3af; text-transform: uppercase; }
.mono { font-family: monospace; font-size: 1.1rem; color: #60a5fa; word-break: break-all; }
.graph-container { height: 400px; background: #111827; border-radius: 8px; border: 1px dashed #374151; margin-top: 1rem; overflow: hidden; }
</style>
