<script setup lang="ts">
import { ref } from 'vue';
import { ExclamationTriangleIcon, BoltIcon, StopIcon } from '@heroicons/vue/24/outline';

const isPaused = ref(false);
const shadowMode = ref(true);

const togglePause = () => {
  isPaused.value = !isPaused.value;
  // Sprint 58: Comunicação com a API de Governança L2 em C#
  console.warn(`[GOVERNANCE] Rede ${isPaused.value ? 'SUSPENSA' : 'REATIVADA'} pelo comando Admin.`);
};

const toggleShadow = () => {
  shadowMode.value = !shadowMode.value;
  console.info(`[SHADOW-MODE] Teste A/B de Modelos ${shadowMode.value ? 'ATIVADO' : 'DESATIVADO'}.`);
};
</script>

<template>
  <div class="emergency-panel">
    <div class="panel-header">
      <ExclamationTriangleIcon class="icon-warning" />
      <h3>Global L2 Governance Controls</h3>
    </div>

    <div class="controls-grid">
      <button @click="togglePause" :class="['btn-action', isPaused ? 'btn-resume' : 'btn-pause']">
        <StopIcon v-if="!isPaused" class="icon-sm" />
        <BoltIcon v-else class="icon-sm" />
        {{ isPaused ? 'RESUME NETWORK' : 'EMERGENCY PAUSE' }}
      </button>

      <div class="config-item">
        <span>Shadow Mode (A/B Test)</span>
        <label class="switch">
          <input type="checkbox" :checked="shadowMode" @change="toggleShadow">
          <span class="slider round"></span>
        </label>
      </div>
    </div>
  </div>
</template>

<style scoped>
.emergency-panel {
  background: #1a0000;
  border: 1px solid #ff4444;
  padding: 1.5rem;
  border-radius: 12px;
  color: #ffcccc;
}
.panel-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
}
.icon-warning { width: 24px; color: #ff4444; }
.controls-grid {
  display: flex;
  flex-direction: column;
  gap: 15px;
}
.btn-action {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px;
  font-weight: bold;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
}
.btn-pause { background: #ff4444; color: white; }
.btn-pause:hover { background: #cc0000; }
.btn-resume { background: #00ff44; color: #003300; }
.icon-sm { width: 18px; }

/* Switch Style */
.switch { position: relative; display: inline-block; width: 44px; height: 22px; }
.switch input { opacity: 0; width: 0; height: 0; }
.slider { position: absolute; cursor: pointer; top: 0; left: 0; right: 0; bottom: 0; background-color: #333; transition: .4s; border-radius: 34px; }
.slider:before { position: absolute; content: ""; height: 16px; width: 16px; left: 3px; bottom: 3px; background-color: white; transition: .4s; border-radius: 50%; }
input:checked + .slider { background-color: #ffd700; }
input:checked + .slider:before { transform: translateX(22px); }
</style>
