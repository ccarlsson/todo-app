<script setup lang="ts">
import { computed, onBeforeUnmount, ref, watch } from 'vue'
import { RouterLink, RouterView } from 'vue-router'
import { useAuthStore } from './stores/authStore'
import { useTodoStore } from './stores/todoStore'
import { useI18n } from 'vue-i18n'
import { setLocale } from './i18n'
const authStore = useAuthStore()
const todoStore = useTodoStore()
const { t, locale } = useI18n()
const isAuthenticated = computed(() => authStore.isAuthenticated)
const errorBanner = computed(() => authStore.errorMessage || todoStore.errorMessage)
const dismissTimer = ref<number | null>(null)

function selectLocale(value: 'sv' | 'en') {
  setLocale(value)
}

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
      <button type="button" class="ghost" @click="clearErrors">{{ t('actions.close') }}</button>
    </div>
    <header class="card header-bar">
      <div>
        <p class="eyebrow">{{ t('app.eyebrow') }}</p>
        <h1>{{ t('app.title') }}</h1>
        <p class="subtitle">{{ t('app.tagline') }}</p>
      </div>
      <nav class="filters">
        <RouterLink v-if="isAuthenticated" class="ghost" to="/todos">{{ t('nav.todos') }}</RouterLink>
        <RouterLink v-if="isAuthenticated" class="ghost" to="/todos/new">{{ t('nav.newTodo') }}</RouterLink>
        <RouterLink v-if="!isAuthenticated" class="ghost" to="/login">{{ t('nav.login') }}</RouterLink>
        <RouterLink v-if="!isAuthenticated" class="ghost" to="/register">{{ t('nav.register') }}</RouterLink>
        <div class="mode-switch" aria-label="Language">
          <button type="button" :class="{ active: locale === 'sv' }" @click="selectLocale('sv')">
            SV
          </button>
          <button type="button" :class="{ active: locale === 'en' }" @click="selectLocale('en')">
            EN
          </button>
        </div>
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
