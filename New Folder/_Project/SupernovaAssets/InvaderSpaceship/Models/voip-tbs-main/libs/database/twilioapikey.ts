import { Database } from "sqlite";
import { ITwilioApiKey } from "$types/data.ts";

export class TwilioApiKeyHandler  {
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS twilio_keys (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          key TEXT NOT NULL,
          number TEXT NOT NULL,
          auth_token TEXT NOT NULL,
          sid TEXT NOT NULL,
          user_id INTEGER NOT NULL
        );
      `,
    ).run();
  }

  public insertTwilioKey(api_key: Omit<ITwilioApiKey, "id">) {
    this.db.prepare(
      `
        INSERT INTO twilio_keys (key, number, auth_token, sid, user_id)
        VALUES (?, ?, ?, ?, ?);
      `,
    ).run(
      api_key.key,
      api_key.number,
      api_key.auth_token,
      api_key.sid,
      api_key.user_id,
    );
  }

  public deleteTwilioKey(id: number) {
    this.db.prepare(
      `
        DELETE FROM twilio_keys
        WHERE id = ?;
      `,
    ).run(id);
  }

  public deleteTwilioKeyByUUID(user_id: number) {
    this.db.prepare(
      `
        DELETE FROM twilio_keys
        WHERE user_id = ?;
      `,
    ).run(user_id);
  }

  public getTwilioKey(key: string) {
    return this.db.prepare(
      `
        SELECT * FROM twilio_keys
        WHERE key = ?;
      `,
    ).get(key) as ITwilioApiKey;
  }

  public getTwilioKeyByUserID(user_id: number) {
    return this.db.prepare(
      `
        SELECT * FROM twilio_keys
        WHERE user_id = ?;
      `,
    ).get(user_id) as ITwilioApiKey;
  }
}
