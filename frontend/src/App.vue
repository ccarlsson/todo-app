<script setup lang="ts">
import AuthPanel from './components/AuthPanel.vue'
import HeroStats from './components/HeroStats.vue'
import TodoForm from './components/TodoForm.vue'
import TodoList from './components/TodoList.vue'
import { useStatus } from './composables/useStatus'

const { errorMessage } = useStatus()
</script>

<template>
  <main class="page">
    <HeroStats />

    <section class="grid">
      <AuthPanel />
      <TodoForm />
    </section>

    <TodoList />

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </main>
</template>
