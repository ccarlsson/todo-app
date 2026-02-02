<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTodoStore } from '../stores/todoStore'
import { useAuthStore } from '../stores/authStore'

const todoStore = useTodoStore()
const authStore = useAuthStore()
const router = useRouter()

const stats = computed(() => ({
  total: todoStore.todos.length,
  open: todoStore.todos.filter((t) => t.status !== 'Completed').length,
  done: todoStore.todos.filter((t) => t.status === 'Completed').length,
}))

onMounted(() => {
  todoStore.loadTodos()
})

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <section class="card">
    <header class="card-header">
      <div>
        <h2>Dina todos</h2>
        <p class="subtitle">Totalt: {{ stats.total }} · Öppna: {{ stats.open }} · Klara: {{ stats.done }}</p>
      </div>
      <div class="filters">
        <RouterLink v-if="authStore.isAuthenticated" class="ghost" to="/todos/new">Skapa</RouterLink>
        <button
          v-if="authStore.isAuthenticated"
          type="button"
          class="ghost"
          @click="todoStore.loadTodos"
          :disabled="todoStore.isBusy"
        >
          Uppdatera
        </button>
        <button v-if="authStore.isAuthenticated" type="button" class="ghost" @click="handleLogout">Logga ut</button>
      </div>
    </header>

    <div class="filters">
      <select v-model="todoStore.filters.status" @change="todoStore.loadTodos">
        <option value="">Alla status</option>
        <option value="NotStarted">NotStarted</option>
        <option value="InProgress">InProgress</option>
        <option value="Completed">Completed</option>
      </select>
      <select v-model="todoStore.filters.priority" @change="todoStore.loadTodos">
        <option value="">Alla prioriteringar</option>
        <option value="Low">Low</option>
        <option value="Medium">Medium</option>
        <option value="High">High</option>
      </select>
      <select v-model="todoStore.filters.dueDate" @change="todoStore.loadTodos">
        <option value="">Alla förfallodatum</option>
        <option value="upcoming">Kommande</option>
        <option value="overdue">Förfallna</option>
      </select>
      <select v-model="todoStore.filters.sortBy" @change="todoStore.loadTodos">
        <option value="createdAt">Sortera: Skapad</option>
        <option value="dueDate">Sortera: Förfallodatum</option>
      </select>
    </div>

    <p v-if="todoStore.todos.length === 0" class="muted">Inga todos ännu.</p>

    <ul v-else class="todo-list">
      <li v-for="todo in todoStore.todos" :key="todo.id" class="todo-item">
        <div class="todo-main">
          <div>
            <h3>{{ todo.title }}</h3>
            <p v-if="todo.description">{{ todo.description }}</p>
            <div class="meta">
              <span>Status: {{ todo.status }}</span>
              <span>Prioritet: {{ todo.priority ?? '-' }}</span>
              <span>Förfallo: {{ todo.dueDate ? todo.dueDate.slice(0, 10) : '-' }}</span>
            </div>
          </div>
          <div class="actions">
            <select :value="todo.status" @change="todoStore.updateStatus(todo, ($event.target as HTMLSelectElement).value as any)">
              <option value="NotStarted">NotStarted</option>
              <option value="InProgress">InProgress</option>
              <option value="Completed">Completed</option>
            </select>
            <RouterLink class="ghost" :to="`/todos/${todo.id}`">Redigera</RouterLink>
            <button type="button" class="danger" @click="todoStore.deleteTodo(todo.id)">Ta bort</button>
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
