export type TodoStatus = 'Pending' | 'InProgress' | 'Completed';

export interface Todo {
  id: string;
  title: string;
  description: string;
  createdAt: string;
  completedAt: string | null;
  status: TodoStatus;
}
