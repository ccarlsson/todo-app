<script setup lang="ts">
import { computed, reactive } from 'vue'
import type { Priority } from '../models/todo'
import { useAuth } from '../composables/useAuth'
import { useTodos } from '../composables/useTodos'

const form = reactive({
  title: '',
  description: '',
  dueDate: '',
  priority: 'Medium' as Priority,
})

const { isAuthenticated } = useAuth()
const { isBusy, createTodo } = useTodos()

const isDisabled = computed(() => !isAuthenticated.value || isBusy.value)

async function handleSubmit() {
  await createTodo({
    title: form.title,
    description: form.description || null,
    dueDate: form.dueDate ? new Date(form.dueDate).toISOString() : null,
    priority: form.priority,
  })

  form.title = ''
  form.description = ''
  form.dueDate = ''
  form.priority = 'Medium'
}
</script>

<template>
  <article class="card">
    <header class="card-header">
      <h2>Ny todo</h2>
    </header>
    <form class="stack" @submit.prevent="handleSubmit">
      <label>
        Titel
        <input v-model="form.title" type="text" required />
      </label>
      <label>
        Beskrivning
        <textarea v-model="form.description" rows="3"></textarea>
      </label>
      <label>
        Förfallodatum
        <input v-model="form.dueDate" type="date" />
      </label>
      <label>
        Prioritet
        <select v-model="form.priority">
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>
      </label>
      <button type="submit" :disabled="isDisabled">
        Skapa
      </button>
    </form>
  </article>
</template>
