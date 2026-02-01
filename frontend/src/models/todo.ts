export type Priority = 'Low' | 'Medium' | 'High'
export type TodoStatus = 'NotStarted' | 'InProgress' | 'Completed'

export interface AuthResponse {
  token: string
  expiresIn: number
}

export interface TodoResponse {
  id: string
  title: string
  description?: string | null
  dueDate?: string | null
  priority?: Priority | null
  status: TodoStatus
  createdAt: string
  updatedAt: string
}

export interface AuthPayload {
  mode: 'login' | 'register'
  email: string
  password: string
}

export interface TodoCreatePayload {
  title: string
  description?: string | null
  dueDate?: string | null
  priority?: Priority | null
}
