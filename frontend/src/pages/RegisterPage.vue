<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

const form = reactive({
  email: '',
  password: '',
})

const errors = ref<{ email?: string; password?: string }>({})
const touched = reactive({
  email: false,
  password: false,
})

function validate() {
  const next: { email?: string; password?: string } = {}
  if (!form.email.trim()) {
    next.email = 'E-post är obligatoriskt.'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    next.email = 'Ange en giltig e-postadress.'
  }

  if (!form.password.trim()) {
    next.password = 'Lösenord är obligatoriskt.'
  } else if (form.password.length < 6) {
    next.password = 'Lösenord måste vara minst 6 tecken.'
  }

  errors.value = next
  return Object.keys(next).length === 0
}

watch(
  () => [form.email, form.password],
  () => {
    if (touched.email || touched.password) {
      validate()
    }
  },
)

async function handleRegister() {
  touched.email = true
  touched.password = true
  if (!validate()) return

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

    <div v-if="authStore.isBusy" class="skeleton skeleton-card">
      <div class="skeleton-stack">
        <div class="skeleton-line large" style="width: 45%"></div>
        <div class="skeleton-line" style="width: 80%"></div>
        <div class="skeleton-line" style="width: 70%"></div>
        <div class="skeleton-line" style="width: 50%"></div>
      </div>
    </div>

    <form v-else class="stack" @submit.prevent="handleRegister">
      <label>
        E‑post
        <input
          v-model="form.email"
          type="email"
          required
          :class="{ invalid: touched.email && errors.email }"
          @input="touched.email = true"
          @blur="touched.email = true"
        />
        <span v-if="touched.email && errors.email" class="field-error">{{ errors.email }}</span>
      </label>
      <label>
        Lösenord
        <input
          v-model="form.password"
          type="password"
          required
          :class="{ invalid: touched.password && errors.password }"
          @input="touched.password = true"
          @blur="touched.password = true"
        />
        <span v-if="touched.password && errors.password" class="field-error">{{ errors.password }}</span>
      </label>
      <button type="submit" :disabled="authStore.isBusy">Skapa konto</button>
      <p class="muted">Har du redan ett konto? <RouterLink to="/login">Logga in</RouterLink></p>
    </form>

    <p v-if="authStore.errorMessage" class="error">{{ authStore.errorMessage }}</p>
  </section>
</template>

<style scoped>
.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1.25rem;
}

.stack {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.stack label {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  font-size: 0.9rem;
  color: var(--text-muted);
}
</style>
