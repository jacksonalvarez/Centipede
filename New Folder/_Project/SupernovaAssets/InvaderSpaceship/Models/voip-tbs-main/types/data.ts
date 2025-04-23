export type Optional<T, K extends keyof T> = Pick<Partial<T>, K> & Omit<T, K>;

export interface IUser {
  id: number;
  email: string;
  username: string;
  password: string;
  uuid: string;
  verified: boolean;
  creation_timestamp: number;
  subscribed: boolean;
}

export interface ISession {
  id: number;
  token: string;
  user_id: number;
  uuid: string;
  unix_timestamp: number;
}

export interface IPrompt {
  id: number;
  input: string;
  output: string;
  user_id: number;
}

export interface IMessage {
  id: number;
  user_id: number;
  message: string;
  number_to: string;
  number_from: string;
  unix_timestamp: number;
}

export interface IContact {
  id: number;
  user_id: number;
  name: string;
  number: string;
}

export interface ITwilioApiKey {
  id: number;
  user_id: number;
  key: string;
  number: string;
  auth_token: string;
  sid: string;
}

export interface IAgent {
  id: number;
  user_id: number;
  agent_name: string;
  business_name: string;
  personality: string;
  directives: string;
  fallback_contact?: string;
  fallback_number?: string;
}

export interface ICalendarEvent { 
  id: number;
  user_id: number;
  date: string; // YYYY-MM-DD
  time: string; // HH:MM
  header: string;
  description: string;
  color: string;
  isRR: number;  // To indicate if it's a recurring event
  freq?: string;  // Frequency of recurrence (e.g., DAILY, WEEKLY) (optional)
  until_date?: string;  // Until date for recurring events (optional)
  attendees?: string[];  // List of attendees (optional)
  rrule?: { freq: string, until: string } | null; // Recurrence rule (optional)
}

export interface ICalendarDate {
  id: number;
  date: string;
  day_of_week: string;
  month: string;
  year: string;
  federal_holiday: boolean;
  holiday_name?: string;
  weekend: boolean;
}

export interface ICalendarRange {
  id: number;
  user_id: number;
  start_date: string;
  end_date: string;
  recurrence_rule: string;
  min_recurrence_date: string;
  max_recurrence_date: string;
}