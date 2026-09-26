import { NEXT_STATUS, STATUS_LABELS } from '../constants/todoStatus';
import type { Todo } from '../types';
import { StatusBadge } from './StatusBadge';

interface TodoCardProps {
  todo: Todo;
  onAdvance: (todo: Todo) => void;
}

export function TodoCard({ todo, onAdvance }: TodoCardProps) {
  const next = NEXT_STATUS[todo.status];

  return (
    <li style={{ border: '1px solid #ddd', borderRadius: 8, padding: 12 }}>
      <strong>{todo.title}</strong>
      <StatusBadge status={todo.status} />
      <p>{todo.description}</p>
      {next && <button onClick={() => onAdvance(todo)}>Mover para {STATUS_LABELS[next]}</button>}
    </li>
  );
}
