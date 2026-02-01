<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useAuthStore } from './stores/authStore'
import { useTodoStore } from './stores/todoStore'

const authStore = useAuthStore()
const todoStore = useTodoStore()
</script>

<template>
  <main class="page">
    <header class="card header-bar">
      <div>
        <p class="eyebrow">Todo App</p>
        <h1>Vue + Pinia + Router</h1>
        <p class="subtitle">Senaste status: {{ authStore.lastStatus || todoStore.lastStatus || '-' }}</p>
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
}

.filters {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}
</style>
