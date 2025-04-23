import { Database } from "sqlite";
import { ISession } from "$types/data.ts";

export class SessionHandler  {
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS sessions (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          token TEXT NOT NULL,
          user_id INTEGER NOT NULL,
          uuid TEXT NOT NULL,
          unix_timestamp INTEGER NOT NULL
        );
      `
    ).run();
  }

  public insertSession(session: Omit<ISession, "id" | "unix_timestamp">) {
    this.db.prepare(
      `
        INSERT INTO sessions (token, user_id, uuid, unix_timestamp)
        VALUES (?, ?, ?, unixepoch());
      `,
    ).run(
      session.token,
      session.user_id,
      session.uuid,
    );
  }

  public deleteSession(token: string) {
    this.db.prepare(
      `
        DELETE FROM sessions
        WHERE token = ?;
      `,
    ).run(token);
  }

  public getSession(token: string) {
    return this.db.prepare(
      `
        SELECT * FROM sessions
        WHERE token = ?;
      `,
    ).get(token) as ISession;
  }
}
