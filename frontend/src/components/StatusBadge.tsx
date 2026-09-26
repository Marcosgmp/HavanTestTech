import { STATUS_COLORS, STATUS_LABELS } from '../constants/todoStatus';
import type { TodoStatus } from '../types';

export function StatusBadge({ status }: { status: TodoStatus }) {
  return (
    <span
      style={{
        marginLeft: 8,
        padding: '2px 8px',
        borderRadius: 12,
        color: '#fff',
        fontSize: 12,
        background: STATUS_COLORS[status],
      }}
    >
      {STATUS_LABELS[status]}
    </span>
  );
}
