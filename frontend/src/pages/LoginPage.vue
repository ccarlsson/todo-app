<script setup lang="ts">
import { reactive } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()

const form = reactive({
  email: '',
  password: '',
})

async function handleLogin() {
  const ok = await authStore.login(form.email, form.password)
  if (ok) {
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/todos'
    await router.push(redirect)
  }
}
</script>

<template>
  <section class="card">
    <header class="card-header">
      <h2>Logga in</h2>
    </header>

    <form class="stack" @submit.prevent="handleLogin">
      <label>
        E‑post
        <input v-model="form.email" type="email" required />
      </label>
      <label>
        Lösenord
        <input v-model="form.password" type="password" required />
      </label>
      <button type="submit" :disabled="authStore.isBusy">Logga in</button>
      <p class="muted">Inget konto? <RouterLink to="/register">Registrera dig</RouterLink></p>
    </form>

    <p v-if="authStore.errorMessage" class="error">{{ authStore.errorMessage }}</p>
  </section>
</template>
