<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <main class="page">
    <header class="card header-bar">
      <div>
        <p class="eyebrow">Todo App</p>
        <h1>Klart &amp; Tydligt</h1>
        <p class="subtitle">Håll koll på allt som behöver bli klart.</p>
      </div>
      <nav class="filters">
        <RouterLink class="ghost" to="/todos">Todos</RouterLink>
        <RouterLink class="ghost" to="/todos/new">Ny todo</RouterLink>
        <RouterLink class="ghost" to="/login">Logga in</RouterLink>
        <RouterLink class="ghost" to="/register">Registrera</RouterLink>
      </nav>
    </header>

    <RouterView />
  </main>
</template>

<style scoped>
.page {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.header-bar {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
  align-items: center;
  justify-content: space-between;
  border: 1px solid var(--border);
  background: linear-gradient(135deg, rgba(97, 175, 239, 0.08), rgba(152, 195, 121, 0.08));
}

.header-bar h1 {
  color: var(--text-strong);
}

.header-bar .subtitle {
  color: var(--text-muted);
}

.filters {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}
</style>
