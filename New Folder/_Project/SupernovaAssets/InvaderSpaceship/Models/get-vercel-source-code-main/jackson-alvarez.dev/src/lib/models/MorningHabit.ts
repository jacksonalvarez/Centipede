import mongoose from 'mongoose';

// Define the schema for MorningHabit
const MorningHabitSchema = new mongoose.Schema({
  userId: { type: String, required: true },
  habitName: { type: String, required: true },
  type: { type: String, required: true, enum: ['boolean', 'string', 'list'] },
  value: { type: mongoose.Schema.Types.Mixed, required: true },
});

// Use existing model if it exists, otherwise define it
const MorningHabit =
  mongoose.models.MorningHabit || mongoose.model('MorningHabit', MorningHabitSchema);

export default MorningHabit;
