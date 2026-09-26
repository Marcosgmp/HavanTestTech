import { useCallback, useEffect, useState } from 'react';
import { todoApi } from '../api/todoApi';
import { NEXT_STATUS } from '../constants/todoStatus';
import type { Todo } from '../types';

function toMessage(error: unknown): string {
  return error instanceof Error ? error.message : 'Erro inesperado.';
}

export function useTodos() {
  const [todos, setTodos] = useState<Todo[]>([]);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    try {
      setTodos(await todoApi.list());
      setError(null);
    } catch (e) {
      setError(toMessage(e));
    }
  }, []);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  // Returns whether the task was created, so the form knows if it can clear its fields.
  const createTodo = async (title: string, description: string): Promise<boolean> => {
    try {
      await todoApi.create(title, description);
      await refresh();
      return true;
    } catch (e) {
      setError(toMessage(e));
      return false;
    }
  };

  const advanceTodo = async (todo: Todo) => {
    const next = NEXT_STATUS[todo.status];
    if (!next) return;

    try {
      await todoApi.changeStatus(todo.id, next);
      await refresh();
    } catch (e) {
      setError(toMessage(e));
    }
  };

  return { todos, error, createTodo, advanceTodo };
}
