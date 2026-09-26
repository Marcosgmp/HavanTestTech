import type { TodoStatus } from '../types';

export const STATUS_LABELS: Record<TodoStatus, string> = {
  Pending: 'Pendente',
  InProgress: 'Em andamento',
  Completed: 'Concluída',
};

export const STATUS_COLORS: Record<TodoStatus, string> = {
  Pending: '#f59e0b',
  InProgress: '#3b82f6',
  Completed: '#10b981',
};

// Each status advances to the next one. A completed task has no next step,
// which mirrors the backend rule that forbids changing it.
export const NEXT_STATUS: Record<TodoStatus, TodoStatus | null> = {
  Pending: 'InProgress',
  InProgress: 'Completed',
  Completed: null,
};
