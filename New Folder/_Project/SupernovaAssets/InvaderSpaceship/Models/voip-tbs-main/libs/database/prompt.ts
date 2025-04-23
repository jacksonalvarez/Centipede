import { Database } from "sqlite";
import { IPrompt } from "$types/data.ts";

export class PromptHandler  {
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS prompts (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          input TEXT NOT NULL,
          output TEXT NOT NULL,
          user_id INTEGER NOT NULL
        );
      `,
    ).run();
  }

  public insertPrompt(prompt: IPrompt) {
    this.db.prepare(
      `
        INSERT INTO prompts (input, output, user_id)
        VALUES (?, ?, ?);
      `,
    ).run(
      prompt.input,
      prompt.output,
      prompt.user_id,
    );
  }

  public updatePrompt(prompt: IPrompt) {
    this.db.prepare(
      `
        UPDATE prompts
        SET input = ?, output = ?
        WHERE id = ?;
      `,
    ).run(
      prompt.input,
      prompt.output,
      prompt.id,
    );
  }

  public deletePrompt(id: number) {
    this.db.prepare(
      `
        DELETE FROM prompts
        WHERE id = ?;
      `,
    ).run(id);
  }

  public getPrompt(id: number) {
    return this.db.prepare(
      `
        SELECT * FROM prompts
        WHERE id = ?;
      `,
    ).get(id) as IPrompt;
  }

  public getPrompts(user_id: number) {
    return this.db.prepare(
      `
        SELECT * FROM prompts
        WHERE user_id = ?;
      `,
    ).all(user_id) as IPrompt[];
  }

  public getLastRowID() {
    return this.db.lastInsertRowId;
  }
}
