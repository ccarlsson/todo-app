import { defineStore } from 'pinia'
import { ApiError, apiFetch } from '../api/client'
import type { TodoCreatePayload, TodoResponse, TodoStatus, TodoUpdatePayload } from '../models/todo'

interface TodoState {
  todos: TodoResponse[]
  isBusy: boolean
  errorMessage: string
  lastStatus: string
  filters: {
    status: string
    priority: string
    sortBy: string
  }
}

export const useTodoStore = defineStore('todos', {
  state: (): TodoState => ({
    todos: [],
    isBusy: false,
    errorMessage: '',
    lastStatus: '',
    filters: {
      status: '',
      priority: '',
      sortBy: 'createdAt',
    },
  }),
  actions: {
    clearError() {
      this.errorMessage = ''
    },
    setStatus(status: number, statusText: string) {
      this.lastStatus = `${status} ${statusText}`.trim()
    },
    async loadTodos() {
      this.clearError()
      this.isBusy = true

      try {
        const query = new URLSearchParams()
        if (this.filters.status) query.set('status', this.filters.status)
        if (this.filters.priority) query.set('priority', this.filters.priority)
        if (this.filters.sortBy) query.set('sortBy', this.filters.sortBy)

        const result = await apiFetch<TodoResponse[]>(`/todos?${query.toString()}`)
        this.setStatus(result.status, result.statusText)
        this.todos = result.data
      } catch (error) {
        if (error instanceof ApiError) {
          this.setStatus(error.status, error.statusText)
          this.errorMessage = error.message
        } else {
          this.errorMessage = error instanceof Error ? error.message : 'Kunde inte hämta todos.'
        }
      } finally {
        this.isBusy = false
      }
    },
    async fetchTodo(id: string) {
      this.clearError()
      this.isBusy = true

      try {
        const result = await apiFetch<TodoResponse>(`/todos/${id}`)
        this.setStatus(result.status, result.statusText)
        return result.data
      } catch (error) {
        if (error instanceof ApiError) {
          this.setStatus(error.status, error.statusText)
          this.errorMessage = error.message
        } else {
          this.errorMessage = error instanceof Error ? error.message : 'Kunde inte hämta todo.'
        }
        return null
      } finally {
        this.isBusy = false
      }
    },
    async createTodo(payload: TodoCreatePayload) {
      this.clearError()
      this.isBusy = true

      try {
        const result = await apiFetch('/todos', {
          method: 'POST',
          body: JSON.stringify(payload),
        })
        this.setStatus(result.status, result.statusText)
        await this.loadTodos()
        return true
      } catch (error) {
        if (error instanceof ApiError) {
          this.setStatus(error.status, error.statusText)
          this.errorMessage = error.message
        } else {
          this.errorMessage = error instanceof Error ? error.message : 'Kunde inte skapa todo.'
        }
        return false
      } finally {
        this.isBusy = false
      }
    },
    async updateTodo(payload: TodoUpdatePayload) {
      this.clearError()
      this.isBusy = true

      try {
        const result = await apiFetch(`/todos/${payload.id}`, {
          method: 'PUT',
          body: JSON.stringify(payload),
        })
        this.setStatus(result.status, result.statusText)
        await this.loadTodos()
        return true
      } catch (error) {
        if (error instanceof ApiError) {
          this.setStatus(error.status, error.statusText)
          this.errorMessage = error.message
        } else {
          this.errorMessage = error instanceof Error ? error.message : 'Kunde inte uppdatera todo.'
        }
        return false
      } finally {
        this.isBusy = false
      }
    },
    async deleteTodo(id: string) {
      this.clearError()
      this.isBusy = true

      try {
        const result = await apiFetch(`/todos/${id}`, {
          method: 'DELETE',
        })
        this.setStatus(result.status, result.statusText)
        await this.loadTodos()
        return true
      } catch (error) {
        if (error instanceof ApiError) {
          this.setStatus(error.status, error.statusText)
          this.errorMessage = error.message
        } else {
          this.errorMessage = error instanceof Error ? error.message : 'Kunde inte ta bort todo.'
        }
        return false
      } finally {
        this.isBusy = false
      }
    },
    updateStatus(todo: TodoResponse, status: TodoStatus) {
      return this.updateTodo({
        id: todo.id,
        title: todo.title,
        description: todo.description ?? null,
        dueDate: todo.dueDate ?? null,
        priority: todo.priority ?? null,
        status,
      })
    },
    reset() {
      this.todos = []
      this.lastStatus = ''
      this.clearError()
    },
  },
})
