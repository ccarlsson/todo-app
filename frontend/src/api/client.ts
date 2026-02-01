const API_BASE = import.meta.env.VITE_API_BASE ?? 'http://localhost:5134';

export interface ApiResult<T> {
  data: T;
  status: number;
  statusText: string;
}

export class ApiError extends Error {
  status: number;
  statusText: string;

  constructor(status: number, statusText: string, message: string) {
    super(message);
    this.status = status;
    this.statusText = statusText;
  }
}

export function getApiBase(): string {
  return API_BASE;
}

export function getToken(): string | null {
  return localStorage.getItem('todo_token');
}

export function setToken(token: string | null): void {
  if (token) {
    localStorage.setItem('todo_token', token);
  } else {
    localStorage.removeItem('todo_token');
  }
}

export async function apiFetch<T>(
  path: string,
  options: RequestInit = {}
): Promise<ApiResult<T>> {
  const headers = new Headers(options.headers ?? {});
  headers.set('Content-Type', 'application/json');

  const token = getToken();
  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }

  const response = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new ApiError(
      response.status,
      response.statusText,
      errorText || response.statusText
    );
  }

  if (response.status === 204) {
    return { data: null as T, status: response.status, statusText: response.statusText };
  }

  const data = (await response.json()) as T;
  return { data, status: response.status, statusText: response.statusText };
}
