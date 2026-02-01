<script setup lang="ts">
import { reactive, ref } from 'vue'
import type { AuthPayload } from '../models/todo'

defineProps<{
  isAuthenticated: boolean
  isBusy: boolean
}>()

const emit = defineEmits<{
  (event: 'submit', payload: AuthPayload): void
  (event: 'logout'): void
}>()

const mode = ref<'login' | 'register'>('login')
const form = reactive({
  email: '',
  password: '',
})

function handleSubmit() {
  emit('submit', {
    mode: mode.value,
    email: form.email,
    password: form.password,
  })
}
</script>

<template>
  <article class="card">
    <header class="card-header">
      <h2>Autentisering</h2>
      <div class="mode-switch">
        <button
          type="button"
          :class="{ active: mode === 'login' }"
          @click="mode = 'login'"
        >
          Logga in
        </button>
        <button
          type="button"
          :class="{ active: mode === 'register' }"
          @click="mode = 'register'"
        >
          Registrera
        </button>
      </div>
    </header>

    <form class="stack" @submit.prevent="handleSubmit">
      <label>
        E‑post
        <input v-model="form.email" type="email" required />
      </label>
      <label>
        Lösenord
        <input v-model="form.password" type="password" required />
      </label>
      <button type="submit" :disabled="isBusy">
        {{ mode === 'login' ? 'Logga in' : 'Skapa konto' }}
      </button>
      <button v-if="isAuthenticated" type="button" class="ghost" @click="emit('logout')">
        Logga ut
      </button>
    </form>
  </article>
</template>
