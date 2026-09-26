import type { Todo, TodoStatus } from '../types';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5080';

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;

  try {
    response = await fetch(`${API_URL}${path}`, {
      headers: { 'Content-Type': 'application/json' },
      ...init,
    });
  } catch {
    throw new Error('Não foi possível conectar à API.');
  }

  if (!response.ok) {
    // The backend reports errors as RFC 7807 problem details, and "detail"
    // carries the message meant for the user (for example, the title validation).
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail ?? 'Não foi possível concluir a operação.');
  }

  return response.json() as Promise<T>;
}

export const todoApi = {
  list: () => request<Todo[]>('/api/todos'),

  create: (title: string, description: string) =>
    request<Todo>('/api/todos', {
      method: 'POST',
      body: JSON.stringify({ title, description }),
    }),

  changeStatus: (id: string, status: TodoStatus) =>
    request<Todo>(`/api/todos/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ status }),
    }),
};
