import { useState } from 'preact/hooks';

interface Task {
  text: string;
  importance: 'low' | 'medium' | 'high';
  completed: boolean;
}

export default function TaskBar() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [draggedTask, setDraggedTask] = useState<number | null>(null);

  const addTask = (text: string, importance: 'low' | 'medium' | 'high') => {
    setTasks((prev) => [...prev, { text, importance, completed: false }]);
  };

  const toggleTask = (index: number) => {
    setTasks((prev) =>
      prev.map((task, i) => (i === index ? { ...task, completed: !task.completed } : task))
    );
  };

  const handleDragStart = (index: number) => {
    setDraggedTask(index);
  };

  const handleDrop = (index: number) => {
    if (draggedTask === null) return;
    const newTasks = [...tasks];
    const [movedTask] = newTasks.splice(draggedTask, 1);
    newTasks.splice(index, 0, movedTask);
    setTasks(newTasks);
    setDraggedTask(null);
  };

  return (
    <div class="p-4 bg-darker text-white rounded-lg">
      <h3 class="text-lg font-bold mb-2">Tasks</h3>
      <ul>
        {tasks.map((task, index) => (
          <li
            key={index}
            class={`p-2 rounded mb-2 cursor-pointer ${task.completed ? 'line-through text-gray-400' : ''}`}
            draggable
            onDragStart={() => handleDragStart(index)}
            onDragOver={(e) => e.preventDefault()}
            onDrop={() => handleDrop(index)}
          >
            <input
              type="checkbox"
              checked={task.completed}
              onChange={() => toggleTask(index)}
              class="mr-2"
            />
            {task.text} ({task.importance})
          </li>
        ))}
      </ul>
      <button class="mt-2 p-2 bg-darker rounded" onClick={() => addTask('New Task', 'medium')}>
        Add Task
      </button>
    </div>
  );
}