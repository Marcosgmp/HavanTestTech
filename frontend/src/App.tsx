import { TodoCard } from './components/TodoCard';
import { TodoForm } from './components/TodoForm';
import { useTodos } from './hooks/useTodos';

export default function App() {
  const { todos, error, createTodo, advanceTodo } = useTodos();

  return (
    <main style={{ maxWidth: 640, margin: '2rem auto', fontFamily: 'sans-serif' }}>
      <h1>Tarefas</h1>

      <TodoForm onSubmit={createTodo} />

      {error && (
        <p role="alert" style={{ color: '#b91c1c' }}>
          {error}
        </p>
      )}

      <ul style={{ listStyle: 'none', padding: 0, display: 'grid', gap: 8 }}>
        {todos.map((todo) => (
          <TodoCard key={todo.id} todo={todo} onAdvance={advanceTodo} />
        ))}
      </ul>
    </main>
  );
}
