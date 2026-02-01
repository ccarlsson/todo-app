<script setup lang="ts">
import type { Priority, TodoResponse, TodoStatus } from '../models/todo'

const props = defineProps<{
  todos: TodoResponse[]
  isAuthenticated: boolean
  isBusy: boolean
  statusFilter: string
  priorityFilter: string
  sortBy: string
}>()

const emit = defineEmits<{
  (event: 'refresh'): void
  (event: 'updateStatus', todo: TodoResponse, status: TodoStatus): void
  (event: 'remove', todoId: string): void
  (event: 'update:statusFilter', value: string): void
  (event: 'update:priorityFilter', value: string): void
  (event: 'update:sortBy', value: string): void
}>()

function handleStatusChange(event: Event, todo: TodoResponse) {
  const value = (event.target as HTMLSelectElement).value as TodoStatus
  emit('updateStatus', todo, value)
}

function updateStatusFilter(event: Event) {
  emit('update:statusFilter', (event.target as HTMLSelectElement).value)
}

function updatePriorityFilter(event: Event) {
  emit('update:priorityFilter', (event.target as HTMLSelectElement).value)
}

function updateSortBy(event: Event) {
  emit('update:sortBy', (event.target as HTMLSelectElement).value)
}

const statusOptions: TodoStatus[] = ['NotStarted', 'InProgress', 'Completed']
const priorityOptions: Priority[] = ['Low', 'Medium', 'High']
</script>

<template>
  <section class="card">
    <header class="card-header">
      <h2>Todos</h2>
      <div class="filters">
        <select :value="statusFilter" @change="updateStatusFilter">
          <option value="">Alla status</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
        <select :value="priorityFilter" @change="updatePriorityFilter">
          <option value="">Alla prioriteringar</option>
          <option v-for="priority in priorityOptions" :key="priority" :value="priority">
            {{ priority }}
          </option>
        </select>
        <select :value="sortBy" @change="updateSortBy">
          <option value="createdAt">Sortera: Skapad</option>
          <option value="dueDate">Sortera: Förfallodatum</option>
        </select>
        <button type="button" class="ghost" @click="emit('refresh')" :disabled="isBusy">
          Uppdatera
        </button>
      </div>
    </header>

    <p v-if="!isAuthenticated" class="muted">Logga in för att se dina todos.</p>
    <p v-else-if="todos.length === 0" class="muted">Inga todos ännu.</p>

    <ul v-else class="todo-list">
      <li v-for="todo in props.todos" :key="todo.id" class="todo-item">
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
            <button type="button" class="danger" @click="emit('remove', todo.id)">
              Ta bort
            </button>
          </div>
        </div>
      </li>
    </ul>
  </section>
</template>
