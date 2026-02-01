import { computed, ref } from 'vue'
import { ApiError, apiFetch, getToken, setToken } from '../api/client'
import type { AuthPayload, AuthResponse } from '../models/todo'
import { useStatus } from './useStatus'
import { useTodos } from './useTodos'

const isBusy = ref(false)
const isAuthenticated = computed(() => Boolean(getToken()))

export function useAuth() {
  const { clearError, clearStatus, setError, setStatus } = useStatus()
  const { todos } = useTodos()

  async function handleAuth(payload: AuthPayload): Promise<boolean> {
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
      return true
    } catch (error) {
      if (error instanceof ApiError) {
        setStatus(error.status, error.statusText)
        setError(error.message)
      } else {
        setError(error instanceof Error ? error.message : 'Okänt fel.')
      }
      return false
    } finally {
      isBusy.value = false
    }
  }

  function logout() {
    setToken(null)
    todos.value = []
    clearStatus()
    clearError()
  }

  return {
    isBusy,
    isAuthenticated,
    handleAuth,
    logout,
  }
}
