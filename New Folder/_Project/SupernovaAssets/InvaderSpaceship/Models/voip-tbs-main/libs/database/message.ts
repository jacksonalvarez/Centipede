import { Database } from "sqlite";
import { IMessage } from "$types/data.ts";

export class MessageHandler  {
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS messages (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          user_id INTEGER NOT NULL,
          message TEXT NOT NULL,
          number_to TEXT NOT NULL,
          number_from TEXT NOT NULL,
          unix_timestamp INTEGER NOT NULL
        );
      `,
    ).run();
  }

  public insertMessage(message: Omit<IMessage, "id" | "unix_timestamp">) {
    this.db.prepare(
      `
        INSERT INTO messages (user_id, message, number_to, number_from, unix_timestamp)
        VALUES (?, ?, ?, ?, unixepoch());
      `,
    ).run(
      message.user_id,
      message.message,
      message.number_to,
      message.number_from,
    );
  }

  public deleteMessage(id: number) {
    this.db.prepare(
      `
        DELETE FROM messages
        WHERE id = ?;
      `,
    ).run(id);
  }

  public deleteMessages(
    user_id: number,
    number_to: string,
    number_from: string,
  ) {
    this.db.prepare(
      `
        DELETE FROM messages
        WHERE user_id = ? AND (number_to = ? AND number_from = ?) OR (number_to = ? AND number_from = ?);
      `,
    ).run(user_id, number_to, number_from, number_from, number_to);
  }

  public archiveMessage(id: number) {
    const message = this.db.prepare(
      `
        SELECT * FROM messages
        WHERE id = ?;
      `,
    ).get(id) as IMessage;

    this.db.prepare(
      `
        INSERT INTO message_archives (user_id, message, number_to, number_from, unix_timestamp)
        VALUES (?, ?, ?, ?, unixepoch());
      `,
    ).run(
      message.user_id,
      message.message,
      message.number_to,
      message.number_from,
    );

    this.deleteMessage(id);
  }

  public archiveMessages(
    user_id: number,
    number_to: string,
    number_from: string,
  ) {
    const messages = this.db.prepare(
      `
        SELECT * FROM messages
        WHERE user_id = ? AND number_to = ? AND number_from = ?;
      `,
    ).all(user_id, number_to, number_from) as IMessage[];

    messages.forEach((message) => {
      this.db.prepare(
        `
          INSERT INTO message_archives (user_id, message, number_to, number_from, unix_timestamp)
          VALUES (?, ?, ?, ?, unixepoch());
        `,
      ).run(
        message.user_id,
        message.message,
        message.number_to,
        message.number_from,
      );

      this.deleteMessage(message.id);
    });
  }

  public getMessage(id: number) {
    return this.db.prepare(
      `
        SELECT * FROM messages
        WHERE id = ?;
      `,
    ).get(id) as IMessage;
  }

  public getMessages(user_id: number) {
    return this.db.prepare(
      `
        SELECT * FROM messages
        WHERE user_id = ?;
      `,
    ).all(user_id) as IMessage[];
  }

  public getMessagesByNumbers(
    user_id: number,
    number_to: string,
    number_from: string,
  ) {
    return this.db.prepare(
      `
        SELECT * FROM messages
        WHERE user_id = ? AND (number_to = ? AND number_from = ?) OR (number_to = ? AND number_from = ?);
      `,
    ).all(
      user_id,
      number_to,
      number_from,
      number_from,
      number_to,
    ) as IMessage[];
  }
}
