<script setup lang="ts">
import { computed, onBeforeUnmount, ref, watch } from 'vue'
import { RouterLink, RouterView } from 'vue-router'
import { useAuthStore } from './stores/authStore'
import { useTodoStore } from './stores/todoStore'
const authStore = useAuthStore()
const todoStore = useTodoStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)
const errorBanner = computed(() => authStore.errorMessage || todoStore.errorMessage)
const dismissTimer = ref<number | null>(null)

function clearErrors() {
  authStore.clearError()
  todoStore.clearError()
}

watch(errorBanner, (value) => {
  if (!value) return
  if (dismissTimer.value) {
    window.clearTimeout(dismissTimer.value)
  }
  dismissTimer.value = window.setTimeout(() => {
    clearErrors()
    dismissTimer.value = null
  }, 6000)
})

onBeforeUnmount(() => {
  if (dismissTimer.value) {
    window.clearTimeout(dismissTimer.value)
  }
})
</script>

<template>
  <main class="page">
    <div v-if="errorBanner" class="error-banner" role="alert">
      <span>{{ errorBanner }}</span>
      <button type="button" class="ghost" @click="clearErrors">Stäng</button>
    </div>
    <header class="card header-bar">
      <div>
        <p class="eyebrow">Todo App</p>
        <h1>Klart &amp; Tydligt</h1>
        <p class="subtitle">Håll koll på allt som behöver bli klart.</p>
      </div>
      <nav class="filters">
        <RouterLink v-if="isAuthenticated" class="ghost" to="/todos">Todos</RouterLink>
        <RouterLink v-if="isAuthenticated" class="ghost" to="/todos/new">Ny todo</RouterLink>
        <RouterLink v-if="!isAuthenticated" class="ghost" to="/login">Logga in</RouterLink>
        <RouterLink v-if="!isAuthenticated" class="ghost" to="/register">Registrera</RouterLink>
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
