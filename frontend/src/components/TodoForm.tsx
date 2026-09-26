import { useState, type FormEvent } from 'react';

interface TodoFormProps {
  onSubmit: (title: string, description: string) => Promise<boolean>;
}

export function TodoForm({ onSubmit }: TodoFormProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    // The fields are only cleared on success, so a rejected title can be corrected in place.
    const created = await onSubmit(title, description);
    if (created) {
      setTitle('');
      setDescription('');
    }
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'grid', gap: 8, marginBottom: 16 }}>
      <input placeholder="Título" value={title} onChange={(e) => setTitle(e.target.value)} />
      <textarea
        placeholder="Descrição"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
      />
      <button type="submit">Adicionar</button>
    </form>
  );
}
