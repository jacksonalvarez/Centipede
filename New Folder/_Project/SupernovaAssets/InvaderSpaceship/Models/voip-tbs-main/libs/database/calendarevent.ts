import { Database } from "sqlite";
import { ICalendarEvent } from "$types/data.ts";

export class CalendarEventHandler {
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS calendar_events (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          user_id INTEGER NOT NULL,
          date TEXT NOT NULL,
          time TEXT NOT NULL,
          header TEXT NOT NULL,
          description TEXT NOT NULL,
          color TEXT NOT NULL,
          isRR INTEGER NOT NULL DEFAULT 0,  -- To indicate if it's a recurring event
          until_date TEXT,  -- Until date for recurring events
          freq TEXT,  -- Frequency of recurrence (e.g., DAILY, WEEKLY)
          attendees TEXT,  -- Comma-separated list of attendees
          rrule TEXT  -- Recurrence rule as JSON string
        );
      `,
    ).run();
  }

  public insertCalendarEvent(event: ICalendarEvent) {
    this.db.prepare(
      `
        INSERT INTO calendar_events (
          user_id, date, time, header, description, color, isRR, until_date, freq, attendees, rrule
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
      `,
    ).run(
      event.user_id,
      event.date,
      event.time,
      event.header,
      event.description,
      event.color,
      event.isRR,
      event.until_date,
      event.freq,
      event.attendees ? event.attendees.join(",") : "", // Save attendees as a comma-separated string
      event.rrule ? JSON.stringify(event.rrule) : null // Save rrule as JSON string
    );
  }

  public updateCalendarEvent(event: ICalendarEvent) {
    this.db.prepare(
      `
        UPDATE calendar_events
        SET date = ?, time = ?, header = ?, description = ?, color = ?, isRR = ?, until_date = ?, freq = ?, attendees = ?, rrule = ?
        WHERE id = ?;
      `,
    ).run(
      event.date,
      event.time,
      event.header,
      event.description,
      event.color,
      event.isRR,
      event.until_date,
      event.freq,
      event.attendees ? event.attendees.join(",") : "",
      event.rrule ? JSON.stringify(event.rrule) : null,
      event.id,
    );
  }

  public deleteCalendarEvent(
    event: Pick<ICalendarEvent, "user_id" | "date" | "time">,
  ) {
    this.db.prepare(
      `
        DELETE FROM calendar_events
        WHERE user_id = ? AND date = ? AND time = ?;
      `,
    ).run(event.user_id, event.date, event.time);
  }

  public getCalendarEvent(id: number) {
    const event = this.db.prepare(
      `
        SELECT * FROM calendar_events
        WHERE id = ?;
      `,
    ).get(id) as ICalendarEvent;
    if (event.rrule) {
      event.rrule = JSON.parse(event.rrule as unknown as string);
    }
    return event;
  }

  public getCalendarEvents(user_id: number) {
    const events = this.db.prepare(
      `
        SELECT * FROM calendar_events
        WHERE user_id = ?;
      `,
    ).all(user_id) as ICalendarEvent[];
    events.forEach(event => {
      if (event.rrule) {
        event.rrule = JSON.parse(event.rrule as unknown as string);
      }
    });
    return events;
  }

  public getRecurringEvents(user_id: number) {
    const events = this.db.prepare(
      `
        SELECT * FROM calendar_events
        WHERE user_id = ? AND isRR = 1;
      `,
    ).all(user_id) as ICalendarEvent[];
    events.forEach(event => {
      if (event.rrule) {
        event.rrule = JSON.parse(event.rrule as unknown as string);
      }
    });
    return events;
  }
}
