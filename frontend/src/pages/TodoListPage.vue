<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTodoStore } from '../stores/todoStore'
import { useAuthStore } from '../stores/authStore'
import { useI18n } from 'vue-i18n'

const todoStore = useTodoStore()
const authStore = useAuthStore()
const router = useRouter()
const { t } = useI18n()

const stats = computed(() => ({
  total: todoStore.todos.length,
  open: todoStore.todos.filter((t) => t.status !== 'Completed').length,
  done: todoStore.todos.filter((t) => t.status === 'Completed').length,
}))

onMounted(() => {
  todoStore.loadTodos()
})

function statusLabel(value: string) {
  switch (value) {
    case 'NotStarted':
      return t('todos.status.notStarted')
    case 'InProgress':
      return t('todos.status.inProgress')
    case 'Completed':
      return t('todos.status.completed')
    default:
      return value
  }
}

function priorityLabel(value: string | null | undefined) {
  switch (value) {
    case 'Low':
      return t('todos.priority.low')
    case 'Medium':
      return t('todos.priority.medium')
    case 'High':
      return t('todos.priority.high')
    default:
      return '-'
  }
}

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <section class="card">
    <header class="card-header">
      <div>
        <h2>{{ t('todos.title') }}</h2>
        <p class="subtitle">{{ t('todos.stats', stats) }}</p>
      </div>
      <div class="filters">
        <RouterLink v-if="authStore.isAuthenticated" class="ghost" to="/todos/new">{{ t('actions.create') }}</RouterLink>
        <button
          v-if="authStore.isAuthenticated"
          type="button"
          class="ghost"
          @click="todoStore.loadTodos"
          :disabled="todoStore.isBusy"
        >
          {{ t('actions.update') }}
        </button>
        <button v-if="authStore.isAuthenticated" type="button" class="ghost" @click="handleLogout">{{ t('actions.logout') }}</button>
      </div>
    </header>

    <div class="filters">
      <select v-model="todoStore.filters.status" @change="todoStore.loadTodos">
        <option value="">{{ t('todos.status.all') }}</option>
        <option value="NotStarted">{{ t('todos.status.notStarted') }}</option>
        <option value="InProgress">{{ t('todos.status.inProgress') }}</option>
        <option value="Completed">{{ t('todos.status.completed') }}</option>
      </select>
      <select v-model="todoStore.filters.priority" @change="todoStore.loadTodos">
        <option value="">{{ t('todos.priority.all') }}</option>
        <option value="Low">{{ t('todos.priority.low') }}</option>
        <option value="Medium">{{ t('todos.priority.medium') }}</option>
        <option value="High">{{ t('todos.priority.high') }}</option>
      </select>
      <select v-model="todoStore.filters.dueDate" @change="todoStore.loadTodos">
        <option value="">{{ t('todos.dueFilter.all') }}</option>
        <option value="upcoming">{{ t('todos.dueFilter.upcoming') }}</option>
        <option value="overdue">{{ t('todos.dueFilter.overdue') }}</option>
      </select>
      <select v-model="todoStore.filters.sortBy" @change="todoStore.loadTodos">
        <option value="createdAt">{{ t('todos.sort.createdAt') }}</option>
        <option value="dueDate">{{ t('todos.sort.dueDate') }}</option>
      </select>
    </div>

    <div v-if="todoStore.isBusy" class="todo-list">
      <div v-for="n in 3" :key="n" class="skeleton skeleton-card">
        <div class="skeleton-stack">
          <div class="skeleton-line large" style="width: 45%"></div>
          <div class="skeleton-line" style="width: 70%"></div>
          <div class="skeleton-line" style="width: 55%"></div>
        </div>
      </div>
    </div>

    <p v-else-if="todoStore.todos.length === 0" class="empty-state">{{ t('todos.empty') }}</p>

    <ul v-else class="todo-list">
      <li v-for="todo in todoStore.todos" :key="todo.id" class="todo-item">
        <div class="todo-main">
          <div>
            <h3>{{ todo.title }}</h3>
            <p v-if="todo.description">{{ todo.description }}</p>
            <div class="meta">
              <span>{{ t('todos.statusLabel') }}: {{ statusLabel(todo.status) }}</span>
              <span>{{ t('todos.priorityLabel') }}: {{ priorityLabel(todo.priority) }}</span>
              <span>{{ t('todos.dueLabel') }}: {{ todo.dueDate ? todo.dueDate.slice(0, 10) : '-' }}</span>
            </div>
          </div>
          <div class="actions">
            <select :value="todo.status" @change="todoStore.updateStatus(todo, ($event.target as HTMLSelectElement).value as any)">
              <option value="NotStarted">{{ t('todos.status.notStarted') }}</option>
              <option value="InProgress">{{ t('todos.status.inProgress') }}</option>
              <option value="Completed">{{ t('todos.status.completed') }}</option>
            </select>
            <RouterLink class="ghost" :to="`/todos/${todo.id}`">{{ t('todos.editTitle') }}</RouterLink>
            <button type="button" class="danger" @click="todoStore.deleteTodo(todo.id)">{{ t('actions.delete') }}</button>
          </div>
        </div>
      </li>
    </ul>

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

.todo-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.todo-item {
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 1rem 1.25rem;
  background: var(--surface-strong);
}

.todo-main {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  gap: 1rem;
}

.meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  font-size: 0.85rem;
  color: var(--text-muted);
}

.actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}
</style>
