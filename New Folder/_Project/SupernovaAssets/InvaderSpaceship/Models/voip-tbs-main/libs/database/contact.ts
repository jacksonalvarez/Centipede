import { Database } from "sqlite";
import { IContact } from "$types/data.ts";

export class ContactHandler  {
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS contacts (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          user_id INTEGER NOT NULL,
          name TEXT NOT NULL,
          number TEXT NOT NULL
        );
      `,
    ).run();
  }

  public insertContact(contact: Omit<IContact, "id" | "name">) {
    this.db.prepare(
      `
        INSERT INTO contacts (user_id, number)
        VALUES (?, ?);
      `,
    ).run(contact.user_id, contact.number);
  }

  public updateContact(contact: IContact) {
    this.db.prepare(
      `
        UPDATE contacts
        SET name = ?, number = ?
        WHERE id = ?;
      `,
    ).run(contact.name, contact.number, contact.id);
  }

  public deleteContact(id: number) {
    this.db.prepare(
      `
        DELETE FROM contacts
        WHERE id = ?;
      `,
    ).run(id);
  }

  public getContact(id: number) {
    return this.db.prepare(
      `
        SELECT * FROM contacts
        WHERE id = ?;
      `,
    ).get(id) as IContact;
  }

  public getContacts(user_id: number) {
    return this.db.prepare(
      `
        SELECT * FROM contacts
        WHERE user_id = ?;
      `,
    ).all(user_id) as IContact[];
  }

  public getContactByNumber(user_id: number, number: string) {
    return this.db.prepare(
      `
        SELECT * FROM contacts
        WHERE user_id = ? AND number = ?;
      `,
    ).get(user_id, number) as IContact;
  }
}
