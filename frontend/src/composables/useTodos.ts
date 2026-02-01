import { computed, reactive, ref } from 'vue'
import { ApiError, apiFetch } from '../api/client'
import type { TodoCreatePayload, TodoResponse, TodoStatus } from '../models/todo'
import { useStatus } from './useStatus'

const todos = ref<TodoResponse[]>([])
const filters = reactive({
  status: '',
  priority: '',
  sortBy: 'createdAt',
})
const isBusy = ref(false)

export function useTodos() {
  const { clearError, setError, setStatus } = useStatus()

  const stats = computed(() => ({
    total: todos.value.length,
    open: todos.value.filter((t) => t.status !== 'Completed').length,
    done: todos.value.filter((t) => t.status === 'Completed').length,
  }))

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
        setError(error.message)
      } else {
        setError(error instanceof Error ? error.message : 'Kunde inte hämta todos.')
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
        setError(error.message)
      } else {
        setError(error instanceof Error ? error.message : 'Kunde inte skapa todo.')
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
        setError(error.message)
      } else {
        setError(error instanceof Error ? error.message : 'Kunde inte uppdatera todo.')
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
        setError(error.message)
      } else {
        setError(error instanceof Error ? error.message : 'Kunde inte ta bort todo.')
      }
    } finally {
      isBusy.value = false
    }
  }

  return {
    todos,
    filters,
    stats,
    isBusy,
    loadTodos,
    createTodo,
    updateStatus,
    removeTodo,
  }
}
