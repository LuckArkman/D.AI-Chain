<template>
  <div id="aethos-dashboard">
    <header class="navbar">
      <h1>Aethos Layer 2 // Admin Control Plane</h1>
    </header>
    <main>
      <NetworkHealth />
    </main>
  </div>
</template>

<script setup lang="ts">
import NetworkHealth from './views/NetworkHealth.vue'
</script>

<style>
body { margin: 0; font-family: 'Inter', sans-serif; background: #0b0f19; color: #fff; }
.navbar { padding: 1rem; border-bottom: 1px solid #1f2937; background: #111827; }
h1 { margin: 0; font-size: 1.25rem; font-weight: 600; letter-spacing: 1px; color: #10b981; }
main { padding: 2rem; }
</style>
