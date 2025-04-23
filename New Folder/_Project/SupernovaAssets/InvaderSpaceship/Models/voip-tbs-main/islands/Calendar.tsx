import { useState } from 'preact/hooks';
import { ICalendarEvent } from '$types/data.ts';

interface Props {
  events: ICalendarEvent[];
}

interface Task {
  text: string;
  importance: 'low' | 'medium' | 'high';
}

const getDaysInMonth = (year: number, month: number): Date[] => {
  const date = new Date(year, month, 1);
  const days = [];
  while (date.getMonth() === month) {
    days.push(new Date(date));
    date.setDate(date.getDate() + 1);
  }
  return days;
};

export default function Calendar(props: Props) {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [events, setEvents] = useState<ICalendarEvent[]>(props.events);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [draggedEvent, setDraggedEvent] = useState<{ date: string; index: number } | null>(null);
  const [modal, setModal] = useState<{ date: string; text: string; color: string; index?: number } | null>(null);

  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();
  const daysInMonth = getDaysInMonth(year, month);
  const startDay = new Date(year, month, 1).getDay();

  const changeMonth = (offset: number) => {
    setCurrentDate(new Date(year, month + offset, 1));
  };

  const addEvent = (date: string) => {
    setModal({ date, text: '', color: 'bg-blue-500' });
  };

  const saveEvent = () => {
    if (!modal) return;
    const { date, text, color, index } = modal;

    if (text.trim()) {
      setEvents((prev) => ({
        ...prev,
        [date]: index !== undefined
          ? prev[date].map((e, i) => (i === index ? { text, color } : e))
          : [...(prev[date] || []), { text, color }],
      }));
    } else if (index !== undefined) {
      setEvents((prev) => {
        const updatedEvents = [...prev[date]];
        updatedEvents.splice(index, 1);
        return { ...prev, [date]: updatedEvents };
      });
    }
    setModal(null);
  };

  const handleCalendarDrop = (targetDate: string) => {
    if (draggedEvent) {
      const { date, index } = draggedEvent;
      const eventToMove = events[date][index];
      
      setEvents((prev) => {
        const updatedSourceEvents = [...prev[date]];
        updatedSourceEvents.splice(index, 1);

        return {
          ...prev,
          [date]: updatedSourceEvents,
          [targetDate]: [...(prev[targetDate] || []), eventToMove],
        };
      });
      setDraggedEvent(null);
    }
  };

  return (
    <div class="p-6 bg-darkest text-white flex flex-col justify-between">
      <div class="flex justify-between items-center mb-4">
        <button onClick={() => changeMonth(-1)} class="bg-darker px-4 py-2 rounded">&#8592; Prev</button>
        <h2 class="text-2xl font-bold">{currentDate.toLocaleString('default', { month: 'long', year: 'numeric' })}</h2>
        <button onClick={() => changeMonth(1)} class="bg-darker px-4 py-2 rounded">Next &#8594;</button>
      </div>

      <div class="grid grid-cols-7 sm:gap-0 md:gap-1 gap-2 p-4 bg-darker shadow-lg rounded-lg">
        {Array.from({ length: startDay }).map((_, i) => (
          <div key={`empty-${i}`} class="p-4"></div>
        ))}

        {daysInMonth.map((date) => {
          const formattedDate = date.toISOString().split('T')[0];
          return (
            <div
              key={formattedDate}
              class="border-2 border-darkest p-4 rounded-lg shadow-md bg-darker flex flex-col items-center relative cursor-pointer"
              onClick={() => addEvent(formattedDate)}
              onDragOver={(e) => e.preventDefault()}
              onDrop={() => handleCalendarDrop(formattedDate)}
            >
              <h3 class="text-lg font-bold mb-2">{date.getDate()}</h3>
              <div class="w-full">
                {events[formattedDate]?.map((event, index) => (
                  <div
                    key={index}
                    class={`${event.color} text-white text-xs px-2 py-1 rounded mb-1 cursor-pointer`}
                    draggable
                    onDragStart={() => setDraggedEvent({ date: formattedDate, index })}
                    onClick={(e) => {
                      e.stopPropagation();
                      setModal({ date: formattedDate, text: event.text, color: event.color, index });
                    }}
                  >
                    {event.text}
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>

      {modal && (
        <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div class="bg-darker p-4 rounded-lg">
            <h3 class="text-lg font-bold">{modal.index !== undefined ? 'Edit Event' : 'New Event'}</h3>
            <input
              type="text"
              class="w-full p-2 border border-dark bg-darker text-white rounded my-2"
              value={modal.text}
              onInput={(e) => setModal((prev) => prev && { ...prev, text: (e.target as HTMLInputElement).value })}
            />
            <select
              class="w-full p-2 border border-dark bg-darker text-white rounded my-2"
              value={modal.color}
              onChange={(e) => setModal((prev) => prev && { ...prev, color: (e.target as HTMLInputElement).value })}
            >
              <option value="bg-blue-500">Blue</option>
              <option value="bg-green-500">Green</option>
              <option value="bg-red-500">Red</option>
              <option value="bg-yellow-500">Yellow</option>
            </select>
            <div class="flex justify-end space-x-2">
              {modal.index !== undefined && (
                <button class="bg-red-500 px-4 py-2 rounded" onClick={() => setModal((prev) => prev && { ...prev, text: '' })}>
                  Delete
                </button>
              )}
              <button class="bg-dark px-4 py-2 rounded" onClick={saveEvent}>
                Save
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
