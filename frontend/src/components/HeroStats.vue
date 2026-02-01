<script setup lang="ts">
interface Stats {
  total: number
  open: number
  done: number
}

defineProps<{
  apiBase: string
  stats: Stats
  lastStatus: string
}>()
</script>

<template>
  <section class="hero">
    <div>
      <p class="eyebrow">Todo App</p>
      <h1>Minimal klient för att testa API:t</h1>
      <p class="subtitle">Backend: {{ apiBase }}</p>
      <p v-if="lastStatus" class="status">Senaste HTTP‑status: {{ lastStatus }}</p>
    </div>
    <div class="stats">
      <div>
        <span>Totalt</span>
        <strong>{{ stats.total }}</strong>
      </div>
      <div>
        <span>Öppna</span>
        <strong>{{ stats.open }}</strong>
      </div>
      <div>
        <span>Klara</span>
        <strong>{{ stats.done }}</strong>
      </div>
    </div>
  </section>
</template>
