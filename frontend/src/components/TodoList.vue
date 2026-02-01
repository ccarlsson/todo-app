<script setup lang="ts">
import { onMounted } from 'vue'
import type { Priority, TodoResponse, TodoStatus } from '../models/todo'
import { useAuth } from '../composables/useAuth'
import { useTodos } from '../composables/useTodos'

const { isAuthenticated } = useAuth()
const { todos, filters, isBusy, loadTodos, updateStatus, removeTodo } = useTodos()

function handleStatusChange(event: Event, todo: TodoResponse) {
  const value = (event.target as HTMLSelectElement).value as TodoStatus
  updateStatus(todo, value)
}

const statusOptions: TodoStatus[] = ['NotStarted', 'InProgress', 'Completed']
const priorityOptions: Priority[] = ['Low', 'Medium', 'High']

onMounted(() => {
  if (isAuthenticated.value) {
    loadTodos()
  }
})
</script>

<template>
  <section class="card">
    <header class="card-header">
      <h2>Todos</h2>
      <div class="filters">
        <select v-model="filters.status" @change="loadTodos">
          <option value="">Alla status</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
        <select v-model="filters.priority" @change="loadTodos">
          <option value="">Alla prioriteringar</option>
          <option v-for="priority in priorityOptions" :key="priority" :value="priority">
            {{ priority }}
          </option>
        </select>
        <select v-model="filters.sortBy" @change="loadTodos">
          <option value="createdAt">Sortera: Skapad</option>
          <option value="dueDate">Sortera: Förfallodatum</option>
        </select>
        <button type="button" class="ghost" @click="loadTodos" :disabled="isBusy">
          Uppdatera
        </button>
      </div>
    </header>

    <p v-if="!isAuthenticated" class="muted">Logga in för att se dina todos.</p>
    <p v-else-if="todos.length === 0" class="muted">Inga todos ännu.</p>

    <ul v-else class="todo-list">
      <li v-for="todo in todos" :key="todo.id" class="todo-item">
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
            <select :value="todo.status" @change="handleStatusChange($event, todo)">
              <option v-for="status in statusOptions" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
            <button type="button" class="danger" @click="removeTodo(todo.id)">
              Ta bort
            </button>
          </div>
        </div>
      </li>
    </ul>
  </section>
</template>
