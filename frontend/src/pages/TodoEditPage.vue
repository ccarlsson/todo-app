<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useTodoStore } from '../stores/todoStore'
import type { Priority, TodoStatus } from '../models/todo'

const todoStore = useTodoStore()
const route = useRoute()
const router = useRouter()

const todoId = computed(() => route.params.id as string | undefined)
const isNew = computed(() => !todoId.value || todoId.value === 'new')
const isLoading = ref(false)

const form = reactive({
  title: '',
  description: '',
  dueDate: '',
  priority: 'Medium' as Priority,
  status: 'NotStarted' as TodoStatus,
})

const isBusy = computed(() => todoStore.isBusy)

async function loadTodo() {
  if (!todoId.value || isNew.value) return
  isLoading.value = true
  const todo = await todoStore.fetchTodo(todoId.value)
  if (todo) {
    form.title = todo.title
    form.description = todo.description ?? ''
    form.dueDate = todo.dueDate ? todo.dueDate.slice(0, 10) : ''
    form.priority = (todo.priority ?? 'Medium') as Priority
    form.status = todo.status
  }

  isLoading.value = false
}

async function handleSave() {
  if (isNew.value) {
    await todoStore.createTodo({
      title: form.title,
      description: form.description || null,
      dueDate: form.dueDate ? new Date(form.dueDate).toISOString() : null,
      priority: form.priority,
    })
  } else if (todoId.value) {
    await todoStore.updateTodo({
      id: todoId.value,
      title: form.title,
      description: form.description || null,
      dueDate: form.dueDate ? new Date(form.dueDate).toISOString() : null,
      priority: form.priority,
      status: form.status,
    })
  }

  await router.push('/todos')
}

onMounted(loadTodo)
</script>

<template>
  <section class="card">
    <header class="card-header">
      <div>
        <h2>{{ isNew ? 'Skapa todo' : 'Redigera todo' }}</h2>
        <p class="subtitle">Fyll i detaljerna nedan.</p>
      </div>
      <div class="filters">
        <RouterLink class="ghost" to="/todos">Tillbaka</RouterLink>
      </div>
    </header>

    <div v-if="isLoading" class="skeleton skeleton-card">
      <div class="skeleton-stack">
        <div class="skeleton-line large" style="width: 50%"></div>
        <div class="skeleton-line" style="width: 85%"></div>
        <div class="skeleton-line" style="width: 80%"></div>
        <div class="skeleton-line" style="width: 60%"></div>
        <div class="skeleton-line" style="width: 40%"></div>
      </div>
    </div>

    <form class="stack" @submit.prevent="handleSave">
      <label>
        Titel
        <input v-model="form.title" type="text" required />
      </label>
      <label>
        Beskrivning
        <textarea v-model="form.description" rows="4"></textarea>
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
      <label v-if="!isNew">
        Status
        <select v-model="form.status">
          <option value="NotStarted">NotStarted</option>
          <option value="InProgress">InProgress</option>
          <option value="Completed">Completed</option>
        </select>
      </label>

      <button type="submit" :disabled="isBusy">
        {{ isNew ? 'Skapa' : 'Spara' }}
      </button>
    </form>

    <p v-if="todoStore.errorMessage" class="error">{{ todoStore.errorMessage }}</p>
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

.filters {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
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
