<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useTodoStore } from '../stores/todoStore'
import type { Priority, TodoStatus } from '../models/todo'
import { useI18n } from 'vue-i18n'

const todoStore = useTodoStore()
const route = useRoute()
const router = useRouter()
const { t } = useI18n()

const todoId = computed(() => route.params.id as string | undefined)
const isNew = computed(() => !todoId.value || todoId.value === 'new')
const isLoading = ref(false)
const errors = ref<{ title?: string }>({})
const touchedTitle = ref(false)

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
  touchedTitle.value = true
  const next: { title?: string } = {}
  if (!form.title.trim()) {
    next.title = t('validation.titleRequired')
  }
  errors.value = next
  if (Object.keys(next).length > 0) return

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

watch(
  () => form.title,
  () => {
    if (!touchedTitle.value) return
    if (!form.title.trim()) {
      errors.value = { title: t('validation.titleRequired') }
    } else {
      errors.value = {}
    }
  },
)
</script>

<template>
  <section class="card">
    <header class="card-header">
      <div>
        <h2>{{ isNew ? t('todos.createTitle') : t('todos.editTitle') }}</h2>
        <p class="subtitle">{{ t('todos.formHint') }}</p>
      </div>
      <div class="filters">
        <RouterLink class="ghost" to="/todos">{{ t('actions.back') }}</RouterLink>
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
        {{ t('todos.fields.title') }}
        <input
          v-model="form.title"
          type="text"
          required
          :class="{ invalid: touchedTitle && errors.title }"
          @input="touchedTitle = true"
          @blur="touchedTitle = true"
        />
        <span v-if="touchedTitle && errors.title" class="field-error">{{ errors.title }}</span>
      </label>
      <label>
        {{ t('todos.fields.description') }}
        <textarea v-model="form.description" rows="4"></textarea>
      </label>
      <label>
        {{ t('todos.fields.dueDate') }}
        <input v-model="form.dueDate" type="date" />
      </label>
      <label>
        {{ t('todos.fields.priority') }}
        <select v-model="form.priority">
          <option value="Low">{{ t('todos.priority.low') }}</option>
          <option value="Medium">{{ t('todos.priority.medium') }}</option>
          <option value="High">{{ t('todos.priority.high') }}</option>
        </select>
      </label>
      <label v-if="!isNew">
        {{ t('todos.fields.status') }}
        <select v-model="form.status">
          <option value="NotStarted">{{ t('todos.status.notStarted') }}</option>
          <option value="InProgress">{{ t('todos.status.inProgress') }}</option>
          <option value="Completed">{{ t('todos.status.completed') }}</option>
        </select>
      </label>

      <button type="submit" :disabled="isBusy">
        {{ isNew ? t('actions.create') : t('actions.save') }}
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
