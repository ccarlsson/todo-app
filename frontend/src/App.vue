<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { ApiError, apiFetch, getApiBase, getToken, setToken } from './api/client'
import AuthPanel from './components/AuthPanel.vue'
import HeroStats from './components/HeroStats.vue'
import TodoForm from './components/TodoForm.vue'
import TodoList from './components/TodoList.vue'
import type {
  AuthPayload,
  AuthResponse,
  TodoCreatePayload,
  TodoResponse,
  TodoStatus,
} from './models/todo'

const apiBase = getApiBase()
const errorMessage = ref('')
const lastStatus = ref('')
const isBusy = ref(false)

const isAuthenticated = computed(() => Boolean(getToken()))

const todos = ref<TodoResponse[]>([])
const filters = reactive({
  status: '',
  priority: '',
  sortBy: 'createdAt',
})

const todoStats = computed(() => ({
  total: todos.value.length,
  open: todos.value.filter((t) => t.status !== 'Completed').length,
  done: todos.value.filter((t) => t.status === 'Completed').length,
}))

function clearError() {
  errorMessage.value = ''
}

function setStatus(status: number, statusText: string) {
  lastStatus.value = `${status} ${statusText}`.trim()
}

async function handleAuth(payload: AuthPayload) {
  clearError()
  isBusy.value = true

  try {
    if (payload.mode === 'register') {
      const registerResult = await apiFetch('/auth/register', {
        method: 'POST',
        body: JSON.stringify({
          email: payload.email,
          password: payload.password,
        }),
      })
      setStatus(registerResult.status, registerResult.statusText)
    }

    const authResult = await apiFetch<AuthResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({
        email: payload.email,
        password: payload.password,
      }),
    })

    setStatus(authResult.status, authResult.statusText)
    setToken(authResult.data.token)
    await loadTodos()
  } catch (error) {
    if (error instanceof ApiError) {
      setStatus(error.status, error.statusText)
      errorMessage.value = error.message
    } else {
      errorMessage.value = error instanceof Error ? error.message : 'Okänt fel.'
    }
  } finally {
    isBusy.value = false
  }
}

async function loadTodos() {
  clearError()
  isBusy.value = true

  try {
    const query = new URLSearchParams()
    if (filters.status) query.set('status', filters.status)
    if (filters.priority) query.set('priority', filters.priority)
    if (filters.sortBy) query.set('sortBy', filters.sortBy)

    const result = await apiFetch<TodoResponse[]>(`/todos?${query.toString()}`)
    setStatus(result.status, result.statusText)
    todos.value = result.data
  } catch (error) {
    if (error instanceof ApiError) {
      setStatus(error.status, error.statusText)
      errorMessage.value = error.message
    } else {
      errorMessage.value = error instanceof Error ? error.message : 'Kunde inte hämta todos.'
    }
  } finally {
    isBusy.value = false
  }
}

async function createTodo(payload: TodoCreatePayload) {
  clearError()
  isBusy.value = true

  try {
    const result = await apiFetch('/todos', {
      method: 'POST',
      body: JSON.stringify(payload),
    })
    setStatus(result.status, result.statusText)

    await loadTodos()
  } catch (error) {
    if (error instanceof ApiError) {
      setStatus(error.status, error.statusText)
      errorMessage.value = error.message
    } else {
      errorMessage.value = error instanceof Error ? error.message : 'Kunde inte skapa todo.'
    }
  } finally {
    isBusy.value = false
  }
}

async function updateStatus(todo: TodoResponse, status: TodoStatus) {
  clearError()
  isBusy.value = true

  try {
    const result = await apiFetch(`/todos/${todo.id}`, {
      method: 'PUT',
      body: JSON.stringify({
        title: todo.title,
        description: todo.description ?? null,
        dueDate: todo.dueDate ?? null,
        priority: todo.priority ?? null,
        status,
      }),
    })
    setStatus(result.status, result.statusText)
    await loadTodos()
  } catch (error) {
    if (error instanceof ApiError) {
      setStatus(error.status, error.statusText)
      errorMessage.value = error.message
    } else {
      errorMessage.value = error instanceof Error ? error.message : 'Kunde inte uppdatera todo.'
    }
  } finally {
    isBusy.value = false
  }
}

async function removeTodo(todoId: string) {
  clearError()
  isBusy.value = true

  try {
    const result = await apiFetch(`/todos/${todoId}`, {
      method: 'DELETE',
    })
    setStatus(result.status, result.statusText)
    await loadTodos()
  } catch (error) {
    if (error instanceof ApiError) {
      setStatus(error.status, error.statusText)
      errorMessage.value = error.message
    } else {
      errorMessage.value = error instanceof Error ? error.message : 'Kunde inte ta bort todo.'
    }
  } finally {
    isBusy.value = false
  }
}

function logout() {
  setToken(null)
  todos.value = []
  lastStatus.value = ''
}

onMounted(() => {
  if (isAuthenticated.value) {
    loadTodos()
  }
})
</script>

<template>
  <main class="page">
    <HeroStats :api-base="apiBase" :stats="todoStats" :last-status="lastStatus" />

    <section class="grid">
      <AuthPanel
        :is-authenticated="isAuthenticated"
        :is-busy="isBusy"
        @submit="handleAuth"
        @logout="logout"
      />

      <TodoForm :is-authenticated="isAuthenticated" :is-busy="isBusy" @create="createTodo" />
    </section>

    <TodoList
      :todos="todos"
      :is-authenticated="isAuthenticated"
      :is-busy="isBusy"
      :status-filter="filters.status"
      :priority-filter="filters.priority"
      :sort-by="filters.sortBy"
      @refresh="loadTodos"
      @update-status="updateStatus"
      @remove="removeTodo"
      @update:status-filter="filters.status = $event"
      @update:priority-filter="filters.priority = $event"
      @update:sort-by="filters.sortBy = $event"
    />

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </main>
</template>
