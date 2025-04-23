import mongoose, { Schema, Document } from 'mongoose';

// Define the Habit document interface
interface IHabit extends Document {
  userId: string;
  id: string;
  habitName: string;
  type: string;
  value: string;
}

// Define the Habit schema
const habitSchema = new Schema<IHabit>({
  userId: { type: String, required: true },
  id: { type: String, required: true },
  habitName: { type: String, required: true },
  type: { type: String, required: true },
  value: { type: String, default: '' },
});

// Create and export the Habit model
export const Habit = mongoose.models.Habit || mongoose.model<IHabit>('Habit', habitSchema);
