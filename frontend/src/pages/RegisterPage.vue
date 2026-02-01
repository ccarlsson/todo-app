<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

const form = reactive({
  email: '',
  password: '',
})

async function handleRegister() {
  const ok = await authStore.registerAndLogin(form.email, form.password)
  if (ok) {
    await router.push('/todos')
  }
}
</script>

<template>
  <section class="card">
    <header class="card-header">
      <h2>Registrera konto</h2>
    </header>

    <form class="stack" @submit.prevent="handleRegister">
      <label>
        E‑post
        <input v-model="form.email" type="email" required />
      </label>
      <label>
        Lösenord
        <input v-model="form.password" type="password" required />
      </label>
      <button type="submit" :disabled="authStore.isBusy">Skapa konto</button>
      <p class="muted">Har du redan ett konto? <RouterLink to="/login">Logga in</RouterLink></p>
    </form>

    <p v-if="authStore.errorMessage" class="error">{{ authStore.errorMessage }}</p>
  </section>
</template>
