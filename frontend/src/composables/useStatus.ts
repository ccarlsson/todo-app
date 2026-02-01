import { ref } from 'vue'

const lastStatus = ref('')
const errorMessage = ref('')

function setStatus(status: number, statusText: string) {
  lastStatus.value = `${status} ${statusText}`.trim()
}

function setError(message: string) {
  errorMessage.value = message
}

function clearError() {
  errorMessage.value = ''
}

function clearStatus() {
  lastStatus.value = ''
}

export function useStatus() {
  return {
    lastStatus,
    errorMessage,
    setStatus,
    setError,
    clearError,
    clearStatus,
  }
}
