import { Database } from "sqlite";
import { IUser } from "$types/data.ts";

export class UserHandler {
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS users (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          email TEXT NOT NULL UNIQUE,
          username TEXT NOT NULL UNIQUE,
          password TEXT NOT NULL,
          uuid TEXT NOT NULL UNIQUE,
          verified BOOLEAN NOT NULL,
          creation_timestamp INTEGER NOT NULL,
          subscribed BOOLEAN NOT NULL
        );
      `,
    ).run();
  }

  public insertUser(user: Omit<IUser, "id" | "creation_timestamp">) {
    this.db.prepare(
      `
        INSERT INTO users (email, username, password, uuid, verified, creation_timestamp, subscribed)
        VALUES (?, ?, ?, ?, ?, unixepoch(), ?);
      `,
    ).run(
      user.email,
      user.username,
      user.password,
      user.uuid,
      user.verified,
      user.subscribed
    );
  }

  public updateUser(user: IUser) {
    this.db.prepare(
      `
        UPDATE users
        SET email = ?, username = ?, password = ?, verified = ?, subscribed = ?
        WHERE id = ?;
      `,
    ).run(
      user.email,
      user.username,
      user.password,
      user.verified,
      user.subscribed,
      user.id
    );
  }

  public deleteUser(id: number) {
    this.db.prepare(
      `
        DELETE FROM users
        WHERE id = ?;
      `,
    ).run(id);
  }

  public getUser(id: number) {
    return this.db.prepare(
      `
        SELECT * FROM users
        WHERE id = ?;
      `,
    ).get(id) as IUser;
  }

  public getUserByUUID(uuid: string) {
    return this.db.prepare(
      `
        SELECT * FROM users
        WHERE uuid = ?;
      `
    ).get(uuid) as IUser;
  }

  public getUserByUsername(username: string) {
    return this.db.prepare(
      `
        SELECT * FROM users
        WHERE username = ?;
      `,
    ).get(username) as IUser;
  }

  public getUserByEmail(email: string) {
    return this.db.prepare(
      `
        SELECT * FROM users
        WHERE email = ?;
      `,
    ).get(email) as IUser;
  }
}