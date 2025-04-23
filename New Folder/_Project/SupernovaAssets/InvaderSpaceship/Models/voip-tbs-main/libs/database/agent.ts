import { Database } from "sqlite";
import { IAgent } from "$types/data.ts";

export class AgentHandler { 
  private db: Database;

  constructor(db: Database) {
    this.db = db;
    this.init();
  }

  private init() {
    this.db.prepare(
      `
        CREATE TABLE IF NOT EXISTS agents (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          user_id INTEGER NOT NULL,
          agent_name TEXT NOT NULL,
          business_name TEXT NOT NULL,
          personality TEXT NOT NULL,
          directives TEXT NOT NULL,
          fallback_contact TEXT NOT NULL,
          fallback_number TEXT NOT NULL
        );
      `,
    ).run();
  }

  public insertAgent(agent: Omit<IAgent, "id">) {
    this.db.prepare(
      `
        INSERT INTO agents (user_id, agent_name, business_name, personality, directives, fallback_contact, fallback_number)
        VALUES (?, ?, ?, ?, ?, ?, ?);
      `,
    ).run(
      agent.user_id,
      agent.agent_name,
      agent.business_name,
      agent.personality,
      agent.directives,
      agent.fallback_contact,
      agent.fallback_number,
    );
  }

  public updateAgent(agent: IAgent) {
    this.db.prepare(
      `
        UPDATE agents
        SET agent_name = ?, business_name = ?, personality = ?, directives = ?, fallback_contact = ?, fallback_number = ?
        WHERE user_id = ?;
      `,
    ).run(
      agent.agent_name,
      agent.business_name,
      agent.personality,
      agent.directives,
      agent.fallback_contact,
      agent.fallback_number,
      agent.user_id,
    );
  }

  public deleteAgent(id: number) {
    this.db.prepare(
      `
        DELETE FROM agents
        WHERE user_id = ?;
      `,
    ).run(id);
  }

  public getAgent(id: number) {
    return this.db.prepare(
      `
        SELECT * FROM agents
        WHERE user_id = ?;
      `,
    ).get(id) as IAgent;
  }
}
