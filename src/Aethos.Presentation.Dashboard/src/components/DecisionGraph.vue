<script setup lang="ts">
import { ref, onMounted } from 'vue';
import * as d3 from 'd3';

const container = ref<HTMLElement | null>(null);

onMounted(() => {
  if (container.value) {
    // Sprint 57: Visualização D3.js da arquitetura LSTM Aethos
    const width = 600;
    const height = 400;
    const svg = d3.select(container.value)
      .append('svg')
      .attr('width', width)
      .attr('height', height);

    const layers = [4, 8, 8, 4]; // Arquitetura Visual das 4 Camadas da LSTM
    const nodeRadius = 15;
    const layerSpacing = width / (layers.length + 1);

    layers.forEach((count, lIdx) => {
      const x = layerSpacing * (lIdx + 1);
      const nodeSpacing = height / (count + 1);

      for (let i = 0; i < count; i++) {
        const y = nodeSpacing * (i + 1);

        // Desenha os Neurônios Dourados (Design Premium)
        svg.append('circle')
          .attr('cx', x)
          .attr('cy', y)
          .attr('r', nodeRadius)
          .attr('fill', '#FFD700') // Ouro Aethos
          .attr('stroke', '#000')
          .attr('stroke-width', 2);

        // Conexões (Sinapses) se houver próxima camada
        if (lIdx < layers.length - 1) {
          const nextCount = layers[lIdx + 1];
          const nextX = layerSpacing * (lIdx + 2);
          const nextNodeSpacing = height / (nextCount + 1);

          for (let j = 0; j < nextCount; j++) {
            const nextY = nextNodeSpacing * (j + 1);
            svg.append('line')
              .attr('x1', x + nodeRadius)
              .attr('y1', y)
              .attr('x2', nextX - nodeRadius)
              .attr('y2', nextY)
              .attr('stroke', '#444')
              .attr('stroke-width', 1)
              .attr('opacity', 0.4);
          }
        }
      }
    });

    // Animação de Pulsar (Rede Neural Ativa)
    svg.selectAll('circle')
      .append('animate')
      .attr('attributeName', 'r')
      .attr('values', '13;17;13')
      .attr('dur', '2s')
      .attr('repeatCount', 'indefinite');
  }
});
</script>

<template>
  <div class="decision-graph-container">
    <h3 class="premium-title">Aethos PoR Neural Architecture Audit</h3>
    <div ref="container" class="svg-wrapper"></div>
    <div class="metrics-overlay">
      <p>Precision: 128-bit FixedPoint</p>
      <p>Audit Model: LSTM-PoR-v1</p>
    </div>
  </div>
</template>

<style scoped>
.decision-graph-container {
  background: #111;
  padding: 20px;
  border-radius: 12px;
  border: 1px solid #333;
  color: #FFD700;
  position: relative;
}
.premium-title {
  text-transform: uppercase;
  letter-spacing: 2px;
  font-size: 14px;
}
.svg-wrapper {
  display: flex;
  justify-content: center;
}
.metrics-overlay {
  font-size: 10px;
  opacity: 0.7;
}
</style>
