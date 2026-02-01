import { defineStore } from 'pinia'
import { ApiError, apiFetch, getToken, setToken } from '../api/client'
import type { AuthResponse } from '../models/todo'

interface AuthState {
  token: string | null
  isBusy: boolean
  errorMessage: string
  lastStatus: string
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: getToken(),
    isBusy: false,
    errorMessage: '',
    lastStatus: '',
  }),
  getters: {
    isAuthenticated: (state) => Boolean(state.token),
  },
  actions: {
    clearError() {
      this.errorMessage = ''
    },
    setStatus(status: number, statusText: string) {
      this.lastStatus = `${status} ${statusText}`.trim()
    },
    async register(email: string, password: string) {
      this.clearError()
      this.isBusy = true

      try {
        const result = await apiFetch('/auth/register', {
          method: 'POST',
          body: JSON.stringify({ email, password }),
        })
        this.setStatus(result.status, result.statusText)
        return true
      } catch (error) {
        if (error instanceof ApiError) {
          this.setStatus(error.status, error.statusText)
          this.errorMessage = error.message
        } else {
          this.errorMessage = error instanceof Error ? error.message : 'Okänt fel.'
        }
        return false
      } finally {
        this.isBusy = false
      }
    },
    async login(email: string, password: string) {
      this.clearError()
      this.isBusy = true

      try {
        const result = await apiFetch<AuthResponse>('/auth/login', {
          method: 'POST',
          body: JSON.stringify({ email, password }),
        })
        this.setStatus(result.status, result.statusText)
        this.token = result.data.token
        setToken(result.data.token)
        return true
      } catch (error) {
        if (error instanceof ApiError) {
          this.setStatus(error.status, error.statusText)
          this.errorMessage = error.message
        } else {
          this.errorMessage = error instanceof Error ? error.message : 'Okänt fel.'
        }
        return false
      } finally {
        this.isBusy = false
      }
    },
    async registerAndLogin(email: string, password: string) {
      const registered = await this.register(email, password)
      if (!registered) {
        return false
      }
      return await this.login(email, password)
    },
    logout() {
      this.token = null
      setToken(null)
      this.lastStatus = ''
      this.clearError()
    },
  },
})
